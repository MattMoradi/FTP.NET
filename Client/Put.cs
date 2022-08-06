using System;
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
					throw new Exception("Error: No connection to remote server!");
				}
				Console.WriteLine("");


				// Progress bar related code //
				var options = new ProgressBarOptions
				{
					ForegroundColor = ConsoleColor.Yellow,
					ForegroundColorDone = ConsoleColor.DarkGreen,
					BackgroundColor = ConsoleColor.DarkGray,
					BackgroundCharacter = '\u2593'
				};

				using var progressBar = new ProgressBar(10000, "uploaded", options);
				Action<FtpProgress> progress = delegate (FtpProgress upload)
				{
					var progress = progressBar.AsProgress<double>();
					progress.Report(upload.Progress / 100);
				};
				// Progress bar related code 


				if (file.Files != null && file.Files.Count() > 1)
				{
					//foreach (string file in files){}
					return MultipleFiles(ref client, file.Files, path, progress);
				}
				else
				{
					return SingleFile(ref client, file, path, progress);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				return -1;
			}
			return 0;
		}

		public static int SingleFile(ref FtpClient client, Commands.Put file, in Program.FilePath path, Action<FtpProgress> progress)
		{
			try
			{
				if (file.Files == null)
					throw new Exception("No argument passed!");
				string localPath = file.Files.ElementAt(0);
				string fullRemotePath = path.Remote + Path.GetFileName(localPath);
				client.UploadFile(@localPath, @fullRemotePath, FtpRemoteExists.Overwrite, true, FtpVerify.OnlyChecksum, progress);
				return 1;
			}
			catch (Exception e)
			{
				Console.WriteLine(e.InnerException);
				return -1;
			}
		}

		public static int MultipleFiles(ref FtpClient client, IEnumerable<string> files, in Program.FilePath path, Action<FtpProgress> progress)
		{
			try
			{
				int number = client.UploadFiles(files, path.Remote, FtpRemoteExists.Overwrite, true, FtpVerify.OnlyChecksum, FtpError.Throw, progress);
				Console.WriteLine(number + " files uploaded successfully");
			}
			catch (Exception e)
			{
				Console.WriteLine(e.InnerException);
				return -1;
			}
			return 0;
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
