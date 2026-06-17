namespace TechMove.Models.ViewModels
{
    public class ContractListViewModel
    {
        public int Id { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ServiceLevel { get; set; } = string.Empty;
        public string? AgreementFilePath { get; set; }
    }
}