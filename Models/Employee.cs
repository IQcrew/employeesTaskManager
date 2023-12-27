using employeesTaskManager.Data.Migrations;

namespace employeesTaskManager.Models
{
    public class Employee
    {
        public string EmployeeId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<string> Managers { get; set; }
        public ManageFirm Company { get; set; }
        public List<WorkTask> tasks { get; set; }

    }
}
