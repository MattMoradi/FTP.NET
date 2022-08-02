using System;
using System.IO;
using FluentFTP;
namespace Client
{
    public static class Put
    {
        public static int File(ref FtpClient client, Commands.Put file)
        {
            Console.WriteLine("file: " + file.File);

            if (file.Files.Count() > 1)
                return MultipleFiles(file.Files);
            return 0;
        }

        public static int MultipleFiles(IEnumerable<string> files)
        {
            foreach (string file in files)
                Console.WriteLine(file); // remove this

            return 0;
        }

        public static int Create(ref FtpClient client, Commands.CreateDirectory directory)
        {
            if (client.IsAuthenticated)
            {
                if (client.DirectoryExists(directory.Name))
                    Console.WriteLine("ERROR: Directory already exists\n");
                else
                    client.CreateDirectory(directory.Name);

                return 0;
            }
            else
            {
                Console.WriteLine("ERROR: Not connected to remote server!\n");
                return -1;
            }
        }

        public static int Copy(ref FtpClient client, Commands.Copy file)
        {
            string sourcedir;
            string targetdir;

            //Check whether there are two arguments, if there are not, return error message
            if (file.Directories.Count() != 2)
            {
                Console.WriteLine("You must provide two arguments for the copy directory command.");
                return -1;
            }
            else
            {
                sourcedir = file.Directories.First();
                targetdir = file.Directories.Last();
            }

            //Check whether the source dir exists. If it does not, return an error message
            if (!client.DirectoryExists(file.Directories.First()))
            {
                Console.WriteLine("The indicated directory does not exist.");
                return -1;
            }

            //WE SHOULD HAVE A CHECK HERE TO ENSURE THAT THERE ARE NOT 'BAD CHARS' IN THE DIR STRING
            if (!Path.IsPathFullyQualified(targetdir))
            {
                Console.WriteLine("You have provided an invalid target directory string.");
                return -1;
            }

            //Check whether the destination dir exists. If it does ask whether they would like to delete the contents and replace w/ the source dir and it's contents
            if (client.DirectoryExists(file.Directories.Last()))
            {
                Console.WriteLine("The destination directory already exists. Would you like to overwrite it? (Y/Yes)");

                var response = Console.ReadLine();

                if (String.IsNullOrEmpty(response))
                {
                    response = "No";
                }

                if ((String.Equals(response, "Y")) || (String.Equals(response, "Yes")) || (String.Equals(response, "YES")))
                {
                    client.DeleteDirectory(file.Directories.Last());
                }
                else
                {
                    return 0;
                }

            }
            //ALL CHECKS SHOULD BE PERFORMED AT THIS POINT SO OK TO GO FORWARD WITH THE DL/UL/DEL

            string localtempdir = "C:\\fluentftp_tempdir";

            client.DownloadDirectory(localtempdir, sourcedir, FtpFolderSyncMode.Update);

            string[] dirs = Directory.GetDirectories(localtempdir);

            client.CreateDirectory(targetdir);

            foreach (string dir in dirs)
            {
                client.UploadDirectory(dir, targetdir, FtpFolderSyncMode.Update);
            }

            string[] files = Directory.GetFiles(localtempdir);

            foreach (string f in files)
            {
                client.UploadFile(f, targetdir);
            }

            Directory.Delete(localtempdir, true);

            return 0;
        }
    }
}
