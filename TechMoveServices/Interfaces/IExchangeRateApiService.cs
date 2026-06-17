namespace TechMoveServices.Interfaces
{
    public interface IExchangeRateApiService
    {
        Task<decimal> GetUsdToZarRateAsync();
    }
}