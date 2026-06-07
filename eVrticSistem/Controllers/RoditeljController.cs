using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EVrtic.Data;
using EVrtic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace eVrticSistem.Controllers
{
    public class RoditeljController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Korisnik> _userManager;

        public RoditeljController(ApplicationDbContext context, UserManager<Korisnik> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ─── RODITELJ: Pregled vlastitog profila ─────────────────────────────

        [Authorize(Roles = "RODITELJ")]
        public async Task<IActionResult> MojProfil()
        {
            var korisnik = await _userManager.GetUserAsync(User);
            if (korisnik == null) return Challenge();

            var roditelj = await _context.Roditelji
                .Include(r => r.Djeca)
                .FirstOrDefaultAsync(r => r.Id == korisnik.Id);

            if (roditelj == null) return Challenge();
            return View(roditelj);
        }

        // ─── RODITELJ: Uređivanje vlastitog profila ──────────────────────────

        [Authorize(Roles = "RODITELJ")]
        public async Task<IActionResult> UrediProfil()
        {
            var korisnik = await _userManager.GetUserAsync(User);
            if (korisnik == null) return Challenge();

            var roditelj = await _context.Roditelji
                .FirstOrDefaultAsync(r => r.Id == korisnik.Id);

            if (roditelj == null) return Challenge();

            var vm = new UrediProfilViewModel
            {
                ImePrezime = roditelj.ImePrezime,
                KontaktTelefon = roditelj.KontaktTelefon,
                Email = roditelj.Email
            };

            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "RODITELJ")]
        public async Task<IActionResult> UrediProfil(UrediProfilViewModel vm)
        {
            var korisnik = await _userManager.GetUserAsync(User);
            if (korisnik == null) return Challenge();

            var roditelj = await _context.Roditelji
                .FirstOrDefaultAsync(r => r.Id == korisnik.Id);

            if (roditelj == null) return Challenge();

            // Email je login-identitet i ne mijenja se ovdje — vraćamo postojeći radi prikaza
            vm.Email = roditelj.Email;

            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            roditelj.ImePrezime = vm.ImePrezime.Trim();
            roditelj.KontaktTelefon = string.IsNullOrWhiteSpace(vm.KontaktTelefon)
                ? null
                : vm.KontaktTelefon.Trim();

            await _context.SaveChangesAsync();

            TempData["Uspjeh"] = "Profil je uspješno ažuriran.";
            return RedirectToAction(nameof(MojProfil));
        }

        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Roditelji
                .Include(r => r.Djeca)
                .ToListAsync());
        }

        // ─── ADMINISTRATOR: Deaktivacija / aktivacija profila roditelja ──────
        // (scenarij 6.6 - administrator deaktivira profil roditelja)

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> Deaktiviraj(int id)
        {
            var roditelj = await _context.Roditelji.FindAsync(id);
            if (roditelj == null) return NotFound();

            roditelj.StatusNaloga = StatusNaloga.DEAKTIVIRAN;
            await _context.SaveChangesAsync();

            TempData["Uspjeh"] = $"Profil roditelja \"{roditelj.ImePrezime}\" je deaktiviran.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> Aktiviraj(int id)
        {
            var roditelj = await _context.Roditelji.FindAsync(id);
            if (roditelj == null) return NotFound();

            roditelj.StatusNaloga = StatusNaloga.AKTIVAN;
            await _context.SaveChangesAsync();

            TempData["Uspjeh"] = $"Profil roditelja \"{roditelj.ImePrezime}\" je ponovo aktiviran.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var roditelj = await _context.Roditelji.FirstOrDefaultAsync(m => m.Id == id);
            if (roditelj == null) return NotFound();
            return View(roditelj);
        }

        [Authorize(Roles = "ADMINISTRATOR")]
        public IActionResult Create() => View();

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> Create([Bind("Id,ImePrezime,Email,Uloga,StatusNaloga")] Roditelj roditelj)
        {
            if (ModelState.IsValid) { _context.Add(roditelj); await _context.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
            return View(roditelj);
        }

        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var roditelj = await _context.Roditelji.FindAsync(id);
            if (roditelj == null) return NotFound();
            return View(roditelj);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ImePrezime,Email,Uloga,StatusNaloga")] Roditelj roditelj)
        {
            if (id != roditelj.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try { _context.Update(roditelj); await _context.SaveChangesAsync(); }
                catch (DbUpdateConcurrencyException) { if (!RoditeljExists(roditelj.Id)) return NotFound(); else throw; }
                return RedirectToAction(nameof(Index));
            }
            return View(roditelj);
        }

        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var rodatelj = await _context.Roditelji.FirstOrDefaultAsync(m => m.Id == id);
            if (rodatelj == null) return NotFound();
            return View(rodatelj);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rodatelj = await _context.Roditelji.FindAsync(id);
            if (rodatelj != null) _context.Roditelji.Remove(rodatelj);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoditeljExists(int id) => _context.Roditelji.Any(e => e.Id == id);
    }

    public class UrediProfilViewModel
    {
        [Required(ErrorMessage = "Ime i prezime je obavezno.")]
        [StringLength(100, ErrorMessage = "Ime i prezime može imati najviše 100 karaktera.")]
        [Display(Name = "Ime i prezime")]
        public string ImePrezime { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Kontakt telefon može imati najviše 20 karaktera.")]
        [Display(Name = "Kontakt telefon")]
        public string? KontaktTelefon { get; set; }

        // Samo za prikaz — email se ne mijenja na ovoj stranici
        public string? Email { get; set; }
    }
}
