# AFC Asian Cup 2027 - Knowledge Management System (KMS)

A comprehensive Knowledge Management System built for the AFC Asian Cup 2027 tournament organization.

## Technology Stack

### Backend (.NET 8)
- **Framework**: ASP.NET Core 8.0 Modular Monolith
- **Database**: SQL Server (Production) / SQLite (Development)
- **Cache**: Redis (Production) / In-Memory (Development)
- **Authentication**: Azure AD / OpenID Connect
- **ORM**: Entity Framework Core 8
- **Validation**: FluentValidation
- **Logging**: Serilog

### Frontend (Vue.js 3)
- **Framework**: Vue.js 3 with Composition API
- **UI Library**: PrimeVue 4
- **State Management**: Pinia
- **Internationalization**: Vue I18n (English/Arabic RTL)
- **Build Tool**: Vite
- **Type Checking**: TypeScript

## Project Structure

```
Asia Cup POC/
├── backend/                    # .NET 8 Backend
│   └── src/
│       ├── AFC27.KMS.WebApi/   # Main API Host
│       ├── Modules/            # Feature Modules
│       │   ├── AFC27.KMS.Identity/
│       │   ├── AFC27.KMS.Content/
│       │   ├── AFC27.KMS.Documents/
│       │   ├── AFC27.KMS.Search/
│       │   ├── AFC27.KMS.Collaboration/
│       │   ├── AFC27.KMS.Media/
│       │   ├── AFC27.KMS.Workflow/
│       │   ├── AFC27.KMS.Calendar/
│       │   ├── AFC27.KMS.Notifications/
│       │   ├── AFC27.KMS.AI/
│       │   └── AFC27.KMS.Integration/
│       └── Shared/             # Shared Libraries
│           ├── AFC27.KMS.SharedKernel/
│           ├── AFC27.KMS.Contracts/
│           └── AFC27.KMS.Infrastructure/
├── frontend/                   # Vue.js 3 Frontend
│   └── src/
│       ├── views/
│       ├── components/
│       ├── stores/
│       ├── services/
│       └── locales/
└── docs/                       # Documentation
```

## Quick Start (Local Development)

### Prerequisites
- .NET 8 SDK
- Node.js 18+
- npm 9+

### Backend

```bash
cd backend
ASPNETCORE_ENVIRONMENT=Development dotnet run --project src/AFC27.KMS.WebApi
```

### Frontend

```bash
cd frontend
npm install
npm run dev
```

### Access Points

| Service | URL |
|---------|-----|
| Frontend | http://localhost:3000 |
| Backend API | http://localhost:5001 |
| Swagger UI | http://localhost:5001/swagger |

## Documentation

- [Local Development Setup](docs/LOCAL_DEVELOPMENT_SETUP.md) - Complete guide for local testing
- [Configuration Reference](docs/CONFIGURATION_REFERENCE.md) - All configuration options

## Features

### Core Modules
- **Identity**: User management, roles, permissions, org chart
- **Content**: Articles, news, blogs, announcements
- **Documents**: Document library with versioning
- **Search**: Full-text search (Elasticsearch)
- **Collaboration**: Communities, forums, comments
- **Media**: Image/video gallery
- **Workflow**: Service catalog, forms, approvals
- **Calendar**: Events, RSVP
- **Notifications**: Email, push, in-app
- **AI**: Document summarization, transcription
- **Integration**: M365, SharePoint, ERP connectors

### Key Features
- Bilingual support (English/Arabic RTL)
- Role-based access control
- Document versioning and audit trail
- Real-time notifications
- Advanced search with facets
- Mobile-responsive design

## Development vs Production

| Feature | Development | Production |
|---------|-------------|------------|
| Database | SQLite | SQL Server |
| Cache | In-Memory | Redis |
| Auth | Disabled | Azure AD |
| Data | Seeded demo data | Empty/Migrated |

See [Configuration Reference](docs/CONFIGURATION_REFERENCE.md) for switching between environments.

## API Endpoints

```
/api/identity/*        - User and authentication
/api/content/*         - Articles and content
/api/documents/*       - Document management
/api/search/*          - Global search
/api/collaboration/*   - Communities and discussions
/api/media/*           - Media gallery
/api/workflow/*        - Service catalog and tasks
/api/calendar/*        - Events
/api/notifications/*   - Notifications
```

## License

Proprietary - AFC Asian Cup 2027 Local Organizing Committee
