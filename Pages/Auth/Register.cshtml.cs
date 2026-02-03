using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using UserManagementApp.Data;
using UserManagementApp.Models;

namespace UserManagementApp.Pages.Auth;

public class RegisterModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly PasswordHasher<User> _passwordHasher = new();

    public RegisterModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Required, MinLength(6)]
        public string Password { get; set; } = "";

        [Required, MinLength(2)]
        public string Name { get; set; } = "";

        // optional
        public string? Address { get; set; }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = Input.Email.Trim(),

            // REQUIRED → always valid
            Name = Input.Name.Trim(),

            // Optional → fallback ONLY if empty
            Address = string.IsNullOrWhiteSpace(Input.Address)
                ? "N/A"
                : Input.Address.Trim(),

            Status = UserStatus.Unverified,
            RegisteredAt = DateTime.UtcNow,
            LastLoginAt = null
        };

        user.PasswordHash =
            _passwordHasher.HashPassword(user, Input.Password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return RedirectToPage("/Auth/Login");
    }
}
