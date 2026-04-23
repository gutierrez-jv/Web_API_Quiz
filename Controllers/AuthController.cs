using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Web_API_Quiz.Models;
using Web_API_Quiz.Utils;
using static Web_API_Quiz.Utils.MockDatabase;

namespace Web_API_Quiz.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;

        public AuthController(JwtService jwtService)
        {
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        [ApiKeyAuthorize]
        [EnableRateLimiting("loginPolicy")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = MockData.Users.FirstOrDefault(u =>
                u.Username.Equals(request.Username, StringComparison.OrdinalIgnoreCase) &&
                u.Password == request.Password);

            if (user == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            var token = _jwtService.GenerateToken(user.Username, user.Role);

            return Ok(new LoginResponse
            {
                Token = token,
                Username = user.Username,
                Role = user.Role
            });
        }
    }
}
