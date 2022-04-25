using Budgeteer.Web.Auth.Models;

namespace Budgeteer.Web.Auth
{
    public class RefreshTokenService
    {
        private readonly TokenAuthenticationStateProvider _authProvider;
        private readonly AuthenticationService _authService;
        
        public RefreshTokenService(TokenAuthenticationStateProvider authProvider, AuthenticationService authService)
        {
            _authProvider = authProvider;
            _authService = authService;
        }
        
        public async Task<(bool, User?)> TryRefreshActiveSession()
        {
            var authState = await _authProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            var exp = user.FindFirst(c => c.Type.Equals("exp"))?.Value;
            var expTime = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(exp));
            var timeUtc = DateTime.UtcNow;
            var diff = expTime - timeUtc;
            
            return diff.TotalMinutes <= 2 
                ? (true, await _authService.RefreshCurrentUserToken())
                : (false, await _authService.GetActiveUserAsync());
        }
    }
}