using Newtonsoft.Json;

namespace VolgaTermalBathsEnter.Models.Client.DebtDetail;

public class Vendor
{
    [JsonProperty("vendorName")] public string? VendorName { get; set; }
    [JsonProperty("vendorINN")] public string? VendorINN { get; set; }
}