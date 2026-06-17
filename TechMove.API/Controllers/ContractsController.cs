using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechMove.API.DTOs;
using TechMoveData.Entities;
using TechMoveData.Enums;
using TechMoveServices.Interfaces;

namespace TechMove.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ContractsController : ControllerBase
    {
        private readonly IContractService _contractService;

        public ContractsController(IContractService contractService)
        {
            _contractService = contractService;
        }


        [HttpGet]
        public async Task<IActionResult> GetContracts(
            int? clientId,
            ContractStatus? status,
            DateTime? startDate,
            DateTime? endDate)
        {
            var contracts = await _contractService.SearchAsync(
                clientId, status, startDate, endDate);

            var result = contracts.Select(c => new ContractDto
            {
                Id = c.Id,
                ClientId = c.ClientId,
                ClientName = c.Client?.Name ?? string.Empty,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                Status = c.Status.ToString(),
                ServiceLevel = c.ServiceLevel
            });

            return Ok(result);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetContract(int id)
        {
            var contract = await _contractService.GetByIdAsync(id);

            if (contract == null)
                return NotFound(new { Message = $"Contract {id} not found" });

            var dto = new ContractDto
            {
                Id = contract.Id,
                ClientId = contract.ClientId,
                ClientName = contract.Client?.Name ?? string.Empty,
                StartDate = contract.StartDate,
                EndDate = contract.EndDate,
                Status = contract.Status.ToString(),
                ServiceLevel = contract.ServiceLevel
            };

            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateContract(
            [FromForm] CreateContractRequestDto dto)
        {
            try
            {
                var contract = new Contract
                {
                    ClientId = dto.ClientId,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    Status = dto.Status,
                    ServiceLevel = dto.ServiceLevel ?? "Standard"
                };

                var created = await _contractService.CreateContractAsync(
                    contract, dto.File);

                var result = new ContractDto
                {
                    Id = created.Id,
                    ClientId = created.ClientId,
                    StartDate = created.StartDate,
                    EndDate = created.EndDate,
                    Status = created.Status.ToString(),
                    ServiceLevel = created.ServiceLevel
                };

                return CreatedAtAction(
                    nameof(GetContract),
                    new { id = created.Id },
                    result);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message,
                    Detail = ex.InnerException?.Message
                });
            }
        }


        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(
            int id,
            [FromBody] UpdateContractStatusDto dto)
        {
            if (!Enum.TryParse<ContractStatus>(dto.Status, true, out var status))
            {
                return BadRequest(
                    $"Invalid status. Allowed: {string.Join(", ", Enum.GetNames(typeof(ContractStatus)))}");
            }

            var contract = await _contractService.GetByIdAsync(id);
            if (contract == null)
                return NotFound(new { Message = $"Contract {id} not found" });

            await _contractService.UpdateStatusAsync(id, status);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContract(int id)
        {
            var contract = await _contractService.GetByIdAsync(id);
            if (contract == null)
                return NotFound(new { Message = $"Contract {id} not found" });

            await _contractService.DeleteAsync(id);
            return NoContent();
        }
    }
}
