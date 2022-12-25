using HospitalManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace HospitalManagement.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<User> userManager { get; set; }
        private SignInManager<User> signInManager { get; set; }
        private RoleManager<UserRole> roleManager { get; set; }

        public AccountController(UserManager<User> uMgr, SignInManager<User> signMgr, RoleManager<UserRole> roleMgr)
        {
            userManager = uMgr;
            signInManager = signMgr;
            roleManager = roleMgr;
        }

        // Registration Starts
        //GET
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user)
        {
            try
            {
                var UserFromDb = await userManager.FindByEmailAsync(user.Email);
                if(UserFromDb == null)
                {
                    user.EmailConfirmed = true;
                    IdentityResult result = await userManager.CreateAsync(user, user.PasswordHash);
                    if (result.Succeeded)
                        ViewBag.Message = "User created successfully";
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach(var error in result.Errors)
                        {
                            sb.Append(error.Description);
                            sb.Append('\n');
                        }
                        throw new Exception(sb.ToString());
                    }
                }
            }
            catch(Exception ex)
            {
                ViewBag.Message = ex.Message;
            }
            return View();
        }
        // Registration Ends

        // Authentication Starts
        //GET
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(User user)
        {
            try
            {
                var result = await signInManager.PasswordSignInAsync(user.UserName, user.PasswordHash, false, false);
                if (result.Succeeded)
                { 
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    if (result.IsNotAllowed)
                        throw new Exception("User is not allowed, please confirm your email");
                }
            }
            catch(Exception ex)
            {
                ViewBag.Message = ex.Message;
            }

            return View();
        }

        public async Task<IActionResult> LogOut()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        // Authentication Ends

        // Authorization Starts
        //GET
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole(UserRole role)
        {
            try
            {
                var roleFromDb = await roleManager.FindByNameAsync(role.Name);
                if (roleFromDb == null)
                {
                    var result = await roleManager.CreateAsync(role);
                    if(result.Succeeded)
                        ViewBag.Message = "Role created successfully";
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (var error in result.Errors)
                        {
                            sb.Append(error.Description);
                            sb.Append('\n');
                        }
                        throw new Exception(sb.ToString());
                    }
                }
            }
            catch(Exception ex)
            {
                ViewBag.Message = ex.Message;
            }
            return View();
        }
        // Authorization Ends
    }
}
