using Microsoft.AspNetCore.Mvc;
using StudyHubAPI.Models.DTOs;
using StudyHubAPI.Models.DTOs.Reservation;
using StudyHubAPI.Models.Entities;
using StudyHubAPI.Models.Enums;
using StudyHubAPI.Models.Filter;
using StudyHubAPI.Repositories;
using StudyHubAPI.Utils;

namespace StudyHubAPI.Services
{
    public class ReservationService
    {
        private readonly ReservationRepository _reservationRepository;
        private readonly WorkspaceRepository _WorkspaceRepository;
        private readonly PaymentRepository _paymentRepository;

        public ReservationService(ReservationRepository reservationRepository,WorkspaceRepository workspaceRepository,
            PaymentRepository paymentRepository)
        {
            _reservationRepository = reservationRepository;
            _WorkspaceRepository = workspaceRepository;
            _paymentRepository = paymentRepository;
        }

        private async Task<decimal> _GetTotalPrice(int WorkspaceID,DateTime StartDate,DateTime EndDate)
        {
            var diff = EndDate - StartDate;
            decimal HourRate = (decimal)diff.TotalMinutes / 60;
            return await _WorkspaceRepository.GetHourlyRate(WorkspaceID) * HourRate;
        }

        public Task<PagedResponse<ReservationSummaryDto>> GetAllReservation(ReservationQueryFilter filter)
        {
            return _reservationRepository.GetAllReservations(filter);
        }


        public Task<ReservationDetails?> GetReservationByID(int ReservationID)
        {
            return _reservationRepository.GetReservationByIDDTO(ReservationID);

        }

        public Task<List<ReservationSummaryDto>> GetAllReservationByCustomerID(int CustomerID)
        {
            return _reservationRepository.GetAllReservationsByCustomerID(CustomerID);
        }


