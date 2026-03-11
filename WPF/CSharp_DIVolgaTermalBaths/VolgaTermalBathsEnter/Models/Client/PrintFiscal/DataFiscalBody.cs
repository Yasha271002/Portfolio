using Newtonsoft.Json;

namespace VolgaTermalBathsEnter.Models.Client.PrintFiscal
{
    public class DataFiscalBody
    {
        [JsonProperty("data_fiscal")] public List<DataFiscal> DataFiscals { get; set; } = [];
    }

    public class DataFiscal
    {
        [JsonProperty("document_id")] public string DocumentId { get; set; }
        [JsonProperty("fiscal_number")] public string FiscalNumber { get; set; }
    }
}