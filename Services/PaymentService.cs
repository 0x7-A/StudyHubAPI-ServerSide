using StudyHubAPI.Repositories;
using StudyHubAPI.Models.DTOs.Payment;
using StudyHubAPI.Models.Entities;
using StudyHubAPI.Models.Enums;
using StudyHubAPI.Utils;
using Microsoft.Identity.Client;
using Microsoft.AspNetCore.Http.HttpResults;

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
        public PaymentService(PaymentRepository paymentRepository,
            ReservationRepository reservationRepository, WorkspaceRepository workspaceRepository)
        {
          _PaymentRepository = paymentRepository;
          _ReservationRepository = reservationRepository;
          _WorkspaceRepository = workspaceRepository;
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
                PaymentDate = DateTime.Now,
                TotalPrice = price
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




           decimal price = await _GetPrice(dto.ReservationID, Reservation, dto.PaymentReason);

           if (price == -1)
           {
                return ServiceResult<int>.Failure(ResultType.Failure, "Failed to calculate payment amount");
           }

            return ServiceResult<int>.Success(await _PaymentRepository.AddPayment(GetPaymentObj(dto, price)));
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
