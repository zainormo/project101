using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DormitoryMS.Core.Enums;

namespace DormitoryMS.Core.Entities;

public class Payment : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PaymentId { get; private set; }

    public int BillId { get; private set; }

    [Required]
    [MaxLength(20)]
    public string StudentId { get; private set; } = string.Empty;

    [Column(TypeName = "decimal(10,2)")]
    public decimal Amount { get; private set; }

    public DateTime Date { get; private set; } = DateTime.UtcNow;

    [Required]
    [MaxLength(30)]
    public string Method { get; private set; } = PaymentMethod.Cash.ToString();

    [Required]
    [MaxLength(20)]
    public string Status { get; private set; } = "Completed";

    [MaxLength(100)]
    public string? Reference { get; private set; }

    // Navigation
    [ForeignKey("BillId")]
    public virtual Bill Bill { get; private set; } = null!;

    [ForeignKey("StudentId")]
    public virtual Student Student { get; private set; } = null!;

    private Payment() { }

    public static Payment Create(int billId, string studentId, decimal amount, PaymentMethod method, string? reference = null)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be positive.", nameof(amount));

        return new Payment
        {
            BillId = billId,
            StudentId = studentId,
            Amount = amount,
            Date = DateTime.UtcNow,
            Method = method.ToString(),
            Status = "Completed",
            Reference = reference
        };
    }
}
