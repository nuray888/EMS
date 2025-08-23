using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UnitedPayment.Model;
using UnitedPayment.Model.Enums;
using UnitedPayment.Repository;
using UnitedPayment.Services;

namespace UnitedPayment.Controllerr
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController(IAuthService service,IRepository<Employee> employeeRepo) : ControllerBase
    {
        /// <summary>
        /// Change user's role 
        /// </summary>
        [Authorize(Roles ="Admin")]
        [HttpPut("user-role/{userId}")]
        public async Task<IActionResult> UpdateUserRole([FromRoute] int userId, UserRole roleName)
        {
            var user = await employeeRepo.FindByIdAsync(userId);
            user.UpdatedAt = DateTime.UtcNow;
            if (user == null)
            {
                return BadRequest("user not found");
            }
            await service.UpdateUserRoleAsync(userId, roleName);
            return Ok("User role updated");
        }
    }
}
