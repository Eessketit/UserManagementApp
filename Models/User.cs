using System;
using System.ComponentModel.DataAnnotations;

namespace UserManagementApp.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Email { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

        // NEW
        [Required]
        public string Name { get; set; } = "N/A";

        // NEW
        [Required]
        public string Address { get; set; } = "N/A";

        public UserStatus Status { get; set; } = UserStatus.Unverified;

        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastLoginAt { get; set; }

        public string? EmailConfirmationToken { get; set; }
    }
}
