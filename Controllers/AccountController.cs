using HospitalManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Security.Claims;
using System.Text;

namespace HospitalManagement.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<User> userManager { get; set; }
        private SignInManager<User> signInManager { get; set; }
        private RoleManager<UserRole> roleManager { get; set; }
        private ILogger<AccountController> _logger { get; set; }

        public AccountController(UserManager<User> uMgr, SignInManager<User> signMgr, RoleManager<UserRole> roleMgr, ILogger<AccountController> logger)
        {
            userManager = uMgr;
            signInManager = signMgr;
            roleManager = roleMgr;
            _logger = logger;
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
                if (UserFromDb != null)
                {
                    throw new Exception("User with this username already present");
                }
                // Comment below line if not required
                // user.Id = Guid.NewGuid().ToString();
                    
                // Uncomment if email confirmed not required
                // user.EmailConfirmed = true;

                IdentityResult result = await userManager.CreateAsync(user, user.PasswordHash);

                if (result.Succeeded)
                {
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action("ConfirmEmail", "Account",
                                            new { UserId = user.Id, token = token }, Request.Scheme);

                    ViewBag.ErrorTitle = "Registration Success";
                    ViewBag.ErrorMessage = "Before you can login, please confirm your email, by clicking the confirmation link we have emailed you";

                    return View(viewName: "ImportantInfo");
                    //ViewBag.Message = "User created successfully";
                }
                else
                {
                    throw new Exception(GetErrorList(result.Errors));
                }
            }
            catch(Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
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

        #region ExternalLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account",
                                            new { ReturnUrl = returnUrl });

            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl, string? remoteError)
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

                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                User user = null;

                if(email != null)
                {
                    user = await userManager.FindByEmailAsync(email);
                    if(user != null && !user.EmailConfirmed)
                    {
                        throw new Exception("Email not confirmed yet");
                    }
                }

                var signInResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false, true);

                if (signInResult.Succeeded)
                {
                    return LocalRedirect(returnUrl);
                }
                else
                {
                    if (email != null)
                    {
                        user = await userManager.FindByEmailAsync(email);

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

                            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                            var confirmationLink = Url.Action("ConfirmEmail", "Account",
                                                    new { UserId = user.Id, token = token }, Request.Scheme);

                            _logger.Log(Microsoft.Extensions.Logging.LogLevel.Information, confirmationLink);

                            ViewBag.ErrorTitle = "Registration Success";
                            ViewBag.ErrorMessage = "Before you can login, please confirm your email, by clicking the confirmation link we have emailed you";

                            return View("ImportantInfo", model);
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
                ModelState.AddModelError(string.Empty, ex.Message);
                //ViewBag.Message = ex.Message;
            }
            return View("Login", model);
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if(userId == null || token == null)
            {
                return RedirectToAction("Index", "Home");
            }
            try
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    throw new Exception($"The User ID {userId} is invalid");
                }
                var result = await userManager.ConfirmEmailAsync(user, token);

                if (result.Succeeded)
                {
                    return View(viewName: "ConfirmEmail");
                }
                else
                {
                    ViewBag.ErrorTitle = "Email could not be confirmed";
                    return View(viewName: "ImportantInfo");
                }
            }
            catch(Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return View(viewName: "Login");
        }

        #endregion
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            LoginViewModel model = new LoginViewModel()
            {
                //ReturnUrl = returnUrl,
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            try
            {
                var user = await userManager.FindByNameAsync(username);
                if (user != null && !user.EmailConfirmed)
                {
                    throw new Exception("Email not confirmed yet");
                }

                var result = await signInManager.PasswordSignInAsync(username, password, false, true);
                if (result.Succeeded)
                { 
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    if (result.IsLockedOut)
                        return RedirectToAction("AccountLockout");

                    throw new Exception("Login Failed");
                }
            }
            catch(Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View(model);
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

        #region Forgot and Reset Password
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.Email != null)
                    {
                        var user = await userManager.FindByEmailAsync(model.Email);
                        if (user == null)
                        {
                            throw new Exception("User not found");
                        }

                        var token = await userManager.GeneratePasswordResetTokenAsync(user);
                        var confirmationLink = Url.Action("ResetPassword", "Account",
                                                    new { Email = user.Email, token = token }, Request.Scheme);

                        _logger.Log(Microsoft.Extensions.Logging.LogLevel.Information, confirmationLink);

                        ViewBag.ErrorTitle = "Link Generation Success";
                        ViewBag.ErrorMessage = "You can reset the password by clicking the confirmation link we have emailed you";

                        return View(viewName: "ImportantInfo");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return View(model);
        }

        public IActionResult ResetPassword(string token, string email)
        {
            try
            {
                if(token == null || email == null)
                {
                    throw new Exception("Invalid password reset token");
                }
            }
            catch(Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(PasswordResetViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await userManager.FindByEmailAsync(model.Email);
                    if (user != null)
                    {
                        var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
                        if (user.LockoutEnabled)
                        {
                            await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
                        }

                        if (result.Succeeded)
                        {
                            return View(viewName: "ResetPasswordConfirmation");
                        }
                        else
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                        }
                    }
                    return View(viewName: "ResetPasswordConfirmation");
                }
            }
            catch(Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View(model);
        }

        #endregion

        public IActionResult AccountLockout()
        {
            return View();
        }

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

        public IActionResult ImportantInfo()
        {
            return View();
        }
    }
}
