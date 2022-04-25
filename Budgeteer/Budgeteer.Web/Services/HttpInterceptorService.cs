using System.Net.Http.Headers;
using Budgeteer.Web.Auth;
using Toolbelt.Blazor;

namespace Budgeteer.Web.Services
{
    public class HttpInterceptorService
    {
        private readonly HttpClientInterceptor _interceptor;
        private readonly RefreshTokenService _refreshTokenService;

        public HttpInterceptorService(HttpClientInterceptor interceptor, RefreshTokenService refreshTokenService)
        {
            _interceptor = interceptor;
            _refreshTokenService = refreshTokenService;
        }
        
        public void RegisterEvent() => _interceptor.BeforeSendAsync += InterceptBeforeHttpAsync;
        public async Task InterceptBeforeHttpAsync(object sender, HttpClientInterceptorEventArgs e)
        {
            var absPath = e.Request.RequestUri.AbsolutePath; 
            if (!absPath.Contains("login") || !absPath.Contains("register") || !absPath.Contains("refresh"))
            {
                var (hasRefreshed, user) = await _refreshTokenService.TryRefreshActiveSession();
                if(hasRefreshed)
                {
                    e.Request.Headers.Authorization = new AuthenticationHeaderValue("bearer", user.Token);
                }
            }
        }
        public void DisposeEvent() => _interceptor.BeforeSendAsync -= InterceptBeforeHttpAsync;
    }
}