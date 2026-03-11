using Newtonsoft.Json;

namespace VolgaTermalBathsEnter.Models.Client.DebtDetail;

public class ApiTicketModel
{
    [JsonProperty("id")] public string? Id { get; set; }
    [JsonProperty("title")] public string? Title { get; set; }
    [JsonProperty("count")] public float Count { get; set; }
    [JsonProperty("price")] public float Price { get; set; }
    [JsonProperty("discount_sum")] public float DiscountSum { get; set; }
    [JsonProperty("tax")] public int Tax { get; set; }
    [JsonProperty("calculation_subject")] public string? CalculationSubject { get; set; }
    [JsonProperty("сalculationAgent")] public int CalculationAgent { get; set; }
    [JsonProperty("vendorData")] public Vendor? VendorData { get; set; }
}