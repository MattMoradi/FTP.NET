using Xunit;
using FakeItEasy;
using FluentFTP;
//using Client;
//using CommandLine;
using System;
using System.Collections;
using ShellProgressBar;

namespace Client.Tests
{
    public class PutTests
    {
        //FtpClient client = A.Fake<FtpClient>();
        ClientWrapper client = A.Fake<ClientWrapper>();
        Commands.Put commands = A.Fake<Commands.Put>();
        Program.FilePath path = new();

        [Fact]
        public void Not_Authenticated_Should_Fail()
        {

            A.CallTo(() => client.IsAuthenticated()).Returns(false);
            path.SetInitalPaths("./", "/");
            string[] opts = { "test" };
            commands.Files = opts;

            Assert.Equal(-1, Put.File(ref client, commands, in path));
        }

		[Fact]
		public void No_Files_Should_Fail()
		{
			path.SetInitalPaths("./", "/");
            A.CallTo(() => client.IsAuthenticated()).Returns(true);
            commands.Files = null;
            Assert.Equal(-1, Put.File(ref client, commands, in path));
        }

		[Fact]
        public void Should_Call_Single_File()
		{
            path.SetInitalPaths("./", "/");
            A.CallTo(() => client.IsAuthenticated()).Returns(true);
            string[] opts = { "test.test" };
            commands.Files = opts;
            var options = new ProgressBarOptions
            {
                ForegroundColor = ConsoleColor.Yellow,
                ForegroundColorDone = ConsoleColor.DarkGreen,
                BackgroundColor = ConsoleColor.DarkGray,
                BackgroundCharacter = '\u2593'
            };
            A.CallTo(() => Put.SingleFile(ref client, opts, in path, options)).MustHaveHappened();
        }


    }
}
