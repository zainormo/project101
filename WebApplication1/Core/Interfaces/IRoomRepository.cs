using DormitoryMS.Core.Entities;

namespace DormitoryMS.Core.Interfaces;

public interface IRoomRepository : IRepository<Room>
{
    Task<Room?> GetByRoomNoAsync(string roomNo);
    Task<IEnumerable<Room>> GetActiveRoomsAsync();
    Task<IEnumerable<Room>> GetAvailableRoomsAsync();
    Task<IEnumerable<Room>> GetRoomsWithStudentsAsync();
}
