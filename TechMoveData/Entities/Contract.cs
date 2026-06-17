using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TechMoveData.Enums;

namespace TechMoveData.Entities
{
    public class Contract
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Client ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Client ID must be valid")]
        public int ClientId { get; set; }

        [ValidateNever]
        public Client Client { get; set; } = null!;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public ContractStatus Status { get; set; }

        public string ServiceLevel { get; set; } = string.Empty;

        public string? AgreementFilePath { get; set; }

        [ValidateNever]
        public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
    }
}