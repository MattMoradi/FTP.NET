using System;
using FluentFTP;
using ShellProgressBar;

namespace Client
{
    public static class Get
    {
        public static int File(ref FtpClient client, Commands.Get files)
        {
            if (!client.IsAuthenticated)
            {
                Console.WriteLine("ERROR: No connection to remote server!");
                return -1;
            }

            Console.WriteLine("Downloading File: " + files.Path);

            if (files.Files.Count() > 1)
                return MultipleFiles(files.Files);

            var options = new ProgressBarOptions
            {
                ForegroundColor = ConsoleColor.Yellow,
                ForegroundColorDone = ConsoleColor.DarkGreen,
                BackgroundColor = ConsoleColor.DarkGray,
                BackgroundCharacter = '\u2593'
            };

            using var progressBar = new ProgressBar(10000, "downloaded", options);

            Action<FtpProgress> progress = delegate (FtpProgress download)
            {
                    var progress = progressBar.AsProgress<double>();
                    progress.Report(download.Progress / 100);
            };

            try
            {
                string fileName = files.Path.Substring(files.Path.LastIndexOf('/') + 1);

                if (files.Path != null)
                    client.DownloadFile(fileName, files.Path, FtpLocalExists.Overwrite, FtpVerify.OnlyChecksum, progress);
                else
                    Console.WriteLine("ERROR: File not specified!");
            }
            catch (Exception x)
            {
                Console.WriteLine(x.ToString());
            }
            return 0;
        }

        public static int MultipleFiles(IEnumerable<string> files)
        {
            foreach (string file in files)
                Console.WriteLine(file); // remove this
            return 0;
        }

        public static int List(ref FtpClient client, Commands.List directory)
        {
            Console.WriteLine();
            string path = "./";
            try
            {
                if (directory.Local != null)
                {
                    path = Path.GetFullPath(directory.Local);
                }
                else if (directory.Remote != null)
                {
                    throw new InvalidOperationException("listing remote files (ls -r) not implemented");         // to be implemented!
                }

                DirectoryInfo dir = new DirectoryInfo(path);
                DirectoryInfo[] sub_directories = dir.GetDirectories();
                FileInfo[] files = dir.GetFiles();

                // List sub-directories
                foreach (DirectoryInfo i in sub_directories)
                {
                    Console.WriteLine("{0}/", i.Name);
                }

                if (sub_directories.Length > 0)
                    Console.WriteLine();

                // List files
                foreach (FileInfo j in files)
                {
                    Console.WriteLine(j.Name);
                }
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine("Directory not found");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine();
            return 0;
        }
    }
}
