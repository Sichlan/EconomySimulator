﻿using EconomySimulator.WPF.Views.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wpf.Ui.Mvvm.Contracts;

namespace EconomySimulator.WPF.ExtensionMethods.HostBuilders;

public static class AddViewsHostBuilderExtension
{
    public static IHostBuilder AddViews(this IHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureServices(services =>
        {
            services.AddScoped<INavigationWindow, MainWindow>();
        });

        return hostBuilder;
    }
}