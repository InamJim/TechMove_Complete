namespace TechMoveServices.Strategies
{
    public interface ICurrencyStrategy
    {
        Task<decimal> ConvertAsync(decimal amount, decimal rate);
    }
}