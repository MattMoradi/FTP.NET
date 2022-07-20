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
                    // do remote stuff
                }

                DirectoryInfo dir = new DirectoryInfo(path);
                DirectoryInfo[] sub_directories = dir.GetDirectories();
                FileInfo[] files = dir.GetFiles();

                foreach (DirectoryInfo i in sub_directories)
                {
                    Console.WriteLine(".\\{0}", i.Name);
                }
                if (sub_directories.Length > 0)
                    Console.WriteLine();
                foreach (FileInfo j in files)
                {
                    Console.WriteLine("{0}", j.Name);
                }
                Console.WriteLine();
            }
            catch(DirectoryNotFoundException e)
            {
                Console.WriteLine("Directory not found\n");
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
            return 0;
        }
    }
}
