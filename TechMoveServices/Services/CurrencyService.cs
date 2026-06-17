using TechMoveServices.Interfaces;
using TechMoveServices.Strategies;

namespace TechMoveServices.Services
{
    public class CurrencyService
    {
        private readonly IExchangeRateApiService _apiService;
        private readonly ICurrencyStrategy _strategy;

        public CurrencyService(
            IExchangeRateApiService apiService,
            ICurrencyStrategy strategy)
        {
            _apiService = apiService;
            _strategy = strategy;
        }

        public async Task<decimal> ConvertUsdToZarAsync(decimal usdAmount)
        {
            var rate = await _apiService.GetUsdToZarRateAsync();
            return await _strategy.ConvertAsync(usdAmount, rate);
        }
    }
}