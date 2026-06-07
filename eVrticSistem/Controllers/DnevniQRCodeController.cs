using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EVrtic.Data;
using EVrtic.Models;
using Microsoft.AspNetCore.Authorization;

namespace EVrtic.Controllers
{
     
    [Authorize(Roles = "ODGAJATELJ,ADMINISTRATOR")]
    public class DnevniQRCodeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DnevniQRCodeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DnevniQRCode
        public async Task<IActionResult> Index()
        {
            var danas = DateTime.Today;
            var sada = DateTime.Now;

            var aktivniKod = await _context.DnevniQRCodovi
                .Where(q => q.Aktivan
                    && q.DatumVazenja.Date == danas
                    && q.VrijemeIsteka >= sada)
                .OrderByDescending(q => q.VrijemeIsteka)
                .FirstOrDefaultAsync();

            var historija = await _context.DnevniQRCodovi
                .OrderByDescending(q => q.DatumVazenja)
                .ThenByDescending(q => q.VrijemeIsteka)
                .Take(10)
                .ToListAsync();

            var vm = new DnevniQRCodePregledViewModel
            {
                AktivniKod = aktivniKod,
                HistorijaKodova = historija
            };

            return View(vm);
        }

        // POST: DnevniQRCode/GenerisiDanas
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerisiDanas()
        {
            var danas = DateTime.Today;

            var aktivniDanas = await _context.DnevniQRCodovi
                .Where(q => q.Aktivan && q.DatumVazenja.Date == danas)
                .ToListAsync();

            foreach (var kod in aktivniDanas)
            {
                kod.Aktivan = false;
            }

            var noviKod = new DnevniQRCode
            {
                VrijednostKoda = GenerisiVrijednostKoda(),
                DatumVazenja = danas,
                VrijemeIsteka = danas.AddDays(1).AddSeconds(-1),
                Aktivan = true
            };

            _context.DnevniQRCodovi.Add(noviKod);
            await _context.SaveChangesAsync();

            TempData["Poruka"] = "Novi dnevni QR kod je uspješno generisan.";
            return RedirectToAction(nameof(Index));
        }

        // POST: DnevniQRCode/Deaktiviraj/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deaktiviraj(int id)
        {
            var kod = await _context.DnevniQRCodovi.FindAsync(id);

            if (kod == null)
            {
                return NotFound();
            }

            kod.Aktivan = false;
            await _context.SaveChangesAsync();

            TempData["Poruka"] = "QR kod je deaktiviran.";
            return RedirectToAction(nameof(Index));
        }

        private static string GenerisiVrijednostKoda()
        {
            var token = Guid.NewGuid().ToString("N")[..10].ToUpper();
            return $"EVRTIC-{DateTime.Today:yyyyMMdd}-{token}";
        }
    }

    public class DnevniQRCodePregledViewModel
    {
        public DnevniQRCode? AktivniKod { get; set; }
        public List<DnevniQRCode> HistorijaKodova { get; set; } = new();
    }
}