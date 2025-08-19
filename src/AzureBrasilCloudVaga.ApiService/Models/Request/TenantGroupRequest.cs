using Microsoft.AspNetCore.Mvc;

namespace AzureBrasilCloudVaga.ApiService.Models.Request;

public class TenantGroupRequest
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
