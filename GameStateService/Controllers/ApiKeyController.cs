using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

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

        public ApiKeyController(IConfiguration config)
        {
            var keyVaultUrl = new Uri(config["KeyVaultUri"] ?? throw new InvalidOperationException("KeyVaultUri not found"));
            _secretClient = new SecretClient(keyVaultUrl, new DefaultAzureCredential());
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateApiKey([FromQuery] string owner = "anonymous")
        {
            Console.WriteLine("🔥 GenerateApiKey 진입");
            // 키 생성
            var key = Guid.NewGuid().ToString("N");
            var expiration = DateTime.UtcNow.AddHours(12);

            var metadata = new ApiKeyMetadata
            {
                Key = key,
                Owner = owner,
                Expiration = expiration
            };
            Console.WriteLine($"Generated API Key: {key}, Owner: {owner}, Expiration: {expiration}");

            // JSON 문자열로 시크릿 저장
            var json = JsonSerializer.Serialize(metadata);
            Console.WriteLine($"Generated API Key: {key}, Owner: {owner}, Expiration: {expiration}");

            // 🔐 Key Vault에 저장
            var response = await _secretClient.SetSecretAsync($"apikey-{owner}", json);
            Console.WriteLine($"Secret stored: {response.ToString()}");
            Console.WriteLine($"Saving to KeyVault: apikey-{owner}");
            // 발급된 키만 응답 (만료도 함께 전달)
            return Ok();
        }
    }
}
