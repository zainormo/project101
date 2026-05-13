using DormitoryMS.Core.Entities;
using DormitoryMS.Core.Enums;
using DormitoryMS.Core.Interfaces;

namespace DormitoryMS.Core.Services;

public class BillingService : IBillingService
{
    private readonly IBillRepository _billRepo;
    private readonly IPaymentRepository _paymentRepo;
    private readonly IRoomRepository _roomRepo;

    public BillingService(IBillRepository billRepo, IPaymentRepository paymentRepo, IRoomRepository roomRepo)
    {
        _billRepo = billRepo;
        _paymentRepo = paymentRepo;
        _roomRepo = roomRepo;
    }

    public async Task<IEnumerable<Bill>> GetAllBillsAsync()
        => await _billRepo.GetAllAsync();

    public async Task<IEnumerable<Bill>> GetBillsByStudentAsync(string regNo)
        => await _billRepo.GetBillsByStudentAsync(regNo);

    public async Task<IEnumerable<Bill>> GetBillsByStatusAsync(string status)
        => await _billRepo.GetByStatusAsync(status);

    public async Task<IEnumerable<Bill>> GetOverdueBillsAsync()
        => await _billRepo.GetOverdueBillsAsync();

    public async Task<Bill?> GetBillByIdAsync(int billId)
        => await _billRepo.GetByIdAsync(billId);

    public async Task GenerateBillsForMonthAsync(string month, decimal amount, DateTime dueDate)
    {
        var rooms = await _roomRepo.GetActiveRoomsAsync();
        foreach (var room in rooms)
        {
            // Check if bill already exists for this room and month
            var existingBills = await _billRepo.GetByRoomIdAsync(room.RoomId);
            if (existingBills.Any(b => b.Month == month))
                continue;

            var bill = Bill.Create(room.RoomId, month, amount, dueDate);
            await _billRepo.AddAsync(bill);
        }
    }

    public async Task MarkBillAsPaidAsync(int billId)
    {
        var bill = await _billRepo.GetByIdAsync(billId)
            ?? throw new InvalidOperationException("Bill not found.");
        bill.MarkAsPaid();
        await _billRepo.UpdateAsync(bill);
    }

    public async Task<Payment> ProcessPaymentAsync(int billId, string studentId, decimal amount, string method, string? reference)
    {
        var bill = await _billRepo.GetByIdAsync(billId)
            ?? throw new InvalidOperationException("Bill not found.");

        var paymentMethod = Enum.Parse<PaymentMethod>(method);
        var payment = Payment.Create(billId, studentId, amount, paymentMethod, reference);
        await _paymentRepo.AddAsync(payment);

        bill.MarkAsPaid();
        await _billRepo.UpdateAsync(bill);

        return payment;
    }

    public async Task<IEnumerable<Payment>> GetPaymentsByStudentAsync(string studentId)
        => await _paymentRepo.GetByStudentIdAsync(studentId);

    public async Task<IEnumerable<Payment>> GetAllPaymentsAsync()
        => await _paymentRepo.GetAllAsync();

    public async Task<IEnumerable<Payment>> GetRecentPaymentsAsync(int count = 10)
        => await _paymentRepo.GetRecentPaymentsAsync(count);

    public async Task<decimal> GetTotalDueAmountAsync()
        => await _billRepo.GetTotalDueAmountAsync();

    public async Task<decimal> GetTotalPaidAmountAsync(string? month = null)
        => await _billRepo.GetTotalPaidAmountAsync(month);
}
