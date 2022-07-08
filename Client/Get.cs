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
            Console.WriteLine("list");
            return 0;
        }
    }
}
