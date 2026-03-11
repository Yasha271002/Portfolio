using System.Windows.Controls;
using Serilog;
using VolgaTermalBathsEnter.Models.Client.PriceList;
using VolgaTermalBathsEnter.Models.Client.User;
using VolgaTermalBathsEnter.Models.Interfaces;
using VolgaTermalBathsEnter.Utilities;
using VolgaTermalBathsEnter.Views.Controls;
using Calendar = System.Windows.Controls.Calendar;

namespace VolgaTermalBathsEnter.Managers;

public class CalendarManager(IUserApi client, ILogger logger, UserDataModel userData, CalendarControlers control) : ICalendarManager
{
    private readonly Calendar _calendar = control.Calendar;
    private string _clientId = userData.ClientId;
    private readonly List<DateTime> _unavailableDates = [];

    public DateTime? GetSelectedDate() => _calendar.SelectedDate; 

    public async Task Initialize()
    {
        try
        {
            var today = DateTime.Today;
            _calendar.SelectedDate = today;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
            var lastDayOfMonth =
                new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
            GetInitialBlackoutDates(today, firstDayOfMonth);

            await Filters(today, lastDayOfMonth);
        }
        catch (Exception e)
        {
            logger.Error(e.Message + " Ошибка CalendarControl в методе GetData");
        }
    }

    public async Task Initialize(string clientId)
    {
        try
        {
            _clientId = clientId;
            var today = DateTime.Today;
            _calendar.SelectedDate = today;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
            var lastDayOfMonth =
                new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
            GetInitialBlackoutDates(today, firstDayOfMonth);

            await Filters(today, lastDayOfMonth);
        }
        catch (Exception e)
        {
            logger.Error(e.Message + " Ошибка CalendarControl в методе GetData");
        }
    }

    public async Task<ApiParameters<ApiPriceListModel>> GetDates(string today) =>
        await client.RequestProcessingWithErrorHandling(() => client.GetPriceList(_clientId, today), logger,
            "Ошибка получения Dates");

    public async Task Filters(DateTime today, DateTime lastDayOfMonth)
    {
        for (var date = today; date <= lastDayOfMonth; date = date.AddDays(1))
        {
            var day = FormatDate(date);
            var response = await GetDates(day);
            if (response != null)
            {
                var result = response.Parameters;
                if (!result!.Any(p => p.Availability)) _unavailableDates.Add(date);
            }
        }

        UpdateCalendar();
    }

    private void GetInitialBlackoutDates(DateTime today, DateTime firstDayOfMonth)
    {
        _unavailableDates.Clear();

        for (var date = firstDayOfMonth; date < today; date = date.AddDays(1))
            _unavailableDates.Add(date);

        UpdateCalendar();
    }

    private async Task UpdateCalendar()
    {
        try
        {
            _calendar.BlackoutDates.Clear();
            foreach (var date in _unavailableDates)
            {
                _calendar.BlackoutDates.Add(new CalendarDateRange(date, date));
            }
        }
        catch (Exception e)
        {
            logger.Error(e.Message + " Ошибка в методе UpdateCalendar");
        }
    }

    private static string FormatDate(DateTime? day)
    {
        var dayString = day?.ToString("yyyy-MM-dd");
        return dayString + "T10:00";
    }

    public void Calendar_OnSelectedDatesChanged()
    {
    }

    public async Task Calendar_OnDisplayDateChanged()
    {
        try
        {
            //var today = DateTime.Today;
            //var firstDayOfMonth = e.AddedDate!.Value;

            ////Валидация выбранной даты
            //if (Calendar_ValidateDate(ref firstDayOfMonth, ref today))
            //    firstDayOfMonth = new DateTime(today.Year, today.Month, 1);

            //var lastDayOfMonth = new DateTime(firstDayOfMonth.Year, firstDayOfMonth.Month, DateTime.DaysInMonth(firstDayOfMonth.Year, firstDayOfMonth.Month));
            //await Filters(today, lastDayOfMonth, "CLIENT_ID_НУЖЕН");
        }
        catch (Exception ex)
        {
            logger.Error(ex.Message);
        }
    }
}