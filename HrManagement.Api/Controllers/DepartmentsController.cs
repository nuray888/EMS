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
    public class DepartmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public DepartmentsController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
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
                var all = await _db.Departments.AsNoTracking().ToListAsync();
                return Ok(all);
            }
            var userId = _userManager.GetUserId(User);
            var my = await _db.Departments.AsNoTracking().Where(d => d.ManagerUserId == userId).ToListAsync();
            return Ok(my);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Create([FromBody] Department request)
        {
            request.Id = Guid.NewGuid();
            _db.Departments.Add(request);
            await _db.SaveChangesAsync();
            return Ok(request);
        }
    }
}