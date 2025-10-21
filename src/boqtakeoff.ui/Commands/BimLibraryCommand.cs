using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using boqtakeoff.ui.Views;
using System;

namespace boqtakeoff.ui.Commands
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class BimLibraryCommand : IExternalCommand
    {
        public static string GetPath()
        {
            return typeof(BimLibraryCommand).FullName;
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

                // Open BIM Library Browser window
                var window = new FamilyBrowserWindow(commandData);
                window.ShowDialog();

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                boqtakeoff.core.Libraries.Utility.Logger(ex);
                message = ex.Message;
                TaskDialog.Show("Error", $"Failed to open BIM Library: {ex.Message}");
                return Result.Failed;
            }
        }
    }
}