using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Xml.Linq;
using ComboBox = System.Windows.Forms.ComboBox;
using View = Autodesk.Revit.DB.View;
using mshtml;
using MessageBox = System.Windows.Forms.MessageBox;
using System.Collections.Concurrent;
using Remotion.Collections;
using System.Security.Cryptography;
using System.Windows.Input;
using boqtakeoff.core.Libraries;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows.Documents;
using System.Diagnostics;
using Document = Autodesk.Revit.DB.Document;
using System.Xml;
using Newtonsoft.Json.Linq;

namespace boqtakeoff.core
{
    public partial class frmAnnotateElement : System.Windows.Forms.Form
    {
        private static VisualStyleRenderer DisabledCheckBoxRenderer;

        #region [Members]
        public int Step { get; set; }
        public frmAnnotateElement()
        {
            //InitializeComponent();
        }
        public ExternalCommandData externalCommandData { get; set; }
        public Hashtable project_uid_hash_project_details = new Hashtable();
        string revit_project_name;
        DataGridViewCheckBoxColumn checkBoxColumn = new DataGridViewCheckBoxColumn();
        System.Windows.Forms.CheckBox headerCheckBox = new System.Windows.Forms.CheckBox();
        #endregion
        public frmAnnotateElement(ExternalCommandData _externalCommandData)
        {
            InitializeComponent();
            externalCommandData = _externalCommandData;
            Step = 1;
            DisabledCheckBoxRenderer = new VisualStyleRenderer(VisualStyleElement.Button.CheckBox.UncheckedDisabled);
        }
        private void btnAnnotate_Click(object sender, EventArgs e)
        {

            //textBox1.Text = 
        }


        private void frmAnnotateElement_Load(object sender, EventArgs e)
        {

            //RvtRoomUtility.SaveImage(externalCommandData.Application.ActiveUIDocument);
            this.Width = 777;
            PopulateRoomLevels();
            BindNameSuggestionGrid();

        }

        //private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        //{
        //    BackgroundWorker helperBW = sender as BackgroundWorker;
        //    int arg = (int)e.Argument;
        //    e.Result = BackgroundProcessLogicMethod(helperBW, arg);
        //    if (helperBW.CancellationPending)
        //    {
        //        e.Cancel = true;
        //    }
        //}

        //private int BackgroundProcessLogicMethod(BackgroundWorker bw, int a)
        //{
        //    int result = 0;
        //    //ReadProjectProjects();
        //    //RvtRoomUtility.SaveImage(externalCommandData.Application.ActiveUIDocument);
        //    //PopulateRoomLevels();
        //    //BindNameSuggestionGrid();

        //    return result;
        //}

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {

            if (Step == 1)
            {
                // UpdateRoomName();

                pnlRoom.Visible = false;
                pnlStatus.Visible = false;

                pnlFamily.Left = pnlRoom.Left - 10;
                pnlFamily.Top = pnlRoom.Top - 10;

                pnlFamily.Visible = true;
                bindFamilyGridWith();
                btnNext.Text = "Finish";

            }
            else if (Step == 2)
            {
                if (MessageBox.Show("Do you want to Annotate the selected categoties?", "Annotation Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Hashtable selectedValues = new Hashtable();
                    selectedValues = GetFamilyGridValues();
                    AnnotateElements.AnnotateRoomElement(externalCommandData, selectedValues);

                    btnBack.Visible = false;
                    btnNext.Visible = false;

                    Utility.ShowMessage("Annotation Completed Successfully", "Completed");
                    this.Close();
                }
            }
            if (Step >= 2)
            {
                Step = 2;
            }
            Step++;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Step--;
            if (Step == 1)
            {
                btnNext.Text = "Next";

                pnlFamily.Visible = false;
                pnlStatus.Visible = false;
                pnlRoom.Visible = true;

                //777, 607
                this.Width = 777;
            }
            else if (Step == 2)
            {
                // Family
                pnlRoom.Visible = false;
                pnlStatus.Visible = false;
                pnlFamily.Left = pnlRoom.Left;
                pnlFamily.Top = pnlRoom.Top;

                pnlFamily.Visible = true;

                this.Width = 777;
            }
            if (Step <= 0)
            {
                Step = 1;
            }

        }

        private void pnlFamily_Paint(object sender, PaintEventArgs e)
        {

        }

        private void PopulateRoomLevels()
        {
            var activeUidoc = externalCommandData.Application.ActiveUIDocument;
            //Hashtable roomLevels = RvtRoomUtility.GetFloorPlanAssociatedlevelDetails(activeUidoc, "level_id");
            Hashtable roomLevels = RvtRoomUtility.GetRoomLevelDetail(activeUidoc);

            //MessageBox.Show("roomLevels Count:" + roomLevels.Count);
            cmdRoomLevel.Items.Clear();
            cmdRoomLevel.DisplayMember = "value";
            cmdRoomLevel.ValueMember = "key";

            /// cmdRoomLevel.DataSource = roomLevels;
            foreach (var item in roomLevels)
            {

                //MessageBox.Show("hashtbl[key]:" + roomLevels[Key].ToString() + "---" + Key.ToString());
                cmdRoomLevel.Items.Add(item);
            }
            cmdRoomLevel.SelectedIndex = 0;
        }


        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                LargeImage largeImage = new LargeImage(ImageFilePath);
                largeImage.Show();
            }
            catch (Exception exp)
            {
                Utility.Logger(exp);
            }
        }

