using Microsoft.EntityFrameworkCore;
using StudyHubAPI.Data;
using StudyHubAPI.Models.DTOs;
using StudyHubAPI.Models.DTOs.Customer;
using StudyHubAPI.Models.Entities;
using StudyHubAPI.Models.Filter;

namespace StudyHubAPI.Repositories
{
    public class CustomerRepository
    {
        private readonly StudyHubDbContext _context;

        public CustomerRepository(StudyHubDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddCustomer(Customers NewCustomer)
        {
            _context.Customers.Add(NewCustomer);
            await _context.SaveChangesAsync();

            return NewCustomer.PersonID;
        }

        public async Task<Customers?> GetCustomerByID(int personID)
        {
            return await _context.Customers.SingleOrDefaultAsync(p => p.PersonID  == personID && p.IsActive);
        }

        public async Task<Customers?> GetCustomerByIdReadOnly(int personID)
        {
            return await _context.Customers.AsNoTracking().SingleOrDefaultAsync( p => p.PersonID == personID && p.IsActive);
        }

        public async Task<PagedResponse<CustomerSummaryDto>> GetAllCustomer(CustomerQueryFilter filter)
        {
            var query = _context.Customers.AsNoTracking().Where(p => p.IsActive);

            if(!string.IsNullOrEmpty(filter.FullName))
            {
                query = query.Where(p => (p.FirstName + " " + p.LastName).Contains(filter.FullName));
            }

            if (filter.RegisteredAt.HasValue)
            {
                query = query.Where(p => p.RegisteredAt >= filter.RegisteredAt.Value);
            }

            int totalCount = await query.CountAsync();


            var customers = await query
                .OrderBy(a => a.PersonID)
                .Skip((filter.pageNumber - 1) * filter.pageSize)
                .Take(filter.pageSize)
                .Select(a => new CustomerSummaryDto
                {
                    PersonID = a.PersonID,
                    FullName = a.FirstName + " " + a.LastName,
                    RegisteredAt = a.RegisteredAt

                }).ToListAsync();

            return new PagedResponse<CustomerSummaryDto>(customers, totalCount, filter.pageNumber, filter.pageSize);
        }

    }
}
