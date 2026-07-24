using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.IdentityModel.Tokens.Jwt;
using EDG_B2B.Models;
using EDG_B2B.Services;

namespace EDG_B2B.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, message, userId) = await _authService.RegisterAsync(request);

            if (!success)
                return BadRequest(new { success, message });

            return StatusCode(201, new { success, message, userId });
        }

        [HttpPost("login")]
        [EnableRateLimiting("LoginPolicy")] // Apply rate limiting to the login endpoint
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, message, user, token) = await _authService.LoginAsync(request);

            if (!success)
                return Unauthorized(new { success, message });

            return Ok(new { success, message, user, token });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var jti = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
            if (string.IsNullOrEmpty(jti))
                return Unauthorized(new { success = false, message = "Oturum kimliği doğrulanamadı." });

            await _authService.LogoutAsync(jti);

            return Ok(new { success = true, message = "Başarıyla çıkış yaptınız." });
        }
    }
}
