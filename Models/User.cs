using System;
using System.ComponentModel.DataAnnotations;

namespace projectAsp.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, EmailAddress]
        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public string DisplayName { get; set; }

        public string PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpires { get; set; }
    }
}