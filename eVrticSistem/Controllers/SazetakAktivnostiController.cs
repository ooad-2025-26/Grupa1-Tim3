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
    public class SazetakAktivnostiController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Korisnik> _userManager;

        public SazetakAktivnostiController(ApplicationDbContext context, UserManager<Korisnik> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: SazetakAktivnosti
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.SazeciAktivnosti.Include(s => s.Dijete);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: SazetakAktivnosti/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sazetakAktivnosti = await _context.SazeciAktivnosti
                .Include(s => s.Dijete)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (sazetakAktivnosti == null)
            {
                return NotFound();
            }

            return View(sazetakAktivnosti);
        }

        // GET: SazetakAktivnosti/Create
        public IActionResult Create()
        {
            ViewData["DijeteId"] = new SelectList(_context.Djeca, "Id", "IdentifikacioniKod");
            return View();
        }

        // POST: SazetakAktivnosti/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DatumPocetka,DatumKraja,TipSazetka,BrojObroka,BrojDolazaka,AgregiranoSpavanjeMinuta,OsnovneNapomene,DatumGenerisanja,DijeteId")] SazetakAktivnosti sazetakAktivnosti)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sazetakAktivnosti);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["DijeteId"] = new SelectList(_context.Djeca, "Id", "IdentifikacioniKod", sazetakAktivnosti.DijeteId);
            return View(sazetakAktivnosti);
        }

        // GET: SazetakAktivnosti/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sazetakAktivnosti = await _context.SazeciAktivnosti.FindAsync(id);

            if (sazetakAktivnosti == null)
            {
                return NotFound();
            }

            ViewData["DijeteId"] = new SelectList(_context.Djeca, "Id", "IdentifikacioniKod", sazetakAktivnosti.DijeteId);
            return View(sazetakAktivnosti);
        }

        // POST: SazetakAktivnosti/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DatumPocetka,DatumKraja,TipSazetka,BrojObroka,BrojDolazaka,AgregiranoSpavanjeMinuta,OsnovneNapomene,DatumGenerisanja,DijeteId")] SazetakAktivnosti sazetakAktivnosti)
        {
            if (id != sazetakAktivnosti.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sazetakAktivnosti);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SazetakAktivnostiExists(sazetakAktivnosti.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["DijeteId"] = new SelectList(_context.Djeca, "Id", "IdentifikacioniKod", sazetakAktivnosti.DijeteId);
            return View(sazetakAktivnosti);
        }

        // GET: SazetakAktivnosti/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sazetakAktivnosti = await _context.SazeciAktivnosti
                .Include(s => s.Dijete)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (sazetakAktivnosti == null)
            {
                return NotFound();
            }

            return View(sazetakAktivnosti);
        }

        // POST: SazetakAktivnosti/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sazetakAktivnosti = await _context.SazeciAktivnosti.FindAsync(id);

            if (sazetakAktivnosti != null)
            {
                _context.SazeciAktivnosti.Remove(sazetakAktivnosti);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // RODITELJ: Pregled sedmičnog/mjesečnog sažetka
        [Authorize(Roles = "RODITELJ")]
        public async Task<IActionResult> RoditeljPregled(string period = "Sedmica", DateTime? pocetak = null, int? dijeteId = null)
        {
            var korisnik = await _userManager.GetUserAsync(User);

            if (korisnik == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var djeca = await _context.Djeca
                .Include(d => d.Grupa)
                .Where(d => d.RoditeljId == korisnik.Id && d.Aktivno)
                .OrderBy(d => d.ImePrezime)
                .ToListAsync();

            var odabranoDijete = dijeteId.HasValue
                ? djeca.FirstOrDefault(d => d.Id == dijeteId.Value)
                : djeca.FirstOrDefault();

            var datumPocetka = pocetak?.Date ?? DateTime.Today;
            DateTime datumKraja;

            if (period.Equals("Mjesec", StringComparison.OrdinalIgnoreCase))
            {
                datumPocetka = new DateTime(datumPocetka.Year, datumPocetka.Month, 1);
                datumKraja = datumPocetka.AddMonths(1).AddDays(-1);
                period = "Mjesec";
            }
            else
            {
                int diff = ((int)datumPocetka.DayOfWeek + 6) % 7;
                datumPocetka = datumPocetka.AddDays(-diff).Date;
                datumKraja = datumPocetka.AddDays(6);
                period = "Sedmica";
            }

            var vm = new RoditeljSazetakPregledViewModel
            {
                Period = period,
                DatumPocetka = datumPocetka,
                DatumKraja = datumKraja,
                Djeca = djeca,
                Dijete = odabranoDijete,
                OdabraniDijeteId = odabranoDijete?.Id
            };

            if (odabranoDijete == null)
            {
                return View(vm);
            }

            var izvjestaji = await _context.DnevniIzvjestaji
                .Where(i => i.DijeteId == odabranoDijete.Id
                    && i.Datum.Date >= datumPocetka
                    && i.Datum.Date <= datumKraja)
                .OrderBy(i => i.Datum)
                .ToListAsync();

            vm.BrojDolazaka = izvjestaji
                .Select(i => i.Datum.Date)
                .Distinct()
                .Count();

            vm.BrojObroka =
                izvjestaji.Count(i => !string.IsNullOrWhiteSpace(i.Dorucak)
                    && i.StatusDorucka != StatusObroka.NIJE_POJEDENO)
                +
                izvjestaji.Count(i => !string.IsNullOrWhiteSpace(i.Rucak)
                    && i.StatusRucka != StatusObroka.NIJE_POJEDENO);

            vm.UkupnoSpavanjeMinuta = izvjestaji.Sum(i => i.SpavanjeMinuta);

            vm.Napomene = string.Join("\n",
                izvjestaji
                    .Where(i => !string.IsNullOrWhiteSpace(i.NapomenaAktivnosti))
                    .Select(i => $"{i.Datum:dd.MM.yyyy}: {i.NapomenaAktivnosti}"));

            return View(vm);
        }

        private bool SazetakAktivnostiExists(int id)
        {
            return _context.SazeciAktivnosti.Any(e => e.Id == id);
        }
    }

    public class RoditeljSazetakPregledViewModel
    {
        public string Period { get; set; } = "Sedmica";
        public DateTime DatumPocetka { get; set; }
        public DateTime DatumKraja { get; set; }

        public List<Dijete> Djeca { get; set; } = new();
        public int? OdabraniDijeteId { get; set; }
        public Dijete? Dijete { get; set; }

        public int BrojDolazaka { get; set; }
        public int BrojObroka { get; set; }
        public int UkupnoSpavanjeMinuta { get; set; }
        public string Napomene { get; set; } = string.Empty;
    }
}