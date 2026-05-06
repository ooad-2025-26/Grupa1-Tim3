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
    public class EvidencijaDolaskaOdlaskaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EvidencijaDolaskaOdlaskaController(ApplicationDbContext context)
        {
            _context = context;
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

        private bool EvidencijaDolaskaOdlaskaExists(int id)
        {
            return _context.EvidencijeDolaskaOdlaska.Any(e => e.Id == id);
        }
    }
}
