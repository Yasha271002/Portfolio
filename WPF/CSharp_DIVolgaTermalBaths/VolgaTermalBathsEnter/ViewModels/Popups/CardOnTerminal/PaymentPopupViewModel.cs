using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FreeKassaPayOnline.Model;
using FreeKassaPayOnline.Models;
using FreeKassaPayOnlineFramework.Models;
using MainComponents.Popups;
using MvvmNavigationLib.Services;
using Newtonsoft.Json;
using Serilog;
using System.Collections.ObjectModel;
using System.IO;
using VolgaTermalBathsEnter.Managers;
using VolgaTermalBathsEnter.Models.Client.DebtPayment;
using VolgaTermalBathsEnter.Models.Client.Tickets.PostPaymentTicket;
using VolgaTermalBathsEnter.Models.Client.User;
using VolgaTermalBathsEnter.Models.Client.User.Price;
using VolgaTermalBathsEnter.Models.Client.User.Ticket;
using VolgaTermalBathsEnter.Models.Interfaces;
using VolgaTermalBathsEnter.Utilities;
using VolgaTermalBathsEnter.ViewModels.Pages;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace VolgaTermalBathsEnter.ViewModels.Popups.CardOnTerminal;

public partial class PaymentPopupViewModel(
    ObservableCollection<TicketModel> ticketModels,
    TotalPriceModel totalPriceModel,
    UserDataModel userDataModel,
    KassaManager kassaManager,
    ILogger logger,
    IUserApi client,
    NavigationService<ErrorPopupViewModel> errorPopupNavigationService,
    NavigationService<PaymentSuccessfulPageViewModel> paymentSuccessfulNavigationService,
    INavigationService closeNavigationService)
    : BasePopupViewModel(closeNavigationService)
{
    [ObservableProperty] private TotalPriceModel _totalPriceModel = totalPriceModel;
    private List<BasketModel> _cart = [];
    private readonly string _imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logo", "logo.bmp");


    private async Task<bool> Payment()
    {
        foreach (var ticketModel in ticketModels)
        {
            _cart.Add(new BasketModel() //Список позиций
            {
                Cost = ticketModel.Price * 100,
                Name = "Курсовка №ВТ_CI00003012\nПосещение " + ticketModel.Title!,
                Quantity = ticketModel.Count,
                AgentInfo = null,
                TaxType = ticketModel.Tax.IntToTax1Value()
            });
        }

        var sum = ticketModels.Sum(ticketModel => ticketModel.Price) * 100;
        logger.Information("Оплата на сбер: " + sum);

        var request = kassaManager.CheckKktState();
        if (!request)
        {
            errorPopupNavigationService.ErrorMethod(
                $"Check KKT is false \n Сумма корзины: {sum} \n Итоговая сумма в cart: {_cart.Sum(price => price.Cost)} \n Итоговая сумма в TicketModel: {ticketModels.Sum(price => price.Price) * 100}",
                logger);
            return false;
        }

        kassaManager.OnPaymentResult += KassaManagerOnOnPaymentResult;
        kassaManager.MakePayment(sum);

        return true;
    }

    private async Task<bool> PaymentMethod(string rrn)
    {
        var cart = new List<CartModel>();
        var paymentList = new List<PaymentListModel> { new() { Amount = ticketModels.Sum(t => t.Price), Type = "card" } };
        foreach (var ticketModel in ticketModels)
        {
            cart.Add(new CartModel
            {
                PurchaseId = ticketModel.PurchaseId,
                Count = ticketModel.Count,
                Bso =
                [
                    new BsoPostModel
                    {
                        LastName = ticketModel.TicketPersonalInfo.LastName,
                        Name = ticketModel.TicketPersonalInfo.FirstName,
                        SecondName = ticketModel.TicketPersonalInfo.SecondName,
                        Phone = "7" + userDataModel.PhoneNumber
                    }
                ]
            });
        }

        var paymentModel = new ApiPostPaymentModel()
        {
            ClientId = userDataModel.ClientId,
            TransactionId = rrn,
            Cart = cart,
            PaymentList = paymentList
        };

        var response = await client.RequestProcessingWithErrorHandling(() => client.PostPaymentTicket(paymentModel),
            logger,
            "Ошибка оплаты билетов");

        if (response.ErrorMessage != null)
            logger.Error(response.ErrorMessage.ToString()!);

        return response.Parameters.Result;
    }

    [RelayCommand]
    private async Task Loaded()
    {
        logger.Information($"TicketModels:\n{JsonConvert.SerializeObject(ticketModels)}");

        await Payment().MethodProcessingWithErrorHandling(
            logger,
            errorPopupNavigationService,
            "Ошибка метода Payment");
    }

    [RelayCommand]
    private void Unloaded() => kassaManager.OnPaymentResult -= KassaManagerOnOnPaymentResult;


    private async void KassaManagerOnOnPaymentResult(bool isSuccess)
    {
        try
        {
            if (isSuccess)
            {
                var image = kassaManager.DrowImageState(_imagePath);
                if (!image)
                {
                    errorPopupNavigationService.ErrorMethod("Ошибка изображения", logger);
                    return;
                }

                var lastPayment = await kassaManager.LastPaymentInfo(); //данные последней транзакции
                if (lastPayment is not SberPayModel sberPayModel)
                {
                    errorPopupNavigationService.ErrorMethod("Ошибка получение последнего платежа", logger);
                    return;
                }

                var rrn = Convert.ToInt64(sberPayModel.RRN); // получаем RRN
                logger.Information("RRN: " + sberPayModel.RRN);
                if (!string.IsNullOrEmpty(sberPayModel.ErrorData))
                    logger.Error("ErrorData: " + sberPayModel.ErrorData);

                var paymentRequest = await PaymentMethod(rrn.ToString()) // отправляем данные об оплате
                    .MethodProcessingWithErrorHandling(logger, errorPopupNavigationService,
                        "Ошибка PaymentMethod");

                logger.Information("PaymentMethod: " + paymentRequest);

                if (!paymentRequest)
                {
                    errorPopupNavigationService.ErrorMethod("PaymentRequest is false", logger);
                    return;
                }

                var purchase = await client.RequestProcessingWithErrorHandling(
                    () => client.GetSerialTicket(sberPayModel.RRN), logger,
                    "Ошибка получения SerialTicket"); //запрос серийных номеров для пробития в чеке

                #region Печать чека

                _cart.Clear();

                foreach (var ticketModel in ticketModels)
                {
                    var serial = purchase.Data.FirstOrDefault(p => p.PurchaseId == ticketModel.PurchaseId);
                    if (serial == null)
                    {
                        errorPopupNavigationService.ErrorMethod("Нет необходимого серийного номера", logger);
                    }

                    _cart.Add(new BasketModel()
                    {
                        Cost = ticketModel.Price,
                        Name = "Курсовка " + serial!.PurchaseSerial + "\nПосещение " + ticketModel.Title!,
                        Quantity = ticketModel.Count,
                        AgentInfo = null,
                        PaymentTypeSign = PaymentTypeSign.Prepayment, //1
                        PaymentItemSign = PaymentItemSign.AdvancePayment, //10
                        TaxType = ticketModel.Tax.IntToTax1Value()
                    });

                    purchase.Data.Remove(serial!);
                }

                logger.Information("Cart: " + JsonConvert.SerializeObject(_cart));

                var sum = ticketModels.Sum(ticketModel => ticketModel.Price);
                var payModel = new PayModel //модель оплаты
                {
                    PaymentType = SummType.ElectronicallyMir,
                    Sum = sum
                };

                logger.Information("Модель оплаты: " + JsonConvert.SerializeObject(payModel));

                var ready = kassaManager.RegisterReceipt(payModel, _cart,
                    new ClientInfo() { EmailOrPhone = "+7" + userDataModel.PhoneNumber }); //печать фискальника
                if (ready.IsSucces)
                {
                    closeNavigationService.Navigate();
                    paymentSuccessfulNavigationService.Navigate();
                    return;
                }

                errorPopupNavigationService.ErrorMethod("Чек не зарегистрирован", logger);

                #endregion
            }
            else
            {
                errorPopupNavigationService.ErrorMethod("isSuccess is false", logger);
            }
        }
        catch (Exception e)
        {
            errorPopupNavigationService.ErrorMethod($"Ошибка KassaManagerOnPaymentResult: {e.Message}: {e.StackTrace}",
                logger);
        }
    }
}