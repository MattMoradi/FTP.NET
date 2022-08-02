using System;
using System.IO;
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

            Console.WriteLine("\nDownloading File: " + files.Path);

            if (files.Files.Count() > 1)
                return MultipleFiles(files.Files);
            else if (!string.IsNullOrEmpty(files.Directory))
            {
                return Directory(client, files.Directory, files.LocalPath);
            }
            
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

        /// <summary>
        /// Uses FluentFtp library method to find files on a remote server as a query executed against the passed in
        /// ftpClient.
        /// </summary>
        /// <param name="ftpClient">Connection to remote ftp protocal server.</param>
        /// <param name="remoteDirs">List of file directories.</param>
        /// <param name="localDir">Local directory to save files.</param>
        /// <returns>Number of files found or -1 if error occurs.</returns>
        public static int MultipleFiles(FtpClient ftpClient, IEnumerable<string> remoteDirs, string localDir)
        {
            try
            {
                // check if local was provided or not. if not use default.
                localDir = string.IsNullOrEmpty(localDir) ? Environment.CurrentDirectory : localDir;
                
                // executes download of remoteDirectories to the local location.
                var result = ftpClient.DownloadFiles(localDir, remoteDirs);

                // let user know where files were downloaded incase local dir not provided
                if (result > 0)
                    Console.WriteLine($"Files Found Saved to {localDir}");

                // # files downloaded
                return result;
            }
            catch(ArgumentException aExc)
            {
                Console.WriteLine($"Incorrect Parameters: Auto Exception Message {aExc.Message}");
                return -1;
            }
            catch(FtpException ftpExc)
            {
                Console.WriteLine($"FtpClient unexpected error, download failed. Auto Exception Msg: {ftpExc.Message}");
                return -1;
            }
            catch(Exception exc)
            {
                Console.WriteLine($"Failed to retrieve multiple files. Auto Exception Msg: {exc.Message}");
                return -1;
            }
        }

        /// <summary>
        /// Uses the FluentFtp library to find and download a directory from the remote server to the local directory.
        /// </summary>
        /// <param name="ftpClient">Connection to remote ftp protocal server.</param>
        /// <param name="remoteDir">Remote directory to grab.</param>
        /// <param name="localDir">Local directory where items are saved.</param>
        /// <returns>Number of files downloaded from directory.</returns>
        public static int Directory(FtpClient ftpClient, string remoteDir, string localDir = "")
        {
            try
            {
                var result = 0;

                // execute directory download
                var dir = ftpClient.DownloadDirectory(string.IsNullOrEmpty(localDir) ? Environment.CurrentDirectory : localDir, remoteDir, FtpFolderSyncMode.Update);

                // determine how many files were downloaded ignoring the skipped and overwritten files.
                dir.ForEach(d => { if (d.IsDownload) ++result; });
                
                // return number of files downloaded successfully
                return result;
            }
            catch(ArgumentException aExc)
            {
                Console.WriteLine($"Incorrect Parameters, {aExc.Message}");
                return -1;
            }
            catch(Exception exc)
            {
                Console.WriteLine($"Failed to retrieve directory: {remoteDir}. Auto Generated Exception Msg: {exc.Message}");
                return -1;
            }
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
