using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using RestSharp;
using System.Text.Json;
using System.IO;
using OfficeOpenXml;
using boqtakeoff.core.Libraries;
using boqtakeoff.core;

namespace boqtakeoff.ui.Windows
{
    public partial class DrawingScheduleWidget : Window
    {
        private string _projectId;
        private List<getDetailedDrawingDetails> _drawingDetails;

        // TODO: Update these URLs with your actual API endpoints
        private const string _get_drawing_records_in_schedule_url = "https://creator.zoho.in/api/v2/bigfish.central/drawing-schedule/report/All_Drawing_Records";
        private const string _get_drawing_record_details_url = "https://creator.zoho.in/api/v2/bigfish.central/drawing-schedule/report/Drawing_Record_Details";

        public DrawingScheduleWidget(string projectId)
        {
            InitializeComponent();
            _projectId = projectId;
            _drawingDetails = new List<getDetailedDrawingDetails>();

            // Load data when window is loaded
            this.Loaded += DrawingScheduleWidget_Loaded;
        }

        private async void DrawingScheduleWidget_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadDrawingScheduleAsync();
        }

        private async Task LoadDrawingScheduleAsync()
        {
            try
            {
                // Show loading state
                ShowLoadingState(true);
                txtStatus.Text = "Loading drawing schedule...";

                // Load data in background thread
                await Task.Run(() => LoadDrawingSchedule());

                // Update UI on main thread
                dgDrawingSchedule.ItemsSource = _drawingDetails;
                txtRecordCount.Text = _drawingDetails.Count.ToString();
                txtStatus.Text = $"Loaded {_drawingDetails.Count} records successfully";

                ShowLoadingState(false);
            }
            catch (Exception ex)
            {
                ShowLoadingState(false);
                txtStatus.Text = "Error loading data";
                MessageBox.Show($"Error loading drawing schedule: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Utility.Logger(ex);
            }
        }

        private void LoadDrawingSchedule()
        {
            try
            {
                string access_token = BigFishRestAPIs.get_access_token();
                List<DesignScheduleRecord> drawing_record_list = GetDrawingScheduleDetailsList(_projectId, access_token);
                List<getDetailedDrawingDetails> detailed_drawing_details = new List<getDetailedDrawingDetails>();

                int processedCount = 0;
                foreach (DesignScheduleRecord curr_project_data in drawing_record_list)
                {
                    processedCount++;

                    // Update progress on UI thread
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        txtStatus.Text = $"Processing record {processedCount} of {drawing_record_list.Count}...";
                    });

                    string record_id = curr_project_data.ID;
                    string criteria = "ID == " + record_id;
                    int from = 0;
                    int limit = 1;
                    string get_url = _get_drawing_record_details_url + "?from=" + from + "&limit=" + limit + "&criteria=" + criteria;
                    string auth_token = "Zoho-oauthtoken " + access_token;

                    var client = new RestClient(get_url);
                    var request = new RestRequest(get_url, Method.GET);
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
                        // Handle error response
                        string errorMsg = content_obj.message ?? "Unknown error occurred";
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show(errorMsg,
                                "Response from Bigfish API",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                        });
                        break;
                    }
                }

                _drawingDetails = detailed_drawing_details;
            }
            catch (Exception exp)
            {
                Utility.Logger(exp);
                throw;
            }
        }

        private List<DesignScheduleRecord> GetDrawingScheduleDetailsList(string project_id, string access_token)
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
                var request = new RestRequest(get_url, Method.GET);
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
                    string errorMsg = content_obj.description ?? content_obj.message ?? "Unknown error";
                    MessageBox.Show(errorMsg,
                        "Response from Bigfish Project List",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    break;
                }
            }

            return drawing_schedule_details_list;
        }

        private async void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            await LoadDrawingScheduleAsync();
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_drawingDetails == null || _drawingDetails.Count == 0)
                {
                    MessageBox.Show("No data to export.", "Export",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Create Save File Dialog
                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    Title = "Export Drawing Schedule",
                    FileName = $"DrawingSchedule_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    ExportToExcel(saveFileDialog.FileName);
                    MessageBox.Show($"Data exported successfully to:\n{saveFileDialog.FileName}",
                        "Export Successful",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting data: {ex.Message}",
                    "Export Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Utility.Logger(ex);
            }
        }

        private void ExportToExcel(string filePath)
        {
            // Fix for LicenseContext ambiguity - use fully qualified name
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Drawing Schedule");

                // Add headers
                worksheet.Cells[1, 1].Value = "Drawing Number";
                worksheet.Cells[1, 2].Value = "Action";
                worksheet.Cells[1, 3].Value = "Description";
                worksheet.Cells[1, 4].Value = "Project Specificity";
                worksheet.Cells[1, 5].Value = "Planned Start Date";
                worksheet.Cells[1, 6].Value = "Upload Due Date";
                worksheet.Cells[1, 7].Value = "Progress";
                worksheet.Cells[1, 8].Value = "Digital Designer Name";
                worksheet.Cells[1, 9].Value = "Drawing File";

                // Style headers
                using (var range = worksheet.Cells[1, 1, 1, 9])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                // Add data
                int row = 2;
                foreach (var item in _drawingDetails)
                {
                    worksheet.Cells[row, 1].Value = item.Drawing_Number;
                    worksheet.Cells[row, 2].Value = item.action_field;
                    worksheet.Cells[row, 3].Value = item.Description;
                    worksheet.Cells[row, 4].Value = item.Project_Specificity;
                    worksheet.Cells[row, 5].Value = item.Planned_Start_Date;
                    worksheet.Cells[row, 6].Value = item.Upload_Due_date;
                    worksheet.Cells[row, 7].Value = item.Progress;
                    worksheet.Cells[row, 8].Value = item.Digital_Designer_Name;
                    worksheet.Cells[row, 9].Value = item.Design_File;
                    row++;
                }

                // Auto-fit columns
                worksheet.Cells.AutoFitColumns();

                // Save the file
                FileInfo file = new FileInfo(filePath);
                package.SaveAs(file);
            }
        }

        private void ShowLoadingState(bool isLoading)
        {
            progressBar.Visibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
            btnRefresh.IsEnabled = !isLoading;
            btnExport.IsEnabled = !isLoading;
        }
    }

    #region Data Models

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
        public string message { get; set; }
    }

    public class getDetailedDrawingSubFormAPIResponse
    {
        public int code { get; set; }
        public IList<getDetailedDrawingDetails> data { get; set; }
        public string description { get; set; }
        public string message { get; set; }
    }

    public class getDetailedDrawingDetails : INotifyPropertyChanged
    {
        private string _drawing_Number;
        private string _action_field;
        private string _description;
        private string _project_Specificity;
        private string _planned_Start_Date;
        private string _upload_Due_date;
        private string _progress;
        private string _digital_Designer_Name;
        private string _design_File;

        public string Approved_on_by_Project_director_design { get; set; }

        public string Progress
        {
            get => _progress;
            set
            {
                _progress = value;
                OnPropertyChanged(nameof(Progress));
            }
        }

        public string Submission_date { get; set; }

        public string Planned_Start_Date
        {
            get => _planned_Start_Date;
            set
            {
                _planned_Start_Date = value;
                OnPropertyChanged(nameof(Planned_Start_Date));
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public string Project_Specificity
        {
            get => _project_Specificity;
            set
            {
                _project_Specificity = value;
                OnPropertyChanged(nameof(Project_Specificity));
            }
        }

        public string action_field
        {
            get => _action_field;
            set
            {
                _action_field = value;
                OnPropertyChanged(nameof(action_field));
            }
        }

        public string Drawing_Number
        {
            get => _drawing_Number;
            set
            {
                _drawing_Number = value;
                OnPropertyChanged(nameof(Drawing_Number));
            }
        }

        public string TAT_timeline { get; set; }

        public string Upload_Due_date
        {
            get => _upload_Due_date;
            set
            {
                _upload_Due_date = value;
                OnPropertyChanged(nameof(Upload_Due_date));
            }
        }

        public string Approval_status_by_Project_director_design { get; set; }
        public string Remark { get; set; }

        public string Digital_Designer_Name
        {
            get => _digital_Designer_Name;
            set
            {
                _digital_Designer_Name = value;
                OnPropertyChanged(nameof(Digital_Designer_Name));
            }
        }

        public string Working_date { get; set; }
        public string actual_date { get; set; }
        public string project_id { get; set; }
        public string Version { get; set; }

        public string Design_File
        {
            get => _design_File;
            set
            {
                _design_File = value;
                OnPropertyChanged(nameof(Design_File));
            }
        }

        public string ID { get; set; }
        public string New_File_Uploaded { get; set; }
        public string Submit1 { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    #endregion
}