using System;
using FluentFTP;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
           FtpClient client = new FtpClient();

            Console.WriteLine("FTP.NET v1.0");

            if(Connection.Connect(ref client))
                Console.WriteLine("Successfully connected to server!");
            else
                Console.WriteLine("ERROR: did not connect to server!");


            Console.WriteLine(client.Credentials.UserName);
            Console.WriteLine(client.Credentials.Password);
            Console.WriteLine(client.Host + ":" + client.Port);
        }
    }
}
