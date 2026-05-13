using DormitoryMS.Core.Entities;

namespace DormitoryMS.ViewModels.Admin;

public class AdminDashboardViewModel
{
    public int TotalStudents { get; set; }
    public int TotalRooms { get; set; }
    public int PendingComplaints { get; set; }
    public decimal DueAmount { get; set; }
    public IEnumerable<Complaint> RecentComplaints { get; set; } = Enumerable.Empty<Complaint>();
    public IEnumerable<Notice> RecentNotices { get; set; } = Enumerable.Empty<Notice>();
    public IEnumerable<Payment> RecentPayments { get; set; } = Enumerable.Empty<Payment>();
}

public class ResidentListViewModel
{
    public IEnumerable<DormitoryMS.Core.Entities.Student> Students { get; set; } = Enumerable.Empty<DormitoryMS.Core.Entities.Student>();
    public IEnumerable<Room> AvailableRooms { get; set; } = Enumerable.Empty<Room>();
    public string? SearchTerm { get; set; }
}

public class RoomViewModel
{
    public IEnumerable<Room> Rooms { get; set; } = Enumerable.Empty<Room>();
}

public class BillingViewModel
{
    public IEnumerable<Bill> Bills { get; set; } = Enumerable.Empty<Bill>();
    public IEnumerable<Payment> Payments { get; set; } = Enumerable.Empty<Payment>();
    public decimal TotalDue { get; set; }
    public decimal TotalCollected { get; set; }
    public string ActiveTab { get; set; } = "bills";
}

public class ComplaintAdminViewModel
{
    public IEnumerable<Complaint> Complaints { get; set; } = Enumerable.Empty<Complaint>();
    public string? StatusFilter { get; set; }
}

public class ReportViewModel
{
    public IEnumerable<Room> Rooms { get; set; } = Enumerable.Empty<Room>();
    public IEnumerable<DormitoryMS.Core.Entities.Student> Students { get; set; } = Enumerable.Empty<DormitoryMS.Core.Entities.Student>();
    public IEnumerable<Bill> Bills { get; set; } = Enumerable.Empty<Bill>();
    public IEnumerable<Payment> Payments { get; set; } = Enumerable.Empty<Payment>();
    public IEnumerable<Complaint> Complaints { get; set; } = Enumerable.Empty<Complaint>();
    public decimal TotalCollected { get; set; }
    public decimal TotalOutstanding { get; set; }
    public string ActiveTab { get; set; } = "occupancy";
}

public class AddStudentViewModel
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int? RoomId { get; set; }
}

public class EditStudentViewModel
{
    public string RegNo { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int? RoomId { get; set; }
}

public class AddRoomViewModel
{
    public string RoomNo { get; set; } = string.Empty;
    public int Floor { get; set; }
    public int Capacity { get; set; } = 2;
    public string Type { get; set; } = "Double";
}

public class GenerateBillViewModel
{
    public string Month { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
}

public class PostNoticeViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsPinned { get; set; }
    public DateTime? ExpiresAt { get; set; }
}
