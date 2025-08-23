namespace Taskflow.Data
{
    public class User
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? PasswordHash { get; set; } // Хеш пароля, не plain text!
        public List<string>? Roles { get; set; }
    }
}
