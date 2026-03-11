using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MvvmNavigationLib.Services;
using Newtonsoft.Json.Linq;
using Refit;
using Serilog;
using Serilog.Core;
using VolgaTermalBathsEnter.Helpers;
using VolgaTermalBathsEnter.Models.Client;
using VolgaTermalBathsEnter.Models.Client.CreateClient;
using VolgaTermalBathsEnter.Models.Client.Settings.SmsSettings;
using VolgaTermalBathsEnter.Models.Client.Tickets;
using VolgaTermalBathsEnter.Models.Client.User;
using VolgaTermalBathsEnter.Models.Client.User.Ticket;
using VolgaTermalBathsEnter.Models.Interfaces;
using VolgaTermalBathsEnter.Utilities;
using VolgaTermalBathsEnter.ViewModels.Popups;

namespace VolgaTermalBathsEnter.ViewModels.Pages
{
    public partial class WelcomeUserViewModel(ILogger logger, SmsApiModel smsApiModel, IUserApi client, UserDataModel user, ObservableCollection<TicketModel> ticketList,
        NavigationService<UserRegisterViewModel> userRegisterNavigationService,
        NavigationService<SelectedTicketPageViewModel> selectadTicketNavigationService,
        NavigationService<PaidTicketViewModel> paidTicketNavigationService,
        NavigationService<ErrorPopupViewModel> errorNavigationService) : ObservableObject
    {
        private const string PhonePattern = @"^\(?\d{3}\)?[- ]?\d{3}[- ]?\d{4}$";

        [ObservableProperty] private string _phoneNumber=string.Empty;
        [ObservableProperty] private string _phoneNumberError = string.Empty;

        [ObservableProperty] private string _verificationCode = string.Empty;
        [ObservableProperty] private string _verificationCodeError = string.Empty;

        [ObservableProperty] private bool _isCodeSent;
        [ObservableProperty] private bool _isCodeVerification;

        private string? _generatedCode;
        private string _appSettings = File.ReadAllText("appsettings.json");


        [RelayCommand]
        private async Task VerifyNumber()
        {
            ticketList.Clear();
            IsCodeVerification = false;

            if (!IsValidPhoneNumber(PhoneNumber))
            {
                PhoneNumberError = "Неверный формат номера телефона";
                return;
            }

            PhoneNumberError = string.Empty;
            var formattedPhoneNumber = FormatPhoneNumberToDigits(PhoneNumber);

            try
            {
                await client.GetUserByPhone(formattedPhoneNumber);
            }
            catch (ApiException ex)
            {
                if (ex.StatusCode != HttpStatusCode.BadRequest)
                {
                    logger.Error($"Ошибка запроса: {ex.Message}");
                    errorNavigationService.Navigate();
                    return;
                }
            }
            catch (Exception ex)
            {
                logger.Error($"Ошибка исполнения запроса /client_by_phone: {ex.Message} : {ex.StackTrace}");
                errorNavigationService.Navigate();
                return;
            }

            _generatedCode = await SmsApiHelper.SendVerificationCodeAsync(formattedPhoneNumber, logger, smsApiModel);
            IsCodeSent = true;
            user.PhoneNumber = PhoneNumber;

        }

        [RelayCommand]
        private async Task VerifyCode()
        {
            if (string.IsNullOrEmpty(VerificationCode))
            {
                VerificationCodeError = "Введите код";
                return;
            }

            if (VerificationCode != _generatedCode &&
                VerificationCode != (string)JObject.Parse(_appSettings)["smsSettings"]!["admin_code"]!)
            {
                VerificationCodeError = "Неверный код";
                return;
            }

            try
            {
                VerificationCodeError = string.Empty;
                await ProcessPhoneNumberAsync();
            }
            catch (Exception e)
            {
                VerificationCodeError = "Ошибка при проверке";
                logger.Error(e.Message);
            }
        }


        private async Task ProcessPhoneNumberAsync()
        {
            var formattedPhoneNumber = FormatPhoneNumberToDigits(PhoneNumber);

            var response = await client.RequestProcessingWithErrorHandling(
                () => client.GetUserByPhone(formattedPhoneNumber), logger,
                "Ошибка получения пользователя по номеру телефона"
            );
            
            IsCodeVerification = true;
            if (response is null)
            {
                userRegisterNavigationService.Navigate();
                return;
            }
            user.ClientId = response.Data.ClientId;
            var haveTickets = await client.RequestProcessingWithErrorHandling(
                () => client.Get_Tickets(formattedPhoneNumber), logger,
                "Ошибка получения билетов пользователя");

            if (haveTickets.Parameters.Count == 0)
            {
                user.HaveTicket = false;
                selectadTicketNavigationService.Navigate();
                return;
            }

            user.HaveTicket = true;
            paidTicketNavigationService.Navigate();
        }

        [RelayCommand]
        private void OnPinPadButtonPressed(object parameter)
        {
            if (parameter is not string value) return;

            if (!IsCodeSent)
            {
                if (PhoneNumber!.Length <= 9)
                    PhoneNumber += value;
            }
            else
            {
                if (VerificationCode!.Length < 4)
                    VerificationCode += value;
            }

        }
        [RelayCommand]
        private void BackPinPad()
        {
            VerificationCodeError = string.Empty;
            if (!IsCodeSent)
            {
                if (!string.IsNullOrEmpty(PhoneNumber))
                    PhoneNumber = PhoneNumber[..^1];
            }
            else
            {
                if (!string.IsNullOrEmpty(VerificationCode))
                    VerificationCode = VerificationCode[..^1];
            }
        }

        private bool IsValidPhoneNumber(string? phoneNumber) => Regex.IsMatch(phoneNumber!, PhonePattern);
        private static string? FormatPhoneNumberToDigits(string? phoneNumber)
        {
            var digits = new string(phoneNumber!.Where(char.IsDigit).ToArray());

            if (digits.Length > 11)
                digits = digits[..11];

            while (digits.Length < 11)
                digits = "0" + digits;

            return $"+7{digits[1..]}";
        }

        [RelayCommand]
        private void GoBack()
        {
            IsCodeSent = false;
            PhoneNumber = string.Empty;
            VerificationCode = string.Empty;
            VerificationCodeError = string.Empty;
        }
    }
}
