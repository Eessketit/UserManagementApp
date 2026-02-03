using System;
using System.ComponentModel.DataAnnotations;

namespace UserManagementApp.Models;

public class User
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid(); // ✅ FIX #1

    [Required, EmailAddress]
    public string Email { get; set; } = null!;

    public string? Name { get; set; } // ✅ FIX #2

    [Required]
    public string PasswordHash { get; set; } = null!;

    public UserStatus Status { get; set; } = UserStatus.Unverified;

    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastLoginAt { get; set; }

    public string? EmailConfirmationToken { get; set; }
}
