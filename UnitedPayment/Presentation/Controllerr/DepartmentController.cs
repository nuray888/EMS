using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using UnitedPayment.Model.DTOs.Requests;
using UnitedPayment.Model.DTOs.Responses;
using UnitedPayment.Services;

namespace UnitedPayment.Controllerr
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        readonly IDepartmentService service;
        public DepartmentController(IDepartmentService service)
        {
            this.service = service;
        }
        /// <summary>
        /// Get all Departments
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<DepartmentResponseDTO>>> GetAll()
        {
            var departments = await service.GetAllAsync();
            if (departments == null)
            {
                return NotFound();
            }
            return departments;
        }

        /// <summary>
        /// Get department by id 
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DepartmentResponseDTO>> GetById([FromRoute] int id)
        {
            var department = await service.FindByIdAsync(id);
            if (department == null)
            {
                return NotFound();
            }
            return Ok(department);
        }


        /// <summary>
        /// Create new Department 
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DepartmentResponseDTO>> Create([FromBody] DepartmentRequestDTO request)
        {
            var result = await service.CreateAsync(request);

            return Ok(result);
        }
        /// <summary>
        /// Update department
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromBody] DepartmentRequestDTO request, [FromRoute] int id)
        {
            var existingDepartment = await service.FindByIdAsync(id);
            if (existingDepartment == null)
            {
                return NotFound();
            }
            var department = service.UpdateAsync(request, id);
            return Ok();
        }
        /// <summary>
        /// Delete department
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var department = await service.FindByIdAsync(id);
            if (department == null)
            {
                return NotFound();
            }
            return Ok();
        }


    }
}
