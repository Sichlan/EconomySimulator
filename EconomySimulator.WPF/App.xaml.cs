using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using EconomySimulator.WPF.ExtensionMethods.HostBuilders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace EconomySimulator.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    [Localizable(false)]
    public partial class App : Application
    {
        private static IHost _host;

        public static IHostBuilder CreateHostBuilder(string[] args = null)
        {
            var builder = new ConfigurationBuilder();
            BuildConfig(builder);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build())
                .Enrich.FromLogContext()
                .WriteTo.File("\\Logs\\log.txt", rollingInterval: RollingInterval.Hour)
                .CreateLogger();
            
            Log.Logger.Information("Application startup");
            
            return Host.CreateDefaultBuilder(args)
                .AddConfiguration(args)
                .AddServices()
                .AddViews()
                .AddViewModels()
                .UseSerilog();
        }

        /// <summary>
        /// Gets registered service.
        /// </summary>
        /// <typeparam name="T">Type of the service to get.</typeparam>
        /// <returns>Instance of the service or <see langword="null"/>.</returns>
        public static T GetService<T>()
            where T : class
        {
            return _host.Services.GetService(typeof(T)) as T;
        }

        /// <summary>
        /// Occurs when the application is loading.
        /// </summary>
        private async void OnStartup(object sender, StartupEventArgs e)
        {
            _host = CreateHostBuilder(e.Args).Build();
            await _host.StartAsync();
        }

        /// <summary>
        /// Occurs when the application is closing.
        /// </summary>
        private async void OnExit(object sender, ExitEventArgs e)
        {
            if (_host == null)
                return;

            await _host.StopAsync();
            _host.Dispose();
            
            Log.Logger.Information("Application shutdown");
        }
        
        /// <summary>
        /// Occurs when an exception is thrown by an application but not handled.
        /// </summary>
        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Log.Logger.Fatal("Unhandled dispatcher exception:\nsender {Sender}\nexception: {@E}", sender, e);
        }
        
        [Localizable(false)]
        static void BuildConfig(IConfigurationBuilder builder)
        {
            // Check the current directory that the application is running on 
            // Then once the file 'appsetting.json' is found, we are adding it.
            // We add env variables, which can override the configs in appsettings.json
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
        }

    }
}