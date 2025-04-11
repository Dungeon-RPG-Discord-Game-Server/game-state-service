using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

using Microsoft.Extensions.Caching.Memory;

namespace GameStateService.Controllers
{
    public class ApiKeyMetadata
    {
        public string Key { get; set; }
        public string Owner { get; set; }
        public DateTime Expiration { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class ApiKeyController : ControllerBase
    {
        private readonly SecretClient _secretClient;
        private readonly IMemoryCache _cache;

        public ApiKeyController(IConfiguration config, IMemoryCache cache)
        {
            var keyVaultUrl = new Uri(config["KeyVaultUri"] ?? throw new InvalidOperationException("KeyVaultUri not found"));
            _secretClient = new SecretClient(keyVaultUrl, new DefaultAzureCredential());
            _cache = cache;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateApiKey([FromQuery] string owner = "anonymous")
        {
            // 키 생성
            var key = Guid.NewGuid().ToString("N");
            var expiration = DateTime.UtcNow.AddHours(12);

            var metadata = new ApiKeyMetadata
            {
                Key = key,
                Owner = owner,
                Expiration = expiration
            };

            // JSON 문자열로 시크릿 저장
            var json = JsonSerializer.Serialize(metadata);

            // 🔐 Key Vault에 저장
            var response = await _secretClient.SetSecretAsync($"apikey-{owner}", json);
            _cache.Set($"apikey-{owner}", key, TimeSpan.FromHours(12));
            return Ok();
        }
    }
}
