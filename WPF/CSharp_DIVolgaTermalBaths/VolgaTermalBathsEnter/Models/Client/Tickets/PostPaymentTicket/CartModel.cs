using Newtonsoft.Json;

namespace VolgaTermalBathsEnter.Models.Client.Tickets.PostPaymentTicket;

public class CartModel
{
    [JsonProperty("purchase_id")] public string? PurchaseId { get; set; }
    [JsonProperty("count")] public int Count { get; set; }

    [JsonProperty("bso")]
    public List<BsoPostModel> Bso { get; set; }

}

public class BsoPostModel
{
    [JsonProperty("last_name")]
    public string LastName { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("secondname")]
    public string SecondName { get; set; }

    [JsonProperty("phone")]
    public string Phone { get; set; }
}