namespace AzureBrasilCloudVaga.Web.Models;

public record UserResponse(string Id, string DisplayName, string PrincipalName, DateTimeOffset CreatedDateTime);
