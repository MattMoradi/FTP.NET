using System;
using FluentFTP;

namespace Client
{
    public static class Connection
    {
        public static int Connect(ref FtpClient client, ref Logger logger, Commands.Connect commands, ref Program.FilePath path)
        {
            
            if (commands.IP == null)
            {
                Console.WriteLine("ERROR: must provide a host name to connect!");
                return -1;
            }

            string password = String.Empty;
            ConsoleKey key;
            client.Host = commands.IP;

            Console.Write("Enter the username: ");
            client.Credentials.UserName = Console.ReadLine();
            Console.Write("Enter the password: ");

            do
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);
                key = keyInfo.Key;

                if (key == ConsoleKey.Backspace && password.Length > 0)
                {
                    Console.Write("\b \b");
                    password = password[0..^1];
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    Console.Write("*");
                    password += keyInfo.KeyChar;
                }
            } while (key != ConsoleKey.Enter);

            client.Credentials.Password = password;
            Console.WriteLine();

            try
            {
                client.Connect();
                path.ResetPaths();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    Console.WriteLine(ex.InnerException.Message);
                else
                    Console.WriteLine(ex.Message);
                return -1;
            }

            if (client.IsAuthenticated)
            {
                logger = new Logger(client.Credentials.UserName);
                Console.WriteLine("Successfully connected to server!");
                return 0;
            }
            else
            {
                Console.WriteLine("ERROR: unable to connect to server!");
                return -1;
            }
        }
     
        public static int Save(ref FtpClient client)
        {
            return 0;
        }

        public static void Timeout(ref FtpClient client)
        {

        }

        public static int Disconnect(ref FtpClient client, ref Logger? logger)
        {
            client.Disconnect();
            logger = null;
            Console.WriteLine("--- All Connections Terminated ---\n");
            return 0;
        }

        public static int Exit()
        {
            Environment.Exit(0);
            return 0;
        }
    }
}
