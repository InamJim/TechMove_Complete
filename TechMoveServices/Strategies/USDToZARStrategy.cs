namespace TechMoveServices.Strategies
{
    public class USDToZARStrategy : ICurrencyStrategy
    {
        public Task<decimal> ConvertAsync(decimal amount, decimal rate)
        {
            return Task.FromResult(amount * rate);
        }
    }
}