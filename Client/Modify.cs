using System;
using FluentFTP;

namespace Client
{
    public static class Modify
    {
        public static int Delete(ref FtpClient client, Commands.Delete file)
        {
            if (client.IsAuthenticated)
            {
                bool isDir = client.DirectoryExists(file.File);
                bool isFile = client.FileExists(file.File);

                if (isDir)
                {
                    client.DeleteDirectory(file.File);
                }
                else if (isFile)
                {
                    client.DeleteFile(file.File);
                }
                else
                {
                    Console.WriteLine("ERROR: The specified file/directory does not exist\n");
                }

                return 0;
            }
            else
            {
                Console.WriteLine("ERROR: Not connected to remote server!\n");
                return -1;
            }
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
                if (!client.IsConnected)
                {
                    Console.WriteLine("Host not specified, Try the \"Connect\" command.");
                    return -1;
                }

                if (string.IsNullOrEmpty(file.FilePath))
                {
                    Console.WriteLine("File path cannot be empty, enter a valid file name");
                    return -1;
                }

                if (!file.FilePath.Contains('.') || file.FilePath.Last().Equals("."))
                {
                    Console.WriteLine($"Incorrect File Name: {file.FilePath}");
                    return -1;
                }

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
                if (!string.IsNullOrEmpty(file.RemoteName) && client.IsConnected && file.RenameValue.Contains('.'))
                {
                    if (file.RemoteName.Contains('.') && !file.RemoteName.Last().Equals('.'))
                    {
                        Console.WriteLine($"Executing Remote Rename Of: {file.RenameValue}...");
                        // rename remote file
                        result = client.MoveFile(file.RemoteName, file.RenameValue) ? 0 : -1;
                    }
                    else
                    {
                        Console.WriteLine($"Incorrect file name: {file.RemoteName}. Try again.");
                        return -1;
                    }
                }
                else if (string.IsNullOrEmpty(file.LocalName) && !client.IsConnected)
                {
                    Console.WriteLine("Host not specified. Try the \"Connect\" command.");
                    return -1;
                }
                // check if local file exists with given name
                else if (!string.IsNullOrEmpty(file.LocalName))
                {
                    if (!file.RenameValue.Contains('.') || file.RenameValue.Last().Equals('.'))
                    {
                        Console.WriteLine($"Incorrect Rename Value {file.RenameValue}. Missing file extension.");
                        return -1;
                    }
                    if (file.LocalName.Contains('.') && !file.LocalName.Last().Equals('.'))
                    {
                        // moves files and directories
                        Directory.Move(file.LocalName, file.RenameValue);
                        result = 0;
                    }
                    else
                    {
                        Console.WriteLine($"Incorrect file name: {file.LocalName}, try again");
                        return -1;
                    }
                }
                else
                {
                    Console.WriteLine("Error, No File Specified Try Again.");
                    return -1;
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
