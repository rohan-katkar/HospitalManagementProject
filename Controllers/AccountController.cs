using HospitalManagement.Models;
using Microsoft.AspNetCore.Authorization;
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
                    // Comment below line if not required
                    user.EmailConfirmed = true;

                    IdentityResult result = await userManager.CreateAsync(user, user.PasswordHash);
                    if (result.Succeeded)
                        ViewBag.Message = "User created successfully";
                    else
                    {
                        throw new Exception(GetErrorList(result.Errors));
                    }
                }
                else
                {
                    ViewBag.Message = "User already present";
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
        [Authorize(Roles = "Admin")]
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
                        throw new Exception(GetErrorList(result.Errors));
                    }
                }
            }
            catch(Exception ex)
            {
                ViewBag.Message = ex.Message;
            }
            return View();
        }

        // GET
        public IActionResult ShowAllUserDetails()
        {
            var userList = userManager.Users.ToList();
            return View(userList);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignRole(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var userFromDb = await userManager.FindByIdAsync(id.ToString());
            if (userFromDb == null)
            {
                return NotFound();
            }

            var userRoleMapping = new UserRoleMapping();
            userRoleMapping.user = userFromDb;
            var userRole = userManager.GetRolesAsync(userFromDb).Result;
            if(userRole.Count != 0 && userRole != null)
                userRoleMapping.role = userRole[0];

            return View(userRoleMapping);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignRole(UserRoleMapping userRoleMapping)
        {
            try
            {
                var userFromDb = await userManager.FindByNameAsync(userRoleMapping.user.UserName);
                var roleFromDb = await roleManager.FindByNameAsync(userRoleMapping.role);
                if (userFromDb != null && roleFromDb != null)
                {
                    var result = await userManager.AddToRoleAsync(userFromDb, roleFromDb.Name);
                    if(result.Succeeded)
                        ViewBag.Message = "Role change success!!";
                    else
                    {   
                        throw new Exception(GetErrorList(result.Errors));
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }

            return View();
        }
        // Authorization Ends

        public IActionResult AccessDenied()
        {
            return View();
        }

        public string GetErrorList(IEnumerable<IdentityError> errors)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var error in errors)
            {
                sb.Append(error.Description);
                sb.Append("\r\n");
            }
            return sb.ToString();
        }
    }
}
