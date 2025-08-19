using Microsoft.AspNetCore.Mvc;

namespace AzureBrasilCloudVaga.ApiService.Models.Response;

public record TenantGroupResponse(string Id,string DisplayName,string Description,DateTimeOffset CreatedDateTime);