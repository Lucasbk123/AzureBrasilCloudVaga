using Azure.Identity;
using AzureBrasilCloudVaga.ApiService.Interfaces;
using AzureBrasilCloudVaga.ApiService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Graph;
using Microsoft.Identity.Web;

namespace AzureBrasilCloudVaga.ApiService.Extensions
{
    public static class AuthExtension
    {
        public static void AddAzureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                 .AddMicrosoftIdentityWebApi(configuration.GetSection("AzureAd"));

            services.AddAuthorization();

            services.AddSingleton(sp =>
            {
                var tenantId = configuration["AzureAd:TenantId"]!;
                var clientId = configuration["AzureAd:ClientId"]!;
                var clientSecret = configuration["AzureAd:ClientSecret"]!;

                var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
                return new GraphServiceClient(credential, ["https://graph.microsoft.com/.default"]);
            });

            services.AddScoped<ITenantService, AzureTenantService>();

        }
    }
}
