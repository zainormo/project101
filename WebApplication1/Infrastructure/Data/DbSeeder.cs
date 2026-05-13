/*
 * ============================================================
 * SSMS CONNECTION GUIDE
 * ============================================================
 * Server:            (localdb)\mssqllocaldb
 * Authentication:    Windows Authentication
 * Database:          DormitoryMS
 *
 * Default login credentials seeded:
 *   Admin  — username: admin     / password: Admin@123
 *   Student— RegNo: REG2024001   / password: Student@123
 *
 * Key tables to query:
 *   SELECT * FROM Students
 *   SELECT * FROM Rooms
 *   SELECT * FROM Bills
 *   SELECT * FROM Payments
 *   SELECT * FROM Complaints
 *   SELECT * FROM Notices
 *   SELECT * FROM Admins
 * ============================================================
 */

using DormitoryMS.Core.Entities;
using DormitoryMS.Core.Enums;

namespace DormitoryMS.Infrastructure.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext context)
    {
        SeedAdmin(context);
        SeedRooms(context);
        SeedStudents(context);
        SeedNotices(context);
        SeedBillsAndPayments(context);
        SeedComplaints(context);
    }

    private static void SeedAdmin(AppDbContext context)
    {
        if (context.Admins.Any()) return;
        context.Admins.Add(Admin.Create(
            username: "admin",
            passwordHash: BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            fullName: "System Administrator",
            role: "SuperAdmin"
        ));
        context.SaveChanges();
    }

    private static void SeedRooms(AppDbContext context)
    {
        if (context.Rooms.Any()) return;
        var rooms = new[]
        {
            Room.Create("A-101", 1, 2, RoomType.Double),
            Room.Create("A-102", 1, 2, RoomType.Double),
            Room.Create("A-103", 1, 1, RoomType.Single),
            Room.Create("A-201", 2, 3, RoomType.Triple),
            Room.Create("A-202", 2, 2, RoomType.Double),
            Room.Create("B-101", 1, 2, RoomType.Double),
            Room.Create("B-102", 1, 3, RoomType.Triple),
            Room.Create("B-201", 2, 1, RoomType.Single),
        };
        context.Rooms.AddRange(rooms);
        context.SaveChanges();
    }

    private static void SeedStudents(AppDbContext context)
    {
        if (context.Students.Any()) return;
        var pw = BCrypt.Net.BCrypt.HashPassword("Student@123");
        var students = new[]
        {
            Student.Create("REG2024001", "Ali Hassan",     "03001234567", "ali@student.com",    pw),
            Student.Create("REG2024002", "Sara Ahmed",     "03001234568", "sara@student.com",   pw),
            Student.Create("REG2024003", "Usman Khan",     "03001234569", "usman@student.com",  pw),
            Student.Create("REG2024004", "Fatima Malik",   "03001234570", "fatima@student.com", pw),
            Student.Create("REG2024005", "Bilal Raza",     "03001234571", "bilal@student.com",  pw),
        };
        var rooms = context.Rooms.ToList();
        students[0].AssignRoom(rooms[0].RoomId);
        students[1].AssignRoom(rooms[0].RoomId);
        students[2].AssignRoom(rooms[1].RoomId);
        students[3].AssignRoom(rooms[2].RoomId);
        students[4].AssignRoom(rooms[3].RoomId);
        context.Students.AddRange(students);
        context.SaveChanges();
    }

    private static void SeedNotices(AppDbContext context)
    {
        if (context.Notices.Any()) return;
        var adminId = context.Admins.First().AdminId;
        context.Notices.AddRange(
            Notice.Create("Welcome to DMS", "Welcome to the new Dormitory Management System. Register and keep your profile updated.", adminId, isPinned: true),
            Notice.Create("Mess Timing Change", "Lunch will now be served from 12:30 PM to 2:00 PM starting Monday.", adminId, isPinned: false),
            Notice.Create("Maintenance Notice", "Hot water will be unavailable on Saturday 6 AM - 10 AM for pipe servicing.", adminId, isPinned: false)
        );
        context.SaveChanges();
    }

    private static void SeedBillsAndPayments(AppDbContext context)
    {
        if (context.Bills.Any()) return;
        var rooms = context.Rooms.Take(4).ToList();
        var bills = new List<Bill>();
        foreach (var room in rooms)
        {
            bills.Add(Bill.Create(room.RoomId, "April-2026", 8500m, DateTime.UtcNow.AddDays(-10)));
            bills.Add(Bill.Create(room.RoomId, "May-2026",   8500m, DateTime.UtcNow.AddDays(20)));
        }
        context.Bills.AddRange(bills);
        context.SaveChanges();

        var students = context.Students.ToList();
        var paidBill = bills[0];
        paidBill.MarkAsPaid();
        var payment = Payment.Create(paidBill.BillId, students[0].RegNo, paidBill.Amount, PaymentMethod.Online, "TXN-001");
        context.Payments.Add(payment);
        context.SaveChanges();
    }

    private static void SeedComplaints(AppDbContext context)
    {
        if (context.Complaints.Any()) return;
        var students = context.Students.ToList();
        if (!students.Any()) return;
        context.Complaints.AddRange(
            Complaint.Create(students[0].RegNo, students[0].RoomId, "AC not working", "The air conditioner in room A-101 stopped working since last week.", ComplaintCategory.Maintenance),
            Complaint.Create(students[1].RegNo, students[1].RoomId, "WiFi very slow",  "WiFi speed is extremely slow after 8 PM.", ComplaintCategory.WiFi),
            Complaint.Create(students[2].RegNo, students[2].RoomId, "Noise issue",     "Students in the corridor make noise after midnight.", ComplaintCategory.Noise)
        );
        context.SaveChanges();
    }
}
