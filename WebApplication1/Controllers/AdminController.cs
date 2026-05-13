using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DormitoryMS.Core.Interfaces;
using DormitoryMS.ViewModels.Admin;

namespace DormitoryMS.Controllers;

[Authorize(Policy = "AdminOnly")]
public class AdminController : Controller
{
    private readonly IDashboardService _dashboardService;
    private readonly IStudentService _studentService;
    private readonly IRoomService _roomService;
    private readonly IBillingService _billingService;
    private readonly IComplaintService _complaintService;
    private readonly INoticeService _noticeService;

    public AdminController(
        IDashboardService dashboardService,
        IStudentService studentService,
        IRoomService roomService,
        IBillingService billingService,
        IComplaintService complaintService,
        INoticeService noticeService)
    {
        _dashboardService = dashboardService;
        _studentService = studentService;
        _roomService = roomService;
        _billingService = billingService;
        _complaintService = complaintService;
        _noticeService = noticeService;
    }

    private int GetAdminId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

    // GET: /Admin/Dashboard
    public async Task<IActionResult> Dashboard()
    {
        var model = new AdminDashboardViewModel
        {
            TotalStudents = await _dashboardService.GetTotalStudentsAsync(),
            TotalRooms = await _dashboardService.GetTotalRoomsAsync(),
            PendingComplaints = await _dashboardService.GetPendingComplaintsAsync(),
            DueAmount = await _dashboardService.GetDueAmountAsync(),
            RecentComplaints = await _dashboardService.GetRecentComplaintsAsync(),
            RecentNotices = await _dashboardService.GetRecentNoticesAsync(),
            RecentPayments = await _dashboardService.GetRecentPaymentsAsync()
        };

        return View(model);
    }

    // GET: /Admin/Residents
    public async Task<IActionResult> Residents(string? search)
    {
        var students = string.IsNullOrWhiteSpace(search)
            ? await _studentService.GetAllStudentsAsync()
            : await _studentService.SearchStudentsAsync(search);

        var rooms = await _roomService.GetAvailableRoomsAsync();

        var model = new ResidentListViewModel
        {
            Students = students,
            AvailableRooms = rooms,
            SearchTerm = search
        };

        return View(model);
    }

