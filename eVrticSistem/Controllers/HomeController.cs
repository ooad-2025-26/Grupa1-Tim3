using EVrtic.Data;
using EVrtic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Threading.Tasks;

namespace EVrtic.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToAction(nameof(RedirectByRole));

            return Redirect("/Identity/Account/Login");
        }

        [Authorize]
        public IActionResult RedirectByRole()
        {
            if (User.IsInRole("ADMINISTRATOR")) return RedirectToAction(nameof(AdministratorHome));
            if (User.IsInRole("ODGAJATELJ"))   return RedirectToAction(nameof(OdgajateljHome));
            if (User.IsInRole("RODITELJ"))     return RedirectToAction(nameof(RoditeljHome));
            return Redirect("/Identity/Account/Login");
        }

        [Authorize(Roles = "RODITELJ")]
        public IActionResult RoditeljHome() => View();

        [Authorize(Roles = "ODGAJATELJ")]
        public IActionResult OdgajateljHome() => View();

        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> AdministratorHome()
        {
            var vm = new AdministratorHomeViewModel
            {
                BrojOdgajatelja = await _context.Odgajatelji.CountAsync(),
                BrojRoditelja   = await _context.Roditelji.CountAsync(),
                BrojDjece       = await _context.Djeca.CountAsync(),
                BrojGrupa       = await _context.Grupe.CountAsync()
            };
            return View(vm);
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }

    public class AdministratorHomeViewModel
    {
        public int BrojOdgajatelja { get; set; }
        public int BrojRoditelja { get; set; }
        public int BrojDjece { get; set; }
        public int BrojGrupa { get; set; }
    }
}
