# AFC Asian Cup 2027 KMS - Local Development Setup

This document describes the local development configuration for testing the KMS without external dependencies (Docker, SQL Server, Redis).

## Table of Contents

1. [Overview](#overview)
2. [Quick Start](#quick-start)
3. [Configuration Files](#configuration-files)
4. [Backend Configuration](#backend-configuration)
5. [Frontend Configuration](#frontend-configuration)
6. [Mock Data](#mock-data)
7. [Switching to Production](#switching-to-production)
8. [Troubleshooting](#troubleshooting)

---

## Overview

The local development setup uses:
- **SQLite** instead of SQL Server (no Docker required)
- **In-Memory Cache** instead of Redis
- **Disabled Authentication** for testing without Azure AD
- **Seeded Data** with AFC 2027 themed content
- **Frontend Mock Service** as fallback when backend unavailable

---

## Quick Start

### Prerequisites
- .NET 8 SDK
- Node.js 18+
- npm 9+

### Running the Application

**Terminal 1 - Backend:**
```bash
cd "/home/ahmedimam/Asia Cup/Asia Cup POC/backend"
ASPNETCORE_ENVIRONMENT=Development dotnet run --project src/AFC27.KMS.WebApi
```

**Terminal 2 - Frontend:**
```bash
cd "/home/ahmedimam/Asia Cup/Asia Cup POC/frontend"
npm run dev
```

### Access Points
| Service | URL |
|---------|-----|
| Frontend | http://localhost:3000 |
| Backend API | http://localhost:5001 |
| Swagger UI | http://localhost:5001/swagger |
| Health Check | http://localhost:5001/health/live |

---

## Configuration Files

### Files Modified/Created for Local Development

| File | Purpose | Production Action |
|------|---------|-------------------|
| `backend/src/AFC27.KMS.WebApi/appsettings.Development.json` | SQLite + In-memory cache config | Use `appsettings.json` or `appsettings.Production.json` |
| `backend/src/Shared/AFC27.KMS.Infrastructure/Caching/MemoryCacheService.cs` | In-memory cache implementation | Use `RedisCacheService` in production |
| `backend/src/AFC27.KMS.WebApi/Data/KmsDbContext.cs` | EF Core DbContext with entities | Keep - used in all environments |
| `backend/src/AFC27.KMS.WebApi/Data/DatabaseSeeder.cs` | Seeds demo data | Disable `SeedDatabase` in production |
| `frontend/.env.development` | Frontend dev environment | Use `.env.production` |
| `frontend/src/services/mock.service.ts` | Mock data service | Not used when backend available |

---

## Backend Configuration

### appsettings.Development.json

Location: `backend/src/AFC27.KMS.WebApi/appsettings.Development.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=afc27_kms.db",  // SQLite database
    "Redis": ""                                        // Empty = disabled
  },
  "UseInMemoryCache": true,   // Use MemoryCacheService instead of Redis
  "SeedDatabase": true,       // Seed demo data on startup
  "DisableAuth": true,        // Skip authentication for testing
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Information",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  },
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "http://localhost:5173"
    ]
  }
}
```

### Configuration Options

| Setting | Development | Production |
|---------|-------------|------------|
| `ConnectionStrings:DefaultConnection` | `Data Source=afc27_kms.db` | SQL Server connection string |
| `ConnectionStrings:Redis` | `""` (empty) | `localhost:6379` or Redis cluster |
| `UseInMemoryCache` | `true` | `false` |
| `SeedDatabase` | `true` | `false` |
| `DisableAuth` | `true` | `false` |

### ServiceCollectionExtensions Logic

Location: `backend/src/AFC27.KMS.WebApi/Extensions/ServiceCollectionExtensions.cs`

The infrastructure services automatically detect the environment:

```csharp
// Database selection (automatic)
if (ConnectionStringHelper.IsSqlite(connectionString))
{
    // Uses SQLite for development
    services.AddDbContext<KmsDbContext>(options => options.UseSqlite(connectionString));
}
else
{
    // Uses SQL Server for production
    services.AddDbContext<KmsDbContext>(options => options.UseSqlServer(connectionString, ...));
}

// Cache selection (based on config)
if (useInMemoryCache || string.IsNullOrEmpty(redisConnection))
{
    services.AddMemoryCache();
    services.AddScoped<ICacheService, MemoryCacheService>();
}
else
{
    services.AddStackExchangeRedisCache(...);
    services.AddScoped<ICacheService, RedisCacheService>();
}
```

### Program.cs Behavior

Location: `backend/src/AFC27.KMS.WebApi/Program.cs`

```csharp
// Authentication (conditional)
var disableAuth = builder.Configuration.GetValue<bool>("DisableAuth");
if (!disableAuth)
{
    builder.Services.AddAuthenticationServices(builder.Configuration);
}

// Database initialization
if (ConnectionStringHelper.IsSqlite(connectionString))
{
    dbContext.Database.EnsureCreated();  // Creates SQLite DB
}
else
{
    dbContext.Database.Migrate();         // Applies EF migrations
}

// Seeding (conditional)
var seedDatabase = builder.Configuration.GetValue<bool>("SeedDatabase");
if (seedDatabase)
{
    await DatabaseSeeder.SeedAsync(dbContext);
}
```

---

## Frontend Configuration

### .env.development

Location: `frontend/.env.development`

```env
VITE_API_URL=http://localhost:5001/api
VITE_USE_MOCK_DATA=false
VITE_APP_NAME=AFC Asian Cup 2027 KMS
```

### Environment Variables

| Variable | Development | Production |
|----------|-------------|------------|
| `VITE_API_URL` | `http://localhost:5001/api` | `/api` (proxied) or full URL |
| `VITE_USE_MOCK_DATA` | `false` | `false` |

### API Service Fallback

Location: `frontend/src/services/api.service.ts`

The API service automatically falls back to mock data:

```typescript
// Checks backend availability on startup
private async checkBackendAvailability(): Promise<void> {
  try {
    await this.client.get('/health/live', { timeout: 5000 })
    this.backendAvailable = true
    console.log('[API] Backend available - using live API')
  } catch {
    this.backendAvailable = false
    console.log('[API] Backend unavailable - falling back to mock data')
  }
}

// Components can check and use mock data
if (apiService.shouldUseMock()) {
  const data = await apiService.getMockService().getArticles()
}
```

---

## Mock Data

### Seeded Backend Data

Location: `backend/src/AFC27.KMS.WebApi/Data/DatabaseSeeder.cs`

| Entity | Count | Description |
|--------|-------|-------------|
| Departments | 10 | Operations, Media, Venues, Ticketing, Security, IT, HR, Marketing, Volunteers, Legal |
| Roles | 5 | Administrator, Department Manager, Content Editor, Content Reviewer, Viewer |
| Users | 15 | AFC 2027 staff with Arabic names |
| Categories | 8 | News, Venues, Teams, Ticketing, Volunteers, Operations, Media, Legacy |
| Tags | 10 | AFC2027, Football, Saudi Arabia, etc. |
| Articles | 20 | AFC 2027 themed news and articles |

### Frontend Mock Data

Location: `frontend/src/services/mock.service.ts`

| Entity | Count | Description |
|--------|-------|-------------|
| Users | 3 | Sample AFC staff |
| Articles | 5 | Featured news articles |
| Categories | 6 | Content categories |
| Tags | 6 | Content tags |
| Document Libraries | 3 | Operations, Media, Venues |
| Documents | 3 | Sample documents |
| Communities | 3 | Team communities |
| Discussions | 2 | Sample discussions |
| Calendar Events | 3 | Upcoming events |
| Notifications | 3 | Sample notifications |

---

## Switching to Production

### Step 1: Update Backend Configuration

Create or update `appsettings.Production.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=your-sql-server;Database=AFC27_KMS;User Id=sa;Password=YourPassword;TrustServerCertificate=True",
    "Redis": "your-redis-server:6379"
  },
  "UseInMemoryCache": false,
  "SeedDatabase": false,
  "DisableAuth": false,
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "TenantId": "your-tenant-id",
    "ClientId": "your-client-id",
    "Audience": "api://your-client-id"
  }
}
```

### Step 2: Update Frontend Configuration

Create `.env.production`:

```env
VITE_API_URL=/api
VITE_USE_MOCK_DATA=false
VITE_APP_NAME=AFC Asian Cup 2027 KMS
```

### Step 3: Run with Production Settings

```bash
# Backend
ASPNETCORE_ENVIRONMENT=Production dotnet run --project src/AFC27.KMS.WebApi

# Frontend
npm run build
# Deploy dist/ folder to web server
```

### Step 4: Database Migration

For SQL Server, run EF Core migrations:

```bash
cd backend/src/AFC27.KMS.WebApi
dotnet ef migrations add InitialCreate --context KmsDbContext
dotnet ef database update --context KmsDbContext
```

---

## Troubleshooting

### Backend Issues

**SQLite database not created:**
```bash
# Delete existing database and restart
rm backend/src/AFC27.KMS.WebApi/afc27_kms.db
dotnet run --project src/AFC27.KMS.WebApi
```

**Health check returns errors:**
- Check if port 5001 is available
- Verify `ASPNETCORE_ENVIRONMENT=Development` is set

**Swagger not loading:**
- This is a known issue with IFormFile parameters in DocumentsController
- API endpoints still work correctly

### Frontend Issues

**Mock data not loading:**
- Check browser console for errors
- Verify `mock.service.ts` is imported correctly

**API connection failed:**
- Ensure backend is running on port 5001
- Check CORS settings in backend

**Build errors:**
```bash
# Clear node_modules and reinstall
rm -rf node_modules package-lock.json
npm install
npm run build
```

---

## File Structure Reference

```
backend/
├── src/
│   ├── AFC27.KMS.WebApi/
│   │   ├── appsettings.json              # Base config (production)
│   │   ├── appsettings.Development.json  # Dev config (SQLite)
│   │   ├── Program.cs                    # Entry point with seeding
│   │   ├── Extensions/
│   │   │   └── ServiceCollectionExtensions.cs  # DI configuration
│   │   └── Data/
│   │       ├── KmsDbContext.cs           # EF Core DbContext
│   │       └── DatabaseSeeder.cs         # Demo data seeder
│   └── Shared/
│       └── AFC27.KMS.Infrastructure/
│           ├── Caching/
│           │   ├── RedisCacheService.cs    # Production cache
│           │   └── MemoryCacheService.cs   # Development cache
│           └── Persistence/
│               └── ApplicationDbContext.cs # Base DbContext

frontend/
├── .env.development          # Dev environment variables
├── src/
│   └── services/
│       ├── api.service.ts    # API client with fallback
│       └── mock.service.ts   # Mock data service
```

---

## Version History

| Date | Version | Changes |
|------|---------|---------|
| 2025-12-10 | 1.0 | Initial local development setup |

---

## Contact

For questions about this setup, contact the development team.
