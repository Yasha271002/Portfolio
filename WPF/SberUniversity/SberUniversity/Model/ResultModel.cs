using Newtonsoft.Json;
using ObservableObject = Core.ObservableObject;

namespace SberUniversity.Model;

public class ResultModel : ObservableObject
{
    [JsonProperty("PointsCount")]
    public int PointsCount
    {
        get => GetOrCreate<int>();
        set => SetAndNotify(value);
    }

    [JsonProperty("ResultTitle")]
    public string? ResultTitle
    {
        get => GetOrCreate<string>();
        set => SetAndNotify(value);
    }

    [JsonProperty("Description")]
    public string? Description
    {
        get => GetOrCreate<string>();
        set => SetAndNotify(value);
    }
    [JsonProperty("Description2")]
    public string? Description2
    {
        get => GetOrCreate<string>();
        set => SetAndNotify(value);
    }

    [JsonProperty("ResultUpgrade")]
    public ResultUpgradeModel ResultUpgrade
    {
        get => GetOrCreate<ResultUpgradeModel>();
        set => SetAndNotify(value);
    }
    
    [JsonProperty("Links")]
    public List<string> Links
    {
        get => GetOrCreate<List<string>>();
        set => SetAndNotify(value);
    }
    
    [JsonProperty("Words")]
    public List<string> Words
    {
        get => GetOrCreate<List<string>>();
        set => SetAndNotify(value);
    }

    [JsonIgnore]
    public string? EMail
    {
        get => GetOrCreate<string?>();
        set => SetAndNotify(value);
    }

    [JsonIgnore]
    public bool IsAgreePersonalData
    {
        get => GetOrCreate<bool>();
        set => SetAndNotify(value);
    }
}