# 기본 ASP.NET 런타임
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000

# SDK 이미지로 빌드
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src

# .csproj 복사 및 복원
COPY ["GameStateService/GameStateService.csproj", "GameStateService/"]
RUN dotnet restore "GameStateService/GameStateService.csproj"

# 전체 소스 복사 후 빌드
COPY . .
WORKDIR /src/GameStateService
RUN dotnet build "GameStateService.csproj" -c $configuration -o /app/build

# 퍼블리시 단계
FROM build AS publish
ARG configuration=Release
RUN dotnet publish "GameStateService.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

# 최종 런타임 컨테이너
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GameStateService.dll"]
