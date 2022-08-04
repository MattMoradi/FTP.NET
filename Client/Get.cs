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

            Console.WriteLine("\nDownloading File: " + files.Path);

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

        //Wrapper to check which directory should be displayed to the console
        public static int List(ref FtpClient client, in Program.FilePath path, in string[] args)
        {
            if ((!args.Contains("-r")) && (!args.Contains("-l") && !client.IsAuthenticated) || args.Contains("-l"))
                return ListLocalDirectory(in path);//ill let peter configure this part
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
        private static int ListLocalDirectory(in Program.FilePath path)
        {
            Console.WriteLine();
            //string filePath = "/";
            try
            {
                //I don't think this part is needed anymore
                /*
                if (directory.Local != null)
                {
                    filePath = Path.GetFullPath(directory.Local);
                }
                else if (directory.Remote != null)
                {
                    throw new InvalidOperationException("listing remote files (ls -r) not implemented");         // to be implemented!
                }*/

                DirectoryInfo dir = new DirectoryInfo(path.Local);
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
                return -1;
            }
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
            string tempPath = path.Local;
            //find the number of args provided and where the
            //directory that is wanting to be displayed is located
            if (index + 3 == args.Length)//passes when the users provides -l flag
                tempPath += args[2] + "/";
            else if (index + 2 == args.Length)//passes when the user does not provide -l flag
                tempPath += args[1] + "/";

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
