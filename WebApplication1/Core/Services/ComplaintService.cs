using DormitoryMS.Core.Entities;
using DormitoryMS.Core.Enums;
using DormitoryMS.Core.Interfaces;

namespace DormitoryMS.Core.Services;

public class ComplaintService : IComplaintService
{
    private readonly IComplaintRepository _complaintRepo;

    public ComplaintService(IComplaintRepository complaintRepo)
    {
        _complaintRepo = complaintRepo;
    }

    public async Task<IEnumerable<Complaint>> GetAllComplaintsAsync()
        => await _complaintRepo.GetAllWithDetailsAsync();

    public async Task<IEnumerable<Complaint>> GetComplaintsByStudentAsync(string studentId)
        => await _complaintRepo.GetByStudentIdAsync(studentId);

    public async Task<IEnumerable<Complaint>> GetComplaintsByStatusAsync(string status)
        => await _complaintRepo.GetByStatusAsync(status);

    public async Task<Complaint?> GetComplaintByIdAsync(int complaintId)
        => await _complaintRepo.GetByIdAsync(complaintId);

    public async Task<Complaint> SubmitComplaintAsync(string studentId, int? roomId, string issue, string? description, string category)
    {
        var cat = Enum.Parse<ComplaintCategory>(category);
        var complaint = Complaint.Create(studentId, roomId, issue, description, cat);
        await _complaintRepo.AddAsync(complaint);
        return complaint;
    }

    public async Task UpdateComplaintStatusAsync(int complaintId, string status)
    {
        var complaint = await _complaintRepo.GetByIdAsync(complaintId)
            ?? throw new InvalidOperationException("Complaint not found.");
        var newStatus = Enum.Parse<ComplaintStatus>(status);
        complaint.UpdateStatus(newStatus);
        await _complaintRepo.UpdateAsync(complaint);
    }

    public async Task ReplyToComplaintAsync(int complaintId, string reply)
    {
        var complaint = await _complaintRepo.GetByIdAsync(complaintId)
            ?? throw new InvalidOperationException("Complaint not found.");
        complaint.Reply(reply);
        await _complaintRepo.UpdateAsync(complaint);
    }

    public async Task<int> GetPendingCountAsync()
        => await _complaintRepo.GetPendingCountAsync();

    public async Task<IEnumerable<Complaint>> GetRecentComplaintsAsync(int count = 5)
        => await _complaintRepo.GetRecentComplaintsAsync(count);
}
