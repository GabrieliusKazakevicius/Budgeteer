using System.Security.Claims;
using Budgeteer.Web.Auth.Models;
using Budgeteer.Web.Services;
using Microsoft.AspNetCore.Components.Authorization;

namespace Budgeteer.Web.Auth
{
    public class TokenAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly AuthenticationState _anonymous;
        private readonly LocalStorageService _localStorageService;
        
        private const string UserCacheKey = "user";
        
        public TokenAuthenticationStateProvider(LocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
            _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
        
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var user = await GetActiveUserAsync();
            var identity = string.IsNullOrEmpty(user?.Token)
                ? new ClaimsIdentity()
                : new ClaimsIdentity(JwtHelper.ParseClaimsFromJwt(user.Token), "jwt");
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
        
        private async Task<User> GetActiveUserAsync()
        {
            var user = await _localStorageService.GetItemAsync<User>(UserCacheKey);
            if (user is null)
                return null;
            
            return user?.TokenValidUntil > DateTime.UtcNow 
                ? user 
                : null;
        }

        public void NotifyUserAuthentication(User? user)
        {
            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user.UserName) }, "jwtAuthType"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);
        }
        public void NotifyUserLogout()
        {
            var authState = Task.FromResult(_anonymous);
            NotifyAuthenticationStateChanged(authState);
        }
    }
}