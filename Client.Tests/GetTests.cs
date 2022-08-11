using FakeItEasy;
using FluentFTP;
using System.Collections.Generic;
using System.IO;
using Xunit;
using static Client.Program;


namespace Client.Tests
{
    public class GetTests
    {
        FtpClient testClient = A.Fake<FtpClient>();

        [Fact]
        public void Get_File_No_Connection()
        {
            Commands.Get file = A.Fake<Commands.Get>();
            var dirs = new FilePath();
            Assert.Equal(-1, Get.File(ref testClient, file, dirs));
        }

        [Fact]
        public void GetMultiple_HostNotSpecified()
        {
            // non-client
             var remoteDirs = new List<string>() { "Build.txt" };

             var localDir = "ArbitraryDirectoryName";

             var dirs = new FilePath() { Remote = @"\UnitTest\", Local = @"\" };

             Assert.Equal(-1, Get.MultipleFiles(testClient, remoteDirs, localDir, dirs));
        }

        [Fact]
        public void GetMultiple_InvalidLocalDirectory()
        {
            var remoteDir = new List<string>() { "Build.txt" };

            var localDir = @"Arbitrary\Incorrect\Directory.txt";

            var dirs = new FilePath() { Local = @"\", Remote = @"\" };

            Assert.Equal(-1, Get.MultipleFiles(testClient, remoteDir, localDir, dirs));
        }

        [Fact]
        public void GetMultiple_InvalidRemoteFileNameAll()
        {
            var remoteDir = new List<string>() { "Buildtxt", "Hellotxt" };

            var localDir = @"Arbitrary\Directory";

            var dirs = new FilePath() { Local = @"\", Remote = @"\" };

            Assert.Equal(-1, Get.MultipleFiles(testClient, remoteDir, localDir, dirs));
        }

        [Fact]
        public void GetDirectory_InvalidDirectory()
        {
            var remoteDir = "RandomDirectory.txt";

            var localDir = string.Empty;

            var dirs = new FilePath() { Local = @"\", Remote = @"\" };

            Assert.Equal(-1, Get.RemoteDirectory(testClient, remoteDir, dirs, localDir));
        }

        [Fact]
        public void GetDirectory_InvalidLocalDirectory()
        {
            var remoteDir = @"\Random\Directory";

            var localDir = @"\Incorrect\Local\Directory.txt";

            var dirs = new FilePath() { Local = @"\", Remote = @"\" };

            Assert.Equal(-1, Get.RemoteDirectory(testClient, remoteDir, dirs, localDir));
        }

        [Fact]
        public void GetDirectory_HostNotSpecidfied()
        {
            var remoteDir = @"\Random\Directory";

            var localDir = @"\Incorrect\Local\Directory";

            var dirs = new FilePath() { Local = @"\", Remote = @"\" };

            Assert.Equal(-1, Get.RemoteDirectory(testClient, remoteDir, dirs, localDir));
        }
        
        [Fact]
        public void Get_List_Remote_Directory_Client_Not_Authenticated()
        {
            FtpClient client = A.Fake<FtpClient>();
            string[] args = { "ls", "-r" };
            Program.FilePath path = new();
            path.SetInitalPaths("/", "/");
            Assert.Equal(-1, Get.List(ref client, in path, args));
        }

        [Fact]
        public void Get_Go_To_Prev_Local_Directory_Failed_Since_At_Root_Directory()
        {
            Program.FilePath path = new();
            path.SetInitalPaths("/", "/DirOnRemote");
            Assert.Equal(-1, Get.GoToPrevLocalDirectory(ref path));
        }

        [Fact]
        public void Get_Go_To_Prev_Local_Directory_Successs()
        {
            Program.FilePath path = new();
            path.SetInitalPaths("/tempDir/", "/");
            Assert.Equal(0, Get.GoToPrevLocalDirectory(ref path));
            Assert.Equal("/", path.Local);
        }

        [Fact]
        public void Get_Change_Local_Directory_Dir_Does_Not_Exist()
        {
            Program.FilePath path = new();
            path.SetInitalPaths("./", "/");
            int index = 0;
            string[] args = { "cd", "-l", "NonExistingFolder" };
            Assert.Equal(-1, Get.ChangeLocalDirectory(ref path, in args, index));
            Assert.Equal("./", path.Local);
        }
        [Fact]
        public void Get_Is_At_Root_Directory_True()
        {
            Program.FilePath path = new();
            path.SetInitalPaths("./", "/");
            Assert.True(Get.IsAtRootDirectory(path.Local), "Local directory is not at the root!");
            Assert.True(Get.IsAtRootDirectory(path.Remote), "Remote directory is not at root!");
        }
        [Fact]
        public void Get_Is_At_Root_Directory_False()
        {
            Program.FilePath path = new();
            path.SetInitalPaths("./First/Second", "/First/");
            Assert.False(Get.IsAtRootDirectory(path.Local), "Local directory is not at the root!");
            Assert.False(Get.IsAtRootDirectory(path.Remote), "Remote directory is not at root!");
        }
    }
}
