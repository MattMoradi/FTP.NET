using FluentFTP;
namespace ftp
{
    class program
    {
        static void Main(string[] args)
        {
           FtpClient client = new FtpClient();

            if(Connect(ref client))
                Console.WriteLine("Successfully connected to server!");
            else
                Console.WriteLine("ERROR: did not connect to server!");


            Console.WriteLine(client.Credentials.UserName);
            Console.WriteLine(client.Credentials.Password);
            Console.WriteLine(client.Host + ":" + client.Port);
        }

        static public bool Connect(ref FtpClient client)
        {
            Console.WriteLine("\nEnter the server you want to join");
            client.Host = Console.ReadLine();
            Console.WriteLine("Enter the username:");
            client.Credentials.UserName = Console.ReadLine();
            Console.WriteLine("Enter the password:");
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
    }
}
