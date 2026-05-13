using DormitoryMS.Core.Entities;
using DormitoryMS.Core.Interfaces;

namespace DormitoryMS.Core.Services;

public class DashboardService : IDashboardService
{
    private readonly IStudentRepository _studentRepo;
    private readonly IRoomRepository _roomRepo;
    private readonly IComplaintRepository _complaintRepo;
    private readonly IBillRepository _billRepo;
    private readonly INoticeRepository _noticeRepo;
    private readonly IPaymentRepository _paymentRepo;

    public DashboardService(
        IStudentRepository studentRepo,
        IRoomRepository roomRepo,
        IComplaintRepository complaintRepo,
        IBillRepository billRepo,
        INoticeRepository noticeRepo,
        IPaymentRepository paymentRepo)
    {
        _studentRepo = studentRepo;
        _roomRepo = roomRepo;
        _complaintRepo = complaintRepo;
        _billRepo = billRepo;
        _noticeRepo = noticeRepo;
        _paymentRepo = paymentRepo;
    }

    public async Task<int> GetTotalStudentsAsync()
        => await _studentRepo.CountAsync();

    public async Task<int> GetTotalRoomsAsync()
        => await _roomRepo.CountAsync();

    public async Task<int> GetPendingComplaintsAsync()
        => await _complaintRepo.GetPendingCountAsync();

    public async Task<decimal> GetDueAmountAsync()
        => await _billRepo.GetTotalDueAmountAsync();

    public async Task<IEnumerable<Complaint>> GetRecentComplaintsAsync(int count = 5)
        => await _complaintRepo.GetRecentComplaintsAsync(count);

    public async Task<IEnumerable<Notice>> GetRecentNoticesAsync(int count = 5)
        => await _noticeRepo.GetRecentNoticesAsync(count);

    public async Task<IEnumerable<Payment>> GetRecentPaymentsAsync(int count = 5)
        => await _paymentRepo.GetRecentPaymentsAsync(count);
}
