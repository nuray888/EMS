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
using UnitedPayment.Model.DTOs.Requests;
using UnitedPayment.Model.DTOs.Responses;
using UnitedPayment.Model.Enums;
using UnitedPayment.Repository;

namespace UnitedPayment.Services
{
    public interface IAuthService
    {
        Task<Employee?> RegisterAsync(RegisterRequest request);
        Task<TokenResponseDto?> LoginAsync(AuthDto request);
        Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request);
        Task<string> ChangePassword(int userId, ChangePasswordDto request);
        Task UpdateUserRoleAsync(int userId, UserRole role);
    }
    public class AuthService(AppDbContext context, IConfiguration configuration, IRepository<Employee> userRepo) : IAuthService
    {
        public async Task<Employee> ValidateRefreshTokenAsync(int userId, string refreshToken)
        {
            var user = await context.Employees.FindAsync(userId);
            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                return null;
            }
            return user;
        }
        public async Task<TokenResponseDto?> LoginAsync(AuthDto request)
        {
            var user = await context.Employees.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                return null;
            }
            if (user.isDeleted || !user.isActive)
            {
                return null;
            }

            if (new PasswordHasher<Employee>().VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
            {
                return null;
            }
            if ((DateTime.UtcNow - user.PasswordLastChangedTime).TotalDays > 30)
            {
                return null;
            }
            TokenResponseDto response = await CreateTokenResponse(user);

            return response;
        }

        private async Task<TokenResponseDto> CreateTokenResponse(Employee user)
        {
            return new TokenResponseDto()
            {
                AccessToken = CreateToken(user),
                RefreshToken = await GenerateAndSaveTokenAsync(user),
            };
        }

        public async Task<Employee?> RegisterAsync(RegisterRequest request)
        {
            if (await context.Employees.AnyAsync(u => u.Email == request.Email))
            {
                return null;
            }
            var employee = new Employee(); 
            employee.Email = request.Email;
            employee.Name = request.Name;
            employee.Surname = request.Surname;
            employee.VerificationToken = CreateRandomToken();
            var hashedPassword = new PasswordHasher<Employee>().HashPassword(employee, request.Password);
            employee.PasswordHash = hashedPassword;
            employee.PasswordLastChangedTime = DateTime.UtcNow;
            employee.Age = request.Age;
            employee.Address = request.Address;
            employee.CreatedAt = DateTime.UtcNow;
            employee.Role = UserRole.Employee;
            employee.isDeleted = false;
            employee.isVerified = false;
            employee.isActive = true;
            context.Employees.Add(employee);
            await context.SaveChangesAsync();
            var passwordHistory = new PasswordHistory()
            {
                EmployeeId = employee.Id,
                ChangeDate = DateTime.UtcNow,
                PasswordHash = employee.PasswordHash
            };
            context.PasswordHistories.Add(passwordHistory);
            await context.SaveChangesAsync();



            return employee;
        }

        private string CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }

        public async Task<string> ChangePassword(int employeeId, ChangePasswordDto request)
        {
            var user = await context.Employees.FindAsync(employeeId);
            if (user == null)
            {
                return null;
            }
            var passwordHasher = new PasswordHasher<Employee>();

            if (passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.OldPassword) == PasswordVerificationResult.Failed)
            {
                return null;
            }
            var oneYearAgo = DateTime.UtcNow.AddYears(-1);
            var recentPasswords = await context.PasswordHistories.Where(x => x.EmployeeId == employeeId && x.ChangeDate >= oneYearAgo).ToListAsync();
            if (recentPasswords.Any(old =>
passwordHasher.VerifyHashedPassword(user, old.PasswordHash, request.NewPassword)
  == PasswordVerificationResult.Success))
            {
                return "This password has been used in the last year. Choose a new one.";
            }

            user.PasswordHash = passwordHasher.HashPassword(user, request.NewPassword);
            user.PasswordLastChangedTime = DateTime.UtcNow;
            var passwordHistory = new PasswordHistory()
            {
                EmployeeId = employeeId,
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


        private async Task<string> GenerateAndSaveTokenAsync(Employee user)
        {
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await context.SaveChangesAsync();
            return refreshToken;
        }

        private string CreateToken(Employee user)
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
        public async Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request)
        {
            var user = await ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);
            if (user is null)
            {
                return null;
            }
            return await CreateTokenResponse(user);

        }
        public async Task UpdateUserRoleAsync(int userId, UserRole role)
        {
            Employee user = await userRepo.FindByIdAsync(userId);
            if (user == null)
            {
                return;
            }
            user.Role = role;
            userRepo.UpdateAsync(user);
            await userRepo.SaveChangesAsync();

        }


    }
}
