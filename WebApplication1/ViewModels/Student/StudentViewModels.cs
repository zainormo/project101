using DormitoryMS.Core.Entities;

namespace DormitoryMS.ViewModels.Student;

public class StudentDashboardViewModel
{
    public string StudentName { get; set; } = string.Empty;
    public string RegNo { get; set; } = string.Empty;
    public string? RoomNo { get; set; }
    public int PendingBills { get; set; }
    public int ActiveComplaints { get; set; }
    public int UnreadNotices { get; set; }
    public IEnumerable<Notice> RecentNotices { get; set; } = Enumerable.Empty<Notice>();
}

public class BillViewModel
{
    public IEnumerable<Bill> Bills { get; set; } = Enumerable.Empty<Bill>();
    public decimal TotalDue { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal TotalOverdue { get; set; }
    public IEnumerable<Payment> PaymentHistory { get; set; } = Enumerable.Empty<Payment>();
}

public class ComplaintViewModel
{
    public IEnumerable<Complaint> Complaints { get; set; } = Enumerable.Empty<Complaint>();
    public int? StudentRoomId { get; set; }
}

public class NoticeViewModel
{
    public IEnumerable<Notice> PinnedNotices { get; set; } = Enumerable.Empty<Notice>();
    public IEnumerable<Notice> RecentNotices { get; set; } = Enumerable.Empty<Notice>();
}
