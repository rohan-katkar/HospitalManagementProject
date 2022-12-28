using HospitalManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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

        #region Registration
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
                    //user.Id = Guid.NewGuid().ToString();
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
        #endregion

        #region Authentication 
        //GET
        public async Task<IActionResult> Login(string? returnUrl)
        {
            LoginViewModel model = new LoginViewModel()
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account",
                                            new { ReturnUrl = returnUrl });

            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl, string? remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            LoginViewModel model = new LoginViewModel()
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            try
            {
                if (remoteError != null)
                {
                    throw new Exception($"Error from external provider: {remoteError}");
                    //return View("Login", model);
                }

                var info = await signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    throw new Exception("Error loading external login information");
                    //return View("Login", model);
                }

                var signInResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false, true);

                if (signInResult.Succeeded)
                {
                    return LocalRedirect(returnUrl);
                }
                else
                {
                    var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                    if (email != null)
                    {
                        var user = await userManager.FindByEmailAsync(email);

                        if (user == null)
                        {
                            user = new User
                            {
                                UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                                Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                                FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName),
                                LastName = info.Principal.FindFirstValue(ClaimTypes.Surname)
                            };

                            await userManager.CreateAsync(user);
                        }

                        await userManager.AddLoginAsync(user, info);
                        await signInManager.SignInAsync(user, false);

                        return LocalRedirect(returnUrl);
                    }

                    ViewBag.ErrorTitle = $"Email claim not received from: {info.LoginProvider}";
                    ViewBag.ErrorMessage = $"Please contact support";
                }
            }
            catch(Exception ex)
            {
                ViewBag.Message = ex.Message;
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            try
            {
                var result = await signInManager.PasswordSignInAsync(username, password, false, false);
                if (result.Succeeded)
                { 
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("Login Failed\r\n");
                    if (result.IsNotAllowed)
                        sb.Append("User is not allowed, please confirm your email");

                    throw new Exception(sb.ToString());
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
        #endregion

        #region Authorization
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
                    //role.Id = Guid.NewGuid().ToString();
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
        public async Task<IActionResult> AssignRole(string id)
        {
            ViewBag.userId = id;
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var userFromDb = await userManager.FindByIdAsync(id);
            if (userFromDb == null)
            {
                return NotFound();
            }

            var userRoleMapping = new UserRoleMapping();
            userRoleMapping.User = userFromDb;
            var userRole = userManager.GetRolesAsync(userFromDb).Result;
            userRoleMapping.Roles = new List<UserRoleViewModel>();
            foreach (var role in roleManager.Roles)
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };

                if (userRole.Contains(role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }

                userRoleMapping.Roles.Add(userRoleViewModel);
            }

            return View(userRoleMapping);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignRole(UserRoleMapping userRoleMapping)
        {
            try
            {
                var userFromDb = await userManager.FindByNameAsync(userRoleMapping.User.UserName);

                var roles = await userManager.GetRolesAsync(userFromDb);
                var result = await userManager.RemoveFromRolesAsync(userFromDb, roles);

                if (!result.Succeeded)
                {
                    ViewBag.Message = "Could not remove the roles";
                    return View(userRoleMapping);
                }

                result = await userManager.AddToRolesAsync(
                    userFromDb,
                    userRoleMapping.Roles.Where(x => x.IsSelected).Select(y => y.RoleName)
                );

                if (!result.Succeeded)
                {
                    ViewBag.Message = "Could not add the roles";
                    return View(userRoleMapping);
                }

                ViewBag.Message = "Role update success!!";
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }

            return View(userRoleMapping);
        }
        #endregion

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
