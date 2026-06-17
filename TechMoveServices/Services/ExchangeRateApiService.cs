using System.Net.Http.Json;
using TechMoveServices.Interfaces;

namespace TechMoveServices.Services
{
    public class ExchangeRateApiService : IExchangeRateApiService
    {
        private readonly HttpClient _httpClient;

        public ExchangeRateApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<decimal> GetUsdToZarRateAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<ExchangeRateResponse>(
                "https://api.exchangerate-api.com/v4/latest/USD"
            );

            if (response == null || response.Rates == null)
                throw new Exception("Failed to fetch exchange rate");

            if (!response.Rates.TryGetValue("ZAR", out var rate))
                throw new Exception("ZAR rate not found");

            return rate;
        }
    }

    public class ExchangeRateResponse
    {
        public Dictionary<string, decimal> Rates { get; set; } = new();
    }
}