using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MvvmNavigationLib.Services;
using CommunityToolkit.Mvvm.Messaging;
using Serilog;
using VolgaTermalBathsEnter.Managers;
using VolgaTermalBathsEnter.Models.Client.CartCost;
using VolgaTermalBathsEnter.Models.Client.PriceList;
using VolgaTermalBathsEnter.Models.Client.User;
using VolgaTermalBathsEnter.Models.Client.User.Price;
using VolgaTermalBathsEnter.Models.Client.User.Ticket;
using VolgaTermalBathsEnter.Models.Interfaces;
using VolgaTermalBathsEnter.Models.Message;
using VolgaTermalBathsEnter.Utilities;
using VolgaTermalBathsEnter.ViewModels.Popups.CardOnTerminal;
using VolgaTermalBathsEnter.Views.Controls;

namespace VolgaTermalBathsEnter.ViewModels.Pages;

public partial class SelectedTicketPageViewModel(
    ILogger logger,
    IUserApi client,
    IMessenger messenger,
    UserDataModel userData,
    NavigationService<WelcomeUserViewModel> welcomePageNavigationService,
    NavigationService<PaidTicketViewModel> paidTicketNavigationService,
    ParameterNavigationService<PaymentPopupViewModel, (ObservableCollection<TicketModel>, TotalPriceModel)> paymentPopupNavigationService,
    ParameterNavigationService<Popups.AddDataViewModel, TicketModel> addDataNavigationService,
    ObservableCollection<TicketModel> ticketList) : ObservableObject, IRecipient<RefreshMessage>
{
    private CalendarManager? _calendarManager;

    public bool IsTimeSelected
    {
        get
        {
            if (SelectedTickets is null ||  SelectedTickets.Count == 0) return false;
            return AvailableTimes?.TimesData?.Time?.Any(t => t.IsSelected) == true;
        }
    }
        

    [ObservableProperty] private UserDataModel _userDataModel = new();
    [ObservableProperty] private TotalPriceModel _totalPriceModel = new();
    [ObservableProperty] private bool _isRadioButtonChecked;
    [ObservableProperty] private string? _selectedTime = string.Empty;
    [ObservableProperty] private bool _isEnabledNextPageButton;
    [ObservableProperty] private DateTime _selectedDate = DateTime.Now;
    [ObservableProperty] private List<ApiGetCartCost>? _cartCostParameters = [];
    [ObservableProperty] private ObservableCollection<TicketModel> _ticketList = ticketList;
    [ObservableProperty] private List<ApiPriceListModel>? _selectedTickets = [];
    [ObservableProperty] private Dictionary<string, List<ApiPriceListModel>> _groupedTicketList = [];
    [ObservableProperty] private ApiParameters<ApiPriceListModel> _selectedParameters = new();
    [ObservableProperty] private List<List<Stack<TicketModel>>> _ticketStack;
    [ObservableProperty] private ApiPriceListModel _availableTimes;
    [ObservableProperty] private List<ApiPriceListModel> _availableTickets;


    private void FilterData(List<ApiPriceListModel>? ticketModel)
    {
        if (ticketModel is null) return;

        //var isWeekend = SelectedDate.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
        var durationKeywords = new[] { "безлимит", "2часа", "3часа", "4часа", "тест" };
        var sortOrder = new List<string> { "безлимит", "4часа", "3часа", "2часа", "тест" };

        var groupedTickets = ticketModel
            //.Where(ticket => !CollectionStaticMethods.HasSpecialTicket(ticket.Title))
            .Where(ticket => CollectionStaticMethods.HasValidDuration(ticket.Title, durationKeywords))
            //.Where(ticket => CollectionStaticMethods.IsValidDayType(ticket.Title, isWeekend))
            .GroupBy(ticket => CollectionStaticMethods.GetDurationKey(ticket.Title, durationKeywords))
            .Select(group => new
            {
                Key = CollectionStaticMethods.FormatKey(group.Key),
                Tickets = group.ToList()
            })
            .OrderBy(group => sortOrder.IndexOf(group.Key.ToLower()))
            .ToDictionary(group => group.Key, group => group.Tickets);

        GroupedTicketList = groupedTickets.OrderBy(kv => ParseHours(kv.Key))
            .ToDictionary(kv => kv.Key, kv => kv.Value);

        SelectedTickets = null;
        IsRadioButtonChecked = false;
    }

    int ParseHours(string key)
    {
        if (key == "Безлимит")
            return int.MaxValue;

        // Берём первое число в строке: "2 часа" → 2
        if (int.TryParse(key.Split(' ')[0], out int hours))
            return hours;

        return int.MaxValue;
    }

    [RelayCommand]
    private void IncrementCount(ApiPriceListModel? ticket)
    {
        if (ticket is null) return;
        ticket.Count++;
        AddTicketToCart(ticket);

        RecalculateTotal();

    }

    [RelayCommand]
    private void DecrementCount(ApiPriceListModel? ticket)
    {
        if (ticket is null || ticket.Count <= 0) return;
        ticket.Count--;
        RemoveTicketFromCart(ticket);

        RecalculateTotal();
    }

    [RelayCommand]
    private void SwitchTimesData(string index)
    {
        if (GroupedTicketList.ContainsKey(index))
            SelectedTickets = GroupedTicketList[index];

        //SelectedTickets.ForEach(x => x.TimesData.Time.ForEach(x => x.IsSelected = false));
        AvailableTimes = SelectedTickets.MaxBy(x => x.TimesData.Time.Count);
        AvailableTimes.TimesData.Time.ForEach(x => x.IsSelected = false);
        OnPropertyChanged(nameof(IsTimeSelected));

        if (SelectedDate.Date != DateTime.Today) return;
        var currentTime = DateTime.Now.TimeOfDay;
        foreach (var timeRange in AvailableTimes.TimesData.Time)
        {
            TimeSpan.TryParse(timeRange.Name.Split('-').Last(), out var endTime);
            timeRange.IsEnabled = endTime > currentTime;
        }
    }

    [RelayCommand]
    private void SelectTime(string selectedTime)
    {
        SelectedTime = selectedTime;
        OnPropertyChanged(nameof(SelectedTime));
        OnPropertyChanged(nameof(IsTimeSelected));
        IsRadioButtonChecked = true;
        AvailableTickets = SelectedTickets.Where(s => s.TimesData.Time.Any(t => t.Name == selectedTime)).ToList();
    }

    [RelayCommand]
    private void RemoveTicket(TicketModel? ticket)
    {
        if (ticket is null) return;

        if (ticket.SourcePrice.Count > 0)
            ticket.SourcePrice.Count--;

        TicketList.Remove(ticket);

        RecalculateTotal();
        //TicketList.Remove(ticket);
        //OnPropertyChanged(nameof(TicketList));
        //TotalPriceModel.TotalPrice = TicketList.Count == 0 ? 0 : TicketList.Sum(price => price.Price);

        //OnPropertyChanged(nameof(TotalPriceModel));
        //Refresh();
    }

    [RelayCommand]
    private async Task Loaded(CalendarControlers controllers)
    {
        messenger.RegisterAll(this);
        OnPropertyChanged(nameof(TicketList));
        _calendarManager = new CalendarManager(client, logger, _userDataModel, controllers);
        await _calendarManager.Initialize(userData.ClientId);
    }

    [RelayCommand] private void Payment() => paymentPopupNavigationService.Navigate((TicketList, TotalPriceModel));

    [RelayCommand]
    private void GoBack()
    {
        TicketList.Clear();

        if (userData.HaveTicket)
        {
            paidTicketNavigationService.Navigate();
            return;
        }

        welcomePageNavigationService.Navigate();
    }

    [RelayCommand]
    private void AddDataNavigation(TicketModel ticket) => addDataNavigationService.Navigate(ticket);

    partial void OnSelectedDateChanged(DateTime value)
    {
        FilterData(value).ConfigureAwait(false);
        if (SelectedTickets is null || SelectedTickets.Count == 0) return;
        AvailableTimes.TimesData.Time.ForEach(x => x.IsSelected = false);
        OnPropertyChanged(nameof(IsTimeSelected));
    }

    private async Task FilterData(DateTime value)
    {
        var day = FormatDate(value);
        var clientId = userData.ClientId;
        var ticketData = await client.RequestProcessingWithErrorHandling(() => client.GetPriceList(clientId, day),
            logger, "Ошибка получения PriceList");
        FilterData(ticketData.Parameters);
    }

    private void Refresh()
    {
        if (TicketList.Count > 0 && (TicketList.All(ticket => !string.IsNullOrEmpty(ticket.TicketPersonalInfo?.FirstName))))
            IsEnabledNextPageButton = true;
        else
            IsEnabledNextPageButton = false;
    }

    private static string? FormatDate(DateTime? day) => day?.ToString("yyyy-MM-ddThh:mm");

    public void Receive(RefreshMessage message) => Refresh();

    private void AddTicketToCart(ApiPriceListModel price)
    {
        TicketList.Add(new TicketModel
        {
            SourcePrice = price,
            Price = Convert.ToInt32(price.Price),
            Title = price.Title,
            PurchaseId = price.AppointmentId,
            DateTicket = SelectedDate.ToString("dd-MM-yyyy"),
            Time = SelectedTime,
            Count = 1,
            ChildrenOrAdult = price.Title,
            Id = price.AppointmentId,
            Tax = price.Tax,
            AllText = $"{SelectedDate:dd-MM-yyyy} {SelectedTime} {price.Title}"
        });
    }

    private void RemoveTicketFromCart(ApiPriceListModel price)
    {
        var ticket = TicketList.FirstOrDefault(t =>
            t.Id == price.AppointmentId &&
            t.Time == SelectedTime &&
            t.ChildrenOrAdult == price.Title);

        if (ticket != null)
            TicketList.Remove(ticket);
    }

    private void RecalculateTotal()
    {
        TotalPriceModel.TotalPrice = TicketList.Sum(t => t.Price);
        OnPropertyChanged(nameof(TotalPriceModel));

        Refresh();
    }

}