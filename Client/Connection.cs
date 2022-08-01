using System.Security.Cryptography;
using FluentFTP;

namespace Client
{
    public static class Connection
    {
        public static int Connect(ref FtpClient client, Commands.Connect commands)
        {
            client.Host = commands.IP;
            bool firstAuth = false;

            if (File.Exists(commands.IP + ".txt"))
            {
                try
                {
                    string? credFile = commands.IP + ".txt";
                    File.Decrypt(credFile);
                    using (StreamReader Reader = new StreamReader(credFile))
                    {
                        client.Credentials.UserName = Reader.ReadLine();
                        client.Credentials.Password = Reader.ReadLine();
                        client.AutoConnect();
                    };
                    File.Encrypt(credFile);
                }
                catch(Exception x)
                { 
                    Console.WriteLine(x.ToString()); 
                }
            }
            else
            {
                Console.Write("Enter the username: ");
                client.Credentials.UserName = Console.ReadLine();
                Console.Write("Enter the password: ");
                client.Credentials.Password = Console.ReadLine();

                try
                {
                    client.AutoConnect();
                    firstAuth = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            if (client.IsAuthenticated)
            {
                if (firstAuth)
                {
                    Console.WriteLine("Would you like to save your login credentials? (Y/N): ");
                    string? input = Console.ReadLine();
                    if (input == "y" || input == "Y")
                        Save(ref client);
                }
                return 0;
            }
            else
                return -1;
        }

        public static int Save(ref FtpClient client)
        {
            string? credsFile = client.Host + ".txt";
            try
            {
                StreamWriter credentials = new StreamWriter(credsFile);
                credentials.WriteLine(client.Credentials.UserName);
                credentials.WriteLine(client.Credentials.Password);
                credentials.Close();
                File.Encrypt(credsFile);
                return 0;
            }
            catch(Exception x)
            {
                File.Delete(credsFile);
                Console.WriteLine(x.ToString());
                return -1;
            }
        }

        public static void Timeout(ref FtpClient client)
        {

        }

        public static int Disconnect(ref FtpClient client)
        {
            client.Disconnect();
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
