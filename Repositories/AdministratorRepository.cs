using Microsoft.EntityFrameworkCore;
using StudyHubAPI.Data;
using StudyHubAPI.Models.DTOs.Admin;
using StudyHubAPI.Models.DTOs;
using StudyHubAPI.Models.Entities;
using StudyHubAPI.Models.Filter;

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

        public async Task<PagedResponse<AdminSummaryDto>> GetAllAdmins(AdminQueryFilter filter)
        {

            var query = _context.Administrators.AsNoTracking().Where(p => p.IsActive);

            if(!string.IsNullOrEmpty(filter.FullName))
            {
                query = query.Where(a => (a.FirstName + " " + a.LastName).Contains(filter.FullName));
            }  

            if(filter.HireDate != null)
            {
                query = query.Where(a => a.HireDate >= filter.HireDate.Value);
            }   

            int totalCount = await query.CountAsync();

            var admins = await query
                .OrderBy(a => a.PersonID) 
                .Skip((filter.pageNumber - 1) * filter.pageSize)         
                .Take(filter.pageSize)           
                .Select(a => new AdminSummaryDto 
                {
                    PersonID = a.PersonID,
                    FullName = a.FirstName + " " + a.LastName,
                    HireDate = a.HireDate
                }).ToListAsync();

            return new PagedResponse<AdminSummaryDto>(admins, totalCount, filter.pageNumber, filter.pageSize);
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
