using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace GameService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoadController : ControllerBase
    {
        // [HttpGet("{userId}/map")]
        // public Task<IActionResult> GetChoiceOptions([FromQuery] string userId)
        // {
        //     // 실제 구현에서는 사용자 ID에 따른 선택지를 가져오는 로직이 필요합니다.
        //     var options = new List<string> { "1. 공격", "2. 방어", "3. 도망" };
        //     return Ok(new { userId, options });
        // }

    }

    public class LoadResponse
    {
        public string UserId { get; set; }
        public int SelectedOption { get; set; }
    }
}
