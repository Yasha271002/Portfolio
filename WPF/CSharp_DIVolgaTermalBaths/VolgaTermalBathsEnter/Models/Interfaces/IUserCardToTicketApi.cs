using Refit;
using VolgaTermalBathsEnter.Models.Client.Bracelete;

namespace VolgaTermalBathsEnter.Models.Interfaces;

public interface IUserCardToTicketApi
{
    [Post("/card_to_ticket")]
    Task<ApiResponse> PostAddBraceleteToClient(
        [Body] ApiAddBraceleteToClient braceleteToClient);
}