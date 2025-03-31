using StackExchange.Redis;
using System.Text.Json;
using System;
using System.Threading.Tasks;

using GameStateService.Models;

public class RedisManager{
    private readonly ConnectionMultiplexer _redis;
    private readonly IDatabase _db;

    public RedisManager(string connectionString)
    {
        _redis = ConnectionMultiplexer.Connect(connectionString);
        _db = _redis.GetDatabase();
    }

    // 캐시에 값 설정 (예: 플레이어 상태)
    public async Task SetPlayerDataAsync(string playerId, PlayerData data, double expiry = 30)
    {
        // 예: 플레이어 상태를 키 "player:{playerId}"로 저장, 30분 만료
        string jsonData = JsonSerializer.Serialize(data);
        await _db.StringSetAsync($"player:{playerId}", jsonData, TimeSpan.FromMinutes(expiry));
    }

    // 캐시에서 값 가져오기
    public async Task<PlayerData> GetPlayerDataAsync(string playerId)
    {
        RedisValue jsonData = await _db.StringGetAsync($"player:{playerId}");
        if (jsonData.IsNullOrEmpty)
        {
            return null; // 데이터가 없을 경우 null 반환
        }
        return JsonSerializer.Deserialize<PlayerData>(jsonData);
    }
}