using System;
using System.IO;
using FluentFTP;
using ShellProgressBar;
using static Client.Program;

namespace Client
{
    public static class Get
    {
        public static int File(ref FtpClient client, Commands.Get files, FilePath path)
        {
            if (!client.IsAuthenticated)
            {
                Console.WriteLine("ERROR: No connection to remote server!");
                return -1;
            }

            if (files.Files.Count() >= 1)
            {
                var toCollect = new List<string>();
                var corrected = false;
                if (!string.IsNullOrEmpty(files.Path))
                {
                    toCollect.Add(files.Path);
                    files.Files.ToList().ForEach(file => toCollect.Add(file));
                    corrected = true;
                }

                return MultipleFiles(client, corrected ? toCollect : files.Files, files.LocalPath, path);
            }
            else if (!string.IsNullOrEmpty(files.Directory))
            {
                return RemoteDirectory(client, files.Directory, path, files.LocalPath);
            }
            
            var options = new ProgressBarOptions
            {
                ForegroundColor = ConsoleColor.Yellow,
                ForegroundColorDone = ConsoleColor.DarkGreen,
                BackgroundColor = ConsoleColor.DarkGray,
                BackgroundCharacter = '\u2593'
            };

            try
            {
                string fileName; 

                if (files.Path != null)
                {
                    fileName = files.Path.Substring(files.Path.LastIndexOf('/') + 1);

                    if (client.FileExists(files.Path))
                    {
                        if (!client.DirectoryExists(files.Path))
                        {
                            Console.WriteLine("\nDownloading File: " + files.Path);
                            using var progressBar = new ProgressBar(10000, "downloaded", options);

                            Action<FtpProgress> progress = delegate (FtpProgress download)
                            {
                                var progress = progressBar.AsProgress<double>();
                                progress.Report(download.Progress / 100);
                            };

                            client.DownloadFile(fileName, files.Path, FtpLocalExists.Overwrite, FtpVerify.OnlyChecksum, progress);
                        }
                        else
                            Console.WriteLine("ERROR: Get does not support directories!");
                    }
                    else
                        Console.WriteLine($"ERROR: File \"{files.Path}\" not found!");
                }
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
        public static int MultipleFiles(FtpClient ftpClient, IEnumerable<string> remoteDirs, string localDir, FilePath paths)
        {
            try
            {
                var result = 0;
                var badFleNmeCount = 0;

                // Verify local directory is a directory
                if (!string.IsNullOrEmpty(localDir) && localDir.Contains('.'))
                {
                    Console.WriteLine("Error: local directory must be a directory not a file path. Try Again");
                    Console.WriteLine("Expected: get -m <file1.txt> <file2.txt> ... -l <LocalDirectory>");
                    return -1;
                }

                var items = new List<string>();
                // Check for incorrect file names
                remoteDirs.ToList().ForEach(rd =>
                {
                    if (!rd.Contains('.') || rd.Last().Equals('.'))
                    {
                        Console.WriteLine($"Error: Incorrect File Name: {rd}, Missing File Extension");
                        Console.WriteLine("Expected: <filename.txt>");
                        ++badFleNmeCount;
                        items.Add(paths.Remote+rd);
                    }
                    else
                    {
                        items.Add(paths.Remote+rd);
                    }
                });

                // if all file names are bad display error and return
                if (badFleNmeCount == remoteDirs.Count())
                {
                    Console.WriteLine("Error: Incorrect Remote File Names. Try again.");
                    return -1;
                }

                if (ftpClient.IsAuthenticated)
                {
                    Console.WriteLine("Downloading Files...\n");

                    // check if local was provided or not. if not use default.
                    localDir = string.IsNullOrEmpty(localDir) ? Environment.CurrentDirectory : localDir;

                    // executes download of remoteDirectories to the local location.
                    result = ftpClient.DownloadFiles(localDir, items);

                }
                else
                {
                    Console.WriteLine("Error: Host not specified try the \"connect\" command.");
                    return -1;
                }

                if ((result + badFleNmeCount) - remoteDirs.Count() == 0)
                    Console.WriteLine($"{result} File(s) Downloaded. {badFleNmeCount} File(s) Failed for incorrect file name.");

                // let user know where files were downloaded incase local dir not provided
                if (result > 0)
                {
                    Console.WriteLine($"Files Found Saved to {localDir}");
                }
                else
                {
                    Console.WriteLine("Error: Failed to Download Files. Try again.");
                }
                // # files downloaded
                return result;
            }
            catch(ArgumentException aExc)
            {
                Console.WriteLine($"Error: Incorrect Parameters: Auto Exception Message {aExc.Message}");
                Console.WriteLine("Expected: get -m <filename.txt> <filename1.txt> ...");
                return -1;
            }
            catch(FtpException ftpExc)
            {
                Console.WriteLine($"FtpClient unexpected error, download failed. Auto Exception Msg: {ftpExc.Message}");
                Console.WriteLine("Expected: get -m <filename.txt> <filename1.txt> ...");
                return -1;
            }
            catch(Exception exc)
            {
                Console.WriteLine($"Failed to retrieve multiple files. Auto Exception Msg: {exc.Message}");
                Console.WriteLine("Expected: get -m <filename.txt> <filename1.txt> ...");
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
        public static int RemoteDirectory(FtpClient ftpClient, string remoteDir, FilePath path, string localDir = "")
        {
            try
            {
                if (string.IsNullOrEmpty(remoteDir) || remoteDir.Contains('.'))
                {
                    Console.WriteLine($"Incorrect remote directory name value: {remoteDir}. Try again.");
                    return - 1;
                }

                if (!string.IsNullOrEmpty(localDir) && localDir.Contains('.'))
                {
                    Console.WriteLine($"Incorrect local directory name: {localDir} try again.");
                    return -1;
                }

                var result = 0;

                if (ftpClient.IsAuthenticated)
                {
                    Console.WriteLine($"Downloading Directory: {remoteDir}...");

                    // Use the user indicated directory or evironement directory if not specified
                    localDir = string.IsNullOrEmpty(localDir) ? Environment.CurrentDirectory : localDir;

                    // execute directory download
                    var dir = ftpClient.DownloadDirectory(localDir, path.Remote + remoteDir, FtpFolderSyncMode.Update);
                
                    // determine how many files were downloaded ignoring the skipped and overwritten files.
                    dir.ForEach(d => { if (d.IsDownload) ++result; });
                }
                else
                {
                    Console.WriteLine("Error: Host not Specified. Try the \"connect\" command.");
                    return -1;
                }
                
                if (result > 0)
                {
                    Console.WriteLine($"Directory downloaded to {localDir}.");
                }
                else
                {
                    Console.WriteLine("Error: Failed to Download directory");
                }

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

        //Wrapper to check which directory should be displayed to the console
        public static int List(ref FtpClient client, in Program.FilePath path, in string[] args)
        {
            if ((!args.Contains("-r")) && (!args.Contains("-l") && !client.IsAuthenticated) || args.Contains("-l"))
                return ListLocalDirectory(in path, in args);
            else
                return ListRemoteDirectory(ref client, in path, in args);  
        }

        //Lists the current directory the user is in on the remote server
        public static int ListRemoteDirectory(ref FtpClient client, in Program.FilePath path, in string[] args)
        {
            try
            {
                if (!client.IsAuthenticated)
                {
                    Console.WriteLine("Must be connected to a remote server to display directory!");
                    return -1;
                }

                string displayPath = path.Remote;
                int index = Array.IndexOf(args, "ls");

                //find the number of args provided and where the
                //directory that is wanting to be displayed is located
                if (index + 3 == args.Length)//passes when the users provides -r flag
                    displayPath += args[2];
                else if(index + 2 == args.Length && args[1] != "-r")//passes when the user does not provide -r flag
                    displayPath +=args[1];
               
                
                if(!client.DirectoryExists(displayPath))
                {
                    Console.WriteLine("No such directory exists!\n");
                    return -1;
                }
                FtpListItem[] items = client.GetListing(displayPath);
                
                Console.WriteLine(displayPath);

                foreach (FtpListItem item in items)
                {
                    if (item.Type == FtpFileSystemObjectType.Directory)
                        Console.WriteLine(item.Name + "/");
                    else
                        Console.WriteLine(item.Name);
                }

                Console.WriteLine();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    Console.WriteLine(ex.InnerException.Message);
                else
                    Console.WriteLine(ex.Message);
                return -1;
            }
            return 0;
        }

        //Lists the current directory the user is in on their local machine
        private static int ListLocalDirectory(in Program.FilePath path, in string[] args)
        {
            Console.WriteLine();

            try
            {
                DirectoryInfo dir;
                if (args.Length == 2)
				{
                    if (args[1] == "-l")
                        dir = new DirectoryInfo(path.Local);
                    else
                        dir = new DirectoryInfo(args[1]);
                }
                else if (args.Length == 3)
				{
                    dir = new DirectoryInfo(args[2]);
                }
                else if (args.Length >= 4)
				{
                    throw new Exception("Too many arguments!");
				}
                else
				{
                    dir = new DirectoryInfo(path.Local);
				}

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
                Console.WriteLine("Error: Directory not found");
                return -1;
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                {
                    Exception realerror = e;
                    while (realerror.InnerException != null)
                        realerror = realerror.InnerException;

                    Console.WriteLine("Error: " + realerror.Message);
                }
                else
				{
                    Console.WriteLine("Error: " + e.Message);
				}
                return -1;
            }
            
        Console.WriteLine();
        return 0;
    }
        //Wrapper function to change the directory the user wants to change to
        public static int ChangeDirectory(in FtpClient client, ref Program.FilePath path, in string[] args)
        {
            try
            {
                int index = Array.IndexOf(args, "cd");

                if (args.Contains(".."))
                    return GoToPrevDirectory(ref path, in args, in client);
                if ((!args.Contains("-r")) && (!args.Contains("-l") && !client.IsAuthenticated) || args.Contains("-l"))
                    return ChangeLocalDirectory(ref path, in args, index);
                else
                    return ChangeRemoteDirectory(client, ref path, in args, index);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    Console.WriteLine(ex.InnerException.Message);
                else
                    Console.WriteLine(ex.Message);
            }
            return 0;
        }

        //Removes the current directory the user is in from the file path.
        //If the user is at the root directory, nothing is removed.
        public static int GoToPrevDirectory(ref Program.FilePath path, in string[] args, in FtpClient client)
        {
            try
            {
                if ((!args.Contains("-r")) && (!args.Contains("-l") && !client.IsAuthenticated) || args.Contains("-l"))
                    return GoToPrevLocalDirectory(ref path);
                else
                    return GoToPrevRemoteDirectory(ref path, in client);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    Console.WriteLine(ex.InnerException.Message);
                else
                    Console.WriteLine(ex.Message);
                return -1;
            }
        }

        public static int GoToPrevLocalDirectory(ref Program.FilePath path)
        {
            if (IsAtRootDirectory(path.Local))
                return -1;
            string temp = path.Local.Substring(path.Local.Substring(0, path.Local.LastIndexOf("/")).LastIndexOf("/") + 1);
            path.Local = path.Local.Remove(path.Local.LastIndexOf(temp));
            Console.WriteLine(path.Local + "\n");
            return 0;
        }

        public static int GoToPrevRemoteDirectory(ref Program.FilePath path, in FtpClient client)
        {
            if (!client.IsAuthenticated)
            {
                Console.WriteLine("Must be connected to a remote server to change directory!");
                return -1;
            }
            if (IsAtRootDirectory(path.Remote))
                return -1;
            string temp = path.Remote.Substring(path.Remote.Substring(0, path.Remote.LastIndexOf("/")).LastIndexOf("/") + 1);
            path.Remote = path.Remote.Remove(path.Remote.LastIndexOf(temp));
            Console.WriteLine(path.Remote + "\n");
            return 0;
        }
        //method that changes the remote directory of the remote server
        public static int ChangeRemoteDirectory(in FtpClient client, ref Program.FilePath path, in string[] args, int index)
        {
            try
            {
                if (!client.IsAuthenticated)
                {
                    Console.WriteLine("Must be connected to a remote server to display directory!");
                    return -1;
                }
                string tempPath = path.Remote;
                //find the number of args provided and where the
                //directory that is wanting to be displayed is located
                if (args[args.Length - 1] == "")
                    return -1;
                if (index + 3 == args.Length)//passes when the users provides -r flag
                    tempPath += args[2] + "/";
                else if (index + 2 == args.Length)//passes when the user does not provide -r flag
                    tempPath += args[1] + "/";

                //check if user entered a valid directory to change to
                if (!client.DirectoryExists(tempPath))
                {
                    Console.WriteLine("No such directory exists!\n");
                    return -1;
                }
                else
                {
                    path.Remote = tempPath;
                    Console.WriteLine(path.Remote + "\n");
                    return 0;
                }

            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    Console.WriteLine(ex.InnerException.Message);
                else
                    Console.WriteLine(ex.Message);
                return -1;
            }
        }

        //Method that changes the current local directory
        public static int ChangeLocalDirectory(ref Program.FilePath path, in string[] args, int index)
        {
            string tempPath;
            if (args[args.Length-1] == "")
                return -1;
            // For absolute path ex c:\dev
            if (args.Count() == 3 && args[2].Equals(':'))
			{
                tempPath = args[2];
			}
            else if (args.Count() == 2 && args[1][1].Equals(':'))
			{
                tempPath = args[1];
            }
            else
			{
                tempPath = path.Local;
                //find the number of args provided and where the
                //directory that is wanting to be displayed is located
                if (index + 3 == args.Length)//passes when the users provides -l flag
                    tempPath += args[2] + "/";
                else if (index + 2 == args.Length)//passes when the user does not provide -l flag
                    tempPath += args[1] + "/";
            }


            if (!Directory.Exists(tempPath))
            {
                Console.WriteLine("No such directory exists!\n");
                return -1;
            }
            else
            {
                path.Local = tempPath;
                Console.WriteLine(path.Local + "\n");
                return 0;
            }
        }
        //check if current path is at root by counting number of '/'
        //characters that occur in the path string
        public static bool IsAtRootDirectory(in string path)
        {
            if (path.Count(f => f == '/') <= 1)
                return true;
            return false;
        }
    }
}
