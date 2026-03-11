using Newtonsoft.Json;

namespace VolgaTermalBathsEnter.Models.Client.User;

public class UserDataModel
{
    public string Fio { get; set; }
    public string City { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Birthday { get; set; }
    public string ClientId { get; set; }

    [JsonIgnore]
    public bool HaveTicket { get; set;}
}