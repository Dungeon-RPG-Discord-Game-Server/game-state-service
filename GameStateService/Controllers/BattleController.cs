using Microsoft.AspNetCore.Mvc;

using GameStateService.Services;
using GameStateService.Dtos;
using GameStateService.Models;
using GameStateService.Utils;

namespace GameService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BattleController : ControllerBase
    {
        private readonly MemoryCacheService _memoryCacheService;
        private readonly GameFlowManager _gameFlowManager;
        private readonly GameBattleHandler _gameBattleHandler;
        private readonly IConfiguration _configuration;
        private readonly Logger _logger;

        public BattleController(MemoryCacheService memoryCacheService, GameFlowManager gameFlowManager, GameBattleHandler gameBattleHandler, IConfiguration configuration)
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
            _gameBattleHandler = gameBattleHandler;
        }

        [HttpGet("{userId}/summary")]
        public async Task<IActionResult> GetBattleSummary(string userId)
        {
            using(var log = _logger.StartMethod(nameof(GetBattleSummary), HttpContext)){
                try
                {
                    log.SetAttribute("request.userId", userId);
                    string battleSummary = await _gameBattleHandler.GetBattleSummaryAsync(userId);
                    return Content(battleSummary, "text/plain");
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

        [HttpGet("{userId}/boss")]
        public async Task<IActionResult> GetBossCleared(string userId)
        {
            using(var log = _logger.StartMethod(nameof(GetBossCleared), HttpContext)){
                try
                {

                    log.SetAttribute("request.userId", userId);
                    bool bossCleared = await _gameBattleHandler.BossRoomClearedAsync(userId);
                    return Ok(bossCleared);
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

        [HttpPost("{userId}/attack")]
        public async Task<IActionResult> PostAttackMonster(string userId, [FromQuery] bool skillUsed = false)
        {
            using(var log = _logger.StartMethod(nameof(PostAttackMonster), HttpContext)){
                try
                {
                    log.SetAttribute("request.userId", userId);
                    log.SetAttribute("skillUsed", skillUsed);
                    string attackMessage = await _gameBattleHandler.AttackMonsterAsync(userId, skillUsed);
                    return Content(attackMessage, "text/plain");
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

        [HttpGet("{userId}/monster-attack")]
        public async Task<IActionResult> GetMonsterAttack(string userId)
        {
            using(var log = _logger.StartMethod(nameof(GetMonsterAttack), HttpContext)){
                try
                {
                    log.SetAttribute("request.userId", userId);
                    string monsterAttackMessage = await _gameBattleHandler.MonsterAttackAsync(userId);
                    return Content(monsterAttackMessage, "text/plain");
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

        [HttpGet("{userId}/run")]
        public async Task<IActionResult> GetRunaway(string userId)
        {
            using(var log = _logger.StartMethod(nameof(GetRunaway), HttpContext)){
                try
                {
                    log.SetAttribute("request.userId", userId);
                    var resultDto = await _gameBattleHandler.RunAwayAsync(userId);
                    return Ok(resultDto);
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

        [HttpGet("{userId}/moster")]
        public async Task<IActionResult> GetMonsterExist(string userId)
        {
            using(var log = _logger.StartMethod(nameof(GetMonsterExist), HttpContext)){
                try
                {
                    log.SetAttribute("request.userId", userId);
                    bool monsterExists = await _gameBattleHandler.MonsterExistsAsync(userId);
                    return Ok(monsterExists);
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