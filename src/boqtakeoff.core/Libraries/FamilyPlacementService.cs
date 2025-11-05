using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace boqtakeoff.core.Libraries
{
    /// <summary>
    /// Service for loading and placing Revit families in the project
    /// </summary>
    public class FamilyPlacementService
    {
        private readonly Document _doc;
        private readonly UIDocument _uidoc;
        private readonly S3FamilyLibraryService _s3Service;

        public FamilyPlacementService(
            Document doc, 
            UIDocument uidoc, 
            S3FamilyLibraryService s3Service)
        {
            _doc = doc;
            _uidoc = uidoc;
            _s3Service = s3Service;
        }

        /// <summary>
        /// Load family from S3 and place at user-selected location
        /// NOTE: This method must be called from the UI thread that has Revit API context.
        /// Do NOT use ConfigureAwait(false) here because Revit API calls must run on the original thread.
        /// </summary>
        /// <param name="familyMetadata">Family metadata from S3</param>
        /// <returns>True if successful</returns>
        public async Task<bool> LoadAndPlaceFamilyAsync(FamilyMetadata familyMetadata)
        {
            string tempFilePath = null;

            try
            {
                // 1. Download family from S3
                // IMPORTANT: Do NOT use ConfigureAwait(false) here because we need to continue
                // on the UI thread for Revit API operations below
                tempFilePath = await _s3Service.DownloadFamilyAsync(familyMetadata.S3Key);

                if (!File.Exists(tempFilePath))
                {
                    throw new Exception("Failed to download family file");
                }

                // 2. Validate it's a Revit family file
                if (!IsValidRevitFamily(tempFilePath))
                {
                    throw new Exception("Invalid Revit family file");
                }

                // 3. Get user click location (must be called from Revit API context)
                // Note: GetUserClickLocation and LoadAndPlaceFamily must run synchronously
                // on the UI thread that has access to Revit API
                XYZ placementPoint = GetUserClickLocation();
                
                if (placementPoint == null)
                {
                    // User cancelled
                    return false;
                }

                // 4. Load and place family (must be called from Revit API context)
                bool success = LoadAndPlaceFamily(tempFilePath, placementPoint);

                return success;
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                // User cancelled the operation
                return false;
            }
            catch (Exception ex)
            {
                Utility.Logger(ex);
                TaskDialog.Show("Error", $"Failed to load and place family: {ex.Message}");
                return false;
            }
            finally
            {
                // Clean up temp file
                if (!string.IsNullOrEmpty(tempFilePath) && File.Exists(tempFilePath))
                {
                    try
                    {
                        File.Delete(tempFilePath);
                    }
                    catch
                    {
                        // Ignore cleanup errors
                    }
                }
            }
        }

        /// <summary>
        /// Load family and place instance at specified location
        /// NOTE: This method must be called from the Revit API context (UI thread)
        /// </summary>
        private bool LoadAndPlaceFamily(string familyPath, XYZ location)
        {
            // Ensure we're on the correct thread for Revit API operations
            // Check if document is valid and accessible
            if (_doc == null || _doc.IsValidObject == false)
            {
                throw new InvalidOperationException("Document is not valid or accessible");
            }

            // Ensure we're on the UI thread - Transaction.Start() requires Revit API context
            // This check helps debug if called from wrong thread, but can't prevent the error
            // The actual fix is ensuring this method is only called from UI thread context
            
            using (Transaction trans = new Transaction(_doc, "Load and Place Family"))
            {
                trans.Start();

                try
                {
                    // Load family into project
                    Family family;
                    bool familyLoaded = _doc.LoadFamily(
                        familyPath, 
                        new CustomFamilyLoadOption(), 
                        out family);

                    if (!familyLoaded || family == null)
                    {
                        trans.RollBack();
                        TaskDialog.Show("Error", "Failed to load family into project");
                        return false;
                    }

                    // Get first family symbol
                    FamilySymbol symbol = GetFirstFamilySymbol(family);

                    if (symbol == null)
                    {
                        trans.RollBack();
                        TaskDialog.Show("Error", "No valid family symbol found");
                        return false;
                    }

                    // Activate symbol if not active
                    if (!symbol.IsActive)
                    {
                        symbol.Activate();
                    }

                    // Determine placement method based on family category
                    FamilyInstance instance = CreateFamilyInstance(symbol, location);

                    if (instance == null)
                    {
                        trans.RollBack();
                        TaskDialog.Show("Error", "Failed to create family instance");
                        return false;
                    }

                    trans.Commit();
                    
                    // Select the newly placed instance
                    _uidoc.Selection.SetElementIds(new[] { instance.Id }.ToList());

                    return true;
                }
                catch (Exception ex)
                {
                    trans.RollBack();
                    Utility.Logger(ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// Get user click location in the active view
        /// </summary>
        private XYZ GetUserClickLocation()
        {
            try
            {
                // Prompt user to click location
                XYZ point = _uidoc.Selection.PickPoint(
                    ObjectSnapTypes.Endpoints | 
                    ObjectSnapTypes.Midpoints | 
                    ObjectSnapTypes.Nearest | 
                    ObjectSnapTypes.WorkPlaneGrid,
                    "Click to place family instance (ESC to cancel)");

                return point;
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                // User cancelled
                return null;
            }
        }

        /// <summary>
        /// Get first available family symbol from family
        /// </summary>
        private FamilySymbol GetFirstFamilySymbol(Family family)
        {
            foreach (ElementId symbolId in family.GetFamilySymbolIds())
            {
                var symbol = _doc.GetElement(symbolId) as FamilySymbol;
                if (symbol != null)
                {
                    return symbol;
                }
            }
            return null;
        }

        /// <summary>
        /// Create family instance based on category
        /// </summary>
        private FamilyInstance CreateFamilyInstance(FamilySymbol symbol, XYZ location)
        {
            try
            {
                // Get the active view's level for host-based families
                Level level = _doc.GetElement(_doc.ActiveView.LevelId) as Level;

                // Check if family requires a host (wall, ceiling, floor, etc.)
                if (symbol.Family.FamilyPlacementType == FamilyPlacementType.OneLevelBased ||
                    symbol.Family.FamilyPlacementType == FamilyPlacementType.OneLevelBasedHosted ||
                    symbol.Family.FamilyPlacementType == FamilyPlacementType.TwoLevelsBased)
                {
                    // Level-based placement
                    return _doc.Create.NewFamilyInstance(
                        location,
                        symbol,
                        level,
                        Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                }
                else if (symbol.Family.FamilyPlacementType == FamilyPlacementType.WorkPlaneBased)
                {
                    // Work plane based placement
                    return _doc.Create.NewFamilyInstance(
                        location,
                        symbol,
                        _doc.ActiveView.SketchPlane,
                        Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                }
                else
                {
                    // Standard placement (most common)
                    return _doc.Create.NewFamilyInstance(
                        location,
                        symbol,
                        Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                }
            }
            catch (Exception ex)
            {
                Utility.Logger(ex);
                throw new Exception($"Failed to create family instance: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Validate if file is a Revit family
        /// </summary>
        private bool IsValidRevitFamily(string filePath)
        {
            if (!File.Exists(filePath))
                return false;

            if (!filePath.EndsWith(".rfa", StringComparison.OrdinalIgnoreCase))
                return false;

            // Additional validation: check file size
            var fileInfo = new FileInfo(filePath);
            if (fileInfo.Length == 0)
                return false;

            return true;
        }

        /// <summary>
        /// Load family without placing (for pre-loading)
        /// </summary>
        public async Task<Family> LoadFamilyOnlyAsync(FamilyMetadata familyMetadata)
        {
            string tempFilePath = null;

            try
            {
                // Download family from S3
                tempFilePath = await _s3Service.DownloadFamilyAsync(familyMetadata.S3Key);

                using (Transaction trans = new Transaction(_doc, "Load Family"))
                {
                    trans.Start();

                    Family family;
                    bool loaded = _doc.LoadFamily(
                        tempFilePath, 
                        new CustomFamilyLoadOption(), 
                        out family);

                    if (loaded && family != null)
                    {
                        trans.Commit();
                        return family;
                    }
                    else
                    {
                        trans.RollBack();
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.Logger(ex);
                throw;
            }
            finally
            {
                // Clean up
                if (!string.IsNullOrEmpty(tempFilePath) && File.Exists(tempFilePath))
                {
                    try
                    {
                        File.Delete(tempFilePath);
                    }
                    catch { }
                }
            }
        }
    }
}