        public async Task<ServiceResult<int>>AddReservation(CreateReservationDto dto)
        {
            if(!await  _paymentRepository.HasUnpaidPayments(dto.CustomerID))
            {
                return ServiceResult<int>.Failure(ResultType.BadRequest, "Customers already have an open paymetns");
            }

            if (!await _reservationRepository.IsWorkspaceAvailable(dto.WorkspaceID, dto.StartDate, dto.EndDate))
            {
                return ServiceResult<int>.Failure(ResultType.BadRequest, $"Workspace is not available in {dto.StartDate} - {dto.EndDate}");
            }

            if(!await _reservationRepository.HasAwaitingPaymentReservation(dto.CustomerID))
            {
                return ServiceResult<int>.Failure(ResultType.BadRequest, $" ");
            }
         
            var totalPrice = await _GetTotalPrice(dto.WorkspaceID, dto.StartDate, dto.EndDate);

            using var transaction = await _reservationRepository.BeginTransactionAsync();
            try
            {
                var reservation = new Reservations
                {
                    AdminID = dto.AdminID,
                    CustomerID = dto.CustomerID,
                    WorkspaceID = dto.WorkspaceID,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    ReservationStatus = ReservationStatus.AwaitingPayment
                };

                int reservationID = await _reservationRepository.AddReservation(reservation);
               

                var payment = new Payments
                {
                    ReservationID = reservationID,
                    CustomerID = dto.CustomerID,
                    AdminID = dto.AdminID,
                    TotalPrice = totalPrice,
                    PaymentStatus = PaymentStatus.Pending,
                    PaymentReason = PaymentReason.Basic
                };

                await _paymentRepository.AddPayment(payment);
               

                await transaction.CommitAsync();
                return  ServiceResult<int>.Success(reservationID, ResultType.NoContent);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<ServiceResult> CheckIn(int reservationId)
        {
            var reservation = await _reservationRepository.GetReservationByIDUnTracked(reservationId);

            if(reservation == null)
            {
                return ServiceResult.Failure(ResultType.NotFound,$"No reservation match{reservationId} " ) ;
            }

            // [1] check from the current status. done
            if (reservation.ReservationStatus != ReservationStatus.Confirmed)
            {
                return ServiceResult.Failure(ResultType.BadRequest,"Can't check in, make sure to follow the process");
            }

            // [2] is the time passed or not
            var now = DateTime.UtcNow;
            if (reservation.StartDate > now || reservation.EndDate < now)
            {
                return ServiceResult.Failure(ResultType.BadRequest,"Reservation already expired");
            }

            // [3] upate the status
            if(await _reservationRepository.CheckIn(reservationId, now) > 0)
            {
                return ServiceResult.Success(ResultType.NoContent);
            }
            return ServiceResult.Failure(ResultType.Failure, "Could not update the reservation");
        }

        public async Task<ServiceResult> CheckOut(int reservationId, int adminID)
        {
            var reservation = await _reservationRepository.GetReservationByIDUnTracked(reservationId);

            if (reservation == null)
            {
               return ServiceResult.Failure(ResultType.NotFound, $"No reservation match{reservationId} ");
            }

            // [1] check from the current status. done
            if (reservation.ReservationStatus != ReservationStatus.Pending)
            {
                return ServiceResult.Failure(ResultType.BadRequest, "Can't check in, make sure to follow the process");
            }

            var now = DateTime.UtcNow;

            // [2] overstay logic
            // insert a payment with overstay

            // later i can allow lateness like 5 minutes
            using var transaction = await _paymentRepository.BeginTransactionAsync();
            try
            {
                if (reservation.EndDate < now)
                {
                    TimeSpan difference = now.Subtract(reservation.EndDate);
                    int totalMinutes = (int)Math.Ceiling(difference.TotalMinutes);

                    if (totalMinutes > SystemSettings.GracePeriodMinutes)
                    {
                        await _paymentRepository.AddPayment(new Payments
                        {
                            ReservationID = reservationId,
                            PaymentReason = PaymentReason.Overstay,
                            CustomerID = reservation.CustomerID,
                            PaymentStatus = PaymentStatus.Pending,
                            AdminID = adminID,
                            TotalPrice = totalMinutes * SystemSettings.PricePerMinuteOverstay
                        });
                    }
                }

                if (await _reservationRepository.CheckOut(reservationId, now) > 0)
                {
                    await transaction.CommitAsync();
                    return ServiceResult.Success(ResultType.NoContent);
                }

                await transaction.RollbackAsync();
                return ServiceResult.Failure(ResultType.Failure, "Could not update the reservation");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
      
        }

        public async Task<ServiceResult> Cancle(int reservationId)
        {
            //id valid
            var reservation = await _reservationRepository.GetReservationByIDUnTracked(reservationId);

            if (reservation == null)
            {
                return ServiceResult.Failure(ResultType.NotFound, $"No reservation match{reservationId} ");
            }


            // to-do 
            // policy of returing money back.


            // mising check if that already pedning 
            // we should cancle payment  


            if(reservation.ReservationStatus != ReservationStatus.Confirmed || reservation.ReservationStatus != ReservationStatus.AwaitingPayment)
            {
                return ServiceResult.Failure(ResultType.BadRequest, "Can't cancle going on payment");
            }


            //[2] the time is passed and can not cancle 
            //[3] limit
            var now = DateTime.UtcNow;
            var allowedStartTime = reservation.StartDate.AddMinutes(SystemSettings.CanCancleBeforeInMinutesInMinus);
            if (allowedStartTime < now)
            {
                return ServiceResult.Failure(ResultType.BadRequest, "Can't cancle the right now, you passed the specified period Reservation already ");
            }

            if (await _reservationRepository.Cancle(reservationId) > 0)
            {
                return ServiceResult.Success(ResultType.NoContent);
            }
            return ServiceResult.Failure(ResultType.Failure, "Could not update the reservation");
        }


    }
}
