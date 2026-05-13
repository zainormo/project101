using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DormitoryMS.Core.Entities;

public class Admin
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int AdminId { get; private set; }

    [Required]
    [MaxLength(50)]
    public string Username { get; private set; } = string.Empty;

    [Required]
    [MaxLength(256)]
    public string PasswordHash { get; private set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string FullName { get; private set; } = string.Empty;

    [Required]
    [MaxLength(30)]
    public string Role { get; private set; } = "Admin";

    public bool IsActive { get; private set; } = true;

    // Navigation
    public virtual ICollection<Notice> Notices { get; private set; } = new List<Notice>();

    private Admin() { }

    public static Admin Create(string username, string passwordHash, string fullName, string role = "Admin")
    {
        return new Admin
        {
            Username = username.Trim().ToLower(),
            PasswordHash = passwordHash,
            FullName = fullName.Trim(),
            Role = role,
            IsActive = true
        };
    }

    public void UpdatePassword(string newHash) => PasswordHash = newHash;
    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
