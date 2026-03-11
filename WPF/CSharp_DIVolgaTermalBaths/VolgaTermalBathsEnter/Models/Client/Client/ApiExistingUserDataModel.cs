using Newtonsoft.Json;

namespace VolgaTermalBathsEnter.Models.Client.CreateClient;

public class ApiExistingUserDataModel
{
    [JsonProperty("status")] public string? Status { get; set; }
    [JsonProperty("client_id")] public string? ClientId { get; set; }
    [JsonProperty("cards")] public List<object>? Cards { get; set; }
}