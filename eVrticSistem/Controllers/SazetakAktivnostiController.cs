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
    public class SazetakAktivnostiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SazetakAktivnostiController(ApplicationDbContext context)
        {
            _context = context;
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
                    else
                    {
                        throw;
                    }
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

        private bool SazetakAktivnostiExists(int id)
        {
            return _context.SazeciAktivnosti.Any(e => e.Id == id);
        }
    }
}
