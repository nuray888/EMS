
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
        private readonly IEmailService emailService;

        public AuthController(IRepository<Employee> userRepo, IAuthService authService,IEmailService emailService)
        {
            this.userRepo = userRepo;
            this.authService = authService;
            this.emailService = emailService;
        }
        /// <summary>
        /// Register
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult<Employee>> Register(RegisterRequest request)
        {
            var user = await authService.RegisterAsync(request);
            if (user == null)
            {
                return BadRequest("User is already exists");
            }
            if (!user.isVerified)
            { 
                var confirmationLink = Url.Action(nameof(AuthController.Verify), "Auth", new { token=user.VerificationToken },Request.Scheme);
                if (confirmationLink != null)
                {
                    await emailService.SendEmailAsync(request.Name, request.Email, "Confirmation link", confirmationLink);
                }
            }

            return Ok(user);
        }
        /// <summary>
        /// Login
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(AuthDto request)
        {
            var employee = (await userRepo.GetAll(u => u.Email==request.Email)).First();
            var token = await authService.LoginAsync(request);
            if (token == null)
            {
                Log.Warning("Login failed for email : {@email}", request.Email);
                return BadRequest("Username or password is invalid");
            }
            if (!employee.isVerified)
            {
                return BadRequest("User is not verified");
            }
            return Ok(token);
        }
        /// <summary>
        /// Confirm Email
        /// </summary>
        [HttpGet("confirm-email")]
        public async Task<IActionResult> Verify(string token)
        {
            var employee = (await userRepo.GetAll(u => u.VerificationToken == token)).First();
            if (employee == null)
            {
                return BadRequest("Invalid token");
            }
            employee.isVerified = true;
            await userRepo.SaveChangesAsync();
            return Ok("User verified.");
        }
        /// <summary>
        /// Refresh token 
        /// </summary>
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
        /// <summary>
        /// Change password
        /// </summary>
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

        [HttpPost("send-email")]
        public async Task<IActionResult> SendEmail(string toName,string toEmail,string topic,string text)
        {
            await emailService.SendEmailAsync(toName, toEmail, topic, text);
            return Ok("Message is sended");
        }
    }
}
