using Newtonsoft.Json;

namespace VolgaTermalBathsEnter.Models.Client.Settings.SmsSettings;

public class SmsApiModel
{
    [JsonProperty("host")] public string Host { get; set; }
    [JsonProperty("apiKey")] public string ApiKey { get; set; }
    [JsonProperty("senderName")] public string SenderName { get; set; }
}