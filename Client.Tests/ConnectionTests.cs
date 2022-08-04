using Xunit;
using FakeItEasy;
using FluentFTP;
using Client;
namespace Client.Tests
{
    public class ConnectionTests
    {
        [Fact]
        public void Connection_Connect_No_IP()
        {
            FtpClient client = new();
            Logger logger = new();
            Commands.Connect commands = new();
            Program.FilePath path = new();
            Assert.Equal(-1,Connection.Connect(ref client, ref logger, commands,ref path));    
        }
       
    }
}