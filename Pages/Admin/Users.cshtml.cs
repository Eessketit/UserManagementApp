using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using UserManagementApp.Data;
using UserManagementApp.Models;

namespace UserManagementApp.Pages.Admin;

[Authorize]
public class UsersModel : PageModel
{
    private readonly AppDbContext _db;

    public UsersModel(AppDbContext db)
    {
        _db = db;
    }

    public List<User> Users { get; set; } = [];

    public async Task OnGetAsync()
    {
        Users = await _db.Users
            .OrderByDescending(u => u.LastLoginAt)
            .AsNoTracking()
            .ToListAsync();
    }

    // ==========================
    // BULK ACTIONS
    // ==========================

    public async Task<IActionResult> OnPostBlockAsync(Guid[] selectedIds)
    {
        if (selectedIds.Length == 0)
            return RedirectToPage();

        var users = await _db.Users
            .Where(u => selectedIds.Contains(u.Id))
            .ToListAsync();

        foreach (var user in users)
        {
            user.Status = UserStatus.Blocked;
        }

        await _db.SaveChangesAsync();
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostUnblockAsync(Guid[] selectedIds)
    {
        if (selectedIds.Length == 0)
            return RedirectToPage();

        var users = await _db.Users
            .Where(u => selectedIds.Contains(u.Id))
            .ToListAsync();

        foreach (var user in users)
        {
            user.Status = UserStatus.Active;
        }

        await _db.SaveChangesAsync();
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid[] selectedIds)
    {
        if (selectedIds.Length == 0)
            return RedirectToPage();

        var users = await _db.Users
            .Where(u => selectedIds.Contains(u.Id))
            .ToListAsync();

        _db.Users.RemoveRange(users);
        await _db.SaveChangesAsync();

        return RedirectToPage();
    }
}
