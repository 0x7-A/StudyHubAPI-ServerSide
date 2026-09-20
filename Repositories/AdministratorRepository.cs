using Microsoft.EntityFrameworkCore;
using StudyHubAPI.Data;
using StudyHubAPI.Models.DTOs.Admin;
using StudyHubAPI.Models.Entities;

namespace StudyHubAPI.Repositories
{
    public class AdministratorRepository
    {

        private readonly StudyHubDbContext _context;


        public AdministratorRepository(StudyHubDbContext context)
        {
            _context = context;
        }

        public async Task<Administrators?> GetAdminByID(int personID)
        {
            return await _context.Administrators
            .SingleOrDefaultAsync(p => p.PersonID == personID && p.IsActive);
        }

        public async Task<Administrators?> GetAdminByIdReadOnly(int personID)
        {
            return await _context.Administrators.AsNoTracking().SingleOrDefaultAsync(p => p.PersonID == personID && p.IsActive);
        }

        public async Task<List<AdminSummaryDto>> GetAllAdmins(int pageNumber, int pageSize)
        {
            int rowsToSkip = (pageNumber - 1) * pageSize;

            return await _context.Administrators
                .AsNoTracking().Where(p => p.IsActive)
                .OrderBy(a => a.PersonID) 
                .Skip(rowsToSkip)         
                .Take(pageSize)           
                .Select(a => new AdminSummaryDto 
                {
                    PersonID = a.PersonID,
                    FullName = a.FirstName + " " + a.LastName,
                    HireDate = a.HireDate
                }).ToListAsync();

        }

        public async Task<int> AddAdmin(Administrators NewAdmin)
        {
            _context.Administrators.Add(NewAdmin);
            await _context.SaveChangesAsync();

            return  NewAdmin.PersonID;
        }

        public async Task<int> DeleteAdmin(int AdminID)
        {
           return await _context.Administrators.Where(a => a.PersonID == AdminID).ExecuteDeleteAsync();
        }


    }
}
