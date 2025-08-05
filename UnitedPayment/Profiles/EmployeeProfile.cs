using AutoMapper;
using UnitedPayment.Model;
using UnitedPayment.Model.DTOs;

namespace UnitedPayment.Profiles
{
    public class EmployeeProfile:Profile
    {
       public EmployeeProfile() {
            CreateMap<Employee, EmployeeRequestDTO>();
            CreateMap<EmployeeRequestDTO, Employee>();
            CreateMap<Employee, EmployeeResponseDTO>().ReverseMap();
 
        }

       

    }
}
