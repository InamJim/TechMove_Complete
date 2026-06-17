namespace TechMove.API.DTOs
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
    }
}