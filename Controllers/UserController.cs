using HospitalManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepository userRepository;

        public UserController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public IActionResult Index()
        {
            ViewBag.RepositoryName = userRepository.RepositoryName;
            return View();
        }

        public IActionResult ShowAllUserDetails()
        {
            var userList = userRepository.GetAllUsers();
            return View(userList);
        }

        public IActionResult ShowOneUser(int id)
        {
            var user = userRepository.GetUser(id);
            return View(user);
        }
    }
}
