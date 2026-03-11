using Newtonsoft.Json;
using VolgaTermalBathsEnter.Models.Client.Tickets;

namespace VolgaTermalBathsEnter.Models.Client.User.Price;

public class TotalPriceModel
{
    public double BonusCount { get; set; }

    public double TotalPrice {get; set; }

    [JsonIgnore]
    public double FinalPrice { get; set; }

    [JsonIgnore] public List<ApiGetTickets> SelectedTicket { get; set; } = [];
}