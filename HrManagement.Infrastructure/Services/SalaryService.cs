using System;
using System.Linq;
using System.Threading.Tasks;
using HrManagement.Domain.Entities;
using HrManagement.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HrManagement.Infrastructure.Services
{
    public interface ISalaryService
    {
        Task<int> PayAllActiveEmployeesAsync(string initiatedByUserId, string? notes = null);
    }

    public class SalaryService : ISalaryService
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public SalaryService(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<int> PayAllActiveEmployeesAsync(string initiatedByUserId, string? notes = null)
        {
            var activeEmployees = await _userManager.Users.Where(u => u.IsActive).ToListAsync();
            foreach (var emp in activeEmployees)
            {
                _db.SalaryPayments.Add(new SalaryPayment
                {
                    UserId = emp.Id,
                    Amount = emp.Salary,
                    PaidAt = DateTimeOffset.UtcNow,
                    Notes = notes
                });
            }
            return await _db.SaveChangesAsync();
        }
    }
}