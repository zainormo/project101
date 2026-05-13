using Microsoft.EntityFrameworkCore;
using DormitoryMS.Core.Entities;
using DormitoryMS.Core.Enums;
using DormitoryMS.Core.Interfaces;
using DormitoryMS.Infrastructure.Data;

namespace DormitoryMS.Infrastructure.Repositories;

public class ComplaintRepository : Repository<Complaint>, IComplaintRepository
{
    public ComplaintRepository(AppDbContext context) : base(context) { }

    public override async Task<IEnumerable<Complaint>> GetAllAsync()
    {
        return await _dbSet.Include(c => c.Student)
                           .Include(c => c.Room)
                           .OrderByDescending(c => c.CreatedAt)
                           .ToListAsync();
    }

    public async Task<IEnumerable<Complaint>> GetAllWithDetailsAsync()
    {
        return await _dbSet.Include(c => c.Student)
                           .Include(c => c.Room)
                           .OrderByDescending(c => c.CreatedAt)
                           .ToListAsync();
    }

    public async Task<IEnumerable<Complaint>> GetByStudentIdAsync(string studentId)
    {
        return await _dbSet.Include(c => c.Room)
                           .Where(c => c.StudentId == studentId)
                           .OrderByDescending(c => c.CreatedAt)
                           .ToListAsync();
    }

    public async Task<IEnumerable<Complaint>> GetByStatusAsync(string status)
    {
        return await _dbSet.Include(c => c.Student)
                           .Include(c => c.Room)
                           .Where(c => c.Status == status)
                           .OrderByDescending(c => c.CreatedAt)
                           .ToListAsync();
    }

    public async Task<IEnumerable<Complaint>> GetPendingComplaintsAsync()
    {
        return await _dbSet.Include(c => c.Student)
                           .Include(c => c.Room)
                           .Where(c => c.Status == ComplaintStatus.Pending.ToString())
                           .OrderByDescending(c => c.CreatedAt)
                           .ToListAsync();
    }

    public async Task<IEnumerable<Complaint>> GetRecentComplaintsAsync(int count = 5)
    {
        return await _dbSet.Include(c => c.Student)
                           .Include(c => c.Room)
                           .OrderByDescending(c => c.CreatedAt)
                           .Take(count)
                           .ToListAsync();
    }

    public async Task<int> GetPendingCountAsync()
    {
        return await _dbSet.CountAsync(c => c.Status == ComplaintStatus.Pending.ToString()
                                         || c.Status == ComplaintStatus.InProgress.ToString());
    }
}
