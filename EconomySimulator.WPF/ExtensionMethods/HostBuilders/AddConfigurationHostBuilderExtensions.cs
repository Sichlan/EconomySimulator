using System.IO;
using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace EconomySimulator.WPF.ExtensionMethods.HostBuilders;

public static class AddConfigurationHostBuilderExtensions
{
    public static IHostBuilder AddConfiguration(this IHostBuilder hostBuilder, string[] args)
    {
        hostBuilder
            .ConfigureHostConfiguration(builder =>
            {
                builder.AddCommandLine(args);
            })
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", true, true);
                config.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", true, true);
                config.AddEnvironmentVariables();
            })
            .UseSerilog((host, loggerConfiguration) =>
            {
                var outputTemplate = "{Timestamp} [{Level}] ({HttpRequestId}|{UserName}) {Message}{NewLine}{Exception}";

                loggerConfiguration.Enrich.FromLogContext()
                    .ReadFrom.Configuration(host.Configuration)
                    .WriteTo.File("\\Logs\\log.txt", rollingInterval: RollingInterval.Day, outputTemplate: outputTemplate)
                    .WriteTo.Debug(outputTemplate: outputTemplate);
            });

        return hostBuilder;
    }
}