using Refit;
using VolgaTermalBathsEnter.Models.Client;
using VolgaTermalBathsEnter.Models.Client.CartCost;
using VolgaTermalBathsEnter.Models.Client.Client.Create;
using VolgaTermalBathsEnter.Models.Client.Client.GetClient;
using VolgaTermalBathsEnter.Models.Client.CreateClient;
using VolgaTermalBathsEnter.Models.Client.DataBso;
using VolgaTermalBathsEnter.Models.Client.DebtDetail;
using VolgaTermalBathsEnter.Models.Client.DebtPayment;
using VolgaTermalBathsEnter.Models.Client.PriceList;
using VolgaTermalBathsEnter.Models.Client.PrintFiscal;
using VolgaTermalBathsEnter.Models.Client.SaveDataBso;
using VolgaTermalBathsEnter.Models.Client.SerialTicket;
using VolgaTermalBathsEnter.Models.Client.Tickets;
using VolgaTermalBathsEnter.Models.Client.Tickets.PostPaymentTicket;

namespace VolgaTermalBathsEnter.Models.Interfaces;

public interface IUserApi
{
    [Get("/client_by_phone/?phone={phone}")]
    Task<ApiResult<ApiExistingUserDataModel>> GetUserByPhone(string? phone);

    //[Get("/price_list_v2/?client_id={clientId}&startDate={date}")]
    //Task<ApiParameters<ApiPriceListModel>> GetPriceList(string clientId, string date);
    [Get("/price_list_v2/?client_id={clientId}&date={date}")]
    Task<ApiParameters<ApiPriceListModel>> GetPriceList(string clientId, string date);

    [Get("/cart_cost/?client_id={clientId}&cart={cart}")]
    Task<ApiParameters<ApiGetCartCost>> GetCartCost(string clientId, string cart);

    [Get("/Get_tickets_v2/?phone={phoneNumber}")]
    Task<ApiParameters<ApiGetTickets>> Get_Tickets(string phoneNumber);

    [Post("/client")]
    Task<ApiResult<ApiCreateClientModel>> PostCreateUser(ApiCreateClientModel api);

    [Post("/debt_payment")]
    Task<ResultDebt> DebtPayment(DebtPaymentBody payment);

    [Post("/payment")]
    Task<PaymentResult> PostPaymentTicket([Body] ApiPostPaymentModel apiPostPaymentModel);

    [Get("/client_by_card/?card={braceletId}")]
    Task<ApiResult<ApiGetUserByBraceletIdModel>> GetUserByBraceletId(string braceletId);

    [Get("/debt_detail/?client_id={client_id}")]
    Task<ApiDebtDetailResultModel> GetDebtDetail(string client_id);

    [Get("/serial_to_ticket/?transaction_id={transactionId} ")]
    Task<SerialTicket> GetSerialTicket(string transactionId);

    [Post("/data_bso")]
    Task<DataBsoResult> GetDataBso([Body] DataBsoBody body);

    [Post("/save_data_bso")]
    Task<SaveDataBsoResult> SaveDataBso([Body] SaveDataBsoBody body);

    [Post("/print_fiscal")]
    Task<DataFiscalResult> PrintFiscal([Body] DataFiscalBody body);

    [Get("/delete_bso")]
    Task<DeleteBsoResult> Test_Delete_bso([Body] DataBsoBody body);
}