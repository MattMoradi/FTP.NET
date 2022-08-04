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
                int cont = RenameChecks(file);

                // RenameChecks method handles messages to screen and all incorrect arguments
                // Also indicates local vs remote vs dual rename.
                if (cont == -1) { return -1; }

                if (cont == 0 || cont == 99)
                {
                    if (client.IsAuthenticated)
                    {
                        Console.WriteLine($"Executing Remote Rename Of: {file.RemoteName}...");
                        // rename remote file
                        if (client.MoveFile(file.RemoteName, file.RenameValue))
                        {
                            Console.WriteLine("Remote Rename Successfull!");
                        }
                        else
                        {
                            Console.WriteLine("Remote Rename, Server Failure");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Host not specified. Try the \"Connect\" command for remote rename");
                    }
                }
                if (cont == 1 || cont == 99)
                {
                    Console.WriteLine($"Executing Local Rename Of: {file.LocalName}...");
                    //Directory.Move is used for files
                    Directory.Move(file.LocalName, file.RenameValue);
                    result = 0;
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
                Console.WriteLine("Local Rename Successfull!");
            }


            return result;
        }
        

        /// <summary>
        /// Checks all arguments for proper syntax and indicates by the arguments given if 
        /// a local rename should execute vs remote rename vs dual remote and local rename.
        /// </summary>
        /// <param name="files">Rename arguments.</param>
        /// <returns>0 if remote rename and remote arguments are correct, 1 if local rename and local arguments
        /// are correct. 99 if dual local and remote rename and arguments are correct. -1 if any needed arguments
        /// are incorrect and a rename cannot be performed.</returns>
        private static int RenameChecks(Commands.Rename files)
        {
            // base case checks
            if (string.IsNullOrEmpty(files.LocalName) && string.IsNullOrEmpty(files.RenameValue) && string.IsNullOrEmpty(files.RemoteName))
            {
                Console.WriteLine($"Error, missing parameters. Need a local OR remote file to rename AND the new file name");
                Console.WriteLine(@"Expected: rename <rename\file\Path.ext> -l <local\file\path.ext> OR -r <remote\file\path.ext>");
                return -1;
            }
            if (string.IsNullOrEmpty(files.RenameValue))
            {
                Console.WriteLine("Error, Need a rename value.");
                Console.WriteLine(@"Expected: rename <rename\file\Path.ext> -l <local\file\path.ext> OR -r <remote\file\path.ext>");
                return -1;
            }
            if (string.IsNullOrEmpty(files.LocalName) && string.IsNullOrEmpty(files.RemoteName) && !string.IsNullOrEmpty(files.RenameValue))
            {
                if (!CheckExtension(files.RenameValue, "AND"))
                {
                    Console.WriteLine($"Incorrect Rename Value: {files.RenameValue}. Missing Extension.");
                }

                Console.WriteLine($"local and/or remote file name missing");
                Console.WriteLine(@"Expected: rename <rename\file\Path> -l <local\file\path> OR -r <remote\file\path>");
                return -1;

            }

            // check for remote rename case and proper arguments
            if (string.IsNullOrEmpty(files.LocalName) && !string.IsNullOrEmpty(files.RemoteName) && !string.IsNullOrEmpty(files.RenameValue))
            {
                if (CheckExtension(files.RemoteName, "AND") && CheckExtension(files.RenameValue, "AND"))
                {
                    // Indicates remote rename.
                    return 0;
                }
                else if (CheckExtension(files.RemoteName, "OR")) 
                {
                    Console.WriteLine($"Incorrect Remote File Name: {files.RemoteName}, Missing Extension");
                    Console.WriteLine(@"Example: rename SampleRemoteFlder1\NewFileName.ext -r \SampleRemoteFolder2\OldFileName.ext");
                    return -1;
                }
                else if (CheckExtension(files.RenameValue, "OR"))
                {
                    Console.WriteLine($"Incorrect RenameValue: {files.RenameValue}, Missing extension.");
                    Console.WriteLine(@"Example: rename C:\SampleFlder\NewFileName.ext --local \C:\SampleFolder\OldFileName.ext");

                    return -1;
                }
            }
            //Checks for local rename case and proper arguments
            else if (string.IsNullOrEmpty(files.RemoteName) && !string.IsNullOrEmpty(files.RenameValue) && !string.IsNullOrEmpty(files.LocalName))
            {
                if (CheckExtension(files.LocalName, "AND") && CheckExtension(files.RenameValue, "AND"))
                {
                    // local rename case
                    return 1;
                }
                else if (CheckExtension(files.LocalName, "OR"))
                {
                    Console.WriteLine($"Invalid Local File Name: {files.LocalName}. Incorrect Extension.");
                    Console.WriteLine(@"Example: rename C:\SampleFlder\NewFileName.ext -l \C:\SampleFolder\OldFileName.ext");
                    return -1;
                }
                else if (CheckExtension(files.RenameValue, "OR"))
                {
                    Console.WriteLine($"Incorrect Rename Value: {files.RenameValue}. Incorrect Extension.");
                    Console.WriteLine(@"Example: rename C:\SampleFlder\NewFileName.ext --local \C:\SampleFolder\OldFileName.ext");
                    return -1;
                }
            }
            //Check for dual rename case and proper arguments
            else if (!string.IsNullOrEmpty(files.RemoteName) && !string.IsNullOrEmpty(files.RenameValue) && !string.IsNullOrEmpty(files.LocalName))
            {
                if (CheckExtension(files.LocalName, "AND") && CheckExtension(files.RenameValue, "AND") && CheckExtension(files.RemoteName, "AND"))
                {
                    // local rename case
                    return 99;
                }
                else if (CheckExtension(files.LocalName, "OR"))
                {
                    Console.WriteLine($"Invalid Local File Name: {files.LocalName}. Incorrect Extension.");
                    Console.WriteLine(@"Example: rename C:\SampleFlder\NewFileName.ext -l \C:\SampleFolder\OldFileName.ext");
                    return -1;
                }
                else if (CheckExtension(files.RenameValue, "OR"))
                {
                    Console.WriteLine($"Incorrect Rename Value: {files.RenameValue}. Incorrect Extension.");
                    Console.WriteLine(@"Example: rename \SampleRemoteFlder\NewRemoteName.ext -r \SampleRemoteFolder\OldRemoteName.ext");
                    return -1;
                }
                else if (CheckExtension(files.RemoteName, "OR"))
                {
                    Console.WriteLine($"Incorrect Remote Value: {files.RemoteName}. Incorrect Extension.");
                    Console.WriteLine(@"Example: rename \SampleRemoteFlder\NewRemoteName.ext --remote \SampleRemoteFolder\OldRemoteName.ext");
                    return -1;
                }
            }

            Console.WriteLine("Rename Failed. Unexpected Error Try Again...");
            return -1;
        }

        /// <summary>
        /// Checks for a file extension on the file name. Two cases: using "AND" as type will indicate if
        /// the value passed in has a valid extension. Using "OR" as type will indicate if the file name
        /// fails any incorrect file case.
        /// </summary>
        /// <param name="val">string value to check</param>
        /// <param name="type">"AND" Atomic pass or fail, "OR" variable fail</param>
        /// <returns>true if file extension is correct on "AND" and "OR" case. false if file name fails one test 
        /// on "AND" or any test on "OR".</returns>
        private static bool CheckExtension(string val, string type)
        {
            return type == "AND" ? (val.Contains(".") && !val.Last().Equals('.')) : (!val.Contains(".") || val.Last().Equals('.'));
        }
    }
}
