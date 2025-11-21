namespace boqtakeoff.core
{
    using Autodesk.Revit.DB;
    using Autodesk.Revit.UI;
    using System;
    using System.Collections;
    using System.Windows;

    using boqtakeoff.core.Libraries;
    using BitMiracle.LibTiff.Classic;

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]

    public class BOQExtractor : IExternalCommand
    {
        #region IExternalCommand Members

        public static Autodesk.Revit.DB.Document doc;

        /// <summary>
        /// The implementation of the command.
        /// </summary>
        /// <param name="commandData"></param>
        /// <param name="message"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public Result Execute(ExternalCommandData commandData, ref string message, Autodesk.Revit.DB.ElementSet elements)
        {
            var uidoc = commandData.Application.ActiveUIDocument;
            doc = uidoc.Document;

            var application = commandData.Application.Application;

            // string versionName = application.VersionName;
            string versionNumber = application.VersionNumber;
            // string buildNumber = application.VersionBuild;

            if (versionNumber != "2022" && versionNumber != "2023")
            {
                TaskDialog.Show("Error ...", "The plugin is compliant with the Revit 2022 and higher versions...");
                return Result.Failed;
            }

            if (isBigfishViewAvailable(commandData) == true)
            {
                Main form1 = new Main(commandData);
                form1.ShowDialog();
            }


            return Result.Succeeded;
        }
        public static string GetPath()
        {
            // Return constructed namespace path.
            return typeof(BOQExtractor).Namespace + "." + nameof(BOQExtractor);
        }

        // Add this new method
        public static string GetAssemblyPath()
        {
            return typeof(BOQExtractor).Assembly.Location;
        }

        public static string updateProjectInformation(string project_name, string project_key)
        {
            ProjectInfo projectInfo = doc.ProjectInformation;
            ElementId projectInfoElemId = projectInfo.Id;
            Element projectInfo_element = doc.GetElement(projectInfoElemId);

            using (Transaction t = new Transaction(doc))
            {
                if (t.Start("Set Project Parameter Values") == TransactionStatus.Started)
                {
                    Parameter smi_project_name = HelperParams.GetOrCreateElemSharedParam(projectInfo_element, "SMI_PROJECT_NAME", true);
                    if (smi_project_name != null && !smi_project_name.IsReadOnly && smi_project_name.IsShared)
                    {
                        bool bRet = smi_project_name.Set(project_name);
                    }

                    Parameter smi_project_key = HelperParams.GetOrCreateElemSharedParam(projectInfo_element, "SMI_PROJECT_KEY", true);
                    if (smi_project_key != null && !smi_project_key.IsReadOnly && smi_project_key.IsShared)
                    {
                        bool bRet = smi_project_key.Set(project_key);
                    }
                    t.Commit();
                }
                else
                {
                    t.RollBack();
                }
            }
            return "success";

        }
        public static string getProjectInformationFromRevitModel()
        {
            try
            {
                string revit_project_name = "";
                ProjectInfo projectInfo = doc.ProjectInformation;
                ElementId projectInfoElemId = projectInfo.Id;
                Element projectInfo_element = doc.GetElement(projectInfoElemId);

                Hashtable smi_project_name_sp = SharedParameter.getSharedParamGUID(doc, "SMI_PROJECT_NAME");
                if (smi_project_name_sp.Count > 0)
                {
                    string smi_project_name_guid_str = smi_project_name_sp["guid"].ToString();
                    Parameter parameter = projectInfo_element.get_Parameter(new Guid(smi_project_name_guid_str));
                    revit_project_name = parameter.AsString();
                }

                return revit_project_name;
            }
            catch (Exception exp)
            {
                Utility.Logger(exp);
                throw;
            }
        }

        private bool isBigfishViewAvailable(ExternalCommandData commandData)
        {
            try
            {
                var activeUidoc = commandData.Application.ActiveUIDocument;
                Hashtable roomLevels = RvtRoomUtility.GetRoomLevelDetail(activeUidoc);
                bool status = false;
                foreach (DictionaryEntry item in roomLevels)
                {
                    string _level_element_id = item.Key.ToString();
                    status = RvtRoomUtility.isBigfishViewAvailable(activeUidoc, _level_element_id);
                    if (status == false)
                    {
                        Utility.ShowMessage("Please create the same using ‘Create Room Plan’ icon within Bigfish Tools menu.", "Information", "error", "message");
                    }
                }
                return status;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }




    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]

    public class CreateRoomCommand : IExternalCommand
    {
        #region IExternalCommand Members

        public static Autodesk.Revit.DB.Document doc;

        /// <summary>
        /// The implementation of the command.
        /// </summary>
        /// <param name="commandData"></param>
        /// <param name="message"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public Result Execute(ExternalCommandData commandData, ref string message, Autodesk.Revit.DB.ElementSet elements)
        {
            var uidoc = commandData.Application.ActiveUIDocument;
            doc = uidoc.Document;

            var application = commandData.Application.Application;

            // string versionName = application.VersionName;
            string versionNumber = application.VersionNumber;
            // string buildNumber = application.VersionBuild;

            if (versionNumber != "2022" && versionNumber != "2023")
            {
                TaskDialog.Show("Error ...", "The plugin is compliant with the Revit 2022 and higher versions...");
                return Result.Failed;
            }

            if (isBigfishViewAvailable(commandData) == true)
            {
              RvtRoomUtility.ReCreateRoom(commandData);
            }
            else
            {
                RvtRoomUtility.CreateRoomsForAllAvailableLevels(uidoc);

            }

            return Result.Succeeded;
        }


        private bool isBigfishViewAvailable(ExternalCommandData commandData)
        {
            try
            {
                var activeUidoc = commandData.Application.ActiveUIDocument;
                Hashtable roomLevels = RvtRoomUtility.GetRoomLevelDetail(activeUidoc);
                bool status = false;
                foreach (DictionaryEntry item in roomLevels)
                {
                    string _level_element_id = item.Key.ToString();
                    status = RvtRoomUtility.isBigfishViewAvailable(activeUidoc, _level_element_id);
                }
                return status;
            }
            catch (Exception)
            {

                throw;
            }

        }
      
        public static string GetPath()
        {
            // Return constructed namespace path.
            return typeof(CreateRoomCommand).Namespace + "." + nameof(CreateRoomCommand);
        }

        /// <summary>
        /// Gets the assembly path where this command is located.
        /// </summary>
        /// <returns>The assembly location path.</returns>
        public static string GetAssemblyPath()
        {
            return typeof(CreateRoomCommand).Assembly.Location;
        }

        #endregion
    }
}