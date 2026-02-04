using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using UserManagementApp.Models;
using UserManagementApp.Data;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace UserManagementApp.Pages.Auth;

public class LoginModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly PasswordHasher<User> _hasher = new();

    public LoginModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        public string Password { get; set; } = "";
    }

    public async Task<IActionResult> OnPostAsync()
    {
        // prevent empty password crash
        if (!ModelState.IsValid)
            return Page();

        var user = await _db.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Email == Input.Email);

        if (user == null)
        {
            ModelState.AddModelError("", "Invalid email or password.");
            return Page();
        }

        // blocked user check 
        if (user.Status == UserStatus.Blocked)
        {
            ModelState.AddModelError("", "User is blocked.");
            return Page();
        }

        var result = _hasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            Input.Password
        );

        if (result == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError("", "Invalid email or password.");
            return Page();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Email)
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity)
        );

        // Update LastLoginAt safely
        var trackedUser = await _db.Users.FindAsync(user.Id);
        if (trackedUser != null)
        {
            trackedUser.LastLoginAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }

        return RedirectToPage("/Admin/Users");
    }
}
