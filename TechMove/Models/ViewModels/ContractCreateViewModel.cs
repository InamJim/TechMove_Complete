using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechMoveData.Enums;

namespace TechMove.Models.ViewModels
{
    public class ContractCreateViewModel
    {
        [Required]
        public int ClientId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public ContractStatus Status { get; set; }

        [Required]
        public string ServiceLevel { get; set; } = string.Empty;

        public IFormFile? File { get; set; }

        public List<SelectListItem>? Clients { get; set; }
    }
}