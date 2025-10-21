// URL: https://jeremytammik.github.io/tbc/a/0754_shared_param_add_categ.htm


using Autodesk.Revit.Attributes;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Xml.Linq;
using DocRvt = Autodesk.Revit.DB.Document; // to avoid ambiguities
using boqtakeoff.core.Libraries;

namespace boqtakeoff.core
{

    /// <summary>
    /// Major Helper Class for Revit Params
    /// </summary>
    public static class AnnotateElements
    {

        const string _groupname = "SMI Parameters";
        // const string _defname = "SMI_";
        // static ParameterType _deftype = ParameterType.Text;
        public static readonly ForgeTypeId _deftype = SpecTypeId.String.Text; // 2022
        public static StringBuilder CreateSharedParameter = new StringBuilder();


        /// <summary>
        /// 
        /// </summary>
        public static void AnnotateRoomElement(ExternalCommandData _externalCommandData, Hashtable hashtableCategory)
        {
            try
            {
                var uidoc = _externalCommandData.Application.ActiveUIDocument;
                //doc = uidoc.Document;
                var document = uidoc.Document;

                //var application = externalCommandData.Application.Application;

                Hashtable levelIdHashLevelDetails = AnnotateElements.Getinfo_Level(document);
                ICollection levelDetails_key_collection = levelIdHashLevelDetails.Keys;
                //  MessageBox.Show("Called 2");
                foreach (String levelID in levelDetails_key_collection)
                {
                    // StringBuilder ElemeInRoomDetails = new StringBuilder();
                    Hashtable levelDetails;

                    levelDetails = (Hashtable)levelIdHashLevelDetails[levelID];
                    Hashtable roomIdHashroomDetails = (Hashtable)levelDetails["room_details"];
                    //  MessageBox.Show("Called 2" + roomIdHashroomDetails.Count);

                    if (roomIdHashroomDetails.Count > 0)
                    {
                        ICollection roomDetails_key_collection = roomIdHashroomDetails.Keys;
                        foreach (String curr_room_id in roomDetails_key_collection)
                        {
                            int curr_room_id_int = Convert.ToInt32(curr_room_id);
                            ElementId curr_room_elem_id = new ElementId(curr_room_id_int);
                            Element room_element = document.GetElement(curr_room_elem_id);
                            Room room = room_element as Room;

                            int levelIDInt = Convert.ToInt32(levelID);
                            ElementId levelElemId = new ElementId(levelIDInt);

                            ElementLevelFilter levelIdFilter = new ElementLevelFilter(levelElemId);

                            ElementId id = new ElementId(BuiltInParameter.ELEM_ROOM_NUMBER);

                            ParameterValueProvider provider
                              = new ParameterValueProvider(id);

                            FilterStringRuleEvaluator evaluator
                              = new FilterStringEquals();

                            string sRoomNumber = room.Number.ToString();

                            FilterRule rule = new FilterStringRule(provider, evaluator, sRoomNumber);

                            ElementParameterFilter filter = new ElementParameterFilter(rule);

                            FilteredElementCollector collector = new FilteredElementCollector(document).WherePasses(filter);
                            ICollection<Element> ElementCollectionAtCurrLevel = collector.OfClass(typeof(FamilyInstance)).ToElements();
                            foreach (Element curr_elem in ElementCollectionAtCurrLevel)
                            {
                                Category category = curr_elem.Category;
                                // ElemeInRoomDetails.Append("\ncategory: " + category.Name.ToString());

                                if (category.BuiltInCategory.ToString() == "OST_Floors" || curr_elem.Name == "SM_Carpet")
                                {
                                    TaskDialog.Show("BuiltInCategory", "OST_Floors");
                                }
                                AnnotateElements.add_shared_parameters(document, curr_elem, room, levelElemId);
                            } // Elements
                        } // Room
                    }
                } // levels

            }
            catch (Exception excp)
            {
                throw;
            }
        }

