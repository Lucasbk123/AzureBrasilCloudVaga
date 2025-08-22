namespace AzureBrasilCloudVaga.ApiService.Models.Response
{
    public record SigninsResponse(string Id, string DisplayName, string PrincipalName, string IpAddress, DateTimeOffset CreatedDateTime);
}
