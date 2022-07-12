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

        public static int List(ref FtpClient client, Commands.List directory)
        {
            if (directory.Local != null)
            {
                try
                {
                    // throws an exception if invalid
                    string path = Path.GetFullPath(directory.Local);

                    // collect info
                    DirectoryInfo dir = new DirectoryInfo(path);
                    DirectoryInfo[] sub_directories = dir.GetDirectories();
                    FileInfo[] files = dir.GetFiles();

                    // list directories
                    Console.WriteLine();
                    foreach (DirectoryInfo i in sub_directories)
                    {
                        Console.WriteLine("./{0}", i.Name);
                    }

                    // list files
                    Console.WriteLine();
                    foreach (FileInfo j in files)
                    {
                        Console.WriteLine("{0}", j.Name);
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            else if (directory.Remote != null)
            {
                Console.WriteLine("Remote");
            }
            // invlalid input
            else
            {
                Console.WriteLine("Invalid input. Usage: ls -l c:/");
            }
            return 0;
        }
    }
}
