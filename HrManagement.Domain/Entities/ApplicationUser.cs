using System;
using Microsoft.AspNetCore.Identity;

namespace HrManagement.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int Age { get; set; }
        public string? Address { get; set; }
        public decimal Salary { get; set; }
        public string? AdditionalInfo { get; set; }
        public bool IsActive { get; set; } = true;

        public Guid? DepartmentId { get; set; }
        public Department? Department { get; set; }

        public DateTimeOffset LastPasswordChangeAt { get; set; } = DateTimeOffset.UtcNow;
    }
}