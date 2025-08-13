using System;

namespace HrManagement.Domain.Entities
{
    public class SalaryPayment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTimeOffset PaidAt { get; set; } = DateTimeOffset.UtcNow;
        public string? Notes { get; set; }
    }
}