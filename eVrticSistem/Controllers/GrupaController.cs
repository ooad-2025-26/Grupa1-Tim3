using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EVrtic.Data;
using EVrtic.Models;
using Microsoft.AspNetCore.Identity;

namespace EVrtic.Controllers
{
    public class GrupaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Korisnik> _userManager;

        public GrupaController(ApplicationDbContext context, UserManager<Korisnik> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Grupa  — prikaz svih grupa sa odgajateljem i djecom (za expandable kartice)
        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> Index()
        {
            var grupe = await _context.Grupe
                .Include(g => g.Odgajatelj)
                .Include(g => g.Djeca)
                .OrderBy(g => g.ImeGrupe)
                .ToListAsync();

            return View(grupe);
        }

        // ─── DODAVANJE NOVE GRUPE ────────────────────────────────────────────

        // GET: Grupa/Create
        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> Create()
        {
            var vm = new GrupaFormViewModel
            {
                Odgajatelji = await OdgajateljiSelectList(),
                DostupnaDjeca = await DostupnaDjecaZaCreate()
            };
            return View(vm);
        }

        // POST: Grupa/Create
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> Create(GrupaFormViewModel vm)
        {
            vm.OdabranaDjecaIds ??= new List<int>();

            if (string.IsNullOrWhiteSpace(vm.ImeGrupe))
                ModelState.AddModelError(nameof(vm.ImeGrupe), "Ime grupe je obavezno.");

            if (vm.OdgajateljId == null)
                ModelState.AddModelError(nameof(vm.OdgajateljId), "Morate dodijeliti odgajatelja grupi.");

            // Poslovno pravilo: nova grupa mora imati najmanje 2 djeteta
            if (vm.OdabranaDjecaIds.Count < 2)
                ModelState.AddModelError(nameof(vm.OdabranaDjecaIds), "U grupu morate dodati najmanje 2 djeteta.");

            if (!ModelState.IsValid)
            {
                vm.Odgajatelji = await OdgajateljiSelectList(vm.OdgajateljId);
                vm.DostupnaDjeca = await DostupnaDjecaZaCreate();
                return View(vm);
            }

            var grupa = new Grupa
            {
                ImeGrupe = vm.ImeGrupe.Trim(),
                OdgajateljId = vm.OdgajateljId
            };
            _context.Grupe.Add(grupa);
            await _context.SaveChangesAsync();

            // Dodjela djece grupi
            var djeca = await _context.Djeca
                .Where(d => vm.OdabranaDjecaIds.Contains(d.Id))
                .ToListAsync();
            foreach (var d in djeca)
                d.GrupaId = grupa.Id;

            await _context.SaveChangesAsync();

            TempData["Uspjeh"] = $"Grupa \"{grupa.ImeGrupe}\" je uspješno kreirana.";
            return RedirectToAction(nameof(Index));
        }

        // ─── UREĐIVANJE GRUPE ────────────────────────────────────────────────

        // GET: Grupa/Edit/5
        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var grupa = await _context.Grupe
                .Include(g => g.Djeca)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (grupa == null) return NotFound();

            var vm = new GrupaFormViewModel
            {
                Id = grupa.Id,
                ImeGrupe = grupa.ImeGrupe,
                OdgajateljId = grupa.OdgajateljId,
                OdabranaDjecaIds = grupa.Djeca.Select(d => d.Id).ToList(),
                Odgajatelji = await OdgajateljiSelectList(grupa.OdgajateljId),
                DostupnaDjeca = await DostupnaDjecaZaEdit(grupa.Id)
            };

            return View(vm);
        }

        // POST: Grupa/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> Edit(int id, GrupaFormViewModel vm)
        {
            if (id != vm.Id) return NotFound();
            vm.OdabranaDjecaIds ??= new List<int>();

            if (string.IsNullOrWhiteSpace(vm.ImeGrupe))
                ModelState.AddModelError(nameof(vm.ImeGrupe), "Ime grupe je obavezno.");

            if (vm.OdgajateljId == null)
                ModelState.AddModelError(nameof(vm.OdgajateljId), "Morate dodijeliti odgajatelja grupi.");

            if (!ModelState.IsValid)
            {
                vm.Odgajatelji = await OdgajateljiSelectList(vm.OdgajateljId);
                vm.DostupnaDjeca = await DostupnaDjecaZaEdit(id);
                return View(vm);
            }

            var grupa = await _context.Grupe
                .Include(g => g.Djeca)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (grupa == null) return NotFound();

            grupa.ImeGrupe = vm.ImeGrupe.Trim();
            grupa.OdgajateljId = vm.OdgajateljId;

            // Djeca koja su trenutno u grupi a nisu označena — uklanjaju se iz grupe
            var trenutna = grupa.Djeca.ToList();
            foreach (var d in trenutna)
                if (!vm.OdabranaDjecaIds.Contains(d.Id))
                    d.GrupaId = null;

            // Novooznačena djeca — dodaju se u grupu
            var zaDodati = await _context.Djeca
                .Where(d => vm.OdabranaDjecaIds.Contains(d.Id))
                .ToListAsync();
            foreach (var d in zaDodati)
                d.GrupaId = grupa.Id;

            await _context.SaveChangesAsync();

            TempData["Uspjeh"] = $"Grupa \"{grupa.ImeGrupe}\" je uspješno ažurirana.";
            return RedirectToAction(nameof(Index));
        }

