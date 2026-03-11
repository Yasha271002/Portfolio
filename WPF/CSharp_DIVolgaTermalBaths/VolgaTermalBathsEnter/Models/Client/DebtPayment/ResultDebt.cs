using Newtonsoft.Json;

namespace VolgaTermalBathsEnter.Models.Client.DebtPayment;

public class ResultDebt
{
    [JsonProperty("result")] public bool Result { get; set; }
    [JsonProperty("sum")] public float Sum { get; set; }
}