using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;

namespace VolgaTermalBathsEnter.Models.Client.PriceList;

public partial class ApiPriceListModel : ObservableObject
{
    [JsonProperty("ID")] public string? AppointmentId { get; set; }
    [JsonProperty("Title")] public string? Title { get; set; }
    [JsonProperty("Price")] public string? Price { get; set; }
    [JsonProperty("Kid")] public bool IsKid { get; set; }
    [JsonProperty("Tax")] public int Tax { get; set; }
    [JsonProperty("Availability")] public bool Availability { get; set; }
    public TicketTimesData? TimesData { get; set; }
    [ObservableProperty] private int _count = 0;

    private string? _comment;
    [JsonProperty("Comment")]
    public string? Comment
    {
        get => _comment;
        set
        {
            _comment = value;
            TimesData = JsonConvert.DeserializeObject<TicketTimesData>(value!);
        }
    }

    
}