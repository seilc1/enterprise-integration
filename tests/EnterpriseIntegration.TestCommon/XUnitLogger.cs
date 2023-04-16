using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace EnterpriseIntegration.Tests
{
    public class XUnitLogger<T> : ILogger<T>, IDisposable
    {
        private readonly ITestOutputHelper _output;
        public XUnitLogger(ITestOutputHelper output)
        {
            this._output = output;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return this;
        }

        public void Dispose() { }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            _output.WriteLine($"[{logLevel}:{eventId}]: {state}");
        }
    }
}
