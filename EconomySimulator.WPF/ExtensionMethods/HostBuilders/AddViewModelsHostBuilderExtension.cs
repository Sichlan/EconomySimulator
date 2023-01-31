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
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<SettingsViewModel>();
            services.AddSingleton<TestViewModel>();
            services.AddSingleton<SimulationMainViewModel>();
        });

        return hostBuilder;
    }
}