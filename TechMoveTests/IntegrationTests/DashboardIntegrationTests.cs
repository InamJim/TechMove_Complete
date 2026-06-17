using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace TechMoveTests.IntegrationTests
{
    /// <summary>
    /// Integration tests for the Dashboard API endpoint.
    /// </summary>
    public class DashboardIntegrationTests : IClassFixture<ApiWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public DashboardIntegrationTests(ApiWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        private async Task AuthorizeAsync()
        {
            var response = await _client.PostAsJsonAsync(
                "/api/auth/login",
                new { Username = "admin", Password = "admin123" });
            var body = await response.Content.ReadFromJsonAsync<TokenResponse>();
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", body!.Token);
        }

        [Fact]
        public async Task GetDashboard_WithoutToken_Returns401()
        {
            var response = await _client.GetAsync("/api/dashboard");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetDashboard_WithValidToken_Returns200AndCorrectShape()
        {
            // Arrange
            await AuthorizeAsync();

            // Act
            var response = await _client.GetAsync("/api/dashboard");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var dto = await response.Content.ReadFromJsonAsync<DashboardDto>();
            Assert.NotNull(dto);
            Assert.True(dto.TotalContracts >= 0);
            Assert.True(dto.ActiveContracts >= 0);
            Assert.True(dto.TotalServiceRequests >= 0);
        }

        [Fact]
        public async Task GetDashboard_ActiveContractsNotGreaterThanTotal()
        {
            // Arrange
            await AuthorizeAsync();

            // Act
            var response = await _client.GetAsync("/api/dashboard");
            var dto = await response.Content.ReadFromJsonAsync<DashboardDto>();

            // Assert – business rule: active can't exceed total
            Assert.NotNull(dto);
            Assert.True(dto.ActiveContracts <= dto.TotalContracts,
                "Active contracts should never exceed total contracts");
        }

        private class DashboardDto
        {
            public int TotalContracts { get; set; }
            public int ActiveContracts { get; set; }
            public int TotalServiceRequests { get; set; }
        }

        private class TokenResponse
        {
            public string Token { get; set; } = string.Empty;
        }
    }
}
