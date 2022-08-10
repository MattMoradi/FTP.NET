using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentFTP;

namespace Client
{
    public class Logger : ILogger
    {
        public string? mRemotePath;
        public readonly string LocalPath;


        public Logger()
        {
            LocalPath = "Users" + Path.DirectorySeparatorChar + "LocalHost" + ".txt";
            try
            {
                Directory.CreateDirectory("Users");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            mRemotePath = null;
        }
        public Logger(string name)
        {
            LocalPath = "Users" + Path.DirectorySeparatorChar + "LocalHost" + ".txt";
            try
            {
                Directory.CreateDirectory("Users");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            mRemotePath = "Users" + Path.DirectorySeparatorChar + name;
        }


        public void Log(string[] message, in FtpClient client)
        {
            try
            {
                StreamWriter streamWriter;

                if (client.IsAuthenticated)
                    streamWriter = File.AppendText("Users" + Path.DirectorySeparatorChar + client.Credentials.UserName + ".txt");
                else
                    streamWriter = File.AppendText("Users" + Path.DirectorySeparatorChar + "LocalHost" + ".txt");

                for (int i = 0; i < message.Length; i++)
                    streamWriter.Write(message[i] + " ");
                streamWriter.WriteLine("\t" + DateTime.Now);
                streamWriter.Close();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    Console.WriteLine(ex.InnerException.Message);
                else
                    Console.WriteLine(ex.Message);
            }
        }

        //Creates new directory to store log information
        //Directory is located: ftp/Client/bin/Debug/net6.0/Users
        public string CreateDirectory(string name)
        {
            try
            {
                Directory.CreateDirectory("Users");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            //return the file path where the current user's logs will be stored
            return "Users" + Path.DirectorySeparatorChar + name;
        }
    }
}

