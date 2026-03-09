using Core;
using SberUniversity.Model;
using System.Net.Mail;
using System.Windows.Input;
using SberUniversity.Helpers;
using System.Text.RegularExpressions;
using OfficeOpenXml;
using System.IO;
using MailKit;
using MailKit.Net.Imap;
using MimeKit;
using System.Net;
using CustomKeyboard.Helpers;
using CustomKeyboard.Views;

namespace SberUniversity.ViewModel.Views
{
    public class ResultPageViewModel : ObservableObject
    {
        public ResultModel Result
        {
            get => GetOrCreate<ResultModel>();
            set
            {
                SetAndNotify(value);
                Result.PropertyChanged += Result_PropertyChanged;
            }
        }

        private SettingsModel Settings
        {
            get => GetOrCreate<SettingsModel>();
            set => SetAndNotify(value);
        }

        public bool IsEmailValid
        {
            get => GetOrCreate<bool>();
            set
            {
                SetAndNotify(value); 
                ValidateForm(); 
            }
        }

        public string? Points
        {
            get => GetOrCreate<string?>();
            set => SetAndNotify(value);
        }

        public ResultPageViewModel(ResultModel result)
        {
            Result = result;
            Settings = SingletonSettings.Instance.Settings;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            KeyboardManager.SetLayout("English");
            ValidatePoints();
        }

        private void ValidatePoints()
        {
            Points = Result.PointsCount switch
            {
                0 => "баллов",
                1 => "балл",
                > 1 and < 5 => "балла",
                >= 5 => "баллов",
                _ => Points
            };
        }

        public bool IsFormValid { get => GetOrCreate<bool>(); set => SetAndNotify(value); }

        private void ValidateEmail()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var email = Result.EMail!;
            if (email == null) return;

            var regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w)+)+)$");
            var match = regex.Match(email);

            IsEmailValid = match.Success;
        }

        private void Result_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Result.EMail):
                    ValidateEmail();
                    break;
            }
        }

        private void ValidateForm()
        {
            IsFormValid = IsEmailValid;
            SetAndNotify(nameof(IsFormValid));
        }

        public ICommand SendEmail => GetOrCreate(new RelayCommand( f => {  SendEmailAsync(); }));

        private void SendEmailAsync()
        {
            try
            {
                var emailBody = $@"
            <html>
            <head>
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        color: #333;
                    }}
                    h1 {{
                        color: #4CAF50;
                    }}
                    h2 {{
                        color: #2196F3;
                    }}
                    a {{
                        color: #2196F3;
                    }}
                    p {{
                        font-size: 14px;
                        line-height: 1.5;
                    }}
                    .upgrade-title {{
                        font-weight: bold;
                        font-size: 16px;
                        margin-top: 10px;
                    }}
                    .upgrade-description {{
                        margin-bottom: 15px;
                    }}
                </style>
            </head>
            <body>
                <h1>Ваш результат {Result.PointsCount} {Points}</h1>
                <p>{Result.Description} <a href='{Result.Links[0]}'>{Result.Words[0]}</a><br>{Result.Description2}</p>

                <div>
                    <h2 class='upgrade-title'><a href='{Result.Links[1]}'>{Result.Words[1]} {Result.ResultUpgrade.FirstUpgradeTitle}</a></h2>
                    <p class='upgrade-description'>{Result.ResultUpgrade.FirstUpgradeDescription}</p>
                </div>

                <div>
                    <h2 class='upgrade-title'><a href='{Result.Links[2]}'>{Result.Words[2]} {Result.ResultUpgrade.SecondUpgradeTitle}</a> </h2>
                    <p class='upgrade-description'>{Result.ResultUpgrade.SecondUpgradeDescription}</p>
                </div>

                <div>
                    <h2 class='upgrade-title'><a href='{Result.Links[3]}'>{Result.Words[3]} {Result.ResultUpgrade.LastUpgradeTitle}</a> </h2>
                    <p class='upgrade-description'>{Result.ResultUpgrade.LastUpgradeDescription}</p>
                </div>
            </body>
            </html>";

                SendToEmail(emailBody);
                SendFromEmail(emailBody);

                SaveDataToExcel(Result.EMail!, Result.IsAgreePersonalData);
                Result.PropertyChanged -= Result_PropertyChanged;
                CommonCommands.NavigateCommand.Execute(PageTypes.CompletedPage);
            }
            catch (Exception ex)
            {
                CommonCommands.OpenPopupCommand.Execute(PopupTypes.ErrorPopup);
            }
        }

        private async void SendFromEmail(string emailBody)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("SberUniversity", Settings.From));
            message.To.Add(new MailboxAddress("", Result.EMail));
            message.Subject = "Ваши результаты тестирования от СберУниверситета и SberQ";
            message.Body = new TextPart("html") { Text = emailBody };

            using var imapClient = new ImapClient();

            await imapClient.ConnectAsync(Settings.ImapHost, Settings.ImapPort);
            imapClient.Authenticate(Settings.From, Settings.Password);

            var sentFolder = imapClient.GetFolder(SpecialFolder.Sent);
            await sentFolder.OpenAsync(FolderAccess.ReadWrite);
            await sentFolder.AppendAsync(message);
            await imapClient.DisconnectAsync(true);
        }

        private async void SendToEmail(string emailBody)
        {
            try
            {
                var message = new MailMessage();
                message.From = new MailAddress(Settings.From!);
                message.To.Add(new MailAddress(Result.EMail!));
                message.Subject = "Ваши результаты тестирования от СберУниверситета и SberQ";
                message.Body = emailBody;
                message.IsBodyHtml = true;

                var smtpClient = new System.Net.Mail.SmtpClient();
                smtpClient.Host = Settings.Host!;
                smtpClient.Port = Settings.Port;
                smtpClient.EnableSsl = true;
                smtpClient.Credentials = new NetworkCredential(Settings.From, Settings.Password);

                await smtpClient.SendMailAsync(message);

            }
            catch (Exception ex)
            {
            }
        }

        private void SaveDataToExcel(string email, bool isAgreePersonalData)
        {
            var filePath = "UserInfo.xlsx";

            var fileExists = File.Exists(filePath);

            using var package = new ExcelPackage(new FileInfo(filePath));
            ExcelWorksheet worksheet;

            if (fileExists)
            {
                worksheet = package.Workbook.Worksheets[0];
            }
            else
            {
                worksheet = package.Workbook.Worksheets.Add("UserData");

                worksheet.Cells[1, 1].Value = "Email";
                worksheet.Cells[1, 2].Value = "Согласие на обработку персональных данных";
            }

            var row = worksheet.Dimension?.Rows + 1 ?? 2;

            worksheet.Cells[row, 1].Value = email;
            worksheet.Cells[row, 2].Value = isAgreePersonalData ? "Да" : "Нет";

            package.Save();
        }
    }
}