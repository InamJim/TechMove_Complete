using System.Net.Http.Json;

namespace TechMove.ApiServices
{
    /// <summary>
    /// Handles obtaining a JWT token from the Web API.
    /// Registered as scoped — one token fetch per request scope.
    /// The token is cached within the scope lifetime.
    /// </summary>
    public class ApiTokenService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private string? _cachedToken;

        public ApiTokenService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<string> GetTokenAsync()
        {
            if (!string.IsNullOrEmpty(_cachedToken))
                return _cachedToken;

            var response = await _httpClient.PostAsJsonAsync("api/auth/login", new
            {
                Username = _config["ApiSettings:Username"] ?? "admin",
                Password = _config["ApiSettings:Password"] ?? "admin123"
            });

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
            _cachedToken = result?.Token
                ?? throw new InvalidOperationException("No token returned from API");

            return _cachedToken;
        }

        private class TokenResponse
        {
            public string Token { get; set; } = string.Empty;
        }
    }
}
