namespace employeesTaskManager.Models
{
    public class WorkTask
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; } // ToDo, InProgress, Done
        public string Company { get; set; }
        public DateTime DeadLine { get; set; }
        public ApplicationUser Employee { get; set; }
        public string EmployeeName { get; set; }


        public WorkTask()
        {

        }
    }
}
