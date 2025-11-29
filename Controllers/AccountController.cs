using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using projectAsp.Models;
using projectAsp.Services;

namespace projectAsp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserStore _store;
        private readonly PasswordHasher<User> _hasher;

        public AccountController(UserStore store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _hasher = new PasswordHasher<User>();
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register() => View();

        public class RegisterViewModel
        {
            [Required, Display(Name = "Full name")]
            public string DisplayName { get; set; }

            [Required, EmailAddress]
            public string Email { get; set; }

            [Required, MinLength(6), DataType(DataType.Password)]
            public string Password { get; set; }

            [Required, DataType(DataType.Password), Compare("Password")]
            public string ConfirmPassword { get; set; }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var existing = await _store.FindByEmailAsync(model.Email);
            if (existing != null)
            {
                ModelState.AddModelError("", "This email is already registered.");
                return View(model);
            }

            var user = new User
            {
                Email = model.Email,
                DisplayName = model.DisplayName
            };

            user.PasswordHash = _hasher.HashPassword(user, model.Password);
            await _store.CreateAsync(user);

            TempData["Message"] = "Registration successful. Please log in.";
            return RedirectToAction("Login");
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl ?? Url.Content("~/");
            return View();
        }

        public class LoginViewModel
        {
            [Required, EmailAddress]
            public string Email { get; set; }

            [Required, DataType(DataType.Password)]
            public string Password { get; set; }

            public bool RememberMe { get; set; }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl ?? Url.Content("~/");
            if (!ModelState.IsValid) return View(model);

            var user = await _store.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }

            var verify = _hasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
            if (verify == PasswordVerificationResult.Success)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.DisplayName ?? user.Email),
                    new Claim(ClaimTypes.Email, user.Email)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                var props = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = model.RememberMe ?
DateTimeOffset.UtcNow.AddDays(14) : DateTimeOffset.UtcNow.AddHours(8)
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props);

                return LocalRedirect(returnUrl ?? "/");
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword() => View();

        public class ForgotPasswordViewModel
        {
            [Required, EmailAddress]
            public string Email { get; set; }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _store.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // do not reveal in production
                TempData["Message"] = "If the email exists, a reset token was generated.";
                return View();
            }

            // generate token (demo)
            var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray()) + Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            user.PasswordResetToken = token;
            user.PasswordResetTokenExpires = DateTime.UtcNow.AddHours(1);
            await _store.UpdateAsync(user);

            TempData["ResetToken"] = token;
            TempData["Message"] = "Reset token generated (demo).";
            return View();
        }

        // GET /Account/ResetPassword
        [HttpGet]
        public IActionResult ResetPassword() => View();

        public class ResetPasswordViewModel
        {
            [Required, EmailAddress]
            public string Email { get; set; }

            [Required]
            public string Token { get; set; }

            [Required, MinLength(6), DataType(DataType.Password)]
            public string NewPassword { get; set; }

            [Required, DataType(DataType.Password), Compare("NewPassword")]
            public string ConfirmPassword { get; set; }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _store.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid token or email.");
                return View(model);
            }

            if (user.PasswordResetToken == null ||
                user.PasswordResetTokenExpires == null ||
                user.PasswordResetTokenExpires < DateTime.UtcNow ||
                !string.Equals(user.PasswordResetToken, model.Token, StringComparison.Ordinal))
            {
                ModelState.AddModelError("", "Invalid or expired token.");
                return View(model);
            }

            user.PasswordHash = _hasher.HashPassword(user, model.NewPassword);
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpires = null;
            await _store.UpdateAsync(user);

            TempData["Message"] = "Password reset successful. Please login.";
            return RedirectToAction("Login");
        }
    }
}