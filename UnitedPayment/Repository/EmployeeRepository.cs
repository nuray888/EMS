using Microsoft.EntityFrameworkCore;
using UnitedPayment.Model;
using UnitedPayment.Model.DTOs;

namespace UnitedPayment.Repository
{

    public interface IEmployeeRepository
    {
        List<Employee> EmployeesByDepartmentId(int departmentId);

    }
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext db;
        public EmployeeRepository(AppDbContext dbcontext)
        {
            db = dbcontext;
        }
        public List<Employee> EmployeesByDepartmentId(int departmentId)
        {
            return db.Employees
                     .Where(e => e.Departments.Any(d => d.Id == departmentId))
                     .ToList();
        }

    }
}

