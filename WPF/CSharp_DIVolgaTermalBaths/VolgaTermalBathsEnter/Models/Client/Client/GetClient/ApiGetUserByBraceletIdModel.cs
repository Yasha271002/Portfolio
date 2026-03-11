using Newtonsoft.Json;

namespace VolgaTermalBathsEnter.Models.Client.Client.GetClient;

public class ApiGetUserByBraceletIdModel
{
    [JsonProperty("client_id")] public string? ClientId { get; set; }
    [JsonProperty("phone")] public string? Phone { get; set; }
    [JsonProperty("name")] public string? Name { get; set; }
    [JsonProperty("second_name")] public string? SecondName { get; set; }
    [JsonProperty("last_name")] public string? LastName { get; set; }
}