using DormitoryMS.Core.Entities;

namespace DormitoryMS.Core.Interfaces;

public interface IAuthService
{
    Task<Admin?> LoginAdminAsync(string username, string password);
    Task<Student?> LoginStudentAsync(string regNo, string password);
    Task<Student> RegisterStudentAsync(string name, string email, string phone, string password);
    Task<bool> ChangePasswordAsync(string userId, string oldPassword, string newPassword, bool isAdmin = false);
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}

public interface IStudentService
{
    Task<IEnumerable<Student>> GetAllStudentsAsync();
    Task<IEnumerable<Student>> GetActiveStudentsAsync();
    Task<Student?> GetStudentByRegNoAsync(string regNo);
    Task<Student> AddStudentAsync(string name, string email, string phone, string password, int? roomId);
    Task UpdateStudentAsync(string regNo, string name, string email, string phone, int? roomId);
    Task ToggleStudentStatusAsync(string regNo);
    Task DeleteStudentAsync(string regNo);
    Task<IEnumerable<Student>> SearchStudentsAsync(string searchTerm);
    Task AssignRoomAsync(string regNo, int roomId);
}

public interface IRoomService
{
    Task<IEnumerable<Room>> GetAllRoomsAsync();
    Task<IEnumerable<Room>> GetActiveRoomsAsync();
    Task<IEnumerable<Room>> GetAvailableRoomsAsync();
    Task<IEnumerable<Room>> GetRoomsWithStudentsAsync();
    Task<Room?> GetRoomByIdAsync(int roomId);
    Task<Room> AddRoomAsync(string roomNo, int floor, int capacity, string type);
    Task UpdateRoomAsync(int roomId, string roomNo, int floor, int capacity, string type);
    Task DeleteRoomAsync(int roomId);
    Task ToggleRoomStatusAsync(int roomId);
}

public interface IBillingService
{
    Task<IEnumerable<Bill>> GetAllBillsAsync();
    Task<IEnumerable<Bill>> GetBillsByStudentAsync(string regNo);
    Task<IEnumerable<Bill>> GetBillsByStatusAsync(string status);
    Task<IEnumerable<Bill>> GetOverdueBillsAsync();
    Task<Bill?> GetBillByIdAsync(int billId);
    Task GenerateBillsForMonthAsync(string month, decimal amount, DateTime dueDate);
    Task MarkBillAsPaidAsync(int billId);
    Task<Payment> ProcessPaymentAsync(int billId, string studentId, decimal amount, string method, string? reference);
    Task<IEnumerable<Payment>> GetPaymentsByStudentAsync(string studentId);
    Task<IEnumerable<Payment>> GetAllPaymentsAsync();
    Task<IEnumerable<Payment>> GetRecentPaymentsAsync(int count = 10);
    Task<decimal> GetTotalDueAmountAsync();
    Task<decimal> GetTotalPaidAmountAsync(string? month = null);
}

public interface IComplaintService
{
    Task<IEnumerable<Complaint>> GetAllComplaintsAsync();
    Task<IEnumerable<Complaint>> GetComplaintsByStudentAsync(string studentId);
    Task<IEnumerable<Complaint>> GetComplaintsByStatusAsync(string status);
    Task<Complaint?> GetComplaintByIdAsync(int complaintId);
    Task<Complaint> SubmitComplaintAsync(string studentId, int? roomId, string issue, string? description, string category);
    Task UpdateComplaintStatusAsync(int complaintId, string status);
    Task ReplyToComplaintAsync(int complaintId, string reply);
    Task<int> GetPendingCountAsync();
    Task<IEnumerable<Complaint>> GetRecentComplaintsAsync(int count = 5);
}

public interface INoticeService
{
    Task<IEnumerable<Notice>> GetAllNoticesAsync();
    Task<IEnumerable<Notice>> GetActiveNoticesAsync();
    Task<IEnumerable<Notice>> GetPinnedNoticesAsync();
    Task<IEnumerable<Notice>> GetRecentNoticesAsync(int count = 5);
    Task<Notice?> GetNoticeByIdAsync(int noticeId);
    Task<Notice> PostNoticeAsync(string title, string content, int adminId, bool isPinned, DateTime? expiresAt);
    Task UpdateNoticeAsync(int noticeId, string title, string content, bool isPinned, DateTime? expiresAt);
    Task DeleteNoticeAsync(int noticeId);
    Task TogglePinAsync(int noticeId);
}

public interface IDashboardService
{
    Task<int> GetTotalStudentsAsync();
    Task<int> GetTotalRoomsAsync();
    Task<int> GetPendingComplaintsAsync();
    Task<decimal> GetDueAmountAsync();
    Task<IEnumerable<Complaint>> GetRecentComplaintsAsync(int count = 5);
    Task<IEnumerable<Notice>> GetRecentNoticesAsync(int count = 5);
    Task<IEnumerable<Payment>> GetRecentPaymentsAsync(int count = 5);
}
