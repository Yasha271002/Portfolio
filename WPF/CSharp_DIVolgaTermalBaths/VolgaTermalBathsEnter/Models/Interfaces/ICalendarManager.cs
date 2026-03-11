using VolgaTermalBathsEnter.Models.Client.PriceList;

namespace VolgaTermalBathsEnter.Models.Interfaces;

public interface ICalendarManager
{
    Task Initialize();
    Task<ApiParameters<ApiPriceListModel>> GetDates(string today);

    Task Filters(DateTime firstDate, DateTime lastDate);
}