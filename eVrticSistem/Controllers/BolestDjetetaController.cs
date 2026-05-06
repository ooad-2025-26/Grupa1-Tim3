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
    public class BolestDjetetaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BolestDjetetaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BolestDjeteta
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.BolestiDjece.Include(b => b.Dijete);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: BolestDjeteta/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bolestDjeteta = await _context.BolestiDjece
                .Include(b => b.Dijete)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bolestDjeteta == null)
            {
                return NotFound();
            }

            return View(bolestDjeteta);
        }

        // GET: BolestDjeteta/Create
        public IActionResult Create()
        {
            ViewData["DijeteId"] = new SelectList(_context.Djeca, "Id", "IdentifikacioniKod");
            return View();
        }

        // POST: BolestDjeteta/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Naziv,DijeteId")] BolestDjeteta bolestDjeteta)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bolestDjeteta);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DijeteId"] = new SelectList(_context.Djeca, "Id", "IdentifikacioniKod", bolestDjeteta.DijeteId);
            return View(bolestDjeteta);
        }

        // GET: BolestDjeteta/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bolestDjeteta = await _context.BolestiDjece.FindAsync(id);
            if (bolestDjeteta == null)
            {
                return NotFound();
            }
            ViewData["DijeteId"] = new SelectList(_context.Djeca, "Id", "IdentifikacioniKod", bolestDjeteta.DijeteId);
            return View(bolestDjeteta);
        }

        // POST: BolestDjeteta/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Naziv,DijeteId")] BolestDjeteta bolestDjeteta)
        {
            if (id != bolestDjeteta.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bolestDjeteta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BolestDjetetaExists(bolestDjeteta.Id))
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
            ViewData["DijeteId"] = new SelectList(_context.Djeca, "Id", "IdentifikacioniKod", bolestDjeteta.DijeteId);
            return View(bolestDjeteta);
        }

        // GET: BolestDjeteta/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bolestDjeteta = await _context.BolestiDjece
                .Include(b => b.Dijete)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bolestDjeteta == null)
            {
                return NotFound();
            }

            return View(bolestDjeteta);
        }

        // POST: BolestDjeteta/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bolestDjeteta = await _context.BolestiDjece.FindAsync(id);
            if (bolestDjeteta != null)
            {
                _context.BolestiDjece.Remove(bolestDjeteta);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BolestDjetetaExists(int id)
        {
            return _context.BolestiDjece.Any(e => e.Id == id);
        }
    }
}
