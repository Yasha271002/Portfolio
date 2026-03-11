using FreeKassaPayOnline.Model;
using MvvmNavigationLib.Services;
using Refit;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Text.RegularExpressions;
using Serilog;
using VolgaTermalBathsEnter.Models.Interfaces;
using VolgaTermalBathsEnter.ViewModels.Popups;

namespace VolgaTermalBathsEnter.Utilities;

public static class CollectionStaticMethods
{
    public static Tax1Value IntToTax1Value(this int taxValue)
    {
        return taxValue switch
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

    public static async Task<T> MethodProcessingWithErrorHandling<T>(
        this Task<T> methodCall,
        ILogger logger,
        NavigationService<ErrorPopupViewModel> errorPopupNavigationService,
        string errorMessage)
    {
        try
        {
            return await methodCall;
        }
        catch (Exception exception)
        {
            logger.Error($"{errorMessage}: {exception.Message} : {exception.StackTrace}");
            errorPopupNavigationService.Navigate();
            throw;
        }
    }

    public static async Task<T> RequestProcessingWithErrorHandling<T>(
        this IUserApi client,
        Func<Task<T>> apiCall,
        ILogger logger,
        string errorMessage)
    {
        try
        {
            return await apiCall();
        }
        catch (ApiException exception)
        {
            logger.Error($"Ошибка запроса: {exception.Message}");
            return default;
        }
        catch (Exception exception)
        {
            logger.Error($"{errorMessage}: {exception.Message} : {exception.StackTrace}");
            return default;
        }
    }

    public static async Task<T> RequestProcessingWithErrorHandling<T>(
        this IUserCardToTicketApi client,
        Func<Task<T>> apiCall,
        ILogger logger,
        string errorMessage)
    {
        try
        {
            return await apiCall();
        }
        catch (ApiException exception)
        {
            logger.Error($"Ошибка запроса Api: {exception.Message}");
            return default;
        }
        catch (HttpRequestException exception)
        {
            logger.Error($"Ошибка запроса HtppRequest: {exception.Message}");
            return default;
        }
        catch (Exception exception)
        {
            logger.Error($"{errorMessage}: {exception.Message}");
            return default;
        }
    }

    public static void ErrorMethod(this NavigationService<ErrorPopupViewModel> errorPopupNavigationService, string message,
        ILogger logger)
    {
        logger.Error(message);
        errorPopupNavigationService.Navigate();
    }

    #region UserRegisterViewModel

    public static ValidationResult ValidateFio(string fio, ValidationContext context)
    {
        if (string.IsNullOrWhiteSpace(fio))
            return new ValidationResult("ФИО обязательно для заполнения");

        // Разрешаем любые буквы, проверяем только структуру
        if (!Regex.IsMatch(fio, @"^[А-Яа-яЁё]+(-[А-Яа-яЁё]+)?\s[А-Яа-яЁё]+(?:\s[А-Яа-яЁё]+)?$"))
            return new ValidationResult("Требуется формат: Фамилия Имя Отчество");

        return ValidationResult.Success;
    }

    public static ValidationResult ValidateBirthday(string birthday, ValidationContext context)
    {
        if (birthday.Length < 5)
            return !Regex.IsMatch(birthday, @"^(0[1-9]|[12][0-9]|3[01])[-/.](0[1-9]|1[012])[-/.](19|20)\d\d$")
                ? new ValidationResult("Требуется формат: 01.01.2000")
                : ValidationResult.Success;
        var day = birthday.Substring(0, 2);
        var month = birthday.Substring(3, 2);
        if (Convert.ToInt32(day) > 31 || Convert.ToInt32(month) > 12)
            return new ValidationResult("Неверная дата рождения");

        return !Regex.IsMatch(birthday, @"^(0[1-9]|[12][0-9]|3[01])[-/.](0[1-9]|1[012])[-/.](19|20)\d\d$")
            ? new ValidationResult("Требуется формат: 01.01.2000")
            : ValidationResult.Success;
    }

    public static ValidationResult ValidateCity(string city, ValidationContext context) =>
        !Regex.IsMatch(city, @"^[А-Я][а-я]+") ? new ValidationResult("Неверный формат") : ValidationResult.Success;

    public static ValidationResult ValidateEmail(string email, ValidationContext context) =>
        !Regex.IsMatch(email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$")
            ? new ValidationResult("Неверный формат")
            : ValidationResult.Success;

    #endregion

    #region SelectedTicketPageViewModel

    public static bool HasSpecialTicket(string? title) =>
        title != null && Regex.IsMatch(title, "Специальный", RegexOptions.IgnoreCase);

    public static bool HasValidDuration(string? title, string[] durations) =>
        title != null && durations.Any(d => Regex.IsMatch(title, d, RegexOptions.IgnoreCase));

    public static bool IsValidDayType(string? title, bool isWeekend) =>
        title != null &&
        (Regex.IsMatch(title, "будни", RegexOptions.IgnoreCase) && !isWeekend ||
         Regex.IsMatch(title, "выходной", RegexOptions.IgnoreCase) && isWeekend);

    public static string GetDurationKey(string? title, string[] durations) =>
        title != null
            ? durations.FirstOrDefault(d => Regex.IsMatch(title, d, RegexOptions.IgnoreCase)) ?? string.Empty
            : string.Empty;

    public static string FormatKey(string? key) => key switch
    {
        "безлимит" => "Безлимит",
        "4часа" => "4 часа",
        "3часа" => "3 часа",
        "2часа" => "2 часа",
        _ => key ?? string.Empty
    };

    #endregion

}