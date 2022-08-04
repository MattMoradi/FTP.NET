using Xunit;
using FluentFTP;
using System;

namespace Client.Tests
{
    public class GetTests
    {

        [Fact]
        public void Get_List_Remote_Directory_Success()
        {
            FtpClient client = new("ftp.drivehq.com", "agiletesting", "unittest");
            client.Connect();
            Assert.True(client.IsAuthenticated,"The client was not authenticated!");
            client.Disconnect();
        }

        [Fact]
        public void Get_List_Remote_Directory_Client_Not_Authenticated()
        {
            FtpClient client = new ();
            string[] args = { "ls", "-r" };
            Program.FilePath path = new ();
            path.SetInitalPaths("/", "/");
            Assert.Equal(-1, Get.List(ref client, in path, args));
        }

        [Fact]
        public void Get_List_Remote_Directory_Client_Is_Authenticated()
        {
            FtpClient client = new("ftp.drivehq.com", "agiletesting", "unittest");
            client.Connect();
            string[] args = { "ls", "-r" };
            Program.FilePath path = new();
            path.SetInitalPaths("/", "/");
            Assert.Equal(0, Get.List(ref client, in path, args));
            client.Disconnect();
        }

        [Fact]
        public void Get_List_Remote_Directory__Dir_Does_Not_Exist()
        {
            FtpClient client = new("ftp.drivehq.com", "agiletesting", "unittest");
            Program.FilePath path = new ();
            path.SetInitalPaths("/", "/");
            client.Connect();
            string[] args = { "ls","ThisDirectoryDoesNotExist" };
            Assert.Equal(-1, Get.List(ref client, in path, args));
            client.Disconnect();
        }
        [Fact]
        public void Get_List_Remote_Directory_Exist()
        {
            FtpClient client = new("ftp.drivehq.com", "agiletesting", "unittest");
            client.Connect();
            string[] args = { "ls", "-r" };
            Program.FilePath path = new ();
            path.SetInitalPaths("/", "/Test");
            Assert.Equal(0, Get.List(ref client, in path, args));
            client.Disconnect();
        }

        [Fact]
        public void Get_Go_To_Prev_Local_Directory_Failed_Since_At_Root_Directory()
        {
            Program.FilePath path = new ();
            path.SetInitalPaths("/", "/DirOnRemote");
            Assert.Equal(-1, Get.GoToPrevLocalDirectory(ref path));
        }

        [Fact]
        public void Get_Go_To_Prev_Local_Directory_Successs()
        {
            Program.FilePath path = new ();
            path.SetInitalPaths("/OneDirAhead/", "/");
            Assert.Equal(0, Get.GoToPrevLocalDirectory(ref path));
            Assert.Equal("/", path.Local);
        }

        [Fact]
        public void Get_Go_To_Prev_Remote_Directory_Failed_Since_At_Root_Directory()
        {
            FtpClient client = new("ftp.drivehq.com", "agiletesting", "unittest");
            client.Connect();
            Program.FilePath path = new ();
            path.SetInitalPaths("/DirOnLocal", "/");
            Assert.Equal(-1, Get.GoToPrevRemoteDirectory(ref path, in client));
            client.Disconnect();
        }

        [Fact]
        public void Get_Go_To_Prev_Remote_Directory_Successs()
        {
            FtpClient client = new("ftp.drivehq.com", "agiletesting", "unittest");
            client.Connect();
            Program.FilePath path = new ();
            path.SetInitalPaths("./", "/OneDirAhead/");
            Assert.Equal(0, Get.GoToPrevRemoteDirectory(ref path, in client));
            Assert.Equal("/", path.Remote);
            client.Disconnect();
        }

        [Fact]
        public void Get_Change_Remote_Directory_Success()
        {
            FtpClient client = new("ftp.drivehq.com", "agiletesting", "unittest");
            client.Connect();
            Program.FilePath path = new();
            path.SetInitalPaths("./", "/");
            int index = 0;
            string[] args = { "cd", "-r", "Test" };
            Assert.Equal(0, Get.ChangeRemoteDirectory(in client, ref path, in args, index));
            Assert.Equal("/Test/", path.Remote);
            client.Disconnect();
        }

        [Fact]
        public void Get_Change_Remote_Directory_Dir_Does_Not_Exist()
        {
            FtpClient client = new("ftp.drivehq.com", "agiletesting", "unittest");
            client.Connect();
            Program.FilePath path = new();
            path.SetInitalPaths("./", "/");
            int index = 0;
            string[] args = { "cd", "-r", "DoesNotExist" };
            Assert.Equal(-1, Get.ChangeRemoteDirectory(in client, ref path, in args, index));
            Assert.Equal("/", path.Remote);
            client.Disconnect();
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
           Assert.True(Get.IsAtRootDirectory(path.Local),"Local directory is not at the root!");
           Assert.True(Get.IsAtRootDirectory(path.Remote),"Remote directory is not at root!");
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
