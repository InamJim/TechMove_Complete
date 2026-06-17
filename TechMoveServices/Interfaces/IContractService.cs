using Microsoft.AspNetCore.Http;
using TechMoveData.Entities;
using TechMoveData.Enums;

namespace TechMoveServices.Interfaces
{
    public interface IContractService
    {
        Task<Contract> CreateContractAsync ( Contract contract, IFormFile? file );
        Task<List<Contract>> GetAllAsync ();
        Task<Contract?> GetByIdAsync ( int id );
        Task UpdateStatusAsync ( int id, ContractStatus status );
        Task DeleteAsync ( int id );
        Task<List<Contract>> SearchAsync (
                    int? clientId,
                    ContractStatus? status,
                    DateTime? startDate,
                    DateTime? endDate );
    }
}