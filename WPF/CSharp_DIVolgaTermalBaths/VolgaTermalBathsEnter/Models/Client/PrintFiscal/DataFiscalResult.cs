using Newtonsoft.Json;

namespace VolgaTermalBathsEnter.Models.Client.PrintFiscal;

public class DataFiscalResult
{
    [JsonProperty("result")] public bool Result { get; set; }
}