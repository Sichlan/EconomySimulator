using System;
using EconomySimulator.BusinessLogic.Services.FileServices;
using EconomySimulator.BusinessLogic.Services.SimulationServices;
using EconomySimulator.WPF.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wpf.Ui.Mvvm.Contracts;
using Wpf.Ui.Mvvm.Services;

namespace EconomySimulator.WPF.ExtensionMethods.HostBuilders;

public static class AddServicesHostBuilderExtension
{
    public static IHostBuilder AddServices(this IHostBuilder hostBuilder)
    {
        return hostBuilder.ConfigureServices(services =>
        {
            services.AddHostedService<ApplicationHostService>();
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<IThemeService, ThemeService>();
            services.AddSingleton<ITaskBarService, TaskBarService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<ISimulationContainerService, SimulationContainerService>();
            services.AddSingleton<IZipFileService, ZipFileService>();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<Random>();
        });
    }
}