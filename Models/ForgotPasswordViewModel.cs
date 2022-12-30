using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
