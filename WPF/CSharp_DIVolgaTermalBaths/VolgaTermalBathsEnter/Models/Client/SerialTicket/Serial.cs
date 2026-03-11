using Newtonsoft.Json;

namespace VolgaTermalBathsEnter.Models.Client.SerialTicket;

public class Serial
{
    [JsonProperty("purchase_id")] public string PurchaseId { get; set; }
    [JsonProperty("purchase_serial")] public string PurchaseSerial { get; set; }
}