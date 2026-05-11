using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EVrtic.Data;
using EVrtic.Models;
using Microsoft.AspNetCore.Authorization;

namespace eVrticSistem.Controllers
{
    [Authorize(Roles = "ADMINISTRATOR")]
    public class RoditeljController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoditeljController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Roditelj
        public async Task<IActionResult> Index()
        {
            return View(await _context.Roditelji.ToListAsync());
        }

        // GET: Roditelj/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roditelj = await _context.Roditelji
                .FirstOrDefaultAsync(m => m.Id == id);
            if (roditelj == null)
            {
                return NotFound();
            }

            return View(roditelj);
        }

        // GET: Roditelj/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Roditelj/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ImePrezime,Email,Uloga,StatusNaloga")] Roditelj roditelj)
        {
            if (ModelState.IsValid)
            {
                _context.Add(roditelj);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(roditelj);
        }

        // GET: Roditelj/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roditelj = await _context.Roditelji.FindAsync(id);
            if (roditelj == null)
            {
                return NotFound();
            }
            return View(roditelj);
        }

        // POST: Roditelj/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ImePrezime,Email,Uloga,StatusNaloga")] Roditelj roditelj)
        {
            if (id != roditelj.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(roditelj);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoditeljExists(roditelj.Id))
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
            return View(roditelj);
        }

        // GET: Roditelj/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roditelj = await _context.Roditelji
                .FirstOrDefaultAsync(m => m.Id == id);
            if (roditelj == null)
            {
                return NotFound();
            }

            return View(roditelj);
        }

        // POST: Roditelj/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var roditelj = await _context.Roditelji.FindAsync(id);
            if (roditelj != null)
            {
                _context.Roditelji.Remove(roditelj);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoditeljExists(int id)
        {
            return _context.Roditelji.Any(e => e.Id == id);
        }
    }
}
