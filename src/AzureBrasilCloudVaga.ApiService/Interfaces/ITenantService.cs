using AzureBrasilCloudVaga.ApiService.Models.Request;
using AzureBrasilCloudVaga.ApiService.Models.Response;
using AzureBrasilCloudVaga.ApiService.Models.Response.Shared;

namespace AzureBrasilCloudVaga.ApiService.Interfaces
{
    public interface ITenantService
    {
        public Task<PaginatedResponse<GroupResponse>> GetPaginatedGroupsAsync(GroupRequest request);

        public Task<PaginatedResponse<UserResponse>> GetPaginatedUsersAsync(UserRequest request);

        public Task<PaginatedResponse<SigninsResponse>> GetPaginatedSigninsAsync(SignisRequest request);


    }
}
