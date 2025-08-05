using System.ComponentModel.DataAnnotations;

namespace UnitedPayment.Model.DTOs
{
    public class EmployeeRequestDTO
    {
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public required int Age { get; set; }
        public required string Email { get; set; }
        public string? Address { get; set; } = null!;
        public required int Salary { get; set; }
        public string? AdditionalInfo { get; set; }

        public int DepartmentId {  get; set; }

    }
}
