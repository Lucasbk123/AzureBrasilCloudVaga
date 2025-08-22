using AzureBrasilCloudVaga.Web;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();

var scopes = builder.Configuration.GetSection("Api:Scopes").Get<string[]>() ?? Array.Empty<string>();
var apiBase = builder.Configuration["Api:BaseUrl"]!;

builder.Services.AddHttpClient("ServerApi", client => client.BaseAddress = new Uri(apiBase))
    .AddHttpMessageHandler(sp =>
    {
        var handler = sp.GetRequiredService<AuthorizationMessageHandler>()
            .ConfigureHandler(authorizedUrls: [apiBase], scopes: scopes);
        return handler;
    });

builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);

    foreach (var s in scopes)
        options.ProviderOptions.DefaultAccessTokenScopes.Add(s);
});


await builder.Build().RunAsync();
