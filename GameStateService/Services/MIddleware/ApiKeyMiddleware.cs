using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

using System.Text.Json;

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

    public ApiKeyMiddleware(RequestDelegate next, IConfiguration config)
    {
        _next = next;
        _configuration = config;
    }

    public async Task Invoke(HttpContext context)
    {
        if (!context.Request.Path.StartsWithSegments("/api/apikey")){
            if (!context.Request.Headers.TryGetValue("X-API-KEY", out var providedKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("❌ API Key is missing.");
                return;
            }

            var SecretClient = new SecretClient(new Uri(_configuration["KeyVaultUri"]), new DefaultAzureCredential());
            var keyMetaData = JsonSerializer.Deserialize<ApiKeyMetadata>(SecretClient.GetSecret("apikey-discord-bot").Value.Value);
            var expectedKey = keyMetaData.Key;
            if (providedKey != expectedKey)
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("❌ Invalid API Key.");
                return;
            }
        }
        await _next(context);
    }
}