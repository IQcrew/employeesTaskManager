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
    public class ManageUsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ManageUsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ManageUsers
        public async Task<IActionResult> Index()
        {
              return _context.ManageUser != null ? 
                          View(await _context.ManageUser.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.ManageUser'  is null.");
        }

        // GET: ManageUsers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ManageUser == null)
            {
                return NotFound();
            }

            var manageUser = await _context.ManageUser
                .FirstOrDefaultAsync(m => m.Id == id);
            if (manageUser == null)
            {
                return NotFound();
            }

            return View(manageUser);
        }

        // GET: ManageUsers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ManageUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,CompanyId")] ManageUser manageUser)
        {
            if (ModelState.IsValid)
            {
                _context.Add(manageUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(manageUser);
        }

        // GET: ManageUsers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ManageUser == null)
            {
                return NotFound();
            }

            var manageUser = await _context.ManageUser.FindAsync(id);
            if (manageUser == null)
            {
                return NotFound();
            }
            return View(manageUser);
        }

        // POST: ManageUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,CompanyId")] ManageUser manageUser)
        {
            if (id != manageUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(manageUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ManageUserExists(manageUser.Id))
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
            return View(manageUser);
        }

        // GET: ManageUsers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ManageUser == null)
            {
                return NotFound();
            }

            var manageUser = await _context.ManageUser
                .FirstOrDefaultAsync(m => m.Id == id);
            if (manageUser == null)
            {
                return NotFound();
            }

            return View(manageUser);
        }

        // POST: ManageUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ManageUser == null)
            {
                return Problem("Entity set 'ApplicationDbContext.ManageUser'  is null.");
            }
            var manageUser = await _context.ManageUser.FindAsync(id);
            if (manageUser != null)
            {
                _context.ManageUser.Remove(manageUser);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ManageUserExists(int id)
        {
          return (_context.ManageUser?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
