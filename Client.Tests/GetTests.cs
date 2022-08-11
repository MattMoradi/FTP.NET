using Xunit;
using FluentFTP;
using FakeItEasy;

namespace Client.Tests
{
    public class GetTests
    {
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
        public void Get_Change_Local_Directory_Success()
        {
            Program.FilePath path = new();
            path.SetInitalPaths("./", "/");
            int index = 0;
            string[] args = { "cd", "-l", "Users" };
            Assert.Equal(0, Get.ChangeLocalDirectory(ref path, in args, index));
            Assert.Equal("./Users/", path.Local);
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
