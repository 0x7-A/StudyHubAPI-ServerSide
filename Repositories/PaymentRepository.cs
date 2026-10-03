using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using StudyHubAPI.Data;
using StudyHubAPI.Models.DTOs.Payment;
using StudyHubAPI.Models.Entities;
using StudyHubAPI.Models.Enums;


namespace StudyHubAPI.Repositories
{
    public class PaymentRepository
    {
        private readonly StudyHubDbContext _Context;

        public PaymentRepository(StudyHubDbContext context)
        {
            _Context = context;
        }

        public async Task<Payments?> GetPaymentsByID(int id)
        {
            return await _Context.Payments.FindAsync(id);
        }

        public async Task<PaymentDetialsDto?> GetPaymntByIDDto(int id)
        {
            return await _Context.Payments.AsNoTracking().Where(p => p.PaymentID == id).Select(p => new PaymentDetialsDto { PaymentID = p.PaymentID,
                AdminID = p.AdminID, CustomerID = p.CustomerID,ReservationID = p.ReservationID, PaymentDate = p.PaymentDate,
                PaymentMethod = p.PaymentMethod, PaymentReason = p.PaymentReason, PaymentStatus = p.PaymentStatus })
                .FirstOrDefaultAsync();
        }

        public async Task<int> AddPayment(Payments payment)
        {
            _Context.Payments.Add(payment);
            await _Context.SaveChangesAsync();
            return  payment.PaymentID;
        }

        public async Task<List<int>> HasUnpaidPayments(int customerID)
        {
            return await _Context.Payments.Where(p => p.CustomerID == customerID && p.PaymentStatus == PaymentStatus.Pending).Select(p => p.PaymentID).ToListAsync();
        }

        public async Task<List<PaymentDetialsDto>> GetAllPendingByCustomerID(int CustomerID,PaymentStatus status)
        {
            return await _Context.Payments.AsNoTracking().Where(p => p.CustomerID == CustomerID && p.PaymentStatus == status).
                Select(p => new PaymentDetialsDto {
                    PaymentID = p.PaymentID,
                    AdminID = p.AdminID,
                    CustomerID = p.CustomerID,
                    ReservationID = p.ReservationID,
                    PaymentDate = p.PaymentDate,
                    PaymentMethod = p.PaymentMethod,
                    PaymentReason = p.PaymentReason,
                    PaymentStatus = p.PaymentStatus
                }).ToListAsync();
        }


        public async Task<int> SaveChangeAsync()
        {
            return await _Context.SaveChangesAsync();
        }
        public Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return _Context.Database.BeginTransactionAsync();
        }


       public async Task<Reservations?> GetReservationByPaymentId(int paymentId)
        {
            return await _Context.Payments.Where(r => r.PaymentID == paymentId).Select(p => p.Reservation).FirstOrDefaultAsync();
        }

    }
}
