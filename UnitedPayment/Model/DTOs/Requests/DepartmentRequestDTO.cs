namespace UnitedPayment.Model.DTOs.Requests
{
    public class DepartmentRequestDTO
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public List<int>? ManagerIds { get; set; } = new List<int>();
    }
}
