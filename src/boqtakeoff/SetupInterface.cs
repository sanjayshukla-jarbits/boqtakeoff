using Autodesk.Revit.UI;
using boqtakeoff.ui;
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
        }

        #endregion
    }
}
