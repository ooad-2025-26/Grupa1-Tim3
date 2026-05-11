using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EVrtic.Data;
using EVrtic.Models;

namespace EVrtic.Controllers
{
    public class DnevniIzvjestajController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DnevniIzvjestajController(ApplicationDbContext context)
        {
            _context = context;
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
        public async Task<IActionResult> Create([Bind("Id,Datum,Obrok,StatusObroka,SpavanjeMinuta,VrijemeDolaska,VrijemeOdlaska,NapomenaAktivnosti,DatumKreiranja,DijeteId")] DnevniIzvjestaj dnevniIzvjestaj)
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Datum,Obrok,StatusObroka,SpavanjeMinuta,VrijemeDolaska,VrijemeOdlaska,NapomenaAktivnosti,DatumKreiranja,DijeteId")] DnevniIzvjestaj dnevniIzvjestaj)
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

        private bool DnevniIzvjestajExists(int id)
        {
            return _context.DnevniIzvjestaji.Any(e => e.Id == id);
        }
    }
}
