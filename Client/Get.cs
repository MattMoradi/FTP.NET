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

        public static int List(ref FtpClient client, Commands.List directory, in Program.FilePath path, in string[] args)
        {
            if (args.Contains("-r"))
            {
                try
                {
                    Console.WriteLine(path.Remote);
                    FtpListItem[] items = client.GetListing(path.Remote);

                    foreach (FtpListItem item in items)
                        Console.WriteLine(item.Name);
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                        Console.WriteLine(ex.InnerException.Message);
                    else
                        Console.WriteLine(ex.Message);
                    return 0;
                }
                return 1;
            }

            else if (args.Contains("-l"))//i'll let Peter configure this part for his section
            {
                Console.WriteLine();
                string filePath = "./";
                try
                {
                    if (directory.Local != null)
                    {
                        filePath = Path.GetFullPath(directory.Local);
                    }
                    else if (directory.Remote != null)
                    {
                        throw new InvalidOperationException("listing remote files (ls -r) not implemented");         // to be implemented!
                    }

                    DirectoryInfo dir = new DirectoryInfo(filePath);
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
            }
                Console.WriteLine();
                return 0;
        }
    }
}
