using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SampleWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _log;

        public HomeController(ILogger<HomeController> log)
        {
            _log = log;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";
            _log.LogWarning("About page is shown");

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";
            _log.LogError("A bad thing happened!");

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
