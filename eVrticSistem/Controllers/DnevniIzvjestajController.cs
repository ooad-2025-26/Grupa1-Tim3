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
    public class DnevniIzvjestajController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Korisnik> _userManager;

        public DnevniIzvjestajController(ApplicationDbContext context, UserManager<Korisnik> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: DnevniIzvjestaj
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.DnevniIzvjestaji.Include(d => d.Dijete);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: DnevniIzvjestaj/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dnevniIzvjestaj = await _context.DnevniIzvjestaji
                .Include(d => d.Dijete)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dnevniIzvjestaj == null)
            {
                return NotFound();
            }

            return View(dnevniIzvjestaj);
        }

        // GET: DnevniIzvjestaj/Create
        public IActionResult Create()
        {
            ViewData["DijeteId"] = new SelectList(_context.Djeca, "Id", "IdentifikacioniKod");
            return View();
        }

        // POST: DnevniIzvjestaj/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Datum,Dorucak,StatusDorucka,Rucak,StatusRucka,SpavanjeMinuta,VrijemeDolaska,VrijemeOdlaska,NapomenaAktivnosti,DatumKreiranja,DijeteId")] DnevniIzvjestaj dnevniIzvjestaj)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dnevniIzvjestaj);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DijeteId"] = new SelectList(_context.Djeca, "Id", "IdentifikacioniKod", dnevniIzvjestaj.DijeteId);
            return View(dnevniIzvjestaj);
        }

        // GET: DnevniIzvjestaj/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dnevniIzvjestaj = await _context.DnevniIzvjestaji.FindAsync(id);
            if (dnevniIzvjestaj == null)
            {
                return NotFound();
            }
            ViewData["DijeteId"] = new SelectList(_context.Djeca, "Id", "IdentifikacioniKod", dnevniIzvjestaj.DijeteId);
            return View(dnevniIzvjestaj);
        }

        // POST: DnevniIzvjestaj/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Datum,Dorucak,StatusDorucka,Rucak,StatusRucka,SpavanjeMinuta,VrijemeDolaska,VrijemeOdlaska,NapomenaAktivnosti,DatumKreiranja,DijeteId")] DnevniIzvjestaj dnevniIzvjestaj)
        {
            if (id != dnevniIzvjestaj.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dnevniIzvjestaj);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DnevniIzvjestajExists(dnevniIzvjestaj.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DijeteId"] = new SelectList(_context.Djeca, "Id", "IdentifikacioniKod", dnevniIzvjestaj.DijeteId);
            return View(dnevniIzvjestaj);
        }

        // GET: DnevniIzvjestaj/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dnevniIzvjestaj = await _context.DnevniIzvjestaji
                .Include(d => d.Dijete)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dnevniIzvjestaj == null)
            {
                return NotFound();
            }

            return View(dnevniIzvjestaj);
        }

        // POST: DnevniIzvjestaj/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dnevniIzvjestaj = await _context.DnevniIzvjestaji.FindAsync(id);
            if (dnevniIzvjestaj != null)
            {
                _context.DnevniIzvjestaji.Remove(dnevniIzvjestaj);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ─── ODGAJATELJ: Evidencija aktivnosti djeteta ───────────────────────
        // (scenarij — odgajatelj unosi obroke (Doručak i Ručak), spavanje i napomene;
        //  izvještaj se može više puta ažurirati u toku istog dana)

        [Authorize(Roles = "ODGAJATELJ")]
        public async Task<IActionResult> EvidencijaAktivnosti(int? dijeteId, string? datum)
        {
            var korisnik = await _userManager.GetUserAsync(User);
            if (korisnik == null) return RedirectToPage("/Account/Login", new { area = "Identity" });

            var odgajatelj = await _context.Odgajatelji
                .FirstOrDefaultAsync(o => o.Id == korisnik.Id);

            if (odgajatelj == null) return RedirectToAction("OdgajateljHome", "Home");

            var odabraniDatum = (datum != null && DateTime.TryParse(datum, out var d))
                ? d.Date
                : DateTime.Today;

            var djeca = await _context.Djeca
                .Include(dj => dj.Grupa)
                .Where(dj => dj.Grupa != null
                    && dj.Grupa.OdgajateljId == odgajatelj.Id
                    && dj.Aktivno)
                .OrderBy(dj => dj.ImePrezime)
                .ToListAsync();

            var vm = new EvidencijaAktivnostiViewModel
            {
                Djeca = djeca,
                OdabraniDijeteId = dijeteId,
                Datum = odabraniDatum
            };

            if (dijeteId.HasValue)
            {
                // Da li ovo dijete već ima sačuvan izvještaj za odabrani dan?
                var postojeci = await _context.DnevniIzvjestaji
                    .FirstOrDefaultAsync(i => i.DijeteId == dijeteId.Value
                        && i.Datum.Date == odabraniDatum);

                vm.PostojeciIzvjestaj = postojeci;

                if (postojeci != null)
                {
                    // Prikaži djetetov vlastiti, već sačuvani unos
                    vm.DorucakTekst = postojeci.Dorucak;
                    vm.RucakTekst = postojeci.Rucak;
                    vm.StatusDoruckaVal = (int)postojeci.StatusDorucka;
                    vm.StatusRuckaVal = (int)postojeci.StatusRucka;
                }
                else
                {
                    // Prvi unos za OVO dijete danas → predloži tekst obroka na osnovu
                    // zadnjeg unesenog izvještaja za TAJ dan (jelovnik je isti za svu djecu).
                    // Status se NE predlaže — odgajatelj ga bira za svako dijete posebno.
                    var danasnji = await _context.DnevniIzvjestaji
                        .Where(i => i.Datum.Date == odabraniDatum)
                        .OrderByDescending(i => i.DatumKreiranja)
                        .ToListAsync();

                    vm.DorucakTekst = danasnji
                        .FirstOrDefault(i => !string.IsNullOrWhiteSpace(i.Dorucak))?.Dorucak ?? string.Empty;
                    vm.RucakTekst = danasnji
                        .FirstOrDefault(i => !string.IsNullOrWhiteSpace(i.Rucak))?.Rucak ?? string.Empty;

                    vm.TekstJePrijedlog = !string.IsNullOrWhiteSpace(vm.DorucakTekst)
                        || !string.IsNullOrWhiteSpace(vm.RucakTekst);
                    // statusi ostaju -1 (ništa nije unaprijed odabrano)
                }
            }

            return View(vm);
        }

        // POST: DnevniIzvjestaj/SacuvajAktivnosti
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ODGAJATELJ")]
        public async Task<IActionResult> SacuvajAktivnosti(
            int DijeteId,
            string Datum,
            string? DorucakTekst,
            string? StatusDorucka,
            string? RucakTekst,
            string? StatusRucka,
            string? SpavanjePocetakSati,
            string? SpavanjePocetakMinuta,
            string? SpavanjeKrajSati,
            string? SpavanjeKrajMinuta,
            string? Napomena)
        {
            var korisnik = await _userManager.GetUserAsync(User);
            if (korisnik == null) return Unauthorized();

            var odgajatelj = await _context.Odgajatelji
                .FirstOrDefaultAsync(o => o.Id == korisnik.Id);

            if (odgajatelj == null) return Forbid();

            if (!DateTime.TryParse(Datum, out var odabraniDatum))
                odabraniDatum = DateTime.Today;

            odabraniDatum = odabraniDatum.Date;

            // Provjeri da li dijete pripada grupi ovog odgajatelja
            var dijete = await _context.Djeca
                .Include(dj => dj.Grupa)
                .FirstOrDefaultAsync(dj => dj.Id == DijeteId
                    && dj.Grupa != null
                    && dj.Grupa.OdgajateljId == odgajatelj.Id);

            if (dijete == null) return NotFound();

            // Pronađi ili kreiraj izvještaj za taj dan (upsert)
            var izvjestaj = await _context.DnevniIzvjestaji
                .FirstOrDefaultAsync(i => i.DijeteId == DijeteId
                    && i.Datum.Date == odabraniDatum);

            if (izvjestaj == null)
            {
                izvjestaj = new DnevniIzvjestaj
                {
                    DijeteId = DijeteId,
                    Datum = odabraniDatum,
                    DatumKreiranja = DateTime.Now
                };
                _context.DnevniIzvjestaji.Add(izvjestaj);
            }

            // Parsiraj status obroka (vrijednost iz radio dugmadi je int)
            StatusObroka ParseStatus(string? val) =>
                int.TryParse(val, out int i) && Enum.IsDefined(typeof(StatusObroka), i)
                    ? (StatusObroka)i
                    : StatusObroka.NIJE_POJEDENO;

            // Obroci: tekst (šta se jelo) + status (koliko je dijete pojelo)
            izvjestaj.Dorucak = (DorucakTekst ?? string.Empty).Trim();
            izvjestaj.StatusDorucka = ParseStatus(StatusDorucka);
            izvjestaj.Rucak = (RucakTekst ?? string.Empty).Trim();
            izvjestaj.StatusRucka = ParseStatus(StatusRucka);

            // Parsiraj vrijeme spavanja (od / do)
            if (int.TryParse(SpavanjePocetakSati, out int pocSati)
                && int.TryParse(SpavanjePocetakMinuta, out int pocMin))
            {
                izvjestaj.VrijemeDolaska = new TimeSpan(pocSati, pocMin, 0);
            }
            else
            {
                izvjestaj.VrijemeDolaska = null;
            }

            if (int.TryParse(SpavanjeKrajSati, out int krajSati)
                && int.TryParse(SpavanjeKrajMinuta, out int krajMin))
            {
                izvjestaj.VrijemeOdlaska = new TimeSpan(krajSati, krajMin, 0);
            }
            else
            {
                izvjestaj.VrijemeOdlaska = null;
            }

            if (izvjestaj.VrijemeDolaska.HasValue && izvjestaj.VrijemeOdlaska.HasValue)
            {
                var razlika = izvjestaj.VrijemeOdlaska.Value - izvjestaj.VrijemeDolaska.Value;
                izvjestaj.SpavanjeMinuta = razlika.TotalMinutes > 0 ? (int)razlika.TotalMinutes : 0;
            }

            izvjestaj.NapomenaAktivnosti = Napomena ?? string.Empty;

            await _context.SaveChangesAsync();

            TempData["Uspjeh"] = "Aktivnosti su uspješno sačuvane.";
            return RedirectToAction(nameof(EvidencijaAktivnosti), new
            {
                dijeteId = DijeteId,
                datum = odabraniDatum.ToString("yyyy-MM-dd")
            });
        }

        private bool DnevniIzvjestajExists(int id)
        {
            return _context.DnevniIzvjestaji.Any(e => e.Id == id);
        }
    }

    // ─── ViewModel za evidenciju aktivnosti ──────────────────────────────────

    public class EvidencijaAktivnostiViewModel
    {
        public List<Dijete> Djeca { get; set; } = new();
        public int? OdabraniDijeteId { get; set; }
        public DateTime Datum { get; set; } = DateTime.Today;
        public DnevniIzvjestaj? PostojeciIzvjestaj { get; set; }

        // Tekst obroka za prikaz u formi (djetetov vlastiti unos ILI dnevni prijedlog)
        public string DorucakTekst { get; set; } = string.Empty;
        public string RucakTekst { get; set; } = string.Empty;

        // true ako je prikazani tekst samo prijedlog (zadnji uneseni obrok tog dana),
        // a ne vlastiti, već sačuvani unos za ovo dijete
        public bool TekstJePrijedlog { get; set; }

        // Status obroka: -1 = ništa nije odabrano (novi unos), inače 0/1/2
        public int StatusDoruckaVal { get; set; } = -1;
        public int StatusRuckaVal { get; set; } = -1;

        // Zadržano radi kompatibilnosti sa starim (neaktivnim) pogledom
        // Views/Odgajatelj/EvidencijaAktivnosti.cshtml
        public Dictionary<string, int>? ObrociMap { get; set; }
    }
}
