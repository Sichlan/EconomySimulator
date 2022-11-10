using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

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
            .ConfigureAppConfiguration((_, config) =>
            {
                config.AddJsonFile("appsettings.json", true, true);
                config.AddEnvironmentVariables();
            });

        return hostBuilder;
    }
}