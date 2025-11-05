using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using boqtakeoff.core.Libraries;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace boqtakeoff.ui.Views
{
    /// <summary>
    /// External event handler for loading and placing families in Revit API context
    /// </summary>
    public class LoadFamilyEventHandler : IExternalEventHandler
    {
        private string _familyPath;
        private UIDocument _uidoc;
        private TaskCompletionSource<bool> _tcs;
        private Exception _exception;

        public void SetData(string familyPath, UIDocument uidoc, TaskCompletionSource<bool> tcs)
        {
            _familyPath = familyPath;
            _uidoc = uidoc;
            _tcs = tcs;
            _exception = null;
        }

        public void Execute(UIApplication app)
        {
            try
            {
                var doc = _uidoc.Document;
                bool success = false;

                using (Transaction trans = new Transaction(doc, "Load and Place Family"))
                {
                    trans.Start();

                    try
                    {
                        // Load family into project
                        Family family;
                        bool familyLoaded = doc.LoadFamily(
                            _familyPath,
                            new boqtakeoff.core.CustomFamilyLoadOption(),
                            out family);

                        if (!familyLoaded || family == null)
                        {
                            trans.RollBack();
                            TaskDialog.Show("Error", "Failed to load family into project");
                            _tcs?.SetResult(false);
                            return;
                        }

                        // Get first family symbol
                        FamilySymbol symbol = null;
                        foreach (ElementId symbolId in family.GetFamilySymbolIds())
                        {
                            symbol = doc.GetElement(symbolId) as FamilySymbol;
                            if (symbol != null)
                            {
                                break;
                            }
                        }

                        if (symbol == null)
                        {
                            trans.RollBack();
                            TaskDialog.Show("Error", "No valid family symbol found");
                            _tcs?.SetResult(false);
                            return;
                        }

                        // Activate symbol if not active
                        if (!symbol.IsActive)
                        {
                            symbol.Activate();
                        }

                        // Get user click location (must be on Revit API thread)
                        XYZ placementPoint = _uidoc.Selection.PickPoint(
                            ObjectSnapTypes.Endpoints |
                            ObjectSnapTypes.Midpoints |
                            ObjectSnapTypes.Nearest |
                            ObjectSnapTypes.WorkPlaneGrid,
                            "Click to place family instance (ESC to cancel)");

                        if (placementPoint == null)
                        {
                            trans.RollBack();
                            _tcs?.SetResult(false); // User cancelled
                            return;
                        }

                        // Determine placement method based on family category
                        FamilyInstance instance = null;
                        Level level = doc.GetElement(doc.ActiveView.LevelId) as Level;

                        if (symbol.Family.FamilyPlacementType == FamilyPlacementType.OneLevelBased ||
                            symbol.Family.FamilyPlacementType == FamilyPlacementType.OneLevelBasedHosted ||
                            symbol.Family.FamilyPlacementType == FamilyPlacementType.TwoLevelsBased)
                        {
                            instance = doc.Create.NewFamilyInstance(
                                placementPoint, symbol, level,
                                Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                        }
                        else if (symbol.Family.FamilyPlacementType == FamilyPlacementType.WorkPlaneBased)
                        {
                            instance = doc.Create.NewFamilyInstance(
                                placementPoint, symbol, doc.ActiveView.SketchPlane,
                                Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                        }
                        else
                        {
                            instance = doc.Create.NewFamilyInstance(
                                placementPoint, symbol,
                                Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                        }

                        if (instance == null)
                        {
                            trans.RollBack();
                            TaskDialog.Show("Error", "Failed to create family instance");
                            _tcs?.SetResult(false);
                            return;
                        }

                        trans.Commit();

                        // Select the newly placed instance
                        _uidoc.Selection.SetElementIds(new[] { instance.Id }.ToList());

                        success = true;
                    }
                    catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                    {
                        trans.RollBack();
                        // User cancelled - not an error
                        _tcs?.SetResult(false);
                        return;
                    }
                    catch (Exception ex)
                    {
                        trans.RollBack();
                        _exception = ex;
                        throw;
                    }
                }

                _tcs?.SetResult(success);
            }
            catch (Exception ex)
            {
                _exception = ex;
                Utility.Logger(ex);
                TaskDialog.Show("Error", $"Failed to load and place family: {ex.Message}");
                _tcs?.SetException(ex);
            }
        }

        public string GetName()
        {
            return "Load Family Event Handler";
        }
    }
}

