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
            throw new HttpRequestException(
                 content.Message,
                 null,
                 response.StatusCode
             );
        }

        return await response.Content.ReadFromJsonAsync<T>();
    }
}

public record ApiError(string Message,string Error);