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

        foreach (var neighborId in currentRoom.Neighbors)
        {
            var neighbor = allRooms.FirstOrDefault(r => r.Id == neighborId);
            if (neighbor == null) continue;

            int dx = neighbor.X - currentRoom.X;
            int dy = neighbor.Y - currentRoom.Y;

            if (dx == 1) directions.Add("right");
            else if (dx == -1) directions.Add("left");
            else if (dy == 1) directions.Add("up");
            else if (dy == -1) directions.Add("down");
        }

        return directions;
    }

    public async Task<int?> GetNeighborRoomIdByDirection(Room currentRoom, List<Room> allRooms, string direction)
    {
        // 방향별 상대 좌표 정의
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

        // Neighbors 중 위치 일치하는 방 찾기
        foreach (var neighborId in currentRoom.Neighbors)
        {
            var neighbor = allRooms.FirstOrDefault(r => r.Id == neighborId);
            if (neighbor != null && neighbor.X == targetX && neighbor.Y == targetY)
            {
                return neighbor.Id;
            }
        }

        return null; // 해당 방향에 방이 없을 경우
    }

    public async Task<int?> MovePlayerAsync(string userId, string direction)
    {
        var playerData = await _memoryCacheService.GetPlayerDataAsync(userId);
        if (playerData == null)
        {
            throw new Exception("Player data not found.");
        }

        int currentRoomId = playerData.CurrentMapData.CurrentRoom;
        var currentRoom = playerData.CurrentMapData.Rooms[currentRoomId];   
        var allRooms = playerData.CurrentMapData.Rooms;

        // 방향에 따른 방 ID 가져오기
        int? targetRoomId = await GetNeighborRoomIdByDirection(currentRoom, allRooms, direction);
        if (targetRoomId == null)
        {
            throw new Exception("Invalid move direction.");
        }

        //TODO: 방 이동 시 몬스터와의 전투 처리
        
        // 방 방문 처리
        playerData.CurrentMapData.Rooms[currentRoomId].Visited = true;
        playerData.CurrentMapData.CurrentRoom = targetRoomId.Value;
        playerData.CurrentMapData.Rooms[targetRoomId.Value].Visited = true;
        // 방 이동
        playerData.CurrentMapData.CurrentRoom = targetRoomId.Value;

        // 플레이어 데이터 업데이트
        await _memoryCacheService.UpdatePlayerDataAsync(userId, playerData, TimeSpan.FromMinutes(30));

        var check = await _memoryCacheService.GetPlayerDataAsync(userId);
        Console.WriteLine($"[CHECK] Visited: {check.CurrentMapData.Rooms[targetRoomId.Value].Visited}");
        return targetRoomId.Value;
    }
}