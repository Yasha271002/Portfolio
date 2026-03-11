using Newtonsoft.Json;

namespace VolgaTermalBathsEnter.Models.Client.PriceList;

public class ApiParameters<T>
{
    [JsonProperty("Parameters")] public List<T>? Parameters { get; set; }
    public string? Error { get; set; }
    public string? ErrorMessage { get; set; }
}