
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Security.Claims;
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

        private readonly IRepository<Employee> userRepo;
        private readonly IAuthService authService;

        public AuthController(IRepository<Employee> userRepo, IAuthService authService)
        {
            this.userRepo = userRepo;
            this.authService = authService;
        }
        /// <summary>
        /// Register
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<ActionResult<Employee>> Register(RegisterRequest request)
        {
            Log.Information("Register attempt email: {@email}", request.Email);
            var user = await authService.RegisterAsync(request);
            if (user == null)
            {
                Log.Warning("User is already exists with email : {@email}", request.Email);
                return BadRequest("User is already exists");
            }
            return Ok(user);
        }
        /// <summary>
        /// Login
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(AuthDto request)
        {
            Log.Information("Login attempt for email : {@email}", request.Email);
            var token = await authService.LoginAsync(request);
            if (token == null)
            {
                Log.Warning("Login failed for email : {@email}", request.Email);
                return BadRequest("Username or password is invalid");
            }
            return Ok(token);
        }
        /// <summary>
        /// Refresh token 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto request)
        {
            Log.Information("Refresh token attempt for token : {@token}", request.RefreshToken);
            var result = await authService.RefreshTokensAsync(request);
            if (result is null || result.AccessToken is null || result.RefreshToken is null)
            {
                return Unauthorized("Invalid token");
            }
            return Ok(result);
        }
        /// <summary>
        /// Change password
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("change-password")]
        public async Task<ActionResult<string>> ChangePassword(ChangePasswordDto request)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            Log.Information("Password change attempt for email : {@email}", email);
            var user = (await userRepo.GetAll(x => x.Email == email)).First();
            var result = await authService.ChangePassword(user.Id, request);
            if (result is null)
            {
                Log.Warning("User not found with email : {@email}", email);
                return BadRequest("Password is null");
            }
            await userRepo.SaveChangesAsync();
            return Ok(result);
        }
    }
}
