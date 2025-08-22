using Microsoft.AspNetCore.Http;
using Microsoft.Graph.Models;
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

        if (ex.Message.Contains("premium license", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            var result = JsonSerializer.Serialize(new
            {
                error = "Licença Premium necessária",
                message = "Seu tenant não possui licença Premium para acessar este recurso.Entre em contato com o suporte."
            });
            await context.Response.WriteAsync(result);
            return;
        }

        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        var generic = JsonSerializer.Serialize(new
        {
            error = "Erro inesperado",
            message = ex.Message
        });
        await context.Response.WriteAsync(generic);
    }
}
