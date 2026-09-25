using Microsoft.EntityFrameworkCore;
using StudyHubAPI.Data;
using StudyHubAPI.Models.DTOs.Country;


namespace StudyHubAPI.Repositories
{
    public class CountryRepository
    {
        private readonly StudyHubDbContext _context;

        public CountryRepository(StudyHubDbContext context)
        {
            _context = context;
        }

        public async Task<List<CountrySummaryDto>> GetAllCountries()
        {
            return await _context.Countries.AsNoTracking().Select(c => new CountrySummaryDto
            {
                CountryID = c.CountryID,
                Name = c.CountryName
            }).ToListAsync();
        }



    }
}
