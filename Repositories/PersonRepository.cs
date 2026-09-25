using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using StudyHubAPI.Data;
using StudyHubAPI.Models.DTOs.Person;
using StudyHubAPI.Models.Entities;
using StudyHubAPI.Models.Enums;

namespace StudyHubAPI.Repositories
{
    public class PersonRepository
    {
        private readonly StudyHubDbContext _context;

        public PersonRepository(StudyHubDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangeAsync()
        {
            return await _context.SaveChangesAsync();
        }


        public async Task<bool> IsPhoneNumberTaken(string phoneNumber)
        {
            return await _context.LoginInfos.Select(p => p.PhoneNumber).AnyAsync(p => p == phoneNumber);
        }

        public async Task<int> ChangeIsActive(int PersonID)
        {
            //not every person has login info so we can't udpdate them in one step.
            return  await _context.Person
            .Where(p => p.PersonID == PersonID)
            .ExecuteUpdateAsync(setters => setters.SetProperty(p => p.IsActive, false));
        }

        public async Task<int> ChangeRole(int PersonID, PersonRole newRole)
        {
            return await _context.LoginInfos
            .Where(p => p.PersonID == PersonID)
            .ExecuteUpdateAsync(setters => setters.SetProperty(p => p.Role, newRole));
        }


        public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task<RefreshToken?> GetRefreshTokenByHashAsync(string tokenHash)
        {
            return await _context.RefreshTokens
                .Include(rt => rt.loginfo)
                .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);
        }

        public async Task UpdateRefreshTokenAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Update(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
          return  await _context.Database.BeginTransactionAsync();
        }



    }
}
