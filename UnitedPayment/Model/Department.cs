using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace UnitedPayment.Model
{
    public class Department
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();

        public User? manager { get; set; }
    }
}

