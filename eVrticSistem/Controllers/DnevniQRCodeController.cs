using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EVrtic.Data;
using EVrtic.Models;

namespace eVrticSistem.Controllers
{
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
            return View(await _context.DnevniQRCodovi.ToListAsync());
        }

        // GET: DnevniQRCode/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dnevniQRCode = await _context.DnevniQRCodovi
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dnevniQRCode == null)
            {
                return NotFound();
            }

            return View(dnevniQRCode);
        }

        // GET: DnevniQRCode/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DnevniQRCode/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,VrijednostKoda,DatumVazenja,VrijemeIsteka,Aktivan")] DnevniQRCode dnevniQRCode)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dnevniQRCode);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dnevniQRCode);
        }

        // GET: DnevniQRCode/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dnevniQRCode = await _context.DnevniQRCodovi.FindAsync(id);
            if (dnevniQRCode == null)
            {
                return NotFound();
            }
            return View(dnevniQRCode);
        }

        // POST: DnevniQRCode/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,VrijednostKoda,DatumVazenja,VrijemeIsteka,Aktivan")] DnevniQRCode dnevniQRCode)
        {
            if (id != dnevniQRCode.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dnevniQRCode);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DnevniQRCodeExists(dnevniQRCode.Id))
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
            return View(dnevniQRCode);
        }

        // GET: DnevniQRCode/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dnevniQRCode = await _context.DnevniQRCodovi
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dnevniQRCode == null)
            {
                return NotFound();
            }

            return View(dnevniQRCode);
        }

        // POST: DnevniQRCode/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dnevniQRCode = await _context.DnevniQRCodovi.FindAsync(id);
            if (dnevniQRCode != null)
            {
                _context.DnevniQRCodovi.Remove(dnevniQRCode);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DnevniQRCodeExists(int id)
        {
            return _context.DnevniQRCodovi.Any(e => e.Id == id);
        }
    }
}
