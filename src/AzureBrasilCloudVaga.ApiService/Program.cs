using AzureBrasilCloudVaga.ApiService.Extensions;
using AzureBrasilCloudVaga.ApiService.Interfaces;
using AzureBrasilCloudVaga.ApiService.Middleware;
using AzureBrasilCloudVaga.ApiService.Models.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Scalar.AspNetCore;
using System.Text.Json.Serialization;
using ZiggyCreatures.Caching.Fusion;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddProblemDetails();

builder.Services.AddFusionCache()
.WithDefaultEntryOptions(new FusionCacheEntryOptions
{
    Duration = TimeSpan.FromMinutes(2),
    SkipMemoryCacheRead = true,
    SkipMemoryCacheWrite = true

})
.WithSerializer(new ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson.FusionCacheSystemTextJsonSerializer(new System.Text.Json.JsonSerializerOptions
{
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    PropertyNameCaseInsensitive = true
}))
.WithDistributedCache(new RedisCache(new RedisCacheOptions() { Configuration = builder.Configuration.GetConnectionString("cache") }));

builder.Services.AddOpenApi();



builder.Services.AddAzureAuthentication(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}


app.MapGet("/api/tenant/groups", async (
    [AsParameters] GroupRequest request,
    [FromServices] ITenantService tenantService) =>
{
    return await tenantService.GetPaginatedGroupsAsync(request);
})
.WithOpenApi()
.WithName("groups")
.RequireAuthorization();


app.MapGet("/api/tenant/signins", async (
    [AsParameters] SignisRequest request,
    [FromServices] ITenantService tenantService) =>
{
    return Results.Ok(await tenantService.GetPaginatedSigninsAsync(request));
})
.WithOpenApi()
.WithName("signins")
.RequireAuthorization();



app.MapGet("/api/tenant/users", async (
    [AsParameters] UserRequest request,
    [FromServices] ITenantService tenantService) =>
{
    return await tenantService.GetPaginatedUsersAsync(request);
})
.WithOpenApi()
.WithName("users")
.RequireAuthorization();


app.MapDefaultEndpoints();
app.Run();