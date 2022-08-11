using FakeItEasy;
using FluentFTP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using static Client.Program;

namespace Client.Tests
{
    public class ModifyTests
    {
        FtpClient testClient = A.Fake<FtpClient>();

        [Fact]
        public void Rename_LocalFlagSuccess()
        {
            Directory.CreateDirectory(@"UnitTesting");

            var fstream = File.Create(@"UnitTesting\TestFile.txt");

            fstream.Close();

            var delete = Directory.GetCurrentDirectory();

            var fp = new FilePath() { Local = $@"{Directory.GetCurrentDirectory()}\UnitTesting\" };

            // a little backwards due to needing to support both rename <oldname> <newname> and rename -l <oldName> <newname>
            var renCmd = A.Fake<Commands.Rename>(); 
            renCmd.LocalName = "TestFile.txt";
            renCmd.OldName = "NewTestFile.txt";

            Assert.Equal(0, Modify.Rename(ref testClient, renCmd, fp));

            Assert.True(File.Exists(@"UnitTesting\NewTestFile.txt"));

            File.Delete(@"UnitTesting\NewTestFile.txt");

            Directory.Delete(@"UnitTesting");
        }

        [Fact]
        public void Rename_InvalidOldName()
        {
            // non-client
            var cmd = A.Fake<Commands.Rename>();
            cmd.OldName = @"ThisTesttxt";
            cmd.NewName = @"Conntxt";

            var dir = new FilePath() { Remote = @"\UnitTest\" };

            Assert.Equal(-1, Modify.Rename(ref testClient, cmd, dir));
        }

        [Fact]
        public void Rename_InvalidNewName()
        {
            // non client
            var cmd = A.Fake<Commands.Rename>();
            cmd.OldName = @"ThisTest.txt";
            cmd.NewName = @"Conntxt";
            
            var dir = new FilePath() { Remote = @"\UnitTest\" };
            
            Assert.Equal(-1, Modify.Rename(ref testClient, cmd, dir));
            
            testClient.Disconnect();
        }

        [Fact]
        public void Rename_HostNotSpecified()
        {
            // non client
            var cmd = A.Fake<Commands.Rename>();
            cmd.OldName = @"ThisTest.txt";
            cmd.NewName = @"Conntxt";

            var dir = new FilePath() { Remote = @"\UnitTest\" };

            Assert.Equal(-1, Modify.Rename(ref testClient, cmd, dir));

        }

        [Fact]
        public void Rename_InvalidDirectory()
        {
            // non-client
            var cmd = A.Fake<Commands.Rename>();
            cmd.OldName = @"ThisTest.txt";
            cmd.NewName = @"Conntxt";

            var dir = new FilePath() { Remote = @"\UnitTest.txt\" };

            Assert.Equal(-1, Modify.Rename(ref testClient, cmd, dir));
        }

        [Fact]
        public void Rename_InvalidLocalFlagNewName()
        {
            // Non-Client
            var cmd = A.Fake<Commands.Rename>();
            cmd.LocalName = @"ThisTest.txt";
            cmd.NewName = @"Conntxt";
            cmd.OldName = @"Conntxt";

            var dir = new FilePath() { Remote = @"\UnitTesttxt\" };

            Assert.Equal(-1, Modify.Rename(ref testClient, cmd, dir));
        }

        [Fact]
        public void Rename_InvalidLocalFileName()
        {
            // Non-Client
            var cmd = A.Fake<Commands.Rename>();
            cmd.LocalName = @"ThisTesttxt";
            cmd.NewName = @"Conn.txt";
            cmd.OldName = @"Conn.txt";

            var dir = new FilePath() { Remote = @"\UnitTesttxt\" };

            Assert.Equal(-1, Modify.Rename(ref testClient, cmd, dir));
        }

        [Fact]
        public void Rename_InvalidRemoteFlagNewName()
        {
            // non-client
            var cmd = A.Fake<Commands.Rename>();
            cmd.RemoteName = @"ThisTesttxt";
            cmd.NewName = @"Conn.txt";
            cmd.OldName = @"Conn.txt";

            var dir = new FilePath() { Remote = @"\UnitTesttxt\" };

            Assert.Equal(-1, Modify.Rename(ref testClient, cmd, dir));
        }
    }
}
