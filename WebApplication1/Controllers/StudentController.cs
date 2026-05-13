using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DormitoryMS.Core.Enums;
using DormitoryMS.Core.Interfaces;
using DormitoryMS.ViewModels.Student;

namespace DormitoryMS.Controllers;

[Authorize(Policy = "StudentOnly")]
public class StudentController : Controller
{
    private readonly IStudentService _studentService;
    private readonly INoticeService _noticeService;
    private readonly IBillingService _billingService;
    private readonly IComplaintService _complaintService;

    public StudentController(
        IStudentService studentService,
        INoticeService noticeService,
        IBillingService billingService,
        IComplaintService complaintService)
    {
        _studentService = studentService;
        _noticeService = noticeService;
        _billingService = billingService;
        _complaintService = complaintService;
    }

    private string GetRegNo() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
    private string GetStudentName() => User.FindFirstValue(ClaimTypes.Name) ?? "";

    // GET: /Student/Dashboard
    public async Task<IActionResult> Dashboard()
    {
        var regNo = GetRegNo();
        var student = await _studentService.GetStudentByRegNoAsync(regNo);
        var bills = await _billingService.GetBillsByStudentAsync(regNo);
        var complaints = await _complaintService.GetComplaintsByStudentAsync(regNo);
        var notices = await _noticeService.GetRecentNoticesAsync(3);

        var model = new StudentDashboardViewModel
        {
            StudentName = student?.Name ?? GetStudentName(),
            RegNo = regNo,
            RoomNo = student?.Room?.RoomNo,
            PendingBills = bills.Count(b => b.Status == BillStatus.Pending.ToString() || b.Status == BillStatus.Overdue.ToString()),
            ActiveComplaints = complaints.Count(c => c.Status != ComplaintStatus.Resolved.ToString() && c.Status != ComplaintStatus.Rejected.ToString()),
            UnreadNotices = notices.Count(),
            RecentNotices = notices
        };

        return View(model);
    }

    // GET: /Student/Notices
    public async Task<IActionResult> Notices()
    {
        var pinnedNotices = await _noticeService.GetPinnedNoticesAsync();
        var activeNotices = await _noticeService.GetActiveNoticesAsync();

        var model = new NoticeViewModel
        {
            PinnedNotices = pinnedNotices,
            RecentNotices = activeNotices.Where(n => !n.IsPinned)
        };

        return View(model);
    }

    // GET: /Student/Bills
    public async Task<IActionResult> Bills()
    {
        var regNo = GetRegNo();
        var bills = await _billingService.GetBillsByStudentAsync(regNo);
        var paymentHistory = await _billingService.GetPaymentsByStudentAsync(regNo);

        var model = new BillViewModel
        {
            Bills = bills,
            TotalDue = bills.Where(b => b.Status == BillStatus.Pending.ToString()).Sum(b => b.Amount),
            TotalPaid = bills.Where(b => b.Status == BillStatus.Paid.ToString()).Sum(b => b.Amount),
            TotalOverdue = bills.Where(b => b.Status == BillStatus.Overdue.ToString()).Sum(b => b.Amount),
            PaymentHistory = paymentHistory
        };

        return View(model);
    }

    // GET: /Student/DownloadPaymentHistory - CSV export
    public async Task<IActionResult> DownloadPaymentHistory()
    {
        var regNo = GetRegNo();
        var payments = await _billingService.GetPaymentsByStudentAsync(regNo);
        var sb = new StringBuilder();
        sb.AppendLine("Date,Bill Month,Room,Amount (PKR),Method,Reference,Status");
        foreach (var p in payments)
        {
            sb.AppendLine($"\"{p.Date:yyyy-MM-dd}\",\"{p.Bill?.Month}\",\"{p.Bill?.Room?.RoomNo}\",{p.Amount},\"{p.Method}\",\"{p.Reference ?? ""}\",\"{p.Status}\"");
        }
        var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
        return File(bytes, "text/csv", $"MyPayments_{DateTime.Now:yyyyMMdd}.csv");
    }

    // POST: /Student/PayBill
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PayBill(int billId, string method, string? reference)
    {
        try
        {
            var regNo = GetRegNo();
            var bill = await _billingService.GetBillByIdAsync(billId);
            if (bill == null)
            {
                TempData["Error"] = "Bill not found.";
                return RedirectToAction("Bills");
            }

            await _billingService.ProcessPaymentAsync(billId, regNo, bill.Amount, method, reference);
            TempData["Success"] = "Payment processed successfully!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("Bills");
    }

    // GET: /Student/Complaints
    public async Task<IActionResult> Complaints()
    {
        var regNo = GetRegNo();
        var student = await _studentService.GetStudentByRegNoAsync(regNo);
        var complaints = await _complaintService.GetComplaintsByStudentAsync(regNo);

        var model = new ComplaintViewModel
        {
            Complaints = complaints,
            StudentRoomId = student?.RoomId
        };

        return View(model);
    }

    // POST: /Student/SubmitComplaint
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitComplaint(string issue, string? description, string category)
    {
        try
        {
            var regNo = GetRegNo();
            var student = await _studentService.GetStudentByRegNoAsync(regNo);

            await _complaintService.SubmitComplaintAsync(regNo, student?.RoomId, issue, description, category);
            TempData["Success"] = "Complaint submitted successfully!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("Complaints");
    }

    // GET: /Student/AboutHostel
    public IActionResult AboutHostel()
    {
        return View();
    }
}
