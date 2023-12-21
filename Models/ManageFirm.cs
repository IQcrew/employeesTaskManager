using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace employeesTaskManager.Models
{
    public class ManageFirm
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string ContactEmail { get; set; }
        public ManageFirm()
        {

        }
    }
}
