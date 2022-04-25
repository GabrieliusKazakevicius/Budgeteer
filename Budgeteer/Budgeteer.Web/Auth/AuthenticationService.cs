using System.Security.Claims;
using Budgeteer.Web.Auth.Models;
using Budgeteer.Web.Services;
using Microsoft.AspNetCore.Components;

namespace Budgeteer.Web.Auth
{
    public class AuthenticationService
    {
        private readonly BudgetUsHttpClient _httpClient;
        private readonly LocalStorageService _localStorageService;
        private readonly NavigationManager _navigationManager;
        private readonly TokenAuthenticationStateProvider _tokenAuthenticationStateProvider;
        
        private const string UserCacheKey = "user";
        
        public AuthenticationService(
            BudgetUsHttpClient httpService,
            NavigationManager navigationManager,
            LocalStorageService localStorageService, 
            TokenAuthenticationStateProvider tokenAuthenticationStateProvider)
        {
            _httpClient = httpService;
            _navigationManager = navigationManager;
            _localStorageService = localStorageService;
            _tokenAuthenticationStateProvider = tokenAuthenticationStateProvider;
        }

        public async Task<User?> GetActiveUserAsync() =>  await _localStorageService.GetItemAsync<User>(UserCacheKey); 
        
        public async Task Login(string email, string password)
        {
            var user = await _httpClient.PostAsync<User>("auth/login", new LoginDto
            {
                Username = email, 
                Password = password
            });
            
            PopulateUserPropertiesFromJwt(user);
            await _localStorageService.SetItemAsync(UserCacheKey, user);
            _tokenAuthenticationStateProvider.NotifyUserAuthentication(user);
            
            _navigationManager.NavigateTo("Index");
        }

        public async Task Register(string email, string password, string passwordRepeat)
        {
            var user = await _httpClient.PostAsync<User>("auth/register", new RegisterDto
            {
                Username = email,
                Password = password,
                PasswordRepeat = passwordRepeat
            });
            
            PopulateUserPropertiesFromJwt(user);
            await _localStorageService.SetItemAsync(UserCacheKey, user);
            _tokenAuthenticationStateProvider.NotifyUserAuthentication(user);
            
            _navigationManager.NavigateTo("Index");
        }

        public async Task<User> RefreshCurrentUserToken()
        {
            var user = await GetActiveUserAsync();
            var refreshedUser = await _httpClient.PostAsync<User>("auth/refresh", new TokenRefreshDto
            {
                Token = user.Token,
                RefreshToken = user.RefreshToken
            });
            
            PopulateUserPropertiesFromJwt(user);
            await _localStorageService.SetItemAsync(UserCacheKey, user);
            _tokenAuthenticationStateProvider.NotifyUserAuthentication(user);
            
            return refreshedUser;
        }
        
        public async Task Logout()
        {
            await _localStorageService.RemoveItemAsync(UserCacheKey);
            _tokenAuthenticationStateProvider.NotifyUserLogout();
            _navigationManager.NavigateTo("login");
        }

        private static void PopulateUserPropertiesFromJwt(User user)
        {
            var claims = JwtHelper.ParseClaimsFromJwt(user.Token);
            user.UserName = claims?.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Name))?.Value;
        }
    }
}