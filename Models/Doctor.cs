using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagement.Models
{
    public class Doctor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DisplayName("Doctor Name")]
        public string DoctorName { get; set; }

        [Required]
        public string Contact { get; set; }

        public int DeptRefId { get; set; }
        [ForeignKey("DeptRefId")]
        public virtual Department? Department { get; set; }

        //public ICollection<Patient> Patient { get; set; }
    }
}
