using Microsoft.Extensions.Configuration;
using Serilog;
using Services.Contracts;

namespace Services
{
    public class SerilogLoggerManager(IConfiguration configuration) : ILoggerService
    {


        private readonly ILogger logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .WriteTo.Async(w => w.Console())
                .CreateLogger();

        public void LogDebug(string message) => logger.Debug(message);

        public void LogError(string message) => logger.Error(message);

        public void LogInfo(string message) => logger.Information(message);

        public void LogWarning(string message) => logger.Warning(message);
    }
}
