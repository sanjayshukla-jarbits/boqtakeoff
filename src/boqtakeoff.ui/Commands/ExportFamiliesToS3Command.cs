using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using boqtakeoff.core.Libraries;
using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace boqtakeoff.ui.Commands
{
	[Transaction(TransactionMode.Manual)]
	[Regeneration(RegenerationOption.Manual)]
	public class ExportFamiliesToS3Command : IExternalCommand
	{
		public Result Execute(
			ExternalCommandData commandData,
			ref string message,
			ElementSet elements)
		{
			try
			{
				var doc = commandData.Application.ActiveUIDocument.Document;

				// Step 1: Select export folder
				using (var folderDialog = new FolderBrowserDialog())
				{
					folderDialog.Description = "Select folder to export families";

					if (folderDialog.ShowDialog() != DialogResult.OK)
						return Result.Cancelled;

					string exportPath = folderDialog.SelectedPath;

					// Step 2: Export families
					var exportService = new FamilyExportService(doc, exportPath);
					var families = exportService.GetAllLoadableFamilies();

					var progressForm = new System.Windows.Forms.Form
					{
						Text = "Exporting Families",
						Width = 400,
						Height = 150,
						StartPosition = FormStartPosition.CenterScreen
					};

					var progressLabel = new Label
					{
						Text = "Exporting families...",
						Left = 20,
						Top = 20,
						Width = 350
					};
					progressForm.Controls.Add(progressLabel);

					var exportProgress = new Progress<FamilyExportProgress>(p =>
					{
						progressLabel.Text = $"Exporting {p.CurrentIndex}/{p.TotalCount}: {p.CurrentFamilyName}";
					});

					progressForm.Show();

					// Export all families
					var exportResult = exportService.ExportAllFamilies(exportProgress);

					progressForm.Close();

					// Step 3: Ask to upload to S3
					var uploadDialog = MessageBox.Show(
						$"Export complete!\n\nExported: {exportResult.SuccessCount}\nFailed: {exportResult.FailedCount}\n\nUpload to S3?",
						"Export Complete",
						MessageBoxButtons.YesNo,
						MessageBoxIcon.Question);

					if (uploadDialog == DialogResult.Yes)
					{
						// Upload to S3
						string bucketName = ConfigurationManager.AppSettings["AWS_S3_BucketName"];
						string accessKey = ConfigurationManager.AppSettings["AWS_AccessKey"];
						string secretKey = ConfigurationManager.AppSettings["AWS_SecretKey"];
						string region = ConfigurationManager.AppSettings["AWS_S3_Region"] ?? "us-east-1";

						var uploadService = new S3FamilyUploadService(bucketName, accessKey, secretKey, region);

						progressForm = new System.Windows.Forms.Form
						{
							Text = "Uploading to S3",
							Width = 400,
							Height = 150,
							StartPosition = FormStartPosition.CenterScreen
						};

						progressLabel = new Label
						{
							Text = "Uploading files...",
							Left = 20,
							Top = 20,
							Width = 350
						};
						progressForm.Controls.Add(progressLabel);

						var uploadProgress = new Progress<S3UploadProgress>(p =>
						{
							progressLabel.Text = $"Uploading {p.CurrentIndex}/{p.TotalCount}: {p.CurrentFileName}";
						});

						progressForm.Show();

						// Upload folder
						Task.Run(async () =>
						{
							var uploadResult = await uploadService.UploadFolderAsync(
								exportPath,
								"RevitFamilies",
								uploadProgress);

							progressForm.Invoke(new Action(() =>
							{
								progressForm.Close();
								MessageBox.Show(
									$"Upload complete!\n\nUploaded: {uploadResult.SuccessCount}\nFailed: {uploadResult.FailedCount}",
									"Upload Complete",
									MessageBoxButtons.OK,
									MessageBoxIcon.Information);
							}));
						}).Wait();

						uploadService.Dispose();
					}

					TaskDialog.Show("Success", $"Operation completed!\n\nExported: {exportResult.SuccessCount} families");
				}

				return Result.Succeeded;
			}
			catch (Exception ex)
			{
				Utility.Logger(ex);
				message = ex.Message;
				return Result.Failed;
			}
		}

		public static string GetPath()
		{
			return typeof(ExportFamiliesToS3Command).FullName;
		}
	}
}