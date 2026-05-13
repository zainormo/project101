using Microsoft.EntityFrameworkCore;
using DormitoryMS.Core.Entities;
using DormitoryMS.Core.Interfaces;
using DormitoryMS.Infrastructure.Data;

namespace DormitoryMS.Infrastructure.Repositories;

public class PaymentRepository : Repository<Payment>, IPaymentRepository
{
    public PaymentRepository(AppDbContext context) : base(context) { }

    public override async Task<IEnumerable<Payment>> GetAllAsync()
    {
        return await _dbSet.Include(p => p.Bill).ThenInclude(b => b.Room)
                           .Include(p => p.Student)
                           .OrderByDescending(p => p.Date)
                           .ToListAsync();
    }

    public async Task<IEnumerable<Payment>> GetByStudentIdAsync(string studentId)
    {
        return await _dbSet.Include(p => p.Bill).ThenInclude(b => b.Room)
                           .Where(p => p.StudentId == studentId)
                           .OrderByDescending(p => p.Date)
                           .ToListAsync();
    }

    public async Task<IEnumerable<Payment>> GetByBillIdAsync(int billId)
    {
        return await _dbSet.Include(p => p.Student)
                           .Where(p => p.BillId == billId)
                           .ToListAsync();
    }

    public async Task<IEnumerable<Payment>> GetRecentPaymentsAsync(int count = 10)
    {
        return await _dbSet.Include(p => p.Bill).ThenInclude(b => b.Room)
                           .Include(p => p.Student)
                           .OrderByDescending(p => p.Date)
                           .Take(count)
                           .ToListAsync();
    }

    public async Task<decimal> GetTotalCollectedAsync(string? month = null)
    {
        var query = _dbSet.AsQueryable();
        if (!string.IsNullOrEmpty(month))
            query = query.Include(p => p.Bill).Where(p => p.Bill.Month == month);
        return await query.SumAsync(p => p.Amount);
    }
}
