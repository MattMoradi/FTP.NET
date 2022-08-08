using Xunit;
using FakeItEasy;
using FluentFTP;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Client.Tests
{
    public class ConnectionTests
    {
        FtpClient client = A.Fake<FtpClient>();

        [Fact]
        public void Connection_Connect_Success()
        {
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

        [Fact]
        public void SaveCredentialsTest()
        {
            // randomized credentials
            client.Host = "testHost";
            client.Credentials.UserName = Guid.NewGuid().ToString();
            client.Credentials.Password = Guid.NewGuid().ToString();
            Connection.Save(client);

            string? username, password;
            string? credFile = client.Host + ".txt";

            using (StreamReader Reader = new StreamReader(credFile))
            {
                username = Reader.ReadLine();
                password = Reader.ReadLine();
                Reader.Close();
            };

            Aes cipher = Aes.Create();
            cipher.Key = Convert.FromBase64String(Connection.passkey);
            cipher.IV = Convert.FromBase64String(Connection.iv);
            ICryptoTransform cryptoTransform = cipher.CreateDecryptor();
            byte[] pass = Convert.FromBase64String(password);

            Assert.Equal(client.Credentials.UserName, username);
            Assert.Equal(client.Credentials.Password, Encoding.UTF8.GetString(cryptoTransform.TransformFinalBlock(pass, 0, pass.Length)));
        }
    }
}