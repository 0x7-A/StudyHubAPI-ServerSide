using StudyHubAPI.Models.DTOs.Payment;
using StudyHubAPI.Models.Entities;
using StudyHubAPI.Models.Enums;
using StudyHubAPI.Repositories;
using StudyHubAPI.Utils;

namespace StudyHubAPI.Services
{
    public class PaymentService
    {

        // [1] fine logic
        // [2] overstay lowrate
        // [3] allow to  stay over that withot fine => 5 m
        // [4] allow the updation of actual start and actual end
        // [5] check if he had already a fine

        private readonly PaymentRepository _PaymentRepository;
        private readonly ReservationRepository _ReservationRepository;
        private readonly WorkspaceRepository _WorkspaceRepository;
        private readonly InvoiceRepository _InvoiceRepository;
        private readonly OfferRepository _OfferRepository;

        public PaymentService(PaymentRepository paymentRepository,
            ReservationRepository reservationRepository, WorkspaceRepository workspaceRepository,
            OfferRepository offerRepository, InvoiceRepository invoiceRepository)
        {
          _PaymentRepository = paymentRepository;
          _ReservationRepository = reservationRepository;
          _WorkspaceRepository = workspaceRepository;
          _OfferRepository = offerRepository;
          _InvoiceRepository = invoiceRepository;
        }

        private Payments GetPaymentObj(CreatePaymentDto dto, decimal price)
        {
            return new Payments
            {
                AdminID = dto.AdminID,
                CustomerID = dto.CustomerID,
                ReservationID = dto.ReservationID,
                PaymentStatus = PaymentStatus.Pending,
                PaymentReason = dto.PaymentReason,
                PaymentDate = DateTime.Now
            };
        }

        private async Task<decimal> _GetTotalPrice(int WorkspaceID, DateTime StartDate, DateTime EndDate)
        {
            var diff = EndDate - StartDate;
            decimal HourRate = (decimal)diff.TotalMinutes / 60;
            return await _WorkspaceRepository.GetHourlyRate(WorkspaceID) * HourRate;
        }

