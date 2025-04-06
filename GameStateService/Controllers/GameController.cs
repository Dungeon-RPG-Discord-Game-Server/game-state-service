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
        private readonly GameMoveHandler _gameMoveHandler;
        public GameController(MemoryCacheService memoryCacheService, GameFlowManager gameFlowManager, GameMoveHandler gameMoveHandler)
        {
            _memoryCacheService = memoryCacheService;
            _gameFlowManager = gameFlowManager;
            _gameMoveHandler = gameMoveHandler;
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

        [HttpGet("{userId}/map/enter")]
        public async Task<IActionResult> GetEnterMap(string userId)
        {
            var data = await _memoryCacheService.GetPlayerDataAsync(userId);

            if (data == null)
            {
                return NotFound(new { Message = "❌ User not found." });
            }

            await _gameFlowManager.EnterNewDungeonAsync(userId);

            var updated = await _memoryCacheService.GetPlayerDataAsync(userId);

            return Content($"✅ You have entered the map: {updated.CurrentMapData.MapName}.", "text/plain");
        }

        [HttpGet("{userId}/map/neighbors")]
        public async Task<IActionResult> GetUserMapNeighborsDirections(string userId)
        {
            var data = await _memoryCacheService.GetPlayerDataAsync(userId);

            if (data == null)
            {
                return NotFound(new { Message = "❌ User not found." });
            }

            var mapData = data.CurrentMapData;
            var currentRoom = mapData.Rooms[mapData.CurrentRoom];

            var directions = await _gameMoveHandler.GetNeighborRoomDirections(currentRoom, mapData.Rooms);

            return Ok(directions);
        }

        [HttpPost("{userId}/move")]
        public async Task<IActionResult> PostUserMoveRoom(string userId, [FromBody] MovePlayerRequestDto request)
        {
            var data = await _memoryCacheService.GetPlayerDataAsync(userId);

            if (data.CurrentGameState != GameStateType.ExplorationState){
                return BadRequest(new { Message = "❌ You are not in the exploration state." });
            }

            if (data == null)
            {
                return NotFound(new { Message = "❌ User not found." });
            }

            int? nextRoomId = await _gameMoveHandler.MovePlayerAsync(userId, request.Direction);
            if (nextRoomId == null)
            {
                return BadRequest(new { Message = "❌ Invalid move direction." });
            }

            data = await _memoryCacheService.GetPlayerDataAsync(userId);
            var currentRoom = data.CurrentMapData.Rooms[data.CurrentMapData.CurrentRoom];
            bool isMonsterPresent = currentRoom.Monster != null;
            if (isMonsterPresent)
            {
                data.CurrentGameState = GameStateType.BattleState;
                await _memoryCacheService.UpdatePlayerDataAsync(userId, data, TimeSpan.FromMinutes(30));
                return Ok(new { Message = "⚔️ You have encountered a monster! Prepare for battle!" });
            }

            return Ok(new { Message = $"✅ You moved to room {nextRoomId}." });
        }

        [HttpGet("{userId}/state")]
        public async Task<IActionResult> GetUserGameState(string userId)
        {
            var data = await _memoryCacheService.GetPlayerDataAsync(userId);

            if (data == null)
            {
                return NotFound(new { Message = "❌ User not found." });
            }

            var gameState = await _gameFlowManager.GetCurrentGameStateAsync(userId);

            return Content(gameState.ToString(), "text/plain");
        }
        
        [HttpPost("register")]
        public async Task<IActionResult> PostRegisterPlayer([FromBody] RegisterPlayerRequestDto request)
        {
            await _gameFlowManager.StartNewGameAsync(request.UserId, request.WeaponType);

            return Ok(new RegisterPlayerResponseDto
            {
                PlayerId = request.UserId,
                Registered = true,
                Message = "✅ You have been successfully registered for the adventure!\nWelcome, brave traveler. Your journey begins now..."
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
}