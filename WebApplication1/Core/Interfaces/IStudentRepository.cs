using DormitoryMS.Core.Entities;

namespace DormitoryMS.Core.Interfaces;

public interface IStudentRepository : IRepository<Student>
{
    Task<Student?> GetByRegNoAsync(string regNo);
    Task<Student?> GetByEmailAsync(string email);
    Task<IEnumerable<Student>> GetByRoomIdAsync(int roomId);
    Task<IEnumerable<Student>> GetActiveStudentsAsync();
    Task<IEnumerable<Student>> SearchAsync(string searchTerm);
    Task<string> GenerateRegNoAsync();
}
