using System.Windows;
using System.Windows.Threading;
using EconomySimulator.WPF.ExtensionMethods.HostBuilders;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace EconomySimulator.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static IHost _host;

        public static IHostBuilder CreateHostBuilder(string[] args = null)
        {
            return Host.CreateDefaultBuilder(args)
                .AddConfiguration(args)
                .AddServices()
                .AddViews()
                .AddViewModels();
        }

        /// <summary>
        /// Occurs when the application is loading.
        /// </summary>
        private async void OnStartup(object sender, StartupEventArgs e)
        {
            _host = CreateHostBuilder(e.Args).Build();
            await _host.StartAsync();
            
            Log.Logger.Debug("Logging on debug level");
            Log.Logger.Information("Application started");
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

    }
}