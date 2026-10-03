using StudyHubAPI.Models.DTOs.Invoice;
using StudyHubAPI.Models.DTOs;
using StudyHubAPI.Models.Filter;
using StudyHubAPI.Repositories;
using StudyHubAPI.Utils;

namespace StudyHubAPI.Services
{
    public class InvoiceService
    {

        private readonly InvoiceRepository _invoiceRepository;

        public InvoiceService(InvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public async Task<ServiceResult<InvoiceDetailsDto>> GetInvoiceById(int invoiceId)
        {
            var invoice = await _invoiceRepository.GetInvoiceById(invoiceId);
            if (invoice == null)
            {
                return ServiceResult<InvoiceDetailsDto>.Failure(ResultType.NotFound ,"Invoice not found");
            }

            return ServiceResult<InvoiceDetailsDto>.Success(invoice);
        }


        public async Task<PagedResponse<InvoiceSummaryDto>> GetAllInvoices(InvoiceQueryFilter filter)
        {
            return await _invoiceRepository.GetAllInvoices(filter);
        }

    }
}
