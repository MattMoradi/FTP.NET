using Xunit;
using FakeItEasy;
using FluentFTP;
using Client;
namespace Client.Tests
{
    public class ConnectionTests
    {
        [Fact]
        public void Connection_Connect_Success()
        {
            
            FtpClient client = A.Fake<FtpClient>();

           // A.CallTo(() =>
        }
        [Fact]
        public void Connection_Connect_No_IP()
        {
            /*
            FtpClient client = A.Fake<FtpClient>();
            Connection connection = A.Fake<Connection>();
            Logger logger = A.Fake<Logger>();
            Commands.Connect commands = A.Fake<Commands.Connect>();
            Program.FilePath path = new Program.FilePath();
            A.CallTo(() => connection.Connect(ref client, ref logger, commands, ref path)).MustHaveHappenedOnceExactly();
            Assert.True(connection.Connect(ref client, ref logger, commands, ref path) == -1);
            */
        }
        [Fact]
        public void Connection_Connect_Invalid_Username_Exception()
        {

        }
        [Fact]
        public void Connection_Connect_Invalid_Password_Exception()
        {

        }
        [Fact]
        public void Connection_Connect_Invalid_Host_Name_Exception()
        {

        }
        [Fact]
        public void Connection_Logger_Not_Null()
        {
            Logger logger = A.Fake<Logger>();
            
        }
    }
}