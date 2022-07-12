using System;
using FluentFTP;

namespace Client
{
    public static class Modify
    {
        public static int Delete(ref FtpClient client, Commands.Delete file)
        {
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
