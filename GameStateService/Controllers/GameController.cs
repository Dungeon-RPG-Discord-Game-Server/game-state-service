using Microsoft.AspNetCore.Mvc;

using GameStateService.Services;
using GameStateService.Dtos;

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

            // FakePlayerStorage.Save(request.UserId, request.Name);
            await _memoryCacheService.RegisterPlayerDataAsync(request.UserId, TimeSpan.FromMinutes(30));

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
