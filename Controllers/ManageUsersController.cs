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
using Microsoft.AspNetCore.Identity;

namespace employeesTaskManager.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ManageUsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;
        public ManageUsersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }



        // GET: ManageUsers
        public async Task<IActionResult> Index()
        {
            List<ManageUser> res = new List<ManageUser>();
            var users = _userManager.Users;
            foreach (var user in users)
            {
                ManageUser tempUser = new ManageUser();
                tempUser.UserId = user.Id;
                var temp = _context.ManageUser.Where(x => x.UserId == user.Id).ToList();
                if(temp.Count == 1)
                {
                    tempUser.CompanyId = temp[0].CompanyId;
                }
                else
                {
                    tempUser.CompanyId = null;
                }
                res.Add(tempUser);
            }


              return _context.ManageUser != null ? 
                          View(res) :
                          Problem("Entity set 'ApplicationDbContext.ManageUser'  is null.");
        }
        public async Task<IActionResult> SearchResults(string searchString)
        {
            List<ManageUser> res = new List<ManageUser>();
            var users = _userManager.Users;
            foreach (var user in users)
            {

                if(!new string[] { user.Email.ToLower(), user.FirstName.ToLower(), user.LastName.ToLower()}.Any(x => x.Contains(searchString.ToLower()))) { continue; }
                ManageUser tempUser = new ManageUser();
                tempUser.UserId = user.Id;
                var temp = _context.ManageUser.Where(x => x.UserId == user.Id).ToList();
                if (temp.Count == 1)
                {
                    tempUser.CompanyId = temp[0].CompanyId;
                }
                else
                {
                    tempUser.CompanyId = "-1";
                }
                res.Add(tempUser);
            }
            return _context.ManageUser != null ?
                        View("Index",res) :
                        Problem("Entity set 'ApplicationDbContext.ManageUser'  is null.");
        }


        private int GetNextUserId()
        {
            // You need to implement your logic to get the next unique integer
            // This is a simplistic example, you may need to adjust it based on your needs
            var maxId = _context.ManageUser.Max(u => (int?)u.Id) ?? 0;
            return maxId + 1;
        }
        // GET: ManageUsers/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.ManageUser == null)
            {
                return NotFound();
            }
            var user = _userManager.Users.FirstOrDefault(x => x.Id == id);
            var temp = new complexUser() { FirstName = user.FirstName, LastName = user.LastName, UserId = id, Email = user.Email};
            var tempContext = _context.ManageUser.FirstOrDefault(x => x.UserId == id);
            if (tempContext != null)
            {
                temp.CompanyId = tempContext.CompanyId;
            }


            return View(temp);
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
