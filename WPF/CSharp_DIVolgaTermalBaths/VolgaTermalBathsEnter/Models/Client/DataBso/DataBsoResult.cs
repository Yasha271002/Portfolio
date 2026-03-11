using Newtonsoft.Json;

namespace VolgaTermalBathsEnter.Models.Client.DataBso
{
    public class DataBsoResult
    {
        [JsonProperty("result")] public bool Result { get; set; }

        [JsonProperty("data")]
        public List<Data>? Data { get; set; }
    }

    public class Data
    {
        [JsonProperty("package")] public string Package { get; set; }
        [JsonProperty("print_bso")] public bool PrintBso { get; set; }
        [JsonProperty("data_bso")] public List<DataBso>? DataBso { get; set; } = [];
    }

    public class DataBso
    {
        [JsonProperty("document_id")] public string? DocumentId { get; set; }
        [JsonProperty("purchase_serial_id")] public string? PurchaseSerialId { get; set; }
        [JsonProperty("purchase_serial")] public string? PurchaseSerial { get; set; }
        [JsonProperty("purchase_name")] public string? PurchaseName { get; set; }
        [JsonProperty("count")] public int Count { get; set; }
        [JsonProperty("payment_amount")] public int PaymentAmount { get; set; }
        [JsonProperty("tax")] public int Tax { get; set; }
        [JsonProperty("start_date")] public string? StartDate { get; set; }
        [JsonProperty("finish_date")] public string? FinishDate { get; set; }
        [JsonProperty("last_name")] public string? LastName { get; set; }
        [JsonProperty("name")] public string? Name { get; set; }
        [JsonProperty("secondname")] public string? SecondName { get; set; }
        [JsonProperty("phone")] public string? Phone { get; set; }
        [JsonProperty("check_bso")] public string? CheckBso { get; set; }
    }
}