using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using System.Text.Json.Serialization;

using GameStateService.Models;

namespace GameStateService.Services
{
    public class MemoryCacheService
    {
        private readonly IMemoryCache _memoryCache;

        public MemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public async Task RegisterPlayerDataAsync(string playerId, TimeSpan? expiration = null)
        {
            var options = new MemoryCacheEntryOptions();
            if (expiration.HasValue)
            {
                options.SetAbsoluteExpiration(expiration.Value);
            }
            PlayerData data = PlayerFactory.CreateNewPlayer(playerId, playerId);
            string strData = JsonSerializer.Serialize(data);
            _memoryCache.Set(playerId, strData, options);

            Console.WriteLine(data.ToString());
        }

        public async Task SetPlayerDataAsync(string playerId, PlayerData data, TimeSpan? expiration = null)
        {
            var options = new MemoryCacheEntryOptions();
            if (expiration.HasValue)
            {
                options.SetAbsoluteExpiration(expiration.Value);
            }
            string strData = JsonSerializer.Serialize(data);
            _memoryCache.Set(playerId, strData, options);
        }

        public async Task<PlayerData> GetPlayerDataAsync(string playerId)
        {
            bool isKeyExist = _memoryCache.TryGetValue(playerId, out string data);
            if (!isKeyExist)
            {
                return null; // 데이터가 없을 경우 null 반환
            }
            return JsonSerializer.Deserialize<PlayerData>(data);
        }

        public async Task RemovePlayerDataAsync(string playerId)
        {
            _memoryCache.Remove(playerId);
        }
    }
}
