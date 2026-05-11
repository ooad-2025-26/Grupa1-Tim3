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
    public class AlergijaDjetetaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AlergijaDjetetaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AlergijaDjeteta
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.AlergijeDjece.Include(a => a.Dijete);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: AlergijaDjeteta/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alergijaDjeteta = await _context.AlergijeDjece
                .Include(a => a.Dijete)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (alergijaDjeteta == null)
            {
                return NotFound();
            }

            return View(alergijaDjeteta);
        }

        // GET: AlergijaDjeteta/Create
        public IActionResult Create()
        {
            ViewData["DijeteId"] = new SelectList(_context.Djeca, "Id", "IdentifikacioniKod");
            return View();
        }

        // POST: AlergijaDjeteta/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Naziv,DijeteId")] AlergijaDjeteta alergijaDjeteta)
        {
            if (ModelState.IsValid)
            {
                _context.Add(alergijaDjeteta);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DijeteId"] = new SelectList(_context.Djeca, "Id", "IdentifikacioniKod", alergijaDjeteta.DijeteId);
            return View(alergijaDjeteta);
        }

        // GET: AlergijaDjeteta/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alergijaDjeteta = await _context.AlergijeDjece.FindAsync(id);
            if (alergijaDjeteta == null)
            {
                return NotFound();
            }
            ViewData["DijeteId"] = new SelectList(_context.Djeca, "Id", "IdentifikacioniKod", alergijaDjeteta.DijeteId);
            return View(alergijaDjeteta);
        }

        // POST: AlergijaDjeteta/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Naziv,DijeteId")] AlergijaDjeteta alergijaDjeteta)
        {
            if (id != alergijaDjeteta.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(alergijaDjeteta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlergijaDjetetaExists(alergijaDjeteta.Id))
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
            ViewData["DijeteId"] = new SelectList(_context.Djeca, "Id", "IdentifikacioniKod", alergijaDjeteta.DijeteId);
            return View(alergijaDjeteta);
        }

        // GET: AlergijaDjeteta/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alergijaDjeteta = await _context.AlergijeDjece
                .Include(a => a.Dijete)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (alergijaDjeteta == null)
            {
                return NotFound();
            }

            return View(alergijaDjeteta);
        }

        // POST: AlergijaDjeteta/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var alergijaDjeteta = await _context.AlergijeDjece.FindAsync(id);
            if (alergijaDjeteta != null)
            {
                _context.AlergijeDjece.Remove(alergijaDjeteta);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AlergijaDjetetaExists(int id)
        {
            return _context.AlergijeDjece.Any(e => e.Id == id);
        }
    }
}
