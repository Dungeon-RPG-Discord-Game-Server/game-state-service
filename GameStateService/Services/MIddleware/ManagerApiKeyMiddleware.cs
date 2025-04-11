using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

public class ManagerApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _adminKey;

    public ManagerApiKeyMiddleware(RequestDelegate next, IConfiguration config)
    {
        _next = next;

        var SecretClient = new SecretClient(new Uri(config["KeyVaultUri"]), new DefaultAzureCredential());
        _adminKey = SecretClient.GetSecret("admin-api-key").Value.Value;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/api/apikey"))
        {
            if (!context.Request.Headers.TryGetValue("X-MANAGER-KEY", out var key) || key != _adminKey)
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("❌ Unauthorized to access API Key Generator.");
                return;
            }
        }
        await _next(context);
    }
}
