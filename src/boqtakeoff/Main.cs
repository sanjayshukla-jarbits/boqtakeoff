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
            // Initialize whole plugin's user interface.
            var ui = new SetupInterface();
            ui.Initialize(application);

            // application.ControlledApplication.ApplicationInitialized += DockablePaneRegisters;

            return Result.Succeeded;
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
