using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using boqtakeoff.core.Libraries;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Document = Autodesk.Revit.DB.Document;
using MessageBox = System.Windows.Forms.MessageBox;
using View = Autodesk.Revit.DB.View;

namespace boqtakeoff.core
{
    internal class RvtRoomUtility
    {

        public class FloorWithLevel
        {
            public View RoomPlanView { get; set; }
            public string LevelId { get; set; }
            public string LevelName { get; set; }
        }
        // Function gives a hash of level names and ids at various floor in a model
        public static IList<FloorWithLevel> GetFloorPlanAssociatedlevelDetails(UIDocument activeUIDocument, string key_type)
        {
            Document document = activeUIDocument.Document;
            ElementId viewfamily_elem_type_id = (from elem in new FilteredElementCollector(document).OfClass(typeof(ViewFamilyType))
                                                 let type = elem as ViewFamilyType
                                                 where type.ViewFamily == ViewFamily.FloorPlan
                                                 select type).First().Id;

            IList<FloorWithLevel> floorWithLevels = new List<FloorWithLevel>();

            ElementClassFilter filter = new ElementClassFilter(typeof(View));
            FilteredElementCollector collector = new FilteredElementCollector(document);
            collector.WherePasses(filter);
            List<Element> viewElement_list = (from element in collector where element.GetTypeId() == viewfamily_elem_type_id select element).ToList();

            //TaskDialog.Show("viewfamily_elem_type_id ... ", viewfamily_elem_type_id.ToString());
            //TaskDialog.Show("viewElement_list ... ", viewElement_list.Count().ToString());

            //TaskDialog.Show("Before starting transaction ... ", "Before starting transaction");
            List<string> associatedLevelIdList = new List<string>();
            Hashtable levelNameHashLevelID = AnnotateElements.GetLevelNameHashLevelId(document);
            Hashtable mapVar = new Hashtable();

            foreach (View view_family_type_elem in viewElement_list)
            {
                ElementId view_family_type_elem_id = view_family_type_elem.Id;


                View view = view_family_type_elem as View;
                //TaskDialog.Show("view ... ", view.Name.ToString());
                activeUIDocument.Application.ActiveUIDocument.ActiveView = view;

                Level active_view_level = view.GenLevel;
                ElementId Level_element_id = active_view_level.Id;

                if (!associatedLevelIdList.Contains(Level_element_id.ToString()))
                {
                    string level_name = active_view_level.Name;
                    associatedLevelIdList.Add(Level_element_id.ToString());
                    if (key_type == "level_id")
                    {
                        floorWithLevels.Add(new FloorWithLevel
                        {
                            LevelId = Level_element_id.ToString(),
                            LevelName = active_view_level.Name,
                            RoomPlanView = view
                        });
                        mapVar.Add(Level_element_id.ToString(), level_name);
                        // TaskDialog.Show("activeUIDocument.Application.ActiveUIDocument.ActiveView", activeUIDocument.Application.ActiveUIDocument.ActiveView.Name, TaskDialogCommonButtons.Ok, TaskDialogResult.Ok);
                    }
                    else if (key_type == "level_name")
                    {
                        mapVar.Add(level_name, Level_element_id.ToString());
                        floorWithLevels.Add(new FloorWithLevel
                        {
                            LevelId = Level_element_id.ToString(),
                            LevelName = active_view_level.Name,
                            RoomPlanView = view
                        });
                    }
                }
            }
            return floorWithLevels;
        }

        public class RoomValidateMatrixClass
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
        public static string SuggestRoomName(List<Element> furnitures, string layoutRoomName, double roomsize = 400, double layoutsize = 500)
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

                bool isPriorityAvailable = false;
                /* Need to handle scenario, where element belongs to multiple groups/family */
                if (_roomValidateMatrixClass.Count > 0)
                {
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
            }
            catch (Exception ex)
            {
                Utility.Logger(ex);
                throw;
            }
        }

        public class RoomCount
        {
            public string RoomName { get; set; }
            public int count { get; set; }
            public double layoutsize { get; set; }
            public double roomSize { get; set; }
            public double minValue { get; set; }
            public double maxValue { get; set; }

        }

