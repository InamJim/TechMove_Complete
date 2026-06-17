using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TechMoveData.Context;
using TechMoveData.Entities;
using TechMoveData.Enums;
using TechMoveServices.Interfaces;

namespace TechMoveTests.IntegrationTests
{

    public class ApiWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var dbDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (dbDescriptor != null)
                    services.Remove(dbDescriptor);

                var dbName = "TechMoveTest_" + Guid.NewGuid().ToString("N");
                services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase(dbName));

                var exchangeDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IExchangeRateApiService));
                if (exchangeDescriptor != null)
                    services.Remove(exchangeDescriptor);

                services.AddSingleton<IExchangeRateApiService>(
                    new StubExchangeRateService());

                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureCreated();
                SeedTestData(db);
            });

            builder.UseEnvironment("Development");
        }

        private static void SeedTestData(AppDbContext db)
        {
            if (!db.Clients.Any())
            {
                db.Clients.AddRange(
                    new Client
                    {
                        Name = "Test Client Alpha",
                        ContactDetails = "alpha@test.com",
                        Region = "South Africa"
                    },
                    new Client
                    {
                        Name = "Test Client Beta",
                        ContactDetails = "beta@test.com",
                        Region = "Namibia"
                    });

                db.SaveChanges();
            }

            if (!db.Contracts.Any())
            {
                var client = db.Clients.First();

                db.Contracts.Add(new Contract
                {
                    ClientId = client.Id,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddMonths(6),
                    Status = ContractStatus.Active,
                    ServiceLevel = "Standard",
                    AgreementFilePath = "NO_FILE"
                });

                db.SaveChanges();
            }
        }
    }


    public class StubExchangeRateService : IExchangeRateApiService
    {
        public Task<decimal> GetUsdToZarRateAsync() => Task.FromResult(18m);
    }
}
