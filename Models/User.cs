using Microsoft.AspNetCore.Identity;

namespace HospitalManagement.Models
{
    public class User : IdentityUser<int>
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }
    }
}
