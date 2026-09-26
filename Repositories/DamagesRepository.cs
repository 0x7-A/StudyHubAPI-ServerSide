
using Microsoft.EntityFrameworkCore;
using StudyHubAPI.Data;
using StudyHubAPI.Models.DTOs.Damage;
using StudyHubAPI.Models.Entities;

namespace StudyHubAPI.Repositories
{
    public class DamagesRepository
    {
        private readonly StudyHubDbContext _context;

        public DamagesRepository(StudyHubDbContext context)
        {
            _context = context;
        }


       public async Task<int> AddDamagedRecord(Damage damaged)
       {
           _context.Damages.Add(damaged);
           await _context.SaveChangesAsync();
            return damaged.DamageId;
       }


        public async Task<DamageSummaryDto?> GetDamageRecord(int paymentID)
        {
            return await _context.Damages.Select(d => new DamageSummaryDto
            {
                DamageId = d.DamageId,
                PaymentId = d.PaymentId,
                Notes = d.Notes,
            }).Where(d => d.PaymentId == paymentID).FirstOrDefaultAsync();

        }

        public async Task<string?> GetDamageImagePathByPaymentId(int paymentID)
        {
            return await _context.Damages.Where(d => d.PaymentId == paymentID).Select(d => d.ImagePath).FirstOrDefaultAsync();
        }



    }
}
