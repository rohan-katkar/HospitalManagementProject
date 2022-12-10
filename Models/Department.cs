using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagement.Models
{
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }

        [Required]
        [DisplayName("Department Name")]
        public string DepartmentName { get; set; }

        //public virtual ICollection<Doctor> Doctors { get; set; }
    }
}
