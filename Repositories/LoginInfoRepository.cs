using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using StudyHubAPI.Data;
using StudyHubAPI.Models.Entities;

namespace StudyHubAPI.Repositories
{
    public class LoginInfoRepository
    {
        private readonly StudyHubDbContext _context;

        public LoginInfoRepository(StudyHubDbContext context)
        {
            _context = context;
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
            return await _context.LoginInfos.Where(l => l.PersonID == PersonID).ExecuteUpdateAsync(setters => setters.SetProperty(p => p.Role, Models.Enums.PersonRole.Admin));
        }

    }
}
