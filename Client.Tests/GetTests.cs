using FluentFTP;
using System.Collections.Generic;
using System.IO;
using Xunit;
using static Client.Program;

namespace Client.Tests
{
    public class GetTests
    {
        [Fact]
        public void GetMultiple_HostNotSpecified()
        {
            // non-client
            FtpClient testClient = new FtpClient();
            
             var remoteDirs = new List<string>() { "Build.txt" };

             var localDir = "ArbitraryDirectoryName";

             var dirs = new FilePath() { Remote = @"\UnitTest\", Local = @"\" };

             Assert.Equal(-1, Get.MultipleFiles(testClient, remoteDirs, localDir, dirs));
        }

        [Fact]
        public void GetMultiple_InvalidLocalDirectory()
        {
            var testClient = new FtpClient();

            var remoteDir = new List<string>() { "Build.txt" };

            var localDir = @"Arbitrary\Incorrect\Directory.txt";

            var dirs = new FilePath() { Local = @"\", Remote = @"\" };

            Assert.Equal(-1, Get.MultipleFiles(testClient, remoteDir, localDir, dirs));
        }

        [Fact]
        public void GetMultiple_InvalidRemoteFileNameAll()
        {
            var testClient = new FtpClient();

            var remoteDir = new List<string>() { "Buildtxt", "Hellotxt" };

            var localDir = @"Arbitrary\Directory";

            var dirs = new FilePath() { Local = @"\", Remote = @"\" };

            Assert.Equal(-1, Get.MultipleFiles(testClient, remoteDir, localDir, dirs));
        }

        [Fact]
        public void GetDirectory_InvalidDirectory()
        {
            var testClient = new FtpClient();

            var remoteDir = "RandomDirectory.txt";

            var localDir = string.Empty;

            var dirs = new FilePath() { Local = @"\", Remote = @"\" };

            Assert.Equal(-1, Get.RemoteDirectory(testClient, remoteDir, dirs, localDir));
        }

        [Fact]
        public void GetDirectory_InvalidLocalDirectory()
        {
            var testClient = new FtpClient();

            var remoteDir = @"\Random\Directory";

            var localDir = @"\Incorrect\Local\Directory.txt";

            var dirs = new FilePath() { Local = @"\", Remote = @"\" };

            Assert.Equal(-1, Get.RemoteDirectory(testClient, remoteDir, dirs, localDir));
        }

        [Fact]
        public void GetDirectory_HostNotSpecidfied()
        {
            var testClient = new FtpClient();

            var remoteDir = @"\Random\Directory";

            var localDir = @"\Incorrect\Local\Directory";

            var dirs = new FilePath() { Local = @"\", Remote = @"\" };

            Assert.Equal(-1, Get.RemoteDirectory(testClient, remoteDir, dirs, localDir));
        }
    }
}
