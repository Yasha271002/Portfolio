using Newtonsoft.Json;

namespace VolgaTermalBathsEnter.Models.Client.CartCost;

public class ApiGetCartCost
{
    [JsonProperty("purchase_id")] public string? PurchaseId { get; set; }
    [JsonProperty("count")] public int Count { get; set; }
    [JsonProperty("price")] public int Price { get; set; }
    [JsonProperty("payment_amount")] public int PaymentAmount { get; set; }
    [JsonProperty("discount_sum")] public int DiscountSum { get; set; }
}