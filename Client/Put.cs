using System;
using FluentFTP;
namespace Client
{
    public static class Put
    {
        public static int File(ref FtpClient client, Commands.Put file)
        {
            Console.WriteLine("file: " + file.File);

            if (file.Files.Count() > 1)
                return MultipleFiles(file.Files);
            return 0;
        }

        public static int MultipleFiles(IEnumerable<string> files)
        {
            foreach (string file in files)
                Console.WriteLine(file); // remove this

            return 0;
        }

        public static int Create(ref FtpClient client, Commands.CreateDirectory directory)
        {
            client.CreateDirectoryAsync(directory.Name);

            return 0;
        }

        public static int Copy(ref FtpClient client, Commands.Copy file)
        {
            return 0;
        }
    }
}
