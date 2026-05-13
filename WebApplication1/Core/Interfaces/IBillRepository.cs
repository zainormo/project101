using DormitoryMS.Core.Entities;

namespace DormitoryMS.Core.Interfaces;

public interface IBillRepository : IRepository<Bill>
{
    Task<IEnumerable<Bill>> GetByRoomIdAsync(int roomId);
    Task<IEnumerable<Bill>> GetByStatusAsync(string status);
    Task<IEnumerable<Bill>> GetByMonthAsync(string month);
    Task<IEnumerable<Bill>> GetOverdueBillsAsync();
    Task<IEnumerable<Bill>> GetBillsByStudentAsync(string regNo);
    Task<decimal> GetTotalDueAmountAsync();
    Task<decimal> GetTotalPaidAmountAsync(string? month = null);
}
