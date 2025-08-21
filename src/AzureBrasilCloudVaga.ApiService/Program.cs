using AzureBrasilCloudVaga.ApiService.Extensions;
using AzureBrasilCloudVaga.ApiService.Interfaces;
using AzureBrasilCloudVaga.ApiService.Models.Request;
using AzureBrasilCloudVaga.ApiService.Models.Response;
using AzureBrasilCloudVaga.ApiService.Models.Response.Shared;
using AzureBrasilCloudVaga.ApiService.Services;
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
    Duration = TimeSpan.FromMinutes(60),
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

builder.Services.AddScoped<ITenantService, AzureTenantService>();


builder.Services.AddAzureAuthentication(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();


app.UseExceptionHandler();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}


app.MapGet("/api/tenant/groups", async ([AsParameters] TenantGroupRequest request, [FromServices] ITenantService tenantService) =>
{
    return await tenantService.GetPaginatedGroupsAsync(request);
})
.WithOpenApi()
.WithName("groups");

app.MapDefaultEndpoints();
app.Run();