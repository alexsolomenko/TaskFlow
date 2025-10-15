using Microsoft.AspNetCore.Mvc;
using Taskflow.API.Models;
using Taskflow.API.Services;

namespace Taskflow.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private Data.IUserRepository _userService;
        private IJwtService _jwtService;

        public AuthController(
            Data.IUserRepository userService,
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

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignupRequest request)
        {
            try
            {
                var user = await _userService.RegisterUserAsync(request.Email, request.UserName, request.Password, request.Roles);
            }
            catch (Data.Exceptions.UserAlreadyExistsException err)
            {
                return StatusCode(StatusCodes.Status409Conflict, err.Message);
            }
            return Ok();
        }
    }
}
