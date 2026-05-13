using Microsoft.EntityFrameworkCore;
using DormitoryMS.Core.Entities;

namespace DormitoryMS.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Student> Students => Set<Student>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Bill> Bills => Set<Bill>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Complaint> Complaints => Set<Complaint>();
    public DbSet<Notice> Notices => Set<Notice>();
    public DbSet<Admin> Admins => Set<Admin>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Student
        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.RegNo);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.RegNo).HasMaxLength(20);
            entity.HasOne(e => e.Room)
                  .WithMany(r => r.Students)
                  .HasForeignKey(e => e.RoomId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Room
        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomId);
            entity.HasIndex(e => e.RoomNo).IsUnique();
        });

        // Bill
        modelBuilder.Entity<Bill>(entity =>
        {
            entity.HasKey(e => e.BillId);
            entity.HasOne(e => e.Room)
                  .WithMany(r => r.Bills)
                  .HasForeignKey(e => e.RoomId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Payment
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId);
            entity.HasOne(e => e.Bill)
                  .WithMany(b => b.Payments)
                  .HasForeignKey(e => e.BillId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Student)
                  .WithMany(s => s.Payments)
                  .HasForeignKey(e => e.StudentId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Complaint
        modelBuilder.Entity<Complaint>(entity =>
        {
            entity.HasKey(e => e.ComplaintId);
            entity.HasOne(e => e.Student)
                  .WithMany(s => s.Complaints)
                  .HasForeignKey(e => e.StudentId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Room)
                  .WithMany(r => r.Complaints)
                  .HasForeignKey(e => e.RoomId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Notice
        modelBuilder.Entity<Notice>(entity =>
        {
            entity.HasKey(e => e.NoticeId);
            entity.HasOne(e => e.Admin)
                  .WithMany(a => a.Notices)
                  .HasForeignKey(e => e.AdminId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Admin
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.AdminId);
            entity.HasIndex(e => e.Username).IsUnique();
        });
    }
}
