using EVrtic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EVrtic.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(RedirectByRole));
            }

            return Redirect("/Identity/Account/Login");
        }

        [Authorize]
        public IActionResult RedirectByRole()
        {
            if (User.IsInRole("ADMINISTRATOR"))
            {
                return RedirectToAction(nameof(AdministratorHome));
            }

            if (User.IsInRole("ODGAJATELJ"))
            {
                return RedirectToAction(nameof(OdgajateljHome));
            }

            if (User.IsInRole("RODITELJ"))
            {
                return RedirectToAction(nameof(RoditeljHome));
            }

            return Redirect("/Identity/Account/Login");
        }

        [Authorize(Roles = "RODITELJ")]
        public IActionResult RoditeljHome()
        {
            return View();
        }

        [Authorize(Roles = "ODGAJATELJ")]
        public IActionResult OdgajateljHome()
        {
            return View();
        }

        [Authorize(Roles = "ADMINISTRATOR")]
        public IActionResult AdministratorHome()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

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