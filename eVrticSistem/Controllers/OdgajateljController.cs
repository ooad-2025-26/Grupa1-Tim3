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
    public class OdgajateljController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OdgajateljController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Odgajatelj
        public async Task<IActionResult> Index()
        {
            return View(await _context.Odgajatelji.ToListAsync());
        }

        // GET: Odgajatelj/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var odgajatelj = await _context.Odgajatelji
                .FirstOrDefaultAsync(m => m.Id == id);
            if (odgajatelj == null)
            {
                return NotFound();
            }

            return View(odgajatelj);
        }

        // GET: Odgajatelj/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Odgajatelj/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ImePrezime,Email,LozinkaHash,Uloga,StatusNaloga")] Odgajatelj odgajatelj)
        {
            if (ModelState.IsValid)
            {
                _context.Add(odgajatelj);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(odgajatelj);
        }

        // GET: Odgajatelj/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var odgajatelj = await _context.Odgajatelji.FindAsync(id);
            if (odgajatelj == null)
            {
                return NotFound();
            }
            return View(odgajatelj);
        }

        // POST: Odgajatelj/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ImePrezime,Email,LozinkaHash,Uloga,StatusNaloga")] Odgajatelj odgajatelj)
        {
            if (id != odgajatelj.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(odgajatelj);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OdgajateljExists(odgajatelj.Id))
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
            return View(odgajatelj);
        }

        // GET: Odgajatelj/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var odgajatelj = await _context.Odgajatelji
                .FirstOrDefaultAsync(m => m.Id == id);
            if (odgajatelj == null)
            {
                return NotFound();
            }

            return View(odgajatelj);
        }

        // POST: Odgajatelj/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var odgajatelj = await _context.Odgajatelji.FindAsync(id);
            if (odgajatelj != null)
            {
                _context.Odgajatelji.Remove(odgajatelj);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OdgajateljExists(int id)
        {
            return _context.Odgajatelji.Any(e => e.Id == id);
        }
    }
}
