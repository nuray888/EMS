namespace UnitedPayment.Model.DTOs.Responses
{
    public class DepartmentResponseDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }

    }
}
