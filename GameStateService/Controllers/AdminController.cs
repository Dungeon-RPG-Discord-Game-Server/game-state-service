using Microsoft.AspNetCore.Mvc;

using GameStateService.Services;

namespace GameService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly MemoryCacheService _memoryCacheService;

        public AdminController(MemoryCacheService memoryCacheService)
        {
            _memoryCacheService = memoryCacheService;
        }

        [HttpGet("admin/players")]
        public async Task<IActionResult> GetPlayers()
        {
            // bool isOnline = await _memoryCacheService.GetPlayerDataAsync(userId) != null;

            // var result = new PlayerStatusDto
            // {
            //     PlayerId = userId,
            //     Online = isOnline
            // };
            // return Ok(result);
            return Ok();
        }
    }
}