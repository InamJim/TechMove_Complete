using Microsoft.AspNetCore.Http;
using TechMoveData.Enums;

namespace TechMove.API.DTOs
{
    public class CreateContractRequestDto
    {
        public int ClientId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public ContractStatus Status { get; set; }

        public string ServiceLevel { get; set; }

        public IFormFile? File { get; set; }
    }
}