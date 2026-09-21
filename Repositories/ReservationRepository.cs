using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using StudyHubAPI.Data;
using StudyHubAPI.Models.DTOs;
using StudyHubAPI.Models.DTOs.Admin;
using StudyHubAPI.Models.DTOs.Reservation;
using StudyHubAPI.Models.Entities;
using StudyHubAPI.Models.Enums;
using StudyHubAPI.Models.Filter;
using System.Collections.ObjectModel;

namespace StudyHubAPI.Repositories
{
    public class ReservationRepository
    {
        private readonly StudyHubDbContext _context;


        public ReservationRepository(StudyHubDbContext context)
        {
            _context = context;
        }


        public async Task<ReservationDetails?> GetReservationByIDDTO(int ReservationID)
        {
            return await _context.Reservations.Select(r => new ReservationDetails
            {
                ReservationID = r.ReservationID,
                WorkspaceID = r.WorkspaceID,
                CustomerID = r.CustomerID,
                CustomerName = r.Customer.FirstName + " " + r.Customer.LastName,
                StartDate = r.StartDate,
                EndDate = r.EndDate,
                ActualStartDate = r.ActualStartDate,
                ActualEndDate = r.ActualEndDate,
                ReservationStatus = r.ReservationStatus,
                ReservationDate = r.ReservationDate

            }).Where(r => r.ReservationID == ReservationID).FirstOrDefaultAsync();
        }


        public async Task<Reservations?> GetReservationByIDUnTracked(int ReservationID)
        {
            return await _context.Reservations.AsNoTracking().Where(r => r.ReservationID == ReservationID).SingleOrDefaultAsync();
        }


        public async Task<PagedResponse<ReservationSummaryDto>> GetAllReservations(ReservationQueryFilter filter)
        {
            var query = _context.Reservations.AsNoTracking();

            int totalCount = await query.CountAsync();

            if (filter.ReservationStatus.HasValue)
            {
                query = query.Where(r => r.ReservationStatus.Equals(filter.ReservationStatus.Value));
            }

            if (filter.StartTime.HasValue)
            {
                query = query.Where(r => r.StartDate >= filter.StartTime.Value);
            }


            var Reservations =  await query
            .OrderByDescending(r => r.StartDate)
            .Skip((filter.pageNumber - 1) * filter.pageSize)
            .Take(filter.pageSize)
            .Select(r => new ReservationSummaryDto
            {
                ReservationID = r.ReservationID,
                CustomerName = r.Customer.FirstName + " " + r.Customer.LastName,
                StartTime = r.StartDate,
                ReservationStatus = r.ReservationStatus
            }).ToListAsync();

            return new PagedResponse<ReservationSummaryDto>(Reservations, totalCount, filter.pageNumber, filter.pageSize);
        }

        public async Task<List<ReservationSummaryDto>> GetAllReservationsByCustomerID(int CustomerID)
        {
            return await _context.Reservations.AsNoTracking().Where(r => r.CustomerID == CustomerID).
                OrderByDescending(r => r.StartDate).Select(r => new ReservationSummaryDto
                {
                    ReservationID = r.ReservationID,
                    CustomerName = r.Customer.FirstName + " " + r.Customer.LastName,
                    ReservationStatus = r.ReservationStatus,
                }).ToListAsync();
        }

        public async Task<bool> IsWorkspaceAvailable(int WorkspaceID, DateTime Startdate, DateTime EndDate)
        {
            return await _context.Reservations.Where(r => r.WorkspaceID == WorkspaceID
             && (r.ReservationStatus == ReservationStatus.Confirmed || r.ReservationStatus == ReservationStatus.Pending)
            && r.StartDate < EndDate
                    && r.EndDate > Startdate && r.Workspace.WorkspaceStatus == 1

            ).AnyAsync();
        }

        public async Task<int> AddReservation(Reservations reservation)
        {
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();
            return reservation.ReservationID;
        }

        public async Task<bool> HasAwaitingPaymentReservation(int customerID)
        {
            return await _context.Reservations.Where(r => r.CustomerID == customerID && 
            r.ReservationStatus == ReservationStatus.AwaitingPayment).AnyAsync(); 
        }

        public async Task<bool> IsValidReservationForReview(int reservationID)
        {
            return await _context.Reservations.
                Where(r => r.ReservationID == reservationID && r.ReservationStatus == ReservationStatus.Completed
                || r.ReservationStatus == ReservationStatus.Pending).AnyAsync();
        }

        public async Task<int> UpdateReservation(int reservationID, ReservationStatus newStatus)
        {
            return await _context.Reservations.Where(w => w.ReservationID == reservationID)
            .ExecuteUpdateAsync(setters => setters.SetProperty(w => w.ReservationStatus, newStatus));
       }

        public async Task<int> CheckIn(int reservationID,DateTime actualdate)
        {
            return await _context.Reservations.Where(w => w.ReservationID == reservationID).
                ExecuteUpdateAsync(setters => setters.SetProperty(w => w.ReservationStatus, ReservationStatus.Pending)
                .SetProperty(w => w.ActualStartDate, actualdate));
        }

        public async Task<int> CheckOut(int reservationID,DateTime actualdate)
        {
             return await _context.Reservations.Where(w => w.ReservationID == reservationID).
             ExecuteUpdateAsync(setters => setters.SetProperty(w => w.ReservationStatus, ReservationStatus.Completed)
            .SetProperty(w => w.ActualEndDate, actualdate));
        }

        public async Task<int> Cancle(int reservationID)
        {
            return await _context.Reservations.Where(w => w.ReservationID == reservationID).
            ExecuteUpdateAsync(setters => setters.SetProperty(w => w.ReservationStatus, ReservationStatus.Cancelled));
        }

        public Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return _context.Database.BeginTransactionAsync();
        }


    }
}
