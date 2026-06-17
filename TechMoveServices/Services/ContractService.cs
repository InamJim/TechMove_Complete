using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TechMoveData.Entities;
using TechMoveData.Enums;
using TechMoveRepo.Interfaces;
using TechMoveServices.Interfaces;

namespace TechMoveServices.Services
{
    public class ContractService : IContractService
    {
        private readonly IRepository<Contract> _repo;
        private readonly FileService _fileService;
        private readonly ILogger<ContractService> _logger;

        public ContractService (
            IRepository<Contract> repo,
            FileService fileService,
            ILogger<ContractService> logger )
        {
            _repo = repo;
            _fileService = fileService;
            _logger = logger;
        }

        public async Task<Contract> CreateContractAsync ( Contract contract, IFormFile? file )
        {
            if ( file != null && file.Length > 0 )
            {
                var path = await _fileService.SavePdfAsync(file);
                contract.AgreementFilePath = path;
            }
            else
            {
                contract.AgreementFilePath = "NO_FILE";
            }

            if ( contract.StartDate == default )
                contract.StartDate = DateTime.Now;

            if ( contract.EndDate == default )
                contract.EndDate = DateTime.Now.AddMonths(6);

            if ( string.IsNullOrEmpty(contract.ServiceLevel) )
                contract.ServiceLevel = "Standard";

            return await _repo.AddAsync(contract);
        }

        public async Task<List<Contract>> GetAllAsync ()
        {
            return await _repo.GetAllWithIncludeAsync(c => c.Client);
        }

        public async Task<List<Contract>> SearchAsync ( int? clientId, ContractStatus? status, DateTime? startDate, DateTime? endDate )
        {
            var query = (await _repo.GetAllAsync()).AsQueryable();

            if ( clientId.HasValue )
                query = query.Where(x => x.ClientId == clientId.Value);

            if ( status.HasValue )
                query = query.Where(x => x.Status == status.Value);

            if ( startDate.HasValue )
                query = query.Where(x => x.StartDate >= startDate.Value);

            if ( endDate.HasValue )
                query = query.Where(x => x.EndDate <= endDate.Value);

            return query.ToList();
        }

        public async Task DeleteAsync ( int id )
        {
            var contract = await _repo.GetByIdAsync(id);

            if ( contract == null )
                throw new Exception("Contract not found");

            await _repo.DeleteAsync(contract);

            _logger.LogInformation("Contract deleted successfully");
        }

        public async Task<Contract?> GetByIdAsync ( int id )
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task UpdateStatusAsync (
           int id,
           ContractStatus status )
        {
            var contract = await _repo.GetByIdAsync(id);

            if ( contract == null )
                throw new Exception("Contract not found");

            contract.Status = status;

            await _repo.UpdateAsync(contract);
        }
    }
}