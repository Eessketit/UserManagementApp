using System;
using System.ComponentModel.DataAnnotations;

namespace UserManagementApp.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Email { get; set; }
    }
}
