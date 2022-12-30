using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models
{
    public class PasswordResetViewModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [DisplayName("Confirm Password")]
        [Compare("Password", ErrorMessage = "Password and Confirm Password should be same")]
        public string ConfirmPassword { get; set; }

        public string Token { get; set; }
    }
}
