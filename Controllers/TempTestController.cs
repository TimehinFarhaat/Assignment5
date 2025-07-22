using Microsoft.AspNetCore.Mvc;

namespace CSharpMvcBasics.Controllers
{

    public class TempTestController : Controller
    {
        public IActionResult Set()
        {
            TempData["Test"] = "✅ TempData is working!";
            return RedirectToAction("Show");
        }

        public IActionResult Show()
        {
            return Content(TempData["Test"]?.ToString() ?? "❌ TempData is NOT working.");
        }
    }

}
