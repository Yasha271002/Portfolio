using Newtonsoft.Json;

namespace SberUniversity.Model;

public class SettingsModel
{
    [JsonProperty("From")] public string? From { get; set; } = string.Empty;
    [JsonProperty("Password")] public string? Password { get; set; } = string.Empty;
    [JsonProperty("Host")] public string? Host { get; set; } = string.Empty;
    [JsonProperty("ImapHost")] public string? ImapHost { get; set; } = string.Empty;
    [JsonProperty("Quizzes")] public string? QuizzesSettingsPaths { get; set; } = "Quizzes/QuizzesSettings.Json";

    [JsonProperty("Port")] public int Port { get; set; }
    [JsonProperty("ImapPort")] public int ImapPort { get; set; }
    [JsonProperty("InactivityTime")] public int InactivityTime { get; set; }

    [JsonProperty("UserAgreement")] public string? UserAgreement { get; set; } = "";
    [JsonProperty("SberUniversity")] public string? SberUniversity { get; set; } = "";
}