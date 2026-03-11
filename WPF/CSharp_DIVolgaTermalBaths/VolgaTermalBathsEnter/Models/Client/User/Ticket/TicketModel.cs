using VolgaTermalBathsEnter.Models.Client.PriceList;

namespace VolgaTermalBathsEnter.Models.Client.User.Ticket;

public class TicketModel
{
    public int Price { get; set; }
    public string? Title { get; set; }
    public string? PurchaseId { get; set; }
    public string? DateTicket { get; set; }
    public string? Time { get; set; }
    public int Count { get; set; }
    public int Tax { get; set; }
    public string? ChildrenOrAdult { get; set; }
    public string? Id { get; set; }
    public string? AllText { get; set; }

    public TicketPersonalInfoModel? TicketPersonalInfo { get; set; } = new();
    public ApiPriceListModel SourcePrice { get; set; } = null!;

}