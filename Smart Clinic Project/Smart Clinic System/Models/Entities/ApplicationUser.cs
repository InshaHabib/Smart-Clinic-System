using Microsoft.AspNet.Identity.EntityFramework;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; }
    public UserRole Role { get; set; }
    public string Phone { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    
    // Navigation Properties
    public virtual Doctor Doctor { get; set; }
    public virtual Patient Patient { get; set; }
}