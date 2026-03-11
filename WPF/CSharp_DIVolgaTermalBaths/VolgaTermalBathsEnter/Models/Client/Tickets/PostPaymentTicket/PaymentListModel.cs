using Newtonsoft.Json;

namespace VolgaTermalBathsEnter.Models.Client.Tickets.PostPaymentTicket;

public class PaymentListModel
{
    [JsonProperty("type")] public string? Type { get; set; }
    [JsonProperty("amount")] public decimal Amount { get; set; }
}