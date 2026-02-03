using System;
using System.ComponentModel.DataAnnotations;

namespace UserManagementApp.Models
{
    public enum UserStatus
    {
        Unverified = 0,
        Active = 1,
        Blocked = 2
    }

    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Email { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

        public UserStatus Status { get; set; } = UserStatus.Unverified;

        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastLoginAt { get; set; }

        public string? EmailConfirmationToken { get; set; }
    }
}
