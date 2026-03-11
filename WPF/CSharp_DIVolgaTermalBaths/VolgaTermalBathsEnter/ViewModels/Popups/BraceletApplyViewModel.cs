using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FreeKassaPayOnline.Model;
using FreeKassaPayOnline.Models;
using FreeKassaPayOnlineFramework.Models;
using MainComponents.Popups;
using MvvmNavigationLib.Services;
using Newtonsoft.Json;
using Serilog;
using System.Globalization;
using System.IO;
using VolgaTermalBathsEnter.Managers;
using VolgaTermalBathsEnter.Models;
using VolgaTermalBathsEnter.Models.Client.DataBso;
using VolgaTermalBathsEnter.Models.Client.PrintFiscal;
using VolgaTermalBathsEnter.Models.Client.SaveDataBso;
using VolgaTermalBathsEnter.Models.Client.User;
using VolgaTermalBathsEnter.Models.Client.User.Price;
using VolgaTermalBathsEnter.Models.Interfaces;
using VolgaTermalBathsEnter.Utilities;
using VolgaTermalBathsEnter.ViewModels.Pages;
using DataBso = VolgaTermalBathsEnter.Models.Client.SaveDataBso.DataBso;
using DataFiscal = VolgaTermalBathsEnter.Models.Client.SaveDataBso.DataFiscal;

namespace VolgaTermalBathsEnter.ViewModels.Popups;

