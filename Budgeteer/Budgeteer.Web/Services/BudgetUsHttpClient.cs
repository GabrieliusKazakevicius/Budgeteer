using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Budgeteer.Web.Auth.Models;
using Microsoft.AspNetCore.Components;

namespace Budgeteer.Web.Services
{
    public class BudgetUsHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly LocalStorageService _localStorageService;
        private readonly NavigationManager _navigationManager;

        public BudgetUsHttpClient(HttpClient httpClient, LocalStorageService localStorageService, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _localStorageService = localStorageService;
            _navigationManager = navigationManager;
        }
        
        public async Task<TResponse?> GetAsync<TResponse>(string uri, IDictionary<string, string> arguments)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            return await SendRequestAsync<TResponse>(request);      
        }
        
        public async Task<TResponse?> PostAsync<TResponse>(string uri, object requestModel)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = new StringContent(JsonSerializer.Serialize(requestModel), Encoding.UTF8, "application/json")
            };
            
            return await SendRequestAsync<TResponse>(request);
        }
        
        private async Task<T?> SendRequestAsync<T>(HttpRequestMessage request)
        {
            // add jwt auth header if user is logged in and request is to the api url
            var user = await _localStorageService.GetItemAsync<User>("user");
            var isApiUrl = request.RequestUri is not null && !request.RequestUri.IsAbsoluteUri;
            if (user != null && isApiUrl)
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);

            using var response = await _httpClient.SendAsync(request);

            // auto logout on 401 response
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                _navigationManager.NavigateTo("logout");
                return default;
            }
            
            if (response.IsSuccessStatusCode) 
                return await response.Content.ReadFromJsonAsync<T>();
            
            // throw exception on error response
            var error = await response.Content.ReadAsStringAsync();
            if (error != null)
            {
                throw new Exception(error);    
            }

            return await response.Content.ReadFromJsonAsync<T>();
        }
    }
}