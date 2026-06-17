namespace TechMove.Models
{
    public class ServiceRequestViewModel
    {
        public int ContractId { get; set; }
        public string Description { get; set; } = string.Empty;

        public decimal CostUSD { get; set; }

        // Calculated automatically
        public decimal CostZAR { get; set; }
    }
}