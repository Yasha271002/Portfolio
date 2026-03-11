using Newtonsoft.Json;

namespace VolgaTermalBathsEnter.Models.Client.SaveDataBso;

public class SaveDataBsoBody
{
    [JsonProperty("data_bso")] public List<DataBso> DataBso { get; set; } = [];
}

public class DataBso
{
    [JsonProperty("package")] public string Package { get; set; }
    [JsonProperty("document_id")] public string DocumentId { get; set; }
    [JsonProperty("purchase_serial_id")] public string PurchaseSerialId { get; set; }
    [JsonProperty("last_name")] public string LastName { get; set; }
    [JsonProperty("name")] public string Name { get; set; }
    [JsonProperty("secondname")] public string SecondName { get; set; }
    [JsonProperty("phone")] public string Phone { get; set; }
}