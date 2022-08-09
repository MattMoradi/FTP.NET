using Xunit;
using FakeItEasy;
using FluentFTP;

namespace Client.Tests
{
    public class PutTests
    {
        FtpClient client = A.Fake<FtpClient>();
        Commands.Put commands = A.Fake<Commands.Put>();
        Program.FilePath path = new();

        [Fact]
        public void Not_Authenticated_Should_Fail()
        {
            path.SetInitalPaths("./", "/");
            string[] opts = { "test" };
            commands.Files = opts;

            Assert.Equal(-1, Put.File(ref client, commands, in path));
        }

		// Unfortunately - we could not figure out a way to do tests requiring a connection in the time frame.
		// I was attempting to stub some functions like client.IsAuthenticated(); but was unable to due to 
		// FluentFTP's Restrictive API. Some of the tests I was thinking of implementing are below.  - Pete W.

		//[Fact]
		//public void No_Files_Should_Fail()
		//{
		//	path.SetInitalPaths("./", "/");
		//	A.CallTo(() => client.IsAuthenticated).Returns(true);
		//	commands.Files = null;
		//	Assert.Equal(-1, Put.File(ref client, commands, in path));
		//}

		//[Fact]
		//public void Should_Call_Single_File()
		//{
		//	path.SetInitalPaths("./", "/");
		//	A.CallTo(() => client.IsAuthenticated).Returns(true);
		//	string[] opts = { "test.test" };
		//	commands.Files = opts;
		//	var options = new ProgressBarOptions
		//	{
		//		ForegroundColor = ConsoleColor.Yellow,
		//		ForegroundColorDone = ConsoleColor.DarkGreen,
		//		BackgroundColor = ConsoleColor.DarkGray,
		//		BackgroundCharacter = '\u2593'
		//	};
		//	A.CallTo(() => Put.SingleFile(ref client, opts, in path, options)).MustHaveHappened();
		//}

		// Single Files Success
		// Multiple Files Success
		// Permission Errors


	}
}
