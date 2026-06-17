using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace TechMoveTests.IntegrationTests
{

    public class ClientsIntegrationTests : IClassFixture<ApiWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ClientsIntegrationTests(ApiWebApplicationFactory factory)
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
        public async Task GetClients_WithValidToken_Returns200AndNonEmptyList()
        {
            await AuthorizeAsync();

            var response = await _client.GetAsync("/api/clients");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var clients = await response.Content.ReadFromJsonAsync<List<ClientDto>>();
            Assert.NotNull(clients);
            Assert.NotEmpty(clients);
        }

        [Fact]
        public async Task GetClientById_WhenExists_Returns200()
        {
            await AuthorizeAsync();
            var clients = await _client.GetFromJsonAsync<List<ClientDto>>("/api/clients");
            Assert.NotNull(clients);
            var id = clients.First().Id;

            var response = await _client.GetAsync($"/api/clients/{id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var client = await response.Content.ReadFromJsonAsync<ClientDto>();
            Assert.NotNull(client);
            Assert.Equal(id, client.Id);
        }

        [Fact]
        public async Task GetClientById_WhenNotFound_Returns404()
        {
            await AuthorizeAsync();

            var response = await _client.GetAsync("/api/clients/999999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
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