        // ─── BRISANJE GRUPE (zadržano iz scaffolda) ──────────────────────────

        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var grupa = await _context.Grupe
                .Include(g => g.Odgajatelj)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (grupa == null) return NotFound();
            return View(grupa);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var grupa = await _context.Grupe.Include(g => g.Djeca).FirstOrDefaultAsync(g => g.Id == id);
            if (grupa != null)
            {
                // Oslobodi djecu iz grupe prije brisanja
                foreach (var d in grupa.Djeca)
                    d.GrupaId = null;
                _context.Grupe.Remove(grupa);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // ─── ODGAJATELJ: Moja grupa ──────────────────────────────────────────
        // (scenarij — odgajatelj pregleda djecu iz svoje grupe)

        [Authorize(Roles = "ODGAJATELJ")]
        public async Task<IActionResult> MojaGrupa()
        {
            var korisnik = await _userManager.GetUserAsync(User);
            if (korisnik == null) return RedirectToPage("/Account/Login", new { area = "Identity" });

            var odgajatelj = await _context.Odgajatelji
                .FirstOrDefaultAsync(o => o.Id == korisnik.Id);

            if (odgajatelj == null) return RedirectToAction("OdgajateljHome", "Home");

            var grupe = await _context.Grupe
                .Include(g => g.Djeca)
                    .ThenInclude(d => d.Alergije)
                .Include(g => g.Djeca)
                    .ThenInclude(d => d.Bolesti)
                .Where(g => g.OdgajateljId == odgajatelj.Id)
                .OrderBy(g => g.ImeGrupe)
                .ToListAsync();

            ViewBag.Odgajatelj = odgajatelj;
            return View(grupe);
        }

        // ─── POMOĆNE METODE ──────────────────────────────────────────────────

        private async Task<List<SelectListItem>> OdgajateljiSelectList(int? selected = null)
        {
            var odgajatelji = await _context.Odgajatelji
                .OrderBy(o => o.ImePrezime)
                .ToListAsync();

            return odgajatelji.Select(o => new SelectListItem
            {
                Value = o.Id.ToString(),
                Text = string.IsNullOrWhiteSpace(o.ImePrezime) ? (o.Email ?? $"Odgajatelj #{o.Id}") : o.ImePrezime,
                Selected = (selected != null && o.Id == selected)
            }).ToList();
        }

        // Za novu grupu — dostupna su samo djeca koja još nisu ni u jednoj grupi
        private async Task<List<DjeteOpcija>> DostupnaDjecaZaCreate()
        {
            var djeca = await _context.Djeca
                .Include(d => d.Roditelj)
                .Where(d => d.GrupaId == null)
                .OrderBy(d => d.ImePrezime)
                .ToListAsync();

            return djeca.Select(MapDjete).ToList();
        }

        // Za uređivanje — dostupna su djeca bez grupe ILI djeca koja već pripadaju ovoj grupi
        private async Task<List<DjeteOpcija>> DostupnaDjecaZaEdit(int grupaId)
        {
            var djeca = await _context.Djeca
                .Include(d => d.Roditelj)
                .Where(d => d.GrupaId == null || d.GrupaId == grupaId)
                .OrderBy(d => d.ImePrezime)
                .ToListAsync();

            return djeca.Select(MapDjete).ToList();
        }

        private static DjeteOpcija MapDjete(Dijete d) => new DjeteOpcija
        {
            Id = d.Id,
            ImePrezime = string.IsNullOrWhiteSpace(d.ImePrezime) ? $"(kod: {d.IdentifikacioniKod})" : d.ImePrezime,
            Roditelj = d.Roditelj != null ? d.Roditelj.ImePrezime : "Bez roditelja"
        };
    }

    // ─── ViewModeli za grupu ─────────────────────────────────────────────────

    public class GrupaFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ime grupe je obavezno.")]
        [StringLength(100)]
        [Display(Name = "Ime grupe")]
        public string ImeGrupe { get; set; } = string.Empty;

        [Display(Name = "Odgajatelj")]
        public int? OdgajateljId { get; set; }

        public List<int> OdabranaDjecaIds { get; set; } = new();

        // Pomoćni podaci za prikaz forme
        public List<SelectListItem> Odgajatelji { get; set; } = new();
        public List<DjeteOpcija> DostupnaDjeca { get; set; } = new();
    }

    public class DjeteOpcija
    {
        public int Id { get; set; }
        public string ImePrezime { get; set; } = string.Empty;
        public string Roditelj { get; set; } = string.Empty;
    }
}
