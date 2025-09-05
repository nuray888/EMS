using UnitedPayment.Model.DTOs.Responses;

namespace UnitedPayment.Model.DTOs
{
    public class EmployeeResponseDTO
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public int Age { get; set; }
        public string Email { get; set; } = null!;
        public string Address { get; set; } = null!;
        public int Salary { get; set; }

    }
}
