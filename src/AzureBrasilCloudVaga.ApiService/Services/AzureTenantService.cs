using AzureBrasilCloudVaga.ApiService.Interfaces;
using AzureBrasilCloudVaga.ApiService.Models.Request;
using AzureBrasilCloudVaga.ApiService.Models.Response;
using AzureBrasilCloudVaga.ApiService.Models.Response.Shared;
using Microsoft.Graph;
using System.Linq;

namespace AzureBrasilCloudVaga.ApiService.Services
{
    public class AzureTenantService : ITenantService
    {
        private readonly GraphServiceClient _graphServiceClient;

        public AzureTenantService(GraphServiceClient graphServiceClient)
        {
            _graphServiceClient = graphServiceClient;
        }

        public async Task<PagedResponse<TenantGroupResponse>> GetGroupsAsync(TenantGroupRequest request)
        {

            var page = string.IsNullOrEmpty(request.OdataNextLink) ?

                await _graphServiceClient.Groups.GetAsync(req =>
                {
                    req.QueryParameters.Top = request.Limit;
                    req.QueryParameters.Select = ["id", "displayName", "description", "createdDateTime"];
                    req.QueryParameters.Orderby = ["createdDateTime"];
                    req.QueryParameters.Count = true;
                    req.Headers.Add("ConsistencyLevel", "eventual");
                }) 
                :
                await _graphServiceClient.Groups.WithUrl(request.OdataNextLink).GetAsync();


            return new PagedResponse<TenantGroupResponse>()
            {
                Items = page.Value.Select(x => new TenantGroupResponse(x.Id, x.DisplayName, x.Description, x.CreatedDateTime.Value)),
                OdataNextLink = page.OdataNextLink ?? "",
                TotalRecords = page.OdataCount
            };
        }
    }
}
