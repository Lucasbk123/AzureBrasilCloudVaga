using Azure.Core;
using AzureBrasilCloudVaga.ApiService.Interfaces;
using AzureBrasilCloudVaga.ApiService.Models.Request;
using AzureBrasilCloudVaga.ApiService.Models.Response;
using AzureBrasilCloudVaga.ApiService.Models.Response.Shared;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using System.Linq;
using ZiggyCreatures.Caching.Fusion;

namespace AzureBrasilCloudVaga.ApiService.Services
{
    public class AzureTenantService : ITenantService
    {
        private readonly GraphServiceClient _graphServiceClient;
        private readonly IFusionCache _fusionCache;

        public AzureTenantService(GraphServiceClient graphServiceClient, IFusionCache fusionCache)
        {
            _graphServiceClient = graphServiceClient;
            _fusionCache = fusionCache;
        }

        public async Task<PagedResponse<TenantGroupResponse>> GetGroupsAsync(TenantGroupRequest request)
        {
            string cacheKey = "groups:PageNumber-{0}-Size-{1}";

            var groupsPageCache = await _fusionCache.TryGetAsync<GroupCollectionResponse>(string.Format(cacheKey, request.PageNumber, request.PageSize));

            if (groupsPageCache.HasValue)
                return CreateMapperGroups(groupsPageCache, request);

            int currentPage = 1;

            var groupsPage = await _fusionCache.GetOrSetAsync(string.Format(cacheKey, currentPage, request.PageSize),
                await _graphServiceClient.Groups.GetAsync(req =>
            {
                req.QueryParameters.Top = request.PageSize;
                req.QueryParameters.Select = ["id", "displayName", "description", "createdDateTime"];
                req.QueryParameters.Orderby = ["createdDateTime"];
                req.QueryParameters.Count = true;
                req.Headers.Add("ConsistencyLevel", "eventual");
            }));


            if (currentPage != request.PageSize &&  (request.PageSize * request.PageNumber > groupsPage.OdataCount))
                return new PagedResponse<TenantGroupResponse>();

            while (currentPage < request.PageNumber && groupsPage.OdataNextLink != null)
            {
                currentPage++;

                groupsPage = await _fusionCache.GetOrSetAsync(string.Format(cacheKey, currentPage, request.PageSize),
                     await _graphServiceClient.Groups.WithUrl(groupsPage.OdataNextLink).GetAsync());
            }

            return CreateMapperGroups(groupsPage, request);
        }

        private PagedResponse<TenantGroupResponse> CreateMapperGroups(GroupCollectionResponse groupsPage, TenantGroupRequest request) =>
            new()
            {
                Items = groupsPage.Value.Select(x => new TenantGroupResponse(x.Id, x.DisplayName, x.Description, x.CreatedDateTime.Value)),
                TotalRecords = groupsPage.OdataCount ?? 0,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
    }
}
