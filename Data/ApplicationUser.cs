using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    // Add your additional properties here
    public virtual string FirstName { get; set; }
    public virtual string LastName { get; set; }
}