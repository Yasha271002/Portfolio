using Newtonsoft.Json;

namespace VolgaTermalBathsEnter.Models.Client;

public class ApiResult<T>
{
    [JsonProperty("result")] public bool Result { get; set; }
    [JsonProperty("data")] public T? Data { get; set; }
}