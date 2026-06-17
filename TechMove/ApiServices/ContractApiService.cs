using System.Net.Http.Headers;
using System.Net.Http.Json;
using TechMove.Models.DTOs;

namespace TechMove.ApiServices
{
    /// <summary>
    /// MVC service that calls the Web API for all contract operations.
    /// No database access here — pure HTTP calls.
    /// </summary>
    public class ContractApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ApiTokenService _tokenService;

        public ContractApiService(HttpClient httpClient, ApiTokenService tokenService)
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

        public async Task<List<ContractDto>> GetContractsAsync(
            int? clientId = null,
            string? status = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            await AuthorizeAsync();

            var query = BuildQuery(clientId, status, startDate, endDate);
            var url = "api/contracts" + query;

            return await _httpClient.GetFromJsonAsync<List<ContractDto>>(url)
                ?? new List<ContractDto>();
        }

        public async Task<ContractDto?> GetContractByIdAsync(int id)
        {
            await AuthorizeAsync();
            return await _httpClient.GetFromJsonAsync<ContractDto>($"api/contracts/{id}");
        }

        public async Task<ContractDto?> CreateContractAsync(
            int clientId,
            DateTime startDate,
            DateTime endDate,
            string status,
            string serviceLevel,
            IFormFile? file)
        {
            await AuthorizeAsync();

            using var form = new MultipartFormDataContent();
            form.Add(new StringContent(clientId.ToString()), "ClientId");
            form.Add(new StringContent(startDate.ToString("yyyy-MM-dd")), "StartDate");
            form.Add(new StringContent(endDate.ToString("yyyy-MM-dd")), "EndDate");
            form.Add(new StringContent(status), "Status");
            form.Add(new StringContent(serviceLevel), "ServiceLevel");

            if (file != null && file.Length > 0)
            {
                var fileContent = new StreamContent(file.OpenReadStream());
                fileContent.Headers.ContentType =
                    new MediaTypeHeaderValue(file.ContentType);
                form.Add(fileContent, "File", file.FileName);
            }

            var response = await _httpClient.PostAsync("api/contracts", form);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<ContractDto>();
        }

        public async Task UpdateStatusAsync(int id, string status)
        {
            await AuthorizeAsync();
            var response = await _httpClient.PatchAsJsonAsync(
                $"api/contracts/{id}/status", new { Status = status });
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteContractAsync(int id)
        {
            await AuthorizeAsync();
            var response = await _httpClient.DeleteAsync($"api/contracts/{id}");
            response.EnsureSuccessStatusCode();
        }

        private static string BuildQuery(
            int? clientId, string? status, DateTime? startDate, DateTime? endDate)
        {
            var parts = new List<string>();
            if (clientId.HasValue) parts.Add($"clientId={clientId}");
            if (!string.IsNullOrEmpty(status)) parts.Add($"status={status}");
            if (startDate.HasValue) parts.Add($"startDate={startDate:yyyy-MM-dd}");
            if (endDate.HasValue) parts.Add($"endDate={endDate:yyyy-MM-dd}");
            return parts.Count > 0 ? "?" + string.Join("&", parts) : string.Empty;
        }
    }
}
