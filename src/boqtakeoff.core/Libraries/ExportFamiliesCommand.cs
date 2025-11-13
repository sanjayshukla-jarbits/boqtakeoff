using System;
using System.IO;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System.Windows.Forms;

namespace boqtakeoff.core.Libraries
{
    /// <summary>
    /// Command to export all loadable families from the active Revit project to local file system,
    /// organized by category in subdirectories.
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class ExportFamiliesCommand : IExternalCommand
    {
        /// <summary>
        /// Executes the export families command
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

                Document doc = commandData.Application.ActiveUIDocument.Document;

                // Validate document is a project (not a family document)
                if (doc.IsFamilyDocument)
                {
                    message = "This command only works with project files (.rvt), not family files (.rfa).";
                    TaskDialog.Show("Error", message);
                    return Result.Failed;
                }

                // Check if document is read-only
                if (doc.IsReadOnly)
                {
                    message = "The document is read-only. Cannot export families.";
                    TaskDialog.Show("Error", message);
                    return Result.Failed;
                }

                // Prompt user to select export folder
                string familiesExportPath = null;
                using (var folderDialog = new FolderBrowserDialog())
                {
                    folderDialog.Description = "Select folder to export families";
                    folderDialog.ShowNewFolderButton = true;

                    if (folderDialog.ShowDialog() != DialogResult.OK)
                    {
                        return Result.Cancelled;
                    }

                    familiesExportPath = folderDialog.SelectedPath;
                }

                // Validate selected path
                if (string.IsNullOrWhiteSpace(familiesExportPath))
                {
                    message = "No export folder selected.";
                    TaskDialog.Show("Error", message);
                    return Result.Failed;
                }

                // Create the base directory if it doesn't exist
                try
                {
                    if (!Directory.Exists(familiesExportPath))
                    {
                        Directory.CreateDirectory(familiesExportPath);
                    }
                }
                catch (Exception ex)
                {
                    message = $"Failed to create export directory: {ex.Message}";
                    Utility.Logger(ex, $"Failed to create directory: {familiesExportPath}", "error");
                    TaskDialog.Show("Error", message);
                    return Result.Failed;
                }

                // Collect all loaded families in the active document
                FilteredElementCollector familyCollector = new FilteredElementCollector(doc).OfClass(typeof(Family));

                int successCount = 0;
                int failCount = 0;
                int skippedCount = 0;

                foreach (Family family in familyCollector)
                {
                    // Skip system families and in-place families
                    if (!family.IsEditable || family.IsInPlace)
                    {
                        skippedCount++;
                        continue;
                    }

                    Document familyDoc = null;

                    try
                    {
                        // Open family document for editing
                        familyDoc = doc.EditFamily(family);

                        if (familyDoc == null)
                        {
                            failCount++;
                            Utility.Logger(null, $"Failed to open family document for: {family.Name}", "error");
                            continue;
                        }

                        // Create category directory: revit/Category
                        string familyCategory = family.FamilyCategory?.Name ?? "Uncategorized";
                        string sanitizedCategory = SanitizeFileName(familyCategory);
                        string categoryDir = Path.Combine(familiesExportPath, sanitizedCategory);
                        
                        if (!Directory.Exists(categoryDir))
                        {
                            Directory.CreateDirectory(categoryDir);
                        }

                        // Build file path: revit/Category/FamilyName.rfa
                        string sanitizedFileName = SanitizeFileName(family.Name);
                        string fileName = $"{sanitizedFileName}.rfa";
                        string filePath = Path.Combine(categoryDir, fileName);

                        // Configure save options to overwrite existing files
                        SaveAsOptions saveOptions = new SaveAsOptions
                        {
                            OverwriteExistingFile = true
                        };

                        // SaveAs cannot be called within a transaction - it must be called directly
                        familyDoc.SaveAs(filePath, saveOptions);
                        successCount++;
                    }
                    catch (Autodesk.Revit.Exceptions.InvalidOperationException iex)
                    {
                        failCount++;
                        Utility.Logger(iex, $"Invalid operation while exporting family '{family.Name}': {iex.Message}", "error");
                    }
                    catch (Autodesk.Revit.Exceptions.FileAccessException faex)
                    {
                        failCount++;
                        Utility.Logger(faex, $"File access error while exporting family '{family.Name}': {faex.Message}", "error");
                    }
                    catch (Exception ex)
                    {
                        failCount++;
                        Utility.Logger(ex, $"Failed to export family '{family.Name}': {ex.Message}", "error");
                    }
                    finally
                    {
                        // Always close the family document to prevent resource leaks
                        if (familyDoc != null)
                        {
                            try
                            {
                                familyDoc.Close(false);
                            }
                            catch (Exception closeEx)
                            {
                                Utility.Logger(closeEx, $"Error closing family document for '{family.Name}': {closeEx.Message}", "error");
                            }
                        }
                    }
                }

                // Show summary dialog
                string summaryMessage = $"Family export completed:\n\n" +
                    $"✓ Successfully exported: {successCount}\n" +
                    $"✗ Failed: {failCount}\n" +
                    $"⊘ Skipped: {skippedCount}\n\n" +
                    $"Export location:\n{familiesExportPath}";

                TaskDialog.Show("Export Complete", summaryMessage);
                
                // Log summary
                Utility.Logger(null, 
                    $"Family export completed: {successCount} succeeded, {failCount} failed, {skippedCount} skipped out of {successCount + failCount + skippedCount} total families.",
                    "error");

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = $"Command failed: {ex.Message}";
                Utility.Logger(ex, message, "error");
                TaskDialog.Show("Error", message);
                return Result.Failed;
            }
        }

        /// <summary>
        /// Sanitizes a filename or directory name by removing invalid characters
        /// </summary>
        /// <param name="fileName">The original filename or directory name</param>
        /// <returns>Sanitized filename safe for use in file system paths</returns>
        private string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return "Unnamed";

            // Replace invalid characters with underscores
            char[] invalidChars = Path.GetInvalidFileNameChars();
            string sanitized = fileName;

            foreach (char invalidChar in invalidChars)
            {
                sanitized = sanitized.Replace(invalidChar, '_');
            }

            // Remove any consecutive underscores and trim
            while (sanitized.Contains("__"))
            {
                sanitized = sanitized.Replace("__", "_");
            }

            return sanitized.Trim('_');
        }
    }
}