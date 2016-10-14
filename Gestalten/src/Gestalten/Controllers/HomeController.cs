using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Gestalten.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Documentation()
        {
            ViewData["Message"] = "....";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
