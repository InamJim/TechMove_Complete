using System.Net.Http.Headers;
using System.Net.Http.Json;
using TechMove.Models;

namespace TechMove.ApiServices
{
    /// <summary>
    /// MVC service that fetches dashboard stats from the Web API.
    /// </summary>
    public class DashboardApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ApiTokenService _tokenService;

        public DashboardApiService(HttpClient httpClient, ApiTokenService tokenService)
        {
            _httpClient = httpClient;
            _tokenService = tokenService;
        }

        private async Task AuthorizeAsync()
        {
            var token = await _tokenService.GetTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<DashboardViewModel> GetDashboardAsync()
        {
            await AuthorizeAsync();
            return await _httpClient.GetFromJsonAsync<DashboardViewModel>("api/dashboard")
                ?? new DashboardViewModel();
        }
    }
}
