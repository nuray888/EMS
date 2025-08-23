
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Security.Claims;
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
        readonly IRepository<Employee> userRepo;
        readonly AppDbContext db;

        public EmployeeController(IEmployeeService service, IRepository<Employee> userRepo,AppDbContext db)
        {
            this.service = service;
            this.userRepo = userRepo;
            this.db = db;
        }
        /// <summary>
        /// Get all employees
        /// </summary>    
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<EmployeeResponseDTO>>> GetAllEmployees()
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
        /// <summary>
        /// Get employee by id 
        /// </summary>
        /// <param name="id"></param>
       
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeResponseDTO>> GetById([FromRoute] int id)
        {
            var employee = await service.GetByIdAsync(id);
            Log.Information("employee's role is : {@role}", employee.Role);
            if (employee == null)
            {
                Log.Warning("Employee not found with given Id : {@id} not found", id);
                return NotFound();
            }
            Log.Information("Employee returned with id: {@id}", id);
            return Ok(employee);
        }
        /// <summary>
        /// Adds an employee to a department.
        /// </summary>
        [HttpPost("add-employee/{departmentId}")]
        [Authorize(Roles ="Admin,Manager")]
        public async Task<IActionResult> AddEmployeeToDepartment([FromQuery] int employeeId, [FromRoute] int departmentId)
        {
            var employee=await service.GetByIdAsync(employeeId);
            if (employee == null)
            {
                Log.Warning("User is not exist with : {EmployeeId}", employeeId);
                return BadRequest($"Employee with id {employeeId} does not exist.");
            }

            if (employee.Role != UserRole.Employee)
            {
                Log.Warning("Attempt to add employee with id: {EmployeeId} but role is {Role}", employeeId, employee.Role);
                return BadRequest("The selected user is not an employee.");
            }
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = (await userRepo.GetAll(x => x.Email == email)).First();
            Log.Information("User adding to department attempt from  : {@surname} {@name}", user.Name,user.Surname);
            user.Departments=db.Entry(user).Collection(u => u.Departments).Query().ToHashSet();
            if (user.Role == UserRole.Manager)
            {
                var hasAccess = user.Departments.Any(d => d.Id == departmentId);
                if (!hasAccess)
                {
                    Log.Warning("Manager {userId} attempted to add employee to another department {departmentId}", user.Id, departmentId);
                    return Forbid("Managers can only add employees to their own departments.");
                }
            }

            Log.Information("Employee with id : {@id} attempt to add to department with departmentId : {@departmentId}", employeeId, departmentId);
            await service.AddEmployeeToDepartment(employeeId, departmentId);
            return Ok("Employee added to department");
             
        }
        /// <summary>
        /// Get employees by department id 
        /// </summary>
        [Authorize]
        [HttpGet("department/{departmentId}")]
        public async Task<ActionResult<List<EmployeeResponseDTO>>> getByDepartmentId(int departmentId)
        {
            return Ok(await service.GetByDepartmentId(departmentId));
        }

        /// <summary>
        /// Update employee
        /// </summary>
        
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
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
        /// <summary>
        /// Delete employee
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteEmployee([FromRoute] int id)
        {

            var employee = await service.GetByIdAsync(id);
            if (employee == null || employee.Role==UserRole.Admin)
            {
                Log.Warning("Employee not found with given id {@id}", id);
                return NotFound();
            }
            await service.DeleteAsync(id);
            Log.Information("Employee deleted with given id {@id}", id);
            return Ok();

        }
        /// <summary>
        /// Activates an employee by the given ID.
        /// </summary>
        [HttpPatch("activate/{id}")]
        [Authorize(Roles ="Admin,Manager")]
        public async Task<IActionResult> ActivateEmployee([FromRoute] int id)
        {
            await service.ActivateEmployeeAsync(id);
            return Ok();
        }

        /// <summary>
        /// Deactivates an employee by the given ID.
        /// </summary>
        [HttpPatch("deactivate/{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeActivate([FromRoute] int id)
        {
            await service.DeActivateEmployeeAsync(id);
            return Ok();
        }

        /// <summary>
        /// Pay employees salary 
        /// </summary>
        [HttpPost("pay-salary")]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> PaySalaries()
        {
            await service.PaySalariesAsync();
            return Ok();
        }
        [HttpPatch("set-salary")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> SetSalary([FromQuery] int employeeId, [FromQuery] int salary)
        {
            var employee = await service.SetUserSalaryAsync(employeeId,salary);
            if (employee == null) return NotFound("Employee not found");
            return Ok(employee);
        }




    }
}
