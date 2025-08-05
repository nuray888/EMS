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
using UnitedPayment.Services;

namespace UnitedPayment.Controllerr
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {


        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(AuthDto request)
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



    }
}
