using Newtonsoft.Json;

namespace VolgaTermalBathsEnter.Models.Client.Client.Create;

public class ApiCreateClientModel
{
    [JsonProperty("phone")] public string? Phone { get; set; }
    [JsonProperty("email")] public string? Email { get; set; }
    [JsonProperty("name")] public string? Name { get; set; }
    [JsonProperty("second_name")] public string? SecondName { get; set; }
    [JsonProperty("last_name")] public string? LastName { get; set; }
    [JsonProperty("birthday")] public string? Birthday { get; set; }
    [JsonProperty("sex")] public string? Sex { get; set; }
    [JsonProperty("address")] public string? Address { get; set; }
}