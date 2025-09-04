using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Taskflow.API.Services;
using Taskflow.Data;

namespace Taskflow.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private IUserRepository _userService;
        private IJwtService _jwtService;

        public AuthController(
            IUserRepository userService,
            IJwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // 1. Проверяем credentials в БД
            var user = await _userService.ValidateUserAsync(request.Email, request.Password);
            if (user == null) 
                return Unauthorized();

            // 2. Генерируем JWT токен
            var token = _jwtService.GenerateToken(user.Id.ToString(), user.Username ?? string.Empty, user.UserRoles.Select(r => r.Role?.Name ?? string.Empty).ToList());

            return Ok(new { Token = token });
        }
    }
}
