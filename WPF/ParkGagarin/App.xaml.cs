using GoToGagarin.Helpers;
using GoToGagarin.Utilities;
using GoToGagarin.View.Window;
using GoToGagarin.ViewModel.Controls;
using GoToGagarin.ViewModel.Window;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Refit;
using System.Windows;
using GoToGagarin.ViewModel.Popup;
using MvvmNavigationLib.Services.ServiceCollectionExtensions;
using MvvmNavigationLib.Stores;
using CommunityToolkit.Mvvm.Messaging;
using MvvmNavigationLib.Services;
using GoToGagarin.Model;
using Microsoft.Extensions.Logging;
using Serilog;
using ILogger = Serilog.ILogger;

namespace GoToGagarin
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private readonly IHost _host = Host.CreateDefaultBuilder()
            .ConfigureHostConfiguration(s => s.AddJsonFile("appsettings.json"))
            .ConfigureServices((context, services) =>
                {
                    var host = context.Configuration.GetValue<string>("host");
                    var terminalId = context.Configuration.GetValue<int>("terminalId");
                    var refitSettings = new RefitSettings
                    {
                        ContentSerializer = new NewtonsoftJsonContentSerializer(),
                    };
                    services.AddSerilog((_, loggerConfiguration) =>
                        loggerConfiguration.ReadFrom.Configuration(context.Configuration));
                    services.AddUtilityNavigationServices<ModalNavigationStore>();
                    services.AddSingleton<IMessenger>(_ => new WeakReferenceMessenger());
                    services.AddHttpClient<ImageLoadingHttpClient>(c => c.BaseAddress = new Uri(host ?? string.Empty));
                    services.AddRefitClient<IMainApiClient>(settings: refitSettings)
                        .ConfigureHttpClient(c =>
                        {
                            c.BaseAddress = new Uri(host ?? string.Empty);
                            c.Timeout = TimeSpan.FromMinutes(10);
                        });

                    var inactivityTime = context.Configuration.GetValue<int>("inactivityTime");
                    services.AddSingleton<InactivityHelper>(_ => new InactivityHelper(inactivityTime));

                    services.AddSingleton<ModalNavigationStore>();

                    services.AddSingleton<MapViewModel>(s => new MapViewModel(
                        s.GetRequiredService<ImageLoadingHttpClient>(),
                        s.GetRequiredService<IMainApiClient>(),
                        terminalId,
                        s.GetRequiredService<ILogger>()));
                    services.AddSingleton<SearchViewModel>();
                    services.AddSingleton<ObjectInfoViewModel>();
                    services.AddSingleton<NavigationViewModel>();
                    services.AddSingleton<MainWindowViewModel>();

                    services.AddParameterNavigationService<ContentSliderPopupViewModel, ModalNavigationStore, MapViewModel>(
                        s => param => new ContentSliderPopupViewModel(param, s.GetRequiredService<CloseNavigationService<ModalNavigationStore>>()));
                    services.AddParameterNavigationService<InactivityPopupViewModel, ModalNavigationStore, ObjectInfoViewModel>(
                        s => param => new InactivityPopupViewModel(param, s.GetRequiredService<CloseNavigationService<ModalNavigationStore>>()));

                    services.AddSingleton<MainWindow>(
                        s => new MainWindow
                        {
                            DataContext = s.GetRequiredService<MainWindowViewModel>()
                        });
                }
            ).Build();

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (DebugHelper.IsRunningInDebugMode) throw e.Exception;
            var logger = _host.Services.GetRequiredService<ILogger>();
            logger.Error(e.Exception, "Неизвестная ошибка");
            e.Handled = true;
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            MainWindow = _host.Services.GetRequiredService<MainWindow>();
            MainWindow.Show();
            await _host.StartAsync();
            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await _host.StopAsync();
            _host.Dispose();
            base.OnExit(e);
        }
    }
}