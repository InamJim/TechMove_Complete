namespace TechMove.Models.DTOs
{
    public class ContractDto
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string ServiceLevel { get; set; } = string.Empty;
        public string? AgreementFilePath { get; set; }
    }

    public class ServiceRequestDto
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal CostUSD { get; set; }
        public decimal CostZAR { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
