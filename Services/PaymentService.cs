using StudyHubAPI.Repositories;
using StudyHubAPI.Models.DTOs.Payment;
using StudyHubAPI.Models.Entities;
using StudyHubAPI.Models.Enums;
using Microsoft.Identity.Client;

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

        private async Task<bool> _updatePayment(int reservationID, PaymentStatus status)
        {
            using var transaction = await _ReservationRepository.BeginTransactionAsync();

            try
            {
                if (await _PaymentRepository.SaveChangeAsync() == 0)
                {
                    await transaction.RollbackAsync();
                    return false;
                }

                if (status == PaymentStatus.Completed)
                {
                    if (await _ReservationRepository.UpdateReservation(reservationID, ReservationStatus.Confirmed) == 0)
                    {
                        await transaction.RollbackAsync();
                        return false;
                    }
                }
                else if (status == PaymentStatus.Cancelled)
                {
                    if (await _ReservationRepository.UpdateReservation(reservationID, ReservationStatus.Cancelled) == 0)
                    {
                        await transaction.RollbackAsync();
                        return false;
                    }
                }

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        private async Task<decimal> _GetPrice(int ReservationID, PaymentReason reason)
        {
            var Reservation = await _ReservationRepository.GetReservationByIDUnTracked(ReservationID);
            // do a speical one with rate

            if (Reservation == null)
            {
                return -1;
            }

            switch (reason)
            {
                case PaymentReason.Basic:
                    return await _GetTotalPrice(Reservation.WorkspaceID, Reservation.StartDate, Reservation.EndDate);
                case PaymentReason.Overstay:
                    // calculting in reservation
                case PaymentReason.Fine:
                    // do the logic later here 

                    break;
    
            }

            return -1;
        }


        public Task<PaymentDetialsDto?> GetPaymentByID(int ID)
        {
            return _PaymentRepository.GetPaymntByIDDto(ID);

        }

        public async Task<int> AddPayment(CreatePaymentDto dto)
        {
            // initalliay with payment staus = pending after calcukating the price and
            // adding the payment. 
            decimal price = await _GetPrice(dto.ReservationID, dto.PaymentReason);

           if (price == -1)
           {
                return -1;
           }

            return await _PaymentRepository.AddPayment(GetPaymentObj(dto, price));
        }

      
        public async Task<bool> UpdatePayment(int Id, UpdatePaymentDto dto)
        {
            if (dto.PaymentStatus == PaymentStatus.Completed)
            {
                return false;
            }


            using var transaction = await _PaymentRepository.BeginTransactionAsync();
            try
            {
                var payment = await _PaymentRepository.GetPaymentsByID(Id);

                if (payment == null || payment.PaymentStatus != PaymentStatus.Pending)
                {
                    return false;
                }

                payment.PaymentStatus = dto.PaymentStatus;

                if (!string.IsNullOrWhiteSpace(dto.PaymentMethod))
                {
                    payment.PaymentMethod = dto.PaymentMethod;
                }

                // 4. Save Payment changes
                if (await _PaymentRepository.SaveChangeAsync() <= 0)
                {
                    return false;
                }


                if (payment.PaymentReason == PaymentReason.Basic)
                {
                    var rowsAffected = await _ReservationRepository.UpdateReservation(
                        payment.ReservationID, ReservationStatus.Confirmed);

                    if (rowsAffected <= 0)
                    {
                        return false;
                    }
                }
                // 6. Commit all changes together
                await transaction.CommitAsync();
                return true;
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
