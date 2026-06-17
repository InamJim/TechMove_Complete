using TechMoveData.Entities;
using TechMoveData.Enums;
using TechMoveRepo.Interfaces;
using TechMoveServices.Interfaces;

namespace TechMoveServices.Services
{
    public class ServiceRequestService : IServiceRequestService
    {
        private readonly IRepository<ServiceRequest> _serviceRepo;
        private readonly IRepository<Contract> _contractRepo;
        private readonly CurrencyService _currencyService;

        public ServiceRequestService(
            IRepository<ServiceRequest> serviceRepo,
            IRepository<Contract> contractRepo,
            CurrencyService currencyService)
        {
            _serviceRepo = serviceRepo;
            _contractRepo = contractRepo;
            _currencyService = currencyService;
        }

        public async Task<ServiceRequest> CreateAsync(ServiceRequest request, decimal usdAmount)
        {
            var contract = await _contractRepo.GetByIdAsync(request.ContractId);

            if (contract == null)
                throw new Exception("Contract not found");

            if (contract.Status == ContractStatus.Expired ||
                contract.Status == ContractStatus.OnHold)
            {
                throw new Exception("Cannot create Service Request for inactive contract");
            }

            var zarAmount =
                await _currencyService.ConvertUsdToZarAsync(usdAmount);

            request.CostUSD = usdAmount;
            request.CostZAR = zarAmount;

            // IMPORTANT
            request.Status = "Pending";

            return await _serviceRepo.AddAsync(request);
        }

        public async Task<List<ServiceRequest>> GetAllAsync()
        {
            return await _serviceRepo.GetAllAsync();
        }
    }
}