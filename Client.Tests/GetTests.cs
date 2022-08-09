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
        public void GetMultipleSuccess()
        {
            using (FtpClient testClient = new FtpClient("ftp.drivehq.com", "Agile410", "CS410Pdx!"))
            {
                testClient.Connect();

                var remoteDirs = new List<string>() { "Build.txt","Client.txt","NewFile.txt", "Solution.txt" };

                var currentdir = Directory.GetCurrentDirectory();

                Directory.CreateDirectory(currentdir + @"\UnitTesting");

                var localDir = $"{currentdir + @"UnitTesting"}";

                var dirs = new FilePath() { Remote = @"\UnitTest\", Local= @"\" }; 

                Assert.Equal(remoteDirs.Count, Get.MultipleFiles(testClient, remoteDirs, localDir, dirs));
            }
        }

        [Fact]
        public void GetMultiplePartialSuccess()
        {
            using (FtpClient testClient = new FtpClient("ftp.drivehq.com", "Agile410", "CS410Pdx!"))
            {
                testClient.Connect();
                
                var remoteDirs = new List<string>() { "Build.txt","Fail1.txt","Fail2.txt","Solution.txt" };

                var currentdir = Directory.GetCurrentDirectory();

                Directory.CreateDirectory(currentdir + @"\UnitTesting");

                var localDir = $"{currentdir + @"UnitTesting"}";

                var dirs = new FilePath() { Remote = @"\UnitTest\", Local = @"\" };

                Assert.Equal((remoteDirs.Count - 2), Get.MultipleFiles(testClient, remoteDirs, localDir, dirs));
            }
        }

        [Fact]
        public void GetMultipleHostNotSpecified()
        {
            using (FtpClient testClient = new FtpClient("ftp.drivehq.com", "Agile410", "CS410Pdx!"))
            {
                var remoteDirs = new List<string>() { "Build.txt" };

                var localDir = "ArbitraryDirectoryName";

                var dirs = new FilePath() { Remote = @"\UnitTest\", Local = @"\" };

                Assert.Equal(-1, Get.MultipleFiles(testClient, remoteDirs, localDir, dirs));
            }
        }
    }
}
