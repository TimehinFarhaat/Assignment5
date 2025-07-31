using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CSharpMvcBasics.DTO;

namespace CSharpMvcBasics.Controllers
{
    public class CurrencyController : Controller
    {
        private readonly ICurrencyService _currencyService;

        public CurrencyController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = new CurrencyViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(CurrencyViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                model.Result = await _currencyService.ConvertCurrencyAsync(
                    model.FromCurrency.ToString(),
                    model.ToCurrency.ToString(),
                    model.Amount
                );
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Conversion failed: {ex.Message}");
            }

            return View(model);
        }
    }
}
