using EconomySimulator.WPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EconomySimulator.WPF.ExtensionMethods.HostBuilders;

public static class AddViewModelsHostBuilderExtension
{
    public static IHostBuilder AddViewModels(this IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureServices(services =>
        {
            services.AddScoped<MainWindowViewModel>();
            services.AddScoped<SettingsViewModel>();
            services.AddScoped<TestViewModel>();
            services.AddScoped<SimulationMainViewModel>();
        });

        return hostBuilder;
    }
}