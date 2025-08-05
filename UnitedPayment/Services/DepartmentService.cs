using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Collections.Immutable;
using UnitedPayment.Model;
using UnitedPayment.Model.DTOs;
using UnitedPayment.Model.DTOs.Requests;
using UnitedPayment.Model.DTOs.Responses;
using UnitedPayment.Profiles;
using UnitedPayment.Repository;

namespace UnitedPayment.Services
{
    public interface IDepartmentService
    {
        Task<List<DepartmentResponseDTO>> GetAllAsync();
        Task<DepartmentResponseDTO?> FindByIdAsync(int id);
        Task<DepartmentResponseDTO> CreateAsync(DepartmentRequestDTO request);
        Task UpdateAsync(DepartmentRequestDTO request, int id);
        Task DeleteAsync(int id);


    }
    public class DepartmentService : IDepartmentService
    {
        private readonly IRepository<Department> repository;
        private readonly IMapper mapper;
        public DepartmentService(IRepository<Department> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }
        public async Task<DepartmentResponseDTO> CreateAsync(DepartmentRequestDTO request)
        {
            Department department = mapper.Map<Department>(request);
            await repository.AddAsync(department);
            await repository.SaveChangesAsync();
            return mapper.Map<DepartmentResponseDTO>(department);

        }

        public async Task DeleteAsync(int id)
        {
            await repository.DeleteAsync(id);

        }

        public async Task<DepartmentResponseDTO?> FindByIdAsync(int id)
        {
            var department = await repository.FindByIdAsync(id);
            if (department == null)
                return null;

            return mapper.Map<DepartmentResponseDTO>(department);
        }


        public async Task<List<DepartmentResponseDTO>> GetAllAsync()
        {
            List<Department> departments = await repository.GetAll();
            List<DepartmentResponseDTO> list = departments.Select(d => mapper.Map<DepartmentResponseDTO>(d)).ToList();
            return list;
        }

        public async Task UpdateAsync(DepartmentRequestDTO request, int id)
        {
            var existingDepartment = await repository.FindByIdAsync(id);
            mapper.Map(request, existingDepartment);
            repository.UpdateAsync(existingDepartment);
            await repository.SaveChangesAsync();

        }
    }
}
