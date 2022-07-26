using System;
using FluentFTP;

namespace Client
{
    public static class Modify
    {
        public static int Delete(ref FtpClient client, Commands.Delete file)
        {
            bool isDir = client.DirectoryExists(file.File);
            bool isFile = client.FileExists(file.File);

            if (isDir)
            {
                client.DeleteDirectory(file.File);
            }
            else if (isFile)
            {
                client.DeleteFile(file.File);
            }
            else
            {
                Console.WriteLine("The specified file/directory does not exist.");
            }

            return 0;
        }

        public static int Permissions(ref FtpClient client, Commands.Permissions file)
        {
            return 0;
        }

        public static int Rename(ref FtpClient client, Commands.Rename file)
        {
            return 0;
        }
    }
}
