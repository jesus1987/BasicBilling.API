using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using System.Linq;
using System.Web;
using BasicBilling.API.Extensions;

namespace BasicBilling.API.Middleware
{
    public static class LogHelper
    {
        public static string RequestPayload = "";

        public static async void EnrichFromRequest(IDiagnosticContext diagnosticContext, HttpContext httpContext)
        {
            var logger = (ILogger<HttpLoggingMiddleware>)httpContext.RequestServices.GetService(typeof(ILogger<HttpLoggingMiddleware>));

            using (logger.BeginScope(diagnosticContext))
            {
                var responseBodyPayload = await ReadResponseBody(httpContext.Response);

                var request = httpContext.Request;

                if (request.Path.HasValue)
                {
                    if (!request.Path.Value.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase) &&
                        !request.Path.Value.StartsWith("/api/activitylog", StringComparison.OrdinalIgnoreCase))
                    {
                        var maskedQueryString = DestructuramaExtensions.ApplyAttributesToDictionary(HttpUtility.ParseQueryString(request.QueryString.Value)
                            .ToStringDictionary());

                        diagnosticContext.Set("Path", QueryHelpers.AddQueryString(request.Path.Value, maskedQueryString));
                        diagnosticContext.Set("Host", request.Host);
                        diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].FirstOrDefault());
                        diagnosticContext.Set("UserName", httpContext.Request.Headers["UserName"].FirstOrDefault());
                        diagnosticContext.Set("UserId", httpContext.Request.Headers["UserId"].FirstOrDefault());
                        diagnosticContext.Set("ClientIP", httpContext.Connection.RemoteIpAddress.ToString());
                        diagnosticContext.Set("Protocol", request.Protocol);
                        diagnosticContext.Set("Scheme", request.Scheme);
                        diagnosticContext.Set("RequestBody", JsonExtensions.IsJson(RequestPayload) ? JsonConvert.DeserializeObject<dynamic>(DestructuramaExtensions.ApplyAttributesToJson(RequestPayload)) : RequestPayload, true);
                        diagnosticContext.Set("ResponseBody", JsonExtensions.IsJson(responseBodyPayload) ? JsonConvert.DeserializeObject<dynamic>(DestructuramaExtensions.ApplyAttributesToJson(responseBodyPayload)) : responseBodyPayload, true);
                        if (request.QueryString.HasValue)
                            diagnosticContext.Set("QueryString", maskedQueryString, true);


                        diagnosticContext.Set("ContentType", httpContext.Response.ContentType);

                        var endpoint = httpContext.GetEndpoint();
                        if (endpoint is object) 
                            diagnosticContext.Set("EndPoint", endpoint.DisplayName);
                    }
                }
            }
        }

        private static async Task<string> ReadResponseBody(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            return $"{responseBody}";
        }
    }

    public class HttpLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public HttpLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            string requestBodyPayload = await ReadRequestBody(context.Request);
            LogHelper.RequestPayload = requestBodyPayload;

            var originalResponseBodyStream = context.Response.Body;

            await using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context);

            await responseBody.CopyToAsync(originalResponseBodyStream);
        }

        private static async Task<string> ReadRequestBody(HttpRequest request)
        {
            request.EnableBuffering();

            var body = request.Body;
            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            var requestBody = Encoding.UTF8.GetString(buffer);
            body.Seek(0, SeekOrigin.Begin);
            request.Body = body;

            return requestBody;
        }
    }
}
