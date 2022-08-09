using System;
using System.Reflection;
using CommandLine;
using FluentFTP;
using Figgle;

namespace Client
{
    public class Program
    {
        public struct FilePath
        {
            public string Local { get; set; }
            public string Remote { get; set; }
            private string StartingLocalPath;
            private string StartingRemotePath;
            public void ResetPaths()
            { 
                Local = StartingLocalPath;
                Remote = StartingRemotePath;
            }
            public void SetInitalPaths(in string local, in string remote)
            {
                Local = local;
                StartingLocalPath = local;
                Remote = remote;
                StartingRemotePath = remote;
            }
        }

        static void Main(string[] args)
        {
            FtpClient client = new ();
            FilePath path = new ();
            Logger? logger = null;

            path.SetInitalPaths("./", "/");


            Console.WriteLine("FTP Client v1.0");
            Console.WriteLine(FiggleFonts.Big.Render("FTP. NET"));

            while (true)
            {
                Console.Write("> ");
                args = Console.ReadLine().Split(' ');
                logger?.Log(args);
                var types = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.GetCustomAttribute<VerbAttribute>() != null).ToArray();


                //Parser.Default.ParseArguments(args, types).WithParsed(Run);

                Parser.Default.ParseArguments<Commands.Connect, Commands.List, Commands.Get, Commands.Disconnect, Commands.Quit,
                    Commands.Put, Commands.CreateDirectory, Commands.Delete, Commands.Permissions, Commands.Copy, Commands.Save,
                    Commands.ChangeDirectory, Commands.Rename>(args).MapResult(
                (Commands.Connect opts) => Connection.Connect(ref client, ref logger, opts, ref path),
                (Commands.List opts) => Get.List(ref client, opts, in path, args),
                (Commands.Get opts) => Get.File(ref client, opts),
                (Commands.Disconnect opts) => Connection.Disconnect(ref client, ref logger),
                (Commands.Quit opts) => Connection.Exit(),
                (Commands.Put opts) => Put.File(ref client, opts, in path),
                (Commands.CreateDirectory opts) => Put.Create(ref client, opts),
                (Commands.Delete opts) => Modify.Delete(ref client, opts),
                (Commands.Permissions opts) => Modify.Permissions(ref client, opts),
                (Commands.Copy opts) => Put.Copy(ref client, opts),
                (Commands.Save opts) => Connection.Save(ref client),
                (Commands.ChangeDirectory opts) => Get.ChangeDirectory(in client, ref path, in args),
                (Commands.Rename opts) => Modify.Rename(client, opts, path), errs => 1);
                
            }
        }
    }
}
