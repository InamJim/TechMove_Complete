using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace TechMoveTests.IntegrationTests
{
    /// <summary>
    /// Integration tests for the ServiceRequests API endpoint.
    /// </summary>
    public class ServiceRequestsIntegrationTests : IClassFixture<ApiWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ServiceRequestsIntegrationTests(ApiWebApplicationFactory factory)
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
        public async Task GetServiceRequests_WithValidToken_Returns200()
        {
            await AuthorizeAsync();

            var response = await _client.GetAsync("/api/servicerequests");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var list = await response.Content.ReadFromJsonAsync<List<ServiceRequestDto>>();
            Assert.NotNull(list);
        }

        [Fact]
        public async Task CreateServiceRequest_ForActiveContract_Returns201()
        {
            // Arrange
            await AuthorizeAsync();

            // Get an active contract
            var contracts = await _client.GetFromJsonAsync<List<ContractDto>>("/api/contracts");
            Assert.NotNull(contracts);
            var active = contracts.FirstOrDefault(c => c.Status == "Active");
            Assert.NotNull(active);

            // Act
            var response = await _client.PostAsJsonAsync("/api/servicerequests", new
            {
                ContractId = active.Id,
                Description = "Integration test service request",
                AmountUSD = 150.00m
            });

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var created = await response.Content.ReadFromJsonAsync<ServiceRequestDto>();
            Assert.NotNull(created);
            Assert.Equal(active.Id, created.ContractId);
            Assert.Equal("Pending", created.Status);
            Assert.True(created.CostUSD > 0);
            Assert.True(created.CostZAR > 0);
        }

        [Fact]
        public async Task CreateThenRead_ServiceRequest_DataIntegrity()
        {
            // Arrange
            await AuthorizeAsync();

            var contracts = await _client.GetFromJsonAsync<List<ContractDto>>("/api/contracts");
            Assert.NotNull(contracts);
            var active = contracts.FirstOrDefault(c => c.Status == "Active");
            Assert.NotNull(active);

            // Act – Create
            var createResponse = await _client.PostAsJsonAsync("/api/servicerequests", new
            {
                ContractId = active.Id,
                Description = "Data integrity check request",
                AmountUSD = 200.00m
            });
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
            var created = await createResponse.Content.ReadFromJsonAsync<ServiceRequestDto>();
            Assert.NotNull(created);

            // Act – Read all and find the created record
            var listResponse = await _client.GetAsync("/api/servicerequests");
            var list = await listResponse.Content.ReadFromJsonAsync<List<ServiceRequestDto>>();

            // Assert
            Assert.NotNull(list);
            var found = list.FirstOrDefault(r => r.Id == created.Id);
            Assert.NotNull(found);
            Assert.Equal("Data integrity check request", found.Description);
        }

        [Fact]
        public async Task CreateServiceRequest_WithoutToken_Returns401()
        {
            var response = await _client.PostAsJsonAsync("/api/servicerequests", new
            {
                ContractId = 1,
                Description = "Should fail",
                AmountUSD = 100m
            });

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        private class ServiceRequestDto
        {
            public int Id { get; set; }
            public int ContractId { get; set; }
            public string Description { get; set; } = string.Empty;
            public decimal CostUSD { get; set; }
            public decimal CostZAR { get; set; }
            public string Status { get; set; } = string.Empty;
        }

        private class ContractDto
        {
            public int Id { get; set; }
            public string Status { get; set; } = string.Empty;
        }

        private class TokenResponse
        {
            public string Token { get; set; } = string.Empty;
        }
    }
}
