using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.Input;
using MainComponents.Popups;
using MvvmNavigationLib.Services;
using Refit;
using Serilog;
using VolgaTermalBathsEnter.Managers;
using VolgaTermalBathsEnter.Models.Client.Bracelete;
using VolgaTermalBathsEnter.Models.Client.DataBso;
using VolgaTermalBathsEnter.Models.Client.User;
using VolgaTermalBathsEnter.Models.Client.User.Price;
using VolgaTermalBathsEnter.Models.Interfaces;
using VolgaTermalBathsEnter.Utilities;

namespace VolgaTermalBathsEnter.ViewModels.Popups;

public partial class ScanAdultBraceletViewModel(
    TotalPriceModel totalPriceModel,
    UserDataModel userDataModel,
    ILogger logger,
    IUserApi client,
    IUserCardToTicketApi cardClient,
    BraceletManager braceletManager,
    ParameterNavigationService<BraceletApplyViewModel, (TotalPriceModel, UserDataModel)> braceletApplyNavigationService,
    NavigationService<ErrorPopupViewModel> errorPopupNavigationService,
    INavigationService closeModalNavigationService) : BasePopupViewModel(closeModalNavigationService), IDisposable
{
    private async void BraceletManagerOnDataReceived(object sender, string data)
    {
        logger.Information("Start processing bracelet apply");
        var regex = new Regex(@"\b\d{3},\d{5}\b");
        var match = regex.Match(data);
        //066,34181 номер браслета 1
        //036,60837 номер браслета 2
        logger.Information("Received data: " + data);
        logger.Information("Data success: " + match.Success);
        if (!match.Success) return;
        var extractedNumber = match.Value;
        logger.Information("Extract number: " + extractedNumber);
        var braceletToClient = new ApiAddBraceleteToClient
        {
            Package = totalPriceModel.SelectedTicket.First().Package,
            Card = extractedNumber,
            Client = totalPriceModel.SelectedTicket.First().Client
        };

        logger.Information("Билеты для браслета");
        logger.Information(braceletToClient.Package);
        logger.Information("----");

        logger.Information("Тело POST запроса на привязку браслета:");
        logger.Information("Card: {Card}, Client: {Client}, Package: {Package}",
            braceletToClient.Card, braceletToClient.Client, braceletToClient.Package);

        var request = new ApiResponse();

        try
        {
            request = await cardClient.PostAddBraceleteToClient(braceletToClient);
            braceletApplyNavigationService.Navigate((totalPriceModel, userDataModel));
            logger.Information("Try закончил свою работу успешно");
        }
        catch (ApiException exception)
        {
            logger.Error($"Ошибка запроса Api: {exception.Message}");
            errorPopupNavigationService.Navigate();
        }
        catch (HttpRequestException exception)
        {
            logger.Error($"Ошибка запроса HttpRequest: {exception.Message}");
            errorPopupNavigationService.Navigate();
        }
        catch (Exception exception)
        {
            logger.Error($"Ошибка привязки билета к браслету: {exception.Message}");
            logger.Error($"Стектрейс: {exception.StackTrace}");
            if (exception.InnerException is not null)
            {
                logger.Error($"InnerException: {exception.InnerException.Message}");
                logger.Error($"InnerException`s stackTrace: {exception.InnerException.StackTrace}");

                if (exception.InnerException.InnerException is not null)
                {
                    logger.Error($"InnerException: {exception.InnerException.InnerException.Message}");
                    logger.Error($"InnerException`s stackTrace: {exception.InnerException.InnerException.StackTrace}");
                }
            }
            errorPopupNavigationService.Navigate();
        }
        finally
        {
            logger.Information($"Ответ сервера:");
            if (request is not null)
            {
                logger.Information($"result : {request.Result}");
                logger.Information($"data : {request.Data}");
            }
            else
            {
                logger.Information($"Не поступил ответ сервера");
            }
            
            logger.Information("Запрос принят");
        }
    }


    [RelayCommand]
    private void Loaded()
    {
        try
        {
            braceletManager.DataReceived -= BraceletManagerOnDataReceived;
            braceletManager.DataReceived += BraceletManagerOnDataReceived;
            logger.Information("Начало сканирования");
        }
        catch (Exception e)
        {
            logger.Error("Ошибка подключения браслетного менеджера: " + e.Message);
            errorPopupNavigationService.Navigate();
        }
    }

    [RelayCommand]
    private void Unloaded() => Dispose();

    public void Dispose()
    {
        logger.Information("Dispose");
        braceletManager.DataReceived -= BraceletManagerOnDataReceived;
        braceletManager.Dispose();
    }
}