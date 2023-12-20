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
    [Authorize(Roles = "Manager,Employee")]

    public class WorkTasksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WorkTasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: WorkTasks
        public async Task<IActionResult> Index()
        {
              return _context.WorkTask != null ? 
                          View(await _context.WorkTask.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.WorkTask'  is null.");
        }

        // GET: WorkTasks/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.WorkTask == null)
            {
                return NotFound();
            }

            var workTask = await _context.WorkTask
                .FirstOrDefaultAsync(m => m.Id == id);
            if (workTask == null)
            {
                return NotFound();
            }

            return View(workTask);
        }

        // GET: WorkTasks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: WorkTasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Status,Company,DeadLine,EmployeeName")] WorkTask workTask)
        {
            if ( workTask.Company != "")
            {
                workTask.Id = Guid.NewGuid().ToString();
                _context.Add(workTask);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(workTask);
        }

        // GET: WorkTasks/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.WorkTask == null)
            {
                return NotFound();
            }

            var workTask = await _context.WorkTask.FindAsync(id);
            if (workTask == null)
            {
                return NotFound();
            }
            return View(workTask);
        }

        // POST: WorkTasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Description,Status,Company,DeadLine,EmployeeName")] WorkTask workTask)
        {
            if (id != workTask.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workTask);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkTaskExists(workTask.Id))
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
            return View(workTask);
        }

        // GET: WorkTasks/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.WorkTask == null)
            {
                return NotFound();
            }

            var workTask = await _context.WorkTask
                .FirstOrDefaultAsync(m => m.Id == id);
            if (workTask == null)
            {
                return NotFound();
            }

            return View(workTask);
        }

        // POST: WorkTasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.WorkTask == null)
            {
                return Problem("Entity set 'ApplicationDbContext.WorkTask'  is null.");
            }
            var workTask = await _context.WorkTask.FindAsync(id);
            if (workTask != null)
            {
                _context.WorkTask.Remove(workTask);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkTaskExists(string id)
        {
          return (_context.WorkTask?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
