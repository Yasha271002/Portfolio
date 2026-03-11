using Newtonsoft.Json;

namespace VolgaTermalBathsEnter.Models.Client.DebtDetail;

public class ApiDebtDetailModel
{
    [JsonProperty("document_id")] public string? DocumentId { get; set; }
    [JsonProperty("date")] public string? Date { get; set; }
    [JsonProperty("operation")] public string? Operation { get; set; }
    [JsonProperty("cost")] public float Cost { get; set; }
    [JsonProperty("cart")] public List<ApiTicketModel> Cart { get; set; } = [];
}