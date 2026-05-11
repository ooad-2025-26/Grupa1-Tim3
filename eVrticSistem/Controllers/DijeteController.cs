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
    public class DijeteController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DijeteController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Dijete
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Djeca.Include(d => d.Grupa).Include(d => d.Roditelj);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Dijete/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dijete = await _context.Djeca
                .Include(d => d.Grupa)
                .Include(d => d.Roditelj)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dijete == null)
            {
                return NotFound();
            }

            return View(dijete);
        }

        // GET: Dijete/Create
        public IActionResult Create()
        {
            ViewData["GrupaId"] = new SelectList(_context.Grupe, "Id", "ImeGrupe");
            ViewData["RoditeljId"] = new SelectList(_context.Roditelji, "Id", "Email");
            return View();
        }

        // POST: Dijete/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ImePrezime,IdentifikacioniKod,DodatnaNapomena,Aktivno,GrupaId,RoditeljId")] Dijete dijete)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dijete);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GrupaId"] = new SelectList(_context.Grupe, "Id", "ImeGrupe", dijete.GrupaId);
            ViewData["RoditeljId"] = new SelectList(_context.Roditelji, "Id", "Email", dijete.RoditeljId);
            return View(dijete);
        }

        // GET: Dijete/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dijete = await _context.Djeca.FindAsync(id);
            if (dijete == null)
            {
                return NotFound();
            }
            ViewData["GrupaId"] = new SelectList(_context.Grupe, "Id", "ImeGrupe", dijete.GrupaId);
            ViewData["RoditeljId"] = new SelectList(_context.Roditelji, "Id", "Email", dijete.RoditeljId);
            return View(dijete);
        }

        // POST: Dijete/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ImePrezime,IdentifikacioniKod,DodatnaNapomena,Aktivno,GrupaId,RoditeljId")] Dijete dijete)
        {
            if (id != dijete.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dijete);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DijeteExists(dijete.Id))
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
            ViewData["GrupaId"] = new SelectList(_context.Grupe, "Id", "ImeGrupe", dijete.GrupaId);
            ViewData["RoditeljId"] = new SelectList(_context.Roditelji, "Id", "Email", dijete.RoditeljId);
            return View(dijete);
        }

        // GET: Dijete/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dijete = await _context.Djeca
                .Include(d => d.Grupa)
                .Include(d => d.Roditelj)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dijete == null)
            {
                return NotFound();
            }

            return View(dijete);
        }

        // POST: Dijete/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dijete = await _context.Djeca.FindAsync(id);
            if (dijete != null)
            {
                _context.Djeca.Remove(dijete);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DijeteExists(int id)
        {
            return _context.Djeca.Any(e => e.Id == id);
        }
    }
}
