using System.Threading.Tasks;
using HrManagement.Domain.Constants;
using HrManagement.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalaryController : ControllerBase
    {
        private readonly ISalaryService _salaryService;

        public SalaryController(ISalaryService salaryService)
        {
            _salaryService = salaryService;
        }

        [HttpPost("pay-all")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> PayAll()
        {
            var initiatedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "unknown";
            var count = await _salaryService.PayAllActiveEmployeesAsync(initiatedBy, "Monthly payout");
            return Ok(new { paid = count });
        }
    }
}