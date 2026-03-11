using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace VolgaTermalBathsEnter.Models.Client.Tickets;

public partial class ApiGetTickets : ObservableObject
{
    public string? Package { get; set; }
    public string? Ticket { get; set; }
    public bool Active { get; set; }
    public string? Client { get; set; }
    public string? Key { get; set; }

    [JsonIgnore, ObservableProperty] private bool _isSelected;
}