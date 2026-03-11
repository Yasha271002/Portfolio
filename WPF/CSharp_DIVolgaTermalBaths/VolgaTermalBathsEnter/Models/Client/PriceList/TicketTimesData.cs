using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace VolgaTermalBathsEnter.Models.Client.PriceList;

public partial class TicketTimesData : ObservableObject
{
    [JsonProperty("duration")] public string? Duration { get; set; }
    [JsonProperty("time")] public List<TicketDate>? Time { get; set; } = [];
}