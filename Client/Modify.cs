using System;
using FluentFTP;

namespace Client
{
    public static class Modify
    {
        public static int Delete(ref FtpClient client, Commands.Delete file)
        {
            return 0;
        }

        public static int Permissions(ref FtpClient client, Commands.Permissions file)
        {
            return Permissions(client);
        }

        /// <summary>
        /// Changes the permissions of a specified remote file.
        /// </summary>
        /// <param name="client">Ftp server endpoint.</param>
        /// <returns>0 if successful else -1.</returns>
        /// <exception cref="Exception">Library methods could fail.</exception>
        public static int Permissions(FtpClient client)
        {
            int result = -1;
            var fPath = string.Empty;
            try
            {
                // Permission level defaults to read only
                int permLevel = 4;
                Console.WriteLine("Enter File Path: ");
                fPath = Console.ReadLine() ?? String.Empty;

                Console.WriteLine("Enter Permission in CHMOD Format: ");
                int.TryParse(Console.ReadLine() ?? String.Empty, out permLevel);

                // Invoke library method to execute permission change.
                client.SetFilePermissions(fPath, permLevel);
                result = 0;
            }
            catch (Exception exc)
            {
                throw new Exception($"[Client.Modify.Permissions(client)] Failed to Change Permissions of {fPath}. Auto Exception Message: {exc.Message}");
            }
            return result;
        }

        public static int Rename(ref FtpClient client, Commands.Rename file)
        {
            return Rename(client);
        }

        /// <summary>
        /// Will rename a file on local or remote directory depending on where it exists.
        /// </summary>
        /// <param name="client">ftpClient on which potential file to rename exists.</param>
        /// <returns>0 if the file was renamed successfully, -1 if it failed.</returns>
        /// <exception cref="Exception">Could be library call critical errors or System.IO.Directory failure.</exception>
        public static int Rename(FtpClient client)
        {
            int result = -1;
            string oldName = string.Empty;
            string newName = string.Empty;
            try
            {
                Console.WriteLine("Enter Old File Path: ");
                oldName = Console.ReadLine() ?? String.Empty;

                Console.WriteLine("Enter File Path Ending With Rename Value: ");
                newName = Console.ReadLine() ?? String.Empty;

                // Check if the file exists on the remote server.
                if (client.FileExists(oldName))
                {
                    // rename remote file
                    result = client.MoveFile(oldName, newName) ? 0 : -1;
                }
                // check if local file exists with given name
                else if (Directory.Exists(oldName))
                {
                    Directory.Move(oldName, newName);
                    result = 0;
                }
            }
            catch(Exception exc)
            {
                throw new Exception($"[Client.Modify.Rename(client, file, oNm, nNm)] Failed to rename file {oldName}. Auto Exception Msg: {exc.Message}");
            }

            return result;
        }    
    }
}
