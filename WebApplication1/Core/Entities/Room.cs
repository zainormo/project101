using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DormitoryMS.Core.Enums;

namespace DormitoryMS.Core.Entities;

public class Room
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int RoomId { get; private set; }

    [Required]
    [MaxLength(10)]
    public string RoomNo { get; private set; } = string.Empty;

    public int Floor { get; private set; }

    public int Capacity { get; private set; } = 2;

    [Required]
    [MaxLength(20)]
    public string Type { get; private set; } = RoomType.Double.ToString();

    public bool IsActive { get; private set; } = true;

    // Navigation
    public virtual ICollection<Student> Students { get; private set; } = new List<Student>();
    public virtual ICollection<Bill> Bills { get; private set; } = new List<Bill>();
    public virtual ICollection<Complaint> Complaints { get; private set; } = new List<Complaint>();

    private Room() { }

    public static Room Create(string roomNo, int floor, int capacity, RoomType type)
    {
        if (string.IsNullOrWhiteSpace(roomNo))
            throw new ArgumentException("Room number is required.", nameof(roomNo));
        if (floor < 0)
            throw new ArgumentException("Floor must be non-negative.", nameof(floor));
        if (capacity < 1)
            throw new ArgumentException("Capacity must be at least 1.", nameof(capacity));

        return new Room
        {
            RoomNo = roomNo.Trim(),
            Floor = floor,
            Capacity = capacity,
            Type = type.ToString(),
            IsActive = true
        };
    }

    public void Update(string roomNo, int floor, int capacity, RoomType type)
    {
        RoomNo = roomNo.Trim();
        Floor = floor;
        Capacity = capacity;
        Type = type.ToString();
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
    public void ToggleActive() => IsActive = !IsActive;

    public int OccupiedCount => Students.Count(s => s.IsActive);
    public int AvailableSlots => Capacity - OccupiedCount;
    public bool IsFull => OccupiedCount >= Capacity;
}
