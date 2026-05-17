using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EVrtic.Data;
using EVrtic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace EVrtic.Controllers
{
    public class DijeteController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Korisnik> _userManager;

        public DijeteController(ApplicationDbContext context, UserManager<Korisnik> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Dijete
        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Djeca.Include(d => d.Grupa).Include(d => d.Roditelj);
            return View(await applicationDbContext.ToListAsync());
        }
        // GET: Dijete/MojeDijete
        [Authorize(Roles = "RODITELJ")]
        public async Task<IActionResult> MojeDijete()
        {
            var korisnik = await _userManager.GetUserAsync(User);
            if (korisnik == null) return Challenge();

            var dijete = await _context.Djeca
                .Include(d => d.Alergije)
                .Include(d => d.Bolesti)
                .Include(d => d.Grupa)
                .FirstOrDefaultAsync(d => d.RoditeljId == korisnik.Id);

            if (dijete == null)
                return RedirectToAction("UnosPodataka");

            return View(dijete);
        }
        // GET: Dijete/Details/5
        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var dijete = await _context.Djeca
                .Include(d => d.Grupa)
                .Include(d => d.Roditelj)
                .Include(d => d.Alergije)
                .Include(d => d.Bolesti)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (dijete == null) return NotFound();

            return View(dijete);
        }

        // ─────────────────────────────────────────────────────────────────────
        // ADMINISTRATOR: Unos novog identifikacionog koda djeteta
        // ─────────────────────────────────────────────────────────────────────

        // GET: Dijete/DodajIdentifikacioniKod
        [Authorize(Roles = "ADMINISTRATOR")]
        public IActionResult DodajIdentifikacioniKod()
        {
            return View();
        }

        // POST: Dijete/DodajIdentifikacioniKod
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> DodajIdentifikacioniKod([Bind("IdentifikacioniKod")] Dijete dijete)
        {
            // Ukloni validacije za polja koja se ne unose ovdje
            ModelState.Remove("ImePrezime");
            ModelState.Remove("DodatnaNapomena");

            if (string.IsNullOrWhiteSpace(dijete.IdentifikacioniKod))
            {
                ModelState.AddModelError("IdentifikacioniKod", "Identifikacioni kod je obavezan.");
            }
            else
            {
                // Provjeri jedinstvenost koda
                bool kodPostoji = await _context.Djeca
                    .AnyAsync(d => d.IdentifikacioniKod == dijete.IdentifikacioniKod);

                if (kodPostoji)
                {
                    ModelState.AddModelError("IdentifikacioniKod",
                        "Dijete sa ovim identifikacionim kodom već postoji u sistemu.");
                }
            }

            if (!ModelState.IsValid)
                return View(dijete);

            var novoDijete = new Dijete
            {
                IdentifikacioniKod = dijete.IdentifikacioniKod.Trim(),
                ImePrezime = string.Empty,   // roditelj će popuniti
                Aktivno = true
            };

            _context.Add(novoDijete);
            await _context.SaveChangesAsync();

            TempData["Uspjeh"] = $"Identifikacioni kod \"{novoDijete.IdentifikacioniKod}\" je uspješno dodan u sistem.";
            return RedirectToAction(nameof(DodajIdentifikacioniKod));
        }

        // ─────────────────────────────────────────────────────────────────────
        // RODITELJ: Unos podataka o svom djetetu (odmah po registraciji)
        // ─────────────────────────────────────────────────────────────────────

        // GET: Dijete/UnosPodataka
        [Authorize(Roles = "RODITELJ")]
        public async Task<IActionResult> UnosPodataka()
        {
            var korisnik = await _userManager.GetUserAsync(User);
            if (korisnik == null) return Challenge();

            // Pronađi dijete povezano sa ovim roditeljem
            var dijete = await _context.Djeca
                .Include(d => d.Alergije)
                .Include(d => d.Bolesti)
                .FirstOrDefaultAsync(d => d.RoditeljId == korisnik.Id);

            if (dijete == null)
                return RedirectToAction("RoditeljHome", "Home");

            var vm = new UnosPodatakaViewModel
            {
                DijeteId = dijete.Id,
                ImePrezime = dijete.ImePrezime,
                DodatnaNapomena = dijete.DodatnaNapomena,
                Alergije = dijete.Alergije.Select(a => a.Naziv).ToList(),
                Bolesti = dijete.Bolesti.Select(b => b.Naziv).ToList()
            };

            return View(vm);
        }

        // POST: Dijete/UnosPodataka
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "RODITELJ")]
        public async Task<IActionResult> UnosPodataka(UnosPodatakaViewModel vm)
        {
            var korisnik = await _userManager.GetUserAsync(User);
            if (korisnik == null) return Challenge();

            var dijete = await _context.Djeca
                .Include(d => d.Alergije)
                .Include(d => d.Bolesti)
                .FirstOrDefaultAsync(d => d.Id == vm.DijeteId && d.RoditeljId == korisnik.Id);

            if (dijete == null) return Forbid();

            // Provjera duplikata alergija
            if (vm.Alergije != null)
            {
                var alergijeLower = vm.Alergije
                    .Where(a => !string.IsNullOrWhiteSpace(a))
                    .Select(a => a.Trim().ToLower())
                    .ToList();

                if (alergijeLower.Count != alergijeLower.Distinct().Count())
                {
                    ModelState.AddModelError("Alergije", "Lista alergija sadrži duplikate. Svaka alergija mora biti jedinstvena.");
                }
            }

            // Provjera duplikata bolesti
            if (vm.Bolesti != null)
            {
                var bolestiLower = vm.Bolesti
                    .Where(b => !string.IsNullOrWhiteSpace(b))
                    .Select(b => b.Trim().ToLower())
                    .ToList();

                if (bolestiLower.Count != bolestiLower.Distinct().Count())
                {
                    ModelState.AddModelError("Bolesti", "Lista bolesti sadrži duplikate. Svaka bolest mora biti jedinstvena.");
                }
            }

            if (!ModelState.IsValid)
                return View(vm);

            // Ažuriraj osnovne podatke djeteta
            dijete.ImePrezime = vm.ImePrezime?.Trim() ?? string.Empty;
            dijete.DodatnaNapomena = vm.DodatnaNapomena?.Trim() ?? string.Empty;

            // Zamijeni alergije
            _context.AlergijeDjece.RemoveRange(dijete.Alergije);
            if (vm.Alergije != null)
            {
                foreach (var naziv in vm.Alergije.Where(a => !string.IsNullOrWhiteSpace(a)))
                {
                    _context.AlergijeDjece.Add(new AlergijaDjeteta
                    {
                        DijeteId = dijete.Id,
                        Naziv = naziv.Trim()
                    });
                }
            }

            // Zamijeni bolesti
            _context.BolestiDjece.RemoveRange(dijete.Bolesti);
            if (vm.Bolesti != null)
            {
                foreach (var naziv in vm.Bolesti.Where(b => !string.IsNullOrWhiteSpace(b)))
                {
                    _context.BolestiDjece.Add(new BolestDjeteta
                    {
                        DijeteId = dijete.Id,
                        Naziv = naziv.Trim()
                    });
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("RoditeljHome", "Home");
        }

        // ─────────────────────────────────────────────────────────────────────
        // Standardni CRUD (admin only)
        // ─────────────────────────────────────────────────────────────────────

        [Authorize(Roles = "ADMINISTRATOR")]
        public IActionResult Create()
        {
            ViewData["GrupaId"] = new SelectList(_context.Grupe, "Id", "ImeGrupe");
            ViewData["RoditeljId"] = new SelectList(_context.Roditelji, "Id", "Email");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> Create([Bind("Id,ImePrezime,IdentifikacioniKod,DodatnaNapomena,Aktivno,GrupaId,RoditeljId")] Dijete dijete)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dijete);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GrupaId"] = new SelectList(_context.Grupe, "Id", "ImeGrupe", dijete.GrupaId);
            ViewData["RoditeljId"] = new SelectList(_context.Roditelji, "Id", "Email", dijete.RoditeljId);
            return View(dijete);
        }

        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var dijete = await _context.Djeca.FindAsync(id);
            if (dijete == null) return NotFound();

            ViewData["GrupaId"] = new SelectList(_context.Grupe, "Id", "ImeGrupe", dijete.GrupaId);
            ViewData["RoditeljId"] = new SelectList(_context.Roditelji, "Id", "Email", dijete.RoditeljId);
            return View(dijete);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ImePrezime,IdentifikacioniKod,DodatnaNapomena,Aktivno,GrupaId,RoditeljId")] Dijete dijete)
        {
            if (id != dijete.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dijete);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DijeteExists(dijete.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["GrupaId"] = new SelectList(_context.Grupe, "Id", "ImeGrupe", dijete.GrupaId);
            ViewData["RoditeljId"] = new SelectList(_context.Roditelji, "Id", "Email", dijete.RoditeljId);
            return View(dijete);
        }

        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var dijete = await _context.Djeca
                .Include(d => d.Grupa)
                .Include(d => d.Roditelj)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (dijete == null) return NotFound();
            return View(dijete);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dijete = await _context.Djeca.FindAsync(id);
            if (dijete != null) _context.Djeca.Remove(dijete);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DijeteExists(int id)
        {
            return _context.Djeca.Any(e => e.Id == id);
        }
    }

    // ViewModel za unos podataka o djetetu
    public class UnosPodatakaViewModel
    {
        public int DijeteId { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Ime i prezime djeteta je obavezno.")]
        [System.ComponentModel.DataAnnotations.StringLength(100)]
        [System.ComponentModel.DataAnnotations.Display(Name = "Ime i prezime djeteta")]
        public string ImePrezime { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Display(Name = "Dodatna napomena")]
        [System.ComponentModel.DataAnnotations.StringLength(500)]
        public string? DodatnaNapomena { get; set; }

        public List<string> Alergije { get; set; } = new();
        public List<string> Bolesti { get; set; } = new();
    }
}
