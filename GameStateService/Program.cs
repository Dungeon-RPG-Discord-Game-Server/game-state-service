using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

using Azure.Identity;

using GameStateService.Services;

using Telemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

var keyVaultUri = new Uri("https://vbn930-rpg-kv.vault.azure.net/");

builder.Configuration.AddAzureKeyVault(
    keyVaultUri,
    new DefaultAzureCredential());

IConfiguration configuration = builder.Configuration;

string serviceName = configuration["Logging:ServiceName"];
string serviceVersion = configuration["Logging:ServiceVersion"];

builder.Services.AddMemoryCache();
builder.Services.AddOpenTelemetry().WithTracing(tcb =>
{
    tcb
    .AddSource(serviceName)
    .SetResourceBuilder(
        ResourceBuilder.CreateDefault()
            .AddService(serviceName: serviceName, serviceVersion: serviceVersion))
    .AddAspNetCoreInstrumentation()
    .AddJsonConsoleExporter();
});

builder.Services.AddSingleton<HttpClient>();
builder.Services.AddSingleton<MemoryCacheService>();
builder.Services.AddSingleton<GameFlowManager>();
builder.Services.AddSingleton<GameMoveHandler>();
builder.Services.AddSingleton<GameBattleHandler>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ManagerApiKeyMiddleware>();
app.UseMiddleware<ApiKeyMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.Run();

