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
        private readonly Logger _logger;
        private readonly IConfiguration _configuration;
        public GameController(MemoryCacheService memoryCacheService, GameFlowManager gameFlowManager, GameMoveHandler gameMoveHandler, IConfiguration configuration)
        {
            _configuration = configuration;
            if (null == _configuration)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            string serviceName = configuration["Logging:ServiceName"];
            _logger = new Logger(serviceName);
            _memoryCacheService = memoryCacheService;
            _gameFlowManager = gameFlowManager;
            _gameMoveHandler = gameMoveHandler;
        }

        [HttpGet("{userId}/status")]
        public async Task<IActionResult> GetUserOnline(string userId)
        {
            using(var log = _logger.StartMethod(nameof(GetUserOnline), HttpContext)){
                try
                {
                    log.SetAttribute("request.userId", userId);
                    bool isOnline = await _memoryCacheService.GetPlayerDataAsync(userId) != null;

                    var result = new PlayerStatusDto
                    {
                        PlayerId = userId,
                        Online = isOnline
                    };
                    return Ok(result);
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

        [HttpGet("{userId}/summary")]
        public async Task<IActionResult> GetUserSummary(string userId)
        {
            using(var log = _logger.StartMethod(nameof(GetUserSummary), HttpContext)){
                try
                {
                    log.SetAttribute("request.userId", userId);
                    var data = await _memoryCacheService.GetPlayerDataAsync(userId);

                    if (data == null)
                    {
                        return NotFound(new { Message = "User not found." });
                    }

                    string weaponInfo = data.Weapon != null
                            ? $"🗡️ Weapon: **{data.Weapon.Name} (ATK: {data.Weapon.AttackPower}, Mana Cost: {data.Weapon.ManaCost})**"
                            : "❌ No weapon equipped";

                    string mapInfo = data.CurrentMapData != null
                            ? $"🗺️ Current Map: **{data.CurrentMapData.MapName}**"
                            : "🌐 No map assigned";
                    string playerSummary =
                            $@"👤 **{data.UserName}**
                            🏅 Level: {data.Level}
                            ✨ EXP: {data.Experience}
                            ❤️ Health: {data.Health} / {data.MaxHealth}
                            🔵 Mana: {data.Mana} / {data.MaxMana}
                            {weaponInfo}
                            {mapInfo}".Trim();

                    return Content(playerSummary, "text/plain");
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

        [HttpGet("{userId}/map")]
        public async Task<IActionResult> GetUserMapData(string userId)
        {
            using(var log = _logger.StartMethod(nameof(GetUserMapData), HttpContext)){
                try
                {
                    log.SetAttribute("request.userId", userId);
                    var data = await _memoryCacheService.GetPlayerDataAsync(userId);

                    if (data == null)
                    {
                        return NotFound(new { Message = "User not found." });
                    }

                    string mapData = await _gameFlowManager.GetMapStringAsync(data.CurrentMapData);

                    return Content(mapData, "text/plain");
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

        [HttpGet("{userId}/map/enter")]
        public async Task<IActionResult> GetEnterMap(string userId)
        {
            using(var log = _logger.StartMethod(nameof(GetEnterMap), HttpContext)){
                try
                {
                    log.SetAttribute("request.userId", userId);
                    var data = await _memoryCacheService.GetPlayerDataAsync(userId);

                    if (data == null)
                    {
                        return NotFound(new { Message = "User not found." });
                    }

                    await _gameFlowManager.EnterNewDungeonAsync(userId);

                    var updated = await _memoryCacheService.GetPlayerDataAsync(userId);

                    return Content($"✅ You have entered the dungeon: **{updated.CurrentMapData.MapName}**.", "text/plain");
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

        [HttpGet("{userId}/map/neighbors")]
        public async Task<IActionResult> GetUserMapNeighborsDirections(string userId)
        {
            using(var log = _logger.StartMethod(nameof(GetUserMapNeighborsDirections), HttpContext)){
                try
                {
                    log.SetAttribute("request.userId", userId);
                    var data = await _memoryCacheService.GetPlayerDataAsync(userId);

                    if (data == null)
                    {
                        return NotFound(new { Message = "User not found." });
                    }

                    var mapData = data.CurrentMapData;
                    var currentRoom = mapData.Rooms[mapData.CurrentRoom];

                    var directions = await _gameMoveHandler.GetNeighborRoomDirections(currentRoom, mapData.Rooms);

                    return Ok(directions);
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

        [HttpPost("{userId}/move")]
        public async Task<IActionResult> PostUserMoveRoom(string userId, [FromBody] MovePlayerRequestDto request)
        {
            using(var log = _logger.StartMethod(nameof(PostUserMoveRoom), HttpContext)){
                try
                {
                    log.SetAttribute("request.userId", userId);
                    log.SetAttribute("request.direction", request.Direction);
                    var data = await _memoryCacheService.GetPlayerDataAsync(userId);

                    if (data.CurrentGameState != GameStateType.ExplorationState){
                        return BadRequest(new { Message = "You are not in the exploration state." });
                    }

                    if (data == null)
                    {
                        return NotFound(new { Message = "User not found." });
                    }

                    int? nextRoomId = await _gameMoveHandler.MovePlayerAsync(userId, request.Direction);
                    if (nextRoomId == null)
                    {
                        return BadRequest(new { Message = "Invalid move direction." });
                    }

                    data = await _memoryCacheService.GetPlayerDataAsync(userId);
                    var currentRoom = data.CurrentMapData.Rooms[data.CurrentMapData.CurrentRoom];
                    bool isMonsterPresent = currentRoom.Monster != null;
                    if (isMonsterPresent)
                    {
                        data.CurrentGameState = GameStateType.BattleState;
                        await _memoryCacheService.UpdatePlayerDataAsync(userId, data, TimeSpan.FromMinutes(30));
                        return Ok(new { Message = "You have encountered a monster! Prepare for battle!" });
                    }

                    return Ok(new { Message = $"You moved to room {nextRoomId}." });
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

        [HttpGet("{userId}/state")]
        public async Task<IActionResult> GetUserGameState(string userId)
        {
            using(var log = _logger.StartMethod(nameof(GetUserGameState), HttpContext)){
                try
                {
                    log.SetAttribute("request.userId", userId);

                    var data = await _memoryCacheService.GetPlayerDataAsync(userId);

                    if (data == null)
                    {
                        return NotFound(new { Message = "User not found." });
                    }

                    var gameState = await _gameFlowManager.GetCurrentGameStateAsync(userId);

                    return Content(gameState.ToString(), "text/plain");
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
        
        [HttpPost("register")]
        public async Task<IActionResult> PostRegisterPlayer([FromBody] RegisterPlayerRequestDto request)
        {
            using(var log = _logger.StartMethod(nameof(PostRegisterPlayer), HttpContext)){
                try
                {
                    log.SetAttribute("request.userId", request.UserId);
                    log.SetAttribute("request.name", request.Name);
                    log.SetAttribute("request.weaponType", request.WeaponType);

                    await _gameFlowManager.StartNewGameAsync(request.UserId, request.Name, request.WeaponType);

                    return Ok(new RegisterPlayerResponseDto
                    {
                        PlayerId = request.UserId,
                        Registered = true,
                        Message = "✅ You have been successfully registered for the adventure!\nWelcome, brave traveler. Your journey begins now..."
                    });
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

        [HttpGet("{userId}/start")]
        public async Task<IActionResult> GetStartGame(string userId)
        {
            using(var log = _logger.StartMethod(nameof(GetStartGame), HttpContext)){
                try
                {
                    log.SetAttribute("request.userId", userId);
                    bool isOnline = await _memoryCacheService.GetPlayerDataAsync(userId) != null;

                    var result = new PlayerStatusDto
                    {
                        PlayerId = userId,
                        Online = isOnline
                    };
                    return Ok(result);
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

        [HttpPost("{userId}/quit")]
        public async Task<IActionResult> PostQuitGame(string userId)
        {
            using(var log = _logger.StartMethod(nameof(PostQuitGame), HttpContext)){
                try
                {
                    log.SetAttribute("request.userId", userId);
                    await _memoryCacheService.RemovePlayerDataAsync(userId);
                    return Ok(new { Message = "You have successfully quit the game." });
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