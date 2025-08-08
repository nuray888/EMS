using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UnitedPayment.Model;
using UnitedPayment.Model.DTOs;
using UnitedPayment.Model.DTOs.Requests;
using UnitedPayment.Model.DTOs.Responses;
using UnitedPayment.Model.Enums;

namespace UnitedPayment.Services
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(RegisterRequest request);
        Task<TokenResponseDto?> LoginAsync(AuthDto request);
        Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request);
        Task<string> ChangePassword(int userId, ChangePasswordDto request);
    }
    public class AuthService(AppDbContext context, IConfiguration configuration) : IAuthService
    {
        public async Task<User> ValidateRefreshTokenAsync(int userId,string refreshToken)
        {
            var user = await context.Users.FindAsync(userId);
            if(user is null || user.RefreshToken !=refreshToken || user.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                return null;
            }
            return user;
        }
        public async Task<TokenResponseDto?> LoginAsync(AuthDto request)
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
            if ((DateTime.UtcNow - user.PasswordLastChangedTime).TotalDays > 30)
            {
                return null;
            }
            TokenResponseDto response = await CreateTokenResponse(user);

            return response;
        }

        private async Task<TokenResponseDto> CreateTokenResponse(User user)
        {
            return new TokenResponseDto()
            {
                AccessToken = CreateToken(user),
                RefreshToken = await GenerateAndSaveTokenAsync(user),
            };
        }

        public async Task<User?> RegisterAsync(RegisterRequest request)
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
            user.PasswordLastChangedTime = DateTime.UtcNow;
            context.Users.Add(user);
            await context.SaveChangesAsync();

            return user;
        }
        public async Task<string> ChangePassword(int userId,ChangePasswordDto request)
        {
            var user = await context.Users.FindAsync(userId);
            if (user == null)
            {
                return null;
            }
            var passwordHasher = new PasswordHasher<User>();

            if (passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.OldPassword) == PasswordVerificationResult.Failed)
            {
                return null;
            }
            var oneYearAgo= DateTime.UtcNow.AddYears(-1);
            var recentPasswords =await  context.PasswordHistories.Where(x => x.UserId == userId && x.ChangeDate >= oneYearAgo).ToListAsync();
            if (recentPasswords.Any(old =>
passwordHasher.VerifyHashedPassword(user, old.PasswordHash, request.NewPassword)
  == PasswordVerificationResult.Success))
            {
                return "This password has been used in the last year. Choose a new one.";
            }

            user.PasswordHash = passwordHasher.HashPassword(user, request.NewPassword);
            user.PasswordLastChangedTime=DateTime.UtcNow;
            var passwordHistory = new PasswordHistory()
            {
                UserId = user.Id,
                ChangeDate = DateTime.UtcNow,
                PasswordHash = user.PasswordHash
            };
            context.PasswordHistories.Add(passwordHistory);
            await context.SaveChangesAsync();


            return "Password changed";
        }


        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }


        private async Task<string> GenerateAndSaveTokenAsync(User user)
        {
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await context.SaveChangesAsync();
            return refreshToken;
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
        public async  Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request)
        {
            var user = await ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);
            if(user is null)
            {
                return null;
            }
            return await CreateTokenResponse(user);
           
        }

    }
}
