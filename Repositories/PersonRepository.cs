using Microsoft.EntityFrameworkCore;
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
 
        public async Task<bool> SoftDeletePersonByIdAsync(int personId)
        {
            int affectedRows = await _context.Person.Where(p => p.PersonID == personId)
                .ExecuteUpdateAsync(setters => setters.SetProperty(p => p.IsActive, false));

            return affectedRows > 0;
        }
        
        public async Task<PersonAuthDto?> GetPersonByEmail(string email)
        {
            return await _context.Person.AsNoTracking().Where(p => p.Email == email)
                .Select(p => new PersonAuthDto
             {
                PersonID = p.PersonID, Email = p.Email,
                PasswordHash = p.PasswordHash, FirstName = p.FirstName,
                LastName = p.LastName, Role = p.Role
             }).FirstOrDefaultAsync();
        }

        public async Task<int> SaveChangeAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> IsEmailTaken(string email)
        {
            return await _context.Person.Select(p => p.Email).AnyAsync(e => e == email);
        }

        public async Task<bool> IsPhoneNumberTaken(string phoneNumber)
        {
            return await _context.Person.Select(p => p.PhoneNumber).AnyAsync(p => p == phoneNumber);
        }

        public async Task<int> ChangeRole(int PersonID, PersonRole newRole)
        {
            return await _context.Person.Where(p => p.PersonID == PersonID)
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
                .Include(rt => rt.Person)
                .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);
        }

        public async Task UpdateRefreshTokenAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Update(refreshToken);
            await _context.SaveChangesAsync();
        }

    }
}
