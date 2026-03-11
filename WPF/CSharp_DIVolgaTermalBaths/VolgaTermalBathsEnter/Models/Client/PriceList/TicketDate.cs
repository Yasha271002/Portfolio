using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace VolgaTermalBathsEnter.Models.Client.PriceList;

public partial class TicketDate : ObservableObject
{
    [JsonProperty("name")] public string? Name { get; set; }
    [JsonProperty("value")] public string? Value { get; set; }

    [JsonIgnore, ObservableProperty] private bool _isEnabled = true;

    [JsonIgnore, ObservableProperty] private bool _isSelected;
}