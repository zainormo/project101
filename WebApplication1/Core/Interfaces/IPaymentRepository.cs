using DormitoryMS.Core.Entities;

namespace DormitoryMS.Core.Interfaces;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<IEnumerable<Payment>> GetByStudentIdAsync(string studentId);
    Task<IEnumerable<Payment>> GetByBillIdAsync(int billId);
    Task<IEnumerable<Payment>> GetRecentPaymentsAsync(int count = 10);
    Task<decimal> GetTotalCollectedAsync(string? month = null);
}
