using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DormitoryMS.Core.Enums;

namespace DormitoryMS.Core.Entities;

public class Bill : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int BillId { get; private set; }

    public int RoomId { get; private set; }

    [Required]
    [MaxLength(20)]
    public string Month { get; private set; } = string.Empty;

    [Column(TypeName = "decimal(10,2)")]
    public decimal Amount { get; private set; }

    public DateTime DueDate { get; private set; }

    [Required]
    [MaxLength(20)]
    public string Status { get; private set; } = BillStatus.Pending.ToString();

    // Navigation
    [ForeignKey("RoomId")]
    public virtual Room Room { get; private set; } = null!;
    public virtual ICollection<Payment> Payments { get; private set; } = new List<Payment>();

    private Bill() { }

    public static Bill Create(int roomId, string month, decimal amount, DateTime dueDate)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be positive.", nameof(amount));

        return new Bill
        {
            RoomId = roomId,
            Month = month.Trim(),
            Amount = amount,
            DueDate = dueDate,
            Status = BillStatus.Pending.ToString()
        };
    }

    public void MarkAsPaid()
    {
        Status = BillStatus.Paid.ToString();
    }

    public void MarkAsOverdue()
    {
        if (Status == BillStatus.Pending.ToString() && DueDate < DateTime.UtcNow)
            Status = BillStatus.Overdue.ToString();
    }

    public void UpdateAmount(decimal amount) => Amount = amount;

    public BillStatus GetBillStatus()
    {
        return Enum.Parse<BillStatus>(Status);
    }
}
