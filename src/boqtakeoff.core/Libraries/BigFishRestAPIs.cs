using RestSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using System.Windows.Forms;
using boqtakeoff.core.Libraries;

namespace boqtakeoff.core
{

    #region Properties


    /// <summary>
    /// ///////////////////
    /// </summary>
    public class getProjectVersionRecord
    {
        public string bf_version_number { get; set; }
        public string boq_number { get; set; }
        public string project_id { get; set; }
        public string version_number { get; set; }
        public string boq_unique_key { get; set; }
        public string customer_id { get; set; }
        public string boq_id { get; set; }
        public string ID { get; set; }
    }

    public class getProjectVersionListAPIResponse
    {
        public int code { get; set; }
        public IList<getProjectVersionRecord> data { get; set; }
        public string description { get; set; }
    }

    /// <summary>
    /// ///////////////////
    /// </summary>
    public class getErrorLog
    {
        public string sku_label { get; set; }
        public string quantity { get; set; }
        public string uom { get; set; }
        public string error_log { get; set; }
        public string project_uid { get; set; }
    }

    public class getBFErrorLogAPIResponse
    {
        public int code { get; set; }
        public IList<getErrorLog> data { get; set; }
        public string description { get; set; }
    }


    /// <summary>
    /// ///////////////////
    /// </summary>
    public class getProjectDetails
    {
        public string studio_name { get; set; }
        public string project_id { get; set; }
        public string project_key { get; set; }
        public string project_group_name { get; set; }
        public string project_name { get; set; }
    }
    
    public class getProjectDetailsAPIResponse
    {
        public int code { get; set; }
        public IList<getProjectDetails> data { get; set; }
        public string description { get; set; }
    }

    public class DesignScheduleRecord
    {
        public string display_value { get; set; }
        public string ID { get; set; }
    }

    public class getProjectDrawingRecordDetails
    {
        public string user_email { get; set; }
        public IList<DesignScheduleRecord> design_schedule { get; set; }
    }

    public class getDetailedDrawingMainFormAPIResponse
    {
        public int code { get; set; }
        public IList<getProjectDrawingRecordDetails> data { get; set; }
        public string description { get; set; }
    }

    public class getDetailedDrawingSubFormAPIResponse
    {
        public int code { get; set; }
        public IList<getDetailedDrawingDetails> data { get; set; }
    }

    public class getDetailedDrawingDetails
    {
        public string Approved_on_by_Project_director_design { get; set; }
        public string Progress { get; set; }
        public string Submission_date { get; set; }
        public string Planned_Start_Date { get; set; }
        public string Description { get; set; }
        public string Project_Specificity { get; set; }
        public string action_field { get; set; }
        public string Drawing_Number { get; set; }
        public string TAT_timeline { get; set; }
        public string Upload_Due_date { get; set; }
        public string Approval_status_by_Project_director_design { get; set; }
        public string Remark { get; set; }
        public string Digital_Designer_Name { get; set; }
        public string Working_date { get; set; }
        public string actual_date { get; set; }
        public string project_id { get; set; }
        public string Version { get; set; }
        public string Design_File { get; set; }
        public string ID { get; set; }
        public string New_File_Uploaded { get; set; }
        public string Submit1 { get; set; }
       
    }


    /// <summary>
    /// ///////////////////
    /// </summary>

    public class BoqLineItemDetails
    {
        public string item_name { get; set; }
        public string level_name { get; set; }
        public string level_id { get; set; }
        public string delivery_area { get; set; }
        public string room_number { get; set; }
        public string item_sku { get; set; }
        public string quantity { get; set; }
        public string item_category { get; set; }
        public string mapped_sku_label { get; set; }
        public string project_id { get; set; }
        public string version_number { get; set; }

        public string uom { get; set; }
        public List<getErrorLog> error_log { get; set; }

    }
    public class DataBody
    {
        public string project_name { get; set; }
        public string project_key { get; set; }
        public string project_id { get; set; }
        public string version_number { get; set; }
        public string merge_version { get; set; }
        public string merge_required { get; set; }
        public string user_id { get; set; }
        public List<BoqLineItemDetails> create_boq_details { get; set; }
    }
    public class DataMap
    {
        public DataBody data { get; set; }
    }
    /// <summary>
    /// Response from the Creator API
    /// </summary>

    public class ResponseData
    {
        public string ID { get; set; }
    }

    public class ErrorDataCancelSubmit
    {
        public string task { get; set; } // returned in error block for cancel submit event in BF
        public string message { get; set; } // returned in error block for cancel submit event in BF
    }

