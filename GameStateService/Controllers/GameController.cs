using Microsoft.AspNetCore.Mvc;

using GameStateService.Services;
using GameStateService.Dtos;
using GameStateService.Models;
using GameStateService.Utils;

namespace GameService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly MemoryCacheService _memoryCacheService;
        private readonly GameFlowManager _gameFlowManager;
        public GameController(MemoryCacheService memoryCacheService, GameFlowManager gameFlowManager)
        {
            _memoryCacheService = memoryCacheService;
            _gameFlowManager = gameFlowManager;
        }

        [HttpGet("{userId}/status")]
        public async Task<IActionResult> GetUserOnline(string userId)
        {
            bool isOnline = await _memoryCacheService.GetPlayerDataAsync(userId) != null;

            var result = new PlayerStatusDto
            {
                PlayerId = userId,
                Online = isOnline
            };
            return Ok(result);
        }

        [HttpGet("{userId}/summary")]
        public async Task<IActionResult> GetUserSummary(string userId)
        {
            var data = await _memoryCacheService.GetPlayerDataAsync(userId);

            if (data == null)
            {
                return NotFound(new { Message = "❌ User not found." });
            }

            string playerSummary = data.ToString();

            return Content(playerSummary, "text/plain");
        }

        [HttpGet("{userId}/map")]
        public async Task<IActionResult> GetUserMapData(string userId)
        {
            var data = await _memoryCacheService.GetPlayerDataAsync(userId);

            if (data == null)
            {
                return NotFound(new { Message = "❌ User not found." });
            }

            string mapData = await _gameFlowManager.GetMapStringAsync(data.CurrentMapData);

            return Content(mapData, "text/plain");
        }
        
        [HttpPost("register")]
        public async Task<IActionResult> PostRegisterPlayer([FromBody] RegisterPlayerRequestDto request)
        {
            await _gameFlowManager.StartNewGameAsync(request.UserId, request.WeaponType);

            return Ok(new RegisterPlayerResponseDto
            {
                PlayerId = request.UserId,
                Registered = true,
                Message = "✅ User successfully registered."
            });
        }

        [HttpGet("{userId}/start")]
        public async Task<IActionResult> PostStartGame(string userId)
        {
            bool isOnline = await _memoryCacheService.GetPlayerDataAsync(userId) != null;

            var result = new PlayerStatusDto
            {
                PlayerId = userId,
                Online = isOnline
            };
            return Ok(result);
        }
    }

    public class ChoiceResponse
    {
        public string UserId { get; set; }
        public int SelectedOption { get; set; }
    }
}
