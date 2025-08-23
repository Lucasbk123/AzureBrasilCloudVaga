using AzureBrasilCloudVaga.ApiService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
using Microsoft.Graph.Models.TermStore;
using System.ComponentModel;
using System.Net;
using System.Text.Json;


namespace AzureBrasilCloudVaga.ApiService.Middleware;


public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";

        if (ex is ODataError odataError)
        {
            context.Response.StatusCode = odataError.ResponseStatusCode;
            var result = JsonSerializer.Serialize(new
            {
                ErrorCode = odataError.Error?.Code ?? "InternalServerError",
                Message = GraphErrorTranslator.GetMessage(odataError.Error?.Code, odataError.Error?.Message),
                IsThirdPartyIntegrationError = true
            });
            await context.Response.WriteAsync(result);
            return;

        }

        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        var generic = JsonSerializer.Serialize(new
        {
            errorCode = "InternalServerError",
            message = ex.Message,
            IsThirdPartyIntegrationError = false
        });
        await context.Response.WriteAsync(generic);
    }
}
