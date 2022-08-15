using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CommsConsole.Helpers;
using CommsConsole.Marshalling;
using CommsConsole.Services;
using CommsConsole.ViewModels;
using CommsConsole.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace CommsConsole
{
    public partial class App : Application
    {
        public IServiceProvider? ServiceProvider { get; private set; }
        public IConfiguration? Configuration { get; private set; }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var builder = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                Configuration = builder.Build();

                var serviceCollection = new ServiceCollection();
                ConfigureServices(serviceCollection);

                ServiceProvider = serviceCollection.BuildServiceProvider();


                var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();

                desktop.MainWindow = mainWindow;
            }

            base.OnFrameworkInitializationCompleted();
        }
        private void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppSettings>(Configuration.GetSection(nameof(AppSettings)));
            services.Configure<RadioSettings>(Configuration.GetSection(nameof(RadioSettings)));
            services.Configure<NetworkSettings>(Configuration.GetSection(nameof(NetworkSettings)));
            services.Configure<Exclusions>(Configuration.GetSection(nameof(Exclusions)));
            services.Configure<SymbolicPorts>(Configuration.GetSection(nameof(SymbolicPorts)));
            services.AddSingleton<MainWindow>();
            services.AddSingleton<IMainWindowViewModel, MainWindowViewModel>();
            services.AddSingleton<INetworkService, NetworkService>();
            services.AddSingleton<ICommsMarshalling, CommsMarshalling>();
            services.AddSingleton<IPortManager, PortManager>();
        }
    }
}
