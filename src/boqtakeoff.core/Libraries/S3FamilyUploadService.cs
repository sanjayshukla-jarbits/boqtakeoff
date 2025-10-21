using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System;
using System.IO;
using System.Threading.Tasks;

namespace boqtakeoff.core.Libraries
{
	/// <summary>
	/// Service for uploading family files to AWS S3
	/// </summary>
	public class S3FamilyUploadService
	{
		private readonly IAmazonS3 _s3Client;
		private readonly string _bucketName;

		public S3FamilyUploadService(string bucketName, string accessKey, string secretKey, string region)
		{
			_bucketName = bucketName;
			var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(region);
			_s3Client = new AmazonS3Client(accessKey, secretKey, regionEndpoint);
		}

		/// <summary>
		/// Upload a single family file to S3
		/// </summary>
		public async Task<bool> UploadFamilyAsync(
			string localFilePath,
			string s3Key,
			IProgress<int> progress = null)
		{
			try
			{
				var fileTransferUtility = new TransferUtility(_s3Client);

				var uploadRequest = new TransferUtilityUploadRequest
				{
					BucketName = _bucketName,
					FilePath = localFilePath,
					Key = s3Key,
					CannedACL = S3CannedACL.Private
				};

				// Track upload progress
				if (progress != null)
				{
					uploadRequest.UploadProgressEvent += (sender, e) =>
					{
						int percentComplete = (int)e.TransferredBytes * 100 / (int)e.TotalBytes;
						progress.Report(percentComplete);
					};
				}

				await fileTransferUtility.UploadAsync(uploadRequest);
				return true;
			}
			catch (Exception ex)
			{
				Utility.Logger(ex);
				return false;
			}
		}

		/// <summary>
		/// Upload entire folder to S3 preserving structure
		/// </summary>
		public async Task<S3UploadResult> UploadFolderAsync(
			string localFolderPath,
			string s3FolderPrefix,
			IProgress<S3UploadProgress> progress = null)
		{
			var result = new S3UploadResult();

			try
			{
				var files = Directory.GetFiles(localFolderPath, "*.rfa", SearchOption.AllDirectories);
				result.TotalFiles = files.Length;

				for (int i = 0; i < files.Length; i++)
				{
					string localFile = files[i];

					// Calculate relative path for S3 key
					string relativePath = localFile.Substring(localFolderPath.Length + 1);
					string s3Key = s3FolderPrefix + "/" + relativePath.Replace("\\", "/");

					// Report progress
					progress?.Report(new S3UploadProgress
					{
						CurrentIndex = i + 1,
						TotalCount = files.Length,
						CurrentFileName = Path.GetFileName(localFile),
						Status = "Uploading..."
					});

					bool success = await UploadFamilyAsync(localFile, s3Key);

					if (success)
					{
						result.SuccessCount++;
						result.UploadedFiles.Add(relativePath);
					}
					else
					{
						result.FailedCount++;
						result.FailedFiles.Add(relativePath, "Upload failed");
					}
				}
			}
			catch (Exception ex)
			{
				Utility.Logger(ex);
			}

			return result;
		}

		/// <summary>
		/// Check if file already exists in S3
		/// </summary>
		public async Task<bool> FileExistsAsync(string s3Key)
		{
			try
			{
				var request = new GetObjectMetadataRequest
				{
					BucketName = _bucketName,
					Key = s3Key
				};

				await _s3Client.GetObjectMetadataAsync(request);
				return true;
			}
			catch (AmazonS3Exception ex)
			{
				if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
					return false;
				throw;
			}
		}

		public void Dispose()
		{
			_s3Client?.Dispose();
		}
	}

	public class S3UploadProgress
	{
		public int CurrentIndex { get; set; }
		public int TotalCount { get; set; }
		public string CurrentFileName { get; set; }
		public string Status { get; set; }
		public int PercentComplete => (int)((double)CurrentIndex / TotalCount * 100);
	}

	public class S3UploadResult
	{
		public int TotalFiles { get; set; }
		public int SuccessCount { get; set; }
		public int FailedCount { get; set; }
		public System.Collections.Generic.List<string> UploadedFiles { get; set; } = new System.Collections.Generic.List<string>();
		public System.Collections.Generic.Dictionary<string, string> FailedFiles { get; set; } = new System.Collections.Generic.Dictionary<string, string>();
	}
}