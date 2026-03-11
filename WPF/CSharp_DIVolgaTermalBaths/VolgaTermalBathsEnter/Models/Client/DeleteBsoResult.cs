using Newtonsoft.Json;

namespace VolgaTermalBathsEnter.Models.Client
{
    public class DeleteBsoResult
    {
        [JsonProperty("result")] public bool Result { get; set; }
        [JsonProperty("data")] public List<Data> Datas { get; set; } = [];
    }

    public class Data
    {
        [JsonProperty("package")] public string Package { get; set; }
        [JsonProperty("print_bso")] public bool PrintBso { get; set; }
    }
}