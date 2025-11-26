using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using boqtakeoff.ui.Views;
using System;

namespace boqtakeoff.ui.Commands
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class FamilyUploadCommand : IExternalCommand
    {
        public static string GetPath()
        {
            return typeof(FamilyUploadCommand).FullName;
        }

        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            try
            {
                // Check if document is open
                if (commandData.Application.ActiveUIDocument == null)
                {
                    TaskDialog.Show("Error", "Please open a Revit project first");
                    return Result.Failed;
                }

                // Open Family Upload window
                var window = new FamilyUploadWindow(commandData);
                window.ShowDialog();

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                boqtakeoff.core.Libraries.Utility.Logger(ex);
                message = ex.Message;
                TaskDialog.Show("Error", $"Failed to open Family Upload window: {ex.Message}");
                return Result.Failed;
            }
        }
    }
}

