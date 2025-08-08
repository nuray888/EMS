using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using UnitedPayment.Model;
using UnitedPayment.Model.DTOs;
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

    }
    public class EmployeeService : IEmployeeService
    {
        readonly IMapper mapper;
        readonly IRepository<Employee> repository;
        readonly IRepository<Department> departmentRepository;
        readonly IEmployeeRepository employeeRepository;


        public EmployeeService(IRepository<Employee> _repository, IMapper _mapper, IRepository<Department> departmentRepository, IEmployeeRepository employeeRepository)
        {
            repository = _repository;
            this.departmentRepository = departmentRepository;
            mapper = _mapper;
            this.employeeRepository = employeeRepository;
        }

        public async Task<string> CreateAsync(EmployeeRequestDTO employeeRequest)
        {
            Employee employee = mapper.Map<Employee>(employeeRequest);
            //HashSet<Department> allById=await departmentRepository.findByIds(employeeRequest.DepartmentIds);
            //employee.Departments = allById;
            await repository.AddAsync(employee);
            return "Employee created";
        }

        public async Task DeleteAsync(int id)
        {
            await repository.DeleteAsync(id);

        }

        public async Task<List<EmployeeResponseDTO>> GetAllAsync()
        {
            List<Employee> employees = await repository.GetAll();
            return employees.Select(e => mapper.Map<EmployeeResponseDTO>(e)).ToList();
        }

        public async Task<List<EmployeeResponseDTO>> GetByDepartmentId(int departmentId)
        {
            var department = await departmentRepository.FindByIdAsync(departmentId);
            if (department == null)
            {
                return null;
            }
            List<Employee> employees=employeeRepository.EmployeesByDepartmentId(departmentId);
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

            mapper.Map(updatedEmployee, employee);
            repository.UpdateAsync(employee);
        }
    }
}
