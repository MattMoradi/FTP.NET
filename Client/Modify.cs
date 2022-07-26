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

        /// <summary>
        /// Changes the permissions of a specified remote file.
        /// </summary>
        /// <param name="client">Ftp server endpoint.</param>
        /// <param name="file">Permission levels and file path</param>
        /// <returns>0 if successful else -1.</returns>
        /// <remarks>Only works on unix platforms accepting CHMOD operations</remarks>
        public static int Permissions(ref FtpClient client, Commands.Permissions file)
        {
            
            int result = -1;
            try
            {
                // Invoke library method to execute permission change.
                client.SetFilePermissions(file.FilePath, ((file.Owner * 100) + (file.Group * 10) + file.Others));
                
                result = 0;
            }
            catch(FtpCommandException ftpExc)
            {
                Console.WriteLine($"Ftp Client Failed to execute rename: {ftpExc.Message}");
            }
            catch(ArgumentException aExc)
            {
                Console.WriteLine($"Incorrect Parameters: Auto Exception Message{aExc.Message}");
            }
            catch (Exception exc)
            {
               Console.WriteLine($"Failed to Change Permissions of {file.FilePath}. Auto Exception Message: {exc.Message}");
            }
            return result;
        }

        /// <summary>
        /// Renames a local or remote file based on the supplied commands.
        /// </summary>
        /// <param name="client">ftpClient on which potential file to rename exists.</param>
        /// <param name="file">Rename command that specifies to rename local or remtoe file and file names to use.</param>
        /// <returns>0 if the file was renamed successfully, -1 if it failed.</returns>
        public static int Rename(ref FtpClient client, Commands.Rename file)
        {
            int result = -1;
            try
            {
                // Check if the file exists on the remote server.
                if (!string.IsNullOrEmpty(file.RemoteName) && client.IsConnected)
                {
                    // rename remote file
                    result = client.MoveFile(file.RemoteName, file.RenameValue) ? 0 : -1;
                }
                // check if local file exists with given name
                else if (!string.IsNullOrEmpty(file.LocalName))
                {
                    Directory.Move(file.LocalName, file.RenameValue);
                    result = 0;
                }

                // prevents remote rename with forgottent client connection from being "silent"
                if (!client.IsConnected && !string.IsNullOrEmpty(file.RemoteName))
                {
                    Console.WriteLine("Host Not Specified, Try the \"Connect\" Command.");
                }
            }
            catch(ArgumentException aExc)
            {
                Console.WriteLine($"Rename Failed, Incorrect Parameters: {aExc}");
            }
            catch(DirectoryNotFoundException dnfExc)
            {
                Console.WriteLine($"Directory Not Found. Auto Exception Message {dnfExc.Message}");
            }
            catch(UnauthorizedAccessException uaaExc)
            {
                Console.WriteLine($"You Need More Permissions. Auto Exception Message {uaaExc.Message}");
            }
            catch(Exception exc)
            {
                Console.WriteLine($"Failed to rename: {(string.IsNullOrEmpty(file.RemoteName) ? file.LocalName : file.RemoteName)}. Auto Exception Msg: {exc.Message}");
            }

            if (result == 0)
            {
                Console.WriteLine("Rename Successfull!");
            }


            return result;
        }    
    }
}
