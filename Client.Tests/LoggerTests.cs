using Xunit;
using FakeItEasy;
using FluentFTP;
using System.IO;

namespace Client.Tests
{
    public class LoggerTests
    {
        [Fact]
        public void Logger_Log_Success()
        {
            ILogger logger = A.Fake<ILogger>();
            string[] args = { "ls", "-l" };
            FtpClient client = new();
            Program.LogInput(logger, args, in client);
            A.CallTo(() => logger.Log(args, client)).MustHaveHappened();
        }
    }
}
