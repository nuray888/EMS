using System.ComponentModel.DataAnnotations;
using UnitedPayment.Model.Enums;

namespace UnitedPayment.Model
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        public UserRole Role { get; set; } = UserRole.Employee;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }


        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }

    }
}
