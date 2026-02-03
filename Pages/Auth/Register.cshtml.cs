using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UserManagementApp.Data;
using UserManagementApp.Models;
using UserManagementApp.Services;

namespace UserManagementApp.Pages.Auth
{
    public class RegisterModel : PageModel
    {
        private readonly AppDbContext _db;
        private readonly PasswordService _passwordService;

        public RegisterModel(AppDbContext db, PasswordService passwordService)
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
            var user = new User
            {
                Email = Email,
                PasswordHash = _passwordService.Hash(Password),
                Status = UserStatus.Unverified,
                EmailConfirmationToken = Guid.NewGuid().ToString()
            };

            try
            {
                _db.Users.Add(user);
                await _db.SaveChangesAsync();
            }
            catch
            {
                // UNIQUE INDEX violation lands here
                Message = "Registration failed. Email may already exist.";
                return Page();
            }

            Message = "Registration successful. Please check your e-mail.";
            return Page();
        }
    }
}
