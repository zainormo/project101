using DormitoryMS.Core.Entities;
using DormitoryMS.Core.Enums;
using DormitoryMS.Core.Interfaces;

namespace DormitoryMS.Core.Services;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepo;

    public RoomService(IRoomRepository roomRepo)
    {
        _roomRepo = roomRepo;
    }

    public async Task<IEnumerable<Room>> GetAllRoomsAsync()
        => await _roomRepo.GetAllAsync();

    public async Task<IEnumerable<Room>> GetActiveRoomsAsync()
        => await _roomRepo.GetActiveRoomsAsync();

    public async Task<IEnumerable<Room>> GetAvailableRoomsAsync()
        => await _roomRepo.GetAvailableRoomsAsync();

    public async Task<IEnumerable<Room>> GetRoomsWithStudentsAsync()
        => await _roomRepo.GetRoomsWithStudentsAsync();

    public async Task<Room?> GetRoomByIdAsync(int roomId)
        => await _roomRepo.GetByIdAsync(roomId);

    public async Task<Room> AddRoomAsync(string roomNo, int floor, int capacity, string type)
    {
        var existing = await _roomRepo.GetByRoomNoAsync(roomNo);
        if (existing != null)
            throw new InvalidOperationException("A room with this number already exists.");

        var roomType = Enum.Parse<RoomType>(type);
        var room = Room.Create(roomNo, floor, capacity, roomType);
        await _roomRepo.AddAsync(room);
        return room;
    }

    public async Task UpdateRoomAsync(int roomId, string roomNo, int floor, int capacity, string type)
    {
        var room = await _roomRepo.GetByIdAsync(roomId)
            ?? throw new InvalidOperationException("Room not found.");

        var roomType = Enum.Parse<RoomType>(type);
        room.Update(roomNo, floor, capacity, roomType);
        await _roomRepo.UpdateAsync(room);
    }

    public async Task DeleteRoomAsync(int roomId)
    {
        await _roomRepo.DeleteAsync(roomId);
    }

    public async Task ToggleRoomStatusAsync(int roomId)
    {
        var room = await _roomRepo.GetByIdAsync(roomId)
            ?? throw new InvalidOperationException("Room not found.");
        room.ToggleActive();
        await _roomRepo.UpdateAsync(room);
    }
}
