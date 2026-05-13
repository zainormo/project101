using DormitoryMS.Core.Entities;
using DormitoryMS.Core.Interfaces;

namespace DormitoryMS.Core.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _studentRepo;
    private readonly IAuthService _authService;

    public StudentService(IStudentRepository studentRepo, IAuthService authService)
    {
        _studentRepo = studentRepo;
        _authService = authService;
    }

    public async Task<IEnumerable<Student>> GetAllStudentsAsync()
        => await _studentRepo.GetAllAsync();

    public async Task<IEnumerable<Student>> GetActiveStudentsAsync()
        => await _studentRepo.GetActiveStudentsAsync();

    public async Task<Student?> GetStudentByRegNoAsync(string regNo)
        => await _studentRepo.GetByRegNoAsync(regNo);

    public async Task<Student> AddStudentAsync(string name, string email, string phone, string password, int? roomId)
    {
        var existing = await _studentRepo.GetByEmailAsync(email);
        if (existing != null)
            throw new InvalidOperationException("A student with this email already exists.");

        var regNo = await _studentRepo.GenerateRegNoAsync();
        var hash = _authService.HashPassword(password);
        var student = Student.Create(regNo, name, phone, email, hash);
        if (roomId.HasValue)
            student.AssignRoom(roomId.Value);
        await _studentRepo.AddAsync(student);
        return student;
    }

    public async Task UpdateStudentAsync(string regNo, string name, string email, string phone, int? roomId)
    {
        var student = await _studentRepo.GetByRegNoAsync(regNo)
            ?? throw new InvalidOperationException("Student not found.");

        student.Update(name, phone, email);
        if (roomId.HasValue)
            student.AssignRoom(roomId.Value);
        else
            student.RemoveRoom();

        await _studentRepo.UpdateAsync(student);
    }

    public async Task ToggleStudentStatusAsync(string regNo)
    {
        var student = await _studentRepo.GetByRegNoAsync(regNo)
            ?? throw new InvalidOperationException("Student not found.");
        student.ToggleActive();
        await _studentRepo.UpdateAsync(student);
    }

    public async Task DeleteStudentAsync(string regNo)
    {
        await _studentRepo.DeleteAsync(regNo);
    }

    public async Task<IEnumerable<Student>> SearchStudentsAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await _studentRepo.GetAllAsync();
        return await _studentRepo.SearchAsync(searchTerm);
    }

    public async Task AssignRoomAsync(string regNo, int roomId)
    {
        var student = await _studentRepo.GetByRegNoAsync(regNo)
            ?? throw new InvalidOperationException("Student not found.");
        student.AssignRoom(roomId);
        await _studentRepo.UpdateAsync(student);
    }
}
