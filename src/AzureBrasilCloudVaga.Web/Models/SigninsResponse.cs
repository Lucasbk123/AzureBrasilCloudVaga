namespace AzureBrasilCloudVaga.Web.Models;

public record SigninsResponse(string Id, string DisplayName, string PrincipalName, string IpAddress, DateTimeOffset CreatedDateTime);