        public string ImageFilePath { get; set; }
        private void cmdRoomLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmdRoomLevel.SelectedIndex > -1)
            {
                if (cmdRoomLevel.SelectedIndex == -1)
                {
                    MessageBox.Show("Please Select Room Level", "Room", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else
                {
                    string _level_element_id = Convert.ToString(((System.Collections.DictionaryEntry)cmdRoomLevel.SelectedItem).Key);
                    //MessageBox.Show("_level_element_id-: " + _level_element_id);
                    var activeUidoc = externalCommandData.Application.ActiveUIDocument;
                    ImageFilePath = RvtRoomUtility.SaveImage(activeUidoc, _level_element_id);
                    pnlPreview.BackgroundImage = System.Drawing.Image.FromFile(ImageFilePath);
                    pnlPreview.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
                    this.Width = 1490;
                }
            }
        }


        private void cmdRoomLevel_SelectedValueChanged(object sender, EventArgs e)
        {

        }

        private string CreateRunningNames(string RoomCategory)
        {
            try
            {
                string ddlValues = string.Empty;
                string NamesSuggestions = string.Empty;
                Int32 categoryRunningNumber = 0;

                foreach (DataGridViewRow item in dgRoomNames.Rows)
                {
                    if (item.Cells[1].Value != null)
                        if (item.Cells[1].Value.ToString() == RoomCategory)
                        {
                            categoryRunningNumber++;
                        }
                }
                //NamesSuggestions = string.Format("Bigfish-{0}{1}", RoomCategory, categoryRunningNumber);
                NamesSuggestions = string.Format("{0}{1}", RoomCategory, categoryRunningNumber);
                return NamesSuggestions;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void dgRoomNames_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            //if (e.RowIndex >= 0 && e.ColumnIndex == 1)
            //{
            //    if (this.dgRoomNames.Rows[e.RowIndex].Cells[1].Value != null)
            //    {
            //        string SName = CreateRunningNames(this.dgRoomNames.Rows[e.RowIndex].Cells[1].Value.ToString());
            //        this.dgRoomNames.Rows[e.RowIndex].Cells[2].Value = SName;
            //    }
            //    else
            //        this.dgRoomNames.Rows[e.RowIndex].Cells[2].Value = "";
            //}
        }

        class SuggestionClass
        {
            public string SchemeItemName;
            public string SchemeItemName_Suggestion;

        }


        class CustomCategory
        {
            public string Family { get; set; }
            public int NoOfElements { get; set; }
            public string Extracted { get; set; }
            public string ExtractionDate { get; set; }
            public string AnnotationRequired { get; set; }
        }

        class CustomRoomTags
        {
            public string RoomName { get; set; }
            //public IList<RoomTType> Suggestion { get; set; }
            public string Suggestion { get; set; }

        }

        class RoomNameSuggestion
        {
            public string RoomName { get; set; }
            //public IList<RoomTType> Suggestion { get; set; }
            public string Suggestion { get; set; }

        }

        class RoomTType
        {
            public string RoomSuggestionName { get; set; }
            public string RoomSuggestionValue { get; set; }
        }

        class RoomTSuggestion
        {
            public string RoomName { get; set; }
            public string RoomSuggestionName { get; set; }
        }


        IList<RoomTSuggestion> RoomNameSuggestionList = new List<RoomTSuggestion>();
        IList<RoomNameSuggestion> RoomNameSuggestions = new List<RoomNameSuggestion>();
        private void BindNameSuggestionGrid()
        {
            this.dgRoomNames.DefaultCellStyle.Font = new Font("Arial", 9);
            var doc = externalCommandData.Application.ActiveUIDocument.Document;
            View activeView = doc.ActiveView;


            /*  toilet
                    mdRoom
                    Breakout area
                    Conference Room
                    unnamed Room */

            //IList<RoomTType> roomTTypes = new List<RoomTType>();

            //#region [Room Type]
            //RoomTType roomTType = new RoomTType();

            //roomTType.RoomSuggestionName = "Toilet";
            //roomTType.RoomSuggestionValue = "Toilet";

            //roomTTypes.Add(roomTType);
            //roomTType = new RoomTType();
            //roomTType.RoomSuggestionName = "mdRoom";
            //roomTType.RoomSuggestionValue = "mdRoom";

            //roomTTypes.Add(roomTType);

            //roomTType = new RoomTType();
            //roomTType.RoomSuggestionName = "Breakout area";
            //roomTType.RoomSuggestionValue = "Breakout area";

            //roomTTypes.Add(roomTType);

            //roomTType = new RoomTType();
            //roomTType.RoomSuggestionName = "Conference Room";
            //roomTType.RoomSuggestionValue = "Conference Room";

            //roomTTypes.Add(roomTType);

            //roomTType = new RoomTType();
            //roomTType.RoomSuggestionName = "Bigfish-Room";
            //roomTType.RoomSuggestionValue = "Bigfish-Room";

            //roomTTypes.Add(roomTType);
            //#endregion

            //var doc = externalCommandData.Application.ActiveUIDocument.Document.;


            FilteredElementCollector room_collector = new FilteredElementCollector(doc, doc.ActiveView.Id)
                .OfCategory(BuiltInCategory.OST_Rooms).WhereElementIsNotElementType();

            IList<ElementId> room_eids = room_collector.ToElementIds() as IList<ElementId>;
            string rNames = string.Empty;


            string Message = string.Empty;

            string RoomSuggestionName = string.Empty;
            int i = 0;
            string Details = string.Empty;
            double layoutsize = GetLayoutSize();
            foreach (ElementId eid in room_eids)
            {
                //i++;

                //if (i > 5) return;

                Room R = doc.GetElement2(eid) as Room;
                RoomNameSuggestion roomNameItem = new RoomNameSuggestion();

                RoomSuggestionName = GetFurniture(R, layoutsize);

                if (RoomSuggestionName == "")
                {
                    RoomSuggestionName = "Bigfish-Room";
                }
                //if (R == null)
                //{
                //    MessageBox.Show("Room is Null");
                //}
                if (R.Name.StartsWith("Bigfish-Room"))
                {
                    //rNames = String.Format("{0}({1})", R.Name.Split(' ')[0].ToString(), R.Name.Split(' ')[1].ToString());
                    roomNameItem.RoomName = R.Name;
                    Message += rNames + ", ";
                }
                else
                {
                    roomNameItem.RoomName = R.Name;// + "@>" + R;
                    Message += R.Name + ", ";
                }

                //MessageBox.Show("1");
                RoomTSuggestion ts = new RoomTSuggestion();
                ts.RoomName = R.Name;
                ts.RoomSuggestionName = RoomSuggestionName;
                RoomNameSuggestionList.Add(ts);

                RoomNameSuggestions.Add(roomNameItem);
            }

            RoomNameSuggestions = RoomNameSuggestions.GroupBy(z => z.RoomName).Select(x => x.First()).ToList();
            //MessageBox.Show("RoomNameSuggestions :" + iResult.Count);
            // RoomNameSuggestions = RoomNameSuggestions.Select(d => d.RoomName).Distinct() as IList<RoomNameSuggestion>;

            //RoomNameSuggestions.ToLookup(p => p.RoomName).Select(coll => coll.First());

            dgRoomNames.Columns.Clear();
            // MessageBox.Show("RoomNameSuggestions :" + RoomNameSuggestions.Count);
            DataGridViewTextBoxColumn Roomname = new DataGridViewTextBoxColumn();
            Roomname.HeaderText = "Room Name";
            Roomname.DataPropertyName = "RoomName";
            Roomname.Width = 400;
            Roomname.ReadOnly = true;
            dgRoomNames.Columns.Add(Roomname);

            DataGridViewComboBoxColumn roomTypeItems = new DataGridViewComboBoxColumn();
            roomTypeItems.Name = "Suggested Name";
            // roomTypeItems.DataSource = roomTTypes;
            roomTypeItems.DataPropertyName = "RoomSuggestionName";
            roomTypeItems.ValueMember = "RoomSuggestionValue";
            roomTypeItems.Width = 200;

            dgRoomNames.Columns.Add(roomTypeItems);


            //DataGridViewTextBoxColumn RoomName = new DataGridViewTextBoxColumn();
            //RoomName.HeaderText = "Annotated Name";
            //RoomName.DataPropertyName = "Suggestion";
            //RoomName.Width = 200;
            //dgRoomNames.Columns.Add(RoomName);

            dgRoomNames.AutoGenerateColumns = false;

            //dgRoomNames.DataSource = RoomNameSuggestions.OrderBy(x => x.RoomName).Distinct().ToList();
            dgRoomNames.DataSource = RoomNameSuggestions.OrderBy(x => x.RoomName).ToList();

            SetSuggestNameInDropdown();
            //textBox1.Text = roomlist;
        }

        private void UpdateRoomName()
        {
            var doc = externalCommandData.Application.ActiveUIDocument.Document;


            IList<RoomNameSuggestion> changegRoomNameSuggestions = new List<RoomNameSuggestion>();

            FilteredElementCollector room_collector = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms).WhereElementIsNotElementType();
            IList<ElementId> room_eids = room_collector.ToElementIds() as IList<ElementId>;
            string rNames = string.Empty;

            string Message = string.Empty;
            int iCounter = 1;
            //foreach (var item in RoomNameSuggestions)
            //{
            //    if (item.Suggestion != null && item.Suggestion.Trim().Length > 0)
            //    {
            //        RoomNameSuggestion _RoomNameSuggestion = new RoomNameSuggestion();
            //        _RoomNameSuggestion.RoomName = item.RoomName;
            //        _RoomNameSuggestion.Suggestion = item.Suggestion;
            //        changegRoomNameSuggestions.Add(_RoomNameSuggestion);
            //        Message += iCounter.ToString() + "- " + _RoomNameSuggestion.RoomName + " Change to  " + _RoomNameSuggestion.Suggestion + "\r\n";
            //        iCounter++;
            //    }
            //}

            //DialogResult _yesNo = MessageBox.Show(Message, "Room Name Confirmation", MessageBoxButtons.YesNo,MessageBoxIcon.Question);

            //if (changegRoomNameSuggestions.Count > 0)
            //    if (MessageBox.Show(Message, "Room Name Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            //    {
            //        int i = 0;
            //        using (Transaction t = new Transaction(doc))
            //        {

            //            t.Start("Center Room Tags");
            //            foreach (ElementId eid in room_eids)
            //            {
            //                Room R = doc.GetElement2(eid) as Room;
            //                foreach (var item in changegRoomNameSuggestions)
            //                {
            //                    if (R.Name.StartsWith(item.RoomName))
            //                    {
            //                        i++;
            //                        R.Name = item.Suggestion;
            //                    }
            //                }
            //            }
            //            t.Commit();
            //            //MessageBox.Show("Renaming completed Sucessfully..", "Info");
            //        }
            //    }
        }

        public string MyProperty { get; set; }
        private string GetFurniture(Room room, double layoutSize)
        {
            string SuggestedRoomName = string.Empty;
            try
            {
                BoundingBoxXYZ bb = room.get_BoundingBox(null);

                Outline outline = new Outline(bb.Min, bb.Max);

                BoundingBoxIntersectsFilter filter
                  = new BoundingBoxIntersectsFilter(outline);

                Autodesk.Revit.DB.Document doc = room.Document;

                BuiltInCategory[] bics = new BuiltInCategory[] {
                BuiltInCategory.OST_Furniture,
                BuiltInCategory.OST_PlumbingFixtures,
                BuiltInCategory.OST_SpecialityEquipment
              };

                LogicalOrFilter categoryFilter
                  = new LogicalOrFilter(bics
                    .Select<BuiltInCategory, ElementFilter>(
                      bic => new ElementCategoryFilter(bic))
                    .ToList<ElementFilter>());

                FilteredElementCollector familyInstances
                  = new FilteredElementCollector(doc)
                    .WhereElementIsNotElementType()
                    .WhereElementIsViewIndependent()
                    .OfClass(typeof(FamilyInstance))
                    .WherePasses(categoryFilter)
                    .WherePasses(filter);

                int roomid = room.Id.IntegerValue;

                List<Element> a = new List<Element>();

                //try
                //{
                //MyProperty += room.Name + "--";
                foreach (FamilyInstance fi in familyInstances)
                {
                    if (null != fi.Room && fi.Room.Id.IntegerValue.Equals(roomid))
                    {
                        //Debug.Assert(fi.Location is LocationPoint, "expected all furniture to have a location point");
                        a.Add(fi);
                    }
                }
                //}
                //catch (InvalidOperationException exx)
                //{
                //    Utility.Logger(exx);
                //}
                /* Get Room Area */

                Parameter par = room.get_Parameter(BuiltInParameter.ROOM_AREA);
                string valSting = par.AsValueString();
                double roomsize = par.AsDouble(); //mind the value is in native Revit units, not project units. So square feet in this case

                /* ------------------ */
                SuggestedRoomName = SuggestRoomName(a, roomsize, layoutSize);
            }
            catch (Exception ex)
            {
                Utility.Logger(ex);
                //MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
                //throw;
            }
            finally
            {
            }
            return SuggestedRoomName;
        }

        class RoomValidateMatrixClass
        {
            public string roomName { get; set; }
            //public string roomType { get; set; }
            public string furnitureName { get; set; }
            public string category { get; set; }
            public string priority { get; set; }
            public double roomSize { get; set; }
            public double layoutSize { get; set; }
            public Int64 minValue { get; set; }
            public Int64 maxValue { get; set; }
        }
        private string SuggestRoomName(List<Element> furnitures, double roomsize = 400, double layoutsize = 500)
        {
            try
            {
                IList<RoomValidateMatrixClass> _roomValidateMatrixClass = new List<RoomValidateMatrixClass>();
                XmlDocument xDoc = new XmlDocument();
                string xmlfilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Resources\\settings.xml";
                xDoc.Load(xmlfilePath);

                RoomValidateMatrixClass roomValidateMatrix = new RoomValidateMatrixClass();
                string RoomName = string.Empty;

                XmlNodeList roomTypes = xDoc.SelectNodes("//roomtype/@Name");
                //XmlNodeList roomTypes = xDoc.SelectNodes("//roomtype");
                foreach (Element f in furnitures)
                {
                    if (f.Name.ToLower() != "finish_anchor")
                    {
                        XmlNodeList selectedNode = xDoc.SelectNodes("//roomtype/rule");
                        // MessageBox.Show("selectedNode:" + selectedNode.Count);
                        foreach (XmlNode item in selectedNode)

                        {
                            roomValidateMatrix = new RoomValidateMatrixClass();
                            if (f.Name.ToLower().Contains(item.InnerText.ToLower()))
                            {
                                //        MessageBox.Show("item.InnerText :" + item.InnerText + " \r\n item.ParentNode.Attributes[\"Name\"].Value: " + item.ParentNode.Attributes["Name"].Value
                                //+ "\r\n item.Attributes[\"category\"].Value:" + item.Attributes["category"].Value + "\r\n item.Attributes[\"priority\"].Value:" + item.Attributes["priority"].Value 
                                //+ "\r\n roomsize:" + roomsize);

                                roomValidateMatrix.furnitureName = item.InnerText;
                                roomValidateMatrix.roomName = item.ParentNode.Attributes["Name"].Value;
                                roomValidateMatrix.category = item.Attributes["category"].Value;
                                roomValidateMatrix.roomSize = roomsize;
                                roomValidateMatrix.priority = item.Attributes["priority"].Value;
                                roomValidateMatrix.layoutSize = layoutsize;
                                roomValidateMatrix.minValue = Convert.ToInt64(item.ParentNode.Attributes["minvalue"].Value);
                                roomValidateMatrix.maxValue = Convert.ToInt64(item.ParentNode.Attributes["maxvalue"].Value);

                                _roomValidateMatrixClass.Add(roomValidateMatrix);
                            }
                        }
                    }
                }

                /* Need to handle scenario, where element belongs to multiple groups/family */
                if (_roomValidateMatrixClass.Count > 0)
                {
                    bool isPriorityAvailable = false;

                    RoomValidateMatrixClass selectedRow = new RoomValidateMatrixClass();

                    foreach (RoomValidateMatrixClass item in _roomValidateMatrixClass)
                    {
                        if (item.priority.ToLower() == "primary")
                        {
                            selectedRow = item;
                            isPriorityAvailable = true;
                            break;
                        }
                        else
                        { selectedRow = item; }
                    }

                    RoomName = FindMaxAvaialableElement(_roomValidateMatrixClass, roomTypes);

                }
               
                return RoomName;
                //return errorMappings;
            }
            catch (Exception ex)
            {
                Utility.Logger(ex);
                throw;
            }
        }
        class RoomCount
        {
            public string RoomName { get; set; }
            public int count { get; set; }
            public double layoutsize { get; set; }
            public double roomSize { get; set; }
            public double minValue { get; set; }
            public double maxValue { get; set; }

        }
        private string FindMaxAvaialableElement(IList<RoomValidateMatrixClass> roomValidateMatrixClass, XmlNodeList selecteRoomTypes)
        {
            try
            {
                IList<RoomCount> roomCount = new List<RoomCount>();
                RoomValidateMatrixClass selectedRow = new RoomValidateMatrixClass();
                int rCount = 1;
                double _roomSize = 0;
                string RoomName = string.Empty;
                //MessageBox.Show("selecteRoomTypedNode: " + selecteRoomTypedNode.Count + "\r\n roomValidateMatrixClass" + roomValidateMatrixClass.Count);
                foreach (XmlNode item in selecteRoomTypes)
                {
                    rCount = 1;
                    //MessageBox.Show("item.Attributes : " + item.Value.ToString());
                    foreach (RoomValidateMatrixClass CItem in roomValidateMatrixClass)
                    {
                        //MessageBox.Show("item.Attributes[\"Name\"].ToString().ToLower(): " + item.Attributes["Name"].ToString().ToLower() + "\r\n CItem.roomName.ToLower(): " + CItem.roomName.ToLower());
                        if (item.Value.ToString().ToLower().Equals(CItem.roomName.ToLower()))
                        {
                            selectedRow = CItem;
                            rCount++;
                        }
                    }
                    roomCount.Add(new RoomCount { count = rCount, RoomName = item.Value.ToString(), roomSize = selectedRow.roomSize, layoutsize = selectedRow.layoutSize, minValue = selectedRow.minValue, maxValue = selectedRow.maxValue });
                }

                IList<RoomCount> SelectedroomCount = roomCount.OrderByDescending(x => x.count).ToList();
                //MessageBox.Show("SelectedroomCount: " + SelectedroomCount.Count + "\r\n SelectedroomCount: " + SelectedroomCount[0].count + "\r\n SelectedroomCount RoomName:" + SelectedroomCount[0].RoomName
                //    + "\r\n SelectedroomCount[3].count" +  SelectedroomCount[3].count + "\r\n SelectedroomCount RoomName3:" + SelectedroomCount[3].RoomName);


                double roomSizeCalculated = (SelectedroomCount[0].roomSize / SelectedroomCount[0].layoutsize) * 100;
                //MessageBox.Show("roomSizeCalculated: " + roomSizeCalculated 
                //    + "\r\n _roomSize:" + SelectedroomCount[0].roomSize 
                //    + "\r\n SelectedroomCount: " + SelectedroomCount 
                //    + "\r\n SelectedroomCount[0].layoutsize: " + SelectedroomCount[0].layoutsize);

                if (SelectedroomCount[0].minValue < roomSizeCalculated && SelectedroomCount[0].maxValue > roomSizeCalculated)
                {
                    RoomName = SelectedroomCount[0].RoomName;
                }
                else
                {
                    RoomName = "Bigfish-Room";
                }

                return RoomName;
            }
            catch (Exception)
            {
                throw;
            }
        }

        class Staggered
        {
            public string Category { get; set; }
            public string Unit { get; set; }
            public string Controls { get; set; }

        }


        private void BindFamilyStaggeredGrid()
        {
            try
            {

                #region [Unit dummy data]
                IList<RoomTType> roomTTypes = new List<RoomTType>();

                #region [Room Type]
                RoomTType roomTType = new RoomTType();
                //RM, Rft, Sqft, Sqm
                roomTType.RoomSuggestionValue = "RM";
                roomTType.RoomSuggestionValue = "RM";

                roomTTypes.Add(roomTType);
                roomTType = new RoomTType();
                roomTType.RoomSuggestionValue = "Rft";
                roomTType.RoomSuggestionValue = "Rft";

                roomTTypes.Add(roomTType);

                roomTType = new RoomTType();
                roomTType.RoomSuggestionValue = "Sqft";
                roomTType.RoomSuggestionValue = "Sqft";

                roomTTypes.Add(roomTType);

                roomTType = new RoomTType();
                roomTType.RoomSuggestionValue = "Sqm";
                roomTType.RoomSuggestionValue = "Sqm";

                roomTTypes.Add(roomTType);
                #endregion
                #endregion

                IList<string> _staggeredGridData = Properties.Resources.StaggeredGridData.Split(',');

                IList<Staggered> staggered_ = new List<Staggered>();

                dgViewFamily.Columns.Clear();

                dgViewFamily.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.Blue;
                dgViewFamily.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;

                this.dgViewFamily.DefaultCellStyle.Font = new Font("Arial", 10);


                DataGridViewTextBoxColumn RoomSuggestionName = new DataGridViewTextBoxColumn();
                RoomSuggestionName.HeaderText = "Categoryx";
                RoomSuggestionName.DataPropertyName = "Category";
                RoomSuggestionName.Width = 300;
                RoomSuggestionName.ReadOnly = true;
                dgViewFamily.Columns.Add(RoomSuggestionName);



                DataGridViewComboBoxColumn unitType = new DataGridViewComboBoxColumn();
                unitType.Name = "Category";
                unitType.DataSource = roomTTypes;
                unitType.DataPropertyName = "RoomSuggestionName";
                unitType.ValueMember = "RoomSuggestionValue";
                unitType.Width = 200;

                dgViewFamily.Columns.Add(unitType);


                //DataGridViewCheckBoxColumn chkBox = new DataGridViewCheckBoxColumn();

                //dgViewFamily.Columns.Add(chkBox);

                DataGridViewCheckBoxColumn doWork = new DataGridViewCheckBoxColumn();
                doWork.HeaderText = "Include Dog";
                doWork.FalseValue = "0";
                doWork.TrueValue = "1";
                dgViewFamily.Columns.Add(doWork);

                dgViewFamily.AutoGenerateColumns = false;
                Staggered staggered = new Staggered();
                foreach (string s in _staggeredGridData)
                {
                    staggered = new Staggered();
                    staggered.Category = s;
                    staggered.Unit = "";
                    staggered_.Add(staggered);
                }

                dgViewFamily.DataSource = staggered_;

            }
            catch (Exception)
            {

                throw;
            }

        }


        private void SetColumns()
        {

            this.dgViewFamily.DefaultCellStyle.Font = new Font("Arial", 10);

            DataGridViewComboBoxColumn comboColumn = new DataGridViewComboBoxColumn();
            comboColumn.Name = "Grade";
            comboColumn.HeaderText = "";
            comboColumn.ValueMember = "Grade";
            comboColumn.DisplayMember = "Grade";


            dgViewFamily.Columns.Add("Category", "Category");

            dgViewFamily.Columns.Add(comboColumn);

            DataGridViewCheckBoxColumn chkBox = new DataGridViewCheckBoxColumn();

            dgViewFamily.Columns.Add(chkBox);

            dgViewFamily.Columns[0].Width = 300;
            dgViewFamily.Columns[0].ReadOnly = true;
            dgViewFamily.Columns[1].Width = 200;
            dgViewFamily.Columns[2].Width = 80;
        }

        private void AddRows()
        {
            int newRowIndex = 0;
            IList<string> _staggeredGridData = Properties.Resources.StaggeredGridData.Split(',');
            foreach (string s in _staggeredGridData)
            {
                newRowIndex = dgViewFamily.Rows.Add();
                dgViewFamily.Rows[newRowIndex].Cells[0].Value = s;
            }
        }

        private void SetCombo(string comboValueType, DataGridViewComboBoxCell comboCell)
        {
            comboCell.Value = "";
            comboCell.Items.Clear();
            if (comboValueType == "No")
            {
                comboCell.Items.Add("Nos");
                comboCell.Value = "Nos";
            }
            else if (comboValueType == "British")
            {
                /* Sqft, Sqm, SQM, RM, Rft */
                comboCell.Items.Add("Sqft");
                comboCell.Items.Add("Sqm");
                comboCell.Value = "Sqft";

            }
            else if (comboValueType == "Metric")
            {
                //comboCell.Items.Add("Sqm");
                comboCell.Items.Add("Rm");
                //comboCell.Items.Add("Sft");
                comboCell.Value = "Rm";
            }
            //else
            //{
            //    comboCell.Items.Add("Sqft");
            //    comboCell.Items.Add("Sqm");
            //    comboCell.Items.Add("Rm");
            //    comboCell.Items.Add("Sft");
            //    comboCell.Items.Add("No");
            //    comboCell.Value = "Sqft";
            //}
        }

        private void dgViewFamily_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                //MessageBox.Show("dgViewFamily_CellValueChanged");
                if (e.ColumnIndex == 0)
                {
                    if (dgViewFamily.Rows[e.RowIndex].Cells[0].Value != null)
                    {
                        DataGridViewComboBoxCell comboCell = dgViewFamily.Rows[e.RowIndex].Cells[1] as DataGridViewComboBoxCell;

                        DataGridViewCheckBoxCell comboChk = dgViewFamily.Rows[e.RowIndex].Cells[1] as DataGridViewCheckBoxCell;
                        /*
                         
                        Sprinkler
                        Fire Alarm
                        Electrical Fixture
                        Lighting Fixture
                        Communication Device

                        Door
                        Glass Partition
                        Furniture
                        Profile & Generic Elements
                        Lintels
                        Joinery
                         */
                        string currentCellValue = dgViewFamily.Rows[e.RowIndex].Cells[0].Value.ToString().Trim();


                        if (currentCellValue == "MEP - Duct Fitting" || currentCellValue == "MEP - Sprinkler" || currentCellValue == "MEP - Fire Alarm" || currentCellValue == "MEP - Electrical Fixture" || currentCellValue == "MEP - Lighting Fixture"
                            || currentCellValue == "MEP - Communication Device" || currentCellValue == "MEP - HVAC" || currentCellValue == "MEP - Mechanical Equipment" ||
                            currentCellValue == "Interior - Door" || currentCellValue == "Interior - Glass Partition" || currentCellValue == "Interior - Furniture" || currentCellValue == "Interior - Profile & Generic Elements"
                            || currentCellValue == "Interior - Lintels" || currentCellValue == "Interior - Joinery" || currentCellValue == "Interior - Partition")
                        {
                            SetCombo("No", comboCell);
                        }
                        if (rdoBritish.Checked)
                        {

                            if (currentCellValue == "Interior - Floor" || currentCellValue == "Interior - Ceiling" || currentCellValue == "Interior - Speciality Equipment" || currentCellValue == "MEP - Duct"
                                || currentCellValue == "MEP - HVAC" || currentCellValue == "MEP - Fire Pipe" || currentCellValue == "MEP - Duct" || currentCellValue == "MEP - Cable Tray")
                            {

                                SetCombo("British", comboCell);
                            }
                        }
                        else if (rdoMetric.Checked)
                        {
                            if (currentCellValue == "Interior - Floor" || currentCellValue == "Interior - Ceiling" || currentCellValue == "Interior - Speciality Equipment" || currentCellValue == "MEP - Duct"
                            || currentCellValue == "MEP - HVAC" || currentCellValue == "MEP - Fire Pipe" || currentCellValue == "MEP - Duct" || currentCellValue == "MEP - Cable Tray")
                            {

                                SetCombo("Metric", comboCell);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void rdoBritish_CheckedChanged(object sender, EventArgs e)
        {
            bindFamilyGridWith();
        }


        private void btnMetric_CheckedChanged(object sender, EventArgs e)
        {
            bindFamilyGridWith();
        }

        private void bindFamilyGridWith()
        {
            dgViewFamily.Columns.Clear();
            this.dgViewFamily.DefaultCellStyle.Font = new Font("Arial", 9);
            dgViewFamily.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.Blue;
            dgViewFamily.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            SetColumns();
            AddRows();

        }
        private void dgViewFamily_RowPostPaint_1(object sender, DataGridViewRowPostPaintEventArgs e)
        {

        }

        private Hashtable GetFamilyGridValues()
        {
            try
            {
                Hashtable categoryHastable = new Hashtable();
                foreach (DataGridViewRow r in dgViewFamily.Rows)
                {
                    if (r.IsNewRow) continue;
                    var selected = (bool?)r.Cells[2].Value;
                    if (selected.HasValue && selected.Value)
                    {
                        //MessageBox.Show("Found" + r.Cells[0].Value + "----" + r.Cells[1].Value);
                        categoryHastable.Add(r.Cells[0].Value.ToString().Trim(), r.Cells[1].Value.ToString().Trim());
                    }
                }
                return categoryHastable;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool isBigfishViewAvailable()
        {
            try
            {
                var activeUidoc = externalCommandData.Application.ActiveUIDocument;
                Hashtable roomLevels = RvtRoomUtility.GetRoomLevelDetail(activeUidoc);
                bool status = false;
                foreach (DictionaryEntry item in roomLevels)
                {
                    string _level_element_id = item.Key.ToString();
                    status = RvtRoomUtility.isBigfishViewAvailable(activeUidoc, _level_element_id);

                    if (status == false)
                    {
                        Utility.ShowMessage("Bigfish Level is not available, Please create via Bigfish Create Room Command!!", "Information", "error", "message");
                    }

                }
                return status;
            }
            catch (Exception)
            {

                throw;
            }

        }


        private void SetSuggestNameInDropdown()
        {

            try
            {
                // Loop through each row in the DataGridView
                for (int i = 0; i < dgRoomNames.Rows.Count; i++)
                {
                    // MessageBox.Show("selectedValue: 1" + dgRoomNames.Rows.Count);

                    string currentCellValue = dgRoomNames.Rows[i].Cells[0].Value.ToString().ToLower();
                    //MessageBox.Show("currentCellValue :" + currentCellValue);
                    DataGridViewComboBoxCell comboCell = dgRoomNames.Rows[i].Cells[1] as DataGridViewComboBoxCell;
                    //MessageBox.Show("RoomNameSuggestionList :" + RoomNameSuggestionList.Count);
                    string selectedValue = RoomNameSuggestionList
                        .Where(obj => obj.RoomName.ToLower() == currentCellValue)
                        .Select(obj => obj.RoomSuggestionName)
                        .FirstOrDefault();

                    SetRoomNameSuggestionCombo(selectedValue, comboCell);

                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message.ToString());
                throw;
            }
        }



        private void GetFurnituresElementIds(Room room, Document activedoc)
        {
            BoundingBoxXYZ bb = room.get_BoundingBox(null);

            Outline outline = new Outline(bb.Min, bb.Max);

            BoundingBoxIntersectsFilter filter
              = new BoundingBoxIntersectsFilter(outline);

            Autodesk.Revit.DB.Document doc = room.Document;

            // Todo: add category filters and other
            // properties to narrow down the results

            // what categories of family instances
            // are we interested in?

            BuiltInCategory[] bics = new BuiltInCategory[] {
                BuiltInCategory.OST_Furniture,
                BuiltInCategory.OST_PlumbingFixtures,
                BuiltInCategory.OST_SpecialityEquipment
              };

            LogicalOrFilter categoryFilter
              = new LogicalOrFilter(bics
                .Select<BuiltInCategory, ElementFilter>(
                  bic => new ElementCategoryFilter(bic))
                .ToList<ElementFilter>());

            FilteredElementCollector familyInstances
              = new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .WhereElementIsViewIndependent()
                .OfClass(typeof(FamilyInstance))
                .WherePasses(categoryFilter)
                .WherePasses(filter);

            int roomid = room.Id.IntegerValue;

            List<Element> a = new List<Element>();

            foreach (FamilyInstance fi in familyInstances)
            {

                if (null != fi.Room
                  && fi.Room.Id.IntegerValue.Equals(roomid))
                {
                    Debug.Assert(fi.Location is LocationPoint,
                      "expected all furniture to have a location point");

                    a.Add(fi);
                }
            }


            //if (room.Name.Contains("CONFERENCE"))
            //    using (Transaction xt = new Transaction(activedoc, "view Change"))
            //    {
            //        xt.Start();
            //        externalCommandData.Application.ActiveUIDocument.ActiveView.HideElements(familyInstances.ToElementIds());
            //    }
            //return ;
        }

        private void SetRoomNameSuggestionCombo(string comboValueType, DataGridViewComboBoxCell comboCell)
        {
            
            comboCell.Items.Clear();

            comboCell.Items.Add("Ablution Recreation");
            comboCell.Items.Add("AHU");
            comboCell.Items.Add("Applicable to Multiple Areas");
            comboCell.Items.Add("Arrival Area");
            comboCell.Items.Add("BMS & IT Rooms");
            comboCell.Items.Add("Bigfish-Room");
            comboCell.Items.Add("BreakoutArea");
            comboCell.Items.Add("Breakout Area");
            comboCell.Items.Add("Board Room");
            comboCell.Items.Add("Cafetaria");
            comboCell.Items.Add("Cloak Room");
            comboCell.Items.Add("Collab Areas");
            comboCell.Items.Add("ConferenceRoom");
            comboCell.Items.Add("Dishwash Area");
            comboCell.Items.Add("Dry Pantry");
            comboCell.Items.Add("Electrical Room");
            comboCell.Items.Add("Game Zone");
            comboCell.Items.Add("Glazing Wall");
            comboCell.Items.Add("Gym");
            comboCell.Items.Add("Hand Wash");
            comboCell.Items.Add("Hub Room");
            comboCell.Items.Add("Lab");
            comboCell.Items.Add("Live Kitchen");
            comboCell.Items.Add("Prayer Rooms (Male and Female)");
            comboCell.Items.Add("MdRoom");
            comboCell.Items.Add("Music Room");
            comboCell.Items.Add("NAP Room(Mothers Room)");
            comboCell.Items.Add("NOC Room");
            comboCell.Items.Add("Photo Studio");
            comboCell.Items.Add("Preheat Kitchen");
            comboCell.Items.Add("Reception");
            comboCell.Items.Add("Recording Room");
            comboCell.Items.Add("Repro");
            comboCell.Items.Add("Security Room");
            comboCell.Items.Add("Server Room");
            comboCell.Items.Add("Serving Area");
            comboCell.Items.Add("Shaft Areas");
            comboCell.Items.Add("Store Room");
            comboCell.Items.Add("Toilet");
            comboCell.Items.Add("Tuck Shops");
            comboCell.Items.Add("UPS & Battery Room");
            comboCell.Items.Add("Waiting Lounge");
            comboCell.Items.Add("Wet Pantry");
            comboCell.Items.Add("Workstation");
            comboCell.Items.Add("Yogaroom");


            comboCell.Value = Utility.PascalCase(comboValueType);

        }

        private void dgRoomNames_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgRoomNames_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
            e.Cancel = false;
        }


        private double GetLayoutSize()
        {
            try
            {
                double RoomSize = 0;
                var doc = externalCommandData.Application.ActiveUIDocument.Document;
                View activeView = doc.ActiveView;
                FilteredElementCollector room_collector = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms).WhereElementIsNotElementType();

                IList<ElementId> Rooms = room_collector.ToElementIds() as IList<ElementId>;

                foreach (ElementId eid in Rooms)
                {
                    //i++;

                    //if (i > 5) return;
                    Room R = doc.GetElement2(eid) as Room;
                    RoomSize = RoomSize + R.Area;
                }

                return RoomSize;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //RvtRoomUtility.DeleteColorScheme(externalCommandData.Application.ActiveUIDocument);
           // RvtRoomUtility.DeleteAllRooms(externalCommandData.Application.ActiveUIDocument);
        }

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    //RvtRoomUtility.DeleteColorScheme(externalCommandData.Application.ActiveUIDocument);
        //    RvtRoomUtility.DeleteAllRooms(externalCommandData.Application.ActiveUIDocument);
        //}
    }
}
