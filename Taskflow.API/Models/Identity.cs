namespace Taskflow.API.Models
{
    public class SignupRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string ConfirmPassword { get; set; }
        public string? UserName { get; set; } // Опционально
        public required List<string> Roles { get; set; }
    }

    public sealed class LoginRequest
    {
        public required string Email { get; init; }
        public required string Password { get; init; }
    }
}
