﻿using System.ComponentModel.DataAnnotations.Schema;

namespace employeesTaskManager.Models
{
    public class complexUser
    {
        public string UserId { get; set; }
        public string CompanyId { get; set; }
        public string Role { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
