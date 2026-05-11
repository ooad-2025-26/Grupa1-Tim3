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
    public class ObavijestController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ObavijestController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Obavijest
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Obavijesti.Include(o => o.Odgajatelj).Include(o => o.Roditelj);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Obavijest/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obavijest = await _context.Obavijesti
                .Include(o => o.Odgajatelj)
                .Include(o => o.Roditelj)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (obavijest == null)
            {
                return NotFound();
            }

            return View(obavijest);
        }

        // GET: Obavijest/Create
        public IActionResult Create()
        {
            ViewData["OdgajateljId"] = new SelectList(_context.Odgajatelji, "Id", "Email");
            ViewData["RoditeljId"] = new SelectList(_context.Roditelji, "Id", "Email");
            return View();
        }

        // POST: Obavijest/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Naslov,Sadrzaj,DatumKreiranja,DatumSlanja,KanalSlanja,StatusObavijesti,RoditeljId,OdgajateljId")] Obavijest obavijest)
        {
            if (ModelState.IsValid)
            {
                _context.Add(obavijest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OdgajateljId"] = new SelectList(_context.Odgajatelji, "Id", "Email", obavijest.OdgajateljId);
            ViewData["RoditeljId"] = new SelectList(_context.Roditelji, "Id", "Email", obavijest.RoditeljId);
            return View(obavijest);
        }

        // GET: Obavijest/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obavijest = await _context.Obavijesti.FindAsync(id);
            if (obavijest == null)
            {
                return NotFound();
            }
            ViewData["OdgajateljId"] = new SelectList(_context.Odgajatelji, "Id", "Email", obavijest.OdgajateljId);
            ViewData["RoditeljId"] = new SelectList(_context.Roditelji, "Id", "Email", obavijest.RoditeljId);
            return View(obavijest);
        }

        // POST: Obavijest/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Naslov,Sadrzaj,DatumKreiranja,DatumSlanja,KanalSlanja,StatusObavijesti,RoditeljId,OdgajateljId")] Obavijest obavijest)
        {
            if (id != obavijest.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(obavijest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ObavijestExists(obavijest.Id))
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
            ViewData["OdgajateljId"] = new SelectList(_context.Odgajatelji, "Id", "Email", obavijest.OdgajateljId);
            ViewData["RoditeljId"] = new SelectList(_context.Roditelji, "Id", "Email", obavijest.RoditeljId);
            return View(obavijest);
        }

        // GET: Obavijest/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obavijest = await _context.Obavijesti
                .Include(o => o.Odgajatelj)
                .Include(o => o.Roditelj)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (obavijest == null)
            {
                return NotFound();
            }

            return View(obavijest);
        }

        // POST: Obavijest/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var obavijest = await _context.Obavijesti.FindAsync(id);
            if (obavijest != null)
            {
                _context.Obavijesti.Remove(obavijest);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ObavijestExists(int id)
        {
            return _context.Obavijesti.Any(e => e.Id == id);
        }
    }
}
