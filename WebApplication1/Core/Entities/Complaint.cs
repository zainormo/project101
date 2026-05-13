using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DormitoryMS.Core.Enums;

namespace DormitoryMS.Core.Entities;

public class Complaint : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ComplaintId { get; private set; }

    [Required]
    [MaxLength(20)]
    public string StudentId { get; private set; } = string.Empty;

    public int? RoomId { get; private set; }

    [Required]
    [MaxLength(200)]
    public string Issue { get; private set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; private set; }

    [Required]
    [MaxLength(50)]
    public string Category { get; private set; } = ComplaintCategory.Other.ToString();

    [Required]
    [MaxLength(20)]
    public string Status { get; private set; } = ComplaintStatus.Pending.ToString();

    [MaxLength(500)]
    public string? AdminReply { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    // Navigation
    [ForeignKey("StudentId")]
    public virtual Student Student { get; private set; } = null!;

    [ForeignKey("RoomId")]
    public virtual Room? Room { get; private set; }

    private Complaint() { }

    public static Complaint Create(string studentId, int? roomId, string issue, string? description, ComplaintCategory category)
    {
        if (string.IsNullOrWhiteSpace(issue))
            throw new ArgumentException("Issue is required.", nameof(issue));

        return new Complaint
        {
            StudentId = studentId,
            RoomId = roomId,
            Issue = issue.Trim(),
            Description = description?.Trim(),
            Category = category.ToString(),
            Status = ComplaintStatus.Pending.ToString()
        };
    }

    public void UpdateStatus(ComplaintStatus newStatus)
    {
        Status = newStatus.ToString();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Resolve(string reply)
    {
        AdminReply = reply;
        Status = ComplaintStatus.Resolved.ToString();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject(string reply)
    {
        AdminReply = reply;
        Status = ComplaintStatus.Rejected.ToString();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reply(string reply)
    {
        AdminReply = reply;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkInProgress()
    {
        Status = ComplaintStatus.InProgress.ToString();
        UpdatedAt = DateTime.UtcNow;
    }
}
