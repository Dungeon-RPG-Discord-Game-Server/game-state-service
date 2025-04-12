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

        public async Task RegisterPlayerDataAsync(string playerId, WeaponType weaponType, TimeSpan? expiration = null)
        {
            try
            {
                var options = new MemoryCacheEntryOptions();
                if (expiration.HasValue)
                {
                    options.SetAbsoluteExpiration(expiration.Value);
                }

                PlayerData data = PlayerFactory.CreateNewPlayer(playerId, playerId, weaponType);
                string strData = JsonSerializerWrapper.Serialize(data);
                _memoryCache.Set(playerId, strData, options);
            }
            catch (Exception ex)
            {
                throw new Exception($"[RegisterPlayerDataAsync] Failed to register data for player {playerId}: {ex.Message}");
            }
        }

        public async Task UpdatePlayerDataAsync(string playerId, PlayerData data, TimeSpan? expiration = null)
        {
            try
            {
                var options = new MemoryCacheEntryOptions();
                if (expiration.HasValue)
                {
                    options.SetAbsoluteExpiration(expiration.Value);
                }

                string strData = JsonSerializerWrapper.Serialize(data);
                _memoryCache.Set(playerId, strData, options);
            }
            catch (Exception ex)
            {
                throw new Exception($"[UpdatePlayerDataAsync] Failed to update data for player {playerId}: {ex.Message}");
            }
        }

        public async Task<bool> IsPlayerDataExistAsync(string playerId)
        {
            try
            {
                bool isKeyExist = _memoryCache.TryGetValue(playerId, out string data);
                return isKeyExist;
            }
            catch (Exception ex)
            {
                throw new Exception($"[IsPlayerDataExistAsync] Failed to check existence of data for player {playerId}: {ex.Message}");
            }
        }

        public async Task<PlayerData> GetPlayerDataAsync(string playerId)
        {
            try
            {
                bool isKeyExist = _memoryCache.TryGetValue(playerId, out string data);
                if (!isKeyExist || string.IsNullOrWhiteSpace(data))
                {
                    throw new Exception($"[GetPlayerDataAsync] No data found for player {playerId}");
                }

                return JsonSerializerWrapper.Deserialize<PlayerData>(data);
            }
            catch (Exception ex)
            {
                throw new Exception($"[GetPlayerDataAsync] Failed to retrieve data for player {playerId}: {ex.Message}");
            }
        }

        public async Task RemovePlayerDataAsync(string playerId)
        {
            try
            {
                _memoryCache.Remove(playerId);
            }
            catch (Exception ex)
            {
                throw new Exception($"[RemovePlayerDataAsync] Failed to remove data for player {playerId}: {ex.Message}");
            }
        }
    }
}
