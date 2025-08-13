using System;
using System.Linq;
using System.Threading.Tasks;
using HrManagement.Domain.Entities;
using HrManagement.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HrManagement.Infrastructure.Identity
{
    public interface IAuthService
    {
        Task<TokenResult> LoginAsync(string email, string password);
        Task<TokenResult> RefreshAsync(string refreshToken);
        Task RequirePasswordChangeIfNeeded(ApplicationUser user);
        Task ChangePasswordAsync(string userId, string currentPassword, string newPassword);
    }

    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _db;
        private readonly IJwtTokenService _tokenService;
        private readonly IPasswordPolicyService _passwordPolicyService;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext db,
            IJwtTokenService tokenService,
            IPasswordPolicyService passwordPolicyService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
            _tokenService = tokenService;
            _passwordPolicyService = passwordPolicyService;
        }

        public async Task<TokenResult> LoginAsync(string email, string password)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) throw new UnauthorizedAccessException("Invalid credentials");

            var check = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
            if (!check.Succeeded) throw new UnauthorizedAccessException("Invalid credentials");

            await RequirePasswordChangeIfNeeded(user);

            var roles = await _userManager.GetRolesAsync(user);
            var tokens = _tokenService.GenerateTokens(user, roles);

            await SaveRefreshTokenAsync(user.Id, tokens.RefreshToken, tokens.RefreshExpiresAt);

            return tokens;
        }

        public async Task<TokenResult> RefreshAsync(string refreshToken)
        {
            var token = await _db.RefreshTokens.FirstOrDefaultAsync(t => t.Token == refreshToken && t.RevokedAt == null);
            if (token == null || !token.IsActive) throw new UnauthorizedAccessException("Invalid refresh token");

            var user = await _userManager.FindByIdAsync(token.UserId);
            if (user == null) throw new UnauthorizedAccessException("Invalid refresh token");

            var roles = await _userManager.GetRolesAsync(user);
            var tokens = _tokenService.GenerateTokens(user, roles);

            token.RevokedAt = DateTimeOffset.UtcNow;
            token.ReplacedByToken = tokens.RefreshToken;
            await SaveRefreshTokenAsync(user.Id, tokens.RefreshToken, tokens.RefreshExpiresAt);
            await _db.SaveChangesAsync();

            return tokens;
        }

        public async Task RequirePasswordChangeIfNeeded(ApplicationUser user)
        {
            if (_passwordPolicyService.IsPasswordChangeRequired(user))
            {
                throw new InvalidOperationException("Password change required");
            }

            var oneYearAgo = DateTimeOffset.UtcNow.AddYears(-1);
            var recentReuse = await _db.PasswordHistories
                .Where(p => p.UserId == user.Id && p.CreatedAt >= oneYearAgo)
                .OrderByDescending(p => p.CreatedAt)
                .FirstOrDefaultAsync();
            _ = recentReuse; // Place-holder for extended checks depending on flow
        }

        public async Task ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId) ?? throw new InvalidOperationException("User not found");

            var oneYearAgo = DateTimeOffset.UtcNow.AddYears(-1);
            var histories = await _db.PasswordHistories
                .Where(p => p.UserId == user.Id && p.CreatedAt >= oneYearAgo)
                .ToListAsync();

            foreach (var history in histories)
            {
                var verify = _userManager.PasswordHasher.VerifyHashedPassword(user, history.PasswordHash, newPassword);
                if (verify != PasswordVerificationResult.Failed)
                {
                    throw new InvalidOperationException("This password was used within the last year.");
                }
            }

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!result.Succeeded)
            {
                var error = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException(error);
            }

            user.LastPasswordChangeAt = DateTimeOffset.UtcNow;
            await _userManager.UpdateAsync(user);

            _db.PasswordHistories.Add(new PasswordHistory
            {
                UserId = user.Id,
                PasswordHash = user.PasswordHash ?? string.Empty,
                CreatedAt = DateTimeOffset.UtcNow
            });

            await _db.SaveChangesAsync();
        }

        private async Task SaveRefreshTokenAsync(string userId, string refreshToken, DateTimeOffset expiresAt)
        {
            _db.RefreshTokens.Add(new RefreshToken
            {
                UserId = userId,
                Token = refreshToken,
                ExpiresAt = expiresAt
            });
            await _db.SaveChangesAsync();
        }
    }
}