        public static void add_shared_parameters(DocRvt document, Element curr_elem, Room room, ElementId floor_id)
        {
            try
            {
                using (Transaction trans = new Transaction(document, "Change Parameter"))
                {
                    trans.Start();

                    ElementId id_type = curr_elem.GetTypeId();
                    Element element_type = document.GetElement(id_type);

                    // INFO:: Adding the SKU Details
                    Hashtable sharedParamDetailsHashFloorId = SharedParameter.getSharedParamGUID(document, "SKU");
                    String guidStringFI = sharedParamDetailsHashFloorId["guid"].ToString();

                    Parameter parameter = element_type.get_Parameter(new Guid(guidStringFI));
                    String sharedParametervalue = parameter.AsString();
                    if (String.IsNullOrWhiteSpace(sharedParametervalue) || sharedParametervalue == "NA")
                    {
                        sharedParametervalue = curr_elem.Name;
                    }
                    Parameter sku_shared_param = HelperParams.GetOrCreateElemSharedParam(curr_elem, "SMI_SKU_CODE", true);
                    if (sku_shared_param != null && !sku_shared_param.IsReadOnly && sku_shared_param.IsShared)
                    {
                        bool bRet = sku_shared_param.Set(sharedParametervalue);
                    }
                    // INFO:: Adding the Room Number
                    Parameter smi_room_number = HelperParams.GetOrCreateElemSharedParam(curr_elem, "SMI_ROOM_NUMBER", true);
                    if (smi_room_number != null && !smi_room_number.IsReadOnly && smi_room_number.IsShared)
                    {
                        bool bRet = smi_room_number.Set(room.Number.ToString());
                    }
                    // INFO:: Adding the Room Name
                    Parameter smi_room_name = HelperParams.GetOrCreateElemSharedParam(curr_elem, "SMI_ROOM_NAME", true);
                    if (smi_room_name != null && !smi_room_name.IsReadOnly && smi_room_name.IsShared)
                    {
                        bool bRet = smi_room_name.Set(room.Name.ToString());
                    }
                    // INFO:: Adding the level details
                    Parameter smi_level_name = HelperParams.GetOrCreateElemSharedParam(curr_elem, "SMI_LEVEL_NAME", true);
                    Element level_element = document.GetElement(floor_id);
                    if (smi_level_name != null && !smi_level_name.IsReadOnly && smi_level_name.IsShared)
                    {
                        bool bRet = smi_level_name.Set(level_element.Name.ToString());
                    }
                    trans.Commit();
                }

            }
            catch (Exception e)
            {
                TaskDialog.Show("Revit", e.Message.ToString());
                // ElemeInRoomDetails.Append("\n Revit Error: " + e.ToString());
                // ElemeInRoomDetails.Append("\n Revit Error message: " + e.Message.ToString());
            }// end catch
        }

        public static void tag_doors(DocRvt document)
        {
            ICollection<Element> door_element_list = new FilteredElementCollector(document).OfCategory(BuiltInCategory.OST_Doors).ToElements();
            foreach (Element curr_elem in door_element_list)
            {
                try
                {
                    using (Transaction trans = new Transaction(document, "Change Parameter"))
                    {
                        trans.Start();
                        Hashtable sharedParamDetailsHashFloorId = SharedParameter.getSharedParamGUID(document, "SKU");
                        String guidStringFI = sharedParamDetailsHashFloorId["guid"].ToString();
                        Parameter smi_sku_param = curr_elem.get_Parameter(new Guid(guidStringFI));
                        String sharedParametervalue = smi_sku_param.AsString();
                        if (smi_sku_param != null && !smi_sku_param.IsReadOnly && smi_sku_param.IsShared)
                        {
                            Parameter sku_shared_param = HelperParams.GetOrCreateElemSharedParam(curr_elem, "SMI_SKU_CODE", true);
                            bool bRet = sku_shared_param.Set(sharedParametervalue);
                        }
                        Parameter smi_room_number = HelperParams.GetOrCreateElemSharedParam(curr_elem, "SMI_ROOM_NUMBER", true);
                        String room_number = curr_elem.get_Parameter(BuiltInParameter.PATH_OF_TRAVEL_FROM_ROOM).AsString();
                        if (smi_room_number != null && !smi_room_number.IsReadOnly && smi_room_number.IsShared)
                        {
                            bool bRet = smi_room_number.Set(room_number);
                        }
                        Parameter smi_room_name = HelperParams.GetOrCreateElemSharedParam(curr_elem, "SMI_ROOM_NAME", true);
                        if (smi_room_name != null && !smi_room_name.IsReadOnly && smi_room_name.IsShared)
                        {
                            bool bRet = smi_room_name.Set(room_number);
                        }
                        trans.Commit();
                    }//end transaction
                }
                catch (Exception e)
                {
                    TaskDialog.Show("Revit", e.Message.ToString());
                    //ElemeInRoomDetails.Append("\n Revit Error: " + e.ToString());
                    //ElemeInRoomDetails.Append("\n Revit Error message: " + e.Message.ToString());
                }// end catch
            }
        }

