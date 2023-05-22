using Serilog.Events;
using Serilog;

namespace RecipeSocialMediaAPI.Configuration
{
    public static class MapperConfiguration
    {
        public static void ConfigureMapper(HostBuilderContext _, IServiceProvider __, LoggerConfiguration configuration)
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
