using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using boqtakeoff.core.Libraries;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using View = Autodesk.Revit.DB.View;
using BuiltInCategory = Autodesk.Revit.DB.BuiltInCategory;
using static boqtakeoff.core.RvtRoomUtility;

namespace boqtakeoff.core
{
    public partial class Main : System.Windows.Forms.Form
    {
        #region [Class Members]
        public string revit_project_name = "";
        public string selected_project_name = "";
        // public string selected_project_id = "";
        public string selected_project_key = "";

        public string Uom { get; set; }
        public string selected_project_id { get; set; }
        //public int STEP_SELECT_EXCEL_FILE = 1;
        //public int STEP_SELECT_PROJECT = 0;
        //public int STEP_PROCESS_COMPLETED = 0;

        //public string STEP_SELECT_EXCEL_FILE_STATUS = "";
        //public string STEP_SELECT_PROJECT_STATUS = "";
        //public string STEP_PROCESS_COMPLETED_STATUS = "";

        public string filePath { get; set; }
        public Hashtable project_uid_hash_project_details = new Hashtable();
        private Utility log = new Utility();

        public ExternalCommandData externalCommandData { get; set; }
        public Autodesk.Revit.ApplicationServices.Application rvtApp { get; set; }
        public string userAutodeskID { get; set; }

        string project_id = "";
        string merge_required = "";
        string project_name = "";
        string merge_version = "";
        Hashtable project_details = new Hashtable();

        Hashtable errorMappings = new Hashtable();

        #endregion
        public Main(ExternalCommandData _externalCommandData)
        {
            InitializeComponent();
            externalCommandData = _externalCommandData;
            rvtApp = externalCommandData.Application.Application;
            userAutodeskID = rvtApp.Username;
            //userId = rvtApp.LoginUserId;
            //LoadErrorMappingFromConfiguration();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                grpSelectProject.Visible = false;

                InitilizeWithResources();

                // bindFamilyGridWith();
                backgroundWorker1.RunWorkerAsync(2000);
                //backgroundWorker2.RunWorkerAsync(2000);
                pnlModelCategory.Visible = false;


                grpSelectProject.Visible = true;

                grpSelectProject.Left = grpSelectFile.Left;
                grpSelectProject.Top = grpSelectFile.Top + 70;


                grpButtonPanel.Left = grpSelectProject.Left;
                grpButtonPanel.Top = grpSelectProject.Top + 100;


                openFileDialog1.Title = "Browse Files";
                openFileDialog1.FilterIndex = 1;

                cmbProjects.Items.Add("please wait.. still fetching...");
                cmbProjects.Enabled = false;

                this.Height = 345;
                this.Width = 854;

                //cmdRoomLevel.Left = txtVersionLevel.Left;
                grdErrorgrid.AutoGenerateColumns = false;
                this.FormBorderStyle = FormBorderStyle.FixedSingle;
                grpErrorPanel.Height = 200;

                txtFileName.Left = cmbProjects.Left + 10;
                cmdRoomLevel.Left = cmbProjects.Left + 10;
                //btnFileBrowse.Left = txtFileName.Width + 20;


                gvCategory.EnableHeadersVisualStyles = false;

                dgRoomNames.EnableHeadersVisualStyles = false;

                gvCategory.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
                //FromArgb(147, 33, 30);
                gvCategory.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.Navy;
                gvCategory.ColumnHeadersDefaultCellStyle.Padding = new Padding(0, 5, 0, 5);

                //myDataGridView.Columns[col].HeaderCell.Style.BackColor = Color.Green;
                dgRoomNames.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
                dgRoomNames.ColumnHeadersDefaultCellStyle.Padding = new Padding(0, 5, 0, 5);
                //FromArgb(147, 33, 30);
                dgRoomNames.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.Navy;
                gvCategory.RowTemplate.Height = 35;
                dgRoomNames.RowTemplate.Height = 35;
                //SelectProject();
            }
            catch (Exception exp)
            {
                Utility.Logger(exp);
            }
        }

