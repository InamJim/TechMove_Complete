using Microsoft.AspNetCore.Mvc;
using TechMove.ApiServices;

namespace TechMove.Controllers
{
    /// <summary>
    /// MVC Controller — uses ServiceRequestApiService (HttpClient) exclusively.
    /// Zero direct database access.
    /// </summary>
    public class ServiceRequestController : Controller
    {
        private readonly ServiceRequestApiService _serviceRequestApi;
        private readonly ContractApiService _contractApi;
        private readonly ILogger<ServiceRequestController> _logger;

        public ServiceRequestController(
            ServiceRequestApiService serviceRequestApi,
            ContractApiService contractApi,
            ILogger<ServiceRequestController> logger)
        {
            _serviceRequestApi = serviceRequestApi;
            _contractApi = contractApi;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var requests = await _serviceRequestApi.GetAllAsync();
                return View(requests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load service requests from API");
                TempData["Error"] = "Could not load service requests. API may be unavailable.";
                return View(new List<TechMove.Models.DTOs.ServiceRequestDto>());
            }
        }

        public async Task<IActionResult> Create()
        {
            try
            {
                ViewBag.Contracts = await _contractApi.GetContractsAsync();
            }
            catch
            {
                ViewBag.Contracts = new List<object>();
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            int contractId, string description, decimal usdAmount)
        {
            try
            {
                await _serviceRequestApi.CreateAsync(contractId, description, usdAmount);
                TempData["Success"] = "Service Request created successfully.";
                _logger.LogInformation("Service Request created via API.");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Service Request creation failed");
                ModelState.AddModelError("", ex.Message);
                ViewBag.Contracts = await _contractApi.GetContractsAsync();
                return View();
            }
        }
    }
}
