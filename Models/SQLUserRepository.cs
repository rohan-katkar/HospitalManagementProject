using HospitalManagement.Data;

namespace HospitalManagement.Models
{
    public class SQLUserRepository : IUserRepository
    {
        //public readonly string RepositoryName = "SQLUserRepository";
        private readonly ApplicationDbContext dbContext;

        public SQLUserRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public string RepositoryName => "SQLUserRepository";

        public List<User> GetAllUsers()
        {
            return dbContext.Users.ToList();
        }

        public User GetUser(int id)
        {
            return dbContext.Users.Find(id);
        }
    }
}
