using System;
using FluentFTP;

namespace Client
{
    public static class Connection
    {
        public static bool Connect(ref FtpClient client)
        {
            Console.Write("\nEnter Server Address: ");
            client.Host = Console.ReadLine();
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

            return client.IsAuthenticated;
        }

        public static void Save()
        {

        }

        public static void Timeout()
        {

        }
    }
}
