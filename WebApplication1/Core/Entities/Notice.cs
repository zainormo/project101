using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DormitoryMS.Core.Entities;

public class Notice : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int NoticeId { get; private set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; private set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string Content { get; private set; } = string.Empty;

    public bool IsPinned { get; private set; } = false;

    public DateTime? ExpiresAt { get; private set; }

    public int AdminId { get; private set; }

    // Navigation
    [ForeignKey("AdminId")]
    public virtual Admin Admin { get; private set; } = null!;

    private Notice() { }

    public static Notice Create(string title, string content, int adminId, bool isPinned = false, DateTime? expiresAt = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.", nameof(title));
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content is required.", nameof(content));

        return new Notice
        {
            Title = title.Trim(),
            Content = content.Trim(),
            AdminId = adminId,
            IsPinned = isPinned,
            ExpiresAt = expiresAt
        };
    }

    public void Update(string title, string content, bool isPinned, DateTime? expiresAt)
    {
        Title = title.Trim();
        Content = content.Trim();
        IsPinned = isPinned;
        ExpiresAt = expiresAt;
    }

    public void TogglePin() => IsPinned = !IsPinned;
    public void Pin() => IsPinned = true;
    public void Unpin() => IsPinned = false;

    public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value < DateTime.UtcNow;
}
