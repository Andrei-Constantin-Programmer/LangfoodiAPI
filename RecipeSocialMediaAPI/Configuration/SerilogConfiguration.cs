using Serilog;
using Serilog.Events;

namespace RecipeSocialMediaAPI.Utilities;

internal static class SerilogConfiguration
{
    public static void ConfigureLogging(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((_, _, configuration) => configuration
            .WriteTo.Console()
            .WriteTo.File("C:\\Logs\\RecipeSocialMedia\\RecipeSocialMediaLog.txt", rollingInterval: RollingInterval.Day)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithThreadId()
            .MinimumLevel.Is(LogEventLevel.Debug));
    }
}
