using System;

namespace HrManagement.Domain.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public DateTimeOffset ExpiresAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? RevokedAt { get; set; }
        public string? ReplacedByToken { get; set; }

        public bool IsActive => RevokedAt is null && DateTimeOffset.UtcNow <= ExpiresAt;
    }
}