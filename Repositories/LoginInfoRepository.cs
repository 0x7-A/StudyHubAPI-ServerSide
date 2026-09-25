using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using StudyHubAPI.Data;
using StudyHubAPI.Models.DTOs.Person;
using StudyHubAPI.Models.Entities;
using StudyHubAPI.Models.Enums;

namespace StudyHubAPI.Repositories
{
    public class LoginInfoRepository
    {
        private readonly StudyHubDbContext _context;

        public LoginInfoRepository(StudyHubDbContext context)
        {
            _context = context;
        }

        public async Task<PersonAuthDto?> GetLoginInfoByEmail(string email)
        {
            return await _context.LoginInfos.AsNoTracking().Where(p => p.Email == email && p.Role != PersonRole.None)
                .Select(p => new PersonAuthDto
                {
                    PersonID = p.PersonID,
                    Email = p.Email,
                    PasswordHash = p.PasswordHash,
                    Role = p.Role
                }).FirstOrDefaultAsync();
        }
        public async Task<int> AddLoginInfo(LoginInfo loginInfo)
        {
            _context.Add(loginInfo);
            await _context.SaveChangesAsync();
            return loginInfo.LoginID;
        }

        public async Task<bool> IsEmailTaken(string email)
        {
            return await _context.LoginInfos.Select(p => p.Email).AnyAsync(e => e == email);
        }

        public async Task<LoginInfo?> GetLoginByPersonID(int PersonID)
        {
            return await _context.LoginInfos.SingleOrDefaultAsync(p => p.PersonID == PersonID);
        }

        public async Task<int> SaveChangesAsync(LoginInfo loginInfo)
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<int> PromptToAdmin(int PersonID)
        {
            return await _context.LoginInfos.Where(l => l.PersonID == PersonID).ExecuteUpdateAsync(setters => setters.SetProperty(p => p.Role, PersonRole.Admin));
        }

        public async Task<int> ResetPersonRoleToNone(int PersonID)
        {
            return await _context.LoginInfos.Where(l => l.PersonID == PersonID).ExecuteUpdateAsync(setters => setters.SetProperty(p => p.Role, PersonRole.None));
        }

    }
}
