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
using Microsoft.AspNetCore.Identity;
using System.Data;
using NuGet.Packaging.Signing;

namespace employeesTaskManager.Controllers
{
    [Authorize(Roles = "Manager,Employee")]

    public class WorkTasksController : Controller
    {
        private readonly ApplicationDbContext _context;
        UserManager<ApplicationUser> usrManager;

        public WorkTasksController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            usrManager = userManager;
        }

        // GET: WorkTasks
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Employee"))
                return RedirectToAction("EmployeePage");
            ApplicationUser CurrentUser = await usrManager.FindByNameAsync(User.Identity.Name);
            var tempMU = await _context.ManageUser.FirstOrDefaultAsync(x => x.UserId == CurrentUser.Id);
            if (tempMU == null)
                return View("Error","Manager nemá priradenú žiadnu firmu.");
            var firmEmployees = _context.ManageUser.Where(x => x.CompanyId == tempMU.CompanyId);
            var EmployeeApplicationUsers = usrManager.Users.Where(x => firmEmployees.Any(y => y.UserId == x.Id));
            List<Employee> tempList = new List<Employee>();
            foreach (ApplicationUser item in EmployeeApplicationUsers)
            {
                if(await usrManager.IsInRoleAsync(item, "Employee"))
                    tempList.Add(getEmployee(item,tempMU.CompanyId));
            }
            return View(tempList);
        }
        public async Task<IActionResult> EmployeePage(string id = null)
        {
            ApplicationUser CurrentUser;
            if (id == null) 
            {
                if (User.IsInRole("Manager"))
                    return RedirectToAction("Index");
                CurrentUser = await usrManager.FindByNameAsync(User.Identity.Name);
            }
            else
            {
                try
                {
                    CurrentUser = await usrManager.FindByIdAsync(id);
                }
                catch (Exception)
                {
                    return View("Error", "použivateľ sa nenašiel");
                }
            }
            var tempMU = await _context.ManageUser.FirstOrDefaultAsync(x => x.UserId == CurrentUser.Id);
            if (tempMU == null)
                return View("Error", "Použivateľ nemá priradenú žiadnu firmu.");
            var user = getEmployee(CurrentUser,tempMU.CompanyId);
            return View(user);
        }



        // GET: WorkTasks/Create
        public IActionResult Create(string id = null)
        {
            if(id == null)
                RedirectToAction("Index");
            var viewModel = new WorkTask() { EmployeeName = id, Company = _context.ManageUser.FirstOrDefault(x => x.UserId == id).CompanyId, Status = "ToDo" };
            return View(model:viewModel);
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
                workTask.Employee = await usrManager.FindByIdAsync(workTask.EmployeeName);
                if (workTask.Description == null)
                    workTask.Description = "";
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

        private bool WorkTaskExists(string id)
        {
          return (_context.WorkTask?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private Employee getEmployee(ApplicationUser user, string companyId)
        {
            Employee res = new Employee();
            res.Company = _context.ManageFirm.FirstOrDefault(x => x.Id == companyId);
            res.EmployeeId = user.Id;
            res.Name = $"{user.FirstName} {user.LastName}";
            res.Email = user.Email;
            res.tasks = _context.WorkTask.Where(x => x.Employee == user).ToList();
            return res;
        }
    }
}
