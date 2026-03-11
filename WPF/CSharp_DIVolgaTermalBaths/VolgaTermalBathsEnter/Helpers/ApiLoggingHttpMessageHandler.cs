using Serilog;
using System.Net.Http;
using System.Text;

namespace VolgaTermalBathsEnter.Helpers;

public class ApiLoggingHttpMessageHandler(ILogger logger) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            //var content = (request.Content as HttpContent).ReadAsStringAsync().Result;
            string requestContent = null;
            if (request.Content != null)
            {
                requestContent = await request.Content.ReadAsStringAsync(cancellationToken);
            }
            logger.Information("Тело запроса: " + requestContent);

            return await base.SendAsync(request, cancellationToken);
        }
        catch (Exception e)
        {
            logger.Error(e.Message);
            return new HttpResponseMessage { Content = new StringContent(string.Empty) };
        }
    }
}

//public class ApiLoggingHttpMessageHandler : DelegatingHandler
//{
//    private readonly ILogger _logger;

//    public ApiLoggingHttpMessageHandler(ILogger logger)
//    {
//        _logger = logger;
//    }

//    protected override async Task<HttpResponseMessage> SendAsync(
//        HttpRequestMessage request,
//        CancellationToken cancellationToken)
//    {
//        // ===== REQUEST =====
//        string requestContent = null;

//        if (request.Content != null)
//        {
//            requestContent = await request.Content.ReadAsStringAsync(cancellationToken);
//        }

//        _logger.Information(
//            "HTTP REQUEST\n{Method} {Url}\nHeaders: {Headers}\nBody: {Body}",
//            request.Method,
//            request.RequestUri,
//            request.Headers,
//            requestContent
//        );

//        // ===== RESPONSE =====
//        var response = await base.SendAsync(request, cancellationToken);

//        string responseContent = null;

//        if (response.Content != null)
//        {
//            responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

//            // ❗ ВАЖНО: пересоздаём Content, чтобы Refit мог его прочитать
//            response.Content = new StringContent(
//                responseContent,
//                Encoding.UTF8,
//                response.Content.Headers.ContentType?.MediaType
//            );
//        }

//        _logger.Information(
//            "HTTP RESPONSE\nStatus: {StatusCode}\nHeaders: {Headers}\nBody: {Body}",
//            response.StatusCode,
//            response.Headers,
//            responseContent
//        );

//        return response;
//    }
//}
