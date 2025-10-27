using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace boqtakeoff.core.Libraries
{
    /// <summary>
    /// Service for managing Revit family files stored in AWS S3
    /// </summary>
    public class S3FamilyLibraryService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;
        private readonly string _tempDownloadPath;

        public S3FamilyLibraryService(string bucketName, string accessKey, string secretKey, string region)
        {
            _bucketName = bucketName;
            _tempDownloadPath = Path.Combine(Path.GetTempPath(), "BIMLibrary");

            // Create temp directory if it doesn't exist
            if (!Directory.Exists(_tempDownloadPath))
            {
                Directory.CreateDirectory(_tempDownloadPath);
            }

            // Initialize S3 client
            var regionEndpoint = RegionEndpoint.GetBySystemName(region);
            _s3Client = new AmazonS3Client(accessKey, secretKey, regionEndpoint);
        }

        /// <summary>
        /// Get list of all families in the library organized by folders
        /// </summary>
        /// <param name="prefix">Folder path prefix (e.g., "Furniture/", "MEP/")</param>
        /// <returns>List of family metadata</returns>
        public async Task<List<FamilyMetadata>> GetFamilyListAsync(string prefix = "")
        {
            try
            {
                var families = new List<FamilyMetadata>();
                var request = new ListObjectsV2Request
                {
                    BucketName = _bucketName,
                    Prefix = prefix
                };

                ListObjectsV2Response response;
                do
                {
                    response = await _s3Client.ListObjectsV2Async(request);

                    foreach (var s3Object in response.S3Objects)
                    {
                        // Only include .rfa files (Revit family files)
                        if (s3Object.Key.EndsWith(".rfa", StringComparison.OrdinalIgnoreCase))
                        {
                            families.Add(new FamilyMetadata
                            {
                                FileName = Path.GetFileName(s3Object.Key),
                                S3Key = s3Object.Key,
                                SizeInBytes = s3Object.Size ?? 0,
                                LastModified = s3Object.LastModified ?? DateTime.Now,
                                Category = GetCategoryFromKey(s3Object.Key),
                                FolderPath = GetFolderPath(s3Object.Key)
                            });
                        }
                    }

                    request.ContinuationToken = response.NextContinuationToken;
                } while (response.IsTruncated ?? false);

                return families;
            }
            catch (Exception ex)
            {
                Utility.Logger(ex);
                throw new Exception($"Error retrieving family list from S3: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get folder structure from S3 bucket
        /// </summary>
        /// <returns>Hierarchical folder structure</returns>
        public async Task<List<FolderNode>> GetFolderStructureAsync()
        {
            try
            {
                var families = await GetFamilyListAsync();
                var rootFolders = new Dictionary<string, FolderNode>();

                foreach (var family in families)
                {
                    var pathParts = family.S3Key.Split('/');
                    var currentLevel = rootFolders;

                    // Build folder hierarchy
                    for (int i = 0; i < pathParts.Length - 1; i++)
                    {
                        var folderName = pathParts[i];

                        if (!currentLevel.ContainsKey(folderName))
                        {
                            currentLevel[folderName] = new FolderNode
                            {
                                Name = folderName,
                                FullPath = string.Join("/", pathParts.Take(i + 1)),
                                Children = new Dictionary<string, FolderNode>()
                            };
                        }

                        currentLevel = currentLevel[folderName].Children;
                    }
                }

                return rootFolders.Values.ToList();
            }
            catch (Exception ex)
            {
                Utility.Logger(ex);
                throw;
            }
        }

        /// <summary>
        /// Download family file from S3 to temporary location
        /// </summary>
        /// <param name="s3Key">S3 object key</param>
        /// <returns>Local file path</returns>
        public async Task<string> DownloadFamilyAsync(string s3Key)
        {
            try
            {
                var fileName = Path.GetFileName(s3Key);
                var localFilePath = Path.Combine(_tempDownloadPath, fileName);

                // Delete existing file if it exists
                if (File.Exists(localFilePath))
                {
                    File.Delete(localFilePath);
                }

                var request = new GetObjectRequest
                {
                    BucketName = _bucketName,
                    Key = s3Key
                };

                using (var response = await _s3Client.GetObjectAsync(request))
                using (var fileStream = File.Create(localFilePath))
                {
                    await response.ResponseStream.CopyToAsync(fileStream);
                }

                return localFilePath;
            }
            catch (Exception ex)
            {
                Utility.Logger(ex);
                throw new Exception($"Error downloading family from S3: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get metadata for a specific family
        /// </summary>
        /// <param name="s3Key">S3 object key</param>
        /// <returns>Family metadata</returns>
        public async Task<FamilyMetadata> GetFamilyMetadataAsync(string s3Key)
        {
            try
            {
                var request = new GetObjectMetadataRequest
                {
                    BucketName = _bucketName,
                    Key = s3Key
                };

                var response = await _s3Client.GetObjectMetadataAsync(request);

                return new FamilyMetadata
                {
                    FileName = Path.GetFileName(s3Key),
                    S3Key = s3Key,                    
                    SizeInBytes = response.ContentLength,
                    LastModified = response.LastModified ?? DateTime.Now,
                    Category = GetCategoryFromKey(s3Key),
                    FolderPath = GetFolderPath(s3Key),
                    ContentType = response.Headers.ContentType
                };
            }
            catch (Exception ex)
            {
                Utility.Logger(ex);
                throw new Exception($"Error retrieving family metadata: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Search families by name
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <returns>Filtered list of families</returns>
        public async Task<List<FamilyMetadata>> SearchFamiliesAsync(string searchTerm)
        {
            try
            {
                var allFamilies = await GetFamilyListAsync();

                return allFamilies.Where(f =>
                    f.FileName.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    f.Category.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0
                ).ToList();
            }
            catch (Exception ex)
            {
                Utility.Logger(ex);
                throw;
            }
        }

        /// <summary>
        /// Clean up temporary downloaded files
        /// </summary>
        public void CleanupTempFiles()
        {
            try
            {
                if (Directory.Exists(_tempDownloadPath))
                {
                    var files = Directory.GetFiles(_tempDownloadPath, "*.rfa");
                    foreach (var file in files)
                    {
                        try
                        {
                            File.Delete(file);
                        }
                        catch
                        {
                            // Ignore errors deleting individual files
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.Logger(ex);
            }
        }

        /// <summary>
        /// Extract category from S3 key path
        /// </summary>
        private string GetCategoryFromKey(string s3Key)
        {
            var parts = s3Key.Split('/');
            return parts.Length > 1 ? parts[0] : "Uncategorized";
        }

        /// <summary>
        /// Get folder path from S3 key
        /// </summary>
        private string GetFolderPath(string s3Key)
        {
            var lastSlash = s3Key.LastIndexOf('/');
            return lastSlash > 0 ? s3Key.Substring(0, lastSlash) : "";
        }

        /// <summary>
        /// Dispose S3 client
        /// </summary>
        public void Dispose()
        {
            _s3Client?.Dispose();
            CleanupTempFiles();
        }
    }

    public class FamilyMetadata
    {
        public string FileName { get; set; }
        public string S3Key { get; set; }
        public string Category { get; set; }
        public string FamilyType { get; set; }
        public string FolderPath { get; set; }
        public long? SizeInBytes { get; set; }  // Nullable
        public DateTime? LastModified { get; set; }  // Nullable
        public string ThumbnailUrl { get; set; }
        public string ContentType { get; set; }

        public string SizeFormatted => FormatFileSize(SizeInBytes ?? 0);  // Handle null

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
    
    /// <summary>
    /// Represents a folder node in the hierarchy
    /// </summary>
    public class FolderNode
    {
        public string Name { get; set; }
        public string FullPath { get; set; }
        public Dictionary<string, FolderNode> Children { get; set; }
        public int FileCount { get; set; }
    }
}