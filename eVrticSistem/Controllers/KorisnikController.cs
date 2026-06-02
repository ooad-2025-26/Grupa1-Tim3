using System.Linq;
using System.Threading.Tasks;
using EVrtic.Data;
using EVrtic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eVrticSistem.Controllers
{
    public class KorisnikController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Korisnik> _userManager;

        public KorisnikController(ApplicationDbContext context, UserManager<Korisnik> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Korisnik
        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> Index()
        {
            var korisnici = await _context.Users
                .OrderBy(u => u.ImePrezime)
                .ToListAsync();
            return View(korisnici);
        }

        // GET: /Korisnik/Profil[/5]
        // Prikaz osnovnih podataka o korisniku. Ako nije proslijeđen id, koristi se
        // ID trenutno prijavljenog korisnika. Ne-administratori smiju vidjeti samo
        // svoj profil.
        [Authorize]
        public async Task<IActionResult> Profil(int? id)
        {
            var trenutni = await _userManager.GetUserAsync(User);
            if (trenutni == null) return Challenge();

            // Ako id nije proslijeđen, uzmi ID prijavljenog korisnika
            int trazeniId = id ?? trenutni.Id;

            // Ne-administratori smiju gledati samo svoj profil
            bool jeAdmin = await _userManager.IsInRoleAsync(trenutni, "ADMINISTRATOR");
            if (!jeAdmin && trazeniId != trenutni.Id)
            {
                trazeniId = trenutni.Id;
            }

            var bazni = await _context.Users.FindAsync(trazeniId);
            if (bazni == null) return NotFound();

            ViewBag.JeVlastitiProfil = (trazeniId == trenutni.Id);

            if (bazni.Uloga == Uloga.RODITELJ)
            {
                var roditelj = await _context.Roditelji
                    .Include(r => r.Djeca).ThenInclude(d => d.Grupa)
                    .FirstOrDefaultAsync(r => r.Id == trazeniId);
                return View(roditelj);
            }
            if (bazni.Uloga == Uloga.ODGAJATELJ)
            {
                var odgajatelj = await _context.Odgajatelji
                    .Include(o => o.Grupe).ThenInclude(g => g.Djeca)
                    .FirstOrDefaultAsync(o => o.Id == trazeniId);
                return View(odgajatelj);
            }
            return View(bazni);
        }

        // ─── Deaktivacija / aktivacija profila (scenarij 6.6) ────────────────

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> Deaktiviraj(int id)
        {
            var korisnik = await _context.Users.FindAsync(id);
            if (korisnik == null) return NotFound();

            korisnik.StatusNaloga = StatusNaloga.DEAKTIVIRAN;
            await _context.SaveChangesAsync();

            TempData["Uspjeh"] = $"Profil korisnika \"{korisnik.ImePrezime}\" je deaktiviran.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> Aktiviraj(int id)
        {
            var korisnik = await _context.Users.FindAsync(id);
            if (korisnik == null) return NotFound();

            korisnik.StatusNaloga = StatusNaloga.AKTIVAN;
            await _context.SaveChangesAsync();

            TempData["Uspjeh"] = $"Profil korisnika \"{korisnik.ImePrezime}\" je ponovo aktiviran.";
            return RedirectToAction(nameof(Index));
        }

        // ─── Trajno brisanje korisnika ───────────────────────────────────────

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> Obrisi(int id)
        {
            // Sigurnosna provjera: administrator ne smije obrisati vlastiti nalog
            var trenutniKorisnik = await _userManager.GetUserAsync(User);
            if (trenutniKorisnik != null && trenutniKorisnik.Id == id)
            {
                TempData["Greska"] = "Ne možete obrisati vlastiti nalog.";
                return RedirectToAction(nameof(Index));
            }

            var korisnik = await _context.Users.FindAsync(id);
            if (korisnik == null) return NotFound();

            // Raskini FK veze koje bi inače spriječile brisanje
            //   Roditelj ima djecu (Restrict FK) → djeca ostaju u sistemu, samo bez roditelja
            //   Odgajatelj ima grupe → grupe ostaju, samo bez odgajatelja
            if (korisnik.Uloga == Uloga.RODITELJ)
            {
                var djeca = await _context.Djeca.Where(d => d.RoditeljId == id).ToListAsync();
                foreach (var d in djeca) d.RoditeljId = null;
                await _context.SaveChangesAsync();
            }
            else if (korisnik.Uloga == Uloga.ODGAJATELJ)
            {
                var grupe = await _context.Grupe.Where(g => g.OdgajateljId == id).ToListAsync();
                foreach (var g in grupe) g.OdgajateljId = null;
                await _context.SaveChangesAsync();
            }

            var ime = korisnik.ImePrezime;
            var rezultat = await _userManager.DeleteAsync(korisnik);

            if (!rezultat.Succeeded)
            {
                var greske = string.Join(", ", rezultat.Errors.Select(e => e.Description));
                TempData["Greska"] = $"Brisanje neuspješno: {greske}";
                return RedirectToAction(nameof(Index));
            }

            TempData["Uspjeh"] = $"Korisnik \"{ime}\" je trajno obrisan iz sistema.";
            return RedirectToAction(nameof(Index));
        }
    }
}
