using System.Text;
using System.Security.Cryptography;
using FluentFTP;

namespace Client
{
    public static class Connection
    {
        public const string passkey = "1oH8TUmMUeXdwiTftARezWQXCjuFxyIr6mAu/PdQWWg=";
        public const string iv = "21EAPkLZuJ9mYtV0GkDPjQ==";

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
            bool firstAuth = false;

            if (File.Exists(commands.IP + ".txt"))
            {
                try
                {
                    string? credFile = commands.IP + ".txt";
                    using (StreamReader Reader = new StreamReader(credFile))
                    {
                        client.Credentials.UserName = Reader.ReadLine();
                        password = Reader.ReadLine();
                        Reader.Close();
                    };

                    Aes cipher = Aes.Create();
                    cipher.Key = Convert.FromBase64String(passkey);
                    cipher.IV = Convert.FromBase64String(iv);
                    ICryptoTransform cryptoTransform = cipher.CreateDecryptor();
                    byte[] pass = Convert.FromBase64String(password);
                    client.Credentials.Password = Encoding.UTF8.GetString(cryptoTransform.TransformFinalBlock(pass, 0, pass.Length));
                    client.AutoConnect();
                }
                catch (Exception x)
                {
                    Console.WriteLine(x.ToString());
                }
            }
            else
            {
                Console.Write("Enter the username: ");
                client.Credentials.UserName = Console.ReadLine();
                Console.Write("Enter the password: ");
                firstAuth = true;

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
            }

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
                if (firstAuth)
                {
                    Console.Write("Would you like to save your login credentials? (Y/N): ");
                    string? input = Console.ReadLine();
                    if (input == "y" || input == "Y")
                        Save(client);
                }

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

        public static int Save(FtpClient client)
        {
            string? credsFile = client.Host + ".txt";

            try
            {
                Aes cipher = Aes.Create();
                cipher.Key = Convert.FromBase64String(passkey);
                cipher.IV = Convert.FromBase64String(iv);
                ICryptoTransform cryptoTransform = cipher.CreateEncryptor();
                byte[] pass = Encoding.UTF8.GetBytes(client.Credentials.Password);

                StreamWriter credentials = new StreamWriter(credsFile);
                credentials.WriteLine(client.Credentials.UserName);
                credentials.WriteLine(Convert.ToBase64String(cryptoTransform.TransformFinalBlock(pass, 0, pass.Length)));
                credentials.Close();
                return 0;
            }
            catch (Exception x)
            {
                File.Delete(credsFile);
                Console.WriteLine("ERROR: " + x.Message);
                return -1;
            }
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
