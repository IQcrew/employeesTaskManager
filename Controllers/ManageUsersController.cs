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
                if(user.Email == "admin@admin.com") { continue; }
                ManageUser tempUser = new ManageUser();
                tempUser.UserId = user.Id;
                var temp = _context.ManageUser.Where(x => x.UserId == user.Id).ToList();
                if(temp.Count == 1)
                {
                    tempUser.CompanyId = _context.ManageFirm.FirstOrDefault(x => x.Id == temp[0].CompanyId).Name;
                }
                else
                {
                    tempUser.CompanyId = "";
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

                ManageUser tempUser = new ManageUser();
                tempUser.UserId = user.Id;
                var temp = _context.ManageUser.Where(x => x.UserId == user.Id).ToList();
                if (temp.Count == 1)
                {
                    tempUser.CompanyId = _context.ManageFirm.FirstOrDefault(x => x.Id == temp[0].CompanyId).Name;
                }
                else
                {
                    tempUser.CompanyId = "";
                }
                if(!new string[] { user.Email.ToLower(), user.FirstName.ToLower(), user.LastName.ToLower(), tempUser.CompanyId.ToLower()}.Any(x => x.Contains(searchString.ToLower()))) { continue; }
                res.Add(tempUser);
            }
            return _context.ManageUser != null ?
                        View("Index",res) :
                        Problem("Entity set 'ApplicationDbContext.ManageUser'  is null.");
        }


        private int GetNextUserId()
        {
            var maxId = _context.ManageUser.Max(u => (int?)u.Id) ?? 0;
            return maxId + 1;
        }

        private async Task changeUserFirm(string userId, string firmId)
        {
            var findConnection = _context.ManageUser.FirstOrDefault(x => x.UserId == userId);
            if (findConnection != null)
            {
                if (firmId == "None")
                {
                    _context.ManageUser.Remove(findConnection);
                }
                else
                {
                    findConnection.CompanyId = firmId;
                    _context.ManageUser.Update(findConnection);
                }
                await _context.SaveChangesAsync();
            }
            else
            {
                if (firmId != "None")
                {
                    _context.Add(new ManageUser() { UserId = userId, CompanyId = firmId });
                    await _context.SaveChangesAsync();
                }
            }

        }


        // GET: ManageUsers/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.ManageUser == null)
            {
                return NotFound();
            }
            var user = _userManager.Users.FirstOrDefault(x => x.Id == id);
            var temp = new complexUser() { FirstName = user.FirstName, LastName = user.LastName, UserId = id, Email = user.Email, IsEmailConfirmed = user.EmailConfirmed};
            var tempContext = _context.ManageUser.FirstOrDefault(x => x.UserId == id);
            temp.CompanyId = tempContext ==null ? "None" : tempContext.CompanyId;
            var userRoles = await _userManager.GetRolesAsync(user);
            temp.Role = userRoles.Count == 1 ? userRoles[0] : "None";
            temp.Firms = await _context.ManageFirm.ToListAsync();
            return View(temp);
        }

        // POST: ManageUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(complexUser input)
        {
            var user = await _userManager.FindByIdAsync(input.UserId);
            await changeUserFirm(input.UserId, input.CompanyId);
            user.Email = input.Email;
            user.FirstName = input.FirstName;
            user.LastName = input.LastName;
            user.EmailConfirmed = input.IsEmailConfirmed;
            var result = await _userManager.UpdateAsync(user);
            await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));
            if (input.Role != "None")
            {
                await _userManager.AddToRoleAsync(user, input.Role);
            }

            return RedirectToAction("Index");
        }

        // GET: ManageUsers/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.ManageUser == null)
            {
                return NotFound();
            }
            var user = _userManager.Users.FirstOrDefault(x => x.Id == id);
            var temp = new complexUser() { FirstName = user.FirstName, LastName = user.LastName, UserId = id, Email = user.Email, IsEmailConfirmed = user.EmailConfirmed };
            var tempContext = _context.ManageUser.FirstOrDefault(x => x.UserId == id);
            temp.CompanyId = tempContext == null ? "None" : tempContext.CompanyId;
            var userRoles = await _userManager.GetRolesAsync(user);
            temp.Role = userRoles.Count == 1 ? userRoles[0] : "None";
            temp.Firms = await _context.ManageFirm.ToListAsync();
            return View(temp);
        }

        // POST: ManageUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                // Handle the case where the user is not found
                return NotFound();
            }

            // Delete associated WorkTasks
            var userWorkTasks = _context.WorkTask.Where(wt => wt.Employee == user);
            _context.WorkTask.RemoveRange(userWorkTasks);

            // Delete associated ManageUser
            var manageUser = _context.ManageUser.FirstOrDefault(mu => mu.UserId == user.Id);
            if (manageUser != null)
            {
                _context.ManageUser.Remove(manageUser);
            }

            // Get all roles for the user
            var userRoles = await _userManager.GetRolesAsync(user);

            // Remove the user from all roles
            foreach (var role in userRoles)
            {
                await _userManager.RemoveFromRoleAsync(user, role);
            }

            // Delete the user
            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                // Save changes to the database
                await _context.SaveChangesAsync();

                // User successfully deleted
                return RedirectToAction(nameof(Index));
            }
            else
            {
                // Handle the case where user deletion failed
                // You may want to inspect the errors in result.Errors
                return View("Error");
            }
        }

        private bool ManageUserExists(int id)
        {
          return (_context.ManageUser?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
