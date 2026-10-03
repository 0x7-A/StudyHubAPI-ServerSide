using Microsoft.EntityFrameworkCore;
using StudyHubAPI.Data;
using StudyHubAPI.Models.DTOs;
using StudyHubAPI.Models.DTOs.Invoice;
using StudyHubAPI.Models.Entities;
using StudyHubAPI.Models.Filter;

namespace StudyHubAPI.Repositories
{
    public class InvoiceRepository
    {
        private readonly StudyHubDbContext _context;

        public InvoiceRepository(StudyHubDbContext context)
        {
            _context = context;
        }


        public async Task<int> AddInvoice(Invoices invoice)
        {
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();
            return invoice.InvoiceID;
        }

        public async Task<InvoiceDetailsDto?> GetInvoiceById(int invoiceId)
        {
            return await _context.Invoices.AsNoTracking().Where(i => i.InvoiceID == invoiceId).
                Select(i => new InvoiceDetailsDto
            {
                InvoiceID = i.InvoiceID,
                PaymentID = i.PaymentID,
                GeneralOfferID = i.GeneralOfferID,
                OriginalPrice = i.OriginalPrice,
                DiscountAmount = i.DiscountAmount,
                TaxAmount = i.TaxAmount,
                TotalAmount = i.TotalAmount
            }).FirstOrDefaultAsync();
        }

        public async Task<PagedResponse<InvoiceSummaryDto>> GetAllInvoices(InvoiceQueryFilter filter)
        {
            var query = _context.Invoices.AsNoTracking();
          
            if(filter.GeneralOfferID.HasValue)
            {
                query = query.Where(i => i.GeneralOfferID == filter.GeneralOfferID.Value);
            }

            if(filter.PaymentID.HasValue)
            {
                query = query.Where(i => i.PaymentID == filter.PaymentID.Value);
            }

            int totalCount = await query.CountAsync();

            var invoices = await query
             .OrderBy(a => a.InvoiceID)
             .Skip((filter.pageNumber - 1) * filter.pageSize)
             .Take(filter.pageSize)
                .Select(i => new InvoiceSummaryDto
                {
                    InvoiceID = i.InvoiceID,
                    PaymentID = i.PaymentID,
                    GeneralOfferID = i.GeneralOfferID,
                    TotalAmount = i.TotalAmount
                }).ToListAsync();

            return new PagedResponse<InvoiceSummaryDto>(invoices, totalCount, filter.pageNumber, filter.pageSize);

        }


    }
}
