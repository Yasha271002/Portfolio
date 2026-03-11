using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MvvmNavigationLib.Services;
using MvvmNavigationLib.Services.ServiceCollectionExtensions;
using MvvmNavigationLib.Stores;
using Serilog;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Messaging;
using VolgaTermalBathsEnter.Managers;
using VolgaTermalBathsEnter.Models.Client.User;
using VolgaTermalBathsEnter.Models.Client.User.Price;
using VolgaTermalBathsEnter.Models.Client.User.Ticket;
using VolgaTermalBathsEnter.Models.Interfaces;
using VolgaTermalBathsEnter.ViewModels.Pages;
using VolgaTermalBathsEnter.ViewModels.Popups;
using VolgaTermalBathsEnter.ViewModels.Popups.CardOnTerminal;
using AddDataViewModel = VolgaTermalBathsEnter.ViewModels.Popups.AddDataViewModel;
using VolgaTermalBathsEnter.Models;

namespace VolgaTermalBathsEnter.HostBuilders;

public static class BuildModalNavigationExtension
{
    public static IHostBuilder BuildModalNavigation(this IHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            services.AddSingleton<ModalNavigationStore>();
            services.AddUtilityNavigationServices<ModalNavigationStore>();

            services.AddNavigationService<ErrorPopupViewModel, ModalNavigationStore>(s =>
                new ErrorPopupViewModel(s.GetRequiredService<CloseNavigationService<ModalNavigationStore>>(),
                    s.GetRequiredService<NavigationService<WelcomeUserViewModel>>()));

            services.AddNavigationService<ThanksGoTurnstilePopupViewModel, ModalNavigationStore>(s =>
                new ThanksGoTurnstilePopupViewModel(
                    s.GetRequiredService<CloseNavigationService<ModalNavigationStore>>(),
                    s.GetRequiredService<NavigationService<WelcomeUserViewModel>>()));
            services.AddParameterNavigationService<BraceletApplyViewModel, ModalNavigationStore, (TotalPriceModel, UserDataModel)>(s =>
                param => new BraceletApplyViewModel(param, s.GetRequiredService<KassaManager>(),
                    s.GetRequiredService<ILogger>(), s.GetRequiredService<IUserApi>(),
                    s.GetRequiredService<NavigationService<PaidTicketViewModel>>(),
                    s.GetRequiredService<NavigationService<SelectedTicketPageViewModel>>(),
                    s.GetRequiredService<NavigationService<ErrorPopupViewModel>>(),
                    s.GetRequiredService<NavigationService<ThanksGoTurnstilePopupViewModel>>(),
                    s.GetRequiredService<CloseNavigationService<ModalNavigationStore>>(),
                    s.GetRequiredService<UpdateTicketsTrigger>()));

            services
                .AddParameterNavigationService<ScanAdultBraceletViewModel, ModalNavigationStore, TotalPriceModel>(s =>
                    param => new ScanAdultBraceletViewModel(param, s.GetRequiredService<UserDataModel>(),
                        s.GetRequiredService<ILogger>(), s.GetRequiredService<IUserApi>(),
                        s.GetRequiredService<IUserCardToTicketApi>(),
                        s.GetRequiredService<BraceletManager>(),
                        s.GetRequiredService<ParameterNavigationService<BraceletApplyViewModel, (TotalPriceModel, UserDataModel)>>(),
                        s.GetRequiredService<NavigationService<ErrorPopupViewModel>>(),
                        s.GetRequiredService<CloseNavigationService<ModalNavigationStore>>()));

            services.AddParameterNavigationService<AddDataViewModel, ModalNavigationStore, TicketModel>(s =>
                param => new AddDataViewModel(s.GetRequiredService<IMessenger>(),
                    s.GetRequiredService<CloseNavigationService<ModalNavigationStore>>(),
                    param));

            services
                .AddParameterNavigationService<PaymentPopupViewModel, ModalNavigationStore,
                    (ObservableCollection<TicketModel>, TotalPriceModel)>(s =>
                    param => new PaymentPopupViewModel(param.Item1, param.Item2,
                        s.GetRequiredService<UserDataModel>(),
                        s.GetRequiredService<KassaManager>(),
                        s.GetRequiredService<ILogger>(),
                        s.GetRequiredService<IUserApi>(),
                        s.GetRequiredService<NavigationService<ErrorPopupViewModel>>(),
                        s.GetRequiredService<NavigationService<PaymentSuccessfulPageViewModel>>(),
                        s.GetRequiredService<CloseNavigationService<ModalNavigationStore>>()));

            services.AddNavigationService<PasswordPopupViewModel, ModalNavigationStore>(s =>
                new PasswordPopupViewModel(
                    s.GetRequiredService<CloseNavigationService<ModalNavigationStore>>(),
                    context.Configuration.GetValue<string>("exitPassword") ?? "1234"));
        });

        return builder;
    }
}