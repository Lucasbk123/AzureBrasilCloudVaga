namespace AzureBrasilCloudVaga.ApiService.Models.Response;

public record UserResponse(string Id, string DisplayName, string PrincipalName, DateTimeOffset CreatedDateTime);