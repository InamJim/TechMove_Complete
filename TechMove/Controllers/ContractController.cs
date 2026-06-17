using Microsoft.AspNetCore.Mvc;
using TechMove.ApiServices;

namespace TechMove.Controllers
{
    /// <summary>
    /// MVC Controller — uses ContractApiService (HttpClient) exclusively.
    /// Zero direct database access.
    /// </summary>
    public class ContractController : Controller
    {
        private readonly ContractApiService _contractApi;
        private readonly ClientApiService _clientApi;
        private readonly ILogger<ContractController> _logger;

        public ContractController(
            ContractApiService contractApi,
            ClientApiService clientApi,
            ILogger<ContractController> logger)
        {
            _contractApi = contractApi;
            _clientApi = clientApi;
            _logger = logger;
        }

        // GET: /Contract
        public async Task<IActionResult> Index()
        {
            try
            {
                var contracts = await _contractApi.GetContractsAsync();
                return View(contracts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load contracts from API");
                TempData["Error"] = "Could not load contracts. API may be unavailable.";
                return View(new List<TechMove.Models.DTOs.ContractDto>());
            }
        }

        // GET: /Contract/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                var clients = await _clientApi.GetClientsAsync();
                ViewBag.Clients = clients;
            }
            catch
            {
                ViewBag.Clients = new List<object>();
                TempData["Error"] = "Could not load clients from API.";
            }

            return View();
        }

        // POST: /Contract/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            int clientId,
            DateTime startDate,
            DateTime endDate,
            string status,
            string serviceLevel,
            IFormFile? file)
        {
            try
            {
                await _contractApi.CreateContractAsync(
                    clientId, startDate, endDate, status, serviceLevel, file);

                TempData["Success"] = "Contract created successfully.";
                _logger.LogInformation("Contract created via API.");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Contract creation failed");
                ModelState.AddModelError("", ex.Message);
                ViewBag.Clients = await _clientApi.GetClientsAsync();
                return View();
            }
        }

        // GET: /Contract/Search
        public async Task<IActionResult> Search(
            int? clientId,
            string? status,
            DateTime? startDate,
            DateTime? endDate)
        {
            try
            {
                var result = await _contractApi.GetContractsAsync(
                    clientId, status, startDate, endDate);
                return View("Index", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Contract search failed");
                TempData["Error"] = "Search failed. API may be unavailable.";
                return View("Index", new List<TechMove.Models.DTOs.ContractDto>());
            }
        }

        // GET: /Contract/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var contract = await _contractApi.GetContractByIdAsync(id);
                if (contract == null) return NotFound();
                return View(contract);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not load contract for delete");
                return NotFound();
            }
        }

        // POST: /Contract/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _contractApi.DeleteContractAsync(id);
                TempData["Success"] = "Contract deleted successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Contract delete failed");
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