        public static string FindMaxAvaialableElement(IList<RoomValidateMatrixClass> roomValidateMatrixClass, XmlNodeList selecteRoomTypes)
        {
            try
            {
                IList<RoomCount> roomCount = new List<RoomCount>();
                RoomValidateMatrixClass selectedRow = new RoomValidateMatrixClass();
                int rCount = 1;
                double _roomSize = 0;
                string RoomName = string.Empty;
                foreach (XmlNode item in selecteRoomTypes)
                {
                    rCount = 1;
                    foreach (RoomValidateMatrixClass CItem in roomValidateMatrixClass)
                    {
                        if (item.Value.ToString().ToLower().Equals(CItem.roomName.ToLower()))
                        {
                            selectedRow = CItem;
                            rCount++;
                        }
                    }
                    roomCount.Add(new RoomCount { count = rCount, RoomName = item.Value.ToString(), roomSize = selectedRow.roomSize, layoutsize = selectedRow.layoutSize, minValue = selectedRow.minValue, maxValue = selectedRow.maxValue });
                }

                IList<RoomCount> SelectedroomCount = roomCount.OrderByDescending(x => x.count).ToList();
                double roomSizeCalculated = (SelectedroomCount[0].roomSize / SelectedroomCount[0].layoutsize) * 100;
             

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


        public static Hashtable GetRoomLevelDetail(UIDocument activeUIDocument)
        {
            Document document = activeUIDocument.Document;

            StringBuilder levelInformation = new StringBuilder();
            StringBuilder RoomInformation = new StringBuilder();
            Hashtable levelIdHashLevelDetails = new Hashtable();
            Hashtable roomIdHashroomDetails = new Hashtable();

            int levelNumber = 0;
            FilteredElementCollector collector = new FilteredElementCollector(document);
            IOrderedEnumerable<Level> collection = collector.OfClass(typeof(Level)).Cast<Level>().OrderBy(lev => lev.Elevation).OrderBy(s => s.Elevation);
            // Creating a new hash table
            // 

            foreach (Level level in collection)
            {
                Hashtable levelDetails = new Hashtable();
                if (null != level)
                {
                    // keep track of number of levels
                    levelNumber++;
                    levelDetails.Add("name", level.Name);

                    levelDetails.Add("elevation", level.Elevation);
                    levelDetails.Add("project_elevation", level.ProjectElevation);

                    ElementLevelFilter levelIdFilter = new ElementLevelFilter(level.Id);
                    collector = new FilteredElementCollector(document);
                    ICollection<Element> SpatialElementAtCurrLevel = collector.OfClass(typeof(SpatialElement)).WherePasses(levelIdFilter).ToElements();
                    if (SpatialElementAtCurrLevel.Count > 0)
                    {
                        //TaskDialog.Show("level.Name: ", level.Name.ToString());
                        //get the name of the level
                        levelInformation.Append("\nLevel Name: " + level.Name);
                        //get the elevation of the level
                        levelInformation.Append("\n\tElevation: " + level.Elevation);
                        levelInformation.Append("\n\tLevel Id: " + level.Id);

                        // TaskDialog.Show("Infor", "\nLevel Name: " + level.Name + "\n\tLevel Id: " + level.Id + "\n\t level.Elevation: " + level.Elevation);
                        roomIdHashroomDetails.Add(level.Id, level.Name);
                    }
                }
            }
            //number of total levels in current document
            return roomIdHashroomDetails;
        }
        // INFO:: For the circuits that dont have the room object, the function creates the room object
        // INFO:: The function takes the level element id and the document as the input.
        public static string CreateRooms(ElementId Level_element_id, Document document)
        {
            try
            {
                string status = "Failed";
                var roomNamePattern = Properties.Resources.RoomPattern.ToString();
                string transLevel = "transaction" + DateTime.Now.Millisecond.ToString();
                Element level_element = document.GetElement(Level_element_id);
                //  TaskDialog.Show("level_element ... ", level_element.ToString());
                Level level = level_element as Level;

                PlanTopology topology = document.get_PlanTopology(level);
                //TaskDialog.Show("Plan Topology ... ", topology.ToString());
                PlanCircuitSet circuitSet = topology.Circuits;

                //TaskDialog.Show("level ... ", Level_element_id.ToString());
                //TaskDialog.Show("before topology - Level Name ... ", level.Name.ToString()); 

                using (Transaction tran = new Transaction(document))
                {
                    tran.Start(transLevel);

                    //TaskDialog.Show("CircuitSet ... ", circuitSet.Size.ToString());
                    try
                    {
                        int x = 1;
                        foreach (PlanCircuit circuit in circuitSet)
                        {
                            if (!circuit.IsRoomLocated)
                            {
                                Room room = document.Create.NewRoom(null, circuit);
                                //TaskDialog.Show("room ... ", room.ToString());
                                room.Name = roomNamePattern + x;

                                //room.Level.i
                                x++;
                            }
                        }
                        status = "success";
                    }
                    catch (Exception error)
                    {
                        TaskDialog.Show("Error while creating room .. ", error.Message.ToString());
                    }
                    finally
                    {
                        tran.Commit();
                        tran.Dispose();
                    }
                }
                //TaskDialog.Show("Created Successfully", "Created Successfully");
                return status;
            }
            catch (Exception exp)
            {
                Utility.Logger(exp);
                throw;
            }
        }

        public static string CreateRoomWithColorFillandPreview(UIDocument activeUIDocument, string _level_element_id)
        {
            string createdView_id = string.Empty;
            try
            {
                string RoomName = string.Empty;

                var activeUidoc = activeUIDocument.Application.ActiveUIDocument.Document;
                ElementId _ElementId = Autodesk.Revit.DB.ElementId.Parse(_level_element_id);
                //MessageBox.Show("Called CreateRoomWithColorFillandPreview - _level_element_id: 123");

                //FindAndActiveViewByLevelId(activeUIDocument, _level_element_id);
                IList<FloorWithLevel> viewCollection = GetFloorPlanAssociatedlevelDetails(activeUIDocument, "level_id");

                foreach (FloorWithLevel view in viewCollection)
                {
                    activeUIDocument.Application.ActiveUIDocument.ActiveView = view.RoomPlanView;
                }
                //RvtRoomUtility.DeleteAllRooms(activeUIDocument);
                string result = RvtRoomUtility.CreateRooms(_ElementId, activeUidoc);
                if (result == "success")
                {
                    createdView_id = CreateRoomPlanView(activeUIDocument, _level_element_id);
                    GetColorSchemeName(activeUIDocument);
                    string roomTagRespose = CreateRoomTags(activeUIDocument.Document, createdView_id, _ElementId);
                    TurnOffAllGraphics(activeUidoc, createdView_id);
                    Utility.ShowMessage("All the rooms on the Bigfish Room plan(s) have been successfully created. ", "Message", "information", "message");
                }
                else
                {
                    MessageBox.Show("Room Creation failed...");
                }
            }
            catch (Exception exp)
            {
                Utility.Logger(exp);
            }

            return createdView_id;
        }

        public static void TurnOffAllGraphics(Document doc, string created_view_id)
        {
            ElementId _ViewElementId = Autodesk.Revit.DB.ElementId.Parse(created_view_id);
            Element view_element = doc.GetElement(_ViewElementId);
            View CurrentView = view_element as View;
            
            Categories categories = doc.Settings.Categories;             
            List<Category> ToBeOff = new List<Category>();
            foreach (Category curr_category in categories)
            {
                string curr_category_name = curr_category.Name.ToUpper();
                
                if (!curr_category_name.Contains("ROOMS") && !curr_category_name.Contains("ROOM TAGS") && !curr_category_name.Contains("WALLS") && !curr_category_name.Contains("LINES"))
                {                    
                    ToBeOff.Add(curr_category);
                }
            }
            using (Transaction t = new Transaction(doc, "FilteredVG"))
            {
                t.Start();
                foreach (Category One_Cat in ToBeOff)
                {
                    try
                    { 
                            CurrentView.SetCategoryHidden(One_Cat.Id, true);                            
                    }
                    catch (Exception excp)
                    {
                        // Utility.Logger(excp);
                        // TaskDialog.Show("Category causing trouble", One_Cat.Name.ToString());
                    }
                }
                t.Commit();
            }
        }

        private static string CreateRoomPlanView(UIDocument _externalCommandData, string LevelId)
        {
            string createdView_id = "NO";
            try
            {
                UIDocument uidoc = _externalCommandData.Application.ActiveUIDocument;
                Autodesk.Revit.DB.Document doc = uidoc.Document;
                Autodesk.Revit.DB.View ActiveView = null;
                Categories categories = doc.Settings.Categories;

                string RoomViewName = string.Empty;

                //ActiveView = FindAndActiveViewByLevelId(_externalCommandData, LevelId);
                ActiveView = uidoc.ActiveView;

                if (ActiveView != null)
                    using (Transaction t = new Transaction(doc, "Duplicate Views"))
                    {
                        t.Start();
                        //levelViewElement.Document.ActiveView.Duplicate()
                        //Element CreatedView = doc.GetElement(levelViewElement.Document.ActiveView.Duplicate(ViewDuplicateOption.WithDetailing));
                        Element CreatedView = doc.GetElement(ActiveView.Duplicate(ViewDuplicateOption.Duplicate));

                        createdView_id = CreatedView.Id.ToString();

                        RoomViewName = String.Format("Bigfish-(ID{0})-Room Plan", LevelId);
                        CreatedView.Name = RoomViewName;
                        //bigfish-(ID 311) - Room Plan
                        //"DuplicateWithDetail-" + DateTime.Now.Millisecond.ToString();
                        doc.ActiveView.EnableTemporaryViewPropertiesMode(CreatedView.Id);
                        doc.ActiveView.DisplayStyle = DisplayStyle.HLR;
                        t.Commit();

                        //TaskDialog.Show("Revit", "View Created Successfully..");

                        FilteredElementCollector room_collector = new FilteredElementCollector(doc, doc.ActiveView.Id).OfCategory(BuiltInCategory.OST_Rooms).WhereElementIsNotElementType();
                        IList<ElementId> room_eids = room_collector.ToElementIds() as IList<ElementId>;

                        Autodesk.Revit.DB.View newView = CreatedView as Autodesk.Revit.DB.View;

                        uidoc.ActiveView = newView;

                        SetPlanViewOffset(_externalCommandData);                        
                    }

                //return RoomViewName;

            }
            catch (Exception excp)
            {
                Utility.Logger(excp);
            }
            return createdView_id;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="externalCommandData"></param>
        public static void SetPlanViewOffset(UIDocument externalCommandData)
        {
            try
            {
                ViewPlan viewPlan = externalCommandData.Application.ActiveUIDocument.ActiveView as ViewPlan;
                PlanViewRange viewRange = viewPlan.GetViewRange();
                using (Transaction t = new Transaction(externalCommandData.Application.ActiveUIDocument.Document))
                {
                    t.Start("SaveOffsetValue");
                    viewRange.SetOffset(PlanViewPlane.TopClipPlane, 2);
                    viewRange.SetOffset(PlanViewPlane.CutPlane, 1);
                    viewRange.SetOffset(PlanViewPlane.BottomClipPlane, 1);
                    viewPlan.SetViewRange(viewRange);
                    t.Commit();
                }
            }
            catch (Exception ex)
            {
                Utility.Logger(ex);
            }
        }

        public static void DeleteAllRooms(UIDocument externalCommandData)
        {
            var doc = externalCommandData.Document;
            FilteredElementCollector room_collector = new FilteredElementCollector(doc,doc.ActiveView.Id).OfCategory(BuiltInCategory.OST_Rooms).WhereElementIsNotElementType();

            IList<ElementId> room_eids = room_collector.ToElementIds() as IList<ElementId>;
            string rNames = string.Empty;
            Int32 i = 0;
            MessageBox.Show("Room Count Beofre Room Delete: " + room_eids.Count);
            using (Transaction t = new Transaction(doc, "Delete Rooms"))
            {
                t.Start();
                foreach (ElementId eid in room_eids)
                {
                    i++;
                    //Room R = doc.GetElement2(eid) as Room;
                    //R.Unplace();
                }
                t.Commit();
            }

            room_collector = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms).WhereElementIsNotElementType();

            room_eids = room_collector.ToElementIds() as IList<ElementId>;
            MessageBox.Show("Room Count After Room Delete: " + room_eids.Count);
        }
        public static void GetColorSchemeName(UIDocument externalCommandData)
        {
            var _SchemeName = Properties.Resources.RoomPattern.ToString();
            //"Room1";

            string foundColorSchemeName = "";
            var doc = externalCommandData.Application.ActiveUIDocument.Document;
            string _Message = "";
            StorageType storageTypeScheme = StorageType.String;
            Category roomCategory = doc.Settings.Categories.get_Item(BuiltInCategory.OST_Rooms);

            var firstOrDefaultColorScheme = new FilteredElementCollector(doc)
           .OfCategory(BuiltInCategory.OST_ColorFillSchema)
           .Cast<ColorFillScheme>()
           .Where(c => c.CategoryId == roomCategory.Id);

            //MessageBox.Show("Called GetColorSchemeName2: " + firstOrDefaultColorScheme.Count().ToString());

            foreach (var item in firstOrDefaultColorScheme)
            {
                List<ColorFillSchemeEntry> newColorSchemeId = new List<ColorFillSchemeEntry>();
                newColorSchemeId = item.GetEntries().ToList();

                foreach (var _item in newColorSchemeId)
                {

                    if (_item.GetStringValue().ToString().ToLower().Contains(_SchemeName.ToString().ToLower()) == true)
                    {
                        // MessageBox.Show("_item.GetStringValue().ToString():" + _item.GetStringValue().ToString() + "------" + _SchemeName);
                        foundColorSchemeName = item.Name;
                        storageTypeScheme = _item.StorageType;
                        // MessageBox.Show("item Found:" + item.Name);
                        break;
                    }
                    // _Message += "c.Caption.ToString():" + _item.GetStringValue().ToString() + "\r\n item.Color: " + _item.Color.Red.ToString() + "\r\n  item.StorageType: " + _item.StorageType.ToString();
                }
            }

          
            DuplicateColorScheme(externalCommandData, foundColorSchemeName, storageTypeScheme);
            //return "";
        }

        // public static ElementId newColorSchemeId { get; set; }
        private static void DuplicateColorScheme(UIDocument externalCommandData, string _schemeName, StorageType _storageType)
        {
            try
            {
                // MessageBox.Show("DuplicateColorScheme Called 1 :");

                var _schemeNamePattern = Properties.Resources.RoomPattern.ToString();
                var doc = externalCommandData.Application.ActiveUIDocument.Document;
                using (Transaction t = new Transaction(doc))
                {
                    t.Start("Create Color Scheme");
                    Category roomCategory = doc.Settings.Categories.get_Item(BuiltInCategory.OST_Rooms);
                    // Create Color Scheme by duplicating an existing one 
                    // (randomly chosen from category: Room)


                    string DuplicateScheme = Properties.Resources.NewColorSchemeName.ToString();

                    var CustomColorSchemeAvailable = new FilteredElementCollector(doc)
                        .OfCategory(BuiltInCategory.OST_ColorFillSchema)
                        .Cast<ColorFillScheme>()
                        .Where(c => c.CategoryId == roomCategory.Id && c.Name == DuplicateScheme)
                    .FirstOrDefault();
                    //MessageBox.Show("DuplicateColorScheme Called 4 :xxxx");
                    //MessageBox.Show("DuplicateColorScheme Called 4 :" + CustomColorSchemeAvailable.Name);


                    /// if New ColorScheme is not available then created a new scheme 
                    /// else use the created one and add new Room Number color Items

                    if (CustomColorSchemeAvailable == null)
                    {
                        var firstOrDefaultColorScheme = new FilteredElementCollector(doc)
                        .OfCategory(BuiltInCategory.OST_ColorFillSchema)
                        .Cast<ColorFillScheme>()
                        .Where(c => c.CategoryId == roomCategory.Id && c.Name == _schemeName)
                        .FirstOrDefault();

                        //MessageBox.Show("_schemeName:" + _schemeName);
                        ElementId newColorSchemeId = firstOrDefaultColorScheme.Duplicate(DuplicateScheme);


                        ColorFillScheme colorScheme = doc.GetElement(newColorSchemeId) as ColorFillScheme;
                        //colorScheme.RemoveEntry()
                        var solidPattern = new FilteredElementCollector(doc)
                         .OfClass(typeof(FillPatternElement))
                         .Cast<FillPatternElement>()
                         .First(a => a.GetFillPattern().IsSolidFill == true);

                        colorScheme.Name = Properties.Resources.NewColorSchemeName.ToString();
                        //"Bigfish Scheme";

                        string RoomColorValues = Properties.Resources.RoomColorScheme.ToString();

                        List<string> RoomColorScheme = RoomColorValues.Split(':').ToList();

                        // Create entry
                        ColorFillSchemeEntry entry = new ColorFillSchemeEntry(_storageType);

                        IList<ColorFillSchemeEntry> colorFillSchemeEntries = colorScheme.GetEntries();

                        ///MessageBox.Show("colorFillSchemeEntries Length" + colorFillSchemeEntries.Count);
                        List<ColorFillSchemeEntry> customColorFillSchemeEntry = new List<ColorFillSchemeEntry>();
                        string customeRoomNames = string.Empty;
                        foreach (var item in colorFillSchemeEntries)
                        {
                            //customeRoomNames += "\r\n" + item.GetStringValue();
                            if (item.GetStringValue().StartsWith(_schemeNamePattern) == true)
                            {
                                customColorFillSchemeEntry.Add(item);
                                customeRoomNames += "\r\n" + item.GetStringValue();
                            }
                            else
                            {
                                customeRoomNames += "\r\n Not Available: -" + item.Caption;
                            }
                        }
                        //MessageBox.Show("customeRoomNames: " + customeRoomNames);
                        string colorSchemeName = string.Empty;

                        byte RedColor = 0;
                        byte GreenColor = 0;
                        byte BlueColor = 0;

                        int ColorRunningIndex = 1;
                        if (customColorFillSchemeEntry.Count > 0)
                        {
                            //MessageBox.Show("Modified values...");
                            foreach (var item in customColorFillSchemeEntry)
                            {
                                //MessageBox.Show("customeRoomNames: 1");
                                RedColor = Convert.ToByte(RoomColorScheme[ColorRunningIndex].Split(',')[0]);
                                GreenColor = Convert.ToByte(RoomColorScheme[ColorRunningIndex].Split(',')[1]);
                                BlueColor = Convert.ToByte(RoomColorScheme[ColorRunningIndex].Split(',')[2]);
                                item.Color = new Autodesk.Revit.DB.Color(RedColor, GreenColor, BlueColor);
                                colorScheme.UpdateEntry(item);
                                ColorRunningIndex++;
                            }

                        }
                        //MessageBox.Show("colorScheme.Name: " + colorScheme.Name);
                        View currentView = externalCommandData.Application.ActiveUIDocument.ActiveView;
                        currentView.SetColorFillSchemeId(roomCategory.Id, colorScheme.Id);
                        t.Commit();

                        // MessageBox.Show("Completed Modification...");
                    }
                    else
                    {
                        //MessageBox.Show("DuplicateColorScheme else part");
                        View currentView = externalCommandData.Application.ActiveUIDocument.ActiveView;
                        currentView.SetColorFillSchemeId(roomCategory.Id, CustomColorSchemeAvailable.Id);
                        t.Commit();

                        //MessageBox.Show("Completed Modification...");
                    }

                }
            }
            catch (Exception exp)
            {
                Utility.Logger(exp);
                throw;
            }
        }



        public static void CreateRoomsForAllAvailableLevels(UIDocument externalCommandData)
        {
            try
            {

                string xx = "";
                string createdView_id = "";
                string roomTagRespose = "";
                Document doc = externalCommandData.Application.ActiveUIDocument.Document;
                //Hashtable roomLevels = RvtRoomUtility.GetFloorPlanAssociatedlevelDetails(externalCommandData, "level_id");
                Hashtable roomLevels = RvtRoomUtility.GetRoomLevelDetail(externalCommandData);
                foreach (DictionaryEntry item in roomLevels)
                {
                    bool status = false;//RvtRoomUtility.isBigfishViewAvailable(externalCommandData, item.Key.ToString());
                                        //MessageBox.Show(status.ToString());
                    if (status == false)
                    {
                        if (item.Key.ToString().Length > 0)
                        {
                            xx += item.Value + "\r\n";
                            createdView_id = CreateRoomWithColorFillandPreview(externalCommandData, item.Key.ToString());

                        }
                    }
                    //else
                    //{
                    //    Utility.ShowMessage("Room is alreay available", "Information", "error", "message");
                    //}
                }

            }
            catch (Exception excp)
            {
                Utility.Logger(excp);
                System.Windows.Forms.MessageBox.Show(excp.Message.ToString());
                throw;
            }
        }

        public static string CreateRoomTags(Document doc, string createdView_id, ElementId levelId)
        {
            // Document doc = commandData.Application.ActiveUIDocument.Document;
            // UIDocument uidoc = commandData.Application.ActiveUIDocument;
            // Application app = commandData.Application.ActiveUIDocument.Document.Application;
            //TaskDialog.Show("createdView_id ... ", createdView_id);

            using (Transaction trans = new Transaction(doc, "ACTRAN"))
            {
                trans.Start();

                //ElementId view_element_id = Autodesk.Revit.DB.ElementId.Parse(createdView_id);
                //Element view_element = doc.GetElement(view_element_id);
                //View view = view_element as View;
                //Level active_view_level = view.GenLevel;
                //ElementId Level_element_id = active_view_level.Id;

                ElementLevelFilter levelIdFilter = new ElementLevelFilter(levelId);
                FilteredElementCollector FEC = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms).WherePasses(levelIdFilter);
                IList<ElementId> roomids = FEC.ToElementIds() as IList<ElementId>;
                //TaskDialog.Show("ROOM COUNT", roomids.Count().ToString());

                ElementId _ViewElementId = Autodesk.Revit.DB.ElementId.Parse(createdView_id);
                foreach (ElementId roomid in roomids)
                {
                    try
                    {
                        Element elem = doc.GetElement(roomid);
                        //TaskDialog.Show("Elem ... ", elem.ToString());
                        //TaskDialog.Show("Room Name ... ", elem.Name.ToString());
                        // Room r = elem as Room;
                        XYZ cen = GetRoomCenter(elem);
                        UV center = new UV(cen.X, cen.Y);
                        doc.Create.NewRoomTag(new LinkElementId(roomid), center, _ViewElementId);
                    }
                    catch (Exception excp)
                    {
                        Utility.Logger(excp);
                        // throw;
                    }
                }
                trans.Commit();
            }
            return "Succeeded";
        }

        public static XYZ GetRoomCenter(Element elem)
        {
            // Get the room center point.
            // TaskDialog.Show("elem ... ", elem.ToString());
            XYZ boundCenter = GetElementCenter(elem);
            Room room = elem as Room;
            LocationPoint locPt = (LocationPoint)room.Location;
            XYZ roomCenter = new XYZ(boundCenter.X, boundCenter.Y, locPt.Point.Z);
            return roomCenter;
        }

        public static XYZ GetElementCenter(Element elem)
        {
            BoundingBoxXYZ bounding = elem.get_BoundingBox(null);
            //TaskDialog.Show("bounding ... ", bounding.ToString());
            XYZ center = (bounding.Max + bounding.Min) * 0.5;
            return center;
        }

        public static string ImagefilePath { get; set; }

        public static bool isBigfishViewAvailable(UIDocument _externalCommandData, string levelid)
        {
            try
            {
                bool isAvailable = false;
                var uidoc = _externalCommandData.Application.ActiveUIDocument;
                var doc = uidoc.Document;
                ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Views);
                FilteredElementCollector views = new FilteredElementCollector(doc);
                views.WherePasses(filter).WhereElementIsNotElementType().ToElements();
                foreach (Autodesk.Revit.DB.View view in views)
                {
                    if (view.ViewType == ViewType.FloorPlan && view.Name.Contains(levelid) == true)
                    {
                        isAvailable = true;
                    }
                }

                return isAvailable;
            }
            catch (Exception excp)
            {
                Utility.Logger(excp);
                throw;
            }
        }
        public static string SaveImage(UIDocument _externalCommandData, string levelid)
        {
            try
            {
                //MessageBox.Show("ImagefilePath 1 levelid:" + levelid);
                //return "";
                //if (ImagefilePath != null)
                //{
                //    if (ImagefilePath.Contains(levelid) == true)
                //        return ImagefilePath;
                //}
                var uidoc = _externalCommandData.Application.ActiveUIDocument;
                var doc = uidoc.Document;
                ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Views);
                FilteredElementCollector views = new FilteredElementCollector(doc);
                views.WherePasses(filter).WhereElementIsNotElementType().ToElements();
                IList<ElementId> ImageExportList = null;

                ImagefilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\images\\";

                //MessageBox.Show("ImagefilePath 4" + views.Count());

                if (!Directory.Exists(ImagefilePath))
                {
                    Directory.CreateDirectory(ImagefilePath);
                }

                foreach (Autodesk.Revit.DB.View view in views)
                {
                    if (view.ViewType == ViewType.FloorPlan && view.Name.Contains(levelid) == true)
                    {
                        try
                        {
                            using (Transaction tran = new Transaction(doc, "apply Filter"))
                            {
                                tran.Start();
                                string prefix = DateTime.Now.Millisecond.ToString();
                                var ieo = new ImageExportOptions
                                {
                                    ZoomType = ZoomFitType.FitToPage,
                                    PixelSize = 2000,
                                    FilePath = ImagefilePath + prefix,
                                    ViewName = view.Name,
                                    FitDirection = FitDirectionType.Horizontal,
                                    HLRandWFViewsFileType = ImageFileType.JPEGMedium,
                                    ImageResolution = ImageResolution.DPI_600,
                                    ExportRange = ExportRange.SetOfViews,
                                };
                                //MessageBox.Show("XXXX ");

                                //Autodesk.Revit.DB.View newView = view as Autodesk.Revit.DB.View;
                                //uidoc.ActiveView = newView;

                                //MessageBox.Show("XXXX ImagefilePath :ImageExportOptions.GetFileName(doc, view.Id) " + ImageExportOptions.GetFileName(doc, view.Id));

                                ImageExportList = new List<ElementId>();
                                ImageExportList.Add(view.Id);
                                ImagefilePath = ImagefilePath + prefix + ImageExportOptions.GetFileName(doc, view.Id) + ".jpg";

                                ieo.SetViewsAndSheets(ImageExportList);
                                doc.ExportImage(ieo);

                                tran.Commit();
                                tran.Dispose();
                            }
                        }
                        catch (Exception ex)
                        {
                            Utility.Logger(ex);
                            //MessageBox.Show(ex.StackTrace, ex.Message);
                            //System.IO.File.WriteAllText(@"C:\Temp\Errors.txt", view.Name + " " + ex.Message);
                        }
                        finally { }
                    }
                }
               // return ImagefilePath;
            }
            catch (Exception excp)
            {
                Utility.Logger(excp);
            }
            return ImagefilePath;
        }

        public static void ReCreateRoom(ExternalCommandData commandData)
        {
            try
            {
                var activeUidoc = commandData.Application.ActiveUIDocument;
                Hashtable roomLevels = RvtRoomUtility.GetRoomLevelDetail(activeUidoc);
                bool status = false;
                foreach (DictionaryEntry item in roomLevels)
                {
                    string _level_element_id = item.Key.ToString();
                    ElementId _ElementId = Autodesk.Revit.DB.ElementId.Parse(_level_element_id);
                    string result = RvtRoomUtility.CreateRooms(_ElementId, activeUidoc.Document);
                    if (result == "success")
                    {
                        Element layoutView = activeUidoc.Application.ActiveUIDocument.Document.GetElement(activeUidoc.ActiveView.Id);
                        RvtRoomUtility.GetColorSchemeName(activeUidoc);
                        string roomTagRespose = RvtRoomUtility.CreateRoomTags(activeUidoc.Document, layoutView.Id.ToString(), _ElementId);
                        Utility.ShowMessage("All the rooms on the Bigfish Room plan(s) have been successfully created. ", "Message", "information", "message");
                    }
                }
            }
            catch (Exception exp)
            {
                Utility.Logger(exp);
            }
        }
    }
}