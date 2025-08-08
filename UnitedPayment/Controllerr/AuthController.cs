using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UnitedPayment.Model;
using UnitedPayment.Model.DTOs;
using UnitedPayment.Model.DTOs.Requests;
using UnitedPayment.Model.DTOs.Responses;
using UnitedPayment.Repository;
using UnitedPayment.Services;

namespace UnitedPayment.Controllerr
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IRepository<User> userRepo;
        private readonly IAuthService authService;

        public AuthController(IRepository<User> userRepo, IAuthService authService)
        {
            this.userRepo = userRepo;
            this.authService = authService;
        }
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(RegisterRequest request)
        {
            var user = await authService.RegisterAsync(request);
            if (user == null)
            {
                return BadRequest("User is already exists");
            }
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(AuthDto request)
        {
            var token = await authService.LoginAsync(request);
            if (token == null)
            {
                return BadRequest("Username or password is invalid");
            }
            return Ok(token);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto request)
        {
            var result = await authService.RefreshTokensAsync(request);
            if (result is null || result.AccessToken is null || result.RefreshToken is null)
            {
                return Unauthorized("Invalid token");
            }
            return Ok(result);
        }

        [HttpPost("change-password")]
        public async Task<ActionResult<string>> ChangePassword(ChangePasswordDto request)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = (await userRepo.GetAll(x => x.Email == email)).First();
            var result = await authService.ChangePassword(user.Id, request);
            if (result is null)
            {
                return BadRequest("Password is null");
            }
            await userRepo.SaveChangesAsync();


            return Ok(result);
        }
    }
}
