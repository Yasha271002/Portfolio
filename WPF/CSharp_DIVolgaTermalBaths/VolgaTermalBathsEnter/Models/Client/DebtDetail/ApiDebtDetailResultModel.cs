using Newtonsoft.Json;

namespace VolgaTermalBathsEnter.Models.Client.DebtDetail;

public class ApiDebtDetailResultModel
{
    [JsonProperty("result")] public string? Result { get; set; }
    [JsonProperty("TotalCost")] public float TotalCost { get; set; }
    [JsonProperty("Limit")] public string? Limit { get; set; }
    [JsonProperty("data")] public List<ApiDebtDetailModel> Data { get; set; } = [];
}