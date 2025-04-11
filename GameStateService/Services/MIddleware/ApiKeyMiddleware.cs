using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

using System.Text.Json;

using Microsoft.Extensions.Caching.Memory;

public class ApiKeyMiddleware
{
    public class ApiKeyMetadata
    {
        public string Key { get; set; }
        public string Owner { get; set; }
        public DateTime Expiration { get; set; }
    }
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private readonly IMemoryCache _cache;
    private readonly SecretClient _keyVaultClient;

    public ApiKeyMiddleware(RequestDelegate next, IConfiguration config, IMemoryCache cache)
    {
        _next = next;
        _configuration = config;
        _cache = cache;
        var keyVaultUrl = new Uri(config["KeyVaultUri"] ?? throw new InvalidOperationException("KeyVaultUri not found"));
        _keyVaultClient = new SecretClient(keyVaultUrl, new DefaultAzureCredential());
    }

    public async Task Invoke(HttpContext context)
    {
        if (!context.Request.Path.StartsWithSegments("/api/apikey")){
            if (!context.Request.Headers.TryGetValue("X-API-KEY", out var providedKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("API Key is missing.");
                return;
            }
            
            var expectedKey = await GetApiKeyAsync("apikey-discord-bot");
            if (providedKey != expectedKey)
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Invalid API Key.");
                return;
            }
        }
        await _next(context);
    }

    public async Task<string> GetApiKeyAsync(string keyName)
    {
        if (_cache.TryGetValue(keyName, out string cachedValue))
            return cachedValue;

        var secret = await _keyVaultClient.GetSecretAsync(keyName);
        var value = secret.Value.Value;

        // 캐시에 저장 (예: 10분 유효)
        _cache.Set(keyName, value, TimeSpan.FromMinutes(10));
        return value;
    }
}