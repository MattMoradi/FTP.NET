using System;
using FluentFTP;

namespace Client
{
    public static class Connection
    {
        public static int Connect(ref FtpClient client, Commands.Connect commands)
        {
            client.Host = commands.IP;
            Console.Write("Enter the username: ");
            client.Credentials.UserName = Console.ReadLine();
            Console.Write("Enter the password: ");
            client.Credentials.Password = Console.ReadLine();

            try
            {
                client.AutoConnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            if (client.IsAuthenticated)
                return 0;
            else
                return -1;
        }

        public static int Save(ref FtpClient client)
        {
            return 0;
        }

        public static void Timeout(ref FtpClient client)
        {

        }

        public static int Disconnect(ref FtpClient client)
        {
            return 0;
        }

        public static int Exit()
        {
            Environment.Exit(0);
            return 0;
        }
    }
}
