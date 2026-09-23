using StudyHubAPI.Models.DTOs;
using StudyHubAPI.Models.DTOs.Admin;
using StudyHubAPI.Models.Entities;
using StudyHubAPI.Models.Enums;
using StudyHubAPI.Models.Filter;
using StudyHubAPI.Repositories;
using StudyHubAPI.Utils;

namespace StudyHubAPI.Services
{
    public class AdministratorService
    {
        private readonly AdministratorRepository _AdministratorRepository;
        private readonly PersonRepository _PersonRepository;

        public AdministratorService(AdministratorRepository administratorRepository, PersonRepository personRepository)
        {
            _AdministratorRepository = administratorRepository;
            _PersonRepository = personRepository;
        }

        public async Task<ServiceResult<int>> AddAdmin(CreateAdminDto dto)
        {
            if (await _PersonRepository.IsEmailTaken(dto.Email))
            {
                return ServiceResult<int>.Failure(ResultType.Conflict, "Email is already taken.");
            }
            if (await _PersonRepository.IsPhoneNumberTaken(dto.PhoneNumber))
            {
                return ServiceResult<int>.Failure(ResultType.Conflict, "Phone number is already taken.");
            }

            var NewAdmin = new Administrators
            {
                FirstName = dto.FirstName,
                MiddleName = dto.MiddleName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                PasswordHash =  PasswordHasher.HashPassword(dto.Password),
                Role = PersonRole.Admin
            };

            return  ServiceResult<int>.Success(await _AdministratorRepository.AddAdmin(NewAdmin), ResultType.NoContent);
        }

        public async Task<ServiceResult<AdminDetailsDto?>> GetAdminByID(int personID)
        {
            var Admin = await _AdministratorRepository.GetAdminByIdReadOnly(personID);

            if (Admin == null)
            {
                return ServiceResult<AdminDetailsDto?>.Failure(ResultType.NotFound, "Admin not found.");
            }

            return ServiceResult<AdminDetailsDto?>.Success(new AdminDetailsDto
            {
                PersonID = Admin.PersonID,
                FirstName = Admin.FirstName,
                LastName = Admin.LastName,
                Email = Admin.Email,
                PhoneNumber = Admin.PhoneNumber,
                HireDate = Admin.HireDate,
                Role = Admin.Role
            });
        }


        public Task<PagedResponse<AdminSummaryDto>> GetAllAdmins(AdminQueryFilter filter)
        {
            return _AdministratorRepository.GetAllAdmins(filter);
        }


        public async Task<ServiceResult<bool>> UpdateAdmin(int PersonID,UpdateAdminDto dto)
        {
            var Admin = await _AdministratorRepository.GetAdminByID(PersonID);

            if (Admin == null)
            {
                return ServiceResult<bool>.Failure(ResultType.NotFound, "Admin not found.");
            }
            if (!string.IsNullOrEmpty(dto.FirstName))
            {
                Admin.FirstName = dto.FirstName;
            }
            if (!string.IsNullOrEmpty(dto.LastName))
            {
                Admin.LastName = dto.LastName;
            }
            if (!string.IsNullOrEmpty(dto.MiddleName))
            {
                Admin.MiddleName = dto.MiddleName;
            }

            if (!string.IsNullOrEmpty(dto.PhoneNumber))
            {
                if(await _PersonRepository.IsPhoneNumberTaken(dto.PhoneNumber))
                {
                    return ServiceResult<bool>.Failure(ResultType.Conflict, "Phone number is already taken.");
                }

                Admin.PhoneNumber = dto.PhoneNumber;
            }

            return ServiceResult<bool>.Success(await _PersonRepository.SaveChangeAsync() > 0);
        }


        public Task<bool> DeleteAdminByID(int personID)
        {
           return _PersonRepository.SoftDeletePersonByIdAsync(personID);
        }


        // maybe I will need it later. 
        //private async Task<bool> PromoteToAdmin(int PersonID)
        //{
        //    var result = await _PersonRepository.ChangeRole(PersonID, 2);
        //    return result > 0;
        //
    }
}
