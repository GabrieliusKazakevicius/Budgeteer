using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Budgeteer.Web;
using Budgeteer.Web.Auth;
using Budgeteer.Web.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Toolbelt.Blazor.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var serviceCollection = builder.Services;
var builderConfiguration = builder.Configuration;

RegisterServices(serviceCollection, builderConfiguration);

await builder.Build().RunAsync();

static void RegisterServices(IServiceCollection services, IConfiguration configuration)
{
    services.AddScoped(sp => 
        new HttpClient {
            BaseAddress = new Uri(configuration.GetValue<string>("BackendBaseUri"))
        }.EnableIntercept(sp)
    );
            
    services.AddHttpClientInterceptor();
    services.AddScoped<HttpInterceptorService>();
            
    services.AddScoped<BudgetUsHttpClient>();
    services.AddScoped<AuthenticationService>();
    services.AddScoped<RefreshTokenService>();
            
    services.AddScoped<LocalStorageService>();
            
    services.AddOptions();
    services.AddAuthorizationCore();
    services.AddScoped<TokenAuthenticationStateProvider>();
    services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<TokenAuthenticationStateProvider>());
            
}