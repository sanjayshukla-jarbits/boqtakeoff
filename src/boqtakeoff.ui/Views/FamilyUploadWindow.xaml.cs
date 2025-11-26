using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using boqtakeoff.core.Libraries;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace boqtakeoff.ui.Views
{
    public partial class FamilyUploadWindow : Window
    {
        private readonly Document _doc;
        private readonly UIApplication _uiApp;
        private readonly S3FamilyLibraryService _s3Service;
        private string _uploadedFilePath;
        private string _selectedCategory;
        private Dictionary<string, string> _parameters;
        private List<KeyValuePair<string, string>> _customParameters;

        public FamilyUploadWindow(ExternalCommandData commandData)
        {
            InitializeComponent();

            _doc = commandData.Application.ActiveUIDocument?.Document;
            _uiApp = commandData.Application;
            _s3Service = new S3FamilyLibraryService();
            _parameters = new Dictionary<string, string>();
            _customParameters = new List<KeyValuePair<string, string>>();

            LoadCategoriesAsync();
        }

        /// <summary>
        /// Load categories from XML service
        /// </summary>
        private async void LoadCategoriesAsync()
        {
            try
            {
                UpdateStatus("Loading categories...");
                await XmlReaderService.InitializeAsync();
                var xmlService = XmlReaderService.Instance;
                var folders = xmlService.GetAllRootCategories().ToList();
                PopulateFolderTree(folders);
                UpdateStatus("Ready");
            }
            catch (Exception ex)
            {
                UpdateStatus("Error loading categories");
                MessageBox.Show($"Error loading categories: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Utility.Logger(ex);
            }
        }

        /// <summary>
        /// Populate folder tree view
        /// </summary>
        private void PopulateFolderTree(List<FolderNode> folders)
        {
            treeViewFolders.Items.Clear();

            foreach (var folder in folders)
            {
                var treeItem = CreateTreeViewItem(folder);
                treeViewFolders.Items.Add(treeItem);
            }
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
        /// Folder selection changed
        /// </summary>
        private void treeViewFolders_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                var selectedItem = e.NewValue as TreeViewItem;
                if (selectedItem == null)
                {
                    _selectedCategory = null;
                    return;
                }

                var folder = selectedItem.Tag as FolderNode;
                if (folder != null)
                {
                    _selectedCategory = folder.Name;
                    UpdateStatus($"Selected category: {folder.Name}");
                }
            }
            catch (Exception ex)
            {
                Utility.Logger(ex);
            }
        }

        /// <summary>
        /// Upload button clicked
        /// </summary>
        private void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var openFileDialog = new OpenFileDialog
                {
                    Filter = "Revit Family Files (*.rfa)|*.rfa",
                    Title = "Select Revit Family File"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    _uploadedFilePath = openFileDialog.FileName;
                    UpdateStatus($"File selected: {Path.GetFileName(_uploadedFilePath)}");
                    ReadFamilyParametersAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Utility.Logger(ex);
            }
        }

        /// <summary>
        /// Read parameters from uploaded family file
        /// </summary>
        private void ReadFamilyParametersAsync()
        {
            string tempFilePath = null;
            try
            {
                UpdateStatus("Reading family parameters...");
                btnUpload.IsEnabled = false;

                // Copy file to temp location for reading (we'll delete temp copy after reading)
                // Keep original for S3 upload
                tempFilePath = Path.Combine(Path.GetTempPath(), Path.GetFileName(_uploadedFilePath));
                File.Copy(_uploadedFilePath, tempFilePath, true);

                // Read parameters using Revit API (must be on UI thread)
                try
                {
                    // Open the family file from temp location
                    Document familyDoc = _uiApp.Application.OpenDocumentFile(tempFilePath);
                    
                    if (familyDoc == null || !familyDoc.IsFamilyDocument)
                    {
                        MessageBox.Show("Failed to open family file or file is not a valid family document.", 
                            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        btnUpload.IsEnabled = true;
                        UpdateStatus("Error opening file");
                        return;
                    }

                    try
                    {
                        // Extract parameters
                        _parameters = ExtractFamilyParameters(familyDoc);
                        
                        // Display parameters
                        DisplayParameters();
                    }
                    finally
                    {
                        // Always close the document
                        familyDoc.Close(false);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error reading family parameters: {ex.Message}", 
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Utility.Logger(ex);
                }
                finally
                {
                    // Delete temp file after reading parameters
                    try
                    {
                        if (!string.IsNullOrEmpty(tempFilePath) && File.Exists(tempFilePath))
                        {
                            File.Delete(tempFilePath);
                        }
                    }
                    catch (Exception deleteEx)
                    {
                        // Log but don't fail if temp file deletion fails
                        Utility.Logger(deleteEx);
                    }

                    btnUpload.IsEnabled = true;
                    UpdateStatus("Parameters loaded");
                }
            }
            catch (Exception ex)
            {
                // Clean up temp file on error
                try
                {
                    if (!string.IsNullOrEmpty(tempFilePath) && File.Exists(tempFilePath))
                    {
                        File.Delete(tempFilePath);
                    }
                }
                catch
                {
                    // Ignore cleanup errors
                }

                btnUpload.IsEnabled = true;
                UpdateStatus("Error reading parameters");
                MessageBox.Show($"Error reading parameters: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Utility.Logger(ex);
            }
        }

        /// <summary>
        /// Extract parameters from family document
        /// </summary>
        private Dictionary<string, string> ExtractFamilyParameters(Document familyDoc)
        {
            var parameters = new Dictionary<string, string>();

            try
            {
                // First, try using family.GetFamilySymbolIds() like the working code
                Family family = familyDoc.OwnerFamily;
                if (family != null)
                {
                    var symbolIds = family.GetFamilySymbolIds();
                    
                    if (symbolIds.Count > 0)
                    {
                        // Iterate through ALL symbols to get complete parameter set
                        // Different symbols might have different values for the same parameter
                        foreach (var symbolId in symbolIds)
                        {
                            var symbol = familyDoc.GetElement(symbolId) as FamilySymbol;
                            if (symbol == null)
                                continue;

                            foreach (Parameter param in symbol.Parameters)
                            {
                                if (param == null || string.IsNullOrWhiteSpace(param.Definition?.Name))
                                    continue;

                                string paramName = param.Definition.Name;
                                string paramValue = GetParameterValueAsString(familyDoc, param);

                                // Only update if we don't have this parameter yet, or if current value is empty but new value is not
                                if (!parameters.ContainsKey(paramName))
                                {
                                    // Add parameter if it has a value
                                    if (!string.IsNullOrWhiteSpace(paramValue))
                                    {
                                        parameters[paramName] = paramValue;
                                    }
                                }
                                else
                                {
                                    // Update if current value is empty but new value is not
                                    if (string.IsNullOrWhiteSpace(parameters[paramName]) && !string.IsNullOrWhiteSpace(paramValue))
                                    {
                                        parameters[paramName] = paramValue;
                                    }
                                }
                            }
                        }
                    }
                }

                // If we didn't get parameters from GetFamilySymbolIds(), try FilteredElementCollector
                if (parameters.Count == 0)
                {
                    var collector = new FilteredElementCollector(familyDoc);
                    var symbols = collector.OfClass(typeof(FamilySymbol)).Cast<FamilySymbol>();
                    
                    if (symbols.Any())
                    {
                        // Iterate through ALL symbols to merge parameter values
                        foreach (var symbol in symbols)
                        {
                            foreach (Parameter param in symbol.Parameters)
                            {
                                if (param == null || string.IsNullOrWhiteSpace(param.Definition?.Name))
                                    continue;

                                string paramName = param.Definition.Name;
                                
                                // Skip if already added with a value
                                if (parameters.ContainsKey(paramName) && !string.IsNullOrWhiteSpace(parameters[paramName]))
                                    continue;

                                string paramValue = GetParameterValueAsString(familyDoc, param);

                                // Only add non-empty values
                                if (!string.IsNullOrWhiteSpace(paramValue))
                                {
                                    parameters[paramName] = paramValue;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.Logger(ex);
            }

            return parameters;
        }


        /// <summary>
        /// Get parameter value as string (for symbol parameters)
        /// </summary>
        private string GetParameterValueAsString(Document doc, Parameter param)
        {
            if (param == null)
                return string.Empty;

            try
            {
                // First try AsValueString which formats values with units automatically
                if (param.HasValue)
                {
                    string valueString = param.AsValueString();
                    if (!string.IsNullOrWhiteSpace(valueString))
                    {
                        return valueString;
                    }
                }

                // Fallback to storage type-specific extraction
                StorageType storageType = param.StorageType;

                switch (storageType)
                {
                    case StorageType.String:
                        return param.AsString() ?? string.Empty;

                    case StorageType.Integer:
                        return param.AsInteger().ToString();

                    case StorageType.Double:
                        return param.AsDouble().ToString();

                    case StorageType.ElementId:
                        var elemId = param.AsElementId();
                        if (elemId != null && elemId != ElementId.InvalidElementId)
                        {
                            var element = doc.GetElement(elemId);
                            return element?.Name ?? elemId.ToString();
                        }
                        return string.Empty;

                    case StorageType.None:
                        return string.Empty;

                    default:
                        return string.Empty;
                }
            }
            catch (Exception ex)
            {
                Utility.Logger(ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Display parameters in UI
        /// </summary>
        private void DisplayParameters()
        {
            panelParameters.Children.Clear();
            panelParameters.Visibility = System.Windows.Visibility.Visible;
            txtParametersHeader.Visibility = System.Windows.Visibility.Visible;
            btnAddNewParameter.Visibility = System.Windows.Visibility.Visible;

            // Get family name from file
            string familyName = Path.GetFileNameWithoutExtension(_uploadedFilePath);
            txtParametersHeader.Text = $"Family Parameters: {familyName}";

            // Display existing parameters
            foreach (var kvp in _parameters.OrderBy(p => p.Key))
            {
                AddParameterRow(kvp.Key, kvp.Value, false);
            }

            // Display custom parameters
            foreach (var kvp in _customParameters)
            {
                AddParameterRow(kvp.Key, kvp.Value, true);
            }

            ValidateAndEnableSave();
        }

        /// <summary>
        /// Add a parameter row to the UI
        /// </summary>
        private void AddParameterRow(string name, string value, bool isCustom)
        {
            var rowPanel = new StackPanel { Margin = new Thickness(0, 0, 0, 10) };

            // Parameter name label
            var nameTextBlock = new TextBlock
            {
                Text = $"{name}:",
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 2)
            };
            rowPanel.Children.Add(nameTextBlock);

            // Parameter value textbox
            var valueTextBox = new System.Windows.Controls.TextBox
            {
                Text = value,
                Height = 30,
                Padding = new Thickness(5),
                FontSize = 12,
                BorderBrush = System.Windows.Media.Brushes.Gray,
                BorderThickness = new Thickness(1),
                Tag = new { Name = name, IsCustom = isCustom }
            };
            valueTextBox.TextChanged += ParameterValue_TextChanged;
            rowPanel.Children.Add(valueTextBox);

            panelParameters.Children.Add(rowPanel);
        }

        /// <summary>
        /// Parameter value changed
        /// </summary>
        private void ParameterValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as System.Windows.Controls.TextBox;
            if (textBox?.Tag == null) return;

            dynamic tag = textBox.Tag;
            string paramName = tag.Name;
            bool isCustom = tag.IsCustom;

            if (isCustom)
            {
                // Update custom parameter
                var existing = _customParameters.FirstOrDefault(p => p.Key == paramName);
                if (existing.Key != null)
                {
                    _customParameters.Remove(existing);
                }
                _customParameters.Add(new KeyValuePair<string, string>(paramName, textBox.Text));
            }
            else
            {
                // Update existing parameter
                if (_parameters.ContainsKey(paramName))
                {
                    _parameters[paramName] = textBox.Text;
                }
            }

            ValidateAndEnableSave();
        }

        /// <summary>
        /// Add new parameter button clicked
        /// </summary>
        private void btnAddNewParameter_Click(object sender, RoutedEventArgs e)
        {
            panelNewParameter.Visibility = System.Windows.Visibility.Visible;
            txtNewParameterName.Text = string.Empty;
            txtNewParameterValue.Text = string.Empty;
            txtNewParameterName.Focus();
        }

        /// <summary>
        /// New parameter text changed - auto-add when both fields are filled
        /// </summary>
        private void NewParameter_TextChanged(object sender, TextChangedEventArgs e)
        {
            string paramName = txtNewParameterName.Text?.Trim();
            string paramValue = txtNewParameterValue.Text?.Trim();

            if (!string.IsNullOrWhiteSpace(paramName) && !string.IsNullOrWhiteSpace(paramValue))
            {
                // Check if parameter already exists
                bool exists = _parameters.ContainsKey(paramName) || 
                             _customParameters.Any(p => p.Key.Equals(paramName, StringComparison.OrdinalIgnoreCase));

                if (!exists)
                {
                    // Add the parameter
                    _customParameters.Add(new KeyValuePair<string, string>(paramName, paramValue));
                    
                    // Add to UI
                    AddParameterRow(paramName, paramValue, true);

                    // Clear input fields
                    txtNewParameterName.Text = string.Empty;
                    txtNewParameterValue.Text = string.Empty;
                    panelNewParameter.Visibility = System.Windows.Visibility.Collapsed;

                    ValidateAndEnableSave();
                }
            }
        }

        /// <summary>
        /// Handle Enter key in parameter value field
        /// </summary>
        private void txtNewParameterValue_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                string paramName = txtNewParameterName.Text?.Trim();
                string paramValue = txtNewParameterValue.Text?.Trim();

                if (!string.IsNullOrWhiteSpace(paramName) && !string.IsNullOrWhiteSpace(paramValue))
                {
                    // Trigger text changed logic
                    NewParameter_TextChanged(sender, null);
                }
            }
        }

        /// <summary>
        /// Add To Library button clicked
        /// </summary>
        private async void btnAddToLibrary_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrWhiteSpace(_selectedCategory))
                {
                    MessageBox.Show("Please select a category first.", "Validation Error", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(_uploadedFilePath) || !File.Exists(_uploadedFilePath))
                {
                    MessageBox.Show("Please upload a valid family file first.", "Validation Error", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Validate all parameters have values
                if (!ValidateParameters())
                {
                    MessageBox.Show("All parameters must have non-empty values.", "Validation Error", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                UpdateStatus("Saving to library...");
                btnAddToLibrary.IsEnabled = false;

                // Build XML and upload to S3
                await SaveToLibraryAsync();

                UpdateStatus("Successfully saved to library");
                MessageBox.Show("Family has been successfully added to the library.", 
                    "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                
                // Reset form
                ResetForm();
            }
            catch (Exception ex)
            {
                btnAddToLibrary.IsEnabled = true;
                UpdateStatus("Error saving to library");
                MessageBox.Show($"Error saving to library: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Utility.Logger(ex);
            }
        }

        /// <summary>
        /// Validate all parameters have non-empty values
        /// </summary>
        private bool ValidateParameters()
        {
            // Check existing parameters
            foreach (var kvp in _parameters)
            {
                if (string.IsNullOrWhiteSpace(kvp.Value))
                    return false;
            }

            // Check custom parameters
            foreach (var kvp in _customParameters)
            {
                if (string.IsNullOrWhiteSpace(kvp.Key) || string.IsNullOrWhiteSpace(kvp.Value))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Validate and enable/disable save button
        /// </summary>
        private void ValidateAndEnableSave()
        {
            bool isValid = !string.IsNullOrWhiteSpace(_selectedCategory) &&
                          !string.IsNullOrWhiteSpace(_uploadedFilePath) &&
                          File.Exists(_uploadedFilePath) &&
                          ValidateParameters();

            btnAddToLibrary.IsEnabled = isValid;
        }

        /// <summary>
        /// Save family to library (build XML and upload to S3)
        /// </summary>
        private async Task SaveToLibraryAsync()
        {
            try
            {
                // Get file info
                var fileInfo = new FileInfo(_uploadedFilePath);
                string familyName = Path.GetFileNameWithoutExtension(_uploadedFilePath);
                long fileSize = fileInfo.Length;
                string lastUpdated = DateTime.Now.ToString("yyyy-MM-dd");
                string revitVersion = _uiApp.Application.VersionNumber ?? "2023";
                string author = "BigFish BIM Sync";
                string filePath = $"{_selectedCategory}/{familyName}.rfa";

                // Download existing XML from S3 first
                string tempXmlPath = Path.Combine(Path.GetTempPath(), "BIMLibrary", "revitFolder.xml");
                
                // Ensure directory exists
                if (!Directory.Exists(Path.GetDirectoryName(tempXmlPath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(tempXmlPath));
                }

                // Try to download existing XML
                try
                {
                    await _s3Service.DownloadFamilyIndexFileFromS3Async();
                }
                catch
                {
                    // If download fails, we'll create a new XML file
                }

                XDocument xmlDoc;
                XElement root;

                // Load existing XML or create new
                if (File.Exists(tempXmlPath))
                {
                    try
                    {
                        xmlDoc = XDocument.Load(tempXmlPath);
                        root = xmlDoc.Root ?? new XElement("RevitFamilyRepository");
                    }
                    catch
                    {
                        root = new XElement("RevitFamilyRepository");
                        xmlDoc = new XDocument(root);
                    }
                }
                else
                {
                    root = new XElement("RevitFamilyRepository");
                    xmlDoc = new XDocument(root);
                }

                // Find or create category group
                var familiesByCategory = root.Element("FamiliesByCategory");
                if (familiesByCategory == null)
                {
                    familiesByCategory = new XElement("FamiliesByCategory");
                    root.Add(familiesByCategory);
                }

                var categoryGroup = familiesByCategory.Elements("CategoryGroup")
                    .FirstOrDefault(cg => string.Equals(cg.Attribute("name")?.Value, _selectedCategory, StringComparison.OrdinalIgnoreCase));

                if (categoryGroup == null)
                {
                    categoryGroup = new XElement("CategoryGroup",
                        new XAttribute("name", _selectedCategory));
                    familiesByCategory.Add(categoryGroup);
                }

                // Check if family already exists
                var existingFamily = categoryGroup.Elements("Family")
                    .FirstOrDefault(f => string.Equals(f.Element("FamilyName")?.Value, familyName, StringComparison.OrdinalIgnoreCase));

                if (existingFamily != null)
                {
                    // Update existing family
                    existingFamily.Element("FileSize")?.SetValue(fileSize);
                    existingFamily.Element("LastUpdated")?.SetValue(lastUpdated);
                    existingFamily.Element("RevitVersion")?.SetValue(revitVersion);
                    existingFamily.Element("Author")?.SetValue(author);

                    // Update metadata
                    var metadataElement = existingFamily.Element("Metadata");
                    if (metadataElement == null)
                    {
                        metadataElement = new XElement("Metadata");
                        existingFamily.Add(metadataElement);
                    }
                    else
                    {
                        metadataElement.RemoveAll();
                    }

                    // Add all parameters
                    AddParametersToMetadata(metadataElement);
                }
                else
                {
                    // Create new family element
                    var familyElement = new XElement("Family",
                        new XElement("FamilyName", familyName),
                        new XElement("Category", _selectedCategory),
                        new XElement("FilePath", filePath),
                        new XElement("FileSize", fileSize),
                        new XElement("RevitVersion", revitVersion),
                        new XElement("Author", author),
                        new XElement("LastUpdated", lastUpdated),
                        new XElement("Description", "Uploaded via BIM Library Browser")
                    );

                    // Add metadata section
                    var metadataElement = new XElement("Metadata");
                    AddParametersToMetadata(metadataElement);
                    familyElement.Add(metadataElement);

                    categoryGroup.Add(familyElement);
                }

                // Update category list
                UpdateCategoryList(root, _selectedCategory);

                // Upload the RFA file to S3 first (before XML)
                UpdateStatus("Uploading family file to S3...");
                await _s3Service.UploadFamilyFileAsync(_uploadedFilePath, filePath);

                // Save XML to temp file
                xmlDoc.Save(tempXmlPath);

                // Upload XML to S3
                UpdateStatus("Uploading metadata to S3...");
                await UploadXmlToS3Async(tempXmlPath);

                // Delete the original uploaded file after successful upload to S3
                try
                {
                    if (!string.IsNullOrEmpty(_uploadedFilePath) && File.Exists(_uploadedFilePath))
                    {
                        File.Delete(_uploadedFilePath);
                        _uploadedFilePath = null; // Clear the path
                    }
                }
                catch (Exception deleteEx)
                {
                    // Log but don't fail if file deletion fails
                    Utility.Logger(deleteEx);
                }
            }
            catch (Exception ex)
            {
                Utility.Logger(ex);
                throw;
            }
        }

        /// <summary>
        /// Add parameters to metadata element
        /// </summary>
        private void AddParametersToMetadata(XElement metadataElement)
        {
            // Add existing parameters
            foreach (var kvp in _parameters.OrderBy(p => p.Key))
            {
                var paramElement = new XElement("Parameter",
                    new XAttribute("name", kvp.Key),
                    kvp.Value
                );
                metadataElement.Add(paramElement);
            }

            // Add custom parameters
            foreach (var kvp in _customParameters.OrderBy(p => p.Key))
            {
                var paramElement = new XElement("Parameter",
                    new XAttribute("name", kvp.Key),
                    kvp.Value
                );
                metadataElement.Add(paramElement);
            }
        }

        /// <summary>
        /// Update category list in XML
        /// </summary>
        private void UpdateCategoryList(XElement root, string categoryName)
        {
            var categoryList = root.Element("CategoryList");
            if (categoryList == null)
            {
                categoryList = new XElement("CategoryList");
                root.Add(categoryList);
            }

            // Check if category exists
            var existingCategory = categoryList.Elements("Category")
                .FirstOrDefault(c => string.Equals(c.Value, categoryName, StringComparison.OrdinalIgnoreCase));

            if (existingCategory == null)
            {
                // Count families in this category
                var familiesByCategory = root.Element("FamiliesByCategory");
                int fileCount = 0;
                if (familiesByCategory != null)
                {
                    var categoryGroup = familiesByCategory.Elements("CategoryGroup")
                        .FirstOrDefault(cg => string.Equals(cg.Attribute("name")?.Value, categoryName, StringComparison.OrdinalIgnoreCase));
                    if (categoryGroup != null)
                    {
                        fileCount = categoryGroup.Elements("Family").Count();
                    }
                }

                int maxId = categoryList.Elements("Category")
                    .Select(c => int.TryParse(c.Attribute("id")?.Value, out int id) ? id : 0)
                    .DefaultIfEmpty(0)
                    .Max();

                var categoryElement = new XElement("Category",
                    new XAttribute("id", maxId + 1),
                    new XAttribute("FullPath", $"/{categoryName}"),
                    new XAttribute("FileCount", fileCount),
                    categoryName
                );
                categoryList.Add(categoryElement);
            }
            else
            {
                // Update file count
                var familiesByCategory = root.Element("FamiliesByCategory");
                if (familiesByCategory != null)
                {
                    var categoryGroup = familiesByCategory.Elements("CategoryGroup")
                        .FirstOrDefault(cg => string.Equals(cg.Attribute("name")?.Value, categoryName, StringComparison.OrdinalIgnoreCase));
                    if (categoryGroup != null)
                    {
                        int fileCount = categoryGroup.Elements("Family").Count();
                        existingCategory.SetAttributeValue("FileCount", fileCount);
                    }
                }
            }
        }

        /// <summary>
        /// Upload XML file to S3
        /// </summary>
        private async Task UploadXmlToS3Async(string xmlFilePath)
        {
            try
            {
                await _s3Service.UploadXmlFileAsync(xmlFilePath);
            }
            catch (Exception ex)
            {
                Utility.Logger(ex);
                throw;
            }
        }

        /// <summary>
        /// Reset form
        /// </summary>
        private void ResetForm()
        {
            _uploadedFilePath = null;
            _parameters.Clear();
            _customParameters.Clear();
            panelParameters.Children.Clear();
            panelParameters.Visibility = System.Windows.Visibility.Collapsed;
            txtParametersHeader.Visibility = System.Windows.Visibility.Collapsed;
            btnAddNewParameter.Visibility = System.Windows.Visibility.Collapsed;
            panelNewParameter.Visibility = System.Windows.Visibility.Collapsed;
            txtNewParameterName.Text = string.Empty;
            txtNewParameterValue.Text = string.Empty;
            btnAddToLibrary.IsEnabled = false;
        }

        /// <summary>
        /// Cancel button clicked
        /// </summary>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Close button clicked
        /// </summary>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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

