using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using UserManagementApp.Data;
using UserManagementApp.Models;
using UserManagementApp.Services;

namespace UserManagementApp.Pages.Auth
{
    public class LoginModel : PageModel
    {
        private readonly AppDbContext _db;
        private readonly PasswordService _passwordService;

        public LoginModel(AppDbContext db, PasswordService passwordService)
        {
            _db = db;
            _passwordService = passwordService;
        }

        [BindProperty]
        public string Email { get; set; } = null!;

        [BindProperty]
        public string Password { get; set; } = null!;

        public string? Message { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _db.Users
                .SingleOrDefaultAsync(u => u.Email == Email);

            if (user == null ||
                !_passwordService.Verify(user.PasswordHash, Password))
            {
                Message = "Invalid email or password.";
                return Page();
            }

            if (user.Status == UserStatus.Blocked)
            {
                Message = "Your account is blocked.";
                return Page();
            }

            user.LastLoginAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
/*  */
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("Status", user.Status.ToString())
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            return RedirectToPage("/Index");
        }
    }
}
