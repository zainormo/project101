using DormitoryMS.Core.Entities;

namespace DormitoryMS.Core.Interfaces;

public interface IAdminRepository : IRepository<Admin>
{
    Task<Admin?> GetByUsernameAsync(string username);
}
