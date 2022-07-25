using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    internal class Logger
    {
        private string mFilePath;

        public Logger(string name)
        {
            mFilePath = CreateDirectory(name);
        }

        //log what the user typed and append that information to their log
        public void Log(string message)
        {
            StreamWriter streamWriter = File.AppendText(mFilePath + ".txt");
            streamWriter.WriteLine(message + "\t" + DateTime.Now);
            streamWriter.Close();
        }
        public void Log(string[] message)
        {
            StreamWriter streamWriter = File.AppendText(mFilePath + ".txt");
            for (int i = 0; i < message.Length; i++)
                streamWriter.Write(message[i] + " ");
            streamWriter.WriteLine("\t" + DateTime.Now);
            streamWriter.Close();
        }


        //Creates new directory to store log information
        //Directory is located: ftp/Client/bin/Debug/net6.0/Users
        private string CreateDirectory(string name)
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