    // POST: /Admin/AddResident
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddResident(AddStudentViewModel model)
    {
        try
        {
            await _studentService.AddStudentAsync(model.Name, model.Email, model.Phone, model.Password, model.RoomId);
            TempData["Success"] = "Student added successfully!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction("Residents");
    }

    // POST: /Admin/EditResident
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditResident(EditStudentViewModel model)
    {
        try
        {
            await _studentService.UpdateStudentAsync(model.RegNo, model.Name, model.Email, model.Phone, model.RoomId);
            TempData["Success"] = "Student updated successfully!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction("Residents");
    }

    // POST: /Admin/DeleteResident
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteResident(string regNo)
    {
        try
        {
            await _studentService.DeleteStudentAsync(regNo);
            TempData["Success"] = "Student removed successfully!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction("Residents");
    }

    // POST: /Admin/ToggleStudentStatus
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStudentStatus(string regNo)
    {
        try
        {
            await _studentService.ToggleStudentStatusAsync(regNo);
            TempData["Success"] = "Student status updated!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction("Residents");
    }

    // GET: /Admin/GetStudentDetail
    public async Task<IActionResult> GetStudentDetail(string regNo)
    {
        var student = await _studentService.GetStudentByRegNoAsync(regNo);
        if (student == null) return NotFound();

        ViewBag.Bills = await _billingService.GetBillsByStudentAsync(regNo);
        ViewBag.Payments = await _billingService.GetPaymentsByStudentAsync(regNo);
        ViewBag.Complaints = await _complaintService.GetComplaintsByStudentAsync(regNo);

        return PartialView("_StudentDetail", student);
    }

    // GET: /Admin/Rooms
    public async Task<IActionResult> Rooms()
    {
        var rooms = await _roomService.GetAllRoomsAsync();
        var model = new RoomViewModel { Rooms = rooms };
        return View(model);
    }

    // POST: /Admin/AddRoom
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddRoom(AddRoomViewModel model)
    {
        try
        {
            await _roomService.AddRoomAsync(model.RoomNo, model.Floor, model.Capacity, model.Type);
            TempData["Success"] = "Room added successfully!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction("Rooms");
    }

    // POST: /Admin/EditRoom
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditRoom(int roomId, string roomNo, int floor, int capacity, string type)
    {
        try
        {
            await _roomService.UpdateRoomAsync(roomId, roomNo, floor, capacity, type);
            TempData["Success"] = "Room updated successfully!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction("Rooms");
    }

    // POST: /Admin/DeleteRoom
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteRoom(int roomId)
    {
        try
        {
            await _roomService.DeleteRoomAsync(roomId);
            TempData["Success"] = "Room deleted successfully!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction("Rooms");
    }

    // GET: /Admin/Billing
    public async Task<IActionResult> Billing(string tab = "bills")
    {
        var model = new BillingViewModel
        {
            Bills = await _billingService.GetAllBillsAsync(),
            Payments = await _billingService.GetAllPaymentsAsync(),
            TotalDue = await _billingService.GetTotalDueAmountAsync(),
            TotalCollected = await _billingService.GetTotalPaidAmountAsync(),
            ActiveTab = tab
        };
        return View(model);
    }

    // POST: /Admin/GenerateBills
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateBills(GenerateBillViewModel model)
    {
        try
        {
            await _billingService.GenerateBillsForMonthAsync(model.Month, model.Amount, model.DueDate);
            TempData["Success"] = $"Bills generated for {model.Month} successfully!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction("Billing");
    }

    // POST: /Admin/MarkBillPaid
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkBillPaid(int billId)
    {
        try
        {
            await _billingService.MarkBillAsPaidAsync(billId);
            TempData["Success"] = "Bill marked as paid!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction("Billing");
    }

    // GET: /Admin/DownloadPaymentHistory — CSV export
    public async Task<IActionResult> DownloadPaymentHistory()
    {
        var payments = await _billingService.GetAllPaymentsAsync();
        var sb = new StringBuilder();
        sb.AppendLine("Date,Student,RegNo,Bill Month,Room,Amount (PKR),Method,Reference,Status");
        foreach (var p in payments)
        {
            sb.AppendLine($"\"{p.Date:yyyy-MM-dd}\",\"{p.Student?.Name}\",\"{p.StudentId}\",\"{p.Bill?.Month}\",\"{p.Bill?.Room?.RoomNo}\",{p.Amount},\"{p.Method}\",\"{p.Reference ?? ""}\",\"{p.Status}\"");
        }
        var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
        return File(bytes, "text/csv", $"PaymentHistory_{DateTime.Now:yyyyMMdd}.csv");
    }

    // GET: /Admin/Complaints
    public async Task<IActionResult> Complaints(string? status)
    {
        var complaints = string.IsNullOrWhiteSpace(status) || status == "All"
            ? await _complaintService.GetAllComplaintsAsync()
            : await _complaintService.GetComplaintsByStatusAsync(status);

        var model = new ComplaintAdminViewModel
        {
            Complaints = complaints,
            StatusFilter = status
        };
        return View(model);
    }

    // POST: /Admin/UpdateComplaintStatus
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateComplaintStatus(int complaintId, string status)
    {
        try
        {
            await _complaintService.UpdateComplaintStatusAsync(complaintId, status);
            TempData["Success"] = "Complaint status updated!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction("Complaints");
    }

    // POST: /Admin/ReplyComplaint
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ReplyComplaint(int complaintId, string reply)
    {
        try
        {
            await _complaintService.ReplyToComplaintAsync(complaintId, reply);
            TempData["Success"] = "Reply sent successfully!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction("Complaints");
    }

    // GET: /Admin/Notices
    public async Task<IActionResult> Notices()
    {
        var notices = await _noticeService.GetAllNoticesAsync();
        return View(notices);
    }

    // POST: /Admin/PostNotice
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PostNotice(PostNoticeViewModel model)
    {
        try
        {
            var adminId = GetAdminId();
            await _noticeService.PostNoticeAsync(model.Title, model.Content, adminId, model.IsPinned, model.ExpiresAt);
            TempData["Success"] = "Notice posted successfully!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction("Notices");
    }

    // POST: /Admin/EditNotice
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditNotice(int noticeId, string title, string content, bool isPinned, DateTime? expiresAt)
    {
        try
        {
            await _noticeService.UpdateNoticeAsync(noticeId, title, content, isPinned, expiresAt);
            TempData["Success"] = "Notice updated successfully!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction("Notices");
    }

    // POST: /Admin/DeleteNotice
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteNotice(int noticeId)
    {
        try
        {
            await _noticeService.DeleteNoticeAsync(noticeId);
            TempData["Success"] = "Notice deleted successfully!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction("Notices");
    }

    // POST: /Admin/TogglePin
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TogglePin(int noticeId)
    {
        try
        {
            await _noticeService.TogglePinAsync(noticeId);
            TempData["Success"] = "Notice pin status updated!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction("Notices");
    }

    // GET: /Admin/Reports
    public async Task<IActionResult> Reports(string tab = "occupancy")
    {
        var model = new ReportViewModel
        {
            Rooms = await _roomService.GetRoomsWithStudentsAsync(),
            Students = await _studentService.GetAllStudentsAsync(),
            Bills = await _billingService.GetAllBillsAsync(),
            Payments = await _billingService.GetAllPaymentsAsync(),
            Complaints = await _complaintService.GetAllComplaintsAsync(),
            TotalCollected = await _billingService.GetTotalPaidAmountAsync(),
            TotalOutstanding = await _billingService.GetTotalDueAmountAsync(),
            ActiveTab = tab
        };
        return View(model);
    }
}
