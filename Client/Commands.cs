using CommandLine;

namespace Client
{
    public abstract class Commands
    {
        [Verb("connect", HelpText = "Connect to a remote ip address")]
        public class Connect
        {
            [Value(0, MetaName = "ip", HelpText = "Remote server IP address")]
            public string IP { get; set; }
        }

        [Verb("ls", HelpText = "Disconnect from the remote server")]
        public class List
        {
            // this is just one way of separating between remote and local. could also be path based instead

            [Option('l', "local", Required = false, HelpText = "List directories & files on local machine")]
            public string? Local { get; set; }

            [Option('r', "remote", Required = false, HelpText = "List directories & files on remote servers")]
            public string? Remote { get; set; }
        }

        [Verb("get", HelpText = "Get file(s) from remote server")]
        public class Get
        {
            [Value(0, MetaName = "file", HelpText = "File to get from remote server")]
            public string? File { get; set; }

            [Option('l', "local Path", HelpText = "Local file path to save files.")]
            public string LocalPath { get; set; } = string.Empty;

            [Option('d', "directory", HelpText = "Fully Qualified Directory Path.")]
            public string Directory { get; set; } = string.Empty;

            [Option('m', "multiple", Required = false, HelpText = "Get multiple files from remote.")]
            public IEnumerable<string>? Files { get; set; }
        }

        [Verb("disconnect", HelpText = "Disconnect from the remote server")]
        internal class Disconnect { }

        [Verb("quit", HelpText = "Close the FTP client")]
        internal class Quit{};

        [Verb("put", HelpText = "Upload files to remote server")]
        public class Put
        {
            [Value(0, MetaName = "file", HelpText = "File to upload to remote server")]
            public string? File { get; set; }

            [Option('m', "multiple", Required = false, HelpText = "Upload multiple files to remote")]
            public IEnumerable<string>? Files { get; set; }
        }

        [Verb("mkdir", HelpText = "Make a new directory")]
        public class CreateDirectory
        {
            [Value(0, MetaName = "file", HelpText = "Name of directory to create")]
            public string Name { get; set; }
        }

        [Verb("rm", HelpText = "Remove file from remote server")]
        public class Delete
        {
            [Value(0, MetaName = "file", HelpText = "File to be removed from remote server")]
            public string File { get; set; }

            // could potentially add flags for remote / local, but not required
        }

        [Verb("perm", HelpText = "Change A Files Permission Level (CHMOD Format)")]
        public class Permissions
        {
            [Value(0, MetaName = "file name", HelpText = "File that needs permissions changed")]
            public string FilePath { get; set; } = string.Empty;

            [Value(1, MetaName ="owner", HelpText = "Owner Permissions. (CHMOD Format)")]
            public int Owner { get; set; }

            [Value(2, MetaName = "group", HelpText = "Group Permissions. (CHMOD Format)")]
            public int Group { get; set; }

            [Value(3, MetaName = "others", HelpText = "Others Permissions. (CHMOD Format)")]
            public int Others { get; set; }
        }

        [Verb("cp", HelpText = "Copy directories on remote server")]
        public class Copy
        {
            [Value(0, MetaName = "directory", HelpText = "Directory to be copied [source destination]")]
            public IEnumerable<string>? Directories { get; set; }
        }

        [Verb("save", HelpText = "Save connection information")]
        internal class Save { }

        [Verb("rename", HelpText = "Rename file (Note: Endpoint Must Support CHMOD Operations else \"Unknown CHMOD...\" Error May Appear.)")]
        public class Rename
        {
            [Value(0, MetaName = "Rename value", HelpText = "File Rename Value.")]
            public string RenameValue { get; set; } = String.Empty;

            [Option('l', "local", Required = false, HelpText = "Local File Name To Change")]
            public string LocalName { get; set; } = String.Empty;

            [Option('r', "remote", Required = false, HelpText = "Remote File Name To Change. (Must Support CHMOD)")]
            public string? RemoteName { get; set; } = String.Empty;
        }
    }
}
