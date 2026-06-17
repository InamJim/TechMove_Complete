using TechMoveData.Entities;
using TechMoveData.Enums;
using TechMoveData.Context;

namespace TechMoveData.Seed
{
    public static class DataSeeder
    {
        public static void Seed(AppDbContext context)
        {
            // -------------------------
            // SEED CLIENTS ONLY IF EMPTY
            // -------------------------
            if (!context.Clients.Any())
            {
                context.Clients.AddRange(
                    new Client
                    {
                        Name = "Global Freight Ltd",
                        ContactDetails = "contact@globalfreight.com",
                        Region = "South Africa"
                    },
                    new Client
                    {
                        Name = "Oceanic Logistics",
                        ContactDetails = "info@oceaniclogistics.com",
                        Region = "Namibia"
                    },
                    new Client
                    {
                        Name = "Skyline Transport",
                        ContactDetails = "support@skyline.com",
                        Region = "South Africa"
                    }
                );

                context.SaveChanges();
            }

            // -------------------------
            // SEED CONTRACTS ONLY IF EMPTY
            // -------------------------
            if (!context.Contracts.Any())
            {
                var client = context.Clients.First();

                context.Contracts.Add(new Contract
                {
                    ClientId = client.Id,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddMonths(6),
                    Status = ContractStatus.Active,
                    ServiceLevel = "Premium"
                });

                context.SaveChanges();
            }
        }
    }
}