using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;
using UnitedPayment.Model.Enums;

namespace UnitedPayment.Model
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public int Age { get; set; } 
        public string Email { get; set; } = null!;
        public string Address { get; set; } = null!;
        public int Salary { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public bool isActive { get; set; } = true;

        public bool isDeleted { get; set; } = false;

        public bool isVerified { get; set; } = false;
        
        public HashSet<Department> Departments { get; set; } = new HashSet<Department>();
        public DateTime? LastPaidDate { get; set; }
       
        public UserRole Role { get; set; } 

        [JsonIgnore]
        public string? RefreshToken { get; set; }
        [JsonIgnore]
        public DateTime? RefreshTokenExpiryTime { get; set; }
        [JsonIgnore]
        public DateTime PasswordLastChangedTime { get; set; }

        public string? VerificationToken { get; set; } 

    }
}



