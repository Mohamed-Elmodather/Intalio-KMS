# AFC27 KMS - Configuration Reference

Quick reference for all configurable settings.

---

## Backend Settings

### appsettings.{Environment}.json

| Setting | Type | Description | Dev Default | Prod Default |
|---------|------|-------------|-------------|--------------|
| `ConnectionStrings:DefaultConnection` | string | Database connection | `Data Source=afc27_kms.db` | SQL Server string |
| `ConnectionStrings:Redis` | string | Redis connection | `""` (empty) | `localhost:6379` |
| `UseInMemoryCache` | bool | Use in-memory cache | `true` | `false` |
| `SeedDatabase` | bool | Seed demo data | `true` | `false` |
| `DisableAuth` | bool | Skip authentication | `true` | `false` |
| `AzureAd:TenantId` | string | Azure AD tenant | - | Required |
| `AzureAd:ClientId` | string | Azure AD client | - | Required |
| `Cors:AllowedOrigins` | string[] | Allowed CORS origins | `["http://localhost:3000"]` | Production URLs |

### Environment Variables

| Variable | Description |
|----------|-------------|
| `ASPNETCORE_ENVIRONMENT` | `Development` or `Production` |
| `ASPNETCORE_URLS` | Server URLs (e.g., `http://localhost:5001`) |

---

## Frontend Settings

### .env.{environment}

| Variable | Description | Dev Default | Prod Default |
|----------|-------------|-------------|--------------|
| `VITE_API_URL` | Backend API URL | `http://localhost:5001/api` | `/api` |
| `VITE_USE_MOCK_DATA` | Force mock data | `false` | `false` |
| `VITE_APP_NAME` | Application name | `AFC Asian Cup 2027 KMS` | Same |

---

## Quick Configuration Changes

### Enable/Disable Authentication

**Backend** (`appsettings.Development.json`):
```json
{
  "DisableAuth": false  // Set to false to enable auth
}
```

### Switch to SQL Server

**Backend** (`appsettings.Development.json`):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=AFC27_KMS;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

### Enable Redis Cache

**Backend** (`appsettings.Development.json`):
```json
{
  "ConnectionStrings": {
    "Redis": "localhost:6379"
  },
  "UseInMemoryCache": false
}
```

### Disable Database Seeding

**Backend** (`appsettings.Development.json`):
```json
{
  "SeedDatabase": false
}
```

### Force Mock Data in Frontend

**Frontend** (`.env.development`):
```env
VITE_USE_MOCK_DATA=true
```

---

## Database Detection Logic

The backend automatically detects which database to use:

```
Connection String contains "Data Source=" AND ends with ".db"
  → SQLite (EnsureCreated)

Otherwise
  → SQL Server (Migrate)
```

---

## Cache Selection Logic

```
UseInMemoryCache = true OR Redis connection empty
  → MemoryCacheService

Otherwise
  → RedisCacheService
```

---

## Files to Modify for Production

1. **Create** `backend/src/AFC27.KMS.WebApi/appsettings.Production.json`
2. **Create** `frontend/.env.production`
3. **Update** Azure AD settings in backend config
4. **Run** database migrations
5. **Build** and deploy frontend

---

## Common Commands

```bash
# Development
ASPNETCORE_ENVIRONMENT=Development dotnet run --project src/AFC27.KMS.WebApi
npm run dev

# Production build
ASPNETCORE_ENVIRONMENT=Production dotnet publish -c Release
npm run build

# Database migrations (SQL Server)
dotnet ef migrations add InitialCreate --context KmsDbContext
dotnet ef database update --context KmsDbContext

# Reset SQLite database
rm afc27_kms.db && dotnet run
```
