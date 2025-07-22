using Microsoft.AspNetCore.Mvc;
using CSharpMvcBasics.Interface.Services;


namespace MvcCvProject.Controllers
{
    public class TaxController : Controller
    {
        private readonly ITaxService _taxService;

        public TaxController(ITaxService taxService)
        {
            _taxService = taxService;
        }

        [HttpGet]
        public IActionResult CalculateTax() => View();

        [HttpPost]
        public IActionResult CalculateTax(decimal monthlyIncome)
        {
            if (monthlyIncome <= 0)
            {
                ModelState.AddModelError("monthlyIncome", "Monthly income must be greater than zero.");
                return View();
            }

            var taxResult = _taxService.CalculateTaxableIncome(monthlyIncome);
            return View(taxResult);
        }
    }
}
    
