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
				int i = 0;
				string[] files = new string[file.Files.Count()];
				foreach (string fp in file.Files)
				{
					if (fp.Length > 1 && fp[1] == ':')
					{
						files[i] = fp;
					}
					else
					{
						files[i] = path.Local + "\\" + fp;
					}
					if (!System.IO.File.Exists(files[i]))
					{
						throw new Exception("\"" + fp + "\" is not a valid filepath!");
					}
					++i;
				}

				var options = new ProgressBarOptions
				{
					ForegroundColor = ConsoleColor.Yellow,
					ForegroundColorDone = ConsoleColor.DarkGreen,
					BackgroundColor = ConsoleColor.DarkGray,
					BackgroundCharacter = '\u2593'
				};

				if (file.Files.Count() > 1)
				{
					return MultipleFiles(ref client, files, path, options);
				}
				else
				{
					return SingleFile(ref client, files, path, options);
				}
			}
			catch (Exception e)
			{
				if (e.InnerException != null)
				{
					Exception realerror = e;
					while (realerror.InnerException != null)
						realerror = realerror.InnerException;

					Console.WriteLine(realerror.Message);
				}
				else
				{
					Console.WriteLine("Error: " + e.Message);
				}
				return -1;
			}
		}

		public static int SingleFile(ref FtpClient client, string[] files, in Program.FilePath path, ProgressBarOptions options)
		{
			string localPath = files[0];
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
			return 0;
		}

		public static int MultipleFiles(ref FtpClient client, string[] files, in Program.FilePath path, ProgressBarOptions options)
		{
			Console.WriteLine("Uploading (" + files.Count() + ") files to: " + path.Remote);

			using var progressBar = new ProgressBar(10000, "uploaded", options);
			Action<FtpProgress> progress = delegate (FtpProgress upload)
			{
				var progress = progressBar.AsProgress<double>();
				progress.Report(upload.Progress / 100);
			};

			client.UploadFiles(files, path.Remote, FtpRemoteExists.Overwrite, true, FtpVerify.OnlyChecksum, FtpError.Throw, progress);
			return 0;
		}

		public static int Create(ref FtpClient client, Commands.CreateDirectory directory, in Program.FilePath path)
		{
			if (client.IsAuthenticated)
			{
				DirectoryInfo dir = new DirectoryInfo(path.Remote);

				string fullpath = dir.FullName + directory.Name;
				fullpath = fullpath.Remove(0, 2);

				bool isDir = client.DirectoryExists(fullpath);

				if (isDir)
					Console.WriteLine("ERROR: Directory already exists\n");
				else

					try
					{
						client.CreateDirectory(fullpath);
					}
					catch (Exception ex)
					{
						Console.WriteLine("The target directory name provided is invalid.");
						Console.WriteLine("Error Message: " + ex.Message);
						return -1;
					}

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
			string sourcedir;
			string targetdir;

			if (!client.IsAuthenticated)
			{
				Console.WriteLine("ERROR: Not connected to remote server!\n");
				return -1;
			}

			//Check whether there are two arguments, if there are not, return error message
			if (file.Directories.Count() != 2)
			{
				Console.WriteLine("You must provide two arguments for the copy directory command.");
				return -1;
			}
			else
			{
				sourcedir = file.Directories.First();
				targetdir = file.Directories.Last();
			}


			//Check whether the source dir exists. If it does not, return an error message
			if (!client.DirectoryExists(file.Directories.First()))
			{
				Console.WriteLine("The source directory does not exist.");
				return -1;
			}

			//Check whether the destination dir exists. If it does ask whether they would like to delete the contents and replace w/ the source dir and it's contents
			if (!client.DirectoryExists(file.Directories.Last()))
			{
				Console.WriteLine("The destination directory does not exist");

				return -1;
			}

			//ALL CHECKS SHOULD BE PERFORMED AT THIS POINT SO OK TO GO FORWARD WITH THE DL/UL/DEL

			string localtempdir = "C:\\fluentftp_tempdir";

			client.DownloadDirectory(localtempdir, sourcedir, FtpFolderSyncMode.Update);

			client.UploadDirectory(localtempdir, targetdir, FtpFolderSyncMode.Update);

			Directory.Delete(localtempdir, true);

			return 0;
		}
	}
}
