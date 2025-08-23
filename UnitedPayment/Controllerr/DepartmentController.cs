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
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<DepartmentResponseDTO>>> GetAll()
        {
            var departments = await service.GetAllAsync();
            if (departments == null)
            {
                Log.Warning("There is no department");
                return NotFound();
            }
            Log.Information("Departments => {@departments}", departments);
            return departments;
        }

        /// <summary>
        /// Get department by id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DepartmentResponseDTO>> GetById([FromRoute] int id)
        {
            var department = await service.FindByIdAsync(id);
            if (department == null)
            {
                Log.Warning("Department not found with given id {@id}", id);
                return NotFound();
            }
            Log.Information("Department returned with given id {@id} => {@department}", id, department);
            return Ok(department);
        }


        /// <summary>
        /// Create new Department 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
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
        /// <param name="request"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromBody] DepartmentRequestDTO request, [FromRoute] int id)
        {
            var existingDepartment = await service.FindByIdAsync(id);
            if (existingDepartment == null)
            {
                Log.Warning("There is no department with given id {@id}", id);
                return NotFound();
            }
            var department = service.UpdateAsync(request, id);
            Log.Information("Department is updated with given id {@id} =>{@department}", id, department);
            return Ok();
        }
        /// <summary>
        /// Delete department
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var department = await service.FindByIdAsync(id);
            if (department == null)
            {
                Log.Warning("Department not found with given id :{@id}", id);
                return NotFound();
            }
            Log.Information("Department deleted with given id :{@id}", id);
            return Ok();
        }


    }
}
