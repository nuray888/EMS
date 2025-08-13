using System.Threading.Tasks;
using HrManagement.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrManagement.Api.Controllers
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

        public record LoginRequest(string Email, string Password);

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var tokens = await _authService.LoginAsync(request.Email, request.Password);
            return Ok(tokens);
        }

        public record RefreshRequest(string RefreshToken);

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
        {
            var tokens = await _authService.RefreshAsync(request.RefreshToken);
            return Ok(tokens);
        }
    }
}