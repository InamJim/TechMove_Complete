using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace TechMoveTests.IntegrationTests
{

    public class ContractsIntegrationTests : IClassFixture<ApiWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ContractsIntegrationTests(ApiWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }


        private async Task AuthorizeAsync()
        {
            var response = await _client.PostAsJsonAsync(
                "/api/auth/login",
                new { Username = "admin", Password = "admin123" });

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadFromJsonAsync<TokenResponse>();
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", body!.Token);
        }


        [Fact]
        public async Task GetContracts_WithoutToken_Returns401Unauthorized()
        {
            var response = await _client.GetAsync("/api/contracts");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetContracts_WithValidToken_Returns200AndNonNullList()
        {
            await AuthorizeAsync();

            var response = await _client.GetAsync("/api/contracts");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var contracts = await response.Content.ReadFromJsonAsync<List<ContractDto>>();
            Assert.NotNull(contracts);
        }

        [Fact]
        public async Task GetContracts_WithValidToken_ReturnsSeedData()
        {
            await AuthorizeAsync();

            
            var response = await _client.GetAsync("/api/contracts");
            var contracts = await response.Content.ReadFromJsonAsync<List<ContractDto>>();

            Assert.NotNull(contracts);
            Assert.True(contracts.Count >= 1, "Expected at least one seeded contract");
        }

        [Fact]
        public async Task GetContractById_WhenExists_Returns200AndContract()
        {

            await AuthorizeAsync();
            var list = await _client.GetFromJsonAsync<List<ContractDto>>("/api/contracts");
            Assert.NotNull(list);
            Assert.NotEmpty(list);

            var seedId = list.First().Id;

            var response = await _client.GetAsync($"/api/contracts/{seedId}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var contract = await response.Content.ReadFromJsonAsync<ContractDto>();
            Assert.NotNull(contract);
            Assert.Equal(seedId, contract.Id);
        }

        [Fact]
        public async Task GetContractById_WhenNotFound_Returns404()
        {
            // Arrange
            await AuthorizeAsync();

            // Act
            var response = await _client.GetAsync("/api/contracts/999999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CreateContract_WithValidData_Returns201AndCreatedContract()
        {
            // Arrange
            await AuthorizeAsync();

            // Get a valid client ID from the API
            var clients = await _client.GetFromJsonAsync<List<ClientDto>>("/api/clients");
            Assert.NotNull(clients);
            Assert.NotEmpty(clients);
            var clientId = clients.First().Id;

            using var form = new MultipartFormDataContent();
            form.Add(new StringContent(clientId.ToString()), "ClientId");
            form.Add(new StringContent(DateTime.Today.ToString("yyyy-MM-dd")), "StartDate");
            form.Add(new StringContent(DateTime.Today.AddMonths(3).ToString("yyyy-MM-dd")), "EndDate");
            form.Add(new StringContent("Active"), "Status");
            form.Add(new StringContent("Premium"), "ServiceLevel");

            // Act
            var response = await _client.PostAsync("/api/contracts", form);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var created = await response.Content.ReadFromJsonAsync<ContractDto>();
            Assert.NotNull(created);
            Assert.True(created.Id > 0, "Created contract should have a valid ID");
            Assert.Equal(clientId, created.ClientId);
            Assert.Equal("Premium", created.ServiceLevel);
        }

        [Fact]
        public async Task CreateThenRead_DataIntegrity_ContractAppearsInList()
        {
            // Arrange
            await AuthorizeAsync();

            var clients = await _client.GetFromJsonAsync<List<ClientDto>>("/api/clients");
            Assert.NotNull(clients);
            var clientId = clients.First().Id;

            using var form = new MultipartFormDataContent();
            form.Add(new StringContent(clientId.ToString()), "ClientId");
            form.Add(new StringContent(DateTime.Today.ToString("yyyy-MM-dd")), "StartDate");
            form.Add(new StringContent(DateTime.Today.AddMonths(6).ToString("yyyy-MM-dd")), "EndDate");
            form.Add(new StringContent("Draft"), "Status");
            form.Add(new StringContent("Enterprise"), "ServiceLevel");

            // Act – Create
            var createResponse = await _client.PostAsync("/api/contracts", form);
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

            var created = await createResponse.Content.ReadFromJsonAsync<ContractDto>();
            Assert.NotNull(created);

            // Act – Read back the specific contract
            var getResponse = await _client.GetAsync($"/api/contracts/{created.Id}");
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var fetched = await getResponse.Content.ReadFromJsonAsync<ContractDto>();

            // Assert – Data integrity verified
            Assert.NotNull(fetched);
            Assert.Equal(created.Id, fetched.Id);
            Assert.Equal("Enterprise", fetched.ServiceLevel);
        }

        [Fact]
        public async Task UpdateContractStatus_WithValidStatus_Returns204()
        {
            // Arrange
            await AuthorizeAsync();

            var list = await _client.GetFromJsonAsync<List<ContractDto>>("/api/contracts");
            Assert.NotNull(list);
            var id = list.First().Id;

            // Act
            var response = await _client.PatchAsJsonAsync(
                $"/api/contracts/{id}/status",
                new { Status = "OnHold" });

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task UpdateContractStatus_WithInvalidStatus_Returns400()
        {
            // Arrange
            await AuthorizeAsync();

            var list = await _client.GetFromJsonAsync<List<ContractDto>>("/api/contracts");
            Assert.NotNull(list);
            var id = list.First().Id;

            // Act
            var response = await _client.PatchAsJsonAsync(
                $"/api/contracts/{id}/status",
                new { Status = "InvalidStatus" });

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task DeleteContract_WhenExists_Returns204()
        {
            // Arrange
            await AuthorizeAsync();

            var clients = await _client.GetFromJsonAsync<List<ClientDto>>("/api/clients");
            Assert.NotNull(clients);
            var clientId = clients.First().Id;

            // Create a contract to delete
            using var form = new MultipartFormDataContent();
            form.Add(new StringContent(clientId.ToString()), "ClientId");
            form.Add(new StringContent(DateTime.Today.ToString("yyyy-MM-dd")), "StartDate");
            form.Add(new StringContent(DateTime.Today.AddMonths(1).ToString("yyyy-MM-dd")), "EndDate");
            form.Add(new StringContent("Draft"), "Status");
            form.Add(new StringContent("Standard"), "ServiceLevel");

            var createResp = await _client.PostAsync("/api/contracts", form);
            var created = await createResp.Content.ReadFromJsonAsync<ContractDto>();
            Assert.NotNull(created);

            // Act
            var response = await _client.DeleteAsync($"/api/contracts/{created.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Confirm it's gone
            var getResp = await _client.GetAsync($"/api/contracts/{created.Id}");
            Assert.Equal(HttpStatusCode.NotFound, getResp.StatusCode);
        }

        // ── Local DTOs ────────────────────────────────────────────────────────

        private class ContractDto
        {
            public int Id { get; set; }
            public int ClientId { get; set; }
            public string ClientName { get; set; } = string.Empty;
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string Status { get; set; } = string.Empty;
            public string ServiceLevel { get; set; } = string.Empty;
        }

        private class ClientDto
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }

        private class TokenResponse
        {
            public string Token { get; set; } = string.Empty;
        }
    }
}
