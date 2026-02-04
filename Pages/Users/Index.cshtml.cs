using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UserManagementApp.Data;
using UserManagementApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace UserManagementApp.Pages.Users
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _db;

        public IndexModel(AppDbContext db)
        {
            _db = db;
        }

        public List<User> Users { get; set; } = [];

        public Guid CurrentUserId { get; set; }

        [BindProperty]
        public List<Guid> SelectedIds { get; set; } = [];

        public async Task OnGetAsync()
        {
            CurrentUserId = Guid.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );

            Users = await _db.Users
                .OrderByDescending(u => u.LastLoginAt)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostBlockAsync()
        {
            await UpdateStatusAsync(UserStatus.Blocked);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUnblockAsync()
        {
            await UpdateStatusAsync(UserStatus.Active);
            return RedirectToPage();
        }


        public async Task<IActionResult> OnPostDeleteAsync()
        {
            var currentUserId = Guid.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );
            if (SelectedIds.Any())
            {
                var users = _db.Users
                    .Where(u => SelectedIds.Contains(u.Id));

                _db.Users.RemoveRange(users);
                await _db.SaveChangesAsync();
            }
            if (SelectedIds.Contains(currentUserId))
            {
                await HttpContext.SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme
                );
                return RedirectToPage("/Auth/Login");
            }

            return RedirectToPage();
        }

        private async Task UpdateStatusAsync(UserStatus status)
        {
            if (!SelectedIds.Any())
                return;

            var users = await _db.Users
                .Where(u => SelectedIds.Contains(u.Id))
                .ToListAsync();

            foreach (var user in users)
            {
                user.Status = status;
            }

            await _db.SaveChangesAsync();
        }

    }
}