    public class ErrorDataMandatoryChecks
    {
        public string project_name { get; set; } // returned in error block if mandatory field is missing
        public string item_sku { get; set; } // returned in error block if mandatory field is missing
    }

    public class ResultJson
    {
        public int code { get; set; }
        public ResponseData data { get; set; }
        public List<ErrorDataCancelSubmit> error { get; set; }
        public string message { get; set; }
    }
    public class ResultJsonMandatoryChecks
    {
        public int code { get; set; }
        public ResponseData data { get; set; }
        public ErrorDataMandatoryChecks error { get; set; }
        public string message { get; set; }
    }
    public class ResultMap
    {
        public List<ResultJson> result { get; set; }
        public int code { get; set; }
        public string description { get; set; }
    }

    /// <summary>
    /// ///////////////////
    /// </summary>
    public class AuthTokenResponseContent
    {
        public string access_token { get; set; }
        public string api_domain { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
    }
    /// <summary>
    /// Zoho Creator API response
    /// </summary>

    #endregion

    /// Summary
    /// SANINFO:: The method shall be used by using its reference
    /// SANINFO:: It consolidates the qty of an SKU in the model
    /// Summary

    public class MapSKUToSKUDetails
        : Dictionary<string, BoqLineItemDetails>
    {
        public void Cumulate(
          string key,
          string value,
          BoqLineItemDetails boq_details)
        {
            if (ContainsKey(key))
            {
                BoqLineItemDetails existing_boq_details = this[key];
                String existing_qty = existing_boq_details.quantity;
                double sanitized_existing_qty = 1.0;
                double sanitized_new_qty = 1.0;
                if (existing_qty != null)
                {
                    sanitized_existing_qty = Double.Parse(existing_qty);
                }
                if (value != null)
                {
                    sanitized_new_qty = Double.Parse(value);
                }
                double consolidated_qty = sanitized_existing_qty + sanitized_new_qty;

                existing_boq_details.quantity = consolidated_qty.ToString();
                this[key] = existing_boq_details;
            }
            else
            {
                // INFO:: For items that are only used once in model and that don't have quantity explicitly defined, we need to store the quantity
                if (boq_details.quantity == null)
                {
                    boq_details.quantity = "1.0";
                }
                this.Add(key, boq_details);
            }
        }
    }


    public class BigFishRestAPIs
    {

        static readonly string _refresh_token = Properties.Resources._refresh_token;
        static readonly string _client_id = Properties.Resources._client_id;
        static readonly string _client_secret = Properties.Resources._client_secret;

        static readonly string _token_url = Properties.Resources._token_url;
        static readonly string _create_boq_url = Properties.Resources._create_boq_url;
        static readonly string _get_boq_version_details_url = Properties.Resources._get_boq_version_details_url;
        static readonly string _get_project_uid_url = Properties.Resources._get_project_uid_url;
        static readonly string _get_bf_error_log_url = Properties.Resources._get_bf_error_log_url;

        static readonly string _get_drawing_records_in_schedule_url = Properties.Resources._get_drawing_records_in_schedule_url;
        static readonly string _get_drawing_record_details_url = Properties.Resources._get_drawing_record_details_url;

        static Hashtable errorMappings = new Hashtable();
        /// <summary>
        /// HTTP access constant to toggle 
        /// between local and global server.
        /// </summary>
        // public static bool UseLocalServer = false;
        // HTTP access constants.
        public static List<BoqLineItemDetails> ConsolidateSKUQuantities(List<BoqLineItemDetails> create_boq_details)
        {
            MapSKUToSKUDetails SkuHashSkuDetails = new MapSKUToSKUDetails();
            foreach (BoqLineItemDetails curr_boq_lid in create_boq_details)
            {
                String key = curr_boq_lid.item_sku + ": " + curr_boq_lid.delivery_area;
                if (curr_boq_lid.item_sku == null)
                {
                    curr_boq_lid.item_sku = curr_boq_lid.item_name;
                    key = curr_boq_lid.item_name + ": " + curr_boq_lid.delivery_area;
                }
                // if (key == null)
                // {
                //    continue;
                // }
                String value = curr_boq_lid.quantity;
                // TaskDialog.Show("curr_boq_lid details ... ", curr_boq_lid.ToString());
                SkuHashSkuDetails.Cumulate(key, value, curr_boq_lid);
            }
            List<BoqLineItemDetails> consolidated_boq_details = new List<BoqLineItemDetails>();
            ICollection sku_list = SkuHashSkuDetails.Keys;
            foreach (String curr_sku in sku_list)
            {
                // TaskDialog.Show("curr_sku ... ", curr_sku.ToString());
                consolidated_boq_details.Add(SkuHashSkuDetails[curr_sku.ToString()]);
            }
            return consolidated_boq_details;
        }
        
