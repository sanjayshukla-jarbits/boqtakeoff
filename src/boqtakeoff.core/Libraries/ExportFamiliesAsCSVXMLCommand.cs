using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace boqtakeoff.core.Libraries
{
    /// <summary>
    /// Command to export families from Revit model to CSV and XML formats
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    internal class ExportFamiliesAsCSVXMLCommand : IExternalCommand
    {
        private Document _doc;

        /// <summary>
        /// Executes the export families to CSV/XML command
        /// </summary>
        /// <param name="commandData">The command data</param>
        /// <param name="message">Error message to return</param>
        /// <param name="elements">Elements to highlight on failure</param>
        /// <returns>Result indicating success or failure</returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                // Validate active document exists
                if (commandData?.Application?.ActiveUIDocument == null)
                {
                    message = "No active document found. Please open a Revit project.";
                    TaskDialog.Show("Error", message);
                    return Result.Failed;
                }

                _doc = commandData.Application.ActiveUIDocument.Document;

                // Validate document is a project (not a family document)
                if (_doc.IsFamilyDocument)
                {
                    message = "This command only works with project files (.rvt), not family files (.rfa).";
                    TaskDialog.Show("Error", message);
                    return Result.Failed;
                }

                // Prompt user to select export folder
                string selectedFolderPath = null;
                using (var folderDialog = new FolderBrowserDialog())
                {
                    folderDialog.Description = "Select folder to export CSV and XML files";
                    folderDialog.ShowNewFolderButton = true;

                    if (folderDialog.ShowDialog() != DialogResult.OK)
                    {
                        return Result.Cancelled;
                    }

                    selectedFolderPath = folderDialog.SelectedPath;
                }

                // Validate selected path
                if (string.IsNullOrWhiteSpace(selectedFolderPath))
                {
                    message = "No export folder selected.";
                    TaskDialog.Show("Error", message);
                    return Result.Failed;
                }

                // Ensure output directory exists
                try
                {
                    if (!Directory.Exists(selectedFolderPath))
                    {
                        Directory.CreateDirectory(selectedFolderPath);
                    }
                }
                catch (Exception ex)
                {
                    message = $"Failed to create export directory: {ex.Message}";
                    Utility.Logger(ex, $"Failed to create directory: {selectedFolderPath}", "error");
                    TaskDialog.Show("Error", message);
                    return Result.Failed;
                }

                // Set output paths
                string xmlOutputPath = Path.Combine(selectedFolderPath, "revitFolder.xml");
                string csvOutputPath = selectedFolderPath; // ExtractFamiliesToCSVAsync handles directory or file path

                // Show progress dialog
                // TaskDialog.Show("Export Families", "Starting export to CSV and XML...\n\nPlease wait...");

                // Run extraction synchronously (Revit API doesn't handle async well in commands)
                try
                {
                    // Extract to XML
                    ExtractFamiliesToXmlAsync(xmlOutputPath);

                    // Extract to CSV
                    ExtractFamiliesToCSVAsync(csvOutputPath);

                    // Show success message
                    string csvFilePath = Path.Combine(selectedFolderPath, "revit.csv");
                    TaskDialog.Show("Export Complete",
                        $"Export completed successfully!\n\n" +
                        $"✓ XML file saved to:\n{xmlOutputPath}\n\n" +
                        $"✓ CSV file saved to:\n{csvFilePath}\n\n" +
                        $"You can now verify the outputs.");

                    // return Result.Succeeded;
                }
                catch (AggregateException aggEx)
                {
                    // Unwrap aggregate exception
                    Exception innerEx = aggEx.InnerException ?? aggEx;
                    message = $"Export failed: {innerEx.Message}";
                    TaskDialog.Show("Error", message);
                    Utility.Logger(innerEx, message, "error");
                    return Result.Failed;
                }
                catch (Exception ex)
                {
                    message = $"Export failed: {ex.Message}";
                    TaskDialog.Show("Error", message);
                    Utility.Logger(ex, message, "error");
                    return Result.Failed;
                }
            }
            catch (Exception ex)
            {
                message = $"Command failed: {ex.Message}";
                Utility.Logger(ex, message, "error");
                TaskDialog.Show("Error", message);
                return Result.Failed;
            }
            return Result.Succeeded;
        }

        /// <summary>
        /// Collects all families from the Revit document
        /// </summary>
        private List<FamilyInfo> CollectAllFamilies()
        {
            var families = new List<FamilyInfo>();
            var collector = new FilteredElementCollector(_doc).OfClass(typeof(Family));

            foreach (Family family in collector)
            {
                // Skip system families (walls, floors, etc.) - only get loadable families
                if (!family.IsEditable || family.IsInPlace)
                    continue;

                var familyInfo = new FamilyInfo
                {
                    Family = family,
                    FamilyName = family.Name,
                    Category = family.FamilyCategory?.Name ?? "Uncategorized"
                };
                families.Add(familyInfo);
            }
            return families.OrderBy(f => f.Category).ThenBy(f => f.FamilyName).ToList();
        }

        /// <summary>
        /// Extracts all type parameters/properties from FamilySymbol objects
        /// </summary>
        private Dictionary<string, string> ExtractFamilyParameters(Family family)
        {
            var parameters = new Dictionary<string, string>();

            try
            {
                // Get all family symbols for this family
                var symbolIds = family.GetFamilySymbolIds();
                
                if (symbolIds.Count > 0)
                {
                    // Use the first symbol to extract parameters (they share the same parameters)
                    var firstSymbolId = symbolIds.First();
                    var symbol = _doc.GetElement(firstSymbolId) as FamilySymbol;

                    if (symbol != null)
                    {
                        foreach (Parameter param in symbol.Parameters)
                        {
                            if (param == null || string.IsNullOrWhiteSpace(param.Definition?.Name))
                                continue;

                            string paramName = param.Definition.Name;
                            string paramValue = GetParameterValueAsString(param);

                            // Only add non-empty values
                            if (!string.IsNullOrWhiteSpace(paramValue))
                            {
                                // Handle duplicate parameter names (shouldn't happen, but just in case)
                                if (!parameters.ContainsKey(paramName))
                                {
                                    parameters[paramName] = paramValue;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.Logger(ex);
            }

            return parameters;
        }

        /// <summary>
        /// Gets parameter value as string, handling different storage types
        /// </summary>
        private string GetParameterValueAsString(Parameter param)
        {
            if (param == null)
                return string.Empty;

            try
            {
                // First try AsValueString which formats values with units automatically
                if (param.HasValue)
                {
                    string valueString = param.AsValueString();
                    if (!string.IsNullOrWhiteSpace(valueString))
                    {
                        return valueString;
                    }
                }

                // Fallback to storage type-specific extraction
                StorageType storageType = param.StorageType;

                switch (storageType)
                {
                    case StorageType.String:
                        return param.AsString() ?? string.Empty;

                    case StorageType.Integer:
                        return param.AsInteger().ToString();

                    case StorageType.Double:
                        return param.AsDouble().ToString();

                    case StorageType.ElementId:
                        var elemId = param.AsElementId();
                        if (elemId != null && elemId != ElementId.InvalidElementId)
                        {
                            var element = _doc.GetElement(elemId);
                            return element?.Name ?? elemId.ToString();
                        }
                        return string.Empty;

                    case StorageType.None:
                        return string.Empty;

                    default:
                        return string.Empty;
                }
            }
            catch (Exception ex)
            {
                Utility.Logger(ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets a document property value
        /// </summary>
        private string GetDocumentProperty(string propertyName)
        {
            try
            {
                var projectInfo = _doc.ProjectInformation;
                if (projectInfo != null)
                {
                    // Try lookup parameter first
                    var param = projectInfo.LookupParameter(propertyName);
                    if (param != null && param.HasValue)
                    {
                        return param.AsString();
                    }

                    // Try built-in parameters
                    if (propertyName == "Author")
                    {
                        var authorParam = projectInfo.get_Parameter(BuiltInParameter.PROJECT_AUTHOR);
                        if (authorParam != null && authorParam.HasValue)
                        {
                            return authorParam.AsString();
                        }
                    }
                }
            }
            catch
            {
                // Return null if property not found
            }

            return null;
        }

        /// <summary>
        /// Escapes CSV values and creates a CSV line
        /// </summary>
        private string EscapeCsvLine(IEnumerable<string> values)
        {
            var escapedValues = values.Select(value =>
            {
                if (value == null)
                    return string.Empty;

                // If value contains comma, quote, or newline, wrap in quotes and escape quotes
                if (value.Contains(",") || value.Contains("\"") || value.Contains("\n") || value.Contains("\r"))
                {
                    return "\"" + value.Replace("\"", "\"\"") + "\"";
                }

                return value;
            });

            return string.Join(",", escapedValues);
        }

        /// <summary>
        /// Extracts families from Revit model and generates XML output matching revitFolder.xml format
        /// </summary>
        /// <param name="outputPath">Full path to output XML file</param>
        private void ExtractFamiliesToXmlAsync(string outputPath)
        {
            if (string.IsNullOrWhiteSpace(outputPath))
                throw new ArgumentException("Output path cannot be null or empty", nameof(outputPath));

            // Ensure output directory exists
            string outputDir = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrWhiteSpace(outputDir) && !Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            try
            {
                // Collect all families
                var families = CollectAllFamilies();

                // Group families by category
                var familiesByCategory = families.GroupBy(f => f.Category).ToList();

                // Create XML document structure
                var root = new XElement("RevitFamilyRepository");

                // SECTION 1: Master Category List
                var categoryList = new XElement("CategoryList");
                int categoryId = 1;
                foreach (var categoryGroup in familiesByCategory)
                {
                    var categoryElement = new XElement("Category",
                        new XAttribute("id", categoryId++),
                        new XAttribute("FullPath", $"/{categoryGroup.Key}"),
                        new XAttribute("FileCount", categoryGroup.Count()),
                        categoryGroup.Key
                    );
                    categoryList.Add(categoryElement);
                }
                root.Add(categoryList);

                // SECTION 2: Families grouped by Category
                var familiesByCategoryElement = new XElement("FamiliesByCategory");

                string tempFolder = Path.Combine(Path.GetTempPath(), "BIMLibrary", "TempFamily");
                // Create temp directory if it doesn't exist
                if (!Directory.Exists(tempFolder))
                {
                    Directory.CreateDirectory(tempFolder);
                }
                foreach (var categoryGroup in familiesByCategory)
                {
                    var categoryGroupElement = new XElement("CategoryGroup",
                        new XAttribute("name", categoryGroup.Key)
                    );

                    foreach (var familyInfo in categoryGroup)
                    {
                        // Extract parameters
                        var parameters = ExtractFamilyParameters(familyInfo.Family);

                        // Get document properties
                        string revitVersion = _doc.Application.VersionNumber ?? "2023";
                        string author = GetDocumentProperty("Author") ?? "BigFish BIM Sync";
                        // Use current date as last updated (document doesn't expose last saved date easily)
                        string lastUpdated = DateTime.Now.ToString("yyyy-MM-dd");

                        // Get file size (if family file is accessible)
                        long fileSize = 0;
                        try
                        {
                            // Create a temporary file path for this family
                            string exportPath = Path.Combine(tempFolder, familyInfo.FamilyName + ".rfa");
                            if (familyInfo.Family.IsEditable && !familyInfo.Family.IsInPlace)
                            {
                                // Try to get family document path
                                Document familyDoc = _doc.EditFamily(familyInfo.Family);
                                if (familyDoc != null)
                                {
                                    familyDoc.SaveAs(exportPath);
                                    familyDoc.Close(false);
                                    // Get file size
                                    FileInfo fileInfo = new FileInfo(exportPath);
                                    fileSize = fileInfo.Length;
                                    // Optionally delete the temp file after use
                                    File.Delete(exportPath);
                                }
                            }
                        }
                        catch
                        {
                            // If we can't get file size, use default
                            fileSize = 123456; // Default size similar to example XML
                        }

                        // Build file path
                        string filePath = $"{categoryGroup.Key}/{familyInfo.FamilyName}.rfa";

                        // Create Family element
                        var familyElement = new XElement("Family",
                            new XElement("FamilyName", familyInfo.FamilyName),
                            new XElement("Category", categoryGroup.Key),
                            new XElement("FilePath", filePath),
                            new XElement("FileSize", fileSize),
                            new XElement("RevitVersion", revitVersion),
                            new XElement("Author", author),
                            new XElement("LastUpdated", lastUpdated),
                            new XElement("Description", "Standard interior flush door family.") // Default description
                        );

                        // Add Metadata section with all parameters
                        if (parameters.Count > 0)
                        {
                            var metadataElement = new XElement("Metadata");
                            foreach (var param in parameters.OrderBy(p => p.Key))
                            {
                                var paramElement = new XElement("Parameter",
                                    new XAttribute("name", param.Key),
                                    param.Value
                                );
                                metadataElement.Add(paramElement);
                            }
                            familyElement.Add(metadataElement);
                        }

                        categoryGroupElement.Add(familyElement);
                    }

                    familiesByCategoryElement.Add(categoryGroupElement);
                }

                root.Add(familiesByCategoryElement);

                // Create XDocument and save
                var xmlDoc = new XDocument(
                    new XDeclaration("1.0", "UTF-8", null),
                    root
                );

                // Save XML with proper formatting
                xmlDoc.Save(outputPath);
            }
            catch (Exception ex)
            {
                Utility.Logger(ex);
                throw;
            }
        }

        /// <summary>
        /// Extracts families from Revit model and generates CSV/Excel output matching the same data structure as ExtractFamiliesToXmlAsync
        /// </summary>
        /// <param name="outputPath">Full path to output CSV file or directory (if directory, will create revit.csv)</param>
        private void ExtractFamiliesToCSVAsync(string outputPath)
        {
            if (string.IsNullOrWhiteSpace(outputPath))
                throw new ArgumentException("Output path cannot be null or empty", nameof(outputPath));

            // Determine if outputPath is a directory or file
            string csvFilePath;
            if (Directory.Exists(outputPath))
            {
                // It's a directory, append filename
                csvFilePath = Path.Combine(outputPath, "revit.csv");
            }
            else if (Path.HasExtension(outputPath) && outputPath.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                // It's a file path with .csv extension
                csvFilePath = outputPath;
            }
            else
            {
                // Assume it's a directory path (doesn't exist yet or no extension)
                csvFilePath = Path.Combine(outputPath, "revit.csv");
            }

            // Ensure output directory exists
            string outputDir = Path.GetDirectoryName(csvFilePath);
            if (!string.IsNullOrWhiteSpace(outputDir) && !Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            try
            {
                // Collect all families
                var families = CollectAllFamilies();

                // Group families by category
                var familiesByCategory = families.GroupBy(f => f.Category).ToList();

                // First pass: Collect all unique parameter names across all families
                var allParameterNames = new HashSet<string>();
                var familyDataList = new List<Dictionary<string, string>>();

                // Get document properties (same for all families)
                string revitVersion = _doc.Application.VersionNumber ?? "2023";
                string author = GetDocumentProperty("Username") ?? "BigFish BIM Sync";
                string lastUpdated = DateTime.Now.ToString("yyyy-MM-dd");

                foreach (var categoryGroup in familiesByCategory)
                {
                    foreach (var familyInfo in categoryGroup)
                    {
                        // Extract parameters
                        var parameters = ExtractFamilyParameters(familyInfo.Family);

                        // Get file size (if family file is accessible)
                        long fileSize = 0;
                        //try
                        //{
                        //    if (familyInfo.Family.IsEditable && !familyInfo.Family.IsInPlace)
                        //    {
                        //        // Try to get family document path
                        //        Document familyDoc = _doc.EditFamily(familyInfo.Family);
                        //        if (familyDoc != null)
                        //        {
                        //            string familyPath = familyDoc.PathName;
                        //            if (!string.IsNullOrEmpty(familyPath) && File.Exists(familyPath))
                        //            {
                        //                FileInfo fileInfo = new FileInfo(familyPath);
                        //                fileSize = fileInfo.Length;
                        //            }
                        //            familyDoc.Close(false);
                        //        }
                        //    }
                        //}
                        //catch
                        //{
                        //    // If we can't get file size, use default
                        //    fileSize = 552960; // Default size similar to example XML
                        //}

                        // Build file path
                        string filePath = $"{categoryGroup.Key}/{familyInfo.FamilyName}.rfa";

                        // Create data dictionary for this family
                        var familyData = new Dictionary<string, string>
                        {
                            ["Category"] = categoryGroup.Key,
                            ["FamilyName"] = familyInfo.FamilyName,
                            ["FilePath"] = filePath,
                            ["FileSize"] = fileSize.ToString(),
                            ["RevitVersion"] = revitVersion,
                            ["Author"] = author,
                            ["LastUpdated"] = lastUpdated,
                            ["Description"] = "Standard interior flush door family." // Default description
                        };

                        // Add all parameters to the dictionary and track parameter names
                        int paramIndex = 0;
                        foreach (var param in parameters.OrderBy(p => p.Key))
                        {
                            string paramName = "ParamName_"+paramIndex;
                            string paramValue = "ParamValue_"+paramIndex;
                            allParameterNames.Add(paramName);
                            allParameterNames.Add(paramValue);
                            familyData[$"ParamName_{paramIndex}"] = param.Key;
                            familyData[$"ParamValue_{paramIndex}"] = param.Value;
                            paramIndex++;
                        }

                        familyDataList.Add(familyData);
                    }
                }

                // Build CSV header
                var headerColumns = new List<string>
                {
                    "Category",
                    "FamilyName",
                    "FilePath",
                    "FileSize",
                    "RevitVersion",
                    "Author",
                    "LastUpdated",
                    "Description"
                };

                // Add parameter columns (sorted for consistency)
                //var sortedParamNames = allParameterNames.OrderBy(p => p).ToList();
                foreach (var paramName in allParameterNames)
                {
                    headerColumns.Add($"{paramName}");
                }

                // Write CSV file
                using (var writer = new StreamWriter(csvFilePath, false, Encoding.UTF8))
                {
                    // Write header row
                    writer.WriteLine(EscapeCsvLine(headerColumns));

                    // Write data rows
                    foreach (var familyData in familyDataList)
                    {
                        var rowValues = new List<string>();
                        foreach (var column in headerColumns)
                        {
                            if (familyData.TryGetValue(column, out string value))
                            {
                                rowValues.Add(value ?? string.Empty);
                            }
                            else
                            {
                                rowValues.Add(string.Empty);
                            }
                        }
                        writer.WriteLine(EscapeCsvLine(rowValues));
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.Logger(ex);
                throw;
            }
        }

        /// <summary>
        /// Internal class to hold family information
        /// </summary>
        private class FamilyInfo
        {
            public Family Family { get; set; }
            public string FamilyName { get; set; }
            public string Category { get; set; }
        }
    }

}
