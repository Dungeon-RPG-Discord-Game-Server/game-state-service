using StackExchange.Redis;
using System;
using System.Threading.Tasks;

public class RedisManager{
    private readonly ConnectionMultiplexer _redis;
    private readonly IDatabase _db;

    public RedisManager(string connectionString)
    {
        _redis = ConnectionMultiplexer.Connect(connectionString);
        _db = _redis.GetDatabase();
    }

    // 캐시에 값 설정 (예: 플레이어 상태)
    public async Task SetPlayerStateAsync(string playerId, string stateJson)
    {
        // 예: 플레이어 상태를 키 "player:{playerId}"로 저장, 30분 만료
        await _db.StringSetAsync($"player:{playerId}", stateJson, TimeSpan.FromMinutes(30));
    }

    // 캐시에서 값 가져오기
    public async Task<string> GetPlayerStateAsync(string playerId)
    {
        return await _db.StringGetAsync($"player:{playerId}");
    }

    // Pub/Sub 예시: 메시지 발행
    public async Task PublishEventAsync(string channel, string message)
    {
        await _redis.GetSubscriber().PublishAsync(channel, message);
    }

    // Pub/Sub 예시: 구독
    public void SubscribeToEvent(string channel, Action<RedisChannel, RedisValue> handler)
    {
        _redis.GetSubscriber().Subscribe(channel, handler);
    }
}