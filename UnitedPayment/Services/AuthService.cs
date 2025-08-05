using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UnitedPayment.Model;
using UnitedPayment.Model.DTOs;
using UnitedPayment.Model.DTOs.Responses;
using UnitedPayment.Model.Enums;

namespace UnitedPayment.Services
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(AuthDto request);
        Task<string?> LoginAsync(AuthDto request);
    }
    public class AuthService(AppDbContext context, IConfiguration configuration) : IAuthService
    {
        public async Task<string?> LoginAsync(AuthDto request)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                Log.Warning("User not found with email=> {@email}", request.Email);
                return null;
            }

            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
            {
                Log.Warning("Invalid Password with given email =>{@email}", request.Email);
                return null;
            }


            return CreateToken(user);
        }

        public async Task<User?> RegisterAsync(AuthDto request)
        {
            if (await context.Users.AnyAsync(u => u.Email == request.Email))
            {
                Log.Warning("User is already exist with given email => {@email}", request.Email);
                return null;
            }
            var user = new User();
            var hashedPassword = new PasswordHasher<User>()
       .HashPassword(user, request.Password);
            user.Email = request.Email;
            user.PasswordHash = hashedPassword;
            context.Users.Add(user);
            await context.SaveChangesAsync();

            return user;
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Role,user.Role.ToString())
            };
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var TokenDescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                audience: configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
                );
            return new JwtSecurityTokenHandler().WriteToken(TokenDescriptor);
        }


    }
}
