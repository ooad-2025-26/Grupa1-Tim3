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
    public class ObavijestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Korisnik> _userManager;

        public ObavijestController(ApplicationDbContext context, UserManager<Korisnik> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Obavijest (admin)
        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Obavijesti
                .Include(o => o.Odgajatelj)
                .Include(o => o.Roditelj);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Obavijest/OdgajateljObavijesti — Lista obavijesti za odgajatelja
        [Authorize(Roles = "ODGAJATELJ")]
        public async Task<IActionResult> OdgajateljObavijesti()
        {
            var korisnik = await _userManager.GetUserAsync(User);
            if (korisnik == null) return RedirectToPage("/Account/Login", new { area = "Identity" });

            var odgajatelj = await _context.Odgajatelji
                .FirstOrDefaultAsync(o => o.Id == korisnik.Id);

            // Pronađi grupu(e) ovog odgajatelja
            var mojeGrupeIds = await _context.Grupe
                .Where(g => g.OdgajateljId == odgajatelj!.Id)
                .Select(g => g.Id)
                .ToListAsync();

            // Sve obavijesti — one koje je poslao ovaj odgajatelj ili su za sve/za grupu
            var obavijesti = await _context.Obavijesti
                .Include(o => o.Odgajatelj)
                .Include(o => o.Roditelj)
                .OrderByDescending(o => o.DatumKreiranja)
                .ToListAsync();

            var vm = new OdgajateljObavijestViewModel
            {
                Obavijesti = obavijesti,
                Odgajatelj = odgajatelj
            };

            return View(vm);
        }

        // GET: Obavijest/PosaljiObavijest — Forma za slanje obavijesti
        [Authorize(Roles = "ODGAJATELJ")]
        public async Task<IActionResult> PosaljiObavijest()
        {
            var korisnik = await _userManager.GetUserAsync(User);
            if (korisnik == null) return RedirectToPage("/Account/Login", new { area = "Identity" });

            var odgajatelj = await _context.Odgajatelji
                .Include(o => o.Grupe)
                .FirstOrDefaultAsync(o => o.Id == korisnik.Id);

            var vm = new PosaljiObavijestViewModel
            {
                Odgajatelj = odgajatelj
            };

            return View(vm);
        }

        // POST: Obavijest/PosaljiObavijest
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ODGAJATELJ")]
        public async Task<IActionResult> PosaljiObavijest(PosaljiObavijestInputModel input)
        {
            var korisnik = await _userManager.GetUserAsync(User);
            if (korisnik == null) return Unauthorized();

            var odgajatelj = await _context.Odgajatelji
                .Include(o => o.Grupe)
                .FirstOrDefaultAsync(o => o.Id == korisnik.Id);

            if (odgajatelj == null) return Forbid();

            // Određujemo primaoce na osnovu odabira
            List<Roditelj> primaoci = new();

            if (input.Prima == "svi")
            {
                primaoci = await _context.Roditelji.ToListAsync();
            }
            else if (input.Prima == "odgajatelji")
            {
                // Obavijest za odgajatelje — kreiramo globalnu obavijest bez konkretnog roditelja
                primaoci = new List<Roditelj>();
            }
            else if (input.Prima != null && input.Prima.StartsWith("grupa:"))
            {
                // Prima = "grupa:N" — roditelji djece iz konkretne grupe N
                if (int.TryParse(input.Prima.Substring("grupa:".Length), out int grupaId))
                {
                    // Provjeri da grupa pripada odgajatelju
                    var mojeGrupeIds = odgajatelj.Grupe.Select(g => g.Id).ToList();
                    if (mojeGrupeIds.Contains(grupaId))
                    {
                        primaoci = await _context.Djeca
                            .Include(d => d.Roditelj)
                            .Where(d => d.GrupaId == grupaId && d.Roditelj != null)
                            .Select(d => d.Roditelj!)
                            .Distinct()
                            .ToListAsync();
                    }
                }
            }
            else
            {
                // Fallback — roditelji djece u svim mojim grupama
                var moja = odgajatelj.Grupe.Select(g => g.Id).ToList();
                primaoci = await _context.Djeca
                    .Include(d => d.Roditelj)
                    .Where(d => d.GrupaId.HasValue && moja.Contains(d.GrupaId.Value) && d.Roditelj != null)
                    .Select(d => d.Roditelj!)
                    .Distinct()
                    .ToListAsync();
            }

            var kanali = new List<KanalSlanja>();
            if (input.PutemAplikacije) kanali.Add(KanalSlanja.APLIKACIJA);
            if (input.PutemEmaila) kanali.Add(KanalSlanja.EMAIL);
            var kanal = kanali.Count > 0 ? kanali[0] : KanalSlanja.APLIKACIJA;

            if (primaoci.Count == 0)
            {
                // Globalna obavijest (za odgajatelje ili kad nema roditelja) — šaljemo bez konkretnog roditelja
                // Koristimo dummy RoditeljId = 0 (ili neka postoji barem jedan roditelj)
                var prvRoditelj = await _context.Roditelji.FirstOrDefaultAsync();
                if (prvRoditelj != null)
                {
                    var obavijest = new Obavijest
                    {
                        Naslov = input.Naslov ?? string.Empty,
                        Sadrzaj = input.Poruka ?? string.Empty,
                        DatumKreiranja = DateTime.Now,
                        DatumSlanja = DateTime.Now,
                        KanalSlanja = kanal,
                        StatusObavijesti = StatusObavijesti.POSLANA,
                        RoditeljId = prvRoditelj.Id,
                        OdgajateljId = odgajatelj.Id
                    };
                    _context.Obavijesti.Add(obavijest);
                }
            }
            else
            {
                foreach (var roditelj in primaoci)
                {
                    var obavijest = new Obavijest
                    {
                        Naslov = input.Naslov ?? string.Empty,
                        Sadrzaj = input.Poruka ?? string.Empty,
                        DatumKreiranja = DateTime.Now,
                        DatumSlanja = DateTime.Now,
                        KanalSlanja = kanal,
                        StatusObavijesti = StatusObavijesti.POSLANA,
                        RoditeljId = roditelj.Id,
                        OdgajateljId = odgajatelj.Id
                    };
                    _context.Obavijesti.Add(obavijest);
                }
            }

            await _context.SaveChangesAsync();
            TempData["Uspjeh"] = "Obavijest je uspješno poslana.";
            return RedirectToAction(nameof(OdgajateljObavijesti));
        }

        // GET: Obavijest/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var obavijest = await _context.Obavijesti
                .Include(o => o.Odgajatelj)
                .Include(o => o.Roditelj)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (obavijest == null) return NotFound();

            return View(obavijest);
        }

        // GET: Obavijest/Create (admin)
        [Authorize(Roles = "ADMINISTRATOR")]
        public IActionResult Create()
        {
            ViewData["OdgajateljId"] = new SelectList(_context.Odgajatelji, "Id", "Email");
            ViewData["RoditeljId"] = new SelectList(_context.Roditelji, "Id", "Email");
            return View();
        }

        // POST: Obavijest/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> Create([Bind("Id,Naslov,Sadrzaj,DatumKreiranja,DatumSlanja,KanalSlanja,StatusObavijesti,RoditeljId,OdgajateljId")] Obavijest obavijest)
        {
            if (ModelState.IsValid)
            {
                _context.Add(obavijest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OdgajateljId"] = new SelectList(_context.Odgajatelji, "Id", "Email", obavijest.OdgajateljId);
            ViewData["RoditeljId"] = new SelectList(_context.Roditelji, "Id", "Email", obavijest.RoditeljId);
            return View(obavijest);
        }

        // GET: Obavijest/Edit/5
        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var obavijest = await _context.Obavijesti.FindAsync(id);
            if (obavijest == null) return NotFound();
            ViewData["OdgajateljId"] = new SelectList(_context.Odgajatelji, "Id", "Email", obavijest.OdgajateljId);
            ViewData["RoditeljId"] = new SelectList(_context.Roditelji, "Id", "Email", obavijest.RoditeljId);
            return View(obavijest);
        }

        // POST: Obavijest/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Naslov,Sadrzaj,DatumKreiranja,DatumSlanja,KanalSlanja,StatusObavijesti,RoditeljId,OdgajateljId")] Obavijest obavijest)
        {
            if (id != obavijest.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(obavijest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Obavijesti.Any(e => e.Id == obavijest.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["OdgajateljId"] = new SelectList(_context.Odgajatelji, "Id", "Email", obavijest.OdgajateljId);
            ViewData["RoditeljId"] = new SelectList(_context.Roditelji, "Id", "Email", obavijest.RoditeljId);
            return View(obavijest);
        }

        // GET: Obavijest/Delete/5
        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var obavijest = await _context.Obavijesti
                .Include(o => o.Odgajatelj)
                .Include(o => o.Roditelj)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (obavijest == null) return NotFound();
            return View(obavijest);
        }

        // POST: Obavijest/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var obavijest = await _context.Obavijesti.FindAsync(id);
            if (obavijest != null) _context.Obavijesti.Remove(obavijest);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }

    // ─── ViewModeli za obavijesti ──────────────────────────────────────────

    public class OdgajateljObavijestViewModel
    {
        public List<Obavijest> Obavijesti { get; set; } = new();
        public Odgajatelj? Odgajatelj { get; set; }
    }

    public class PosaljiObavijestViewModel
    {
        public Odgajatelj? Odgajatelj { get; set; }
    }

    public class PosaljiObavijestInputModel
    {
        public string? Naslov { get; set; }
        public string? Poruka { get; set; }
        public string Prima { get; set; } = "roditelji";
        public bool PutemAplikacije { get; set; } = true;
        public bool PutemEmaila { get; set; }
    }
}