        private double GetLayoutSize()
        {
            try
            {
                double RoomSize = 0;
                var doc = externalCommandData.Application.ActiveUIDocument.Document;
                View activeView = doc.ActiveView;
                FilteredElementCollector room_collector = new FilteredElementCollector(doc, doc.ActiveView.Id).OfCategory(Autodesk.Revit.DB.BuiltInCategory.OST_Rooms).WhereElementIsNotElementType();

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

        class RoomNameSuggestion
        {
            public string RoomName { get; set; }
            //public IList<RoomTType> Suggestion { get; set; }
            public string Suggestion { get; set; }

        }

        private void BindNameSuggestionGrid()
        {
            this.dgRoomNames.DefaultCellStyle.Font = new Font("Arial", 9);
            var doc = externalCommandData.Application.ActiveUIDocument.Document;
            View activeView = doc.ActiveView;

            FilteredElementCollector room_collector = new FilteredElementCollector(doc, doc.ActiveView.Id)
                .OfCategory(Autodesk.Revit.DB.BuiltInCategory.OST_Rooms).WhereElementIsNotElementType();

            IList<ElementId> room_eids = room_collector.ToElementIds() as IList<ElementId>;
            string rNames = string.Empty;


            string Message = string.Empty;

            string RoomSuggestionName = string.Empty;
            int i = 0;
            string Details = string.Empty;
            double layoutsize = GetLayoutSize();
            foreach (ElementId eid in room_eids)
            {

                Room R = doc.GetElement2(eid) as Room;
                RoomNameSuggestion roomNameItem = new RoomNameSuggestion();

                RoomSuggestionName = GetFurniture(R, layoutsize);

                if (RoomSuggestionName == "")
                {
                    RoomSuggestionName = "Bigfish-Room";
                }

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

            //textBox1.Text = MyProperty;
            RoomNameSuggestions = RoomNameSuggestions.GroupBy(z => z.RoomName).Select(x => x.First()).ToList();


            dgRoomNames.Columns.Clear();
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


            dgRoomNames.AutoGenerateColumns = false;

            dgRoomNames.DataSource = RoomNameSuggestions.OrderBy(x => x.RoomName).ToList();

            SetSuggestNameInDropdown();
            //textBox1.Text = roomlist;
        }
        class RoomTSuggestion
        {
            public string RoomName { get; set; }
            public string RoomSuggestionName { get; set; }
        }

        public string MyProperty { get; set; }
        IList<RoomTSuggestion> RoomNameSuggestionList = new List<RoomTSuggestion>();
        IList<RoomNameSuggestion> RoomNameSuggestions = new List<RoomNameSuggestion>();
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

                Autodesk.Revit.DB.BuiltInCategory[] bics = new Autodesk.Revit.DB.BuiltInCategory[] {
                Autodesk.Revit.DB.BuiltInCategory.OST_Furniture,
                Autodesk.Revit.DB.BuiltInCategory.OST_PlumbingFixtures,
                Autodesk.Revit.DB.BuiltInCategory.OST_SpecialityEquipment
              };

                LogicalOrFilter categoryFilter
                  = new LogicalOrFilter(bics
                    .Select<Autodesk.Revit.DB.BuiltInCategory, ElementFilter>(
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
                //String mes = "";
                MyProperty += "@@@@" + room.Name + "-->>";
                foreach (FamilyInstance fi in familyInstances)
                {
                    if (null != fi.Room && fi.Room.Id.IntegerValue.Equals(roomid))
                    {
                        if (!fi.Name.ToString().ToLower().Equals("finish_anchor") && fi.Name.ToString().ToLower().Equals("divider_top") == false)
                        { MyProperty += fi.Name + "#"; }
                        //Debug.Assert(fi.Location is LocationPoint, "expected all furniture to have a location point");
                        a.Add(fi);
                    }
                }

                /* Get Room Area */

                Parameter par = room.get_Parameter(BuiltInParameter.ROOM_AREA);
                string valSting = par.AsValueString();
                double roomsize = par.AsDouble(); //mind the value is in native Revit units, not project units. So square feet in this case

                /* ------------------ */
                SuggestedRoomName = RvtRoomUtility.SuggestRoomName(a, room.Name, roomsize, layoutSize);
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



        private void SetSuggestNameInDropdown()
        {
            try
            {
                for (int i = 0; i < dgRoomNames.Rows.Count; i++)
                {
                    string currentCellValue = dgRoomNames.Rows[i].Cells[0].Value.ToString().ToLower();
                    DataGridViewComboBoxCell comboCell = dgRoomNames.Rows[i].Cells[1] as DataGridViewComboBoxCell;
                    //MessageBox.Show("RoomNameSuggestionList :" + RoomNameSuggestionList.Count);
                    string selectedValue = RoomNameSuggestionList
                        .Where(obj => obj.RoomName.ToLower() == currentCellValue)
                        .Select(obj => obj.RoomSuggestionName)
                        .FirstOrDefault();

                    currentCellValue = Utility.PascalCase(currentCellValue);
                    selectedValue = Utility.PascalCase(selectedValue);

                    //MessageBox.Show("currentCellValue: " + Utility.PascalCase(currentCellValue) + "\r\nselectedValue :" + Utility.PascalCase(selectedValue));
                    if (currentCellValue.Contains(selectedValue))
                    {
                        //dgRoomNames.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightSeaGreen;
                    }
                    else
                    {

                        XmlDocument xDoc = new XmlDocument();
                        string xmlfilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Resources\\settings.xml";
                        xDoc.Load(xmlfilePath);

                        XmlNodeList roomTypes = xDoc.SelectNodes("//RoomCategory//item");
                        // MessageBox.Show("roomTypes count" + roomTypes.Count);
                        foreach (XmlNode roomType in roomTypes)
                        {
                            if (currentCellValue.Contains(Utility.PascalCase(roomType.Attributes["name"].Value)) && selectedValue == "Bigfish-Room")
                            {
                                selectedValue = Utility.PascalCase(roomType.Attributes["name"].Value);
                                //dgRoomNames.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightYellow;
                            }
                        }

                    }
                    SetRoomNameSuggestionCombo(selectedValue, currentCellValue, comboCell);
                    //MessageBox.Show("currentCellValue: " + Utility.PascalCase(currentCellValue) + "\r\nselectedValue :" + Utility.PascalCase(selectedValue));


                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message.ToString());
            }
        }

        private void SetRoomNameSuggestionCombo(string comboValueType, string RoomName, DataGridViewComboBoxCell comboCell)
        {

            comboCell.Items.Clear();
            XmlDocument xDoc = new XmlDocument();
            string xmlfilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Resources\\settings.xml";
            xDoc.Load(xmlfilePath);

            RoomValidateMatrixClass roomValidateMatrix = new RoomValidateMatrixClass();

            XmlNodeList roomTypes = xDoc.SelectNodes("//RoomCategory//item");
            // MessageBox.Show("roomTypes count" + roomTypes.Count);
            foreach (XmlNode roomType in roomTypes)
            {
                comboCell.Items.Add(roomType.Attributes["name"].Value);
            }



            comboCell.Value = Utility.PascalCase(comboValueType);

        }
        private void InitilizeWithResources()
        {
            openFileDialog1.Filter = Properties.Resources.FileTypes;

            lblSelectfile.Text = Properties.Resources.Step1_select_file_label;
            lblSelectProject.Text = Properties.Resources.Step2_select_project_label;
            lblVersionLabel.Text = Properties.Resources.Step3_select_version_label;

            linkDownload.Text = Properties.Resources.DownloadTemplateTitle;

        }
        /// <summary>
        /// 
        /// </summary>
        public void FillProjectDetails()
        {
            try
            {
                // Get the Project Unique names
                string access_token = BigFishRestAPIs.get_access_token();
                List<getProjectDetails> project_uid_list = BigFishRestAPIs.getProjectNameList(access_token);
                // Add the list to the combo box

                List<string> project_name_list = new List<string>();
                cmbProjects.Items.Clear();
                cmbProjects.Enabled = true;
                cmbProjects.Items.Insert(0, "--Please select---");
                foreach (getProjectDetails curr_project_data in project_uid_list)
                {
                    cmbProjects.Items.Add(curr_project_data.project_name);
                    project_name_list.Add(curr_project_data.project_name);

                    if (!project_uid_hash_project_details.ContainsKey(curr_project_data.project_name))
                    {
                        Hashtable projectDetails = new Hashtable();
                        projectDetails.Add("project_id", curr_project_data.project_id);
                        projectDetails.Add("project_key", curr_project_data.project_key);
                        project_uid_hash_project_details.Add(curr_project_data.project_name, projectDetails);
                    }
                }
                //SM-1225: Qualcomm Incorporated I Noida I D&B I 2024
                revit_project_name = BOQExtractor.getProjectInformationFromRevitModel();
                cmbProjects.SelectedIndex = 0;
                if (revit_project_name != "")
                {
                    if (project_name_list.Contains(revit_project_name))
                    {
                        selected_project_id = revit_project_name;
                        cmbProjects.SelectedItem = revit_project_name;
                    }
                }
            }
            catch (Exception exp)
            {
                Utility.Logger(exp);
            }
        }

        private void PopulateRoomLevels()
        {
            var activeUidoc = externalCommandData.Application.ActiveUIDocument;
            Hashtable roomLevels = RvtRoomUtility.GetRoomLevelDetail(activeUidoc);

            cmdRoomLevel.Items.Clear();
            cmdRoomLevel.DisplayMember = "value";
            cmdRoomLevel.ValueMember = "key";
            foreach (var item in roomLevels)
            {
                cmdRoomLevel.Items.Add(item);
            }
            cmdRoomLevel.SelectedIndex = 0;
        }
        private void btnNext_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbProjects.SelectedIndex < 1)
                {
                    string exception_message = Properties.Resources.Step2_Empty_Project_Message;
                    System.Windows.Forms.MessageBox.Show(exception_message, "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (txtVersionLevel.Text.Trim().Length < 1)
                {
                    System.Windows.Forms.MessageBox.Show(Properties.Resources.Step3_empty_version_lable, "Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtVersionLevel.Focus();
                    return;
                }
                if (cmbProjects.SelectedIndex > 1 && btnNext.Text.Contains("Next") == true)
                {
                    //System.Windows.Forms.MessageBox.Show("Message 1");
                  
                  

                    pnlRoom.Visible = false;
                    //this.Height = 800;
                    //this.Width = 854;

                    grpSelectProject.Visible = false;
                    grpSelectFile.Visible = false;
                    pnlRoom.Visible = false;
                    pnlModelCategory.Visible = true;

                    pnlModelCategory.Left = 2;
                    pnlModelCategory.Top = grpExcelModel.Top + 50;
                    grpButtonPanel.Top = pnlModelCategory.Top + 410;
                    LoadCategoryGrid();
                    this.Width = 854;
                    this.Height = 600;
                    btnNext.Text = "Finish";

                }
                else if (btnNext.Text.Equals("Finish") == true)
                {
                    if ((File.Exists(filePath) == false) && rdoExcel.Checked == true)
                    {
                        string exception_message = "Please Select File";
                        System.Windows.Forms.MessageBox.Show(exception_message, "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (cmbProjects.SelectedIndex < 1)
                    {
                        string exception_message = Properties.Resources.Step2_Empty_Project_Message;
                        System.Windows.Forms.MessageBox.Show(exception_message, "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (txtVersionLevel.Text.Trim().Length < 1)
                    {
                        System.Windows.Forms.MessageBox.Show(Properties.Resources.Step3_empty_version_lable, "Information", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtVersionLevel.Focus();
                        return;
                    }
                    if (rdoModel.Checked == true && btnNext.Text.Equals("Finish") == true)
                    {

                        btnNext.Visible = false;
                        btnBack.Visible = false;

                        //Hashtable selectedValues = new Hashtable();
                        //selectedValues = GetFamilyGridValues();

                        // List<BoqLineItemDetails> _BoqLineItemDetails = ExtractModelDetails.GetBOQDetailsHashtable(externalCommandData.Application.ActiveUIDocument.Document, selectedValues);

                        selected_project_name = cmbProjects.SelectedItem.ToString();
                        project_details = (Hashtable)project_uid_hash_project_details[selected_project_name];
                        project_name = selected_project_name;
                        project_id = (string)project_details["project_id"];

                        string project_key = (string)project_details["project_key"];

                        // INFO::Update project information, if the project name inside Revit and the selected project name are different
                        if (selected_project_name != revit_project_name)
                        {
                            var ret_string = BOQExtractor.updateProjectInformation(project_name, project_key);
                        }

                        string version_label = txtVersionLevel.Text;
                        string version_number = project_key + "_" + version_label + "_" + DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "");

                        // INFO:: Need the categories to be exported in a list.
                        // INFO:: uom can be metric or british


                        List<string> values = GetCheckedRowFromCategoryGrid();

                        List<BoqLineItemDetails> _BoqLineItemDetails = MaterialQuantitiesMain.execMaterialQuantities(externalCommandData.Application.ActiveUIDocument.Document, values, version_number, Uom);


                        // TaskDialog _taskDialog = Utility.ShowTaskDialogMessageWithProgressBar("Please wait dear", "Progress");
                        List<getErrorLog> error_log_json = BigFishRestAPIs.PostBOQDetails2ZCreator(_BoqLineItemDetails, project_name, project_key,
                                project_id, version_number, merge_required, merge_version, userAutodeskID);

                        if (error_log_json.Count > 0)
                        {
                            grdErrorgrid.Visible = true;
                            // _taskDialog.Dispose();
                            grdErrorgrid.DataSource = error_log_json;

                            grpErrorPanel.Visible = true;
                            grdErrorgrid.Visible = true;
                            btnNext.Visible = false;
                            btnBack.Visible = false;
                        }
                        else
                        {
                            //_taskDialog.Dispose();
                            Utility.ShowMessage("BOQ Extraction completed Successfully", "Message", "information", "message");
                            this.Close();
                            return;
                        }
                        Utility.ShowMessage("BOQ Extraction completed Successfully", "Message", "information", "message");
                        this.Close();
                        return;
                    }
                    else if (rdoExcel.Checked == true)
                    {
                        //grdErrorgrid.Columns.Clear();
                        btnBack.Visible = false;
                        btnNext.Visible = false;
                        selected_project_name = cmbProjects.SelectedItem.ToString();
                        project_details = (Hashtable)project_uid_hash_project_details[selected_project_name];
                        project_name = selected_project_name;
                        project_id = (string)project_details["project_id"];

                        string project_key = (string)project_details["project_key"];

                        // INFO::Update project information, if the project name inside Revit and the selected project name are different
                        if (selected_project_name != revit_project_name)
                        {
                            var ret_string = BOQExtractor.updateProjectInformation(project_name, project_key);
                        }

                        string version_label = txtVersionLevel.Text;
                        string version_number = project_key + "_" + version_label + "_" + DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "");

                        List<BoqLineItemDetails> create_boq_details = CSVParser.GetBOQDetailsHashtable_msxp_new(filePath, project_id, version_number);
                        //MessageBox.Show("Call I am on right place 3");
                        if (create_boq_details[0].error_log.Count > 0)
                        {
                            //MessageBox.Show("create_boq_details Error found");
                            grdErrorgrid.Visible = true;
                            grdErrorgrid.DataSource = create_boq_details[0].error_log;

                            string errorMessage = string.Empty;

                            Int32 i = 1;
                            foreach (getErrorLog item in create_boq_details[0].error_log)
                            {
                                errorMessage += i.ToString() + "- SKU -" + item.sku_label + " (Error " + item.error_log + ")";
                                i++;
                            }

                            Utility.Logger(null, errorMessage, "ValidationError");
                            grpErrorPanel.Visible = true;
                            grdErrorgrid.Visible = true;
                            btnNext.Visible = false;
                            btnBack.Visible = false;
                            btnCancel.Visible = true;
                            grpButtonPanel.Visible = true;
                            ///grpButtonPanel.Top = grpErrorPanel.Top + grdErrorgrid.Height - 100;
                        }
                        else
                        {
                            List<getErrorLog> error_log_json = BigFishRestAPIs.PostBOQDetails2ZCreator(create_boq_details, project_name, project_key,
                                project_id, version_number, merge_required, merge_version, userAutodeskID);
                            if (error_log_json.Count > 0)
                            {

                                grdErrorgrid.Visible = true;
                                grdErrorgrid.DataSource = error_log_json;

                                string errorMessage = string.Empty;
                                Int32 i = 1;
                                foreach (getErrorLog item in error_log_json)
                                {
                                    errorMessage += i.ToString() + "- SKU -" + item.sku_label + " (Error " + item.error_log + ")";
                                    i++;
                                }

                                Utility.Logger(null, errorMessage, "ValidationError");
                                //this.Height = grdErrorgrid.Height + 100;
                                grpErrorPanel.Visible = true;
                                grdErrorgrid.Visible = true;

                                grpButtonPanel.Visible = true;
                                btnNext.Visible = false;
                                btnBack.Visible = false;
                                btnCancel.Visible = true;
                            }
                            else
                            {
                                Utility.ShowMessage("File Processed Sucessfully", "Completed", "information", "task");
                                this.Close();
                                return;
                            }
                        }
                        grpSelectFile.Visible = false;
                        pnlModelCategory.Visible = false;
                        grpSelectProject.Visible = false;
                        grpExcelModel.Visible = false;

                        grpErrorPanel.Top = grpExcelModel.Top;
                        grpErrorPanel.Left = grpExcelModel.Left;

                        grpButtonPanel.Top = grpErrorPanel.Top + grpErrorPanel.Height;
                        grpButtonPanel.Width = grpErrorPanel.Width;
                    }
                }
                ////=========================================================================================
            }
            catch (Exception exp)
            {
                Utility.Logger(exp);
            }
        }

        private class CategoryGridClass
        {
            public string Category { get; set; }
            public string BuiltInCategory { get; set; }
            public string CategoryValue { get; set; }
        }
        private void LoadCategoryGrid()
        {

            XmlDocument xDoc = new XmlDocument();
            string xmlfilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Resources\\settings.xml";
            xDoc.Load(xmlfilePath);
            string nn = "aa";
            XmlNodeList categoryItems = xDoc.SelectNodes("//annotate/item");
            gvCategory.DataSource = null;
            IList<CategoryGridClass> categoryGridClasses = new List<CategoryGridClass>();
            CategoryGridClass categoryItem = new CategoryGridClass();
            foreach (XmlNode item in categoryItems)
            {
                categoryGridClasses.Add(new CategoryGridClass { Category = item.Attributes["Category"].Value, BuiltInCategory = item.Attributes["BuiltInCategory"].Value, CategoryValue = item.Attributes["CategoryValue"].Value });
            }
            gvCategory.AutoGenerateColumns = false;
            gvCategory.DataSource = categoryGridClasses;

            //MessageBox.Show("categoryGridClasses Count " + categoryGridClasses.Count + "---" + nn);

        }
        private void btnBack_Click(object sender, EventArgs e)
        {
            grpExcelModel.Visible = true;
            grpSelectFile.Visible = true;
            grpSelectProject.Visible = true;

            pnlModelCategory.Visible = false;
            grpErrorPanel.Visible = false;
        }

        private void btnFileBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    filePath = openFileDialog1.FileName;
                    txtFileName.Text = filePath;
                    //STEP_SELECT_EXCEL_FILE_STATUS = String.Format("Input File is begin parsed Successfully!!");
                }
            }
            catch (Exception exp)
            {
                Utility.Logger(exp);
            }
        }

        private void linkDownload_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                CreateTemplateExcelForSample();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("An error occurred: " + ex.Message);
                Utility.Logger(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker helperBW = sender as BackgroundWorker;
            int arg = (int)e.Argument;
            e.Result = BackgroundProcessLogicMethod(helperBW, arg);
            if (helperBW.CancellationPending)
            {
                e.Cancel = true;
            }
        }

        private int BackgroundProcessLogicMethod(BackgroundWorker bw, int a)
        {
            int result = 0;
            FillProjectDetails();

            return result;
        }
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        private void CreateTemplateExcelForSample()
        {

            Microsoft.Office.Interop.Excel.Application excel;
            Microsoft.Office.Interop.Excel.Workbook worKbooK;
            Microsoft.Office.Interop.Excel.Worksheet worKsheeT;
            try
            {
                int fontSize = 10;
                excel = new Microsoft.Office.Interop.Excel.Application();
                excel.Visible = false;
                excel.DisplayAlerts = false;
                worKbooK = excel.Workbooks.Add(Type.Missing);


                worKsheeT = (Microsoft.Office.Interop.Excel.Worksheet)worKbooK.ActiveSheet;
                worKsheeT.Name = "Template";

                worKsheeT.Cells[1, 1] = "Item Name";
                worKsheeT.Cells.Font.Size = fontSize;

                worKsheeT.Cells[1, 2] = "Item SKU";
                worKsheeT.Cells.Font.Size = fontSize;

                worKsheeT.Cells[1, 3] = "Item Description";
                worKsheeT.Cells.Font.Size = fontSize;

                worKsheeT.Cells[1, 4] = "Quantity";
                worKsheeT.Cells.Font.Size = fontSize;

                worKsheeT.Cells[1, 5] = "UOM";
                worKsheeT.Cells.Font.Size = fontSize;

                worKsheeT.Cells[1, 6] = "Floor Name";
                worKsheeT.Cells.Font.Size = fontSize;

                worKsheeT.Cells[1, 7] = "Family Category";
                worKsheeT.Cells.Font.Size = fontSize;

                string userRoot = System.Environment.GetEnvironmentVariable("USERPROFILE");
                //string downloadFolder = Path.Combine(userRoot, "Downloads\\BOQ Template.xlsx");
                string downloadFolder = Path.Combine(userRoot, Properties.Resources.DownloadFolder);

                System.Windows.Forms.MessageBox.Show("Template has been downloaded successfully!! to " + downloadFolder, "Information", MessageBoxButtons.OK
                    , MessageBoxIcon.Information);

                worKbooK.SaveAs(downloadFolder);
                worKbooK.Close();
                excel.Quit();
            }
            catch (Exception ex)
            {
                Utility.Logger(ex);
            }
            finally
            {
                worKsheeT = null;
                worKbooK = null;
            }
        }


        private void rdoExcel_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoExcel.Checked == false)
            {
                rdoModel.Checked = true;
                grpSelectFile.Visible = false;
                pnlModelCategory.Visible = false;

                pnlModelCategory.Left = grpSelectFile.Left;
                pnlModelCategory.Top = grpSelectFile.Top;
                grpSelectProject.Visible = false;

                grpButtonPanel.Left = pnlRoom.Left;
                grpButtonPanel.Top = pnlRoom.Top + 100;
                pnlRoom.Visible = true;

                btnNext.Text = "Next";
                this.Height = 694;
                //this.Width = 1726;
            }
            else
            {
                btnNext.Text = "Finish";
                //lblModelMessage.Visible = false;
                pnlRoom.Visible = false;
                rdoModel.Checked = false;
                grpSelectFile.Visible = true;
                pnlModelCategory.Visible = false;

                grpSelectProject.Visible = true;

                grpSelectProject.Left = grpSelectFile.Left;
                grpSelectProject.Top = grpSelectFile.Top + 70;

                grpButtonPanel.Visible = true;
                grpButtonPanel.Left = grpSelectProject.Left;
                grpButtonPanel.Top = grpSelectProject.Top + 93;
                this.Height = 338;
                this.Width = 854;

                //this.Height = 350;
            }
        }

        private void rdoModel_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoModel.Checked == false)
            {
                rdoExcel.Checked = true;
                grpSelectFile.Visible = true;
                pnlModelCategory.Visible = false;
                grpButtonPanel.Visible = true;
                pnlRoom.Visible = false;
                btnNext.Text = "Finish";
                this.Height = 338;
                this.Width = 854;
            }
            else
            {
                btnNext.Text = "Next";
                pnlRoom.Visible = true;
                rdoExcel.Checked = false;
                grpSelectFile.Visible = false;
                pnlModelCategory.Visible = false;
                pnlModelCategory.Left = 2;
                pnlModelCategory.Top = grpSelectFile.Top;

                grpSelectProject.Visible = true;

                //grpExcelModel
                grpSelectProject.Left = grpButtonPanel.Left;
                grpSelectProject.Top = grpExcelModel.Top + 93;

                pnlRoom.Top = grpSelectProject.Top + 80;
                grpButtonPanel.Top = pnlRoom.Top + 400;
                //this.Height = 650;
                PopulateRoomLevels();
                BindNameSuggestionGrid();
                //backgroundWorker2.RunWorkerAsync(1000);
                this.Height = 694;
                //this.Width = 1726;
            }
        }

        private void rdoMetric_CheckedChanged(object sender, EventArgs e)
        {
            Uom = "Metric";
        }

        private void rdoBritish_CheckedChanged(object sender, EventArgs e)
        {
            Uom = "British";
        }

        private void grpSelectFile_Enter(object sender, EventArgs e)
        {

        }

        private void Main_Paint(object sender, PaintEventArgs e)
        {

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
                    //string _level_element_id = Convert.ToString(((System.Collections.DictionaryEntry)cmdRoomLevel.SelectedItem).Key);
                    ////MessageBox.Show("_level_element_id-: " + _level_element_id);
                    //var activeUidoc = externalCommandData.Application.ActiveUIDocument;
                    //ImageFilePath = RvtRoomUtility.SaveImage(activeUidoc, _level_element_id);
                    //pnlPreview.BackgroundImage = System.Drawing.Image.FromFile(ImageFilePath);
                    //pnlPreview.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
                }
            }
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

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker helperBW = sender as BackgroundWorker;
            int arg = (int)e.Argument;
            e.Result = BackgroundProcessLogicMethod2(helperBW, arg);
            if (helperBW.CancellationPending)
            {
                e.Cancel = true;
            }
        }
        private int BackgroundProcessLogicMethod2(BackgroundWorker bw, int a)
        {
            int result = 0;
            LoadImage();

            return result;
        }


        private void LoadImage()
        {
            try
            {
                string _level_element_id = Convert.ToString(((System.Collections.DictionaryEntry)cmdRoomLevel.SelectedItem).Key);
                //MessageBox.Show("_level_element_id-: " + _level_element_id);
                var activeUidoc = externalCommandData.Application.ActiveUIDocument;
                ImageFilePath = RvtRoomUtility.SaveImage(activeUidoc, _level_element_id);
                //MessageBox.Show("ImageFilePath-: " + ImageFilePath);
                pnlPreview.BackgroundImage = System.Drawing.Image.FromFile(ImageFilePath);
                pnlPreview.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            }
            catch (Exception exp)
            {
                Utility.Logger(exp);
            }
        }

        private List<string> GetCheckedRowFromCategoryGrid()
        {
            //Cells[3] =  Category Key Valye Hidden
            IList<string> Categories = new List<string>();
            try
            {
                foreach (DataGridViewRow item in gvCategory.Rows)
                {
                    if (Convert.ToBoolean(item.Cells[0].Value) == true)
                    {
                        Categories.Add(item.Cells[3].Value.ToString());
                    }
                }
            }
            catch (Exception exp)
            {
                Utility.Logger(exp);
            }
            return Categories.ToList();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow item in gvCategory.Rows)
            {
                if (Convert.ToBoolean(item.Cells[0].Value) == true)
                {
                    MessageBox.Show(item.Cells[3].Value.ToString());
                }
            }
        }

        private void btnViewImage_Click(object sender, EventArgs e)
        {
            LoadImage();
            this.Width = 1726;
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void gvCategory_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgRoomNames_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
            e.Cancel = false;
        }
    }
}