using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DormitoryMS.Core.Entities;

public class Student : BaseEntity
{
    [Key]
    [MaxLength(20)]
    public string RegNo { get; private set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Name { get; private set; } = string.Empty;

    [Required]
    [MaxLength(15)]
    public string Phone { get; private set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Email { get; private set; } = string.Empty;

    [Required]
    [MaxLength(256)]
    public string PasswordHash { get; private set; } = string.Empty;

    public int? RoomId { get; private set; }

    [MaxLength(500)]
    public string? ProfileImage { get; private set; }

    public DateTime JoinDate { get; private set; } = DateTime.UtcNow;

    public bool IsActive { get; private set; } = true;

    // Navigation
    [ForeignKey("RoomId")]
    public virtual Room? Room { get; private set; }
    public virtual ICollection<Payment> Payments { get; private set; } = new List<Payment>();
    public virtual ICollection<Complaint> Complaints { get; private set; } = new List<Complaint>();

    private Student() { }

    public static Student Create(string regNo, string name, string phone, string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(regNo))
            throw new ArgumentException("Registration number is required.", nameof(regNo));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));

        return new Student
        {
            RegNo = regNo.Trim(),
            Name = name.Trim(),
            Phone = phone.Trim(),
            Email = email.Trim().ToLower(),
            PasswordHash = passwordHash,
            JoinDate = DateTime.UtcNow,
            IsActive = true
        };
    }

    public void Update(string name, string phone, string email)
    {
        Name = name.Trim();
        Phone = phone.Trim();
        Email = email.Trim().ToLower();
    }

    public void AssignRoom(int roomId) => RoomId = roomId;
    public void RemoveRoom() => RoomId = null;
    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
    public void ToggleActive() => IsActive = !IsActive;
    public void UpdatePassword(string newHash) => PasswordHash = newHash;
    public void SetProfileImage(string path) => ProfileImage = path;
}
