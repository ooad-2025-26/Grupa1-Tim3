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

namespace EVrtic.Controllers
{
    [Authorize(Roles = "ADMINISTRATOR")]
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
            return Forbid();
        }

        // GET: AlergijaDjeteta/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            return Forbid();
        }

        // GET: AlergijaDjeteta/Create
        public IActionResult Create()
        {
            return Forbid();
        }

        // POST: AlergijaDjeteta/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Naziv,DijeteId")] AlergijaDjeteta alergijaDjeteta)
        {
            return Forbid();
        }

        // GET: AlergijaDjeteta/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            return Forbid();
        }

        // POST: AlergijaDjeteta/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Naziv,DijeteId")] AlergijaDjeteta alergijaDjeteta)
        {
            return Forbid();
        }

        // GET: AlergijaDjeteta/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            return Forbid();
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
