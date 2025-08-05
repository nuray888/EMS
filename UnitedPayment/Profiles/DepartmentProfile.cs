using AutoMapper;
using UnitedPayment.Model;
using UnitedPayment.Model.DTOs;
using UnitedPayment.Model.DTOs.Requests;
using UnitedPayment.Model.DTOs.Responses;

namespace UnitedPayment.Profiles
{
    public class DepartmentProfile:Profile
    {
        public DepartmentProfile() {
            CreateMap<Department, DepartmentRequestDTO>().ReverseMap();
            CreateMap<Department, DepartmentResponseDTO>().ReverseMap();

        }
    }
}
