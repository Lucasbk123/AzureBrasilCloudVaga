using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace AzureBrasilCloudVaga.Web.Models;

public record TenantGroupResponse(string Id, string DisplayName, string Description, DateTimeOffset CreatedDateTime);
