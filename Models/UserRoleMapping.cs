namespace HospitalManagement.Models
{
    public class UserRoleMapping
    {
        public User User { get; set; }

        //public string role { get; set; }
        public List<UserRoleViewModel> Roles { get; set; }
    }
}
