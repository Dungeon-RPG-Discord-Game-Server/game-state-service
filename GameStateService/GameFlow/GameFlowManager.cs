using GameStateService.Models;
using GameStateService.Utils;
using GameStateService.Services;

public class GameFlowManager
{
    private readonly MemoryCacheService _memoryCacheService;

    public GameFlowManager(MemoryCacheService memoryCacheService)
    {
        _memoryCacheService = memoryCacheService;
    }

    public async Task<string> GetPlayerSummaryAsync(string userId)
    {
        var playerData = await _memoryCacheService.GetPlayerDataAsync(userId);
        if (playerData == null)
        {
            throw new Exception("Player data not found.");
        }

        return playerData.ToString();
    }

    public async Task<string> GetMapStringAsync(MapData map)
    {
        string mapString = MapGenerator.VisualizeMap(map.Rooms, map.CurrentRoom);

        return mapString;
    }

    public async Task<GameStateType> GetCurrentGameStateAsync(string userId)
    {
        var playerData = await _memoryCacheService.GetPlayerDataAsync(userId);
        if (playerData == null)
        {
            throw new Exception("Player data not found.");
        }

        return playerData.CurrentGameState;
    }

    public async Task ChangeGameStateAsync(string userId, GameStateType newState)
    {
        var playerData = await _memoryCacheService.GetPlayerDataAsync(userId);
        if (playerData == null)
        {
            throw new Exception("Player data not found.");
        }

        playerData.CurrentGameState = newState;
        await _memoryCacheService.UpdatePlayerDataAsync(userId, playerData, TimeSpan.FromMinutes(30));
    }
    public async Task AssginNewMapToPlayerAsync(string userId, string mapName, int mapLevel)
    {
        var playerData = await _memoryCacheService.GetPlayerDataAsync(userId);
        if (playerData == null)
        {
            throw new Exception("Player data not found.");
        }

        var map = MapLoader.LoadNewMapAsync(mapName, mapLevel);

        playerData.CurrentMapData = map;
        await _memoryCacheService.UpdatePlayerDataAsync(userId, playerData, TimeSpan.FromMinutes(30));
    }
    public async Task<PlayerData> StartNewGameAsync(string userId, int weaponType)
    {
        bool alreadyRegistered = await _memoryCacheService.GetPlayerDataAsync(userId) != null;
        if (alreadyRegistered)
        {
            await _memoryCacheService.RemovePlayerDataAsync(userId);
        }
        await _memoryCacheService.RegisterPlayerDataAsync(userId, (WeaponType)weaponType,TimeSpan.FromMinutes(30));
        var playerData = await _memoryCacheService.GetPlayerDataAsync(userId);

        return playerData;
    }

    public async Task<PlayerData> EnterNewDungeonAsync(string userId)
    {
        var playerData = await _memoryCacheService.GetPlayerDataAsync(userId);
        if (playerData == null)
        {
            throw new Exception("Player data not found.");
        }

        await AssginNewMapToPlayerAsync(userId, "Tutorial", 1);
        await ChangeGameStateAsync(userId, GameStateType.ExplorationState);

        return playerData;
    }

    public async Task UpdatePlayerStatsAsync(string userId, int health, int mana, int experience)
    {
        var playerData = await _memoryCacheService.GetPlayerDataAsync(userId);
        if (playerData == null)
        {
            throw new Exception("Player data not found.");
        }

        playerData.Health += health;
        playerData.Mana += mana;
        playerData.Experience += experience;

        await _memoryCacheService.UpdatePlayerDataAsync(userId, playerData, TimeSpan.FromMinutes(30));
    }
}