        /// <summary>
        /// HTTP access constant to toggle 
        /// between local and global server.
        /// </summary>
        // public static bool UseLocalServer = false;
        // HTTP access constants.
        public static List<getErrorLog> PostBOQDetails2ZCreator(List<BoqLineItemDetails> boq_lid_details, string project_name, string project_key, string project_id, string version_number, string merge_required, string merge_version, string userId)
        {


            string access_token = get_access_token();

            string responseMessage = "";

            DataMap data_map = new DataMap();
            data_map.data = new DataBody();
            data_map.data.project_name = project_name;
            data_map.data.project_id = project_id;
            data_map.data.project_key = project_key;
            data_map.data.version_number = version_number;
            data_map.data.merge_required = merge_required;
            data_map.data.merge_version = merge_version;
            data_map.data.user_id = userId;

            data_map.data.create_boq_details = new List<BoqLineItemDetails>();

            // Consolidating the SKUs in BOQ list
            List<BoqLineItemDetails> consolidated_boq_details = ConsolidateSKUQuantities(boq_lid_details);
            data_map.data.create_boq_details = consolidated_boq_details;

            String stringjson = JsonSerializer.Serialize(data_map);
            string auth_token = "Zoho-oauthtoken " + access_token;

            var client = new RestClient(_create_boq_url);
            var request = new RestRequest(_create_boq_url, Method.Post);
            request.AddHeader("Authorization", auth_token);
            // request.AddHeader("Content-Type", "application/json");
            // request.AddHeader("accept", "application/json");
            request.AddJsonBody(stringjson);

            var response = client.Execute(request);
            string responseJSON = response.Content;

            ResultJson content_obj = JsonSerializer.Deserialize<ResultJson>(responseJSON);
            int response_code = content_obj.code;

            List<getErrorLog> error_log_list_obj = new List<getErrorLog>();
            if (response_code == 3000)
            {
                // TODO:: The content_obj.result.message is throwing error that the reference is not set to its object instance ...
                responseMessage = content_obj.message;
                // MessageBox.Show(responseMessage, "Response from Bigfish ...", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (response_code == 3001)
            {

                // INFO:: Cancel Submit event invoked in BF validation
                // INFO:: The data is not added in any of the tables
                List<ErrorDataCancelSubmit> error_data_json = content_obj.error;
                string validation_task = error_data_json[0].task;
                responseMessage = error_data_json[0].message;
                error_log_list_obj = getBFErrorLog(access_token, version_number);

                // MessageBox.Show(responseMessage, "Response from Bigfish ...", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (response_code == 3002)
            {
                // INFO:: Mandatory field missing
                // INFO:: The data is not added in any of the tables
                // Implement a function that returns the mandatory keys in the error bloc

                ResultJsonMandatoryChecks content_obj_mandatory_checks = JsonSerializer.Deserialize<ResultJsonMandatoryChecks>(responseJSON);
                ErrorDataMandatoryChecks error_json = content_obj_mandatory_checks.error;
                // MessageBox.Show(responseMessage, "Response from Bigfish ...", MessageBoxButtons.OK, MessageBoxIcon.Information);

                error_log_list_obj = getBFErrorLog(access_token, version_number);
            }
            else
            {
                // responseMessage = content_obj.description;
                // MessageBox.Show(responseMessage, "Response from Bigfish ...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                error_log_list_obj = getBFErrorLog(access_token, version_number);
            }

            if (error_log_list_obj.Count > 0)
            {
                errorMappings = Utility.LoadErrorMappingFromConfiguration();

                foreach (getErrorLog item in error_log_list_obj)
                {
                    if(item.error_log== "missing_sku_definition")
                    {
                        item.error_log = (string)errorMappings["missing_sku_definition"];
                    }
                }
            }
            // Add checks for refresh token expiry
            // URL change
            // scope not set
            //return response.StatusCode;
            // TaskDialog.Show("error_log_list_obj ... ", error_log_list_obj[0].sku_label.ToString());
            return error_log_list_obj;
        }

        public static List<DesignScheduleRecord> getDrawingScheduleDetailsList(string project_id, string access_token)
        {
            int max_itr = 10;
            int query_range = 200;
            string criteria = "project_id == " + project_id;

            List<getProjectDrawingRecordDetails> drawing_record_details = new List<getProjectDrawingRecordDetails>();
            List<DesignScheduleRecord> drawing_schedule_details_list = new List<DesignScheduleRecord>();

            for (int i = 0; i < max_itr; i++)
            {
                int from = i * query_range + 1;
                int limit = query_range;
                string get_url = _get_drawing_records_in_schedule_url + "?from=" + from + "&limit=" + limit + "&criteria=" + criteria;
                string auth_token = "Zoho-oauthtoken " + access_token;

                var client = new RestClient(get_url);
                var request = new RestRequest(get_url, Method.Get);
                request.AddHeader("Authorization", auth_token);

                var response = client.Execute(request);

                var content_obj = JsonSerializer.Deserialize<getDetailedDrawingMainFormAPIResponse>(response.Content);

                if (content_obj == null)
                    break;

                int response_code = content_obj.code;

                if (response_code == 3000)
                {
                    var dataList = content_obj.data;
                    if (dataList != null && dataList.Count > 0)
                    {
                        drawing_record_details.AddRange(dataList);

                        foreach (var record in dataList)
                        {
                            if (record.design_schedule != null)
                                drawing_schedule_details_list.AddRange(record.design_schedule);
                        }
                    }
                }
                else if (response_code == 3100)
                {
                    break; // End of data
                }
                else
                {
                    MessageBox.Show(content_obj.description, "Response from Bigfish Project List ...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
            }

            return drawing_schedule_details_list;
        }

        public void getDrawingSchedule(string project_id)
        {
            try
            {
                string access_token = BigFishRestAPIs.get_access_token();
                List<DesignScheduleRecord> drawing_record_list = getDrawingScheduleDetailsList(project_id, access_token);
                List<getDetailedDrawingDetails> detailed_drawing_details = new List<getDetailedDrawingDetails>();
                
                foreach (DesignScheduleRecord curr_project_data in drawing_record_list)
                {
                    string record_id = curr_project_data.ID;
                    string criteria = "ID == " + record_id;

                    int from = 0;
                    int limit = 1;
                    string get_url = _get_drawing_record_details_url + "?from=" + from + "&limit=" + limit + "&criteria=" + criteria;
                    string auth_token = "Zoho-oauthtoken " + access_token;

                    var client = new RestClient(get_url);
                    var request = new RestRequest(get_url, Method.Get);
                    request.AddHeader("Authorization", auth_token);

                    var response = client.Execute(request);

                    var content_obj = JsonSerializer.Deserialize<getDetailedDrawingSubFormAPIResponse>(response.Content);

                    if (content_obj == null)
                        break;

                    int response_code = content_obj.code;

                    if (response_code == 3000)
                    {
                        var dataList = content_obj.data;
                        if (dataList != null && dataList.Count > 0)
                        {
                            detailed_drawing_details.AddRange(dataList);

                        }
                    }
                    else if (response_code == 3100)
                    {
                        break; // End of data
                    }
                    else
                    {
                        // MessageBox.Show(content_obj.description, "Response from Bigfish Project List ...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }
                }
            }
            catch (Exception exp)
            {
                Utility.Logger(exp);
            }
        }

        public static List<getProjectDetails> getProjectNameList(string access_token)
        {
            int max_itr = 10;
            int query_range = 200;
            string criteria = "ID != 0";
            List<getProjectDetails> project_data_list = new List<getProjectDetails>();

            for (int i = 0; i < max_itr; i++)
            {
                int from = i * query_range + 1;
                int limit = query_range;
                string get_url = _get_project_uid_url + "?from=" + from + "&limit=" + limit + "&criteria=" + criteria;
                string auth_token = "Zoho-oauthtoken " + access_token;
                var client = new RestClient(get_url);
                var request = new RestRequest(get_url, Method.Get);
                request.AddHeader("Authorization", auth_token);
                // request.AddHeader("accept", "application/json");
                // request.AddHeader("Content-Type", "application/json");
                var response = client.Execute(request);

                getProjectDetailsAPIResponse content_obj = JsonSerializer.Deserialize<getProjectDetailsAPIResponse>(response.Content);

                int response_code = content_obj.code;
                string responseMessage = null;
                if (response_code == 3000)
                {
                    project_data_list.AddRange(content_obj.data);
                }
                else if (response_code == 3100)
                {
                    // INFO:: We have come to the end of the project list. Time to break from the loop ...
                    break;
                }
                else
                {
                    // INFO:: If the response code is not 3000 or 3100, we have encountered some API related error ...
                    responseMessage = content_obj.description;
                    MessageBox.Show(responseMessage, "Response from Bigfish Project List ...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // TODO :: If we don't get the project list, the app will not function
                    break;
                }
            }
            return project_data_list;
        }

        public static List<getErrorLog> getBFErrorLog(string access_token, string project_uid)
        {
            int max_itr = 10;
            int query_range = 200;
            string criteria = "project_uid == " + "\"" + project_uid + "\"";
            List<getErrorLog> error_log_list = new List<getErrorLog>();
            for (int i = 0; i < max_itr; i++)
            {
                int from = i * query_range + 1;
                int limit = query_range;
                string get_url = _get_bf_error_log_url + "?from=" + from + "&limit=" + limit + "&criteria=" + criteria;
                string auth_token = "Zoho-oauthtoken " + access_token;
                var client = new RestClient(get_url);
                var request = new RestRequest(get_url, Method.Get);
                request.AddHeader("Authorization", auth_token);
                // request.AddHeader("accept", "application/json");
                // request.AddHeader("Content-Type", "application/json");
                var response = client.Execute(request);

                getBFErrorLogAPIResponse content_obj = JsonSerializer.Deserialize<getBFErrorLogAPIResponse>(response.Content);

                int response_code = content_obj.code;
                string responseMessage = null;
                if (response_code == 3000)
                {
                    error_log_list.AddRange(content_obj.data);
                }
                else if (response_code == 3100)
                {
                    // INFO:: We have come to the end of the project list. Time to break from the loop ...
                    break;
                }
                else
                {
                    // INFO:: If the response code is not 3000 or 3100, we have encountered some API related error ...
                    responseMessage = content_obj.description;
                    // MessageBox.Show(responseMessage, "Response from Bigfish Error Log ...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // TODO :: If we don't get the project list, the app will not function
                    break;
                }
            }
            return error_log_list;
        }

        public static List<string> getProjectBOQVersionList(string project_id)
        {
            string access_token = get_access_token();
            int max_itr = 10;
            int query_range = 200;
            string criteria = "project_id == " + "\"" + project_id + "\"";
            List<getProjectVersionRecord> project_boq_version_data_list = new List<getProjectVersionRecord>();
            for (int i = 0; i < max_itr; i++)
            {
                int from = i * query_range + 1;
                int limit = query_range;
                string get_url = _get_boq_version_details_url + "?from=" + from + "&limit=" + limit + "&criteria=" + criteria;
                string auth_token = "Zoho-oauthtoken " + access_token;
                var client = new RestClient(get_url);
                var request = new RestRequest(get_url, Method.Get);
                request.AddHeader("Authorization", auth_token);
                // request.AddHeader("accept", "application/json");
                // request.AddHeader("Content-Type", "application/json");
                var response = client.Execute(request);
                //MessageBox.Show("response.Content:" + response.Content);
                getProjectVersionListAPIResponse content_obj = JsonSerializer.Deserialize<getProjectVersionListAPIResponse>(response.Content);

                int response_code = content_obj.code;
                string responseMessage = null;
                if (response_code == 3000)
                {
                    project_boq_version_data_list.AddRange(content_obj.data);
                }
                else if (response_code == 3100)
                {
                    // INFO:: We have come to the end of the project list. Time to break from the loop ...
                    break;
                }
                else
                {
                    // INFO:: If the response code is not 3000 or 3100, we have encountered some API related error ...
                    responseMessage = content_obj.description;
                    // MessageBox.Show(responseMessage, "Response from Bigfish Error Log ...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    // TODO :: If we don't get the project list, the app will not function
                    break;
                }
            }
            List<string> project_version_list = new List<string>();
            foreach (getProjectVersionRecord curr_boq_version_data in project_boq_version_data_list)
            {
                project_version_list.Add(curr_boq_version_data.version_number);
            }
            return project_version_list;
        }

        public static string get_access_token()
        {
            var client = new RestClient(_token_url);
            var request = new RestRequest(_token_url, Method.Post);
            // request.AddHeader("content-type", "application/json");
            // request.AddHeader("authorization", "Bearer " & Grant_Token);
            // request.AddHeader("accept", "application/json");

            request.AddParameter("client_id", _client_id, ParameterType.GetOrPost);
            request.AddParameter("client_secret", _client_secret, ParameterType.GetOrPost);
            request.AddParameter("refresh_token", _refresh_token, ParameterType.GetOrPost);
            request.AddParameter("grant_type", "refresh_token", ParameterType.GetOrPost);
            var response = client.Execute(request);

            AuthTokenResponseContent content_obj = JsonSerializer.Deserialize<AuthTokenResponseContent>(response.Content);
            string access_token_str = content_obj.access_token;
            return access_token_str;
        }
    }
}
