using System;
using System.Collections;
using System.Collections.Generic;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using boqtakeoff.ui.Windows;
using boqtakeoff.core;
using boqtakeoff.core.Libraries;

namespace boqtakeoff
{
    /// <summary>
    /// Command to show the Drawing Schedule Widget
    /// Reads project information from Revit model parameters (same pattern as BOQExtractor)
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class ShowDrawingScheduleCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                UIApplication uiApp = commandData.Application;
                UIDocument uiDoc = uiApp.ActiveUIDocument;
                Document doc = uiDoc.Document;

                // Get project information from Revit model (same as BOQExtractor does)
                string projectKey = GetProjectKeyFromRevitModel(doc);

                if (string.IsNullOrEmpty(projectKey))
                {
                    TaskDialog.Show("Project Key Not Found",
                        "Project key information is not available in this Revit model.\n\n" +
                        "Please run 'BOQ Extractor' first to set the project information, or ensure " +
                        "the SMI_PROJECT_KEY parameter exists in the project.",
                        TaskDialogCommonButtons.Ok);
                    return Result.Cancelled;
                }

                // Get project ID from API using project key
                string projectId = GetProjectIdFromProjectKey(projectKey);

                if (string.IsNullOrEmpty(projectId))
                {
                    TaskDialog.Show("Project ID Not Found",
                        $"Could not retrieve project ID for project key: {projectKey}\n\n" +
                        "Please check your BigFish project configuration.",
                        TaskDialogCommonButtons.Ok);
                    return Result.Cancelled;
                }

                // Open the Drawing Schedule widget with the project ID
                DrawingScheduleWidget widget = new DrawingScheduleWidget(projectId);
                widget.ShowDialog();

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = $"Error opening Drawing Schedule: {ex.Message}";
                TaskDialog.Show("Error", message);
                Utility.Logger(ex);
                return Result.Failed;
            }
        }

        /// <summary>
        /// Gets the project key from Revit model shared parameters
        /// Same pattern as BOQExtractor.getProjectInformationFromRevitModel()
        /// </summary>
        private string GetProjectKeyFromRevitModel(Document doc)
        {
            try
            {
                string projectKey = "";
                ProjectInfo projectInfo = doc.ProjectInformation;
                ElementId projectInfoElemId = projectInfo.Id;
                Element projectInfo_element = doc.GetElement(projectInfoElemId);

                // Get the SMI_PROJECT_KEY shared parameter
                Hashtable smi_project_key_sp = SharedParameter.getSharedParamGUID(doc, "SMI_PROJECT_KEY");

                if (smi_project_key_sp != null && smi_project_key_sp.Count > 0)
                {
                    string smi_project_key_guid_str = smi_project_key_sp["guid"].ToString();
                    Parameter parameter = projectInfo_element.get_Parameter(new Guid(smi_project_key_guid_str));

                    if (parameter != null && parameter.HasValue)
                    {
                        projectKey = parameter.AsString();
                    }
                }

                return projectKey;
            }
            catch (Exception ex)
            {
                Utility.Logger(ex);
                return "";
            }
        }

        /// <summary>
        /// Gets project ID from BigFish API using project key
        /// Same pattern as Main.cs populating project_uid_hash_project_details
        /// </summary>
        private string GetProjectIdFromProjectKey(string projectKey)
        {
            try
            {
                // Get access token
                string access_token = BigFishRestAPIs.get_access_token();

                // Get project list from API
                List<getProjectDetails> project_list = BigFishRestAPIs.getProjectNameList(access_token);

                // Find project by key
                foreach (getProjectDetails project in project_list)
                {
                    if (project.project_key == projectKey)
                    {
                        return project.project_id;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Utility.Logger(ex);
                return null;
            }
        }

        /// <summary>
        /// Gets the command path for ribbon button registration
        /// </summary>
        public static string GetPath()
        {
            return typeof(ShowDrawingScheduleCommand).Namespace + "." + nameof(ShowDrawingScheduleCommand);
        }
    }
}