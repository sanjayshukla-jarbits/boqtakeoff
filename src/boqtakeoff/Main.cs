using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using boqtakeoff.ui;

namespace boqtakeoff
{
    /// <summary>
    /// Plugin's main entry point.
    /// </summary>
    /// <seealso cref="Autodesk.Revit.UI.IExternalApplication" />
    public class Main : IExternalApplication
    {
        #region external application public methods

        /// <summary>
        /// Called when Revit starts up.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Result OnStartup(UIControlledApplication application)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("BOQ Plugin initializing...");
                
                // Initialize whole plugin's user interface.
                var ui = new SetupInterface();
                ui.Initialize(application);

                // application.ControlledApplication.ApplicationInitialized += DockablePaneRegisters;

                System.Diagnostics.Debug.WriteLine("BOQ Plugin initialized successfully");
                return Result.Succeeded;
            }
            catch (System.Exception ex)
            {
                System.IO.File.AppendAllText(
                    System.IO.Path.Combine(
                        System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop),
                        "boq_plugin_error.log"),
                    $"{System.DateTime.Now}: {ex}\r\n");
                System.Diagnostics.Debug.WriteLine($"BOQ Plugin initialization failed: {ex}");
                return Result.Failed;
            }
        }

        /// <summary>
        /// Called when [Revit shutdown.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        #endregion
    }
}
