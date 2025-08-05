//using AutoMapper;
//using Azure.Core;
//using System.Threading.Tasks;
//using UnitedPayment.Migrations;
//using UnitedPayment.Model;
//using UnitedPayment.Model.DTOs;
//using UnitedPayment.Model.DTOs.Requests;
//using UnitedPayment.Model.DTOs.Responses;
//using UnitedPayment.Repository;

//namespace UnitedPayment.Services
//{
//    public class ManagerService
//    {
//        private readonly IEmployeeRepository employeeRepository;
//        private readonly IManagerRepository managerRepository;
//        private readonly IMapper mapper;
//        private readonly IDepartmentRepository departmentRepository;



//        public ManagerService(IEmployeeRepository employeeRepository, IManagerRepository managerRepository,IMapper mapper,IDepartmentRepository departmentRepository)
//        {
//            this.employeeRepository = employeeRepository;
//            this.managerRepository = managerRepository;
//            this.mapper = mapper;
//            this.departmentRepository = departmentRepository;
//        }
//        public async Task<string> AddEmployeeToDepartmentAsync(int managerId, int employeeId)
//        {
//            var manager = await managerRepository.FindByIdAsync(managerId);
//            if (manager == null)
//                return "Couldn't find manager";

//            var employee = await employeeRepository.GetByIdAsync(employeeId);
//            if (employee == null)
//                return "Couldn't find employee";

//            // DepartmentId int olduğu üçün null yoxlaması lazım deyil,
//            // amma department obyektini ayrıca çəkmək lazımdır
//            var department = await departmentRepository.FindByIdAsync(manager.DepartmentId);
//            if (department == null)
//                return "Manager does not belong to any department";

//            // Əgər Employees listi null-dursa, yenisini yarad
//            if (department.Employees == null)
//                department.Employees = new List<Employee>();

//            // İşçini department-ə əlavə et
//            department.Employees.Add(employee);

//            // Department-ı güncəllə
//            await departmentRepository.UpdateAsync(department);

//            return "Employee is added to your department";
//        }





//        public async Task<string> CreateAsync(ManagerRequestDto request)
//        {
//            var manager = mapper.Map<Manager>(request);

//            // Department-ı DB-dən async şəkildə tap
//            var department = await departmentRepository.FindByIdAsync(manager.DepartmentId);
//            if (department == null)
//            {
//                return "Department mövcud deyil";
//            }

//            manager.Department = department;

//            await managerRepository.create(manager);

//            return "Manager uğurla yaradıldı";
//        }

//        public async Task<List<ManagerResponseDto>> getAll()
//        {
//            List<Manager> managers= await managerRepository.getAllAsync();
//            List<ManagerResponseDto> all= managers.Select(m => mapper.Map<ManagerResponseDto>(m)).ToList();
//            return all;
            
         
//        }

        
//    }
//}
