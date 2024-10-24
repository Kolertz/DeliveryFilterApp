using Microsoft.Extensions.Logging;

namespace DeliveryFilterApp.Helpers.Logger
{
    public class FileLogger(string path) : ILogger, IDisposable
    {
        private static readonly object _lock = new();

        public IDisposable BeginScope<TState>(TState state) where TState : notnull
        {
            return this;
        }

        public void Dispose() => GC.SuppressFinalize(this);

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId,
                    TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            lock (_lock)
            {
                File.AppendAllText(path, formatter(state, exception) + Environment.NewLine);
            }
        }
    }
}