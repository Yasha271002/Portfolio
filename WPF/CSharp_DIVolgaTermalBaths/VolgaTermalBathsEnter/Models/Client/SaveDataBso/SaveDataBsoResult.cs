using Newtonsoft.Json;

namespace VolgaTermalBathsEnter.Models.Client.SaveDataBso;

public class SaveDataBsoResult
{
    [JsonProperty("result")] public bool Result { get; set; }
    [JsonProperty("print_fiscal")] public bool PrintFiscal { get; set; }
    [JsonProperty("data_fiscal")] public List<DataFiscal>? DataFiscals { get; set; } = [];
}

public class DataFiscal
{
    [JsonProperty("document_id")] public string DocumentId { get; set; }
    [JsonProperty("date")] public string Date { get; set; }
    [JsonProperty("cost")] public int Cost { get; set; }
    [JsonProperty("cart")] public List<Cart>? Carts { get; set; } = [];
}

public class Cart
{
    [JsonProperty("id")] public string Id { get; set; }
    [JsonProperty("title")] public string Title { get; set; }
    [JsonProperty("purchase_serial")] public string PurchaseSerial { get; set; }
    [JsonProperty("count")] public int Count { get; set; }
    [JsonProperty("price")] public int Price { get; set; }
    [JsonProperty("discount_sum")] public int DiscountSum { get; set; }
    [JsonProperty("payment_amount")] public int PaymentAmount { get; set; }
    [JsonProperty("tax")] public int Tax { get; set; }
    [JsonProperty("calculation_subject")] public string CalculationSubject { get; set; }
}