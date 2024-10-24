using Microsoft.Extensions.Logging;

namespace DeliveryFilterApp.Helpers.Logger
{
    public class FileLoggerProvider(string path) : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new FileLogger(path);
        }

        public void Dispose() => GC.SuppressFinalize(this);
    }
}