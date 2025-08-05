using Microsoft.EntityFrameworkCore;
using UnitedPayment.Model;
using UnitedPayment.Model.DTOs.Requests;

namespace UnitedPayment.Repository
{
    public interface IDepartmentRepository
    {

        //Task<HashSet<Department>> findByIds(List<int> departmentIds);


    }
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly AppDbContext db;
        public DepartmentRepository(AppDbContext _db)
        {
            db = _db;
        }

        public async Task<HashSet<Department>> findByIds(List<int> departmentIds)
        {
            var list = await db.Department.Where(d => departmentIds.Contains(d.Id)).ToHashSetAsync();
            return list;

        }


    }
}
