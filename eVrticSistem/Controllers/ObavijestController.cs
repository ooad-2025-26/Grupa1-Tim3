using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EVrtic.Data;
using EVrtic.Models;
using EVrtic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace EVrtic.Controllers
{
    public class ObavijestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Korisnik> _userManager;
        private readonly IEmailService _emailService;

        public ObavijestController(
            ApplicationDbContext context,
            UserManager<Korisnik> userManager,
            IEmailService emailService)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailService;
        }

        // ═══════════════════════════════════════════════════════════════════
        // ADMINISTRATOR — CRUD nad obavijestima (zadržano iz scaffolda)
        // ═══════════════════════════════════════════════════════════════════

        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Obavijesti
                .Include(o => o.Odgajatelj)
                .Include(o => o.Roditelj);
            return View(await applicationDbContext.ToListAsync());
        }

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

        [Authorize(Roles = "ADMINISTRATOR")]
        public IActionResult Create()
        {
            ViewData["OdgajateljId"] = new SelectList(_context.Odgajatelji, "Id", "Email");
            ViewData["RoditeljId"]   = new SelectList(_context.Roditelji,   "Id", "Email");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> Create(
            [Bind("Id,Naslov,Sadrzaj,DatumKreiranja,DatumSlanja,KanalSlanja,StatusObavijesti,RoditeljId,OdgajateljId")]
            Obavijest obavijest)
        {
            if (ModelState.IsValid)
            {
                _context.Add(obavijest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OdgajateljId"] = new SelectList(_context.Odgajatelji, "Id", "Email", obavijest.OdgajateljId);
            ViewData["RoditeljId"]   = new SelectList(_context.Roditelji,   "Id", "Email", obavijest.RoditeljId);
            return View(obavijest);
        }

        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var obavijest = await _context.Obavijesti.FindAsync(id);
            if (obavijest == null) return NotFound();
            ViewData["OdgajateljId"] = new SelectList(_context.Odgajatelji, "Id", "Email", obavijest.OdgajateljId);
            ViewData["RoditeljId"]   = new SelectList(_context.Roditelji,   "Id", "Email", obavijest.RoditeljId);
            return View(obavijest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> Edit(int id,
            [Bind("Id,Naslov,Sadrzaj,DatumKreiranja,DatumSlanja,KanalSlanja,StatusObavijesti,RoditeljId,OdgajateljId")]
            Obavijest obavijest)
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
            ViewData["RoditeljId"]   = new SelectList(_context.Roditelji,   "Id", "Email", obavijest.RoditeljId);
            return View(obavijest);
        }

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

        // ═══════════════════════════════════════════════════════════════════
        // ODGAJATELJ — Pregled i slanje obavijesti
        // ═══════════════════════════════════════════════════════════════════

        // GET: Obavijest/OdgajateljObavijesti
        [Authorize(Roles = "ODGAJATELJ")]
        public async Task<IActionResult> OdgajateljObavijesti()
        {
            var korisnik = await _userManager.GetUserAsync(User);
            if (korisnik == null) return Challenge();

            // Učitaj sve obavijesti u sistemu (admin uvid i obavijesti drugih odgajatelja)
            var obavijesti = await _context.Obavijesti
                .Include(o => o.Odgajatelj)
                .Include(o => o.Roditelj)
                .OrderByDescending(o => o.DatumKreiranja)
                .ToListAsync();

            // DEDUPLIKACIJA: jedna "send akcija" pravi više DB redova (po jedan za svakog
            // primaoca), pa ih grupišemo po (OdgajateljId, Naslov, Sadrzaj, DatumKreiranja).
            // Iz svake grupe prikazujemo samo jedan red.
            var grupisane = obavijesti
                .GroupBy(o => new {
                    o.OdgajateljId,
                    o.Naslov,
                    o.Sadrzaj,
                    DatumKlj = o.DatumKreiranja.Ticks / TimeSpan.TicksPerSecond
                })
                .Select(g => g.First())
                .OrderByDescending(o => o.DatumKreiranja)
                .ToList();

            // Označi sve nepročitane obavijesti kao pročitane — kad odgajatelj otvori
            // listu, brojač novih ide na 0
            var nepročitane = await _context.Obavijesti
                .Where(o => o.StatusObavijesti == StatusObavijesti.POSLANA
                         || o.StatusObavijesti == StatusObavijesti.KREIRANA)
                .ToListAsync();
            foreach (var o in nepročitane)
            {
                o.StatusObavijesti = StatusObavijesti.PROCITANA;
            }
            if (nepročitane.Any())
            {
                await _context.SaveChangesAsync();
            }

            var vm = new OdgajateljObavijestViewModel
            {
                Obavijesti = grupisane,
                TrenutniKorisnikId = korisnik.Id
            };

            return View(vm);
        }

        // GET: Obavijest/PosaljiObavijest
        [Authorize(Roles = "ODGAJATELJ")]
        public async Task<IActionResult> PosaljiObavijest()
        {
            var korisnik = await _userManager.GetUserAsync(User);
            if (korisnik == null) return Challenge();

            var odgajatelj = await _context.Odgajatelji
                .Include(o => o.Grupe)
                .FirstOrDefaultAsync(o => o.Id == korisnik.Id);

            return View(new PosaljiObavijestViewModel { Odgajatelj = odgajatelj });
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

            // ─── Odredi primaoce na osnovu odabira ─────────────────────────
            List<Roditelj> primaoci = new();

            if (input.Prima == "svi")
            {
                primaoci = await _context.Roditelji.ToListAsync();
            }
            else if (input.Prima != null && input.Prima.StartsWith("grupa:"))
            {
                if (int.TryParse(input.Prima.Substring("grupa:".Length), out int grupaId))
                {
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
                // Fallback — roditelji djece iz svih grupa odgajatelja
                var moja = odgajatelj.Grupe.Select(g => g.Id).ToList();
                primaoci = await _context.Djeca
                    .Include(d => d.Roditelj)
                    .Where(d => d.GrupaId.HasValue && moja.Contains(d.GrupaId.Value) && d.Roditelj != null)
                    .Select(d => d.Roditelj!)
                    .Distinct()
                    .ToListAsync();
            }

            if (!primaoci.Any())
            {
                TempData["Greska"] = "Nema roditelja koji se mogu obavijestiti.";
                return RedirectToAction(nameof(OdgajateljObavijesti));
            }

            // Odredi kanal slanja
            var kanal = (input.PutemEmaila && input.PutemAplikacije) ? KanalSlanja.EMAIL
                      : input.PutemEmaila ? KanalSlanja.EMAIL
                      : KanalSlanja.APLIKACIJA;

            // VAŽNO: izračunaj timestamp jednom i koristi isti za sve zapise iz
            // ove "send akcije" — to omogućava deduplikaciju na prikazu
            var trenutak = DateTime.Now;

            // Kreiraj jedan DB zapis po primaocu (svi sa istim timestampom)
            foreach (var roditelj in primaoci)
            {
                var obavijest = new Obavijest
                {
                    Naslov          = input.Naslov ?? string.Empty,
                    Sadrzaj         = input.Poruka ?? string.Empty,
                    DatumKreiranja  = trenutak,
                    DatumSlanja     = trenutak,
                    KanalSlanja     = kanal,
                    StatusObavijesti = StatusObavijesti.POSLANA,
                    RoditeljId      = roditelj.Id,
                    OdgajateljId    = odgajatelj.Id
                };
                _context.Obavijesti.Add(obavijest);
            }

            await _context.SaveChangesAsync();

            // ─── Pošalji email-ove (ako je odabran taj kanal) ──────────────
            if (input.PutemEmaila)
            {
                int poslanoBroj = 0, neuspjelo = 0;
                foreach (var roditelj in primaoci)
                {
                    if (string.IsNullOrWhiteSpace(roditelj.Email)) { neuspjelo++; continue; }

                    var subject = $"[eVrtić] {input.Naslov}";
                    var body =
                        $"Poštovani roditelju {roditelj.ImePrezime},\n\n" +
                        $"{input.Poruka}\n\n" +
                        $"Lijep pozdrav,\n" +
                        $"{odgajatelj.ImePrezime}\n" +
                        $"— Vaš odgajatelj u eVrtiću";

                    var uspjeh = await _emailService.SendAsync(roditelj.Email, subject, body);
                    if (uspjeh) poslanoBroj++;
                    else neuspjelo++;
                }

                if (poslanoBroj > 0)
                {
                    TempData["Uspjeh"] = $"Obavijest poslana. Email dostavljen na {poslanoBroj} adresa."
                        + (neuspjelo > 0 ? $" ({neuspjelo} adresa nije bilo moguće obavijestiti emailom.)" : "");
                }
                else
                {
                    TempData["Uspjeh"] = "Obavijest poslana putem aplikacije."
                        + (neuspjelo > 0 ? $" (Email slanje nije uspjelo ni za jednu adresu.)" : "");
                }
            }
            else
            {
                TempData["Uspjeh"] = "Obavijest uspješno poslana.";
            }

            return RedirectToAction(nameof(OdgajateljObavijesti));
        }

        // POST: Obavijest/ObrisiBatch
        // Briše SVE obavijesti koje pripadaju istoj "send akciji" (isti odgajatelj,
        // isti naslov, isti sadržaj, isti timestamp).
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ODGAJATELJ")]
        public async Task<IActionResult> ObrisiBatch(int id)
        {
            var korisnik = await _userManager.GetUserAsync(User);
            if (korisnik == null) return Unauthorized();

            // Pronađi originalnu obavijest po ID-u
            var origin = await _context.Obavijesti.FindAsync(id);
            if (origin == null) return NotFound();

            // Samo kreator (odgajatelj koji je poslao) može obrisati
            if (origin.OdgajateljId != korisnik.Id) return Forbid();

            // Pronađi sve obavijesti iz iste batch-grupe (po composite key-u)
            var sekunde = origin.DatumKreiranja.Ticks / TimeSpan.TicksPerSecond;
            var batchObavijesti = await _context.Obavijesti
                .Where(o => o.OdgajateljId == origin.OdgajateljId
                         && o.Naslov == origin.Naslov
                         && o.Sadrzaj == origin.Sadrzaj
                         && o.DatumKreiranja.Year == origin.DatumKreiranja.Year
                         && o.DatumKreiranja.Month == origin.DatumKreiranja.Month
                         && o.DatumKreiranja.Day == origin.DatumKreiranja.Day
                         && o.DatumKreiranja.Hour == origin.DatumKreiranja.Hour
                         && o.DatumKreiranja.Minute == origin.DatumKreiranja.Minute
                         && o.DatumKreiranja.Second == origin.DatumKreiranja.Second)
                .ToListAsync();

            _context.Obavijesti.RemoveRange(batchObavijesti);
            await _context.SaveChangesAsync();

            TempData["Uspjeh"] = "Obavijest je obrisana.";
            return RedirectToAction(nameof(OdgajateljObavijesti));
        }
    }

    // ─── ViewModeli za obavijesti ──────────────────────────────────────────

    public class OdgajateljObavijestViewModel
    {
        public List<Obavijest> Obavijesti { get; set; } = new();
        public int TrenutniKorisnikId { get; set; }
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