        public static bool SharedParametersFileExists(UIApplication revitApp)
        {

            // create shared parameter file
            String modulePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            String paramFile = modulePath + "\\SharedParameters.txt";
            if (File.Exists(paramFile))
            {
                return true;
            }

            // NOTE:: Try catch to be used for creating the file.
            // // The error message in case file is either not present or not created the error
            // // response should be false
            FileStream fs = File.Create(paramFile);
            fs.Close();

            revitApp.Application.SharedParametersFilename = paramFile;
            return true;
        }

        public static void AddParameterExample(DocRvt document, Element elem)
        {
            try
            {
                FamilyInstance fyInst = elem as FamilyInstance;
                var setString = JtFamilyInstanceExtensionMethods.GetFamilyTypeMarkParam(fyInst, document);
                TaskDialog.Show("Family Type Mark", setString);
                using (Transaction trans = new Transaction(document, "Change Parameter"))
                {
                    trans.Start();
                    try  // adding parameter
                    {
                        Parameter Param = JtFamilyInstanceExtensionMethods.GetFamilyMarkParam(fyInst);
                        JtFamilyInstanceExtensionMethods.SetFamilyMarkParam(Param, setString);
                    }
                    catch
                    {
                    }
                    trans.Commit();
                }//end transaction
            }//end try
            catch (Exception e)
            {
                TaskDialog.Show("Revit", e.Message.ToString());
            }// end catch
        }// end public void AddInstParameter

        public static void GetInfo_BoundarySegment(Room room)
        {
            IList<IList<Autodesk.Revit.DB.BoundarySegment>> segments = room.GetBoundarySegments(new SpatialElementBoundaryOptions());

            if (null != segments)  //the room may not be bound
            {
                string message = "BoundarySegment";
                foreach (IList<Autodesk.Revit.DB.BoundarySegment> segmentList in segments)
                {
                    foreach (Autodesk.Revit.DB.BoundarySegment boundarySegment in segmentList)
                    {
                        // Get curve start point
                        message += "\nCurve start point: (" + boundarySegment.GetCurve().GetEndPoint(0).X + ","
                                       + boundarySegment.GetCurve().GetEndPoint(0).Y + "," +
                                      boundarySegment.GetCurve().GetEndPoint(0).Z + ")";
                        // Get curve end point
                        message += ";\nCurve end point: (" + boundarySegment.GetCurve().GetEndPoint(1).X + ","
                             + boundarySegment.GetCurve().GetEndPoint(1).Y + "," +
                             boundarySegment.GetCurve().GetEndPoint(1).Z + ")";
                        // Get document path name
                        message += ";\nDocument path name: " + room.Document.PathName;
                        // Get boundary segment element name
                        if (boundarySegment.ElementId != ElementId.InvalidElementId)
                        {
                            message += ";\nElement name: " + room.Document.GetElement(boundarySegment.ElementId).Name;
                        }
                    }
                }
                TaskDialog.Show("Revit", message);
            }
        }

        public static XYZ GetElementCenter(Element elem)
        {
            BoundingBoxXYZ bounding = elem.get_BoundingBox(null);
            // TaskDialog.Show("bounding ...", bounding.ToString());
            if (bounding == null)
                return null;
            XYZ center = (bounding.Max + bounding.Min) * 0.5;
            if (elem.Category.BuiltInCategory.ToString() == "OST_Floors")
            {
                XYZ z_shift = new XYZ(0, 0, 10);
                center = center.Add(z_shift);
                TaskDialog.Show("Element name ...", elem.Name);
                TaskDialog.Show("center ...", z_shift.X.ToString());
                TaskDialog.Show("center ...", z_shift.Y.ToString());
                TaskDialog.Show("center ...", z_shift.Z.ToString());
            }
            return center;
        }

