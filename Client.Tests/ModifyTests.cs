using Xunit;
using FakeItEasy;
using FluentFTP;

namespace Client.Tests
{
    public class ModifyTests
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

        [Fact]
        public void Delete_Remote_Directory_Client_Not_Authenticated()
        {
            FtpClient client = A.Fake<FtpClient>();
            Commands.Delete file = new();
            Program.FilePath path = new();
            Assert.Equal(-1, Modify.Delete(ref client, file, in path));
        }
    }
}
