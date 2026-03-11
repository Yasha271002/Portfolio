using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MvvmNavigationLib.Services;
using Serilog;
using VolgaTermalBathsEnter.Models;
using VolgaTermalBathsEnter.Models.Client.PriceList;
using VolgaTermalBathsEnter.Models.Client.Tickets;
using VolgaTermalBathsEnter.Models.Client.User;
using VolgaTermalBathsEnter.Models.Client.User.Price;
using VolgaTermalBathsEnter.Models.Interfaces;
using VolgaTermalBathsEnter.Utilities;
using VolgaTermalBathsEnter.ViewModels.Popups;

namespace VolgaTermalBathsEnter.ViewModels.Pages;

public partial class PaidTicketViewModel(
    ILogger logger,
    IUserApi client,
    UserDataModel userData,
    NavigationService<WelcomeUserViewModel> welcomePageNavigationService,
    NavigationService<SelectedTicketPageViewModel> selectedTicketPageNavigationService,
    NavigationService<ErrorPopupViewModel> errorPopupNavigationService,
    ParameterNavigationService<ScanAdultBraceletViewModel, TotalPriceModel> scanBraceletNavigationService, UpdateTicketsTrigger updateTicketsTrigger)
    : ObservableObject
{
    [ObservableProperty] private TotalPriceModel _totalPriceModel = new();
    [ObservableProperty] private ApiParameters<ApiGetTickets> _ticketModel = new();
    [ObservableProperty] private bool _isTicketSelected;
    [ObservableProperty] private bool _isAllTicketApproved;

    private List<ApiGetTickets> SelectedTicket =>
        [TicketModel.Parameters!.FirstOrDefault(ticket => ticket.IsSelected)!];

    [RelayCommand]
    private void Loaded()
    {
        updateTicketsTrigger.UpdateTicketsEvent += GetTicketsUser;
        GetTicketsUser();
    }

    [RelayCommand]
    private void Unloaded()
    {
        updateTicketsTrigger.UpdateTicketsEvent -= GetTicketsUser;
    }

    [RelayCommand]
    private void WelcomeUserNavigation() => welcomePageNavigationService.Navigate();

    [RelayCommand]
    private void SelectedTicketPageNavigation() => selectedTicketPageNavigationService.Navigate();


    [RelayCommand]
    private void GoToScanBracelet()
    {
        var checkSelectedTicket = CheckIfTicketIsSelected();
        if (!checkSelectedTicket)
        {
            logger.Error("Билет не выбран");
            return;
        }

        TotalPriceModel.SelectedTicket.Clear();
        TotalPriceModel.SelectedTicket.Add(SelectedTicket.FirstOrDefault());
        scanBraceletNavigationService.Navigate(TotalPriceModel);
    }

    private bool CheckIfTicketIsSelected()
    {
        IsTicketSelected = TicketModel.Parameters != null && TicketModel.Parameters.Any(ticket => ticket.IsSelected);
        return IsTicketSelected;
    }

    private async void GetTicketsUser()
    {
        IsAllTicketApproved = false;
        var phoneNumber = userData.PhoneNumber;
        var response = await client.RequestProcessingWithErrorHandling(() => client.Get_Tickets(phoneNumber),
            logger, "Ошибка получения билетов по номеру телефона");

        if (response.Error is not null)
        {
            logger.Error("Ошибка получения билетов по номеру телефона");
            errorPopupNavigationService.Navigate();
            return;
        }

        //response.Parameters = response!.Parameters!.Where(f => f.Key!.Length == 0).ToList();

        TicketModel = response;
        IsAllTicketApproved = true;
    }
}