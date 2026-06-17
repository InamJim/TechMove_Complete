namespace TechMove.API.DTOs
{
    public class ServiceRequestDto
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal CostUSD { get; set; }
        public decimal CostZAR { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class CreateServiceRequestDto
    {
        public int ContractId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal AmountUSD { get; set; }
    }
}
