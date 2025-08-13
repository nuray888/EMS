using System;
using System.Linq;
using System.Threading.Tasks;
using HrManagement.Domain.Constants;
using HrManagement.Domain.Entities;
using HrManagement.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HrManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmployeesController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize(Roles = Roles.Admin + "," + Roles.DepartmentHead)]
        public async Task<IActionResult> Get()
        {
            if (User.IsInRole(Roles.Admin))
            {
                var all = await _userManager.Users.AsNoTracking().ToListAsync();
                return Ok(all);
            }

            var userId = _userManager.GetUserId(User);
            var deptId = await _db.Departments.Where(d => d.ManagerUserId == userId).Select(d => d.Id).FirstOrDefaultAsync();
            var mine = await _userManager.Users.AsNoTracking().Where(u => u.DepartmentId == deptId).ToListAsync();
            return Ok(mine);
        }

        public record CreateEmployeeRequest(string FirstName, string LastName, int Age, string Email, string? Address, decimal Salary, string? AdditionalInfo, System.Guid? DepartmentId);

        [HttpPost]
        [Authorize(Roles = Roles.Admin + "," + Roles.DepartmentHead)]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeRequest request)
        {
            var currentUserId = _userManager.GetUserId(User);
            Guid? departmentId = null;
            if (User.IsInRole(Roles.DepartmentHead))
            {
                departmentId = await _db.Departments.Where(d => d.ManagerUserId == currentUserId).Select(d => (Guid?)d.Id).FirstOrDefaultAsync();
            }
            else if (User.IsInRole(Roles.Admin))
            {
                departmentId = request.DepartmentId;
            }

            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Age = request.Age,
                Address = request.Address,
                Salary = request.Salary,
                AdditionalInfo = request.AdditionalInfo,
                DepartmentId = departmentId
            };

            var password = "Passw0rd!";
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded) return BadRequest(result.Errors);
            await _userManager.AddToRoleAsync(user, Roles.Employee);
            return Ok(user);
        }

        public record SetActiveRequest(bool IsActive);

        [HttpPatch("{id}/active")]
        [Authorize(Roles = Roles.Admin + "," + Roles.DepartmentHead)]
        public async Task<IActionResult> SetActive(string id, [FromBody] SetActiveRequest request)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            if (User.IsInRole(Roles.DepartmentHead))
            {
                var currentUserId = _userManager.GetUserId(User);
                var deptId = await _db.Departments.Where(d => d.ManagerUserId == currentUserId).Select(d => d.Id).FirstOrDefaultAsync();
                if (user.DepartmentId != deptId) return Forbid();
            }

            user.IsActive = request.IsActive;
            await _userManager.UpdateAsync(user);
            return NoContent();
        }
    }
}