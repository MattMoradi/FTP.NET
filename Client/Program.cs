using System;
using System.Reflection;
using CommandLine;
using FluentFTP;
using Figgle;

namespace Client
{
    public class Program
    {
        //public static FtpClient? Client { get; set; }
        public struct FilePath
        {
            public string local { get; set; }
            public string remote { get; set; }
        }

        static void Main(string[] args)
        {
            FtpClient client = new FtpClient();
            FilePath path = new FilePath();
            Console.WriteLine("FTP Client v1.0");
            Console.WriteLine(FiggleFonts.Big.Render("FTP. NET"));

            while (true)
            {
                Console.Write("> ");
                args = Console.ReadLine().Split(' ');

                var types = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.GetCustomAttribute<VerbAttribute>() != null).ToArray();


                //Parser.Default.ParseArguments(args, types).WithParsed(Run);

                Parser.Default.ParseArguments<Commands.Connect, Commands.List, Commands.Get, Commands.Disconnect, Commands.Quit,
                    Commands.Put, Commands.CreateDirectory, Commands.Delete, Commands.Permissions, Commands.Copy, Commands.Save,
                    Commands.Rename>(args).MapResult(
                (Commands.Connect opts) => Connection.Connect(ref client, opts),
                (Commands.List opts) => Get.List(ref client, opts, ref path),
                (Commands.Get opts) => Get.File(ref client, opts),
                (Commands.Disconnect opts) => Connection.Disconnect(ref client),
                (Commands.Quit opts) => Connection.Exit(),
                (Commands.Put opts) => Put.File(ref client, opts),
                (Commands.CreateDirectory opts) => Put.Create(ref client, opts),
                (Commands.Delete opts) => Modify.Delete(ref client, opts),
                (Commands.Permissions opts) => Modify.Permissions(ref client, opts),
                (Commands.Copy opts) => Put.Copy(ref client, opts),
                (Commands.Save opts) => Connection.Save(ref client),
                (Commands.Rename opts) => Modify.Rename(ref client, opts),
                errs => 1);
            }
        }
    }
}
