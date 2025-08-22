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

    public AzureTenantService(GraphServiceClient graphServiceClient, IFusionCache fusionCache)
    {
        _graphServiceClient = graphServiceClient;
        _fusionCache = fusionCache;
    }
    //TODO: posso salvar o objeto já mapeado para evitar fazer o mapper em toda chamada.
    //Também poderia passa somente OdataNextLink porem o usuario não poderia seleciona a pagina apenas ir para proxima
    public async Task<PaginatedResponse<GroupResponse>> GetPaginatedGroupsAsync(GroupRequest request)
    {

        var cacheKey = request.ToCacheKey("groups:");
        var odataCountCacheKey = request.ToCacheKey("groups:OdataCount:");

        var groupsCache = await _fusionCache.TryGetAsync<GroupCollectionResponse>(cacheKey);

        if (groupsCache.HasValue)
            return  CreateMapperGroups(groupsCache, request, groupsCache.Value.OdataCount 
                ?? await _fusionCache.GetOrDefaultAsync<long>(odataCountCacheKey));

        int currentPage = 1;

        var groups = await _fusionCache.GetOrSetAsync(cacheKey.ReplacePageNumber(request.PageNumber,currentPage),
            await _graphServiceClient.Groups.GetAsync(req =>
        {
            req.QueryParameters.Top = request.PageSize;
            req.QueryParameters.Select = ["id", "displayName", "description", "createdDateTime"];
            req.QueryParameters.Orderby = ["createdDateTime"];
            req.QueryParameters.Count = true;
            req.Headers.Add("ConsistencyLevel", "eventual");
        }));

        await _fusionCache.SetAsync(odataCountCacheKey, groups.OdataCount.Value);


        if (currentPage != request.PageNumber && (request.PageSize * request.PageNumber > groups.OdataCount))
            return new PaginatedResponse<GroupResponse>();

        while (currentPage < request.PageNumber && groups.OdataNextLink != null)
        {
            currentPage++;

            groups = await _fusionCache.GetOrSetAsync(cacheKey.ReplacePageNumber(request.PageNumber,currentPage),
                 await _graphServiceClient.Groups.WithUrl(groups.OdataNextLink).GetAsync());
        }

        return  CreateMapperGroups(groups, request, groups.OdataCount
                ?? await _fusionCache.GetOrDefaultAsync<long>(odataCountCacheKey));
    }

    private PaginatedResponse<GroupResponse> CreateMapperGroups(GroupCollectionResponse groups, GroupRequest request,long OdataCount) =>
        new()
        {
            Items = groups.Value.Select(x => new GroupResponse(x.Id, x.DisplayName, x.Description, x.CreatedDateTime.Value)),
            TotalRecords = OdataCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

    public async Task<PaginatedResponse<UserResponse>> GetPaginatedUsersAsync(UserRequest request)
    {
        var cacheKey = request.ToCacheKey("users:");
        var odataCountCacheKey = request.ToCacheKey("users:OdataCount:");

        var usersCache = await _fusionCache.TryGetAsync<UserCollectionResponse>(cacheKey);

        if (usersCache.HasValue)
            return CreateMapperUsers(usersCache, request, usersCache.Value.OdataCount
                ?? await _fusionCache.GetOrDefaultAsync<long>(odataCountCacheKey));

        int currentPage = 1;

        var users = await _fusionCache.GetOrSetAsync(cacheKey.ReplacePageNumber(request.PageNumber, currentPage),
            await _graphServiceClient.Users.GetAsync(req =>
            {
                req.QueryParameters.Top = request.PageSize;
                req.QueryParameters.Select = ["id", "displayName", "userPrincipalName", "createdDatetime"];
                req.QueryParameters.Orderby = ["createdDateTime"];
                req.QueryParameters.Count = true;
                req.Headers.Add("ConsistencyLevel", "eventual");
            }));

        await _fusionCache.SetAsync(odataCountCacheKey, users.OdataCount.Value);


        if (currentPage != request.PageNumber && (request.PageSize * request.PageNumber > users.OdataCount))
            return new PaginatedResponse<UserResponse>();

        while (currentPage < request.PageNumber && users.OdataNextLink != null)
        {
            currentPage++;

            users = await _fusionCache.GetOrSetAsync(cacheKey.ReplacePageNumber(request.PageNumber, currentPage),
                 await _graphServiceClient.Users.WithUrl(users.OdataNextLink).GetAsync());
        }

        return CreateMapperUsers(users, request, users.OdataCount
                ?? await _fusionCache.GetOrDefaultAsync<long>(odataCountCacheKey));
    }

    private PaginatedResponse<UserResponse> CreateMapperUsers(UserCollectionResponse users, UserRequest request, long OdataCount) =>
    new()
    {
        Items = users.Value.Select(x => new UserResponse(x.Id, x.DisplayName, x.UserPrincipalName, x.CreatedDateTime.Value)),
        TotalRecords = OdataCount,
        PageNumber = request.PageNumber,
        PageSize = request.PageSize
    };


    public async Task<PaginatedResponse<SigninsResponse>> GetPaginatedSigninsAsync(SignisRequest request)
    {
        var cacheKey = request.ToCacheKey("signins:");
        var odataCountCacheKey = request.ToCacheKey("signins:OdataCount:");

        var signInsCache = await _fusionCache.TryGetAsync<SignInCollectionResponse>(cacheKey);

        if (signInsCache.HasValue)
            return CreateMapperSignIns(signInsCache, request, signInsCache.Value.OdataCount
                ?? await _fusionCache.GetOrDefaultAsync<long>(odataCountCacheKey));

        int currentPage = 1;

        var users = await _fusionCache.GetOrSetAsync(cacheKey.ReplacePageNumber(request.PageNumber, currentPage),
            await _graphServiceClient.AuditLogs.SignIns.GetAsync(req =>
            {
                req.QueryParameters.Top = request.PageSize;
                req.QueryParameters.Orderby = ["createdDateTime desc"];
                req.QueryParameters.Select = ["id", "userDisplayName", "userPrincipalName", "ipAddress", "createdDateTime"];
                req.QueryParameters.Count = true;
                req.Headers.Add("ConsistencyLevel", "eventual");
            }));

        await _fusionCache.SetAsync(odataCountCacheKey, users.OdataCount.Value);


        if (currentPage != request.PageNumber && (request.PageSize * request.PageNumber > users.OdataCount))
            return new PaginatedResponse<SigninsResponse>();

        while (currentPage < request.PageNumber && users.OdataNextLink != null)
        {
            currentPage++;

            users = await _fusionCache.GetOrSetAsync(cacheKey.ReplacePageNumber(request.PageNumber, currentPage),
                 await _graphServiceClient.AuditLogs.SignIns.WithUrl(users.OdataNextLink).GetAsync());
        }

        return CreateMapperSignIns(users, request, users.OdataCount
                ?? await _fusionCache.GetOrDefaultAsync<long>(odataCountCacheKey));
    }

    private PaginatedResponse<SigninsResponse> CreateMapperSignIns(SignInCollectionResponse signIns, SignisRequest request, long OdataCount) =>
        new()
        {
            Items = signIns.Value.Select(x => new SigninsResponse(x.Id, x.UserDisplayName, x.UserPrincipalName,x.IpAddress, x.CreatedDateTime.Value)),
            TotalRecords = OdataCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
}