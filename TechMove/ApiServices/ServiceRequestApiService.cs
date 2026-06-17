using System.Net.Http.Headers;
using System.Net.Http.Json;
using TechMove.Models.DTOs;

namespace TechMove.ApiServices
{
    /// <summary>
    /// MVC service that calls the Web API for service request operations.
    /// </summary>
    public class ServiceRequestApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ApiTokenService _tokenService;

        public ServiceRequestApiService(HttpClient httpClient, ApiTokenService tokenService)
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

        public async Task<List<ServiceRequestDto>> GetAllAsync()
        {
            await AuthorizeAsync();
            return await _httpClient.GetFromJsonAsync<List<ServiceRequestDto>>("api/servicerequests")
                ?? new List<ServiceRequestDto>();
        }

        public async Task<ServiceRequestDto?> CreateAsync(
            int contractId, string description, decimal amountUSD)
        {
            await AuthorizeAsync();

            var response = await _httpClient.PostAsJsonAsync("api/servicerequests", new
            {
                ContractId = contractId,
                Description = description,
                AmountUSD = amountUSD
            });

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ServiceRequestDto>();
        }
    }
}
