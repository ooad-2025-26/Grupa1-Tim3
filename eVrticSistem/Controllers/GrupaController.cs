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
    public class GrupaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GrupaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Grupa
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Grupe.Include(g => g.Odgajatelj);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Grupa/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var grupa = await _context.Grupe
                .Include(g => g.Odgajatelj)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (grupa == null)
            {
                return NotFound();
            }

            return View(grupa);
        }

        // GET: Grupa/Create
        public IActionResult Create()
        {
            ViewData["OdgajateljId"] = new SelectList(_context.Odgajatelji, "Id", "Email");
            return View();
        }

        // POST: Grupa/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ImeGrupe,OdgajateljId")] Grupa grupa)
        {
            if (ModelState.IsValid)
            {
                _context.Add(grupa);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OdgajateljId"] = new SelectList(_context.Odgajatelji, "Id", "Email", grupa.OdgajateljId);
            return View(grupa);
        }

        // GET: Grupa/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var grupa = await _context.Grupe.FindAsync(id);
            if (grupa == null)
            {
                return NotFound();
            }
            ViewData["OdgajateljId"] = new SelectList(_context.Odgajatelji, "Id", "Email", grupa.OdgajateljId);
            return View(grupa);
        }

        // POST: Grupa/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ImeGrupe,OdgajateljId")] Grupa grupa)
        {
            if (id != grupa.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(grupa);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GrupaExists(grupa.Id))
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
            ViewData["OdgajateljId"] = new SelectList(_context.Odgajatelji, "Id", "Email", grupa.OdgajateljId);
            return View(grupa);
        }

        // GET: Grupa/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var grupa = await _context.Grupe
                .Include(g => g.Odgajatelj)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (grupa == null)
            {
                return NotFound();
            }

            return View(grupa);
        }

        // POST: Grupa/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var grupa = await _context.Grupe.FindAsync(id);
            if (grupa != null)
            {
                _context.Grupe.Remove(grupa);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GrupaExists(int id)
        {
            return _context.Grupe.Any(e => e.Id == id);
        }
    }
}
