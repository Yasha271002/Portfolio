using Newtonsoft.Json;

namespace VolgaTermalBathsEnter.Models
{
    public record InactivityConfig(
        [property: JsonProperty("inactivityTime")] int InactivityTime,
        [property: JsonProperty("passwordInactivityTime")] int PasswordInactivityTime);
}