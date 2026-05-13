using Microsoft.EntityFrameworkCore;
using DormitoryMS.Core.Entities;
using DormitoryMS.Core.Interfaces;
using DormitoryMS.Infrastructure.Data;

namespace DormitoryMS.Infrastructure.Repositories;

public class StudentRepository : Repository<Student>, IStudentRepository
{
    public StudentRepository(AppDbContext context) : base(context) { }

    public override async Task<IEnumerable<Student>> GetAllAsync()
    {
        return await _dbSet.Include(s => s.Room).OrderByDescending(s => s.JoinDate).ToListAsync();
    }

    public async Task<Student?> GetByRegNoAsync(string regNo)
    {
        return await _dbSet.Include(s => s.Room)
                           .Include(s => s.Payments)
                           .Include(s => s.Complaints)
                           .FirstOrDefaultAsync(s => s.RegNo == regNo);
    }

    public async Task<Student?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(s => s.Email == email.ToLower());
    }

    public async Task<IEnumerable<Student>> GetByRoomIdAsync(int roomId)
    {
        return await _dbSet.Where(s => s.RoomId == roomId && s.IsActive).ToListAsync();
    }

    public async Task<IEnumerable<Student>> GetActiveStudentsAsync()
    {
        return await _dbSet.Include(s => s.Room).Where(s => s.IsActive).OrderBy(s => s.Name).ToListAsync();
    }

    public async Task<IEnumerable<Student>> SearchAsync(string searchTerm)
    {
        var term = searchTerm.ToLower();
        return await _dbSet.Include(s => s.Room)
                           .Where(s => s.Name.ToLower().Contains(term) ||
                                       s.RegNo.ToLower().Contains(term) ||
                                       s.Email.ToLower().Contains(term))
                           .ToListAsync();
    }

    public async Task<string> GenerateRegNoAsync()
    {
        var year = DateTime.UtcNow.Year;
        var prefix = $"REG{year}";
        var lastStudent = await _dbSet
            .Where(s => s.RegNo.StartsWith(prefix))
            .OrderByDescending(s => s.RegNo)
            .FirstOrDefaultAsync();

        int nextSeq = 1;
        if (lastStudent != null)
        {
            var seqStr = lastStudent.RegNo.Substring(prefix.Length);
            if (int.TryParse(seqStr, out int lastSeq))
                nextSeq = lastSeq + 1;
        }
        return $"{prefix}{nextSeq:D3}";
    }
}
