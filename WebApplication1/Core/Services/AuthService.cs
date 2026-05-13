using DormitoryMS.Core.Entities;
using DormitoryMS.Core.Interfaces;

namespace DormitoryMS.Core.Services;

public class AuthService : IAuthService
{
    private readonly IStudentRepository _studentRepo;
    private readonly IAdminRepository _adminRepo;

    public AuthService(IStudentRepository studentRepo, IAdminRepository adminRepo)
    {
        _studentRepo = studentRepo;
        _adminRepo = adminRepo;
    }

    public async Task<Admin?> LoginAdminAsync(string username, string password)
    {
        var admin = await _adminRepo.GetByUsernameAsync(username);
        if (admin == null) return null;
        if (!VerifyPassword(password, admin.PasswordHash)) return null;
        return admin;
    }

    public async Task<Student?> LoginStudentAsync(string regNo, string password)
    {
        var student = await _studentRepo.GetByRegNoAsync(regNo);
        if (student == null || !student.IsActive) return null;
        if (!VerifyPassword(password, student.PasswordHash)) return null;
        return student;
    }

    public async Task<Student> RegisterStudentAsync(string name, string email, string phone, string password)
    {
        var existing = await _studentRepo.GetByEmailAsync(email);
        if (existing != null)
            throw new InvalidOperationException("A student with this email already exists.");

        var regNo = await _studentRepo.GenerateRegNoAsync();
        var hash = HashPassword(password);
        var student = Student.Create(regNo, name, phone, email, hash);
        await _studentRepo.AddAsync(student);
        return student;
    }

    public async Task<bool> ChangePasswordAsync(string userId, string oldPassword, string newPassword, bool isAdmin = false)
    {
        if (isAdmin)
        {
            if (!int.TryParse(userId, out int adminId)) return false;
            var admin = await _adminRepo.GetByIdAsync(adminId);
            if (admin == null) return false;
            if (!VerifyPassword(oldPassword, admin.PasswordHash)) return false;
            admin.UpdatePassword(HashPassword(newPassword));
            await _adminRepo.UpdateAsync(admin);
            return true;
        }
        else
        {
            var student = await _studentRepo.GetByRegNoAsync(userId);
            if (student == null) return false;
            if (!VerifyPassword(oldPassword, student.PasswordHash)) return false;
            student.UpdatePassword(HashPassword(newPassword));
            await _studentRepo.UpdateAsync(student);
            return true;
        }
    }

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
