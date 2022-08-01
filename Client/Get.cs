using System;
using FluentFTP;

namespace Client
{
    public static class Get
    {
        public static int File(ref FtpClient client, Commands.Get files)
        {
            Console.WriteLine("file: " + files.File);

            if (files.Files.Count() > 1)
                return MultipleFiles(files.Files);
            return 0;
        }

        public static int MultipleFiles(IEnumerable<string> files)
        {
            foreach (string file in files)
                Console.WriteLine(file); // remove this
            return 0;
        }

        //Wrapper to check which directory should be displayed to the console
        public static int List(ref FtpClient client, Commands.List directory, in Program.FilePath path, in string[] args)
        {
            if (args.Contains("-l"))//i'll let Peter configure this part for his section
                return ListLocalDirectory(in path);
            else
                return ListRemoteDirectory(ref client, in path, in args);  
        }

        //Lists the current directory the user is in on the remote server
        private static int ListRemoteDirectory(ref FtpClient client, in Program.FilePath path, in string[] args)
        {
            if (!client.IsAuthenticated)
            {
                Console.WriteLine("Must be connected to a remote server to display directory!");
                return -1;
            }
           
            try
            {
                string displayPath = path.Remote;
                int index = Array.IndexOf(args, "ls");

                //check if there is a 3rd argument provided.
                //It should be the directory the user wants to display
                if (index + 3 <= args.Length)
                    displayPath += args[2];
               
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

        public static int ChangeDirectory(in FtpClient client, ref Program.FilePath path, in string[] args)
        {
            bool isRemote = args.Contains("-r");
            bool isLocal = args.Contains("-l");

            if (!client.IsConnected && isRemote)
            {
                Console.WriteLine("Must be connected to a server to change remote directory!\n");
                return -1;
            }

            if (!isRemote && !isLocal)
            {
                Console.WriteLine("Must specify local or remote directory!\n");
                return -1;
            }
           
            try
            {
                //find the index where 'cd' occurs in the args array
                int index = Array.IndexOf(args, "cd");
                string tempPath;

                if (args[index + 2] == "..")
                    return GoToPrevDirectory(ref path, in args);
               
                if (isRemote)
                {
                     tempPath = path.Remote + args[index + 2] + "/";

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

                if (isLocal)
                {
                    tempPath = path.Local + args[index + 2] + "/";
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
                return 0;
                
            }
            catch(IndexOutOfRangeException)
            {
                Console.WriteLine("Must provide a directory to change to!\n");
            }
            catch(Exception ex)
            {
                if (ex.InnerException != null)
                    Console.WriteLine(ex.InnerException.Message);
                else
                    Console.WriteLine(ex.Message);
            }
            return -1;
        }

        private static bool ChangeLocalDirectory()
        {
            return false;
        }
        //check if current path is at root by counting number of '/'
        //characters that occur in the path string
        private static bool IsAtRootDirectory(in string path)
        {
            if (path.Count(f => f == '/') <= 1)
                return true;
            return false;
        }

        //Removes the current directory the user is in from the file path.
        //If the user is at the root directory, nothing is removed.
        private static int GoToPrevDirectory(ref Program.FilePath path, in string[] args)
        {
            try
            {
                if (args.Contains("-r"))
                {
                    if (IsAtRootDirectory(path.Remote))
                        return 0;
                    string temp = path.Remote.Substring(path.Remote.Substring(0, path.Remote.LastIndexOf("/")).LastIndexOf("/") + 1);
                    path.Remote = path.Remote.Remove(path.Remote.LastIndexOf(temp));
                    Console.WriteLine(path.Remote + "\n");
                    return 0;
                }
                if (args.Contains("-l"))
                {
                    if (IsAtRootDirectory(path.Local))
                        return 0;
                    string temp = path.Local.Substring(path.Local.Substring(0, path.Local.LastIndexOf("/")).LastIndexOf("/") + 1);
                    path.Local = path.Local.Remove(path.Local.LastIndexOf(temp));
                    Console.WriteLine(path.Local + "\n");
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
            return 0;
        }
    }
}
