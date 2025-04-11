using GameStateService.Azure;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

using GameStateService.Models;
using GameStateService.Services;
using GameStateService.Utils;
using GameStateService.Azure;

namespace GameService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SaveLoadController : ControllerBase
    {
        private readonly MemoryCacheService _memoryCacheService;
        private readonly CosmosDbWrapper _cosmosDbWrapper;
        private readonly IConfiguration _configuration;
        private readonly Logger _logger;

        public SaveLoadController(IConfiguration configuration, MemoryCacheService memoryCacheService)
        {
            _memoryCacheService = memoryCacheService;
            _configuration = configuration;
            if (null == _configuration)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            string serviceName = configuration["Logging:ServiceName"];
            _logger = new Logger(serviceName);
            _cosmosDbWrapper = new CosmosDbWrapper(configuration);
        }

        [HttpPost("{userId}/save")]
        public async Task<IActionResult> SaveGame(string userId)
        {
            using (var log = _logger.StartMethod(nameof(SaveGame), HttpContext))
            {
                try
                {
                    log.SetAttribute("request.userId", userId);

                    var saveData = await _memoryCacheService.GetPlayerDataAsync(userId);
                    Console.WriteLine($"SaveData: {saveData}");
                    if (await _cosmosDbWrapper.GetItemAsync<PlayerData>(saveData.PlayerId, saveData.PlayerId) != null)
                    {
                        await _cosmosDbWrapper.UpdateItemAsync(saveData.id, saveData.PlayerId, saveData);
                    }
                    else
                    {
                        await _cosmosDbWrapper.AddItemAsync(saveData, saveData.PlayerId);
                    }
                    return Ok(new { Message = "Game state saved successfully." });
                }
                catch (UserErrorException e)
                {
                    log.LogUserError(e.Message);
                    return BadRequest(new { Message = e.Message });
                }
                catch(Exception e)
                {
                    log.HandleException(e);
                    return BadRequest(new { Message = e.Message });
                }
            }
        }

        [HttpGet("{userId}/load")]
        public async Task<IActionResult> LoadGame(string userId)
        {
            using (var log = _logger.StartMethod(nameof(LoadGame), HttpContext))
            {
                try
                {
                    log.SetAttribute("request.userId", userId);

                    var playerData = await _cosmosDbWrapper.GetItemAsync<PlayerData>(userId, userId);

                    await _memoryCacheService.UpdatePlayerDataAsync(userId, playerData, TimeSpan.FromMinutes(30));
                    return Ok(new { Message = "Game state loaded successfully." });
                }
                catch (UserErrorException e)
                {
                    log.LogUserError(e.Message);
                    return BadRequest(new { Message = e.Message });
                }
                catch(Exception e)
                {
                    log.HandleException(e);
                    return BadRequest(new { Message = e.Message });
                }
            }
        }
    }
}
