using Microsoft.AspNetCore.Http;
using TechMoveData.Enums;

namespace TechMove.Models
{
    public class ContractViewModel
    {
        public int ClientId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ContractStatus Status { get; set; }
        public string ServiceLevel { get; set; } = string.Empty;

        public IFormFile? AgreementFile { get; set; }
    }
}