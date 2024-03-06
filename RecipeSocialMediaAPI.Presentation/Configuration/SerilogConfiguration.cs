using Microsoft.Extensions.Options;
using RecipeSocialMediaAPI.Presentation.Options;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Datadog.Logs;

namespace RecipeSocialMediaAPI.Presentation.Utilities;

internal static class SerilogConfiguration
{
    public static void ConfigureLogging(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((_, serviceProvider, configuration) =>
        {
            var dataDogOptions = serviceProvider.GetService<IOptions<DataDogOptions>>()?.Value ?? new();

            configuration
                .WriteTo.Console()
                .WriteTo.File("C:\\Logs\\RecipeSocialMedia\\RecipeSocialMediaLog.txt", rollingInterval: RollingInterval.Day)
                .WriteTo.DatadogLogs(
                    apiKey: dataDogOptions.ApiKey,
                    service: dataDogOptions.Service,
                    logLevel: LogEventLevel.Information,
                    configuration: new DatadogConfiguration() { Url =  dataDogOptions.Url }
                )
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .MinimumLevel.Is(LogEventLevel.Debug);
        });
    }
}
