using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MvvmNavigationLib.Services;
using MvvmNavigationLib.Stores;
using Serilog;
using VolgaTermalBathsEnter.Managers;
using VolgaTermalBathsEnter.Models;
using VolgaTermalBathsEnter.Models.Bracelete.Configuration;
using VolgaTermalBathsEnter.Models.Client.Settings.SmsSettings;
using VolgaTermalBathsEnter.Models.Client.User;
using VolgaTermalBathsEnter.Models.Client.User.Ticket;
using VolgaTermalBathsEnter.ViewModels;
using VolgaTermalBathsEnter.ViewModels.Pages;
using VolgaTermalBathsEnter.ViewModels.Windows;

namespace VolgaTermalBathsEnter.HostBuilders
{
    public static class BuildViewsExtension
    {
        public static IHostBuilder BuildViews(this IHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                var inactivityConfig = context.Configuration.GetSection("inactivity").Get<InactivityConfig>();
                var portConfiguration = context.Configuration.GetSection("portConfiguration").Get<PortModel>();
                services.AddSingleton(context.Configuration.GetSection("smsSettings").Get<SmsApiModel>() ??
                                      new SmsApiModel()
                                      {
                                          ApiKey = "",
                                          Host = "",
                                          SenderName = ""
                                      });

                services.AddSingleton<IMessenger>(_ => new WeakReferenceMessenger());
                services.AddSingleton<InactivityManager<WelcomeUserViewModel>>(s =>
                    new InactivityManager<WelcomeUserViewModel>(
                        inactivityConfig ?? new InactivityConfig(60, 10),
                        s.GetRequiredService<NavigationStore>(),
                        s.GetRequiredService<NavigationService<WelcomeUserViewModel>>(),
                        s.GetRequiredService<CloseNavigationService<ModalNavigationStore>>(),
                        s.GetRequiredService<ModalNavigationStore>()));

                services.AddTransient<BraceletManager>(s => new BraceletManager(portConfiguration, s.GetRequiredService<ILogger>()));
                services.AddSingleton<KassaManager>(provider =>
                {
                    KassaManager manager = null;
                    ILogger logger = provider.GetRequiredService<ILogger>();

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        manager = new KassaManager(logger);
                    });

                    return manager;
                });

                services.AddSingleton<UserDataModel>();
                services.AddSingleton<ObservableCollection<TicketModel>>();

                services.AddSingleton<MainWindowViewModel>();
                services.AddSingleton(s => new Views.Windows.MainWindow()
                {
                    DataContext = s.GetRequiredService<MainWindowViewModel>()
                });
            });
            return builder;
        }
    }
}