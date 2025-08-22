using Microsoft.AspNetCore.Mvc;

namespace AzureBrasilCloudVaga.ApiService.Models.Response;

public record GroupResponse(string Id,string DisplayName,string Description,DateTimeOffset CreatedDateTime);