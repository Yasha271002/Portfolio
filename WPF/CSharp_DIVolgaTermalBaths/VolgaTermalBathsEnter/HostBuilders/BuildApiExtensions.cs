using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Refit;
using System.Net.Http.Headers;
using VolgaTermalBathsEnter.Helpers;
using VolgaTermalBathsEnter.Models.Client.Settings.HostSettings;
using VolgaTermalBathsEnter.Models.Interfaces;

namespace VolgaTermalBathsEnter.HostBuilders;

public static class BuildApiExtensions
{
    public static IHostBuilder BuildApi(this IHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            var hostSettings = context.Configuration.GetSection("hostSettings").Get<HostSettings>();
            var refitSettings = new RefitSettings
            {
                ContentSerializer = new NewtonsoftJsonContentSerializer()
            };
            services.AddTransient<ApiLoggingHttpMessageHandler>();

            services.AddRefitClient<IUserApi>(settings: refitSettings)
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = new Uri($"{hostSettings.Host}/fitness/hs/api/v2");
                    c.Timeout = TimeSpan.FromMinutes(10);
                    c.DefaultRequestHeaders.Add("Accept", "application/json");
                    c.DefaultRequestHeaders.Add("terminalid", hostSettings.TerminalId);
                    c.DefaultRequestHeaders.Add("apikey", hostSettings.ApiKey);
                    c.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Basic", hostSettings.AuthKey);
                }).AddHttpMessageHandler<ApiLoggingHttpMessageHandler>();

            services.AddRefitClient<IUserCardToTicketApi>(settings: refitSettings)
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = new Uri($"{hostSettings.CardHost}/fitness/hs/api/v2");
                    c.Timeout = TimeSpan.FromMinutes(10);
                    c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    c.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Basic", hostSettings.CardAuthKey);
                    c.DefaultRequestHeaders.Add("terminalid", hostSettings.TerminalId);
                    c.DefaultRequestHeaders.Add("apikey", hostSettings.ApiKey);
                    
                }).AddHttpMessageHandler<ApiLoggingHttpMessageHandler>();
        });
        return builder;
    }
}