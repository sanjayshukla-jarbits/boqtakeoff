using Autodesk.Revit.UI;
using boqtakeoff.ui;
using boqtakeoff.ui.Commands;
using boqtakeoff.core;
using System;

namespace boqtakeoff
{
    /// <summary>
    /// Setup whole plugins interface with tabs, panels, buttons,...
    /// </summary>
    public class SetupInterface
    {
        #region constructor

        /// <summary>
        /// Default constructor.
        /// Initializes a new instance of the <see cref="SetupInterface"/> class.
        /// </summary>
        public SetupInterface()
        {

        }

        #endregion

        #region public methods

        /// <summary>
        /// Initializes all interface elements on custom created Revit tab.
        /// </summary>
        /// <param name="app">The application.</param>
        public void Initialize(UIControlledApplication app)
        {

            // Create ribbon tab.
            string tabName = "BigFish Tools";
            app.CreateRibbonTab(tabName);

            // Create the ribbon panels.
            // var managerCommandsPanel = app.CreateRibbonPanel(tabName, "BOQ Extractor");
            var boqRoomPanel = app.CreateRibbonPanel(tabName, "Room");


            var TagRvtCreateRoom = new RevitPushButtonDataModel
            {
                Label = "Create Rooms",
                Panel = boqRoomPanel,
                Tooltip = "Create Room(s) automaticaly by Bigfish",
                CommandNamespacePath = CreateRoomCommand.GetPath(),
                AssemblyPath = CreateRoomCommand.GetAssemblyPath(),
                IconImageName = "icon_TagWallLayers_32x32.png",
                TooltipImageName = "icon_TagWallLayers_32x32.png"
            };

            // Create button from provided data.
            var CreateRoomButton = RevitPushButton.Create(TagRvtCreateRoom);
            // Create button from provided data.
            //var TagRvtRoomAnnotateButton = RevitPushButton.Create(TagRvtRoom);
            //boqRoomPanel.AddSeparator();

            /*-------------------------------------------[Second Section ]-----------------------------------------------------*/

            var SMIBoqPanel = app.CreateRibbonPanel(tabName, "SMI BOQ");
            //var AnnotateCommandElemData = new RevitPushButtonDataModel
            //{
            //    Label = "Annotate",
            //    Panel = SMIBoqPanel,
            //    Tooltip = "This is some sample tooltip text,\nreplace it with real one latter,...",
            //    CommandNamespacePath = AnnotateCommand.GetPath(),
            //    IconImageName = "icon_TagWallLayers_32x32.png",
            //    TooltipImageName = "tooltip_TagWallLayers_320x320.png"
            //};

            ////SMIBoqPanel.AddSeparator();
            //// Create button from provided data.
            //RevitPushButton.Create(AnnotateCommandElemData);

            var ExportBOQCommandElemData = new RevitPushButtonDataModel
            {
                Label = "BOQ Extractor",
                Panel = SMIBoqPanel,
                Tooltip = "Parses the CSV formatted BOQ and sends it to Bigfish ...",
                CommandNamespacePath = BOQExtractor.GetPath(),
                AssemblyPath = BOQExtractor.GetAssemblyPath(),
                IconImageName = "icon_ShowFamilyManager_32x32.png",
                TooltipImageName = "icon_ShowFamilyManager_32x32.png"
            };

            RevitPushButton.Create(ExportBOQCommandElemData);

            //SMIBoqPanel.AddSeparator();
            //var TagRvtRoomAnnotate = new RevitPushButtonDataModel
            //{
            //    Label = "Create Rooms",
            //    Panel = SMIBoqPanel,
            //    Tooltip = "Create Room(s) automaticaly by Bigfish",
            //    CommandNamespacePath = TagRvtRoomAnnotateCommand.GetPath(),
            //    IconImageName = "icon_TagWallLayers_32x32.png",
            //    TooltipImageName = "tooltip_TagWallLayers_320x320.png"
            //};

            /*-------------------------------------------[BIM Library Section]-----------------------------------------------------*/

            // Create BIM Library Panel
            var bimLibraryPanel = app.CreateRibbonPanel(tabName, "BIM Library");

            // Add BIM Library Browser Button
            AddBimLibraryBrowserButton(bimLibraryPanel);

            // Add separator
            bimLibraryPanel.AddSeparator();

            // Add Export Families to S3 Button
            AddExportFamiliesToS3Button(bimLibraryPanel);

            SMIBoqPanel.AddSeparator();

            var DrawingScheduleCommandData = new RevitPushButtonDataModel
            {
                Label = "Drawing Schedule",
                Panel = SMIBoqPanel,
                Tooltip = "View and manage drawing schedules from BigFish",
                CommandNamespacePath = ShowDrawingScheduleCommand.GetPath(),
                AssemblyPath = ShowDrawingScheduleCommand.GetAssemblyPath(),
                IconImageName = "icon_ShowFamilyManager_32x32.png",
                TooltipImageName = "icon_ShowFamilyManager_32x32.png"
            };

            RevitPushButton.Create(DrawingScheduleCommandData);
        }

        /// <summary>
        /// Add BIM Library Browser button to ribbon panel
        /// </summary>
        private void AddBimLibraryBrowserButton(RibbonPanel panel)
        {
            try
            {
                var bimLibraryData = new RevitPushButtonDataModel
                {
                    Label = "Browse\nLibrary",
                    Panel = panel,
                    Tooltip = "Browse and add families from cloud BIM Library",
                    CommandNamespacePath = BimLibraryCommand.GetPath(),
                    IconImageName = "icon_TagWallLayers_32x32.png",  // Replace with your icon
                    TooltipImageName = "icon_TagWallLayers_32x32.png"
                };

                var bimLibraryButton = RevitPushButton.Create(bimLibraryData);
            }
            catch (Exception ex)
            {
                boqtakeoff.core.Libraries.Utility.Logger(ex);
            }
        }

        /// <summary>
        /// Add Export Families to S3 button to ribbon panel
        /// </summary>
        private void AddExportFamiliesToS3Button(RibbonPanel panel)
        {
            try
            {
                var exportFamiliesData = new RevitPushButtonDataModel
                {
                    Label = "Export to\nS3",
                    Panel = panel,
                    Tooltip = "Export all families from current project to S3 bucket",
                    CommandNamespacePath = ExportFamiliesToS3Command.GetPath(),
                    IconImageName = "icon_TagWallLayers_32x32.png",  // Replace with your icon
                    TooltipImageName = "icon_TagWallLayers_32x32.png"
                };

                var exportButton = RevitPushButton.Create(exportFamiliesData);
            }
            catch (Exception ex)
            {
                boqtakeoff.core.Libraries.Utility.Logger(ex);
            }
        }
        #endregion
    }

}
