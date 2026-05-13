using DormitoryMS.Core.Entities;
using DormitoryMS.Core.Interfaces;

namespace DormitoryMS.Core.Services;

public class NoticeService : INoticeService
{
    private readonly INoticeRepository _noticeRepo;

    public NoticeService(INoticeRepository noticeRepo)
    {
        _noticeRepo = noticeRepo;
    }

    public async Task<IEnumerable<Notice>> GetAllNoticesAsync()
        => await _noticeRepo.GetAllAsync();

    public async Task<IEnumerable<Notice>> GetActiveNoticesAsync()
        => await _noticeRepo.GetActiveNoticesAsync();

    public async Task<IEnumerable<Notice>> GetPinnedNoticesAsync()
        => await _noticeRepo.GetPinnedNoticesAsync();

    public async Task<IEnumerable<Notice>> GetRecentNoticesAsync(int count = 5)
        => await _noticeRepo.GetRecentNoticesAsync(count);

    public async Task<Notice?> GetNoticeByIdAsync(int noticeId)
        => await _noticeRepo.GetByIdAsync(noticeId);

    public async Task<Notice> PostNoticeAsync(string title, string content, int adminId, bool isPinned, DateTime? expiresAt)
    {
        var notice = Notice.Create(title, content, adminId, isPinned, expiresAt);
        await _noticeRepo.AddAsync(notice);
        return notice;
    }

    public async Task UpdateNoticeAsync(int noticeId, string title, string content, bool isPinned, DateTime? expiresAt)
    {
        var notice = await _noticeRepo.GetByIdAsync(noticeId)
            ?? throw new InvalidOperationException("Notice not found.");
        notice.Update(title, content, isPinned, expiresAt);
        await _noticeRepo.UpdateAsync(notice);
    }

    public async Task DeleteNoticeAsync(int noticeId)
    {
        await _noticeRepo.DeleteAsync(noticeId);
    }

    public async Task TogglePinAsync(int noticeId)
    {
        var notice = await _noticeRepo.GetByIdAsync(noticeId)
            ?? throw new InvalidOperationException("Notice not found.");
        notice.TogglePin();
        await _noticeRepo.UpdateAsync(notice);
    }
}
