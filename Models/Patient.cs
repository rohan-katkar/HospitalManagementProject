using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagement.Models
{
    public class Patient
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DisplayName("Patient Name")]
        public string PatientName { get; set; }

        [Required]
        public string Prognosis { get; set; }

        [Required]
        [DisplayName("Primary Contact")]
        public string PrimaryContact { get; set; }

        [DisplayName("Secondary Contact")]
        public string? SecondaryContact { get; set; }

        public int DoctorRefId { get; set; }
        [ForeignKey("DoctorRefId")]
        public virtual Doctor? Doctor { get; set; }
    }
}
