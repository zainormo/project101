using Microsoft.EntityFrameworkCore;
using DormitoryMS.Core.Entities;
using DormitoryMS.Core.Interfaces;
using DormitoryMS.Infrastructure.Data;

namespace DormitoryMS.Infrastructure.Repositories;

public class RoomRepository : Repository<Room>, IRoomRepository
{
    public RoomRepository(AppDbContext context) : base(context) { }

    public override async Task<IEnumerable<Room>> GetAllAsync()
    {
        return await _dbSet.Include(r => r.Students.Where(s => s.IsActive))
                           .OrderBy(r => r.RoomNo)
                           .ToListAsync();
    }

    public async Task<Room?> GetByRoomNoAsync(string roomNo)
    {
        return await _dbSet.Include(r => r.Students.Where(s => s.IsActive))
                           .FirstOrDefaultAsync(r => r.RoomNo == roomNo);
    }

    public async Task<IEnumerable<Room>> GetActiveRoomsAsync()
    {
        return await _dbSet.Include(r => r.Students.Where(s => s.IsActive))
                           .Where(r => r.IsActive)
                           .OrderBy(r => r.RoomNo)
                           .ToListAsync();
    }

    public async Task<IEnumerable<Room>> GetAvailableRoomsAsync()
    {
        var rooms = await _dbSet.Include(r => r.Students.Where(s => s.IsActive))
                                .Where(r => r.IsActive)
                                .ToListAsync();
        return rooms.Where(r => !r.IsFull).OrderBy(r => r.RoomNo);
    }

    public async Task<IEnumerable<Room>> GetRoomsWithStudentsAsync()
    {
        return await _dbSet.Include(r => r.Students.Where(s => s.IsActive))
                           .Where(r => r.IsActive)
                           .OrderBy(r => r.RoomNo)
                           .ToListAsync();
    }
}
