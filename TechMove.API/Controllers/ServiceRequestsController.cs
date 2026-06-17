using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechMove.API.DTOs;
using TechMoveData.Entities;
using TechMoveServices.Interfaces;

namespace TechMove.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ServiceRequestsController : ControllerBase
    {
        private readonly IServiceRequestService _service;

        public ServiceRequestsController(IServiceRequestService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var requests = await _service.GetAllAsync();

            var result = requests.Select(r => new ServiceRequestDto
            {
                Id = r.Id,
                ContractId = r.ContractId,
                Description = r.Description,
                CostUSD = r.CostUSD,
                CostZAR = r.CostZAR,
                Status = r.Status
            });

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateServiceRequestDto dto)
        {
            try
            {
                var request = new ServiceRequest
                {
                    ContractId = dto.ContractId,
                    Description = dto.Description,
                    Status = "Pending"
                };

                var created = await _service.CreateAsync(request, dto.AmountUSD);

                var result = new ServiceRequestDto
                {
                    Id = created.Id,
                    ContractId = created.ContractId,
                    Description = created.Description,
                    CostUSD = created.CostUSD,
                    CostZAR = created.CostZAR,
                    Status = created.Status
                };

                return CreatedAtAction(nameof(GetAll), new { id = created.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