public partial class BraceletApplyViewModel(
    (TotalPriceModel, UserDataModel) totalPriceClientInfoModel,
    KassaManager kassaManager,
    ILogger logger,
    IUserApi client,
    NavigationService<PaidTicketViewModel> paidTicketNavigationService,
    NavigationService<SelectedTicketPageViewModel> selectTicketNavigationService,
    NavigationService<ErrorPopupViewModel> errorPopupNavigationService,
    NavigationService<ThanksGoTurnstilePopupViewModel> goToTurnistileNavigationService,
    INavigationService modalBackNavigationService, UpdateTicketsTrigger updateTicketsTrigger) : BasePopupViewModel(modalBackNavigationService)
{
    private SaveDataBsoResult? _saveDataBsoResult = new();
    private int _sum;

    [ObservableProperty] private bool _printCheck;
    private string _checkBso = string.Empty;

    [RelayCommand]
    private void GoToEndPage() =>
        goToTurnistileNavigationService.Navigate();

    [RelayCommand]
    private async Task TicketOnOtherTime(string type)
    {
        CloseContainerCommand.Execute(false);
        await Task.Delay(100);
        if (type == "Другое")
            selectTicketNavigationService.Navigate();
        else
            updateTicketsTrigger.OnUpdateTickets();
    }

    [RelayCommand]
    private async Task Loaded()
    {
        try
        {
            PrintCheck = true;
            await SaveDataBso();
            logger.Information("PrintFiscal: " + _saveDataBsoResult.PrintFiscal);
            if (!_saveDataBsoResult.PrintFiscal)
            {
                PrintCheck = false;
                return;
            }

            _sum = _saveDataBsoResult.DataFiscals!.FirstOrDefault().Cost;
            logger.Information("Сумма _saveDataBsoResult: " + _sum);
            LogData(await Payment());
        }
        catch (Exception exception)
        {
            errorPopupNavigationService.Navigate();
            logger.Error(exception.Message);
        }
    }

    private async Task SaveDataBso()
    {
        var items = await client.RequestProcessingWithErrorHandling(
            () => client.GetDataBso(FiltersDataBsoBody()), logger, "Ошибка получения DataBso");
        logger.Information("Запрос на получение BSO произведен:");
        logger.Information("Result:" + items.Result);
        logger.Information("Data: " + JsonConvert.SerializeObject(items.Data));

        if (!items.Data!.FirstOrDefault()!.PrintBso) return;

        logger.Information("Начало печати BSO");
        _checkBso = items.Data!.FirstOrDefault()!.DataBso!.FirstOrDefault()!.CheckBso!;

        _checkBso = _checkBso.Replace("\n", string.Empty);

        var print = kassaManager.PrintText(_checkBso);
        logger.Information("Текст распечатан: " + print);

        if (items.Data!.Any(f => !f.PrintBso))
        {
            foreach (var data in items.Data!)
            {
                logger.Information(data.Package);
            }

            logger.Information("DataBso все PrintBso == false");
            return;
        }

        _saveDataBsoResult =
            await client.RequestProcessingWithErrorHandling(() => client.SaveDataBso(FilterTickets(items)), logger,
                "Ошибка SaveDataBso");
        logger.Information(
            $"SaveDataBsoResult: {JsonConvert.SerializeObject(_saveDataBsoResult)}");
    }

    private async Task<string> Payment() // фискальная печать
    {
        if (!kassaManager.CheckKktState())
        {
            errorPopupNavigationService.Navigate();
            return "Ошибка состояния Ккт: false";
        }

        var imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logo", "logo.bmp");
        if (!kassaManager.DrowImageState(imagePath))
        {
            errorPopupNavigationService.Navigate();
            return "Ошибка изображения";
        }

        var payModel = new PayModel
        {
            PaymentType = SummType.Advance,
            Sum = _sum
        };
        logger.Information("Сумма в payModel: " + payModel.Sum.ToString(CultureInfo.InvariantCulture));

        var basket = CreateBasket(_saveDataBsoResult.DataFiscals);
        var data = kassaManager.RegisterReceipt(payModel, basket, new ClientInfo() { EmailOrPhone = "+7" + totalPriceClientInfoModel.Item2.PhoneNumber });
        LogData(data.DocumentNumber.ToString());

        var printFiscal = await client.RequestProcessingWithErrorHandling(() =>
            client.PrintFiscal(CreateFiscalBody(_saveDataBsoResult.DataFiscals, data)), logger, "Ошибка PrintFiscal");
        LogData("PrintFiscal: " + printFiscal.Result);

        PrintCheck = false;

        return "Успешно";
    }

    private DataFiscalBody CreateFiscalBody(List<DataFiscal>? data, RegisterResultModel registerResult)
    {
        var fiscal = data.FirstOrDefault();
        var printFiscalBody = new DataFiscalBody();
        var dataFiscal = new Models.Client.PrintFiscal.DataFiscal()
        {
            DocumentId = fiscal.DocumentId,
            FiscalNumber = registerResult.DocumentNumber.ToString()
        };
        printFiscalBody.DataFiscals.Add(dataFiscal);


        var r = JsonConvert.SerializeObject(printFiscalBody);
        LogData(r);

        return printFiscalBody;
    }

    private List<BasketModel> CreateBasket(List<DataFiscal> data)
    {
        return
        [
            new BasketModel
            {
                Cost = data.FirstOrDefault().Cost,
                AgentInfo = null,
                Name = "Курсовка №ВТ_CI00003012 Посещение" + data.FirstOrDefault().Carts.FirstOrDefault().Title,
                Quantity = 1,
                TaxType = FilterTax(data.FirstOrDefault().Carts.FirstOrDefault()),
                PaymentTypeSign = PaymentTypeSign.FullPayment,
                PaymentItemSign = PaymentItemSign.ServiceProvided
            }
        ];
    }

    private Tax1Value FilterTax(Cart ticket)
    {
        return ticket.Tax switch
        {
            1 => Tax1Value.Vat22,
            2 => Tax1Value.Vat10,
            3 => Tax1Value.Vat180,
            4 => Tax1Value.Vat110,
            5 => Tax1Value.Vat0,
            6 => Tax1Value.No,
            7 => Tax1Value.Vat5,
            8 => Tax1Value.Vat7,
            9 => Tax1Value.Vat5105,
            10 => Tax1Value.Vat7107,
            11 => Tax1Value.Vat22,
            12 => Tax1Value.Vat22,
            _ => Tax1Value.No
        };
    }

    private DataBsoBody FiltersDataBsoBody()
    {
        var body = new DataBsoBody
        {
            Packages = []
        };

        LogData("Количество выбранных билетов" + totalPriceClientInfoModel.Item1.SelectedTicket.Count);
        foreach (var package in totalPriceClientInfoModel.Item1.SelectedTicket.Select(ticket =>
                     new Packages { Package = ticket.Package }))
        {
            body.Packages.Add(package);
        }

        return body;
    }

    private SaveDataBsoBody FilterTickets(DataBsoResult body)
    {
        var saveDataBsoBody = new SaveDataBsoBody();
        var tickets = body.Data?.Where(param => param.DataBso != null).ToList();

        foreach (var item in from ticket in tickets
                 let package = ticket.Package
                 from item in ticket.DataBso.Select(dataBso => new DataBso
                 {
                     DocumentId = dataBso.DocumentId,
                     Package = package,
                     PurchaseSerialId = dataBso.PurchaseSerialId,
                     Name = dataBso.Name,
                     SecondName = dataBso.SecondName,
                     LastName = dataBso.LastName,
                     Phone = dataBso.Phone,
                 })
                 select item)
        {
            saveDataBsoBody.DataBso.Add(item);
        }

        return saveDataBsoBody;
    }

    private void LogData(string message)
    {
        try
        {
            logger.Information(message);
        }
        catch (Exception e)
        {
            logger.Error($"Failed to log data: {e.Message}");
        }
    }
}