        private async Task<ServiceResult> _updatePayment(int reservationID, PaymentStatus status)
        {
            using var transaction = await _ReservationRepository.BeginTransactionAsync();

            try
            {
                if (await _PaymentRepository.SaveChangeAsync() == 0)
                {
                    await transaction.RollbackAsync();
                    return ServiceResult.Failure(ResultType.Failure, "Failed to update payment.");
                }

                if (status == PaymentStatus.Completed)
                {
                    if (await _ReservationRepository.UpdateReservation(reservationID, ReservationStatus.Confirmed) == 0)
                    {
                        await transaction.RollbackAsync();
                        return ServiceResult.Failure(ResultType.Failure, "Failed to update reservation.");
                    }
                }
                else if (status == PaymentStatus.Cancelled)
                {
                    if (await _ReservationRepository.UpdateReservation(reservationID, ReservationStatus.Cancelled) == 0)
                    {
                        await transaction.RollbackAsync();
                        return ServiceResult.Failure(ResultType.Failure, "Failed to update reservation.");
                    }
                }

                await transaction.CommitAsync();
                return ServiceResult.Success(ResultType.Ok);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        private async Task<decimal> _GetPrice(int ReservationID, Reservations Reservation, PaymentReason reason)
        {
            switch (reason)
            {
                case PaymentReason.Basic:
                    return await _GetTotalPrice(Reservation.WorkspaceID, Reservation.StartDate, Reservation.EndDate);
                case PaymentReason.Overstay:
                    // calculting in reservation
                case PaymentReason.Damage:
                    // do the logic later here 

                    break;
    
            }

            return -1;
        }

        public async Task<ServiceResult<int>> _AddPayment(CreatePaymentDto dto, Reservations Reservation)
        {
            decimal price = await _GetPrice(dto.ReservationID, Reservation, dto.PaymentReason);

            if (price == -1)
            {
                return ServiceResult<int>.Failure(ResultType.Failure, "Failed to calculate payment amount");
            }

            var activeOffer = await _OfferRepository.GetActiveOffer();
            decimal discountAmount = 0;
            int? offerID = null;

            if (activeOffer != null)
            {
                discountAmount = price * activeOffer.OfferPercentage;
                offerID = activeOffer.OfferID;
            }

            decimal taxAmount = SystemSettings.TaxRate * price;

            // 1. Begin the Database Transaction via DbContext (or via Unit of Work / Repository DbContext)
            await using var transaction = await  _PaymentRepository.BeginTransactionAsync();

            try
            {
                // 2. Insert Payment
                var newPaymentID = await _PaymentRepository.AddPayment(GetPaymentObj(dto, price));

                // 3. Insert Invoice using the new Payment ID
                int invoiceID = await _InvoiceRepository.AddInvoice(new Invoices
                {
                    PaymentID = newPaymentID,
                    GeneralOfferID = offerID,
                    OriginalPrice = price,
                    TaxAmount = taxAmount,
                    DiscountAmount = discountAmount,
                    TotalAmount = price + taxAmount - discountAmount
                });

                // 4. Commit all changes atomically
                await transaction.CommitAsync();

                return ServiceResult<int>.Success(newPaymentID);
            }
            catch (Exception ex)
            {
                // 5. Rollback on failure to prevent orphaned Payments without Invoices
                await transaction.RollbackAsync();

                // Log exception here (e.g., _logger.LogError(ex, "..."))
                return ServiceResult<int>.Failure(ResultType.Failure, "An error occurred while processing the payment and invoice.");
            }
        }


        public  async Task<ServiceResult<PaymentDetialsDto?>> GetPaymentByID(int ID)
        {
            var payment = await  _PaymentRepository.GetPaymntByIDDto(ID);

            if(payment == null)
            {
                return ServiceResult<PaymentDetialsDto?>.Failure(ResultType.NotFound, "Payment not found");
            }

            return ServiceResult<PaymentDetialsDto?>.Success(payment, ResultType.Ok);
        }

        public async Task<ServiceResult<int>> AddPayment(CreatePaymentDto dto)
        {
            // initalliay with payment staus = pending after calcukating the price and
            // adding the payment. 

            if(dto.PaymentReason == PaymentReason.Basic)
            {
                return ServiceResult<int>.Failure(ResultType.BadRequest, "Cannot add basic payment through this endpoint");
            }

            var Reservation = await _ReservationRepository.GetReservationByIDUnTracked(dto.ReservationID);
            if (Reservation == null)
            {
                return ServiceResult<int>.Failure(ResultType.NotFound, "Reservation not found");
            }
            
            if(dto.PaymentReason == PaymentReason.Damage &&  (Reservation.ReservationStatus != ReservationStatus.Completed  
                || Reservation.ReservationStatus != ReservationStatus.Pending))
            {
                return ServiceResult<int>.Failure(ResultType.BadRequest, "Cannot add fine for this reservation, since it is not pending or completed");
            }

            if (dto.PaymentReason == PaymentReason.Overstay && Reservation.ReservationStatus != ReservationStatus.Completed)
            {
                return ServiceResult<int>.Failure(ResultType.BadRequest, "Cannot add fine for this reservation, since it is not pending or completed");
            }

             return await _AddPayment(dto, Reservation);
        }

      
        public async Task<ServiceResult> UpdatePayment(int Id, UpdatePaymentDto dto)
        {
            using var transaction = await _PaymentRepository.BeginTransactionAsync();
            try
            {
                var payment = await _PaymentRepository.GetPaymentsByID(Id);

                if (payment == null)
                {
                    return ServiceResult.Failure(ResultType.NotFound, "Payment not found");
                }

                if (payment.PaymentStatus != PaymentStatus.Pending)
                {
                    return ServiceResult.Failure(ResultType.BadRequest, "cannot edit completed or cancelled payment");
                }

                payment.PaymentStatus = dto.PaymentStatus;

                if (!string.IsNullOrWhiteSpace(dto.PaymentMethod))
                {
                    payment.PaymentMethod = dto.PaymentMethod;
                }

                // 4. Save Payment changes
                if (await _PaymentRepository.SaveChangeAsync() <= 0)
                {
                    return ServiceResult.Failure(ResultType.Failure, "Failed to update payment");
                }


                if (payment.PaymentReason == PaymentReason.Basic)
                {
                    var rowsAffected = await _ReservationRepository.UpdateReservation(
                        payment.ReservationID, ReservationStatus.Confirmed);

                    if (rowsAffected <= 0)
                    {
                        return    ServiceResult.Failure(ResultType.Failure, "Failed to update reservation");
                    }
                }
                // 6. Commit all changes together
                await transaction.CommitAsync();
                return ServiceResult.Success(ResultType.NoContent);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

        }

        public Task<List<PaymentDetialsDto>> GetAllPendingPaymentByCustomerID(int CustomerID)
        {
            return _PaymentRepository.GetAllPendingByCustomerID(CustomerID, PaymentStatus.Pending);
        }

    }
}
