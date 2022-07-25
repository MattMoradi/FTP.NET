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

        [Verb("ls", HelpText = "List directories and files")]
        public class List
        {
            // this is just one way of separating between remote and local. could also be path based instead

            [Option('l', "local", Required = false, HelpText = "List directories & files on local machine")]
            public string? Local { get; set; }

            [Option('r', "remote", Required = false, HelpText = "List directories & files on remote servers")]
            public string? Remote { get; set; }
        }

        [Verb("get", HelpText = "Get files from remote server")]
        public class Get
        {
            [Value(0, MetaName = "file", HelpText = "File to get from remote server")]
            public string? Path { get; set; }

            [Option('m', "multiple", Required = false, HelpText = "Get multiple files from remote")]
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

        [Verb("perm", HelpText = "Make a new directory")]
        public class Permissions
        {
            // research flag parameters
        }

        [Verb("cp", HelpText = "Copy directories on remote server")]
        public class Copy
        {
            [Value(0, MetaName = "directory", HelpText = "Directory to be copied [source destination]")]
            public IEnumerable<string>? Directories { get; set; }
        }

        [Verb("save", HelpText = "Save connection information")]
        internal class Save { }

        [Verb("rename", HelpText = "Save connection information")]
        public class Rename
        {
            [Value(0, MetaName = "default", HelpText = "File to rename on remote server (default)")]
            public string Name { get; set; }

            [Option('l', "local", Required = false, HelpText = "Rename file on local machine")]
            public string? Local { get; set; }

            [Option('r', "remote", Required = false, HelpText = "Rename file on remote server")]
            public string? Remote { get; set; }
        }
    }
}
