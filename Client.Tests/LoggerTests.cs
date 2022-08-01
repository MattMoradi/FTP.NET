using Xunit;
using FakeItEasy;
namespace Client.Tests
{
    public class LoggerTests
    {
        [Fact]
        public void Logger_Log_Create_Success()
        {
            Logger logger = A.Fake<Logger>(x => x.WithArgumentsForConstructor(() => new Logger("test")));
            logger.Log("test");
            A.CallTo(() => logger.CreateDirectory("test")).MustHaveHappenedOnceExactly();
        }
    }
}
