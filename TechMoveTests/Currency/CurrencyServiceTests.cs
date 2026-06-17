using Moq;
using TechMoveServices.Interfaces;
using TechMoveServices.Services;
using TechMoveServices.Strategies;
using Xunit;

namespace TechMoveTests.Currency
{
    public class CurrencyServiceTests
    {
        [Fact]
        public async Task ConvertUsdToZar_ReturnsCorrectValue()
        {
            var apiMock = new Mock<IExchangeRateApiService>();

            apiMock.Setup(x => x.GetUsdToZarRateAsync())
                   .ReturnsAsync(20);

            var strategy = new USDToZARStrategy();

            var service = new CurrencyService(
                apiMock.Object,
                strategy);

            var result = await service.ConvertUsdToZarAsync(10);

            Assert.Equal(200, result);
        }

        [Fact]
        public async Task ConvertUsdToZar_Zero_ReturnsZero()
        {
            var apiMock = new Mock<IExchangeRateApiService>();

            apiMock.Setup(x => x.GetUsdToZarRateAsync())
                   .ReturnsAsync(20);

            var strategy = new USDToZARStrategy();

            var service = new CurrencyService(
                apiMock.Object,
                strategy);

            var result = await service.ConvertUsdToZarAsync(0);

            Assert.Equal(0, result);
        }

        [Fact]
        public async Task ConvertUsdToZar_LargeAmount_ReturnsCorrectValue()
        {
            var apiMock = new Mock<IExchangeRateApiService>();

            apiMock.Setup(x => x.GetUsdToZarRateAsync())
                   .ReturnsAsync(18);

            var strategy = new USDToZARStrategy();

            var service = new CurrencyService(
                apiMock.Object,
                strategy);

            var result = await service.ConvertUsdToZarAsync(100000);

            Assert.Equal(1800000, result);
        }
    }
}