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

        [Fact]
        public void Rename_LocalFlagSuccess()
        {
            Directory.CreateDirectory(@"C:\UnitTesting");

            Directory.CreateDirectory(@"C:\UnitTesting\TestFile.txt");

            var fp = new FilePath() { Local = @"C:\UnitTesting\" };

            var blankClient = new FtpClient();

            // a little backwards due to needing to support both rename <oldname> <newname> and rename -l <oldName> <newname>
            var files = new Commands.Rename() { LocalName = "TestFile.txt", OldName = "NewTestFile.txt"  };

            // EX: var cmd = new Commands.Rename() { LocalName = "OldName.txt", OldName = "NewName.txt"  };

            Assert.Equal(0, Modify.Rename(blankClient, files, fp));

            Assert.True(Directory.Exists(@"C:\UnitTesting\NewTestFile.txt"));

            Directory.Delete(@"C:\UnitTesting\NewTestFile.txt");

            Directory.Delete(@"C:\UnitTesting");
        }

        [Fact]
        public void Rename_LocalNoFlagSuccess()
        {
            Directory.CreateDirectory(@"C:\UnitTesting");

            Directory.CreateDirectory(@"C:\UnitTesting\TestFile.txt");

            var fp = new FilePath() { Local = @"C:\UnitTesting\" };

            var blankClient = new FtpClient();

            var files = new Commands.Rename() { OldName = "TestFile.txt", NewName = "NewTestFile.txt" };

            Assert.Equal(0, Modify.Rename(blankClient, files, fp));

            Assert.True(Directory.Exists(@"C:\UnitTesting\NewTestFile.txt"));

            Directory.Delete(@"C:\UnitTesting\NewTestFile.txt");

            Directory.Delete(@"C:\UnitTesting");
        }

        [Fact]
        public void Rename_InvalidOldName()
        {
            // non-client
            var testClient = new FtpClient();
            
            var cmd = new Commands.Rename() { OldName = @"ThisTesttxt", NewName = @"Conntxt" };

            var dir = new FilePath() { Remote = @"\UnitTest\" };

            Assert.Equal(-1, Modify.Rename(testClient, cmd, dir));
        }

        [Fact]
        public void Rename_InvalidNewName()
        {
            // non client
            var testClient = new FtpClient();

            var cmd = new Commands.Rename() { OldName = @"ThisTest.txt", NewName = @"Conntxt" };
            
            var dir = new FilePath() { Remote = @"\UnitTest\" };
            
            Assert.Equal(-1, Modify.Rename(testClient, cmd, dir));
            
            testClient.Disconnect();
        }

        [Fact]
        public void Rename_HostNotSpecified()
        {
            // non client
            var testClient = new FtpClient();
            
            var cmd = new Commands.Rename() { OldName = @"ThisTest.txt", NewName = @"Conntxt" };

            var dir = new FilePath() { Remote = @"\UnitTest\" };

            Assert.Equal(-1, Modify.Rename(testClient, cmd, dir));

        }

        [Fact]
        public void Rename_InvalidDirectory()
        {
            // non-client
            var testClient = new FtpClient();
            
            var cmd = new Commands.Rename() { OldName = @"ThisTest.txt", NewName = @"Conntxt" };

            var dir = new FilePath() { Remote = @"\UnitTest.txt\" };

            Assert.Equal(-1, Modify.Rename(testClient, cmd, dir));
        }

        [Fact]
        public void Rename_InvalidLocalFlagNewName()
        {
            // Non-Client
            var testClient = new FtpClient();
            
            var cmd = new Commands.Rename() { LocalName = @"ThisTest.txt", NewName = @"Conntxt", OldName = @"Conntxt" };

            var dir = new FilePath() { Remote = @"\UnitTesttxt\" };

            Assert.Equal(-1, Modify.Rename(testClient, cmd, dir));
        }

        [Fact]
        public void Rename_InvalidLocalFileName()
        {
            // Non-Client
            var testClient = new FtpClient();
                var cmd = new Commands.Rename() { LocalName = @"ThisTesttxt", NewName = @"Conn.txt", OldName = @"Conn.txt" };

                var dir = new FilePath() { Remote = @"\UnitTesttxt\" };

                Assert.Equal(-1, Modify.Rename(testClient, cmd, dir));
        }

        [Fact]
        public void Rename_InvalidRemoteFlagNewName()
        {
            // non-client
            var testClient = new FtpClient();

            var cmd = new Commands.Rename() { RemoteName = @"ThisTesttxt", NewName = @"Conn.txt", OldName = @"Conn.txt" };

            var dir = new FilePath() { Remote = @"\UnitTesttxt\" };

            Assert.Equal(-1, Modify.Rename(testClient, cmd, dir));

        }


    }
}
