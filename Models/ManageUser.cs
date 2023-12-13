namespace employeesTaskManager.Models
{
    public class ManageUser
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Role { get; set; }
        public string CompanyId { get; set; }
        public string Email { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        
        public ManageUser() { }
    }
}
