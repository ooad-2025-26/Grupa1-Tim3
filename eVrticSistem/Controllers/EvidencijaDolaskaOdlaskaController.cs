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
            var qrKod = await VratiAktivniQrKod();

            return View(new RoditeljEvidencijaDolaskaOdlaskaViewModel
            {
                DnevniQRCode = qrKod,
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

            var qrKod = await VratiAktivniQrKod();
            model.DnevniQRCode = qrKod;

            if (qrKod == null)
            {
                ModelState.AddModelError(string.Empty, "Trenutno ne postoji aktivan dnevni QR kod.");
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(model.UneseniKodDjeteta))
            {
                ModelState.AddModelError(nameof(model.UneseniKodDjeteta), "Kod djeteta je obavezan.");
                return View(model);
            }

            var uneseniKod = model.UneseniKodDjeteta.Trim();

            var dijete = await _context.Djeca
                .FirstOrDefaultAsync(d =>
                    d.RoditeljId == korisnik.Id &&
                    d.IdentifikacioniKod == uneseniKod &&
                    d.Aktivno);

            if (dijete == null)
            {
                ModelState.AddModelError(nameof(model.UneseniKodDjeteta), "Kod djeteta nije ispravan ili dijete nije povezano s vašim profilom.");
                return View(model);
            }

            var evidencija = new EvidencijaDolaskaOdlaska
            {
                VrijemeDogadjaja = DateTime.Now,
                TipDogadjaja = ParsirajTipDogadjaja(model.TipDogadjaja),
                UneseniKodDjeteta = uneseniKod,
                ValidanQRKod = true,
                KodDjetetaIspravan = true,
                StatusEvidencije = OdrediStatusEvidencije(true),
                DijeteId = dijete.Id,
                DnevniQRCodeId = qrKod.Id
            };

            _context.EvidencijeDolaskaOdlaska.Add(evidencija);
            await _context.SaveChangesAsync();

            ModelState.Clear();

            return View(new RoditeljEvidencijaDolaskaOdlaskaViewModel
            {
                DnevniQRCode = qrKod,
                TipDogadjaja = model.TipDogadjaja,
                UspjesnaPoruka = $"{model.TipDogadjaja} je uspješno evidentiran za dijete {dijete.ImePrezime}."
            });
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

        private static StatusEvidencije OdrediStatusEvidencije(bool validno)
        {
            if (validno)
            {
                if (Enum.TryParse<StatusEvidencije>("VALIDNA", true, out var validna))
                    return validna;

                if (Enum.TryParse<StatusEvidencije>("USPJESNA", true, out var uspjesna))
                    return uspjesna;
            }
            else
            {
                if (Enum.TryParse<StatusEvidencije>("NEVALIDNA", true, out var nevalidna))
                    return nevalidna;
            }

            return Enum.GetValues(typeof(StatusEvidencije))
                .Cast<StatusEvidencije>()
                .First();
        }

        // ═══════════════════════════════════════════════════════════════════
        // ODGAJATELJ — Pregled dolazaka i odlazaka djece iz njegovih grupa
        // ═══════════════════════════════════════════════════════════════════

        [Authorize(Roles = "ODGAJATELJ")]
        public async Task<IActionResult> OdgajateljPregled(DateTime? datum = null, int? grupaId = null, int? dijeteId = null)
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

            var djeca = await djecaQuery
                .OrderBy(d => d.ImePrezime)
                .ToListAsync();

            if (dijeteId.HasValue)
            {
                djeca = djeca
                    .Where(d => d.Id == dijeteId.Value)
                    .ToList();
            }

            var idDjece = djeca.Select(d => d.Id).ToList();

            var evidencije = await _context.EvidencijeDolaskaOdlaska
                .Include(e => e.Dijete)
                    .ThenInclude(d => d.Grupa)
                .Include(e => e.DnevniQRCode)
                .Where(e => idDjece.Contains(e.DijeteId)
                    && e.VrijemeDogadjaja.Date == odabraniDatum)
                .OrderByDescending(e => e.VrijemeDogadjaja)
                .ToListAsync();

            var vm = new OdgajateljEvidencijaPregledViewModel
            {
                Datum = odabraniDatum,
                Grupe = grupe,
                Djeca = djeca,
                Evidencije = evidencije,
                OdabranaGrupaId = grupaId,
                OdabranoDijeteId = dijeteId,
                BrojDolazaka = evidencije.Count(e => e.TipDogadjaja == TipDogadjaja.DOLAZAK),
                BrojOdlazaka = evidencije.Count(e => e.TipDogadjaja == TipDogadjaja.ODLAZAK),
                BrojOdbijenih = evidencije.Count(e => e.StatusEvidencije == StatusEvidencije.ODBIJENO)
            };

            return View(vm);
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
    }
}
