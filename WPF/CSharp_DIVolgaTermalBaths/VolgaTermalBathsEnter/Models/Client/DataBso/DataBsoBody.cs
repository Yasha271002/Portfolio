using Newtonsoft.Json;

namespace VolgaTermalBathsEnter.Models.Client.DataBso;

public class DataBsoBody
{
    [JsonProperty("packages")] public List<Packages> Packages { get; set; } = [];
}

public class Packages
{
    [JsonProperty("package")] public string? Package { get; set; }
}