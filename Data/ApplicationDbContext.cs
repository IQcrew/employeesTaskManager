using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using employeesTaskManager.Models;

namespace employeesTaskManager.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<employeesTaskManager.Models.WorkTask>? WorkTask { get; set; }
        public DbSet<employeesTaskManager.Models.ManageFirm>? ManageFirm { get; set; }
        public DbSet<employeesTaskManager.Models.ManageUser>? ManageUser { get; set; }
        public DbSet<employeesTaskManager.Models.passwordToken>? passwordToken { get; set; }

        // DbSet properties for your custom entities, if any
    }
}