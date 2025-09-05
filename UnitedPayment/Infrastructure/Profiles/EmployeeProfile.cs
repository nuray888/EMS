using AutoMapper;
using UnitedPayment.Model;
using UnitedPayment.Model.DTOs;

namespace UnitedPayment.Profiles
{
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile()
        {
            CreateMap<Employee, EmployeeRequestDTO>().ReverseMap();
            CreateMap<Employee, EmployeeResponseDTO>().ReverseMap();
        }
    }
}
