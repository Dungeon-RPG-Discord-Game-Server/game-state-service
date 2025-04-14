# GameStateService

GameStateService is a stateless, scalable **HTTP-based game server** that manages player state, battle flow, and map progression for a Discord-based turn-based RPG.  
Built with **ASP.NET Core Web API**, this service is deployed via **Azure Container Apps** and optimized with in-memory caching and persistent storage using **Azure Cosmos DB**.

## Technologies Used

- **.NET 8** – Web API backend
- **HTTP Communication** – Stateless RESTful APIs for game commands
- **Azure Container Apps** – Containerized deployment with scale-out support
- **Azure Cosmos DB** – Persistent player and game state storage
- **Azure Key Vault** – Secure API Key storage
- **In-Memory Cache** – Fast API Key + Player State caching (replaces Redis)
- **OpenTelemetry** – Distributed tracing with context propagation
- **GitHub Actions** – CI/CD for Docker image build and Azure push

## Features

- Game logic for player registration, movement, battles, and quit flow
- Stateless REST APIs accessed by the Discord bot via HTTP
- Lightweight API Key-based auth middleware
- Fast in-memory caching for low-latency state reads/writes
- Auto-scaling and Always-On support via Azure

## API Key Security Architecture

- Permanent Admin API Key fetched from Azure Key Vault
- Temporary API Keys issued to Discord service and cached
- Every request validated by middleware using in-memory cache
- If missing, fallback to Key Vault → cached again
- Invalid or expired keys return `403 Forbidden`

> All communication is done over HTTP, allowing clean decoupling and scalability between Discord interaction layer and backend game state service.

## Deployment

1. Containerized with Docker
2. CI via GitHub Actions pushes image to Azure Container Registry
3. Hosted as an Azure Container App with `minReplicas: 1`

## Performance Highlights

- API Key validation reduced from **300ms → 1-2ms** using cache
- Designed to be stateless and horizontally scalable
- Cosmos DB only accessed on cache miss or game save
