
﻿using FakeItEasy;
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
        public void Rename_InvalidOldName()
        {
            // non-client
            var cmd = A.Fake<Commands.Rename>();
            cmd.OldName = @"ThisTesttxt";
            cmd.NewName = @"Conntxt";

            var dir = new FilePath() { Remote = @"\UnitTest\" };

            Assert.Equal(-1, Modify.Rename(ref client, cmd, dir));
        }

        [Fact]
        public void Rename_InvalidNewName()
        {
            // non client
            var cmd = A.Fake<Commands.Rename>();
            cmd.OldName = @"ThisTest.txt";
            cmd.NewName = @"Conntxt";
            
            var dir = new FilePath() { Remote = @"\UnitTest\" };
            
            Assert.Equal(-1, Modify.Rename(ref client, cmd, dir));
        }

        [Fact]
        public void Rename_HostNotSpecified()
        {
            // non client
            var cmd = A.Fake<Commands.Rename>();
            cmd.OldName = @"ThisTest.txt";
            cmd.NewName = @"Conntxt";

            var dir = new FilePath() { Remote = @"\UnitTest\" };

            Assert.Equal(-1, Modify.Rename(ref client, cmd, dir));

        }

        [Fact]
        public void Rename_InvalidDirectory()
        {
            // non-client
            var cmd = A.Fake<Commands.Rename>();
            cmd.OldName = @"ThisTest.txt";
            cmd.NewName = @"Conntxt";

            var dir = new FilePath() { Remote = @"\UnitTest.txt\" };

            Assert.Equal(-1, Modify.Rename(ref client, cmd, dir));
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

            Assert.Equal(-1, Modify.Rename(ref client, cmd, dir));
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

            Assert.Equal(-1, Modify.Rename(ref client, cmd, dir));
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

            Assert.Equal(-1, Modify.Rename(ref client, cmd, dir));
        }

		FtpClient client = A.Fake<FtpClient>();
        Commands.Put commands = A.Fake<Commands.Put>();
        Program.FilePath path = new();

        [Fact]
        public void Not_Authenticated_Should_Fail()
        {
            path.SetInitalPaths("./", "/");
            string[] opts = { "test" };
            commands.Files = opts;

            Assert.Equal(-1, Put.File(ref client, commands, in path));
        }

        [Fact]
        public void Delete_Remote_Directory_Client_Not_Authenticated()
        {
            FtpClient client = A.Fake<FtpClient>();
            Commands.Delete file = new();
            Program.FilePath path = new();
            Assert.Equal(-1, Modify.Delete(ref client, file, in path));
        }
    }
}
