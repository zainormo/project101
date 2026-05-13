namespace DormitoryMS.Core.Entities;

public abstract class BaseEntity
{
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
}
