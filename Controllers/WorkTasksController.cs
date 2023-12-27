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
using Humanizer;
using System.ComponentModel.Design;

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
            List<string> Mngrs = new List<string>();
            foreach (ApplicationUser item in EmployeeApplicationUsers)
            {
                if (await usrManager.IsInRoleAsync(item, "Employee"))
                    tempList.Add(getEmployee(item, tempMU.CompanyId));
                else if (await usrManager.IsInRoleAsync(item, "Manager"))
                    Mngrs.Add($"{item.FirstName} {item.LastName}");
            }
            if (tempList.Count() < 1)
            {
                tempList.Add(new Employee() { Company = _context.ManageFirm.FirstOrDefault(x => x.Id == tempMU.CompanyId) });
            }
            tempList[0].Managers = Mngrs;
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



        [Authorize(Roles = "Manager")]
        public IActionResult Create(string id = null)
        {
            if(id == null)
                RedirectToAction("Index");
            var viewModel = new WorkTask() { EmployeeName = id, Company = _context.ManageUser.FirstOrDefault(x => x.UserId == id).CompanyId, Status = "ToDo" };
            return View(model:viewModel);
        }

        [Authorize(Roles = "Manager")]
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
            }
            return RedirectToAction(nameof(EmployeePage), new { id = workTask.EmployeeName });
        }

        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
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

        [Authorize(Roles = "Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Description,Status,Company,DeadLine,EmployeeName")] WorkTask workTask)
        {
            if (id == null)
            {
                return NotFound();
            }

            var existingWorkTask = await _context.WorkTask.FindAsync(id);
            existingWorkTask.DeadLine = workTask.DeadLine;
            existingWorkTask.Description = workTask.Description;
            existingWorkTask.Name = workTask.Name;
            _context.Update(existingWorkTask);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(EmployeePage), new { id = existingWorkTask.EmployeeName });
        }

        [Authorize(Roles = "Manager")]
        public async Task<ActionResult> Delete(string id)
        {
            if(id == null)
                return RedirectToAction(nameof(EmployeePage));
            var task = _context.WorkTask.FirstOrDefault(x => x.Id == id);
            var EmployeeName = task.EmployeeName;
            _context.WorkTask.Remove(task);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(EmployeePage), new { id = EmployeeName });
        }
        [HttpPost]
        public IActionResult UpdateStatus(string taskId, string newStatus)
        {
            if(newStatus != "")
            {
                WorkTask myTask = _context.WorkTask.FirstOrDefault(x => x.Id == taskId);
                myTask.Status = newStatus;
                _context.WorkTask.Update(myTask);
                _context.SaveChanges();
            }
            return Json(new { success = true });
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
