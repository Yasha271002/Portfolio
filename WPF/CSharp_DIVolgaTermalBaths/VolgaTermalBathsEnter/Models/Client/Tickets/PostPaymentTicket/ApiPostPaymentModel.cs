using Newtonsoft.Json;

namespace VolgaTermalBathsEnter.Models.Client.Tickets.PostPaymentTicket;

public class ApiPostPaymentModel
{
    [JsonProperty("client_id")] public string? ClientId { get; set; }
    [JsonProperty("transaction_id")] public string? TransactionId { get; set; }
    [JsonProperty("cart")] public List<CartModel>? Cart { get; set; } = [];
    [JsonProperty("payment_list")] public List<PaymentListModel>? PaymentList { get; set; } = [];
}