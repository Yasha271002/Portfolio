using FreeKassaPayOnline.Model;
using FreeKassaPayOnline.Models;
using FreeKassaPayOnlineFramework;
using FreeKassaPayOnlineFramework.Enums;
using FreeKassaPayOnlineFramework.Models;
using Serilog;

namespace VolgaTermalBathsEnter.Managers;

public class KassaManager
{
    private readonly ILogger _logger;
    private KktManager _kassaManager;

    public delegate void PaymentResult(bool isSuccess);

    public event PaymentResult? OnPaymentResult;

    public KassaManager(ILogger logger)
    {
        this._logger = logger;

        _kassaManager = new KktManager();
        _kassaManager.Error += KassaOnError;
        _kassaManager.Successfully += KassaManagerOnSuccessfully;
        _kassaManager.PartPayment += KassaManagerOnPartPayment;
        _kassaManager.ResultPayment += KassaManagerOnResultPaymentPayment;
        _kassaManager.ConnectKassa();

        logger.Information("[KASSA] kassaManager initialized");
    }

    public void MakePayment(long sum, PaymentType paymentType = PaymentType.SberbankSbrf) =>
        _kassaManager.StartPayment(paymentType, sum);


    public bool CheckKktState() =>
        _kassaManager.CheckCurrentKktState();


    public bool DrowImageState(string _imagePath) =>
        _kassaManager.PrintImage(_imagePath);

    public bool PrintText(string text)
    {
        _logger.Information($"Печать текста: {text}");
        var print = _kassaManager.PrintString(text);
        if (!print)
        {
            _logger.Error("Ошибка печати текста");
            return false;
        }
        _kassaManager.CutAndFeedCheck();
        return true;
    }

    public RegisterResultModel RegisterReceipt(PayModel payModel, List<BasketModel> basketModels, ClientInfo clientInfo) =>
        _kassaManager.RegisterReceipt(payModel, basketModels, clientInfo);


    private void KassaOnError(int errorcode, string? errordescription, bool isshow) =>
        _logger.Error($"KassaManager: {errordescription} - [{errorcode}]");


    private void KassaManagerOnSuccessfully(int errorcode, string? errordescription, bool isshow) =>
        _logger.Information("KassaManager: Чек успешно зарегистрирован");


    private void KassaManagerOnPartPayment(int sum) =>
        _logger.Information($"KassaManager: Зачислено {sum} рублей");


    public async Task<object> LastPaymentInfo() =>
        await _kassaManager.LastPaymentInfo() ?? throw new InvalidOperationException();

    private void KassaManagerOnResultPaymentPayment(int errorCode)
    {
        if (OnPaymentResult == null) return;
        if (errorCode == 0)
        {
            OnPaymentResult(true);
        }
        else
        {
            OnPaymentResult(false);
            _logger.Error($"KassaManager; payment result: {errorCode}");
        }
    }
}