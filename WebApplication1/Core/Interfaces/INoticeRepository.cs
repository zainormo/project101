using DormitoryMS.Core.Entities;

namespace DormitoryMS.Core.Interfaces;

public interface INoticeRepository : IRepository<Notice>
{
    Task<IEnumerable<Notice>> GetPinnedNoticesAsync();
    Task<IEnumerable<Notice>> GetActiveNoticesAsync();
    Task<IEnumerable<Notice>> GetRecentNoticesAsync(int count = 5);
}
