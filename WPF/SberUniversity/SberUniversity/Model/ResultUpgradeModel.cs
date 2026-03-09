using Core;
using Newtonsoft.Json;

namespace SberUniversity.Model;

public class ResultUpgradeModel:ObservableObject
{
    [JsonProperty("FirstUpgradeTitle")]
    public string? FirstUpgradeTitle
    {
        get => GetOrCreate<string>(); 
        set => SetAndNotify(value);
    }

    [JsonProperty("FirstUpgradeDescription")]
    public string? FirstUpgradeDescription
    {
        get => GetOrCreate<string>(); 
        set => SetAndNotify(value);
    }

    [JsonProperty("SecondUpgradeTitle")]
    public string? SecondUpgradeTitle
    {
        get => GetOrCreate<string>(); 
        set => SetAndNotify(value);
    }

    [JsonProperty("SecondUpgradeDescription")]
    public string? SecondUpgradeDescription
    {
        get => GetOrCreate<string>(); 
        set => SetAndNotify(value);
    }

    [JsonProperty("LastUpgradeTitle")]
    public string? LastUpgradeTitle
    {
        get => GetOrCreate<string>(); 
        set => SetAndNotify(value);
    }

    [JsonProperty("LastUpgradeDescription")]
    public string? LastUpgradeDescription
    {
        get => GetOrCreate<string>(); 
        set => SetAndNotify(value);
    }
}