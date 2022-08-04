using System;
using FluentFTP;
using ShellProgressBar;
namespace Client
{
    public static class Put
    {
        public static int File(ref FtpClient client, Commands.Put file, in Program.FilePath path)
        {
            Console.WriteLine("file: " + file.File);

            if (!client.IsAuthenticated)
            {
                throw new Exception("Error: No connection to remote server!");
            }
            if (file.Files.Count() > 1)
                return MultipleFiles(ref client, file.Files, path);

            // Progress bar related code //
            const int totalTicks = 10;
            var options = new ProgressBarOptions
            {
                ForegroundColor = ConsoleColor.Yellow,
                ForegroundColorDone = ConsoleColor.DarkGreen,
                BackgroundColor = ConsoleColor.DarkGray,
                BackgroundCharacter = '\u2593'
            };
            //var childOptions = new ProgressBarOptions
            //{
            //    ForegroundColor = ConsoleColor.Green,
            //    BackgroundColor = ConsoleColor.DarkGreen,
            //    ProgressCharacter = '─'
            //};
            //using (var pbar = new ProgressBar(totalTicks, "main progressbar", options))
            //{
            //    TickToCompletion(pbar, totalTicks, sleep: 10, childAction: () =>
            //    {
            //        using (var child = pbar.Spawn(totalTicks, "child actions", childOptions))
            //        {
            //            TickToCompletion(child, totalTicks, sleep: 100);
            //        }
            //    });
            //}

            using var progressBar = new ProgressBar(10000, "uploaded", options);

            Action<FtpProgress> progress = delegate (FtpProgress download)
            {
                var progress = progressBar.AsProgress<double>();
                progress.Report(download.Progress / 100);
            };

            try
            {
                string fullRemotePath = path.Remote + Path.GetFileName(file.File);
                client.UploadFile(@file.File, @fullRemotePath, FtpRemoteExists.Overwrite, true, FtpVerify.OnlyChecksum, progress);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException);
                return -1;
            }
            return 0;
        }


        public static int MultipleFiles(ref FtpClient client, IEnumerable<string> files, in Program.FilePath path)
        {
            try
            {
                client.UploadFiles(files, path.Remote, FtpRemoteExists.Overwrite, true, FtpVerify.OnlyChecksum);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException);
                return -1;
            }
            //foreach (string file in files)
            //    Console.WriteLine(file); // remove this

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
