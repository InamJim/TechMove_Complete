using Microsoft.AspNetCore.Mvc;
using TechMoveServices.Services;

namespace TechMove.Controllers
{
    [Route("api/currency")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly CurrencyService _currencyService;

        public CurrencyController(CurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        [HttpGet("convert")]
        public async Task<IActionResult> Convert(decimal usd)
        {
            if (usd <= 0)
                return BadRequest("USD amount must be greater than 0.");

            var zar = await _currencyService.ConvertUsdToZarAsync(usd);

            return Ok(new
            {
                usd,
                zar
            });
        }
    }
}