using Newtonsoft.Json;

namespace VolgaTermalBathsEnter.Models.Client.DebtPayment;

public class DebtPaymentBody
{
    [JsonProperty("client_id")] public string ClientId { get; set; }
    [JsonProperty("sales")] public List<string> Sales { get; set; } = [];
    [JsonProperty("payment_list")] public List<Payment> PaymentList { get; set; } = [];
    [JsonProperty("type")] public string Type { get; set; }
    [JsonProperty("amount")] public double Amount { get; set; }
    [JsonProperty("id")] public string Id { get; set; }
    [JsonProperty("payment_type")] public int? PaymentType { get; set; }
    [JsonProperty("cost")] public double Cost { get; set; }
    [JsonProperty("transaction_id")] public string TransactionId { get; set; }
    [JsonProperty("itn")] public string Itn { get; set; }
}