using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace CSharpMvcBasics.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Pages
        public IActionResult Index() => View();
        public IActionResult About() => View();
        public IActionResult Resume() => View();
        public IActionResult Projects() => View();
        public IActionResult Contact() => View();

    }
}
