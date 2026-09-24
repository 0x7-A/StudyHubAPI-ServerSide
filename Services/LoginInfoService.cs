using StudyHubAPI.Repositories;
using StudyHubAPI.Utils;
using StudyHubAPI.Models.DTOs.LoginInfo;
using StudyHubAPI.Models.Enums;
using StudyHubAPI.Models.Entities;


namespace StudyHubAPI.Services
{
    public class LoginInfoService
    {
        private readonly LoginInfoRepository _loginInfoRepository;

        public LoginInfoService(LoginInfoRepository loginInfoRepository)
        {
            _loginInfoRepository = loginInfoRepository;
        }


        private Task<LoginInfo?> _FindLoginInfo(int PersonID)
        {
            return _loginInfoRepository.GetLoginByPersonID(PersonID);
        }

        public async Task<ServiceResult<int>> AddLoginInfo(CreateLoginInfoDto dto)
        {
            if (await _loginInfoRepository.IsEmailTaken(dto.Email))
            {
                return ServiceResult<int>.Failure(ResultType.Conflict, "Email already exists.");
            }

            return ServiceResult<int>.Success(await _loginInfoRepository.AddLoginInfo(new LoginInfo
            {
                PersonID = dto.PersonID,
                Email = dto.Email,
                PasswordHash = PasswordHasher.HashPassword(dto.Password)
            }), ResultType.Ok);
        }


        public async Task<ServiceResult> UpdateLoginInfo(int PersonID, UpdateLoginInfoDto dto)
        {
             var loginInfo = await _FindLoginInfo(PersonID);
            if (loginInfo == null)
            {
                return ServiceResult.Failure(ResultType.NotFound, "Login info not found.");
            }

            if (!string.IsNullOrEmpty(dto.Email) && dto.Email != loginInfo.Email)
            {
                if (await _loginInfoRepository.IsEmailTaken(dto.Email))
                {
                    return ServiceResult.Failure(ResultType.Conflict, "Email already exists.");
                }
                loginInfo.Email = dto.Email;
            }

            if (!string.IsNullOrEmpty(dto.Password))
            {
                loginInfo.PasswordHash = PasswordHasher.HashPassword(dto.Password);
            }

            if (await _loginInfoRepository.SaveChangesAsync(loginInfo) > 0)
            {
                return ServiceResult.Success(ResultType.Ok);
            }

            return ServiceResult.Failure(ResultType.Failure, "Failed to update login info.");


        }

        public async Task<ServiceResult> PromptToAdmin(int PersonID)
        {
            var loginInfo = await _FindLoginInfo(PersonID);
            if (loginInfo == null)
            {
                return ServiceResult.Failure(ResultType.NotFound, "Login info not found.");
            }

            if(loginInfo.Role == PersonRole.Customer)
            {
               if(await _loginInfoRepository.PromptToAdmin(PersonID) > 0)
               {
                    return ServiceResult.Success(ResultType.NoContent);
               }
                
            }

            return ServiceResult.Failure(ResultType.BadRequest, "User is already an admin.");
        } 





    }
}
