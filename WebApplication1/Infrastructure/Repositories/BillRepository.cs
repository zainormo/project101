using Microsoft.EntityFrameworkCore;
using DormitoryMS.Core.Entities;
using DormitoryMS.Core.Enums;
using DormitoryMS.Core.Interfaces;
using DormitoryMS.Infrastructure.Data;

namespace DormitoryMS.Infrastructure.Repositories;

public class BillRepository : Repository<Bill>, IBillRepository
{
    public BillRepository(AppDbContext context) : base(context) { }

    public override async Task<IEnumerable<Bill>> GetAllAsync()
    {
        return await _dbSet.Include(b => b.Room)
                           .Include(b => b.Payments)
                           .OrderByDescending(b => b.CreatedAt)
                           .ToListAsync();
    }

    public async Task<IEnumerable<Bill>> GetByRoomIdAsync(int roomId)
    {
        return await _dbSet.Include(b => b.Room)
                           .Where(b => b.RoomId == roomId)
                           .OrderByDescending(b => b.CreatedAt)
                           .ToListAsync();
    }

    public async Task<IEnumerable<Bill>> GetByStatusAsync(string status)
    {
        return await _dbSet.Include(b => b.Room)
                           .Where(b => b.Status == status)
                           .OrderByDescending(b => b.CreatedAt)
                           .ToListAsync();
    }

    public async Task<IEnumerable<Bill>> GetByMonthAsync(string month)
    {
        return await _dbSet.Include(b => b.Room)
                           .Where(b => b.Month == month)
                           .ToListAsync();
    }

    public async Task<IEnumerable<Bill>> GetOverdueBillsAsync()
    {
        var now = DateTime.UtcNow;
        return await _dbSet.Include(b => b.Room)
                           .Where(b => (b.Status == BillStatus.Pending.ToString() && b.DueDate < now)
                                    || b.Status == BillStatus.Overdue.ToString())
                           .ToListAsync();
    }

    public async Task<IEnumerable<Bill>> GetBillsByStudentAsync(string regNo)
    {
        // Step 1: get the roomId first
        var roomId = await _context.Students
            .Where(s => s.RegNo == regNo)
            .Select(s => s.RoomId)
            .FirstOrDefaultAsync();

        // If student has no room, return empty
        if (roomId == null)
            return Enumerable.Empty<Bill>();

        // Step 2: query bills separately
        return await _dbSet
            .Include(b => b.Room)
            .Include(b => b.Payments)
            .Where(b => b.RoomId == roomId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalDueAmountAsync()
    {
        return await _dbSet
            .Where(b => b.Status == BillStatus.Pending.ToString()
                     || b.Status == BillStatus.Overdue.ToString())
            .SumAsync(b => b.Amount);
    }

    public async Task<decimal> GetTotalPaidAmountAsync(string? month = null)
    {
        var query = _dbSet.Where(b => b.Status == BillStatus.Paid.ToString());
        if (!string.IsNullOrEmpty(month))
            query = query.Where(b => b.Month == month);
        return await query.SumAsync(b => b.Amount);
    }
}