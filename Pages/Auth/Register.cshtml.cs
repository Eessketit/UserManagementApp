using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using UserManagementApp.Models;
using UserManagementApp.Data;

namespace UserManagementApp.Pages.Auth;

public class RegisterModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly PasswordHasher<User> _hasher = new();

    public RegisterModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = Input.Email,
            Status = UserStatus.Unverified,
            RegisteredAt = DateTime.UtcNow
        };

        user.PasswordHash = _hasher.HashPassword(user, Input.Password);

        _db.Users.Add(user);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch
        {
            ModelState.AddModelError("", "Email already exists.");
            return Page();
        }

        TempData["Success"] = "Registration successful. You can log in now.";
        return RedirectToPage("/Auth/Login");
    }
}
