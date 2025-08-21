using AzureBrasilCloudVaga.ApiService.Models.Request;
using AzureBrasilCloudVaga.ApiService.Models.Response;
using AzureBrasilCloudVaga.ApiService.Models.Response.Shared;

namespace AzureBrasilCloudVaga.ApiService.Interfaces
{
    public interface ITenantService
    {
        public Task<PaginatedResponse<TenantGroupResponse>> GetPaginatedGroupsAsync(TenantGroupRequest request);


    }
}
