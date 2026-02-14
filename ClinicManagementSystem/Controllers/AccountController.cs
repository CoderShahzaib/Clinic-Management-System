using ClinicManagementSystem.Core.Domain.IdentityEntities;
using ClinicManagementSystem.Core.DTOs;
using ClinicManagementSystem.Core.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ClinicManagementSystem.UI.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;

        public AccountController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, SignInManager<ApplicationUser> signInManager, IEmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found");
                return View(dto);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user,
                dto.Password,
                isPersistent: true,  
                lockoutOnFailure: false
            );

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Invalid credentials");
                return View(dto);
            }

            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim("FullName", user.PersonName ?? user.UserName)
    };
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,   
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1)
            };

            await HttpContext.SignInAsync(
                IdentityConstants.ApplicationScheme,
                new ClaimsPrincipal(identity),
                authProperties
            );

            if (roles.Contains("Admin"))
                return RedirectToAction("Dashboard", "Admin");
            if (roles.Contains("Doctor"))
                return RedirectToAction("Dashboard", "Doctor");
            if (roles.Contains("Patient"))
                return RedirectToAction("Dashboard", "Patient");

            await _signInManager.SignOutAsync();
            ModelState.AddModelError("", "No valid role assigned");
            return View(dto);
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            if(ModelState.IsValid == false)
            {
                return View(dto);
            }

            ApplicationUser user = new ApplicationUser()
            {
                PersonName = dto.UserName,
                UserName = dto.Email,
                Email = dto.Email,
               
            };

            IdentityResult result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                ViewBag.Errors = result.Errors.Select(e => e.Description);
                return View(dto);
            }

            if(!await _roleManager.RoleExistsAsync(dto.Role))
            {
                await _roleManager.CreateAsync(new ApplicationRole
                {
                    Name = dto.Role
                });
            }

            await _userManager.AddToRoleAsync(user, dto.Role);

            return RedirectToAction("Login");
        }

        [HttpGet("Register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet("Forgot-Password")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost("Forgot-Password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                ViewBag.Message = "If the email exists, a reset link has been sent.";
                return View();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var resetLink = Url.Action(
                "ResetPassword",
                "Account",
                new { userId = user.Id, token = token },
                protocol: HttpContext.Request.Scheme
            );

            await _emailService.SendAsync(
                dto.Email,
                "Reset Your Password",
                $"""
                <h3>Password Reset</h3>
                <p>Click the link below to reset your password:</p>
                <a href='{resetLink}'>Reset Password</a>
                <p>This link will expire.</p>
                """
                );

            ViewBag.Message = "If the email exists, a reset link has been sent.";
            return View();
        }

        [HttpGet("Reset-Password")]
        public IActionResult ResetPassword(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                return BadRequest("Invalid password reset request.");

            return View(new ResetPasswordDTO
            {
                UserId = userId,
                Token = token
            });
        }

        [HttpPost("Reset-Password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
                return RedirectToAction("Login");

            var result = await _userManager.ResetPasswordAsync(
                user,
                dto.Token,
                dto.NewPassword
            );

            if (!result.Succeeded)
            {
                ViewBag.Errors = result.Errors.Select(e => e.Description);
                return View(dto);
            }

            return RedirectToAction("Login");
        }


        [HttpPost("Logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
