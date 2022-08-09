using System;
using FluentFTP;
using static Client.Program;

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
            Console.WriteLine("Note: Server Must Support CHMOD permission operations.");
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
        public static int Rename(FtpClient client, Commands.Rename file, FilePath dirs)
        {
            int result = -1;
            var tempLoc = Directory.GetCurrentDirectory() + @"\Data";
            var tempRem = dirs.Remote;
            
            try
            {
                int cont = RenameChecks(file, client.IsAuthenticated ? true : false);

                // RenameChecks method handles messages to screen and all incorrect arguments
                // Also indicates local vs remote vs dual rename.
                if (cont == -1) { return -1; }

                if (cont == 0 || cont == 99)
                {
                    
                    if (client.IsAuthenticated)
                    {
                        // avoids renaming to a name that already exists
                        if (!client.FileExists(dirs.Remote + file.NewName))
                        {
                            // rename remote file
                            if (client.MoveFile(dirs.Remote + file.RemoteName, dirs.Remote + file.NewName))
                            {
                                Console.WriteLine("Remote Rename Successfull!");
                                result = 0;
                            }
                            else
                            {
                                // if rename fails, checks that the supplied file exists.
                                if (!client.FileExists(dirs.Remote + file.OldName))
                                {
                                    Console.WriteLine($"Error: file: {file.OldName} does not exist. try again.");
                                }
                                Console.WriteLine("Error: server failed to rename file. make sure file to rename exists");
                                Console.WriteLine(@"Expected: rename <OldName> <NewName>");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Error: cannot rename file to {file.NewName}, file of that name already exists. try again");
                            return -1;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error: Host not specified. Try the \"Connect\" command for remote rename");
                    }
                }
                if (cont == 1 || cont == 99)
                {
                    if (!Directory.Exists(dirs.Local + file.NewName))
                    {
                        //Directory.Move is used for files
                        Directory.Move(dirs.Local + file.LocalName, dirs.Local + file.NewName);
                        result = 0;
                    }
                    else
                    {
                        Console.WriteLine($"Error: cannot rename file to {file.OldName}, file of that name already exists. try again");
                        return -1;
                    }
                }
            }
            catch(ArgumentException aExc)
            {
                Console.WriteLine($"Error: rename failed, incorrect parameters: {aExc}");
                Console.WriteLine(@"Expected: rename <OldName> <NewName>");
            }
            catch(DirectoryNotFoundException dnfExc)
            {
                Console.WriteLine($"Error: directory not found. auto exception message: {dnfExc.Message}");
                Console.WriteLine(@"Expected: rename <OldName> <NewName>");
            }
            catch(UnauthorizedAccessException uaaExc)
            {
                Console.WriteLine($"Error: permission denied. auto exception message: {uaaExc.Message}");
            }
            catch(Exception exc)
            {
                Console.WriteLine($"Error: failed to rename: {(string.IsNullOrEmpty(file.RemoteName) ? file.LocalName : file.RemoteName)}. Auto Exception Msg: {exc.Message}");
                Console.WriteLine(@"Expected: rename <OldName> <NewName>");
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
        private static int RenameChecks(Commands.Rename files, bool isClient)
        {
            // base case checks
            if (string.IsNullOrEmpty(files.LocalName) && string.IsNullOrEmpty(files.NewName) && string.IsNullOrEmpty(files.RemoteName) && string.IsNullOrEmpty(files.OldName))
            {
                Console.WriteLine($"Error: missing parameters. Need a local OR remote file to rename AND the new file name");
                Console.WriteLine(@"Expected: rename <OldName> <NewName>");
                return -1;
            }
            if (string.IsNullOrEmpty(files.LocalName) && string.IsNullOrEmpty(files.RemoteName) && string.IsNullOrEmpty(files.OldName) && !string.IsNullOrEmpty(files.NewName))
            {
                if (!CheckExtension(files.NewName, "AND"))
                {
                    Console.WriteLine($"Error: Incorrect Rename Value: {files.NewName}. missing extension.");
                }

                Console.WriteLine($"Error: local and/or remote file name missing");
                Console.WriteLine(@"Expected: rename <OldName> <NewName>");
                return -1;
            }

            // Look for no flag case
            if (!string.IsNullOrEmpty(files.NewName) && !string.IsNullOrEmpty(files.OldName))
            {
                if (CheckExtension(files.NewName, "AND") && CheckExtension(files.OldName, "AND"))
                {
                    if (isClient)
                    {
                        files.RemoteName = files.OldName;
                        return 0;
                    }
                    else
                    {
                        files.LocalName = files.OldName;
                        return 1;
                    }
                }
                else
                {
                    if (CheckExtension(files.NewName, "OR"))
                    {
                        Console.WriteLine($"Error: Incorrect New File Name: {files.NewName}. missing extension.");
                    }
                    if (CheckExtension(files.OldName, "OR"))
                    {
                        Console.WriteLine($"Error: Incorrect Old File Name: {files.OldName}. missing extension");
                    }
                    Console.WriteLine(@"Expected: <FileName.ext>");
                    return -1;
                }
            }

            // check for remote rename case and proper arguments
            if (string.IsNullOrEmpty(files.LocalName) && !string.IsNullOrEmpty(files.RemoteName) && !string.IsNullOrEmpty(files.OldName))
            {
                // Indicates remote rename.
                // recent feedback to take no flags changed functionality. when flags are used it interferes with
                // no flag usage. setting newname to oldname is actually setting the new name argument with the users
                // new name argumment.
                files.NewName = files.OldName;

                if (CheckExtension(files.RemoteName, "AND") && CheckExtension(files.OldName, "AND"))
                {
                    return 0;
                }
                else if (CheckExtension(files.RemoteName, "OR")) 
                {
                    Console.WriteLine($"Error: Incorrect Remote File Name: {files.RemoteName}, missing extension");
                    Console.WriteLine(@"Expected: <FileName.ext>");
                    return -1;
                }
                else if (CheckExtension(files.OldName, "OR"))
                {
                    Console.WriteLine($"Error: Incorrect RenameValue: {files.NewName}, missing extension.");
                    Console.WriteLine(@"Expected: <FileName.ext>");
                    return -1;
                }
            }
            //Checks for local rename case and proper arguments
            else if (string.IsNullOrEmpty(files.RemoteName) && !string.IsNullOrEmpty(files.OldName) && !string.IsNullOrEmpty(files.LocalName))
            {
                // local rename case
                // recent feedback to take no flags changed functionality. when flags are used it interferes with
                // no flag usage. setting newname to oldname is actually setting the new name argument with the users
                // new name argumment.
                files.NewName = files.OldName;

                if (CheckExtension(files.LocalName, "AND") && CheckExtension(files.OldName, "AND"))
                {
                    return 1;
                }
                else if (CheckExtension(files.LocalName, "OR"))
                {
                    Console.WriteLine($"Invalid Local File Name: {files.LocalName}. Incorrect Extension.");
                    Console.WriteLine(@"Expected: <FileName.ext>");
                    return -1;
                }
                else if (CheckExtension(files.OldName, "OR"))
                {
                    Console.WriteLine($"Incorrect Rename Value: {files.OldName}. Incorrect Extension.");
                    Console.WriteLine(@"Expected: <FileName.ext>");
                    return -1;
                }
            }
            //Check for dual rename case and proper arguments
            else if (!string.IsNullOrEmpty(files.RemoteName) && !string.IsNullOrEmpty(files.NewName) && !string.IsNullOrEmpty(files.LocalName))
            {
                if (CheckExtension(files.LocalName, "AND") && CheckExtension(files.NewName, "AND") && CheckExtension(files.RemoteName, "AND"))
                {
                    // local rename case
                    return 99;
                }
                else if (CheckExtension(files.LocalName, "OR"))
                {
                    Console.WriteLine($"Error: Invalid Local File Name: {files.LocalName}. Incorrect Extension.");
                    Console.WriteLine(@"Expected: <FileName.ext>");
                    return -1;
                }
                else if (CheckExtension(files.NewName, "OR"))
                {
                    Console.WriteLine($"Error: Incorrect Rename Value: {files.NewName}. Incorrect Extension.");
                    Console.WriteLine(@"Expected: <FileName.ext>");
                    return -1;
                }
                else if (CheckExtension(files.RemoteName, "OR"))
                {
                    Console.WriteLine($"Error: Incorrect Remote Value: {files.RemoteName}. Incorrect Extension.");
                    Console.WriteLine(@"Expected: <FileName.ext>");
                    return -1;
                }
            }

            Console.WriteLine("Error: Rename Failed. Unexpected Error Try Again...");
            Console.WriteLine("Your Input (need atleast an old name and new name):");
            Console.WriteLine($" Local File Name: {files.LocalName}");
            Console.WriteLine($"Remote File Name: {files.RemoteName}");
            Console.WriteLine($"   Old File Name: {files.OldName}");
            Console.WriteLine($"   New File Name: {files.NewName}");
            Console.WriteLine(@"Expected: rename <oldname> <newname>");
            return -1;
        }

        /// <summary>
        /// Checks for a file extension on the file name. Two cases: using "AND" as type will indicate if
        /// the value passed and has a valid extension. Using "OR" as type will indicate if the file name
        /// fails any incorrect file case.
        /// </summary>
        /// <param name="val">string value to check</param>
        /// <param name="type">"AND" Indicates correct file name on true, "OR" Indicates incorrect file path on true</param>
        /// <returns>True if "AND" type is used and the file name is valid, False if "AND" type is used and file name is invalid
        ///          True if "OR" type is used and the file name is invalid, False if "OR" type is used and file name is valid</returns>
        private static bool CheckExtension(string val, string type)
        {
            return type == "AND" ? (val.Contains(".") && !val.Last().Equals('.')) : (!val.Contains(".") || val.Last().Equals('.'));
        }
    }
}
