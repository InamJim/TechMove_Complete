using System.Net.Http.Headers;
using System.Net.Http.Json;
using TechMoveData.Entities;

namespace TechMove.ApiServices
{
    /// <summary>
    /// MVC service that fetches client data from the Web API.
    /// </summary>
    public class ClientApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ApiTokenService _tokenService;

        public ClientApiService(HttpClient httpClient, ApiTokenService tokenService)
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

        public async Task<List<Client>> GetClientsAsync()
        {
            await AuthorizeAsync();
            return await _httpClient.GetFromJsonAsync<List<Client>>("api/clients")
                ?? new List<Client>();
        }
    }
}