        public static Hashtable Getinfo_Level(DocRvt document)
        {
            StringBuilder levelInformation = new StringBuilder();
            StringBuilder RoomInformation = new StringBuilder();
            Hashtable levelIdHashLevelDetails = new Hashtable();
            int levelNumber = 0;
            FilteredElementCollector collector = new FilteredElementCollector(document);
            IOrderedEnumerable<Level> collection = collector.OfClass(typeof(Level)).Cast<Level>().OrderBy(lev => lev.Elevation);
            // Creating a new hash table                        
            foreach (Level level in collection)
            {
                Hashtable levelDetails = new Hashtable();
                if (null != level)
                {
                    // keep track of number of levels
                    levelNumber++;
                    //get the name of the level
                    levelInformation.Append("\nLevel Name: " + level.Name);
                    //get the elevation of the level
                    levelInformation.Append("\n\tElevation: " + level.Elevation);
                    levelInformation.Append("\n\tLevel Id: " + level.Id);
                    levelDetails.Add("name", level.Name);
                    levelDetails.Add("elevation", level.Elevation);
                    levelDetails.Add("project_elevation", level.ProjectElevation);
                    ElementLevelFilter levelIdFilter = new ElementLevelFilter(level.Id);
                    collector = new FilteredElementCollector(document);
                    ICollection<Element> SpatialElementAtCurrLevel = collector.OfClass(typeof(SpatialElement)).WherePasses(levelIdFilter).ToElements();
                    Hashtable roomIdHashroomDetails = new Hashtable();
                    foreach (SpatialElement CurrSpatialElement in SpatialElementAtCurrLevel)
                    {
                        Room room = CurrSpatialElement as Room;
                        if (null != room)
                        {
                            Hashtable roomDetails = new Hashtable();
                            roomDetails.Add("room_id", room.Id.ToString());
                            roomDetails.Add("room_name", room.Name.ToString());
                            roomDetails.Add("room_parameter_map", room.ParametersMap);
                            roomIdHashroomDetails.Add(room.Id.ToString(), roomDetails);
                        }
                    }
                    levelDetails.Add("room_details", roomIdHashroomDetails);
                    levelIdHashLevelDetails.Add(level.Id.ToString(), levelDetails);
                }
            }
            //number of total levels in current document
            levelInformation.Append("\n\n There are " + levelNumber + " levels in the document!");
            return levelIdHashLevelDetails;
        }

        public static Hashtable GetLevelNameHashLevelId(DocRvt document)
        {
            StringBuilder levelInformation = new StringBuilder();
            StringBuilder RoomInformation = new StringBuilder();
            Hashtable levelDetails = new Hashtable();

            int levelNumber = 0;
            FilteredElementCollector collector = new FilteredElementCollector(document);
            IOrderedEnumerable<Level> collection = collector.OfClass(typeof(Level)).Cast<Level>().OrderBy(lev => lev.Elevation);
            // Creating a new hash table                        
            foreach (Level level in collection)
            {
                if (null != level)
                {
                    // keep track of number of levels
                    levelNumber++;
                    levelDetails.Add(level.Name, level.Id);
                }
            }
            return levelDetails;
        }


       

        /// <summary>
        /// Retrieve all family names both standard and system
        /// </summary>
        static List<string> GetFamilyNamesWithCategory(
          Autodesk.Revit.DB.Document doc)
        {
            return new FilteredElementCollector(doc)
              .OfClass(typeof(Category))
               .Cast<Category>()
              .Select<Category, string>(a => a.Name)
              .Distinct<string>()
              .ToList();
        }

    
        static IEnumerable<string> GetFamilyNames(
         Autodesk.Revit.DB.Document doc)
        {
            return new FilteredElementCollector(doc)
              .OfClass(typeof(ElementType))
              .Cast<ElementType>()
              .Select<ElementType, string>(a => a.FamilyName)
              .Distinct<string>();
        }

        public static string GetFamilyElements(Autodesk.Revit.DB.Document doc)
        {
            List<Element> familyElements = new List<Element>();

            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(FamilyInstance));
            collector.WhereElementIsElementType();
            string info = "";
            System.Windows.Forms.MessageBox.Show("collector.GetElementCount() :" + collector.GetElementCount().ToString());
            foreach (ElementId element in collector.ToElementIds())
            {
                info += "element: " + element.ToString() + "-doc.GetElement(element): " + doc.GetElement(element);
                familyElements.Add(doc.GetElement(element));
            }

            System.Windows.Forms.MessageBox.Show("info :" + info);

            foreach (Element elem in familyElements)
            {
                //Element elem = doc.GetElement(id);
                info += elem.Name + "\n";
            }
            System.Windows.Forms.MessageBox.Show("info :" + info);
            return info;
        }

        public static ExternalCommandData externalCommandData { get; set; }
      
        public static void TAGAllRooms(ExternalCommandData _externalCommandData)
        {
            var uidoc = _externalCommandData.Application.ActiveUIDocument;
            var doc = uidoc.Document;
            var roomTagTypes = new FilteredElementCollector(doc).
                       OfCategory(BuiltInCategory.OST_RoomTags).
                       Cast<FamilySymbol>();

            
            //doc = uidoc.Document;


        }
    }
}