using SberUniversity.Model;

namespace SberUniversity.Helpers;

public class SingletonSettings
{
    private static readonly Lazy<SingletonSettings> _instance =
        new Lazy<SingletonSettings>(() => new SingletonSettings());

    public static SingletonSettings Instance => _instance.Value;

    public SettingsModel Settings { get; set; }

    private SingletonSettings()
    { }
}