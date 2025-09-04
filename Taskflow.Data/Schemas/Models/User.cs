using System.ComponentModel.DataAnnotations.Schema;
using Taskflow.Data.Schemas.Models;

namespace Taskflow.Data
{
    [Table("users")]
    public class User
    {
        public int Id { get; set; }
        [Column("user_name")]
        public string? Username { get; set; }
        [Column("email")]
        public string? Email { get; set; }
        [Column("password")]
        public string? PasswordHash { get; set; } // Хеш пароля, не plain text!
        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }
        // many to many
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
