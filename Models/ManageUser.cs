using System.ComponentModel.DataAnnotations.Schema;

namespace employeesTaskManager.Models
{
    public class ManageUser
    {
        public int Id { get; set; }
        [ForeignKey("AspNetUsers")]
        public string UserId { get; set; }
        [ForeignKey("ManageFirm")]
        public string CompanyId { get; set; }
        
        public ManageUser() { }
    }
}
