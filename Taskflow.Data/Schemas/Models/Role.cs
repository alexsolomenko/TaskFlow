using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Taskflow.Data.Schemas.Models;

[Table("roles")]
public class Role
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }

    // many to many
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}

[Table("user_roles")]
public class UserRole
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int RoleId { get; set; }

    public User? User { get; }
    public Role? Role { get; }
}

public class UserRolesConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.HasKey(ur => new { ur.UserId, ur.RoleId });

        builder.HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId);

        builder.HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId);
    }
}