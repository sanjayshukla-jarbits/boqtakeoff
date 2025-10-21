using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace boqtakeoff.core.Libraries
{
    /// <summary>
    /// Service for extracting and exporting family files from Revit projects
    /// </summary>
    public class FamilyExportService
    {
        private readonly Document _doc;
        private readonly string _exportPath;

        public FamilyExportService(Document doc, string exportPath)
        {
            _doc = doc;
            _exportPath = exportPath;

            // Create export directory if it doesn't exist
            if (!Directory.Exists(_exportPath))
            {
                Directory.CreateDirectory(_exportPath);
            }
        }

        /// <summary>
        /// Get all loadable families in the project
        /// </summary>
        public List<FamilyExportInfo> GetAllLoadableFamilies()
        {
            var familyList = new List<FamilyExportInfo>();

            // Get all families in the document
            var collector = new FilteredElementCollector(_doc)
                .OfClass(typeof(Family));

            foreach (Family family in collector)
            {
                // Skip system families (walls, floors, etc.) - only get loadable families
                if (!family.IsEditable)
                    continue;

                var familyInfo = new FamilyExportInfo
                {
                    Family = family,
                    FamilyName = family.Name,
                    Category = family.FamilyCategory?.Name ?? "Uncategorized",
                    SymbolCount = family.GetFamilySymbolIds().Count,
                    IsInPlace = family.IsInPlace
                };

                // Skip in-place families (they can't be saved as .rfa)
                if (!familyInfo.IsInPlace)
                {
                    familyList.Add(familyInfo);
                }
            }

            return familyList.OrderBy(f => f.Category).ThenBy(f => f.FamilyName).ToList();
        }

        /// <summary>
        /// Export a single family to .rfa file
        /// </summary>
        public bool ExportFamily(Family family, string destinationPath = null)
        {
            try
            {
                if (family.IsInPlace)
                {
                    throw new Exception("Cannot export in-place families");
                }

                // Use provided path or default export path
                string exportDir = destinationPath ?? _exportPath;

                // Create category subfolder
                string categoryFolder = Path.Combine(exportDir,
                    family.FamilyCategory?.Name ?? "Uncategorized");

                if (!Directory.Exists(categoryFolder))
                {
                    Directory.CreateDirectory(categoryFolder);
                }

                // Build file path
                string fileName = SanitizeFileName(family.Name) + ".rfa";
                string filePath = Path.Combine(categoryFolder, fileName);

                // Open family document in background
                Document familyDoc = _doc.EditFamily(family);

                if (familyDoc != null)
                {
                    // Save as .rfa file
                    SaveAsOptions saveOptions = new SaveAsOptions
                    {
                        OverwriteExistingFile = true
                    };

                    familyDoc.SaveAs(filePath, saveOptions);
                    familyDoc.Close(false);

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Utility.Logger(ex);
                return false;
            }
        }

        /// <summary>
        /// Export all loadable families from the project
        /// </summary>
        public FamilyExportResult ExportAllFamilies(IProgress<FamilyExportProgress> progress = null)
        {
            var result = new FamilyExportResult();
            var families = GetAllLoadableFamilies();

            result.TotalFamilies = families.Count;

            for (int i = 0; i < families.Count; i++)
            {
                var familyInfo = families[i];

                try
                {
                    // Report progress
                    progress?.Report(new FamilyExportProgress
                    {
                        CurrentIndex = i + 1,
                        TotalCount = families.Count,
                        CurrentFamilyName = familyInfo.FamilyName,
                        Status = "Exporting..."
                    });

                    bool success = ExportFamily(familyInfo.Family);

                    if (success)
                    {
                        result.SuccessCount++;
                        result.ExportedFamilies.Add(familyInfo.FamilyName);
                    }
                    else
                    {
                        result.FailedCount++;
                        result.FailedFamilies.Add(familyInfo.FamilyName, "Export failed");
                    }
                }
                catch (Exception ex)
                {
                    result.FailedCount++;
                    result.FailedFamilies.Add(familyInfo.FamilyName, ex.Message);
                    Utility.Logger(ex);
                }
            }

            return result;
        }

        /// <summary>
        /// Export selected families
        /// </summary>
        public FamilyExportResult ExportSelectedFamilies(
            List<FamilyExportInfo> selectedFamilies,
            IProgress<FamilyExportProgress> progress = null)
        {
            var result = new FamilyExportResult();
            result.TotalFamilies = selectedFamilies.Count;

            for (int i = 0; i < selectedFamilies.Count; i++)
            {
                var familyInfo = selectedFamilies[i];

                try
                {
                    progress?.Report(new FamilyExportProgress
                    {
                        CurrentIndex = i + 1,
                        TotalCount = selectedFamilies.Count,
                        CurrentFamilyName = familyInfo.FamilyName,
                        Status = "Exporting..."
                    });

                    bool success = ExportFamily(familyInfo.Family);

                    if (success)
                    {
                        result.SuccessCount++;
                        result.ExportedFamilies.Add(familyInfo.FamilyName);
                    }
                    else
                    {
                        result.FailedCount++;
                        result.FailedFamilies.Add(familyInfo.FamilyName, "Export failed");
                    }
                }
                catch (Exception ex)
                {
                    result.FailedCount++;
                    result.FailedFamilies.Add(familyInfo.FamilyName, ex.Message);
                    Utility.Logger(ex);
                }
            }

            return result;
        }

        /// <summary>
        /// Sanitize filename to remove invalid characters
        /// </summary>
        private string SanitizeFileName(string fileName)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            return string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));
        }

        /// <summary>
        /// Get family file info if it already exists
        /// </summary>
        public string GetExportedFilePath(Family family)
        {
            string categoryFolder = Path.Combine(_exportPath,
                family.FamilyCategory?.Name ?? "Uncategorized");
            string fileName = SanitizeFileName(family.Name) + ".rfa";
            string filePath = Path.Combine(categoryFolder, fileName);

            return File.Exists(filePath) ? filePath : null;
        }
    }

    /// <summary>
    /// Family export information
    /// </summary>
    public class FamilyExportInfo
    {
        public Family Family { get; set; }
        public string FamilyName { get; set; }
        public string Category { get; set; }
        public int SymbolCount { get; set; }
        public bool IsInPlace { get; set; }
        public bool IsSelected { get; set; }
    }

    /// <summary>
    /// Progress information for family export
    /// </summary>
    public class FamilyExportProgress
    {
        public int CurrentIndex { get; set; }
        public int TotalCount { get; set; }
        public string CurrentFamilyName { get; set; }
        public string Status { get; set; }
        public int PercentComplete => (int)((double)CurrentIndex / TotalCount * 100);
    }

    /// <summary>
    /// Result of family export operation
    /// </summary>
    public class FamilyExportResult
    {
        public int TotalFamilies { get; set; }
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
        public List<string> ExportedFamilies { get; set; } = new List<string>();
        public Dictionary<string, string> FailedFamilies { get; set; } = new Dictionary<string, string>();

        public string GetSummary()
        {
            return $"Exported: {SuccessCount}/{TotalFamilies}\nFailed: {FailedCount}";
        }
    }
}