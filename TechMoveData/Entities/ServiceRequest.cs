using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TechMoveData.Entities
{
    public class ServiceRequest
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Contract is required")]
        public int ContractId { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = string.Empty;

        public decimal CostUSD { get; set; }

        public decimal CostZAR { get; set; }

        // REQUIRED FOR DATABASE
        public string Status { get; set; } = "Pending";

        // Navigation Property
        [ValidateNever]
        public Contract? Contract { get; set; }
    }
}