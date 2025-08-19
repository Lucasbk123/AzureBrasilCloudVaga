using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Graph;
using Microsoft.Identity.Web;

namespace AzureBrasilCloudVaga.ApiService.Extensions
{
    public static class AuthExtension
    {
        public static void AddAzureAuthentication(this IServiceCollection services,IConfiguration configuration)
        {
           services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(configuration.GetSection("AzureAd"));

            services.AddAuthorization();


            // Graph client with client credentials (application permissions)
            services.AddSingleton(sp =>
            {
                var tenantId = configuration["AzureAd:TenantId"]!;
                var clientId = configuration["AzureAd:ClientId"]!;
                var clientSecret = configuration["AzureAd:ClientSecret"]!;

                var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
                return new GraphServiceClient(credential, new[] { "https://graph.microsoft.com/.default" });
            });


        }
    }
}
