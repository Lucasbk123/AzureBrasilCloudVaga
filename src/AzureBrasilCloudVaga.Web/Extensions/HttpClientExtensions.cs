using System.Net.Http.Json;
using System.Text.Json;

namespace AzureBrasilCloudVaga.Web.Extensions;


public static class HttpClientExtensions
{

    private static JsonSerializerOptions jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };
    public static async Task<T?> GetOrFailAsync<T>(this HttpClient client, string url)
    {
        var response = await client.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            var content = JsonSerializer.Deserialize<ApiError>(await response.Content.ReadAsStringAsync(), jsonSerializerOptions);

            throw content.IsThirdPartyIntegrationError ? new GraphApiException(content.ErrorCode, content.Message) : new HttpRequestException(
                 content.Message,
                 null,
                 response.StatusCode
             );
        }

        return await response.Content.ReadFromJsonAsync<T>();
    }
}

public record ApiError(string Message, string ErrorCode, bool IsThirdPartyIntegrationError);

public class GraphApiException : Exception
{
    public string ErrorCode { get; }
    public GraphApiException(string errorCode, string? message = null, Exception? innerException = null)
        : base(message ?? "Ocorreu um erro ao acessar o Microsoft Graph.", innerException)
    {
        ErrorCode = errorCode;
    }
}