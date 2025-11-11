using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace boqtakeoff.core.Libraries
{
    /// <summary>
    /// Service for reading and parsing XML files containing Revit family metadata.
    /// 
    /// <para>
    /// This service provides functionality to:
    /// <list type="bullet">
    /// <item><description>Download and load XML index files from S3</description></item>
    /// <item><description>Parse XML content into XDocument objects using LINQ to XML</description></item>
    /// <item><description>Extract category and family metadata from structured XML</description></item>
    /// <item><description>Validate XML file extensions</description></item>
    /// </list>
    /// </para>
    /// 
    /// <para>
    /// The service uses a singleton pattern with asynchronous initialization. 
    /// Call <see cref="InitializeAsync"/> before accessing the <see cref="Instance"/> property.
    /// </para>
    /// 
    /// <para>
    /// Thread Safety: This class is thread-safe for read operations after initialization.
    /// However, the XML document is loaded once during initialization and should not be 
    /// modified after that point.
    /// </para>
    /// </summary>
    public class XmlReaderService
    {
        #region Singleton Implementation

        private static XmlReaderService _instance;
        private static readonly object _lock = new object();
        private static bool _isInitialized = false;

        /// <summary>
        /// Gets the singleton instance of XmlReaderService.
        /// </summary>
        /// <remarks>
        /// This property uses double-checked locking for thread-safe singleton creation.
        /// Ensure <see cref="InitializeAsync"/> has been called before using this instance.
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the service has not been initialized via <see cref="InitializeAsync"/>.
        /// </exception>
        public static XmlReaderService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new XmlReaderService();
                        }
                    }
                }

                if (!_isInitialized)
                {
                    throw new InvalidOperationException(
                        "XmlReaderService has not been initialized. Call InitializeAsync() before using the Instance property.");
                }

                return _instance;
            }
        }

        /// <summary>
        /// Initializes the singleton instance asynchronously by downloading and loading the XML index file from S3.
        /// </summary>
        /// <remarks>
        /// This method must be called once before accessing the <see cref="Instance"/> property.
        /// Subsequent calls will re-initialize the service with fresh data from S3.
        /// </remarks>
        /// <exception cref="ArgumentException">Thrown when the downloaded file path is invalid.</exception>
        /// <exception cref="FileNotFoundException">Thrown when the XML file cannot be found.</exception>
        /// <exception cref="XmlException">Thrown when the XML file is malformed or cannot be parsed.</exception>
        /// <exception cref="IOException">Thrown when there is an error reading the XML file.</exception>
        public static async Task InitializeAsync()
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new XmlReaderService();
                }
            }

            await _instance.LoadXmlDocumentAsync();
            _isInitialized = true;
        }

        #endregion

        #region Private Fields

        private XDocument _xmlDocument;
        private string _filePath;

        #endregion

        #region Constructor

        /// <summary>
        /// Private constructor to enforce singleton pattern.
        /// </summary>
        private XmlReaderService() { }

        #endregion

        #region Initialization

        /// <summary>
        /// Downloads the XML index file from S3 and loads it into memory.
        /// </summary>
        /// <remarks>
        /// This method:
        /// 1. Downloads the family index XML file from S3 using <see cref="S3FamilyLibraryService"/>
        /// 2. Validates the file path and existence
        /// 3. Validates the file extension
        /// 4. Loads the XML content into an <see cref="XDocument"/> for querying
        /// </remarks>
        /// <exception cref="ArgumentException">Thrown when the file path is null, empty, or not an XML file.</exception>
        /// <exception cref="FileNotFoundException">Thrown when the XML file cannot be found.</exception>
        /// <exception cref="XmlException">Thrown when the XML file is malformed or cannot be parsed.</exception>
        /// <exception cref="IOException">Thrown when there is an error reading the XML file.</exception>
        private async Task LoadXmlDocumentAsync()
        {
            // Download the XML index file from S3
            var s3Service = new S3FamilyLibraryService();
            _filePath = await s3Service.DownloadFamilyIndexFileFromS3Async();

            // Validate file path
            if (string.IsNullOrWhiteSpace(_filePath))
            {
                throw new ArgumentException("File path cannot be null or empty.", nameof(_filePath));
            }

            // Validate file exists
            if (!File.Exists(_filePath))
            {
                throw new FileNotFoundException($"XML file not found: {_filePath}", _filePath);
            }

            // Validate file extension
            if (!IsXmlFile(_filePath))
            {
                throw new ArgumentException(
                    $"File '{_filePath}' does not have an XML extension (.xml).", nameof(_filePath));
            }

            try
            {
                // Read file asynchronously using FileStream with async I/O enabled
                using (var fileStream = new FileStream(
                    _filePath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read,
                    bufferSize: 4096,
                    useAsync: true))
                {
                    // Load XML from stream
                    // Note: XDocument.Load is synchronous, but we're using async file I/O
                    // Task.Run is used to avoid blocking the calling thread during XML parsing
                    _xmlDocument = await Task.Run(() => XDocument.Load(fileStream));
                }
            }
            catch (XmlException ex)
            {
                throw new XmlException(
                    $"Failed to parse XML file '{_filePath}'. The file may be malformed or corrupted.", ex);
            }
            catch (IOException ex)
            {
                throw new IOException(
                    $"Error reading XML file '{_filePath}'. The file may be in use or inaccessible.", ex);
            }
        }

        #endregion


        #region Validation

        /// <summary>
        /// Validates that a file path points to an XML file by checking its extension.
        /// </summary>
        /// <remarks>
        /// This method performs a case-insensitive comparison of the file extension against ".xml".
        /// It helps prevent attempting to parse non-XML files.
        /// </remarks>
        /// <param name="filePath">The file path to validate.</param>
        /// <returns>
        /// <c>true</c> if the file has an .xml extension (case-insensitive); otherwise <c>false</c>.
        /// Also returns <c>false</c> if <paramref name="filePath"/> is null or empty.
        /// </returns>
        /// <example>
        /// <code>
        /// IsXmlFile("data.xml")    // Returns true
        /// IsXmlFile("data.XML")    // Returns true (case-insensitive)
        /// IsXmlFile("data.txt")    // Returns false
        /// IsXmlFile("data")        // Returns false (no extension)
        /// </code>
        /// </example>
        public bool IsXmlFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return false;
            }

            var extension = Path.GetExtension(filePath);
            return string.Equals(extension, ".xml", StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region XML Query Methods

        /// <summary>
        /// Extracts all root category elements from the CategoryList section of the loaded XML document.
        /// </summary>
        /// <remarks>
        /// This method:
        /// <list type="number">
        /// <item><description>Locates the CategoryList element in the XML document</description></item>
        /// <item><description>Extracts all Category child elements</description></item>
        /// <item><description>Maps each Category element to a <see cref="FolderNode"/> object</description></item>
        /// </list>
        /// </remarks>
        /// <returns>
        /// A read-only list of <see cref="FolderNode"/> objects representing root categories.
        /// Returns an empty list if no CategoryList or Category elements are found.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the XML document has not been loaded (service not initialized).
        /// </exception>
        /// <example>
        /// Example XML structure:
        /// <code>
        /// &lt;CategoryList&gt;
        ///   &lt;Category id="1" FullPath="Doors" FileCount="5"&gt;Doors&lt;/Category&gt;
        ///   &lt;Category id="2" FullPath="Windows" FileCount="3"&gt;Windows&lt;/Category&gt;
        /// &lt;/CategoryList&gt;
        /// </code>
        /// </example>
        public IReadOnlyList<FolderNode> GetAllRootCategories()
        {
            EnsureDocumentLoaded();

            var categoryList = _xmlDocument.Descendants("CategoryList").FirstOrDefault();
            if (categoryList == null)
            {
                return new List<FolderNode>();
            }

            var categories = categoryList.Elements("Category")
                .Select(categoryElement =>
                {
                    var fullPathAttribute = categoryElement.Attribute("FullPath");
                    var fullPath = fullPathAttribute?.Value ?? string.Empty;
                    var name = categoryElement.Value ?? string.Empty;

                    // Parse FileCount if available
                    int fileCount = 0;
                    var fileCountAttribute = categoryElement.Attribute("FileCount");
                    if (fileCountAttribute != null && int.TryParse(fileCountAttribute.Value, out var parsedCount))
                    {
                        fileCount = parsedCount;
                    }

                    return new FolderNode
                    {
                        FileCount = fileCount,
                        FullPath = fullPath,
                        Name = name,
                        Children = new Dictionary<string, FolderNode>()
                    };
                })
                .ToList();

            return categories;
        }


        /// <summary>
        /// Extracts all Family elements for a given category name from the FamiliesByCategory section.
        /// </summary>
        /// <remarks>
        /// This method:
        /// <list type="number">
        /// <item><description>Locates the FamiliesByCategory element in the XML document</description></item>
        /// <item><description>Finds the CategoryGroup element with the matching category name (case-insensitive)</description></item>
        /// <item><description>Extracts all Family child elements from that CategoryGroup</description></item>
        /// <item><description>For each Family element, extracts FamilyName, FilePath, FileSize, LastUpdated, and Metadata parameters</description></item>
        /// </list>
        /// </remarks>
        /// <param name="categoryName">The name of the category to search for (e.g., "Doors", "Windows").</param>
        /// <returns>
        /// A read-only list of <see cref="FamilyMetadata"/> objects containing family information and metadata.
        /// Returns an empty list if the category is not found or has no families.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="categoryName"/> is null, empty, or whitespace.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the XML document has not been loaded (service not initialized).
        /// </exception>
        /// <example>
        /// Example XML structure:
        /// <code>
        /// &lt;FamiliesByCategory&gt;
        ///   &lt;CategoryGroup name="Doors"&gt;
        ///     &lt;Family&gt;
        ///       &lt;FamilyName&gt;Single-Flush-800x2100&lt;/FamilyName&gt;
        ///       &lt;FilePath&gt;Doors/Single-Flush-800x2100.rfa&lt;/FilePath&gt;
        ///       &lt;FileSize&gt;1024000&lt;/FileSize&gt;
        ///       &lt;LastUpdated&gt;2024-01-15T10:30:00&lt;/LastUpdated&gt;
        ///       &lt;Metadata&gt;
        ///         &lt;Parameter name="Width"&gt;800 mm&lt;/Parameter&gt;
        ///         &lt;Parameter name="Height"&gt;2100 mm&lt;/Parameter&gt;
        ///       &lt;/Metadata&gt;
        ///     &lt;/Family&gt;
        ///   &lt;/CategoryGroup&gt;
        /// &lt;/FamiliesByCategory&gt;
        /// </code>
        /// </example>
        public IReadOnlyList<FamilyMetadata> ReadFamiliesByCategory(string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                throw new ArgumentException("Category name cannot be null or whitespace.", nameof(categoryName));
            }

            EnsureDocumentLoaded();

            var familiesByCategory = _xmlDocument.Descendants("FamiliesByCategory").FirstOrDefault();
            if (familiesByCategory == null)
            {
                return new List<FamilyMetadata>();
            }

            var categoryGroup = familiesByCategory.Elements("CategoryGroup")
                .FirstOrDefault(cg =>
                    string.Equals(cg.Attribute("name")?.Value, categoryName, StringComparison.OrdinalIgnoreCase));

            if (categoryGroup == null)
            {
                return new List<FamilyMetadata>();
            }

            var families = categoryGroup.Elements("Family")
                .Select(familyElement =>
                {
                    var familyName = familyElement.Element("FamilyName")?.Value ?? string.Empty;
                    var filePath = familyElement.Element("FilePath")?.Value ?? string.Empty;

                    // Parse FileSize safely
                    long? fileSize = null;
                    var fileSizeElement = familyElement.Element("FileSize");
                    if (fileSizeElement != null && long.TryParse(fileSizeElement.Value, out var parsedSize))
                    {
                        fileSize = parsedSize;
                    }

                    // Parse LastUpdated safely
                    DateTime? lastUpdated = null;
                    var lastUpdatedElement = familyElement.Element("LastUpdated");
                    if (lastUpdatedElement != null && 
                        DateTime.TryParse(lastUpdatedElement.Value, out var parsedDate))
                    {
                        lastUpdated = parsedDate;
                    }

                    // Extract Metadata Parameter elements
                    var metadata = new Dictionary<string, string>();
                    var metadataElement = familyElement.Element("Metadata");
                    if (metadataElement != null)
                    {
                        foreach (var parameter in metadataElement.Elements("Parameter"))
                        {
                            var parameterName = parameter.Attribute("name")?.Value;
                            if (!string.IsNullOrWhiteSpace(parameterName))
                            {
                                metadata[parameterName] = parameter.Value ?? string.Empty;
                            }
                        }
                    }

                    return new FamilyMetadata
                    {
                        FileName = familyName,
                        S3Key = filePath,
                        SizeInBytes = fileSize,
                        LastModified = lastUpdated,
                        Category = GetCategoryFromKey(filePath),
                        FolderPath = GetFolderPath(filePath),
                        metadata = metadata
                    };
                })
                .ToList();

            return families;
        }

        /// <summary>
        /// Retrieves a single family's metadata for the given category and family name.
        /// </summary>
        /// <remarks>
        /// This method searches the loaded XML document for a specific family within a category.
        /// The search is case-insensitive for both category and family names.
        /// </remarks>
        /// <param name="categoryName">The CategoryGroup name (e.g., "Doors").</param>
        /// <param name="familyName">The FamilyName value (e.g., "Single-Flush-800x2100").</param>
        /// <returns>
        /// A populated <see cref="FamilyMetadata"/> instance when found; otherwise <c>null</c>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="categoryName"/> or <paramref name="familyName"/> is null, empty, or whitespace.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the XML document has not been loaded (service not initialized).
        /// </exception>
        public FamilyMetadata ReadFamilyMetadata(string categoryName, string familyName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                throw new ArgumentException("Category name cannot be null or whitespace.", nameof(categoryName));
            }

            if (string.IsNullOrWhiteSpace(familyName))
            {
                throw new ArgumentException("Family name cannot be null or whitespace.", nameof(familyName));
            }

            EnsureDocumentLoaded();

            var familiesByCategory = _xmlDocument.Descendants("FamiliesByCategory").FirstOrDefault();
            if (familiesByCategory == null)
            {
                return null;
            }

            var categoryGroup = familiesByCategory.Elements("CategoryGroup")
                .FirstOrDefault(cg => 
                    string.Equals(cg.Attribute("name")?.Value, categoryName, StringComparison.OrdinalIgnoreCase));

            if (categoryGroup == null)
            {
                return null;
            }

            var familyElement = categoryGroup.Elements("Family")
                .FirstOrDefault(fe => 
                    string.Equals(fe.Element("FamilyName")?.Value, familyName, StringComparison.OrdinalIgnoreCase));

            if (familyElement == null)
            {
                return null;
            }

            var filePath = familyElement.Element("FilePath")?.Value ?? string.Empty;

            // Parse FileSize safely
            long? sizeInBytes = null;
            var fileSizeValue = familyElement.Element("FileSize")?.Value;
            if (!string.IsNullOrWhiteSpace(fileSizeValue) && long.TryParse(fileSizeValue, out var parsedSize))
            {
                sizeInBytes = parsedSize;
            }

            // Parse LastUpdated safely
            DateTime? lastUpdated = null;
            var lastUpdatedValue = familyElement.Element("LastUpdated")?.Value;
            if (!string.IsNullOrWhiteSpace(lastUpdatedValue) && 
                DateTime.TryParse(lastUpdatedValue, out var parsedLastUpdated))
            {
                lastUpdated = parsedLastUpdated;
            }

            // Extract Metadata Parameter elements
            var metadata = new Dictionary<string, string>();
            var metadataElement = familyElement.Element("Metadata");
            if (metadataElement != null)
            {
                foreach (var parameter in metadataElement.Elements("Parameter"))
                {
                    var parameterName = parameter.Attribute("name")?.Value;
                    if (!string.IsNullOrWhiteSpace(parameterName))
                    {
                        metadata[parameterName] = parameter.Value ?? string.Empty;
                    }
                }
            }

            return new FamilyMetadata
            {
                FileName = familyElement.Element("FamilyName")?.Value ?? string.Empty,
                S3Key = filePath,
                SizeInBytes = sizeInBytes,
                LastModified = lastUpdated,
                Category = GetCategoryFromKey(filePath),
                FolderPath = GetFolderPath(filePath),
                metadata = metadata
            };
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Ensures that the XML document has been loaded before performing operations.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the XML document is null (service not initialized).
        /// </exception>
        private void EnsureDocumentLoaded()
        {
            if (_xmlDocument == null)
            {
                throw new InvalidOperationException(
                    "XML document has not been loaded. Ensure InitializeAsync() has been called.");
            }
        }

        /// <summary>
        /// Extracts the category name from an S3 key path.
        /// </summary>
        /// <remarks>
        /// Assumes the S3 key follows the pattern: "prefix/category/filename.rfa"
        /// Returns the first path segment after the prefix (index 0).
        /// </remarks>
        /// <param name="s3Key">The S3 object key (e.g., "RevitFamilies/Doors/Door-800x2100.rfa").</param>
        /// <returns>
        /// The category name extracted from the path, or "Uncategorized" if the path structure is invalid.
        /// </returns>
        private string GetCategoryFromKey(string s3Key)
        {
            if (string.IsNullOrWhiteSpace(s3Key))
            {
                return "Uncategorized";
            }

            var parts = s3Key.Split('/');
            // Return the first part (index 0) which should be the category
            // If path is "RevitFamilies/Doors/file.rfa", parts[0] = "RevitFamilies", parts[1] = "Doors"
            // Based on S3FamilyLibraryService, category is typically the first segment
            return parts.Length > 0 ? parts[0] : "Uncategorized";
        }

        /// <summary>
        /// Extracts the folder path from an S3 key by removing the filename.
        /// </summary>
        /// <param name="s3Key">The S3 object key (e.g., "Doors/Door-800x2100.rfa").</param>
        /// <returns>
        /// The folder path without the filename, or an empty string if no folder path exists.
        /// </returns>
        private string GetFolderPath(string s3Key)
        {
            if (string.IsNullOrWhiteSpace(s3Key))
            {
                return string.Empty;
            }

            var lastSlash = s3Key.LastIndexOf('/');
            return lastSlash > 0 ? s3Key.Substring(0, lastSlash) : string.Empty;
        }

        #endregion
    }
}

