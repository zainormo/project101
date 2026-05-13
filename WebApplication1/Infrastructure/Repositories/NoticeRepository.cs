using Microsoft.EntityFrameworkCore;
using DormitoryMS.Core.Entities;
using DormitoryMS.Core.Interfaces;
using DormitoryMS.Infrastructure.Data;

namespace DormitoryMS.Infrastructure.Repositories;

public class NoticeRepository : Repository<Notice>, INoticeRepository
{
    public NoticeRepository(AppDbContext context) : base(context) { }

    public override async Task<IEnumerable<Notice>> GetAllAsync()
    {
        return await _dbSet.Include(n => n.Admin)
                           .OrderByDescending(n => n.IsPinned)
                           .ThenByDescending(n => n.CreatedAt)
                           .ToListAsync();
    }

    public async Task<IEnumerable<Notice>> GetPinnedNoticesAsync()
    {
        return await _dbSet.Where(n => n.IsPinned)
                           .OrderByDescending(n => n.CreatedAt)
                           .ToListAsync();
    }

    public async Task<IEnumerable<Notice>> GetActiveNoticesAsync()
    {
        var now = DateTime.UtcNow;
        return await _dbSet.Where(n => !n.ExpiresAt.HasValue || n.ExpiresAt > now)
                           .OrderByDescending(n => n.IsPinned)
                           .ThenByDescending(n => n.CreatedAt)
                           .ToListAsync();
    }

    public async Task<IEnumerable<Notice>> GetRecentNoticesAsync(int count = 5)
    {
        return await _dbSet.OrderByDescending(n => n.IsPinned)
                           .ThenByDescending(n => n.CreatedAt)
                           .Take(count)
                           .ToListAsync();
    }
}
