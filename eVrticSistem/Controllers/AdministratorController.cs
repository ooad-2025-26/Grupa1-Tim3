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
    public class AdministratorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdministratorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Administrator
        public async Task<IActionResult> Index()
        {
            return Forbid();
        }

        // GET: Administrator/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            return Forbid();
        }

        // GET: Administrator/Create
        public IActionResult Create()
        {
            return Forbid();
        }

        // POST: Administrator/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ImePrezime,Email,Uloga,StatusNaloga")] Administrator administrator)
        {
            return Forbid();
        }

        // GET: Administrator/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            return Forbid();
        }

        // POST: Administrator/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ImePrezime,Email,Uloga,StatusNaloga")] Administrator administrator)
        {
            return Forbid();
        }

        // GET: Administrator/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            return NotFound();
        }

        // POST: Administrator/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var administrator = await _context.Administratori.FindAsync(id);
            if (administrator != null)
            {
                _context.Administratori.Remove(administrator);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdministratorExists(int id)
        {
            return _context.Administratori.Any(e => e.Id == id);
        }
    }
}
