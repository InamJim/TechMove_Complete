using Moq;
using TechMoveData.Entities;
using TechMoveData.Enums;
using TechMoveRepo.Interfaces;
using TechMoveServices.Interfaces;
using TechMoveServices.Services;
using TechMoveServices.Strategies;
using Xunit;

namespace TechMoveTests.Workflow
{
    public class ContractWorkflowTests
    {
        [Fact]
        public async Task CreateRequest_ShouldThrow_WhenContractExpired()
        {
            var serviceRepoMock = new Mock<IRepository<ServiceRequest>>();
            var contractRepoMock = new Mock<IRepository<Contract>>();
            var currencyApiMock = new Mock<IExchangeRateApiService>();

            currencyApiMock.Setup(x => x.GetUsdToZarRateAsync())
                           .ReturnsAsync(20);

            contractRepoMock.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(new Contract
                {
                    Id = 1,
                    Status = ContractStatus.Expired
                });

            var currencyService = new CurrencyService(
                currencyApiMock.Object,
                new USDToZARStrategy()
            );

            var service = new ServiceRequestService(
                serviceRepoMock.Object,
                contractRepoMock.Object,
                currencyService
            );

            var request = new ServiceRequest
            {
                ContractId = 1,
                Description = "Test Request"
            };

            await Assert.ThrowsAsync<Exception>(() =>
                service.CreateAsync(request, 100));
        }

        [Fact]
        public async Task CreateRequest_ShouldThrow_WhenContractOnHold()
        {
            var serviceRepoMock = new Mock<IRepository<ServiceRequest>>();
            var contractRepoMock = new Mock<IRepository<Contract>>();
            var currencyApiMock = new Mock<IExchangeRateApiService>();

            currencyApiMock.Setup(x => x.GetUsdToZarRateAsync())
                           .ReturnsAsync(20);

            contractRepoMock.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(new Contract
                {
                    Id = 1,
                    Status = ContractStatus.OnHold
                });

            var currencyService = new CurrencyService(
                currencyApiMock.Object,
                new USDToZARStrategy()
            );

            var service = new ServiceRequestService(
                serviceRepoMock.Object,
                contractRepoMock.Object,
                currencyService
            );

            var request = new ServiceRequest
            {
                ContractId = 1,
                Description = "Blocked Request"
            };

            await Assert.ThrowsAsync<Exception>(() =>
                service.CreateAsync(request, 100));
        }
    }
}