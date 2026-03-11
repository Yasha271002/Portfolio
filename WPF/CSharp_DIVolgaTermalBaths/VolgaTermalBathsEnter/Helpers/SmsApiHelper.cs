using System.Net.Http;
using Serilog;
using VolgaTermalBathsEnter.Models.Client.Settings.SmsSettings;

namespace VolgaTermalBathsEnter.Helpers;

public static class SmsApiHelper
{
    public static async Task<string> SendVerificationCodeAsync(string phoneNumber, ILogger logger, SmsApiModel model)
    {
        var verificationCode = new Random().Next(1000, 9999).ToString();

        using var client = new HttpClient();

        var queryParams = new Dictionary<string, string>
        {
            { "method", "push_msg" },
            { "key", model.ApiKey },
            { "text", verificationCode },
            { "phone", phoneNumber.Replace("+", "") },
            { "sender_name", model.SenderName }
        };

        var queryString = string.Join("&", queryParams.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"));
        var requestUrl = $"{model.Host}?{queryString}";

        try
        {
            var response = await client.GetAsync(requestUrl);
            if (response.IsSuccessStatusCode)
            {
                return verificationCode;
            }
            throw new Exception("Ошибка отправки СМС");
        }
        catch (Exception ex)
        {
            logger.Error($"Ошибка отправки СМС: {ex.Message}");
            throw;
        }
    }
}