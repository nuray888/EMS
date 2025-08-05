//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using UnitedPayment.Model;
//using UnitedPayment.Model.DTOs;
//using UnitedPayment.Model.DTOs.Requests;
//using UnitedPayment.Model.DTOs.Responses;
//using UnitedPayment.Repository;
//using UnitedPayment.Services;

//namespace UnitedPayment.Controllerr
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class ManagerController : ControllerBase
//    {

//        private readonly ManagerService _managerService;

//        public ManagerController(ManagerService managerService)
//        {
//            _managerService = managerService;
//        }

//        [HttpPost("add-employee")]
//        public async Task<ActionResult<string>> AddEmployeeToDepartment(int managerId, int employeeId)
//        {
//            var employee = await _managerService.AddEmployeeToDepartmentAsync(managerId, employeeId);
//            if (employee == null)
//                return "Employee not found";

//            return Ok(employee);
//        }

//        //[HttpPost("remove-employee")]
//        //public async Task<ActionResult<string>> RemoveEmployeeFromDepartment(int managerId, [FromBody] EmployeeRequestDTO request)
//        //{
//        //    var employee = await _managerService.RemoveEmployeeFromDepartmentAsync(managerId, request);
//        //    if (employee == null)
//        //        return BadRequest("Manager tapılmadı və ya department icazəsi yoxdur.");

//        //    return Ok(employee);
//        //}

//        [HttpPost]
//        public async Task<string> Create(ManagerRequestDto request)
//        {
//            return await _managerService.CreateAsync(request);
//        }

//        [HttpGet]
//        public async Task<List<ManagerResponseDto>> GetAll()
//        {
//            return await _managerService.getAll();
//        }
        

        

//    }
//}
