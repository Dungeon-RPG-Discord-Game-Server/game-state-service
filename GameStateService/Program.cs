using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using GameStateService.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. 서비스 구성 (DI 컨테이너에 서비스 등록)
builder.Services.AddMemoryCache();

builder.Services.AddSingleton<APIRequestWrapper>();
builder.Services.AddSingleton<HttpClient>();
builder.Services.AddSingleton<MemoryCacheService>();
builder.Services.AddSingleton<GameFlowManager>();
builder.Services.AddSingleton<GameMoveHandler>(); // GameStateService 등록
builder.Services.AddSingleton<GameBattleHandler>(); // GameStateService 등록

builder.Services.AddControllers();  // MVC 컨트롤러 등록
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();   // Swagger 설정 (선택)

// 예: 추가적인 서비스 등록
// builder.Services.AddSingleton<IMyService, MyService>();

var app = builder.Build();

// 2. HTTP 요청 파이프라인 구성
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

// 컨트롤러 엔드포인트 매핑
app.MapControllers();

app.Run();

