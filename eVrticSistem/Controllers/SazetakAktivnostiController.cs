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
        public async Task<IActionResult> RoditeljPregled(
            string period = "Sedmica",
            DateTime? pocetak = null,
            int? dijeteId = null)
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
                : null;

            const int prvaGodinaAplikacije = 2024;
            int zadnjaGodinaAplikacije = DateTime.Today.Year;

            var prviDanKalendara = new DateTime(prvaGodinaAplikacije, 1, 1);
            var zadnjiDanKalendara = new DateTime(zadnjaGodinaAplikacije, 12, 31);

            period = period.Equals("Mjesec", StringComparison.OrdinalIgnoreCase)
                ? "Mjesec"
                : "Sedmica";

            var datumPocetka = pocetak?.Date ?? DateTime.Today.Date;
            DateTime datumKraja;

            if (period == "Mjesec")
            {
                datumPocetka = new DateTime(datumPocetka.Year, datumPocetka.Month, 1);
                datumKraja = datumPocetka.AddMonths(1).AddDays(-1);
            }
            else
            {
                datumPocetka = PocetakRadneSedmice(datumPocetka);
                datumKraja = datumPocetka.AddDays(4);
            }

            var vm = new RoditeljSazetakPregledViewModel
            {
                Period = period,
                DatumPocetka = datumPocetka,
                DatumKraja = datumKraja,
                Djeca = djeca,
                Dijete = odabranoDijete,
                OdabraniDijeteId = dijeteId,
                SedmicneOpcije = GenerisiSedmicneOpcije(prviDanKalendara, zadnjiDanKalendara),
                MjesecneOpcije = GenerisiMjesecneOpcije(prvaGodinaAplikacije, zadnjaGodinaAplikacije)
            };

            if (odabranoDijete == null)
            {
                return View(vm);
            }

            var izvjestajiRaw = await _context.DnevniIzvjestaji
                .Where(i => i.DijeteId == odabranoDijete.Id
                    && i.Datum.Date >= datumPocetka
                    && i.Datum.Date <= datumKraja)
                .OrderBy(i => i.Datum)
                .ToListAsync();

            var izvjestaji = izvjestajiRaw
                .Where(i => JeRadniDan(i.Datum))
                .ToList();

            var evidencijeRaw = await _context.EvidencijeDolaskaOdlaska
                .Where(e => e.DijeteId == odabranoDijete.Id
                    && e.StatusEvidencije == StatusEvidencije.EVIDENTIRANO
                    && e.VrijemeDogadjaja.Date >= datumPocetka
                    && e.VrijemeDogadjaja.Date <= datumKraja)
                .ToListAsync();

            var evidencije = evidencijeRaw
                .Where(e => JeRadniDan(e.VrijemeDogadjaja))
                .ToList();

            vm.BrojDolazaka = evidencije.Count(e => e.TipDogadjaja == TipDogadjaja.DOLAZAK);
            vm.BrojOdlazaka = evidencije.Count(e => e.TipDogadjaja == TipDogadjaja.ODLAZAK);

            var dorucakOcjene = izvjestaji
                .Where(i => !string.IsNullOrWhiteSpace(i.Dorucak))
                .Select(i => VrijednostStatusaObroka(i.StatusDorucka))
                .ToList();

            var rucakOcjene = izvjestaji
                .Where(i => !string.IsNullOrWhiteSpace(i.Rucak))
                .Select(i => VrijednostStatusaObroka(i.StatusRucka))
                .ToList();

            vm.BrojDorucaka = dorucakOcjene.Count;
            vm.BrojRucaka = rucakOcjene.Count;
            vm.BrojObroka = vm.BrojDorucaka + vm.BrojRucaka;

            vm.ProsjekDorucka = dorucakOcjene.Any()
                ? dorucakOcjene.Average()
                : null;

            vm.ProsjekRucka = rucakOcjene.Any()
                ? rucakOcjene.Average()
                : null;

            vm.OcjenaDorucka = PorukaZaObrok("Doručak", vm.ProsjekDorucka);
            vm.OcjenaRucka = PorukaZaObrok("Ručak", vm.ProsjekRucka);

            vm.UkupnoSpavanjeMinuta = izvjestaji.Sum(i => i.SpavanjeMinuta);

            if (vm.BrojDolazaka > 0)
            {
                vm.ProsjecnoSpavanjeMinuta = vm.UkupnoSpavanjeMinuta / vm.BrojDolazaka;
                vm.OcjenaSpavanja = PorukaZaSpavanje(vm.ProsjecnoSpavanjeMinuta);
            }
            else
            {
                vm.ProsjecnoSpavanjeMinuta = 0;
                vm.OcjenaSpavanja = "Nema dovoljno podataka za ocjenu spavanja jer nema evidentiranih dolazaka.";
            }

            vm.NapomenePoDatumima = izvjestaji
                .Where(i => !string.IsNullOrWhiteSpace(i.NapomenaAktivnosti))
                .Select(i => new NapomenaSazetkaViewModel
                {
                    Datum = i.Datum.Date,
                    Tekst = i.NapomenaAktivnosti
                })
                .ToList();

            vm.Napomene = string.Join("\n",
                vm.NapomenePoDatumima.Select(n => $"{n.Datum:dd.MM.yyyy}: {n.Tekst}"));

            return View(vm);
        }
        // ═══════════════════════════════════════════════════════════════════
        // ODGAJATELJ — Pregled sažetaka djece iz njegovih grupa
        // ═══════════════════════════════════════════════════════════════════
        [Authorize(Roles = "ODGAJATELJ")]
        public async Task<IActionResult> OdgajateljPregled(
            string period = "Sedmica",
            DateTime? pocetak = null,
            int? grupaId = null,
            bool generisi = false)
        {
            var korisnik = await _userManager.GetUserAsync(User);

            if (korisnik == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var grupe = await _context.Grupe
                .Where(g => g.OdgajateljId == korisnik.Id)
                .OrderBy(g => g.ImeGrupe)
                .ToListAsync();

            var idGrupa = grupe.Select(g => g.Id).ToList();

            var djecaQuery = _context.Djeca
                .Include(d => d.Grupa)
                .Where(d => d.Aktivno && d.GrupaId != null && idGrupa.Contains(d.GrupaId.Value));

            if (grupaId.HasValue)
            {
                djecaQuery = djecaQuery.Where(d => d.GrupaId == grupaId.Value);
            }

            var djeca = await djecaQuery
                .OrderBy(d => d.Grupa!.ImeGrupe)
                .ThenBy(d => d.ImePrezime)
                .ToListAsync();

            const int prvaGodinaAplikacije = 2024;
            int zadnjaGodinaAplikacije = DateTime.Today.Year;

            var prviDanKalendara = new DateTime(prvaGodinaAplikacije, 1, 1);
            var zadnjiDanKalendara = new DateTime(zadnjaGodinaAplikacije, 12, 31);

            period = period.Equals("Mjesec", StringComparison.OrdinalIgnoreCase)
                ? "Mjesec"
                : "Sedmica";

            var datumPocetka = pocetak?.Date ?? DateTime.Today.Date;
            DateTime datumKraja;

            if (period == "Mjesec")
            {
                datumPocetka = new DateTime(datumPocetka.Year, datumPocetka.Month, 1);
                datumKraja = datumPocetka.AddMonths(1).AddDays(-1);
            }
            else
            {
                datumPocetka = PocetakRadneSedmice(datumPocetka);
                datumKraja = datumPocetka.AddDays(4);
            }

            var vm = new OdgajateljSazetakPregledViewModel
            {
                Period = period,
                DatumPocetka = datumPocetka,
                DatumKraja = datumKraja,
                Grupe = grupe,
                Djeca = djeca,
                OdabranaGrupaId = grupaId,
                PrikaziSazetak = generisi,
                SedmicneOpcije = GenerisiSedmicneOpcije(prviDanKalendara, zadnjiDanKalendara),
                MjesecneOpcije = GenerisiMjesecneOpcije(prvaGodinaAplikacije, zadnjaGodinaAplikacije)
            };

            if (!generisi)
            {
                return View(vm);
            }

            var idDjece = djeca.Select(d => d.Id).ToList();

            if (!idDjece.Any())
            {
                return View(vm);
            }

            var izvjestajiRaw = await _context.DnevniIzvjestaji
                .Where(i => idDjece.Contains(i.DijeteId)
                    && i.Datum.Date >= datumPocetka
                    && i.Datum.Date <= datumKraja)
                .OrderBy(i => i.Datum)
                .ToListAsync();

            var izvjestaji = izvjestajiRaw
                .Where(i => JeRadniDan(i.Datum))
                .ToList();

            var evidencijeRaw = await _context.EvidencijeDolaskaOdlaska
                .Where(e => idDjece.Contains(e.DijeteId)
                    && e.StatusEvidencije == StatusEvidencije.EVIDENTIRANO
                    && e.VrijemeDogadjaja.Date >= datumPocetka
                    && e.VrijemeDogadjaja.Date <= datumKraja)
                .ToListAsync();

            var evidencije = evidencijeRaw
                .Where(e => JeRadniDan(e.VrijemeDogadjaja))
                .ToList();

            foreach (var dijete in djeca)
            {
                var izvjestajiDjeteta = izvjestaji
                    .Where(i => i.DijeteId == dijete.Id)
                    .ToList();

                var evidencijeDjeteta = evidencije
                    .Where(e => e.DijeteId == dijete.Id)
                    .ToList();

                var sazetak = new DijeteSazetakViewModel
                {
                    Dijete = dijete,
                    BrojDolazaka = evidencijeDjeteta.Count(e => e.TipDogadjaja == TipDogadjaja.DOLAZAK),
                    BrojOdlazaka = evidencijeDjeteta.Count(e => e.TipDogadjaja == TipDogadjaja.ODLAZAK)
                };

                var dorucakOcjene = izvjestajiDjeteta
                    .Where(i => !string.IsNullOrWhiteSpace(i.Dorucak))
                    .Select(i => VrijednostStatusaObroka(i.StatusDorucka))
                    .ToList();

                var rucakOcjene = izvjestajiDjeteta
                    .Where(i => !string.IsNullOrWhiteSpace(i.Rucak))
                    .Select(i => VrijednostStatusaObroka(i.StatusRucka))
                    .ToList();

                sazetak.BrojDorucaka = dorucakOcjene.Count;
                sazetak.BrojRucaka = rucakOcjene.Count;
                sazetak.BrojObroka = sazetak.BrojDorucaka + sazetak.BrojRucaka;

                sazetak.ProsjekDorucka = dorucakOcjene.Any()
                    ? dorucakOcjene.Average()
                    : null;

                sazetak.ProsjekRucka = rucakOcjene.Any()
                    ? rucakOcjene.Average()
                    : null;

                sazetak.OcjenaDorucka = PorukaZaObrok("Doručak", sazetak.ProsjekDorucka);
                sazetak.OcjenaRucka = PorukaZaObrok("Ručak", sazetak.ProsjekRucka);

                sazetak.UkupnoSpavanjeMinuta = izvjestajiDjeteta.Sum(i => i.SpavanjeMinuta);

                if (sazetak.BrojDolazaka > 0)
                {
                    sazetak.ProsjecnoSpavanjeMinuta = sazetak.UkupnoSpavanjeMinuta / sazetak.BrojDolazaka;
                    sazetak.OcjenaSpavanja = PorukaZaSpavanje(sazetak.ProsjecnoSpavanjeMinuta);
                }
                else
                {
                    sazetak.ProsjecnoSpavanjeMinuta = 0;
                    sazetak.OcjenaSpavanja = "Nema dovoljno podataka za ocjenu spavanja jer nema evidentiranih dolazaka.";
                }

                sazetak.NapomenePoDatumima = izvjestajiDjeteta
                    .Where(i => !string.IsNullOrWhiteSpace(i.NapomenaAktivnosti))
                    .Select(i => new NapomenaSazetkaViewModel
                    {
                        Datum = i.Datum.Date,
                        Tekst = i.NapomenaAktivnosti
                    })
                    .ToList();

                vm.SazeciDjece.Add(sazetak);
            }

            return View(vm);
        }



        private static bool JeRadniDan(DateTime datum)
        {
            return datum.DayOfWeek != DayOfWeek.Saturday
                && datum.DayOfWeek != DayOfWeek.Sunday;
        }

        private static DateTime PocetakRadneSedmice(DateTime datum)
        {
            int diff = ((int)datum.DayOfWeek + 6) % 7;
            return datum.Date.AddDays(-diff);
        }

        private static int VrijednostStatusaObroka(StatusObroka status)
        {
            return status switch
            {
                StatusObroka.NIJE_POJEDENO => 1,
                StatusObroka.DJELIMICNO_POJEDENO => 2,
                StatusObroka.POTPUNO_POJEDENO => 3,
                _ => 1
            };
        }

        private static string PorukaZaObrok(string nazivObroka, double? prosjek)
        {
            if (!prosjek.HasValue)
            {
                return $"Nema dovoljno podataka za ocjenu: {nazivObroka.ToLower()}.";
            }

            if (prosjek.Value < 1.5)
            {
                return $"{nazivObroka} je uglavnom slabo pojeden.";
            }

            if (prosjek.Value < 2.5)
            {
                return $"{nazivObroka} je uglavnom djelimično pojeden.";
            }

            return $"{nazivObroka} je uglavnom dobro pojeden.";
        }

        private static string PorukaZaSpavanje(int prosjekMinuta)
        {
            if (prosjekMinuta < 60)
            {
                return "Dijete je slabo spavalo u odabranom periodu.";
            }

            if (prosjekMinuta <= 90)
            {
                return "Dijete je dovoljno spavalo u odabranom periodu.";
            }

            return "Dijete je dobro spavalo u odabranom periodu.";
        }

        private static List<PeriodOpcijaViewModel> GenerisiSedmicneOpcije(DateTime najranijiDatum, DateTime najkasnijiDatum)
        {
            var opcije = new List<PeriodOpcijaViewModel>();

            var pocetak = PocetakRadneSedmice(najranijiDatum);
            var kraj = PocetakRadneSedmice(najkasnijiDatum);

            for (var sedmica = kraj; sedmica >= pocetak; sedmica = sedmica.AddDays(-7))
            {
                var radniKraj = sedmica.AddDays(4);

                opcije.Add(new PeriodOpcijaViewModel
                {
                    Vrijednost = sedmica.ToString("yyyy-MM-dd"),
                    Tekst = $"{sedmica:dd.MM.yyyy.} - {radniKraj:dd.MM.yyyy.}"
                });
            }

            return opcije;
        }

        private static List<PeriodOpcijaViewModel> GenerisiMjesecneOpcije(int pocetnaGodina, int krajnjaGodina)
        {
            var mjeseci = new[]
            {
        "Januar", "Februar", "Mart", "April", "Maj", "Juni",
        "Juli", "August", "Septembar", "Oktobar", "Novembar", "Decembar"
    };

            var opcije = new List<PeriodOpcijaViewModel>();

            for (int godina = krajnjaGodina; godina >= pocetnaGodina; godina--)
            {
                for (int mjesec = 12; mjesec >= 1; mjesec--)
                {
                    var datum = new DateTime(godina, mjesec, 1);

                    opcije.Add(new PeriodOpcijaViewModel
                    {
                        Vrijednost = datum.ToString("yyyy-MM-dd"),
                        Tekst = $"{mjeseci[mjesec - 1]} {godina}"
                    });
                }
            }

            return opcije;
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
        public int BrojOdlazaka { get; set; }

        public int BrojObroka { get; set; }
        public int BrojDorucaka { get; set; }
        public int BrojRucaka { get; set; }

        public double? ProsjekDorucka { get; set; }
        public double? ProsjekRucka { get; set; }

        public string OcjenaDorucka { get; set; } = string.Empty;
        public string OcjenaRucka { get; set; } = string.Empty;

        public int UkupnoSpavanjeMinuta { get; set; }
        public int ProsjecnoSpavanjeMinuta { get; set; }
        public string OcjenaSpavanja { get; set; } = string.Empty;

        public string Napomene { get; set; } = string.Empty;
        public List<NapomenaSazetkaViewModel> NapomenePoDatumima { get; set; } = new();

        public List<PeriodOpcijaViewModel> SedmicneOpcije { get; set; } = new();
        public List<PeriodOpcijaViewModel> MjesecneOpcije { get; set; } = new();
    }

    public class OdgajateljSazetakPregledViewModel
    {
        public List<DijeteSazetakViewModel> SazeciDjece { get; set; } = new();
        public string Period { get; set; } = "Sedmica";
        public DateTime DatumPocetka { get; set; }
        public DateTime DatumKraja { get; set; }

        public List<Grupa> Grupe { get; set; } = new();
        public List<Dijete> Djeca { get; set; } = new();

        public int? OdabranaGrupaId { get; set; }
        public int? OdabraniDijeteId { get; set; }

        public Dijete? Dijete { get; set; }

        public int BrojDolazaka { get; set; }
        public int BrojOdlazaka { get; set; }

        public int BrojObroka { get; set; }
        public int BrojDorucaka { get; set; }
        public int BrojRucaka { get; set; }

        public double? ProsjekDorucka { get; set; }
        public double? ProsjekRucka { get; set; }

        public string OcjenaDorucka { get; set; } = string.Empty;
        public string OcjenaRucka { get; set; } = string.Empty;

        public int UkupnoSpavanjeMinuta { get; set; }
        public int ProsjecnoSpavanjeMinuta { get; set; }
        public string OcjenaSpavanja { get; set; } = string.Empty;

        public string Napomene { get; set; } = string.Empty;
        public List<NapomenaSazetkaViewModel> NapomenePoDatumima { get; set; } = new();

        public List<PeriodOpcijaViewModel> SedmicneOpcije { get; set; } = new();
        public List<PeriodOpcijaViewModel> MjesecneOpcije { get; set; } = new();
        public bool PrikaziSazetak { get; set; }
    }

    public class DijeteSazetakViewModel
    {
        public Dijete Dijete { get; set; } = new();

        public int BrojDolazaka { get; set; }
        public int BrojOdlazaka { get; set; }

        public int BrojObroka { get; set; }
        public int BrojDorucaka { get; set; }
        public int BrojRucaka { get; set; }

        public double? ProsjekDorucka { get; set; }
        public double? ProsjekRucka { get; set; }

        public string OcjenaDorucka { get; set; } = string.Empty;
        public string OcjenaRucka { get; set; } = string.Empty;

        public int UkupnoSpavanjeMinuta { get; set; }
        public int ProsjecnoSpavanjeMinuta { get; set; }
        public string OcjenaSpavanja { get; set; } = string.Empty;

        public List<NapomenaSazetkaViewModel> NapomenePoDatumima { get; set; } = new();
    }

    public class PeriodOpcijaViewModel
    {
        public string Vrijednost { get; set; } = string.Empty;
        public string Tekst { get; set; } = string.Empty;
    }

    public class NapomenaSazetkaViewModel
    {
        public DateTime Datum { get; set; }
        public string Tekst { get; set; } = string.Empty;
    }


}