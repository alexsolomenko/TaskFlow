using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Taskflow.Data.Schemas.Models;

namespace Taskflow.Data;

public interface IUserRepository
{
    Task<User?> ValidateUserAsync(string username, string password);
    Task<User?> GetUserByIdAsync(int userId);
}

public class UserRepository : IUserRepository
{
    private readonly AppDBContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public UserRepository(AppDBContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<User?> ValidateUserAsync(string userEmail, string password)
    {
        string email = userEmail.ToLower();
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Email != null && u.Email.ToLower() == email);

        if (user == null)
            return null;

        var hash = _passwordHasher.HashPassword(password);

        var passwordVerificationResult = _passwordHasher
            .VerifyHashedPassword(user.PasswordHash ?? string.Empty, password);

        return passwordVerificationResult ? user : null;
    }

    public async Task<User?> RegisterUserAsync(string userEmail, string userName, string password)
    {
        string email = userEmail.ToLower();
        string name = userName.ToLower();
        var user = await _context.Users.FirstOrDefaultAsync(u => (u.Email != null && u.Email.ToLower() == email) || (u.Username != null && u.Username.ToLower() == name));
        if (user != null)
            return null;

        user = new User()
        {
            Email = userEmail,
            Username = userName,
            PasswordHash = _passwordHasher.HashPassword(password),
            CreatedAt = DateTime.UtcNow
        };
        user.UserRoles.Add(new UserRole() { Id = user.Id, });

        var entry = await _context.Users.AddAsync(new User()
        {
            Email = userEmail,
            Username = userName,
            PasswordHash = _passwordHasher.HashPassword(password),
            CreatedAt = DateTime.UtcNow
        });
        return user;
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<User?> GetUserByEmail()
}
