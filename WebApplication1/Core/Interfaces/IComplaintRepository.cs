using DormitoryMS.Core.Entities;

namespace DormitoryMS.Core.Interfaces;

public interface IComplaintRepository : IRepository<Complaint>
{
    Task<IEnumerable<Complaint>> GetByStudentIdAsync(string studentId);
    Task<IEnumerable<Complaint>> GetByStatusAsync(string status);
    Task<IEnumerable<Complaint>> GetPendingComplaintsAsync();
    Task<IEnumerable<Complaint>> GetRecentComplaintsAsync(int count = 5);
    Task<int> GetPendingCountAsync();
    Task<IEnumerable<Complaint>> GetAllWithDetailsAsync();
}
