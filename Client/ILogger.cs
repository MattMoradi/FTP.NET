namespace Client
{
    public interface ILogger
    {
        void Log(string[] message);
        string CreateDirectory(string path);
    }
}
