
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Security.Claims;
using UnitedPayment.Migrations;
using UnitedPayment.Model;
using UnitedPayment.Model.DTOs;
using UnitedPayment.Model.Enums;
using UnitedPayment.Repository;
using UnitedPayment.Services;

namespace UnitedPayment.Controllerr
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        readonly IEmployeeService service;
        readonly IRepository<User> userRepo;
        public EmployeeController(IEmployeeService service,IRepository<User> userRepo)
        {
            this.service = service;
            this.userRepo = userRepo;
        }

        [HttpGet]
        //[Authorize(Roles ="Admin")]
        public async Task<ActionResult<List<EmployeeResponseDTO>>> Employees()
        {
            var employees = await service.GetAllAsync();
            if (employees == null)
            {
                Log.Warning("Employee not found");
                return NotFound();
            }
            Log.Information("Employees =>{@employees}", employees);
            return Ok(employees);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeResponseDTO>> GetById([FromRoute] int id)
        {
            var employee = await service.GetByIdAsync(id);
            if (employee == null)
            {
                Log.Warning("Employee not found with given Id : {@id} not found", id);
                return NotFound();
            }
            Log.Information("Employee returned with id: {@id}", id);
            return Ok(employee);
        }

        [HttpGet("department/{departmentId}")]
        public async Task<ActionResult<List<EmployeeResponseDTO>>> getByDepartmentId(int departmentId)
        {
            return Ok(await service.GetByDepartmentId(departmentId));
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<string>> AddEmployee([FromBody] EmployeeRequestDTO employee)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user=(await userRepo.GetAll(x=>x.Email==email)).First();
            if (user.Role == UserRole.Manager)
            {
                if (user == null || user.DepartmentId != employee.DepartmentId) 
                {
                    return Forbid("Manager yalnız öz departmentinə işçi əlavə edə bilər.");
                }
            }
            var result = await service.CreateAsync(employee);
            var newUser= new User()
            {
                Email = employee.Email,
                Role = UserRole.Employee
            };
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee([FromBody] EmployeeRequestDTO employee, [FromRoute] int id)
        {
            var ExistingEmployee = await service.GetByIdAsync(id);
            if (ExistingEmployee == null)
            {
                Log.Warning("Employee not found with given id {@id}", id);
                return NotFound();
            }

            await service.UpdateAsync(employee, id);
            Log.Information("Employee updated with given id {@id}", id);
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> DeleteEmployee([FromRoute] int id)
        {
            var employee = await service.GetByIdAsync(id);
            if (employee == null)
            {
                Log.Warning("Employee not found with given id {@id}", id);
                return NotFound();
            }
            await service.DeleteAsync(id);
            Log.Information("Employee deleted with given id {@id}", id);
            return Ok();

        }

    }
}
