namespace HospitalManagement.Models
{
    public class MockUserRepository : IUserRepository
    {
        //public readonly string RepositoryName = "MockUserRepository";
        public List<User> userList = null;
        public MockUserRepository()
        {
            userList = new List<User>();
            userList.Add(new User() { UserId = 123, UserName = "Retard Amigo", PasswordHash = "Admin123!", UserRoleId = 1 });
            userList.Add(new User() { UserId = 124, UserName = "Joker King", PasswordHash = "Joker321!", UserRoleId = 1 });
        }

        string IUserRepository.RepositoryName => "MockUserRepository";

        public List<User> GetAllUsers()
        {
            return userList;
        }

        public User GetUser(int id)
        {
            var user = userList.Find(x => x.UserId == id);
            return user;
        }
    }
}
