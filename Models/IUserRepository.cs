using Microsoft.EntityFrameworkCore.Metadata;

namespace HospitalManagement.Models
{
    public interface IUserRepository
    {
        string RepositoryName { get; }

        public List<User> GetAllUsers();
        public User GetUser(int id);
    }
}
