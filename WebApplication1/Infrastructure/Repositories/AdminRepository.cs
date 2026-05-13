using Microsoft.EntityFrameworkCore;
using DormitoryMS.Core.Entities;
using DormitoryMS.Core.Interfaces;
using DormitoryMS.Infrastructure.Data;

namespace DormitoryMS.Infrastructure.Repositories;

public class AdminRepository : Repository<Admin>, IAdminRepository
{
    public AdminRepository(AppDbContext context) : base(context) { }

    public async Task<Admin?> GetByUsernameAsync(string username)
    {
        return await _dbSet.FirstOrDefaultAsync(a => a.Username == username.ToLower() && a.IsActive);
    }
}
