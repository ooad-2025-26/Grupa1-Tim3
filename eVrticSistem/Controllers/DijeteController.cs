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

        // GET: Dijete (admin)
        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Djeca.Include(d => d.Grupa).Include(d => d.Roditelj);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Dijete/Details/5 (admin)
        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var dijete = await _context.Djeca
                .Include(d => d.Grupa).Include(d => d.Roditelj)
                .Include(d => d.Alergije).Include(d => d.Bolesti)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dijete == null) return NotFound();
            return View(dijete);
        }

        // ─── ADMINISTRATOR: Unos identifikacionog koda ───────────────────────

        [Authorize(Roles = "ADMINISTRATOR")]
        public IActionResult DodajIdentifikacioniKod() => View();

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> DodajIdentifikacioniKod([Bind("IdentifikacioniKod")] Dijete dijete)
        {
            ModelState.Remove("ImePrezime");
            ModelState.Remove("DodatnaNapomena");
            ModelState.Remove("DatumRodjenja");

            if (string.IsNullOrWhiteSpace(dijete.IdentifikacioniKod))
                ModelState.AddModelError("IdentifikacioniKod", "Identifikacioni kod je obavezan.");
            else if (await _context.Djeca.AnyAsync(d => d.IdentifikacioniKod == dijete.IdentifikacioniKod))
                ModelState.AddModelError("IdentifikacioniKod", "Dijete sa ovim identifikacionim kodom već postoji.");

            if (!ModelState.IsValid) return View(dijete);

            _context.Add(new Dijete
            {
                IdentifikacioniKod = dijete.IdentifikacioniKod.Trim(),
                ImePrezime = string.Empty,
                Aktivno = true
            });
            await _context.SaveChangesAsync();

            TempData["Uspjeh"] = $"Identifikacioni kod \"{dijete.IdentifikacioniKod.Trim()}\" je uspješno dodan.";
            return RedirectToAction(nameof(DodajIdentifikacioniKod));
        }

        // ─── RODITELJ: Unos podataka o djetetu ──────────────────────────────

        [Authorize(Roles = "RODITELJ")]
        public async Task<IActionResult> UnosPodataka(int? id, bool novaRegistracija = false)
        {
            var korisnik = await _userManager.GetUserAsync(User);
            if (korisnik == null) return Challenge();

            Dijete? dijete;

            if (id.HasValue)
            {
                dijete = await _context.Djeca.Include(d => d.Alergije).Include(d => d.Bolesti)
                    .FirstOrDefaultAsync(d => d.Id == id && d.RoditeljId == korisnik.Id);
                if (dijete == null) return Forbid();
            }
            else
            {
                dijete = await _context.Djeca.Include(d => d.Alergije).Include(d => d.Bolesti)
                    .FirstOrDefaultAsync(d => d.RoditeljId == korisnik.Id && d.ImePrezime == string.Empty);
                if (dijete == null)
                    return RedirectToAction("RoditeljHome", "Home");
            }

            ViewData["NovaRegistracija"] = novaRegistracija;

            var vm = new UnosPodatakaViewModel
            {
                DijeteId = dijete.Id,
                ImePrezime = dijete.ImePrezime,
                DatumRodjenja = dijete.DatumRodjenja == default ? DateTime.Today : dijete.DatumRodjenja,
                DodatnaNapomena = dijete.DodatnaNapomena,
                Alergije = dijete.Alergije.Select(a => a.Naziv).ToList(),
                Bolesti = dijete.Bolesti.Select(b => b.Naziv).ToList()
            };

            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "RODITELJ")]
        public async Task<IActionResult> UnosPodataka(UnosPodatakaViewModel vm, bool novaRegistracija = false)
        {
            var korisnik = await _userManager.GetUserAsync(User);
            if (korisnik == null) return Challenge();

            var dijete = await _context.Djeca.Include(d => d.Alergije).Include(d => d.Bolesti)
                .FirstOrDefaultAsync(d => d.Id == vm.DijeteId && d.RoditeljId == korisnik.Id);
            if (dijete == null) return Forbid();

            if (vm.Alergije != null)
            {
                var al = vm.Alergije.Where(a => !string.IsNullOrWhiteSpace(a)).Select(a => a.Trim().ToLower()).ToList();
                if (al.Count != al.Distinct().Count())
                    ModelState.AddModelError("Alergije", "Lista alergija sadrži duplikate.");
            }
            if (vm.Bolesti != null)
            {
                var bo = vm.Bolesti.Where(b => !string.IsNullOrWhiteSpace(b)).Select(b => b.Trim().ToLower()).ToList();
                if (bo.Count != bo.Distinct().Count())
                    ModelState.AddModelError("Bolesti", "Lista bolesti sadrži duplikate.");
            }

            if (!ModelState.IsValid)
            {
                ViewData["NovaRegistracija"] = novaRegistracija;
                return View(vm);
            }

            dijete.ImePrezime = vm.ImePrezime.Trim();
            dijete.DatumRodjenja = vm.DatumRodjenja;
            dijete.DodatnaNapomena = vm.DodatnaNapomena?.Trim() ?? string.Empty;

            _context.AlergijeDjece.RemoveRange(dijete.Alergije);
            if (vm.Alergije != null)
                foreach (var naziv in vm.Alergije.Where(a => !string.IsNullOrWhiteSpace(a)))
                    _context.AlergijeDjece.Add(new AlergijaDjeteta { DijeteId = dijete.Id, Naziv = naziv.Trim() });

            _context.BolestiDjece.RemoveRange(dijete.Bolesti);
            if (vm.Bolesti != null)
                foreach (var naziv in vm.Bolesti.Where(b => !string.IsNullOrWhiteSpace(b)))
                    _context.BolestiDjece.Add(new BolestDjeteta { DijeteId = dijete.Id, Naziv = naziv.Trim() });

            await _context.SaveChangesAsync();
            return RedirectToAction("RoditeljHome", "Home");
        }

        // ─── RODITELJ: Dodaj novo dijete sa kodom ───────────────────────────

        [Authorize(Roles = "RODITELJ")]
        public IActionResult DodajDijete() => View();

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "RODITELJ")]
        public async Task<IActionResult> DodajDijete(string identifikacioniKod)
        {
            var korisnik = await _userManager.GetUserAsync(User);
            if (korisnik == null) return Challenge();

            if (string.IsNullOrWhiteSpace(identifikacioniKod))
            {
                ModelState.AddModelError("identifikacioniKod", "Identifikacioni kod je obavezan.");
                return View();
            }

            var dijete = await _context.Djeca.FirstOrDefaultAsync(d => d.IdentifikacioniKod == identifikacioniKod.Trim());

            if (dijete == null)
            {
                ModelState.AddModelError("identifikacioniKod", "Dijete sa ovim kodom nije pronađeno.");
                return View();
            }

            if (dijete.RoditeljId != null)
            {
                ModelState.AddModelError("identifikacioniKod", "Dijete je već povezano sa roditeljem.");
                return View();
            }

            dijete.RoditeljId = korisnik.Id;
            await _context.SaveChangesAsync();

            return RedirectToAction("UnosPodataka", new { id = dijete.Id });
        }

        // ─── RODITELJ: Pregled jednog djeteta ───────────────────────────────

        [Authorize(Roles = "RODITELJ")]
        public async Task<IActionResult> MojeDijete(int id)
        {
            var korisnik = await _userManager.GetUserAsync(User);
            if (korisnik == null) return Challenge();

            var dijete = await _context.Djeca
                .Include(d => d.Alergije).Include(d => d.Bolesti).Include(d => d.Grupa)
                .FirstOrDefaultAsync(d => d.Id == id && d.RoditeljId == korisnik.Id);

            if (dijete == null) return Forbid();
            return View(dijete);
        }

        // ─── CRUD (admin) ────────────────────────────────────────────────────

        [Authorize(Roles = "ADMINISTRATOR")]
        public IActionResult Create()
        {
            ViewData["GrupaId"] = new SelectList(_context.Grupe, "Id", "ImeGrupe");
            ViewData["RoditeljId"] = new SelectList(_context.Roditelji, "Id", "Email");
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> Create([Bind("Id,ImePrezime,IdentifikacioniKod,DatumRodjenja,DodatnaNapomena,Aktivno,GrupaId,RoditeljId")] Dijete dijete)
        {
            if (ModelState.IsValid) { _context.Add(dijete); await _context.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
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

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ImePrezime,IdentifikacioniKod,DatumRodjenja,DodatnaNapomena,Aktivno,GrupaId,RoditeljId")] Dijete dijete)
        {
            if (id != dijete.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try { _context.Update(dijete); await _context.SaveChangesAsync(); }
                catch (DbUpdateConcurrencyException) { if (!DijeteExists(dijete.Id)) return NotFound(); else throw; }
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
            var dijete = await _context.Djeca.Include(d => d.Grupa).Include(d => d.Roditelj).FirstOrDefaultAsync(m => m.Id == id);
            if (dijete == null) return NotFound();
            return View(dijete);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        [Authorize(Roles = "ADMINISTRATOR")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dijete = await _context.Djeca.FindAsync(id);
            if (dijete != null) _context.Djeca.Remove(dijete);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DijeteExists(int id) => _context.Djeca.Any(e => e.Id == id);
    }

    public class UnosPodatakaViewModel
    {
        public int DijeteId { get; set; }

        [Required(ErrorMessage = "Ime i prezime djeteta je obavezno.")]
        [StringLength(100)]
        [Display(Name = "Ime i prezime djeteta")]
        public string ImePrezime { get; set; } = string.Empty;

        [Required(ErrorMessage = "Datum rođenja je obavezan.")]
        [DataType(DataType.Date)]
        [Display(Name = "Datum rođenja")]
        public DateTime DatumRodjenja { get; set; }

        [StringLength(500)]
        [Display(Name = "Dodatna napomena")]
        public string? DodatnaNapomena { get; set; }

        public List<string> Alergije { get; set; } = new();
        public List<string> Bolesti { get; set; } = new();
    }
}