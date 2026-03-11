using Newtonsoft.Json;

namespace VolgaTermalBathsEnter.Models.Client.Settings.HostSettings;

public class HostSettings
{
    [JsonProperty("terminalId")] public string TerminalId {get; set; }
    [JsonProperty("apiKey")] public string ApiKey { get; set; }
    [JsonProperty("authKey")] public string AuthKey { get; set; }
    [JsonProperty("cardAuthKey")] public string CardAuthKey { get; set; }
    [JsonProperty("host")] public string Host { get; set; }
    [JsonProperty("cardHost")] public string CardHost { get; set; }
}