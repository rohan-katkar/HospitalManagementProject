using Microsoft.AspNetCore.Authentication;

namespace HospitalManagement.Models
{
    public class LoginViewModel
    {
        public string? UserName { get; set; }

        public string? Password { get; set; }

        public string? ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }
    }
}
