using System;
using System.Collections.Generic;

namespace HrManagement.Domain.Entities
{
    public class Department
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string? AdditionalInfo { get; set; }

        public ICollection<ApplicationUser> Employees { get; set; } = new List<ApplicationUser>();
        public string? ManagerUserId { get; set; }
    }
}