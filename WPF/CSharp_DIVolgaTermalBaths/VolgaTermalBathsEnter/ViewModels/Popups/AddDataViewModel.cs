using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MvvmNavigationLib.Services;
using MvvmNavigationLib.Stores;
using VolgaTermalBathsEnter.Models.Client.User.Ticket;
using VolgaTermalBathsEnter.Models.Message;
using System.Globalization;
using VolgaTermalBathsEnter.ViewModels.Pages;
using Newtonsoft.Json.Linq;

namespace VolgaTermalBathsEnter.ViewModels.Popups;

public partial class AddDataViewModel(
    IMessenger messenger,
    CloseNavigationService<ModalNavigationStore> closeNavigationService,
    TicketModel ticketModel) : ObservableValidator
{
    [ObservableProperty] private bool _canContinue;
    [ObservableProperty] private bool _shiftPressed;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Поле обязательно для заполнения")]
    [CustomValidation(typeof(AddDataViewModel), nameof(ValidateName))]
    private string _name;

    partial void OnNameChanged(string value) => OnChanged(value);

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Поле обязательно для заполнения")]
    [CustomValidation(typeof(AddDataViewModel), nameof(ValidateLastName))]
    private string _lastName;

    partial void OnLastNameChanged(string value) => OnChanged(value);

    [ObservableProperty] [NotifyDataErrorInfo] [CustomValidation(typeof(AddDataViewModel), nameof(ValidateSurname))]
    private string _surname;

    partial void OnSurnameChanged(string value) => OnChanged(value);

    private bool _isFormating;

    private void OnChanged(string value)
    {
        if (_isFormating) return;

        try
        {
            _isFormating = true;
            if (!string.IsNullOrEmpty(value)) ShiftPressed = value.Last() == ' ';
            else ShiftPressed = true;
        }
        finally
        {
            _isFormating = false;
            UpdateCanContinue();
        }
    }

    [RelayCommand]
    private void Loaded()
    {
        ErrorsChanged += (s, e) => UpdateCanContinue();
        CanContinue = false;
    }

    [RelayCommand]
    private void GotNameFocus() => OnChanged(Name);

    [RelayCommand]
    private void GotSurnameFocus() => OnChanged(Surname);

    [RelayCommand]
    private void GotLastNameFocus() => OnChanged(LastName);

    [RelayCommand]
    private void UpdateSelectedTicketPage()
    {
        ticketModel.TicketPersonalInfo.FirstName = Name;
        ticketModel.TicketPersonalInfo.SecondName = Surname;
        ticketModel.TicketPersonalInfo.LastName = LastName;
        ticketModel.TicketPersonalInfo.FullName = $"{LastName} {Name} {Surname}";
        messenger.Send(new RefreshMessage());
        closeNavigationService.Navigate();
    }

    [RelayCommand]
    private void GoBack() => closeNavigationService.Navigate();

    private void UpdateCanContinue()
    {
        CanContinue = !HasErrors &&
                      !string.IsNullOrWhiteSpace(LastName) &&
                      !string.IsNullOrWhiteSpace(Name);
    }

    public static ValidationResult ValidateLastName(string lastName, ValidationContext context)
    {
        if (string.IsNullOrEmpty(lastName))
            return new ValidationResult("Поле обязательно для заполнения");

        return !Regex.IsMatch(lastName, @"^[А-ЯЁ][а-яё]+(-[А-ЯЁ][а-яё]+)?$")
            ? new ValidationResult("Требуется формат: Фамилия (с заглавной буквы)")
            : ValidationResult.Success;
    }

    public static ValidationResult ValidateName(string name, ValidationContext context)
    {
        if (string.IsNullOrEmpty(name))
            return new ValidationResult("Поле обязательно для заполнения");

        return !Regex.IsMatch(name, @"^[А-ЯЁ][а-яё]+$")
            ? new ValidationResult("Требуется формат: Имя (с заглавной буквы)")
            : ValidationResult.Success;
    }

    public static ValidationResult ValidateSurname(string surname, ValidationContext context)
    {
        if (string.IsNullOrEmpty(surname))
            return ValidationResult.Success;

        return !Regex.IsMatch(surname, @"^[А-ЯЁ][а-яё]+$")
            ? new ValidationResult("Требуется формат: Отчество (с заглавной буквы)")
            : ValidationResult.Success;
    }
}