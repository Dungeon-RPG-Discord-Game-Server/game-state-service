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

        [HttpGet("apikey/generate")]
        public async Task<IActionResult> GetPlayers()
        {
            return Ok();
        }
    }
}