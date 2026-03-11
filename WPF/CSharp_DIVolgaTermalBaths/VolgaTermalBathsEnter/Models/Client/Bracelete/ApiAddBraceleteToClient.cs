using Newtonsoft.Json;

namespace VolgaTermalBathsEnter.Models.Client.Bracelete;

public class ApiAddBraceleteToClient
{
    [JsonProperty("package")] public string? Package { get; set; }
    [JsonProperty("card")] public string? Card { get; set; }
    [JsonProperty("client")] public string? Client { get; set; }
}

public class ApiResponse
{
    [JsonProperty("result")]
    public bool Result { get; set; }

    [JsonProperty("data")]
    public ApiAddBraceleteToClient Data { get; set; }
}