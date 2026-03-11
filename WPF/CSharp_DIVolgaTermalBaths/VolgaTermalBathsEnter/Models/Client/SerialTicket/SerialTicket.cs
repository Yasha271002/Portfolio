using Newtonsoft.Json;

namespace VolgaTermalBathsEnter.Models.Client.SerialTicket;

public class SerialTicket
{
    [JsonProperty("result")] public bool Result { get; set; }
    [JsonProperty("data")] public List<Serial> Data { get; set; } = [];
}