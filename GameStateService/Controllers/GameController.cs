using Microsoft.AspNetCore.Mvc;

using GameStateService.Services;
using GameStateService.Dtos;
using GameStateService.Models;

namespace GameService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly MemoryCacheService _memoryCacheService;
        public GameController(MemoryCacheService memoryCacheService)
        {
            _memoryCacheService = memoryCacheService;
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
        
        [HttpPost("register")]
        public async Task<IActionResult> PostRegisterPlayer([FromBody] RegisterPlayerRequestDto request)
        {
            bool alreadyRegistered = await _memoryCacheService.GetPlayerDataAsync(request.UserId) != null;
            
            if (alreadyRegistered)
            {
                return Conflict(new RegisterPlayerResponseDto
                {
                    PlayerId = request.UserId,
                    Registered = false,
                    Message = "❌ User already registered."
                });
            }

            await _memoryCacheService.RegisterPlayerDataAsync(request.UserId, (WeaponType)request.WeaponType,TimeSpan.FromMinutes(30));

            return Ok(new RegisterPlayerResponseDto
            {
                PlayerId = request.UserId,
                Registered = true,
                Message = "✅ User successfully registered."
            });
        }

        [HttpPost("{userId}/select-weapon")]
        public async Task<IActionResult> PostPlayerSelectWeapon([FromBody] PlayerSelectWeaponDto request)
        {
            var playerData = await _memoryCacheService.GetPlayerDataAsync(request.UserId);
            if (playerData == null)
            {
                return NotFound(new { Message = "❌ User not found." });
            }

            playerData.Weapon = request.Weapon switch
            {
                1 => WeaponFactory.CreateWeapon("Sword", 10, 0, WeaponType.Sword),
                2 => WeaponFactory.CreateWeapon("MagicWand", 20, 5, WeaponType.MagicWand),
                _ => null
            };

            await _memoryCacheService.UpdatePlayerDataAsync(request.UserId, playerData);

            return Ok(new { Message = "✅ Weapon selected successfully." });
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
