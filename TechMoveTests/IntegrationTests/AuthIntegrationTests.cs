using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace TechMoveTests.IntegrationTests
{

    public class AuthIntegrationTests : IClassFixture<ApiWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public AuthIntegrationTests(ApiWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOkAndToken()
        {
            var loginRequest = new { Username = "admin", Password = "admin123" };

            
            var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var body = await response.Content.ReadFromJsonAsync<TokenResponse>();
            Assert.NotNull(body);
            Assert.NotNull(body.Token);
            Assert.NotEmpty(body.Token);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            var loginRequest = new { Username = "hacker", Password = "wrongpassword" };

            var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        private class TokenResponse
        {
            public string Token { get; set; } = string.Empty;
        }
    }
}
