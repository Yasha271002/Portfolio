using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MvvmNavigationLib.Services;
using MvvmNavigationLib.Services.ServiceCollectionExtensions;
using MvvmNavigationLib.Stores;
using Serilog;
using VolgaTermalBathsEnter.Models;
using VolgaTermalBathsEnter.Models.Client.User;
using VolgaTermalBathsEnter.Models.Client.User.Price;
using VolgaTermalBathsEnter.Models.Client.User.Ticket;
using VolgaTermalBathsEnter.Models.Interfaces;
using VolgaTermalBathsEnter.ViewModels.Pages;
using VolgaTermalBathsEnter.ViewModels.Popups.CardOnTerminal;
using AddDataViewModel = VolgaTermalBathsEnter.ViewModels.Popups.AddDataViewModel;

namespace VolgaTermalBathsEnter.HostBuilders;

public static class BuildMainNavigationExtension
{
    public static IHostBuilder BuildMainNavigation(this IHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            services.AddSingleton<NavigationStore>();
            services.AddUtilityNavigationServices<NavigationStore>();
            services.AddNavigationService<WelcomeUserViewModel, NavigationStore>();
            services.AddNavigationService<UserRegisterViewModel, NavigationStore>();

            services.AddSingleton<UpdateTicketsTrigger>();
            services.AddNavigationService<SelectedTicketPageViewModel, NavigationStore>(s =>
                new SelectedTicketPageViewModel(s.GetRequiredService<ILogger>(),
                    s.GetRequiredService<IUserApi>(),
                    s.GetRequiredService<IMessenger>(),
                    s.GetRequiredService<UserDataModel>(),
                    s.GetRequiredService<NavigationService<WelcomeUserViewModel>>(),
                    s.GetRequiredService<NavigationService<PaidTicketViewModel>>(),
                    s.GetRequiredService<ParameterNavigationService<PaymentPopupViewModel,
                        (ObservableCollection<TicketModel>, TotalPriceModel)>>(),
                    s.GetRequiredService<ParameterNavigationService<AddDataViewModel, TicketModel>>(),
                    s.GetRequiredService<ObservableCollection<TicketModel>>()));


            services.AddNavigationService<PaidTicketViewModel, NavigationStore>();
            services.AddNavigationService<PaymentSuccessfulPageViewModel, NavigationStore>();
        });

        return builder;
    }
}