using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagement.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [DisplayName("User name")]
        public string UserName { get; set; }

        [Required]
        [DisplayName("Password")]
        public string PasswordHash { get; set; }

        [ForeignKey("UserRoleId")]
        public int UserRoleId { get; set; }

        public UserRole? UserRole { get; set; }
    }
}
