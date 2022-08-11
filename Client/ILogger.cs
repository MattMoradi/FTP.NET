using FluentFTP;

namespace Client
{
    public interface ILogger
    {
        void Log(string[] message, in FtpClient client);
    }
}
