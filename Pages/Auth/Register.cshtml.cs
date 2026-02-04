using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using UserManagementApp.Data;
using UserManagementApp.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

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

        [Required]
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

        try
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            ModelState.AddModelError("Input.Email", "An account with this email already exists.");
            return Page();
        }

        TempData["RegistrationSuccess"] = "User registered successfully.";
        return RedirectToPage("/Auth/Login");
    }
    private static bool IsUniqueConstraintViolation(DbUpdateException ex)
    {
        return ex.InnerException is PostgresException postgresException
            && postgresException.SqlState == PostgresErrorCodes.UniqueViolation;
    }
}

