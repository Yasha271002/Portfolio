using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FreeKassaPayOnline.Models;
using MvvmNavigationLib.Services;
using Newtonsoft.Json;
using Serilog;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using VolgaTermalBathsEnter.Models.Client.Client.Create;
using VolgaTermalBathsEnter.Models.Client.User;
using VolgaTermalBathsEnter.Models.Interfaces;
using VolgaTermalBathsEnter.Utilities;
using VolgaTermalBathsEnter.ViewModels.Popups;
using CollectionExtensions = VolgaTermalBathsEnter.Utilities.CollectionExtensions;

namespace VolgaTermalBathsEnter.ViewModels.Pages;

public partial class UserRegisterViewModel(
    ILogger logger,
    IUserApi client,
    UserDataModel user,
    NavigationService<WelcomeUserViewModel> welcomePageNavigationService,
    NavigationService<SelectedTicketPageViewModel> selectedTicketPageNavigationService,
    NavigationService<ErrorPopupViewModel> errorPopupNavigationService) : ObservableValidator
{
    [ObservableProperty] private bool _canContinue;
    [ObservableProperty] private bool _isDigit;
    [ObservableProperty] private bool _shiftPressed;
    [ObservableProperty] private bool _isEmailPrinting;
    [ObservableProperty] private string _language;

    // ФИО
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Поле обязательно для заполнения")]
    [CustomValidation(typeof(UserRegisterViewModel), nameof(ValidateFio))]
    private string _fio;

    private bool _isFormattingFio = false;

    partial void OnFioChanged(string value)
    {
        if (_isFormattingFio) return;

        try
        {
            _isFormattingFio = true;
            if (!string.IsNullOrEmpty(value)) ShiftPressed = value.Last() == ' ';
            else ShiftPressed = true;
        }
        finally
        {
            _isFormattingFio = false;
            UpdateCanRegister();
        }
    }

    // День рождения
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Поле обязательно для заполнения")]
    [CustomValidation(typeof(UserRegisterViewModel), nameof(ValidateBirthday))]
    private string _birthday;
    [ObservableProperty] private bool _isBirthdayPopupOpen;
    [ObservableProperty] private string _birthdayFromCalendar;

    partial void OnBirthdayChanged(string value)
    {
        UpdateCanRegister();
        IsBirthdayPopupOpen = false;
    }

    partial void OnBirthdayFromCalendarChanged(string value)
    {
        var result = value.Split(" ").First().Split("/");
        for (int i = 0; i < result.Length; i++)
        {
            if (result[i].Length == 1)
                result[i] = "0" + result[i];
        }

        Birthday = result[1] + "-" + result[0] + "-" + result[2];
    }

    // Город
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Поле обязательно для заполнения")]
    [CustomValidation(typeof(UserRegisterViewModel), nameof(ValidateCity))]
    private string _city = "Город";

    [ObservableProperty] private ObservableCollection<string>? _cities;
    [ObservableProperty] private bool _isCitySelected = false;

    [RelayCommand]
    private void ClearCity()
    {
        City = null;
        City = "Город";
        IsCitySelected = false;
    }

    partial void OnCityChanged(string value)
    {
        if (City != "Город")
            IsCitySelected = true;
        UpdateCanRegister();
    }

    // Email
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [CustomValidation(typeof(UserRegisterViewModel), nameof(ValidateEmail))]
    private string _email;

    partial void OnEmailChanged(string value)
    {
        UpdateCanRegister();
    }

    [RelayCommand]
    private void Loaded()
    {
        var json = File.ReadAllText("cities.txt");
        Cities = new ObservableCollection<string>( JsonConvert.DeserializeObject<List<string>>(json)!);
        Cities.Add("Другое");
        ErrorsChanged += (s, e) => UpdateCanRegister();
        CanContinue = false;
    }

    [RelayCommand]
    private async Task Register() => await CreateClient();

    [RelayCommand]
    private void WelcomePageNavigate() => welcomePageNavigationService.Navigate();
    private void UpdateCanRegister()
    {
        CanContinue = !HasErrors &&
                      !string.IsNullOrWhiteSpace(Fio) &&
                      !string.IsNullOrWhiteSpace(Birthday) &&
                      City != "Город";
    }

    // Методы форматирования
    private static string FormatFio(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        var culture = new CultureInfo("ru-RU");
        var textInfo = culture.TextInfo;
        return textInfo.ToTitleCase(input.ToLower(culture));
    }

    private static string FormatCity(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        var culture = new CultureInfo("ru-RU");
        var textInfo = culture.TextInfo;
        return textInfo.ToTitleCase(input.ToLower(culture));
    }

    // Методы валидации
    public static ValidationResult ValidateFio(string fio, ValidationContext context)
    {
        if (string.IsNullOrWhiteSpace(fio))
            return new ValidationResult("ФИО обязательно для заполнения");

        // Проверяем структуру (количество слов)
        var parts = fio.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2 || parts.Length > 3)
            return new ValidationResult("Требуется формат: Фамилия Имя Отчество");

        // Проверяем, что каждая часть состоит только из букв
        foreach (var part in parts)
        {
            if (!Regex.IsMatch(part, @"^[А-Яа-яЁё-]+$"))
                return new ValidationResult("ФИО может содержать только буквы и дефисы");
        }

        return ValidationResult.Success;
    }

    public static ValidationResult ValidateBirthday(string birthday, ValidationContext context)
    {
        if (string.IsNullOrWhiteSpace(birthday))
            return new ValidationResult("Дата рождения обязательна");

        // Проверяем формат дд.мм.гггг
        if (!Regex.IsMatch(birthday, @"^\d{2}\-\d{2}\-\d{4}$"))
            return new ValidationResult("Требуется формат: дд-мм-гггг");

        // Проверяем, что дата валидна
        if (!DateTime.TryParseExact(birthday, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            return new ValidationResult("Некорректная дата");

        return ValidationResult.Success;
    }

    public static ValidationResult ValidateCity(string city, ValidationContext context)
    {
        if (string.IsNullOrWhiteSpace(city) || city == "Город")
            return new ValidationResult("Город обязателен для заполнения");

        // Проверяем, что город состоит только из букв, пробелов и дефисов
        if (!Regex.IsMatch(city, @"^[А-Яа-яЁё\s-]+$"))
            return new ValidationResult("Город может содержать только буквы, пробелы и дефисы");

        return ValidationResult.Success;
    }

    public static ValidationResult ValidateEmail(string email, ValidationContext context)
    {
        // Проверяем базовый формат email
        if (!string.IsNullOrEmpty(email) && !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            return new ValidationResult("Некорректный формат email");

        return ValidationResult.Success;
    }

    private async Task CreateClient()
    {
        var createClientModel = new ApiCreateClientModel
        {
            Phone = user.PhoneNumber,
            Name = ExtractName(Fio),
            SecondName = ExtractSecondName(Fio),
            LastName = ExtractLastName(Fio),
            Email = Email,
            Birthday = ExtractBirthday(Birthday),
            Address = City,
            Sex = "null"
        };


        var responseCreateClient = await client.RequestProcessingWithErrorHandling(
            () => client.PostCreateUser(createClientModel), logger, "Ошибка создания пользователя");

        var responseGetClient = await client.RequestProcessingWithErrorHandling(
            () => client.GetUserByPhone(user.PhoneNumber), logger, "Ошибка получения пользователя");

        user.ClientId = responseGetClient.Data.ClientId;
        user.HaveTicket = false;

        if (responseGetClient is not null) selectedTicketPageNavigationService.Navigate();
        else errorPopupNavigationService.Navigate();
    }

    [RelayCommand]
    private void LostShiftFocus()
    {
        ShiftPressed = false;
    }

    [RelayCommand]
    private void GotFioFocus()
    {
        IsBirthdayPopupOpen = false;
        UpdateKeyboard(false, false, "Ru");
        OnFioChanged(Fio);
    }

    [RelayCommand]
    private void GotBirthdayFocus()
    {
        UpdateKeyboard(true, false, "Ru");
        IsBirthdayPopupOpen = true;
    }


    [RelayCommand]
    private void GotCityFocus()
    {
        IsBirthdayPopupOpen = false;
        //UpdateKeyboard(false, false, "Ru");
        //OnCityChanged(City);
    }

    [RelayCommand]
    private void GotEmailFocus()
    {
        IsBirthdayPopupOpen = false;
        UpdateKeyboard(false, true, "Eng");
    }

    private string ExtractName(string fio)
    {
        var parts = fio.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length >= 2 ? parts[1] : string.Empty; // Имя - второй элемент
    }

    private string ExtractSecondName(string fio)
    {
        var parts = fio.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length >= 3 ? parts[2] : string.Empty; // Отчество - третий элемент
    }

    private string ExtractLastName(string fio)
    {
        var parts = fio.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length >= 1 ? parts[0] : string.Empty; // Фамилия - первый элемент
    }

    private string ExtractBirthday(string birthday)
    {
        if (DateTime.TryParseExact(birthday, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
            return parsedDate.ToString("yyyy-MM-dd");

        return DateTime.MinValue.ToString("yyyy-MM-dd");
    }

    private void UpdateKeyboard(bool digit, bool email, string language)
    {
        IsDigit = digit;
        IsEmailPrinting = email;
        Language = language;
    }
}