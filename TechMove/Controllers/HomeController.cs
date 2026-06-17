using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TechMove.ApiServices;
using TechMove.Models;

namespace TechMove.Controllers
{
    /// <summary>
    /// MVC Controller — fetches dashboard stats from Web API only.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DashboardApiService _dashboardApi;

        public HomeController(
            ILogger<HomeController> logger,
            DashboardApiService dashboardApi)
        {
            _logger = logger;
            _dashboardApi = dashboardApi;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var vm = await _dashboardApi.GetDashboardAsync();
                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load dashboard from API");
                return View(new DashboardViewModel());
            }
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
