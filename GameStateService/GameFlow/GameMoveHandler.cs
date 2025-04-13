using GameStateService.Models;
using GameStateService.Utils;
using GameStateService.Services;

public class GameMoveHandler
{
    private readonly MemoryCacheService _memoryCacheService;
    private readonly GameFlowManager _gameFlowManager;

    public GameMoveHandler(MemoryCacheService memoryCacheService, GameFlowManager gameFlowManager)
    {
        _memoryCacheService = memoryCacheService;
        _gameFlowManager = gameFlowManager;
    }

    public async Task<List<string>> GetNeighborRoomDirections(Room currentRoom, List<Room> allRooms)
    {
        var directions = new List<string>();

        var neighbors = currentRoom.Neighbors
            .Select(id => allRooms.FirstOrDefault(r => r.Id == id))
            .Where(r => r != null)
            .ToList();

        // left
        if (neighbors.Any(r => r.X - currentRoom.X == -1 && r.Y == currentRoom.Y))
            directions.Add("left");
        // right
        if (neighbors.Any(r => r.X - currentRoom.X == 1 && r.Y == currentRoom.Y))
            directions.Add("right");
        // up
        if (neighbors.Any(r => r.Y - currentRoom.Y == 1 && r.X == currentRoom.X))
            directions.Add("up");
        // down
        if (neighbors.Any(r => r.Y - currentRoom.Y == -1 && r.X == currentRoom.X))
            directions.Add("down");

        return directions;
    }

    public async Task<int?> GetNeighborRoomIdByDirection(Room currentRoom, List<Room> allRooms, string direction)
    {
        var directionOffsets = new Dictionary<string, (int dx, int dy)>
        {
            { "up", (0, 1) },
            { "down", (0, -1) },
            { "right",  (1, 0) },
            { "left",  (-1, 0) }
        };

        if (!directionOffsets.ContainsKey(direction.ToLower()))
            return null;

        var (dx, dy) = directionOffsets[direction.ToLower()];
        int targetX = currentRoom.X + dx;
        int targetY = currentRoom.Y + dy;

        foreach (var neighborId in currentRoom.Neighbors)
        {
            var neighbor = allRooms.FirstOrDefault(r => r.Id == neighborId);
            if (neighbor != null && neighbor.X == targetX && neighbor.Y == targetY)
            {
                return neighbor.Id;
            }
        }

        return null;
    }

    public async Task<int?> MovePlayerAsync(string userId, string direction)
    {
        var playerData = await _memoryCacheService.GetPlayerDataAsync(userId);
        if (playerData == null)
        {
            throw new UserErrorException($"Player: {userId} data not found.");
        }

        int currentRoomId = playerData.CurrentMapData.CurrentRoom;
        var currentRoom = playerData.CurrentMapData.Rooms[currentRoomId];   
        var allRooms = playerData.CurrentMapData.Rooms;

        int? targetRoomId = await GetNeighborRoomIdByDirection(currentRoom, allRooms, direction);
        if (targetRoomId == null)
        {
            throw new Exception("Invalid move direction.");
        }
        
        playerData.CurrentMapData.Rooms[currentRoomId].Visited = true;
        playerData.CurrentMapData.CurrentRoom = targetRoomId.Value;
        playerData.CurrentMapData.Rooms[targetRoomId.Value].Visited = true;
        playerData.CurrentMapData.CurrentRoom = targetRoomId.Value;

        await _memoryCacheService.UpdatePlayerDataAsync(userId, playerData, TimeSpan.FromMinutes(30));
        return targetRoomId.Value;
    }
}