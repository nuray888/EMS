using Microsoft.AspNetCore.SignalR;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using UnitedPayment.Model.Enums;

namespace UnitedPayment.Model
{
    public class Employee
    {

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public int Age { get; set; }
        public string Email { get; set; } = null!;
        public string Address { get; set; } = null!;
        public int Salary { get; set; }

        public bool isActive { get; set; } = true;

        public HashSet<Department>? Departments { get; set; }

        public UserRole Role { get; set; } = UserRole.Employee;

        public int? UserId { get; set; }
        public User? User { get; set; } = null!;

    }
}



