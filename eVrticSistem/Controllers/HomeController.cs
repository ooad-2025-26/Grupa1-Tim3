using EVrtic.Data;
using EVrtic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVrtic.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Korisnik> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<Korisnik> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
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
        public async Task<IActionResult> OdgajateljHome()
        {
            var korisnik = await _userManager.GetUserAsync(User);
            if (korisnik == null) return Challenge();

            var odgajatelj = await _context.Odgajatelji
                .Include(o => o.Grupe)
                    .ThenInclude(g => g.Djeca)
                .FirstOrDefaultAsync(o => o.Id == korisnik.Id);

            if (odgajatelj == null) return NotFound();

            // Broj novih obavijesti (poslane ili kreirane, ali nisu pročitane)
            var brojNovihObavijesti = await _context.Obavijesti
                .CountAsync(o => o.StatusObavijesti == StatusObavijesti.POSLANA
                              || o.StatusObavijesti == StatusObavijesti.KREIRANA);

            // Uzmi samo ime (prvo riječ iz ImePrezime)
            var ime = odgajatelj.ImePrezime?.Split(' ', 2)[0] ?? "Odgajatelju";

            var vm = new OdgajateljHomeViewModel
            {
                Ime = ime,
                ImePrezime = odgajatelj.ImePrezime ?? "",
                Grupe = odgajatelj.Grupe?.OrderBy(g => g.ImeGrupe).ToList() ?? new List<Grupa>(),
                BrojNovihObavijesti = brojNovihObavijesti
            };
            return View(vm);
        }

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

    public class OdgajateljHomeViewModel
    {
        public string Ime { get; set; } = string.Empty;
        public string ImePrezime { get; set; } = string.Empty;
        public List<Grupa> Grupe { get; set; } = new();
        public int BrojNovihObavijesti { get; set; }
    }

    public class AdministratorHomeViewModel
    {
        public int BrojOdgajatelja { get; set; }
        public int BrojRoditelja { get; set; }
        public int BrojDjece { get; set; }
        public int BrojGrupa { get; set; }
    }
}
