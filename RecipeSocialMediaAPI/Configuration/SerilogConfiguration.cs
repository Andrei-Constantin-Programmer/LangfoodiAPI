using Serilog;
using Serilog.Events;

namespace RecipeSocialMediaAPI.Configuration
{
    public static class SerilogConfiguration
    {
        public static void ConfigureSerilog(HostBuilderContext _, IServiceProvider __, LoggerConfiguration configuration)
        {
            configuration
                .WriteTo.Console()
                .WriteTo.File("C:\\Logs\\RecipeSocialMedia\\RecipeSocialMediaLog.txt", rollingInterval: RollingInterval.Day)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .MinimumLevel.Is(LogEventLevel.Debug);
        }
    }
}
