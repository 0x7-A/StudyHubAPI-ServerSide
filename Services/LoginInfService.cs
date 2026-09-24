using StudyHubAPI.Repositories;
using StudyHubAPI.Utils;
using StudyHubAPI.Models.DTOs.LoginInfo;
using StudyHubAPI.Models.Entities;


namespace StudyHubAPI.Services
{
    public class LoginInfService
    {
        private readonly LoginInfoRepository _loginInfoRepository;

        public LoginInfService(LoginInfoRepository loginInfoRepository)
        {
            _loginInfoRepository = loginInfoRepository;
        }

        public async Task<ServiceResult<int>> AddLoginInfo(CreateLoginInfoDto dto)
        {
            if(await _loginInfoRepository.IsEmailTaken(dto.Email))
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
    }
}
