using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    // Add your additional properties here
    public string FirstName { get; set; }
    public string LastName { get; set; }
}