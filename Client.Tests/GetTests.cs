using FluentFTP;
using System.Collections.Generic;
using System.IO;
using Xunit;

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

                var remoteDirs = new List<string>() { @"\UnitTest\Build.txt",
                                                  @"\UnitTest\Client.txt",
                                                  @"\UnitTest\NewFile.txt",
                                                  @"\UnitTest\Solution.txt" };

                var currentdir = Directory.GetCurrentDirectory();

                Directory.CreateDirectory(currentdir + @"\UnitTesting");

                var localDir = $"{currentdir + @"UnitTesting"}";

                Assert.Equal(remoteDirs.Count, Get.MultipleFiles(testClient, remoteDirs, localDir));
            }
        }

        [Fact]
        public void GetMultiplePartialSuccess()
        {
            using (FtpClient testClient = new FtpClient("ftp.drivehq.com", "Agile410", "CS410Pdx!"))
            {
                testClient.Connect();

                var remoteDirs = new List<string>() { @"\UnitTest\Build.txt",
                                                  @"\UnitTest\Fail1.txt",
                                                  @"\UnitTest\Fail2.txt",
                                                  @"\UnitTest\Solution.txt" };

                var currentdir = Directory.GetCurrentDirectory();

                Directory.CreateDirectory(currentdir + @"\UnitTesting");

                var localDir = $"{currentdir + @"UnitTesting"}";

                Assert.Equal((remoteDirs.Count - 2), Get.MultipleFiles(testClient, remoteDirs, localDir));
            }
        }

        public void GetMultipleHostNotSpecified()
        {
            using (FtpClient testClient = new FtpClient("ftp.drivehq.com", "Agile410", "CS410Pdx!"))
            {
                var remoteDirs = new List<string>() { @"\UnitTest\Build.txt" };

                var localDir = "ArbitraryDirectoryName";

                Assert.Equal(-1, Get.MultipleFiles(testClient, remoteDirs, localDir));
            }
        }

    }
}
