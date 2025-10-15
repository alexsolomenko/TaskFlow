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
    Task<User?> ValidateUserAsync(
        string username,
        string password);
    Task<User?> GetUserByIdAsync(int userId);
    Task<User> RegisterUserAsync(
        string userEmail,
        string? userName,
        string password,
        List<string> roleNames);
}

public class UserRepository : IUserRepository
{
    private readonly AppDBContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public UserRepository(
        AppDBContext context, 
        IPasswordHasher passwordHasher)
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

    public async Task<User> RegisterUserAsync(
        string userEmail, 
        string? userName, 
        string password, 
        List<string> roleNames)
    {
        string email = userEmail.ToLower();
        string? name = userName?.ToLower();
        var user = await _context.Users.FirstOrDefaultAsync(u => (u.Email != null && u.Email.ToLower() == email) 
            || (u.Username != null && u.Username.ToLower() == name));

        if (user != null)
            throw new Exceptions.UserAlreadyExistsException();

        user = new User()
        {
            Email = userEmail,
            Username = userName,
            PasswordHash = _passwordHasher.HashPassword(password),
            CreatedAt = DateTime.UtcNow
        };

        var existingRoles = await _context.Roles
            .Where(r => roleNames.Contains(r.Name)).ToListAsync();

        foreach (var role in existingRoles)
        {
            user.UserRoles.Add(new UserRole() { Id = user.Id, RoleId = role.Id });
        }

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }
}
