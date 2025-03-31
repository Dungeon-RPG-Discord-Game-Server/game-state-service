using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace GameService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChoiceController : ControllerBase
    {
        // GET: api/choice-options?userId=...
        [HttpGet("choice-options")]
        public IActionResult GetChoiceOptions([FromQuery] string userId)
        {
            // 실제 구현에서는 사용자 ID에 따른 선택지를 가져오는 로직이 필요합니다.
            var options = new List<string> { "1. 공격", "2. 방어", "3. 도망" };
            return Ok(new { userId, options });
        }

        // POST: api/choice-response
        [HttpPost("choice-response")]
        public IActionResult PostChoiceResponse([FromBody] ChoiceResponse response)
        {
            // response 예: { "userId": "123456789", "selectedOption": 1 }
            // 사용자의 선택을 게임 로직에 반영하는 로직을 구현합니다.
            // 예: 전투 시작, 보상 지급 등
            // 처리 결과를 반환합니다.
            return Ok(new { message = "선택이 처리되었습니다." });
        }
    }

    public class ChoiceResponse
    {
        public string UserId { get; set; }
        public int SelectedOption { get; set; }
    }
}
