using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UnitedPayment.Model;
using UnitedPayment.Model.DTOs;
using UnitedPayment.Model.Enums;
using UnitedPayment.Persistance.context;
using UnitedPayment.Repository;

namespace UnitedPayment.Services
{
    public interface IEmployeeService
    {
        Task<string> CreateAsync(EmployeeRequestDTO employeeRequest);
        Task<List<EmployeeResponseDTO>> GetAllAsync();
        Task<Employee> GetByIdAsync(int id);
        Task UpdateAsync(EmployeeRequestDTO employee, int id);
        Task DeleteAsync(int id);

        Task<List<EmployeeResponseDTO>> GetByDepartmentId(int departmentId);
        Task PaySalariesAsync();

        Task ActivateEmployeeAsync(int id);
        Task DeActivateEmployeeAsync(int id);
        Task AddEmployeeToDepartment(int id, int departmentId);
        Task<Employee?> SetUserSalaryAsync(int employeeId, int salary);

    }
    public class EmployeeService : IEmployeeService
    {
        private readonly IMapper mapper;
        private readonly IRepository<Employee> repository;
        private readonly IRepository<Department> departmentRepository;
        private readonly IEmployeeRepository employeeRepository;
        public readonly AppDbContext db;


        public EmployeeService(IMapper mapper, IRepository<Employee> repository, IRepository<Department> departmentRepository, IEmployeeRepository employeeRepository, AppDbContext db)
        {
            this.mapper = mapper;
            this.repository = repository;
            this.departmentRepository = departmentRepository;
            this.employeeRepository = employeeRepository;
            this.db = db;
        }

        public async Task<List<EmployeeResponseDTO>> GetAllAsync()
        {
            List<Employee> employees = await repository.GetAll();
            return employees.Select(e => mapper.Map<EmployeeResponseDTO>(e)).ToList();
        }


        public async Task<string> CreateAsync(EmployeeRequestDTO employeeRequest)
        {
            Employee employee = mapper.Map<Employee>(employeeRequest);
            await repository.AddAsync(employee);
            return "Employee created";
        }


        public async Task<List<EmployeeResponseDTO>> GetByDepartmentId(int departmentId)
        {
            var department = await departmentRepository.FindByIdAsync(departmentId);
            if (department == null)
            {
                return null;
            }
            List<Employee> employees = employeeRepository.EmployeesByDepartmentId(departmentId);
            return employees.Select(e => mapper.Map<EmployeeResponseDTO>(e)).ToList();

        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            var employee = await repository.FindByIdAsync(id);
            if (employee == null) return null;
            return employee;
        }

        public async Task UpdateAsync(EmployeeRequestDTO updatedEmployee, int id)
        {
            var employee = await repository.FindByIdAsync(id);
            if (employee == null) throw new Exception("Employee not found");
            employee.UpdatedAt = DateTime.UtcNow;
            mapper.Map(updatedEmployee, employee);
            repository.UpdateAsync(employee);
        }

        public async Task DeleteAsync(int id)
        {
            var employee=await repository.FindByIdAsync(id);
            if (employee != null)
            {
                employee.isDeleted = true;
                employee.isActive = false;
            }
            await db.SaveChangesAsync();

        }

        public async Task PaySalariesAsync()
        {
            var oneMonthAgo = DateTime.UtcNow.AddMonths(-1);

            await db.Employees
                .Where(e => e.isActive &&
                            !e.isDeleted &&
                            (e.LastPaidDate == null || e.LastPaidDate < oneMonthAgo))
                .ExecuteUpdateAsync(s => s
                    .SetProperty(e => e.LastPaidDate, DateTime.UtcNow)
                );
        }
        public async Task ActivateEmployeeAsync(int id)
        {
            var employee=await repository.FindByIdAsync(id);
            if(employee ==null || employee.Role != UserRole.Employee)
            {
                return;
            }
            employee.isActive = true;
            await repository.SaveChangesAsync();
        }


        public async Task DeActivateEmployeeAsync(int id)
        {
            var employee = await repository.FindByIdAsync(id);
            if (employee == null || employee.Role != UserRole.Employee)
            {
                return;
            }
            employee.isActive = false;
            await repository.SaveChangesAsync();
        }

        public async Task AddEmployeeToDepartment(int id,int departmentId)
        {
            var employee = await repository.FindByIdAsync(id);
            var department = await departmentRepository.FindByIdAsync(departmentId);
            employee.Departments.Add(department);
            await departmentRepository.SaveChangesAsync();
        }

        public async Task<Employee?> SetUserSalaryAsync(int employeeId, int salary)
        {
            var user = await repository.FindByIdAsync(employeeId);
            if (user == null) return null;
            if (!user.Salary.Equals(salary))
            {
                user.Salary = salary;
                await repository.SaveChangesAsync();
            }
            return user;
        }
    }

}
