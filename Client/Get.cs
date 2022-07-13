using System;
using System.IO;
using FluentFTP;

namespace Client
{
    public static class Get
    {
        public static int File(ref FtpClient client, Commands.Get files)
        {
            Console.WriteLine("file: " + files.File);

            if (files.Files.Count() > 1)
            {
                Console.WriteLine($"Enter local File Path for download or press enter to skip and use current directory: ");
                var localDir = Console.ReadLine() ?? "";
                return MultipleFiles(client, files.Files, localDir);
            }
            return 0;
        }

        /// <summary>
        /// Uses FluentFtp library method to find files on a remote server as a query executed against the passed int
        /// ftpClient.
        /// </summary>
        /// <param name="ftpClient">Connection to remote ftp protocal server.</param>
        /// <param name="remoteDirs">List of file directories.</param>
        /// <param name="localDir">Local directory to save files.</param>
        /// <returns>Number of files found.</returns>
        /// <exception cref="Exception">Library method could fail.</exception>
        public static int MultipleFiles(FtpClient ftpClient, IEnumerable<string> remoteDirs, string localDir = "")
        {
            try
            {
                return ftpClient.DownloadFiles(string.IsNullOrEmpty(localDir) ? Environment.CurrentDirectory : localDir, remoteDirs);
            }
            catch(Exception exc)
            {
                throw new Exception($"[Client.Get.MultipleFiles(client, IE<rDir>, lDir)] Failed to retrieve multiple files. Auto Exception Msg: {exc.Message}");
            }
        }

        /// <summary>
        /// Uses the FluentFtp library to find and download a directory from the remote server to the local directory.
        /// </summary>
        /// <param name="ftpClient">Connection to remote ftp protocal server.</param>
        /// <param name="remoteDir">Remote directory to grab.</param>
        /// <param name="localDir">Local directory where items are saved.</param>
        /// <returns>Collection of the items downloaded.</returns>
        /// <exception cref="Exception">Library method could fail.</exception>
        public static IEnumerable<FtpResult> Directory(FtpClient ftpClient, string remoteDir, string localDir = "")
        {
            try
            {
                return ftpClient.DownloadDirectory(string.IsNullOrEmpty(localDir) ? Environment.CurrentDirectory : localDir, remoteDir, FtpFolderSyncMode.Update);
            }
            catch(Exception exc)
            {
                throw new Exception($"[Client.Get.Directory(client, rDir, lDir)]: Failed to retrieve directory: {remoteDir}. Auto Generated Exception Msg: {exc.Message}");
            }
        }

        public static int List(ref FtpClient client, Commands.List directory)
        {
            Console.WriteLine("list");
            return 0;
        }
    }
}
