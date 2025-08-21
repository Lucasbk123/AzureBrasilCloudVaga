using AzureBrasilCloudVaga.Web;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

var scopes = builder.Configuration.GetSection("Api:Scopes").Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);

    foreach (var s in scopes)
        options.ProviderOptions.DefaultAccessTokenScopes.Add(s);
});


await builder.Build().RunAsync();
