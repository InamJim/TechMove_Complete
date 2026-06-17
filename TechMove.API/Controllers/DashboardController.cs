using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechMove.API.DTOs;
using TechMoveServices.Interfaces;

namespace TechMove.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IContractService _contractService;
        private readonly IServiceRequestService _serviceRequestService;

        public DashboardController(
            IContractService contractService,
            IServiceRequestService serviceRequestService)
        {
            _contractService = contractService;
            _serviceRequestService = serviceRequestService;
        }


        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            var contracts = await _contractService.GetAllAsync();
            var serviceRequests = await _serviceRequestService.GetAllAsync();

            var dto = new DashboardDto
            {
                TotalContracts = contracts.Count,
                ActiveContracts = contracts.Count(c =>
                    c.Status == TechMoveData.Enums.ContractStatus.Active),
                TotalServiceRequests = serviceRequests.Count
            };

            return Ok(dto);
        }
    }
}
