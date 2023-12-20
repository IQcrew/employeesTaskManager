using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using employeesTaskManager.Data;
using employeesTaskManager.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace employeesTaskManager.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ManageFirmsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ManageFirmsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ManageFirms
        public async Task<IActionResult> Index()
        {
              return _context.ManageFirm != null ? 
                          View(await _context.ManageFirm.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.ManageFirm'  is null.");
        }

        // GET: ManageFirms/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.ManageFirm == null)
            {
                return NotFound();
            }

            var manageFirm = await _context.ManageFirm
                .FirstOrDefaultAsync(m => m.Id == id);
            if (manageFirm == null)
            {
                return NotFound();
            }

            return View(manageFirm);
        }

        // GET: ManageFirms/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ManageFirms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ContactEmail")] ManageFirm manageFirm)
        {
            if (manageFirm.Name != "")
            {
                manageFirm.Id = Guid.NewGuid().ToString();
                _context.Add(manageFirm);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(manageFirm);
        }

        // GET: ManageFirms/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.ManageFirm == null)
            {
                return NotFound();
            }

            var manageFirm = await _context.ManageFirm.FindAsync(id);
            if (manageFirm == null)
            {
                return NotFound();
            }
            return View(manageFirm);
        }

        // POST: ManageFirms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,ContactEmail")] ManageFirm manageFirm)
        {
            if (id != manageFirm.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(manageFirm);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ManageFirmExists(manageFirm.Id))
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
            return View(manageFirm);
        }

        // GET: ManageFirms/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.ManageFirm == null)
            {
                return NotFound();
            }

            var manageFirm = await _context.ManageFirm
                .FirstOrDefaultAsync(m => m.Id == id);
            if (manageFirm == null)
            {
                return NotFound();
            }

            return View(manageFirm);
        }

        // POST: ManageFirms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.ManageFirm == null)
            {
                return Problem("Entity set 'ApplicationDbContext.ManageFirm'  is null.");
            }
            var manageFirm = await _context.ManageFirm.FindAsync(id);
            if (manageFirm != null)
            {
                _context.ManageFirm.Remove(manageFirm);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ManageFirmExists(string id)
        {
          return (_context.ManageFirm?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
