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
    public class EvidencijaDolaskaOdlaskaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Korisnik> _userManager;

        public EvidencijaDolaskaOdlaskaController(ApplicationDbContext context, UserManager<Korisnik> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: EvidencijaDolaskaOdlaska
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.EvidencijeDolaskaOdlaska.Include(e => e.Dijete).Include(e => e.DnevniQRCode);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: EvidencijaDolaskaOdlaska/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evidencijaDolaskaOdlaska = await _context.EvidencijeDolaskaOdlaska
                .Include(e => e.Dijete)
                .Include(e => e.DnevniQRCode)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (evidencijaDolaskaOdlaska == null)
            {
                return NotFound();
            }

            return View(evidencijaDolaskaOdlaska);
        }

        // GET: EvidencijaDolaskaOdlaska/Create
        public IActionResult Create()
        {
            ViewData["DijeteId"] = new SelectList(_context.Djeca, "Id", "IdentifikacioniKod");
            ViewData["DnevniQRCodeId"] = new SelectList(_context.DnevniQRCodovi, "Id", "VrijednostKoda");
            return View();
        }

        // POST: EvidencijaDolaskaOdlaska/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,VrijemeDogadjaja,TipDogadjaja,UneseniKodDjeteta,ValidanQRKod,KodDjetetaIspravan,StatusEvidencije,DijeteId,DnevniQRCodeId")] EvidencijaDolaskaOdlaska evidencijaDolaskaOdlaska)
        {
            if (ModelState.IsValid)
            {
                _context.Add(evidencijaDolaskaOdlaska);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DijeteId"] = new SelectList(_context.Djeca, "Id", "IdentifikacioniKod", evidencijaDolaskaOdlaska.DijeteId);
            ViewData["DnevniQRCodeId"] = new SelectList(_context.DnevniQRCodovi, "Id", "VrijednostKoda", evidencijaDolaskaOdlaska.DnevniQRCodeId);
            return View(evidencijaDolaskaOdlaska);
        }

        // GET: EvidencijaDolaskaOdlaska/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evidencijaDolaskaOdlaska = await _context.EvidencijeDolaskaOdlaska.FindAsync(id);
            if (evidencijaDolaskaOdlaska == null)
            {
                return NotFound();
            }
            ViewData["DijeteId"] = new SelectList(_context.Djeca, "Id", "IdentifikacioniKod", evidencijaDolaskaOdlaska.DijeteId);
            ViewData["DnevniQRCodeId"] = new SelectList(_context.DnevniQRCodovi, "Id", "VrijednostKoda", evidencijaDolaskaOdlaska.DnevniQRCodeId);
            return View(evidencijaDolaskaOdlaska);
        }

        // POST: EvidencijaDolaskaOdlaska/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,VrijemeDogadjaja,TipDogadjaja,UneseniKodDjeteta,ValidanQRKod,KodDjetetaIspravan,StatusEvidencije,DijeteId,DnevniQRCodeId")] EvidencijaDolaskaOdlaska evidencijaDolaskaOdlaska)
        {
            if (id != evidencijaDolaskaOdlaska.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(evidencijaDolaskaOdlaska);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EvidencijaDolaskaOdlaskaExists(evidencijaDolaskaOdlaska.Id))
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
            ViewData["DijeteId"] = new SelectList(_context.Djeca, "Id", "IdentifikacioniKod", evidencijaDolaskaOdlaska.DijeteId);
            ViewData["DnevniQRCodeId"] = new SelectList(_context.DnevniQRCodovi, "Id", "VrijednostKoda", evidencijaDolaskaOdlaska.DnevniQRCodeId);
            return View(evidencijaDolaskaOdlaska);
        }

        // GET: EvidencijaDolaskaOdlaska/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evidencijaDolaskaOdlaska = await _context.EvidencijeDolaskaOdlaska
                .Include(e => e.Dijete)
                .Include(e => e.DnevniQRCode)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (evidencijaDolaskaOdlaska == null)
            {
                return NotFound();
            }

            return View(evidencijaDolaskaOdlaska);
        }

        // POST: EvidencijaDolaskaOdlaska/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var evidencijaDolaskaOdlaska = await _context.EvidencijeDolaskaOdlaska.FindAsync(id);
            if (evidencijaDolaskaOdlaska != null)
            {
                _context.EvidencijeDolaskaOdlaska.Remove(evidencijaDolaskaOdlaska);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "RODITELJ")]
        public async Task<IActionResult> RoditeljEvidencija()
        {
            var korisnik = await _userManager.GetUserAsync(User);
            if (korisnik == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            var qrKod = await VratiAktivniQrKod();
            var djeca = await VratiDjecuRoditelja(korisnik.Id);

            return View(new RoditeljEvidencijaDolaskaOdlaskaViewModel
            {
                DnevniQRCode = qrKod,
                Djeca = djeca,
                OdabranoDijeteId = djeca.FirstOrDefault()?.Id,
                TipDogadjaja = "DOLAZAK"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "RODITELJ")]
        public async Task<IActionResult> RoditeljEvidencija(RoditeljEvidencijaDolaskaOdlaskaViewModel model)
        {
            var korisnik = await _userManager.GetUserAsync(User);
            if (korisnik == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            var djeca = await VratiDjecuRoditelja(korisnik.Id);
            model.Djeca = djeca;
            model.DnevniQRCode = await VratiAktivniQrKod();

            // 1) Validacija skeniranog dnevnog QR koda
            var skenirano = (model.ScaniraniKod ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(skenirano))
            {
                ModelState.AddModelError(string.Empty, "Niste skenirali QR kod. Skenirajte dnevni QR kod prije evidentiranja.");
                return View(model);
            }

            var qrKod = await _context.DnevniQRCodovi
                .Where(q => q.Aktivan
                    && q.DatumVazenja.Date == DateTime.Today
                    && q.VrijemeIsteka >= DateTime.Now
                    && q.VrijednostKoda == skenirano)
                .OrderByDescending(q => q.VrijemeIsteka)
                .FirstOrDefaultAsync();

            if (qrKod == null)
            {
                ModelState.AddModelError(string.Empty, "QR kod nije validan ili je istekao. Zatražite od odgajatelja današnji QR kod.");
                return View(model);
            }
            model.DnevniQRCode = qrKod;

            // 2) Odabir djeteta (ponuđena su samo djeca prijavljenog roditelja)
            if (!model.OdabranoDijeteId.HasValue)
            {
                ModelState.AddModelError(nameof(model.OdabranoDijeteId), "Odaberite dijete za evidentiranje.");
                return View(model);
            }

            var dijete = djeca.FirstOrDefault(d => d.Id == model.OdabranoDijeteId.Value);
            if (dijete == null)
            {
                ModelState.AddModelError(nameof(model.OdabranoDijeteId), "Odabrano dijete nije povezano s vašim profilom.");
                return View(model);
            }

            // 3) QR kod mora pripadati odgajatelju grupe u kojoj se nalazi dijete
            var odgajateljIdKoda = ParsirajOdgajateljaIzKoda(qrKod.VrijednostKoda);
            if (odgajateljIdKoda == null)
            {
                ModelState.AddModelError(string.Empty, "QR kod nije ispravan. Zatražite od odgajatelja novi QR kod.");
                return View(model);
            }

            if (dijete.GrupaId == null || dijete.Grupa == null || dijete.Grupa.OdgajateljId != odgajateljIdKoda)
            {
                ModelState.AddModelError(string.Empty, "Ovaj QR kod ne pripada odgajatelju grupe vašeg djeteta. Skenirajte QR kod odgajatelja iz grupe u kojoj je vaše dijete.");
                return View(model);
            }

            // 4) Tip događaja (dolazak/odlazak) dolazi iz skeniranog QR koda
            var tip = ParsirajTipDogadjaja(model.TipDogadjaja);

            // 5) Sprječavanje dvostrukog evidentiranja istog dana
            var danas = DateTime.Today;
            var danasnjeEvidencije = await _context.EvidencijeDolaskaOdlaska
                .Where(e => e.DijeteId == dijete.Id
                    && e.StatusEvidencije == StatusEvidencije.EVIDENTIRANO
                    && e.VrijemeDogadjaja.Date == danas)
                .ToListAsync();

            bool vecImaDolazak = danasnjeEvidencije.Any(e => e.TipDogadjaja == TipDogadjaja.DOLAZAK);
            bool vecImaOdlazak = danasnjeEvidencije.Any(e => e.TipDogadjaja == TipDogadjaja.ODLAZAK);

            if (tip == TipDogadjaja.DOLAZAK && vecImaDolazak)
            {
                ModelState.AddModelError(string.Empty, $"Dolazak za dijete {dijete.ImePrezime} je već evidentiran danas.");
                return View(model);
            }

            if (tip == TipDogadjaja.ODLAZAK)
            {
                if (!vecImaDolazak)
                {
                    ModelState.AddModelError(string.Empty, $"Nije moguće evidentirati odlazak — dolazak za dijete {dijete.ImePrezime} danas nije evidentiran.");
                    return View(model);
                }

                if (vecImaOdlazak)
                {
                    ModelState.AddModelError(string.Empty, $"Odlazak za dijete {dijete.ImePrezime} je već evidentiran danas.");
                    return View(model);
                }
            }

            var evidencija = new EvidencijaDolaskaOdlaska
            {
                VrijemeDogadjaja = DateTime.Now,
                TipDogadjaja = tip,
                UneseniKodDjeteta = dijete.IdentifikacioniKod,
                ValidanQRKod = true,
                KodDjetetaIspravan = true,
                StatusEvidencije = StatusEvidencije.EVIDENTIRANO,
                DijeteId = dijete.Id,
                DnevniQRCodeId = qrKod.Id
            };

            _context.EvidencijeDolaskaOdlaska.Add(evidencija);
            await _context.SaveChangesAsync();

            ModelState.Clear();

            return View(new RoditeljEvidencijaDolaskaOdlaskaViewModel
            {
                DnevniQRCode = qrKod,
                Djeca = djeca,
                OdabranoDijeteId = dijete.Id,
                TipDogadjaja = tip.ToString(),
                UspjesnaPoruka = $"{(tip == TipDogadjaja.DOLAZAK ? "Dolazak" : "Odlazak")} je uspješno evidentiran za dijete {dijete.ImePrezime}."
            });
        }

        private async Task<List<Dijete>> VratiDjecuRoditelja(int roditeljId)
        {
            return await _context.Djeca
                .Include(d => d.Grupa)
                .Where(d => d.RoditeljId == roditeljId && d.Aktivno)
                .OrderBy(d => d.ImePrezime)
                .ToListAsync();
        }

        private async Task<DnevniQRCode?> VratiAktivniQrKod()
        {
            var sada = DateTime.Now;
            var danas = DateTime.Today;

            return await _context.DnevniQRCodovi
                .Where(q => q.Aktivan
                    && q.DatumVazenja.Date == danas
                    && q.VrijemeIsteka >= sada)
                .OrderByDescending(q => q.VrijemeIsteka)
                .FirstOrDefaultAsync();
        }

        private static TipDogadjaja ParsirajTipDogadjaja(string? vrijednost)
        {
            if (Enum.TryParse<TipDogadjaja>(vrijednost, true, out var tip))
                return tip;

            return Enum.GetValues(typeof(TipDogadjaja))
                .Cast<TipDogadjaja>()
                .First();
        }

        // ═══════════════════════════════════════════════════════════════════
        // ODGAJATELJ — Pregled dolazaka i odlazaka djece iz njegovih grupa
        // ═══════════════════════════════════════════════════════════════════

        [Authorize(Roles = "ODGAJATELJ")]
        public async Task<IActionResult> OdgajateljPregled(DateTime? datum = null, int? grupaId = null, int? dijeteId = null, string? tip = null)
        {
            var korisnik = await _userManager.GetUserAsync(User);

            if (korisnik == null)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var odabraniDatum = (datum ?? DateTime.Today).Date;

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

            // OVA LISTA IDE U DROPDOWN — uvijek sva djeca iz odabrane grupe/svih grupa
            var djecaZaDropdown = await djecaQuery
                .OrderBy(d => d.ImePrezime)
                .ToListAsync();

            // OVA LISTA IDE ZA PRIKAZ — sva djeca ili samo odabrano dijete
            var djecaZaPrikaz = djecaZaDropdown;

            if (dijeteId.HasValue)
            {
                djecaZaPrikaz = djecaZaDropdown
                    .Where(d => d.Id == dijeteId.Value)
                    .ToList();
            }

            var idDjeceZaPrikaz = djecaZaPrikaz.Select(d => d.Id).ToList();

            var evidencije = await _context.EvidencijeDolaskaOdlaska
                .Include(e => e.Dijete)
                    .ThenInclude(d => d.Grupa)
                .Include(e => e.DnevniQRCode)
                .Where(e => idDjeceZaPrikaz.Contains(e.DijeteId)
                    && e.VrijemeDogadjaja.Date == odabraniDatum)
                .OrderByDescending(e => e.VrijemeDogadjaja)
                .ToListAsync();

            var vm = new OdgajateljEvidencijaPregledViewModel
            {
                Datum = odabraniDatum,
                Grupe = grupe,

                // BITNO: dropdown dobija sva djeca, ne samo filtrirano dijete
                Djeca = djecaZaDropdown,

                Evidencije = evidencije,
                OdabranaGrupaId = grupaId,
                OdabranoDijeteId = dijeteId,

                BrojDolazaka = evidencije.Count(e => e.TipDogadjaja == TipDogadjaja.DOLAZAK),
                BrojOdlazaka = evidencije.Count(e => e.TipDogadjaja == TipDogadjaja.ODLAZAK),
                BrojOdbijenih = evidencije.Count(e => e.StatusEvidencije == StatusEvidencije.ODBIJENO),

                AktivniQRKod = await VratiAktivniQrKodZaOdgajatelja(korisnik.Id),
                OdabraniTip = (tip == "ODLAZAK") ? "ODLAZAK" : "DOLAZAK"
            };

            return View(vm);
        }

        // POST: EvidencijaDolaskaOdlaska/OdgajateljGenerisiQR
        // Generiše novi jedinstveni dnevni QR kod i sprema ga u bazu.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "ODGAJATELJ")]
        public async Task<IActionResult> OdgajateljGenerisiQR(string? tip = null)
        {
            var korisnik = await _userManager.GetUserAsync(User);
            if (korisnik == null)
                return RedirectToPage("/Account/Login", new { area = "Identity" });

            var danas = DateTime.Today;
            var prefiks = $"EVRTIC-{danas:yyyyMMdd}-O{korisnik.Id}-";

            // Deaktiviraj prethodne današnje kodove OVOG odgajatelja (jedan aktivan kod po odgajatelju/danu)
            var mojiDanasnji = await _context.DnevniQRCodovi
                .Where(q => q.Aktivan
                    && q.DatumVazenja.Date == danas
                    && q.VrijednostKoda.StartsWith(prefiks))
                .ToListAsync();

            foreach (var kod in mojiDanasnji)
            {
                kod.Aktivan = false;
            }

            var noviKod = new DnevniQRCode
            {
                VrijednostKoda = GenerisiVrijednostKoda(korisnik.Id),
                DatumVazenja = danas,
                VrijemeIsteka = danas.AddDays(1).AddSeconds(-1),
                Aktivan = true
            };

            _context.DnevniQRCodovi.Add(noviKod);
            await _context.SaveChangesAsync();

            TempData["Poruka"] = "Novi dnevni QR kod je uspješno generisan.";
            return RedirectToAction(nameof(OdgajateljPregled), new { tip });
        }

        private static string GenerisiVrijednostKoda(int odgajateljId)
        {
            var token = Guid.NewGuid().ToString("N")[..10].ToUpper();
            return $"EVRTIC-{DateTime.Today:yyyyMMdd}-O{odgajateljId}-{token}";
        }

        // Iz vrijednosti koda (EVRTIC-yyyyMMdd-O{id}-XXXX) vraća ID odgajatelja koji ga je kreirao
        private static int? ParsirajOdgajateljaIzKoda(string? kod)
        {
            if (string.IsNullOrWhiteSpace(kod))
                return null;

            var dijelovi = kod.Split('-');
            if (dijelovi.Length < 4)
                return null;

            var segment = dijelovi[2];
            if (segment.Length < 2 || segment[0] != 'O')
                return null;

            return int.TryParse(segment.Substring(1), out var id) ? id : (int?)null;
        }

        // Trenutno aktivni dnevni QR kod konkretnog odgajatelja
        private async Task<DnevniQRCode?> VratiAktivniQrKodZaOdgajatelja(int odgajateljId)
        {
            var danas = DateTime.Today;
            var sada = DateTime.Now;
            var prefiks = $"EVRTIC-{danas:yyyyMMdd}-O{odgajateljId}-";

            return await _context.DnevniQRCodovi
                .Where(q => q.Aktivan
                    && q.DatumVazenja.Date == danas
                    && q.VrijemeIsteka >= sada
                    && q.VrijednostKoda.StartsWith(prefiks))
                .OrderByDescending(q => q.VrijemeIsteka)
                .FirstOrDefaultAsync();
        }

        private bool EvidencijaDolaskaOdlaskaExists(int id)
        {
            return _context.EvidencijeDolaskaOdlaska.Any(e => e.Id == id);
        }
    }

    public class RoditeljEvidencijaDolaskaOdlaskaViewModel
    {
        public string UneseniKodDjeteta { get; set; } = string.Empty;
        public string TipDogadjaja { get; set; } = "DOLAZAK";
        public DnevniQRCode? DnevniQRCode { get; set; }
        public string? UspjesnaPoruka { get; set; }

        // Vrijednost (token) skeniranog dnevnog QR koda
        public string? ScaniraniKod { get; set; }

        // Odabrano dijete iz ponuđene liste djece prijavljenog roditelja
        public int? OdabranoDijeteId { get; set; }

        // Djeca prijavljenog roditelja (za padajući izbornik)
        public List<Dijete> Djeca { get; set; } = new();
    }

    public class OdgajateljEvidencijaPregledViewModel
    {
        public DateTime Datum { get; set; } = DateTime.Today;

        public List<Grupa> Grupe { get; set; } = new();
        public List<Dijete> Djeca { get; set; } = new();
        public List<EvidencijaDolaskaOdlaska> Evidencije { get; set; } = new();

        public int? OdabranaGrupaId { get; set; }
        public int? OdabranoDijeteId { get; set; }

        public int BrojDolazaka { get; set; }
        public int BrojOdlazaka { get; set; }
        public int BrojOdbijenih { get; set; }

        // Trenutno aktivni dnevni QR kod (za prikaz/generisanje)
        public DnevniQRCode? AktivniQRKod { get; set; }

        // Odabrani tip koji se kodira u QR ("DOLAZAK" ili "ODLAZAK")
        public string OdabraniTip { get; set; } = "DOLAZAK";
    }
}
