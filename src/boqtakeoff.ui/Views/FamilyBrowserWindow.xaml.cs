using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using boqtakeoff.core.Libraries;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace boqtakeoff.ui.Views
{
    public partial class FamilyBrowserWindow : Window
    {
        private readonly Document _doc;
        private readonly UIDocument _uidoc;
        private S3FamilyLibraryService _s3Service;
        private FamilyPlacementService _placementService;
        private List<FamilyMetadata> _allFamilies;
        private FamilyMetadata _selectedFamily;

        public FamilyBrowserWindow(ExternalCommandData commandData)
        {
            InitializeComponent();

            _doc = commandData.Application.ActiveUIDocument.Document;
            _uidoc = commandData.Application.ActiveUIDocument;

            InitializeServices();
            LoadFamiliesAsync();
        }

        /// <summary>
        /// Initialize S3 and placement services
        /// </summary>
        private void InitializeServices()
        {
            try
            {
                // Read configuration from app.config
                string bucketName = ConfigurationManager.AppSettings["AWS_S3_BucketName"];
                string accessKey = ConfigurationManager.AppSettings["AWS_AccessKey"];
                string secretKey = ConfigurationManager.AppSettings["AWS_SecretKey"];
                string region = ConfigurationManager.AppSettings["AWS_S3_Region"] ?? "us-east-1";

                if (string.IsNullOrEmpty(bucketName) || string.IsNullOrEmpty(accessKey) || string.IsNullOrEmpty(secretKey))
                {
                    throw new Exception("AWS configuration not found in app.config");
                }

                _s3Service = new S3FamilyLibraryService(bucketName, accessKey, secretKey, region);
                _placementService = new FamilyPlacementService(_doc, _uidoc, _s3Service);

                UpdateStatus("Services initialized successfully");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing services: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Utility.Logger(ex);
            }
        }

        /// <summary>
        /// Load families from S3
        /// </summary>
        private async void LoadFamiliesAsync()
        {
            try
            {
                ShowLoading(true);
                UpdateStatus("Loading families from library...");

                // Load folder structure
                var folders = await _s3Service.GetFolderStructureAsync();
                PopulateFolderTree(folders);

                // Load all families
                _allFamilies = await _s3Service.GetFamilyListAsync();

                UpdateFamilyList(_allFamilies);
                UpdateStatus($"Loaded {_allFamilies.Count} families");

                ShowLoading(false);
            }
            catch (Exception ex)
            {
                ShowLoading(false);
                UpdateStatus("Error loading families");
                MessageBox.Show($"Error loading families: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Utility.Logger(ex);
            }
        }

        /// <summary>
        /// Populate folder tree view
        /// </summary>
        private void PopulateFolderTree(List<FolderNode> folders)
        {
            treeViewFolders.Items.Clear();

            // Add "All Families" root node
            var allNode = new TreeViewItem
            {
                Header = CreateFolderHeader("All Families", _allFamilies?.Count ?? 0),
                Tag = null
            };
            treeViewFolders.Items.Add(allNode);

            // Add folder nodes
            foreach (var folder in folders)
            {
                var treeItem = CreateTreeViewItem(folder);
                treeViewFolders.Items.Add(treeItem);
            }

            // Expand all root nodes
            allNode.IsExpanded = true;
        }

        /// <summary>
        /// Create tree view item from folder node
        /// </summary>
        private TreeViewItem CreateTreeViewItem(FolderNode folder)
        {
            var item = new TreeViewItem
            {
                Header = CreateFolderHeader(folder.Name, folder.FileCount),
                Tag = folder
            };

            foreach (var child in folder.Children.Values)
            {
                item.Items.Add(CreateTreeViewItem(child));
            }

            return item;
        }

        /// <summary>
        /// Create folder header with name and count
        /// </summary>
        private StackPanel CreateFolderHeader(string name, int count)
        {
            var panel = new StackPanel { Orientation = Orientation.Horizontal };

            panel.Children.Add(new TextBlock
            {
                Text = "📁 ",
                FontSize = 14
            });

            panel.Children.Add(new TextBlock
            {
                Text = name,
                FontSize = 12
            });

            if (count > 0)
            {
                panel.Children.Add(new TextBlock
                {
                    Text = $" ({count})",
                    FontSize = 11,
                    Foreground = System.Windows.Media.Brushes.Gray,
                    Margin = new Thickness(5, 0, 0, 0)
                });
            }

            return panel;
        }

        /// <summary>
        /// Update family list view
        /// </summary>
        private void UpdateFamilyList(List<FamilyMetadata> families)
        {
            listViewFamilies.ItemsSource = null;
            listViewFamilies.ItemsSource = families.OrderBy(f => f.FileName).ToList();
        }

        /// <summary>
        /// Folder selection changed
        /// </summary>
        private void treeViewFolders_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                var selectedItem = e.NewValue as TreeViewItem;
                if (selectedItem == null) return;

                var folder = selectedItem.Tag as FolderNode;

                if (folder == null)
                {
                    // "All Families" selected
                    UpdateFamilyList(_allFamilies);
                    UpdateStatus($"Showing all families ({_allFamilies.Count})");
                }
                else
                {
                    // Filter by folder
                    var filteredFamilies = _allFamilies
                        .Where(f => f.FolderPath.StartsWith(folder.FullPath))
                        .ToList();

                    UpdateFamilyList(filteredFamilies);
                    UpdateStatus($"Showing {filteredFamilies.Count} families in {folder.Name}");
                }
            }
            catch (Exception ex)
            {
                Utility.Logger(ex);
            }
        }

        /// <summary>
        /// Family selection changed
        /// </summary>
        private void listViewFamilies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                _selectedFamily = listViewFamilies.SelectedItem as FamilyMetadata;

                if (_selectedFamily == null)
                {
                    ShowFamilyDetails(null);
                }
                else
                {
                    ShowFamilyDetails(_selectedFamily);
                }
            }
            catch (Exception ex)
            {
                Utility.Logger(ex);
            }
        }

        /// <summary>
        /// Show family details in panel
        /// </summary>
        private void ShowFamilyDetails(FamilyMetadata family)
        {
            if (family == null)
            {
                txtNoSelection.Visibility = Visibility.Visible;
                panelFamilyDetails.Visibility = Visibility.Collapsed;
                return;
            }

            txtNoSelection.Visibility = Visibility.Collapsed;
            panelFamilyDetails.Visibility = Visibility.Visible;

            txtFileName.Text = family.FileName;
            txtCategory.Text = family.Category;
            txtFolderPath.Text = family.FolderPath;
            txtSize.Text = family.SizeFormatted;
            txtLastModified.Text = family.LastModified.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// Search button clicked
        /// </summary>
        private async void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string searchTerm = txtSearch.Text.Trim();

                if (string.IsNullOrEmpty(searchTerm))
                {
                    UpdateFamilyList(_allFamilies);
                    UpdateStatus($"Showing all families ({_allFamilies.Count})");
                    return;
                }

                ShowLoading(true);
                UpdateStatus($"Searching for '{searchTerm}'...");

                var results = await _s3Service.SearchFamiliesAsync(searchTerm);

                UpdateFamilyList(results);
                UpdateStatus($"Found {results.Count} families matching '{searchTerm}'");

                ShowLoading(false);
            }
            catch (Exception ex)
            {
                ShowLoading(false);
                MessageBox.Show($"Search error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Utility.Logger(ex);
            }
        }

        /// <summary>
        /// Refresh button clicked
        /// </summary>
        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = string.Empty;
            LoadFamiliesAsync();
        }

        /// <summary>
        /// Add to project button clicked
        /// </summary>
        private async void btnAddToProject_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedFamily == null)
            {
                MessageBox.Show("Please select a family first", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                UpdateStatus($"Adding {_selectedFamily.FileName} to project...");
                btnAddToProject.IsEnabled = false;

                // Hide window temporarily so user can see Revit view
                this.Hide();

                // Load and place family
                bool success = await _placementService.LoadAndPlaceFamilyAsync(_selectedFamily);

                // Show window again
                this.Show();
                btnAddToProject.IsEnabled = true;

                if (success)
                {
                    UpdateStatus($"Successfully added {_selectedFamily.FileName}");
                    MessageBox.Show($"Family '{_selectedFamily.FileName}' has been added to the project",
                        "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    UpdateStatus("Operation cancelled or failed");
                }
            }
            catch (Exception ex)
            {
                this.Show();
                btnAddToProject.IsEnabled = true;
                UpdateStatus("Error adding family");
                MessageBox.Show($"Error adding family: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Utility.Logger(ex);
            }
        }

        /// <summary>
        /// Close button clicked
        /// </summary>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Show/hide loading message
        /// </summary>
        private void ShowLoading(bool show)
        {
            txtLoadingMessage.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Update status message
        /// </summary>
        private void UpdateStatus(string message)
        {
            txtStatusMessage.Text = message;
        }

        /// <summary>
        /// Window closing - cleanup
        /// </summary>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                _s3Service?.Dispose();
            }
            catch (Exception ex)
            {
                Utility.Logger(ex);
            }

            base.OnClosing(e);
        }
    }
}