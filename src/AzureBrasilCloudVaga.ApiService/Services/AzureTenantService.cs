using Azure.Core;
using AzureBrasilCloudVaga.ApiService.Extensions;
using AzureBrasilCloudVaga.ApiService.Interfaces;
using AzureBrasilCloudVaga.ApiService.Models.Request;
using AzureBrasilCloudVaga.ApiService.Models.Response;
using AzureBrasilCloudVaga.ApiService.Models.Response.Shared;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using System.Linq;
using ZiggyCreatures.Caching.Fusion;

namespace AzureBrasilCloudVaga.ApiService.Services;

public class AzureTenantService : ITenantService
{
    private readonly GraphServiceClient _graphServiceClient;
    private readonly IFusionCache _fusionCache;
    const string cacheKeyOdataCount = "{0}:OdataCount-PageNumber-{1}-Size-{2}";
    const string cacheKey = "{0}:PageNumber-{1}-Size-{2}";

    public AzureTenantService(GraphServiceClient graphServiceClient, IFusionCache fusionCache)
    {
        _graphServiceClient = graphServiceClient;
        _fusionCache = fusionCache;
    }
    //TODO: posso salvar o objeto já mapeado para evitar fazer o mapper em toda chamada.
    //Também poderia passa somente OdataNextLink porem o usuario não poderia seleciona a pagina apenas ir para proxima
    public async Task<PaginatedResponse<TenantGroupResponse>> GetPaginatedGroupsAsync(TenantGroupRequest request)
    {

        var cacheKey = request.ToCacheKey("groups:");
        var odataCountCacheKey = request.ToCacheKey("groups:OdataCount:");

        var groupsPageCache = await _fusionCache.TryGetAsync<GroupCollectionResponse>(cacheKey);

        if (groupsPageCache.HasValue)
            return await CreateMapperGroups(groupsPageCache, request, groupsPageCache.Value.OdataCount 
                ?? await _fusionCache.GetOrDefaultAsync<long>(odataCountCacheKey));

        int currentPage = 1;

        var groupsPage = await _fusionCache.GetOrSetAsync(cacheKey.ReplacePageNumber(request.PageNumber,currentPage),
            await _graphServiceClient.Groups.GetAsync(req =>
        {
            req.QueryParameters.Top = request.PageSize;
            req.QueryParameters.Select = ["id", "displayName", "description", "createdDateTime"];
            req.QueryParameters.Orderby = ["createdDateTime"];
            req.QueryParameters.Count = true;
            req.Headers.Add("ConsistencyLevel", "eventual");
        }));

        await _fusionCache.SetAsync(odataCountCacheKey, groupsPage.OdataCount.Value);


        if (currentPage != request.PageNumber && (request.PageSize * request.PageNumber > groupsPage.OdataCount))
            return new PaginatedResponse<TenantGroupResponse>();

        while (currentPage < request.PageNumber && groupsPage.OdataNextLink != null)
        {
            currentPage++;

            groupsPage = await _fusionCache.GetOrSetAsync(cacheKey.ReplacePageNumber(request.PageNumber,currentPage),
                 await _graphServiceClient.Groups.WithUrl(groupsPage.OdataNextLink).GetAsync());
        }

        return await CreateMapperGroups(groupsPage, request, groupsPage.OdataCount
                ?? await _fusionCache.GetOrDefaultAsync<long>(odataCountCacheKey));
    }

    private async Task<PaginatedResponse<TenantGroupResponse>> CreateMapperGroups(GroupCollectionResponse groupsPage, TenantGroupRequest request,long OdataCount) =>
        new()
        {
            Items = groupsPage.Value.Select(x => new TenantGroupResponse(x.Id, x.DisplayName, x.Description, x.CreatedDateTime.Value)),
            TotalRecords = OdataCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
}