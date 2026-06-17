using TechMoveData.Entities;

namespace TechMoveServices.Interfaces
{
    public interface IServiceRequestService
    {
        Task<ServiceRequest> CreateAsync(ServiceRequest request, decimal usdAmount);
        Task<List<ServiceRequest>> GetAllAsync();
    }
}