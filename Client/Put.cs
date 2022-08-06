using System;
using System.IO;
using FluentFTP;
using ShellProgressBar;
namespace Client
{
	public static class Put
	{
		public static int File(ref FtpClient client, Commands.Put file, in Program.FilePath path)
		{
			try
			{
				if (!client.IsAuthenticated)
				{
					throw new Exception("No connection to remote server!");
				}
				if (file.Files == null || !file.Files.Any())
				{
					throw new Exception("No argument(s) passed for upload!");
				}
				foreach (string filepath in file.Files)
				{
					if (!System.IO.File.Exists(filepath))
					{
						throw new Exception("\"" + filepath + "\" is not a valid filepath!");
					}
				}

				var options = new ProgressBarOptions
				{
					ForegroundColor = ConsoleColor.Yellow,
					ForegroundColorDone = ConsoleColor.DarkGreen,
					BackgroundColor = ConsoleColor.DarkGray,
					BackgroundCharacter = '\u2593'
				};

				if (file.Files != null && file.Files.Count() > 1)
				{
					return MultipleFiles(ref client, file.Files, path, options);
				}
				else
				{
					return SingleFile(ref client, file, path, options);
				}
			}
			catch (Exception e)
			{
				if (e.InnerException != null)
				{
					Console.WriteLine("Error: " + e.InnerException.Message);
				}
				else
				{
					Console.WriteLine("Error: " + e.Message);
				}
				return -1;
			}
		}

		public static int SingleFile(ref FtpClient client, Commands.Put file, in Program.FilePath path, ProgressBarOptions options)
		{
			string localPath = file.Files.ElementAt(0);
			string fullRemotePath = path.Remote + Path.GetFileName(localPath);

			Console.WriteLine("Uploading File to: " + fullRemotePath);

			using var progressBar = new ProgressBar(10000, "uploaded", options);
			Action<FtpProgress> progress = delegate (FtpProgress upload)
			{
				var progress = progressBar.AsProgress<double>();
				progress.Report(upload.Progress / 100);
			};

			FtpStatus status = client.UploadFile(@localPath, @fullRemotePath, FtpRemoteExists.Overwrite, true, FtpVerify.OnlyChecksum, progress);
			if (status == FtpStatus.Failed)
			{
				throw new Exception("Failed to upload file: " + localPath);
			}
			else
			{
				Console.WriteLine("Successfully uploaded file: " + fullRemotePath);     // load bar seems to overwrite this?
				return 1;
			}
		}

		public static int MultipleFiles(ref FtpClient client, IEnumerable<string> files, in Program.FilePath path, ProgressBarOptions options)
		{
			Console.WriteLine("Uploading (" + files.Count() + ") files to: " + path.Remote);

			using var progressBar = new ProgressBar(10000, "uploaded", options);
			Action<FtpProgress> progress = delegate (FtpProgress upload)
			{
				var progress = progressBar.AsProgress<double>();
				progress.Report(upload.Progress / 100);
			};

			int number = client.UploadFiles(files, path.Remote, FtpRemoteExists.Overwrite, true, FtpVerify.OnlyChecksum, FtpError.Throw, progress);
			Console.WriteLine(number + " files uploaded successfully!");    // load bar seems to overwrite this?
			return 1;
		}

		public static int Create(ref FtpClient client, Commands.CreateDirectory directory)
		{
			if (client.IsAuthenticated)
			{
				if (client.DirectoryExists(directory.Name))
					Console.WriteLine("ERROR: Directory already exists\n");
				else
					client.CreateDirectoryAsync(directory.Name);

				return 0;
			}
			else
			{
				Console.WriteLine("ERROR: Not connected to remote server!\n");
				return -1;
			}
		}

		public static int Copy(ref FtpClient client, Commands.Copy file)
		{
			return 0;
		}
	}
}
