using Newtonsoft.Json;

namespace VolgaTermalBathsEnter.Models.Client.Tickets.PostPaymentTicket;

public class PaymentResult
{
    [JsonProperty("Parameters")]
    public Parameters Parameters { get; set; }

    [JsonProperty("Error")]
    public object Error { get; set; }

    [JsonProperty("Error_message")]
    public object ErrorMessage { get; set; }
}

public class Parameters
{
    [JsonProperty("Result")]
    public bool Result { get; set; }
}