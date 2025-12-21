# AFC 2027 Knowledge Management System

A comprehensive enterprise knowledge management platform designed for the AFC Asian Cup 2027. Built with .NET 8 and Vue 3, featuring AI-powered analytics, real-time collaboration, workflow automation, and extensive integration capabilities.

## Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Features](#features)
- [Technology Stack](#technology-stack)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [API Reference](#api-reference)
- [Frontend Structure](#frontend-structure)
- [Security](#security)
- [Internationalization](#internationalization)
- [Integrations](#integrations)
- [License](#license)

---

## Overview

The AFC 2027 KMS is an enterprise-grade knowledge management system that provides:

- **Document Management** - Version control, templates, and metadata management
- **AI-Powered Analytics** - Summarization, entity extraction, sentiment analysis, and semantic search
- **Real-time Collaboration** - Multi-user editing with SignalR
- **Workflow Automation** - Service catalog, approvals, and task management
- **Meeting Integration** - Teams, Outlook, Google, and Zoom support
- **Learning Management** - Learning paths, quizzes, and certificates

### System Statistics

| Metric | Value |
|--------|-------|
| Backend Files (C#) | 348 |
| Frontend Components (Vue) | 799 |
| TypeScript Files | 3,260 |
| API Endpoints | ~134 |
| Database Entities | 25+ |

---

## Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                         FRONTEND                                 │
│  Vue 3 + TypeScript + Vite + PrimeVue + Pinia                   │
│  Port: 3001                                                      │
├─────────────────────────────────────────────────────────────────┤
│                         BACKEND API                              │
│  .NET 8 + Entity Framework Core + MassTransit                   │
│  Port: 5001                                                      │
├─────────────────────────────────────────────────────────────────┤
│                         DATABASE                                 │
│  SQLite (Dev) / SQL Server (Prod) + Redis Cache                 │
├─────────────────────────────────────────────────────────────────┤
│                      INTEGRATIONS                                │
│  OCR | Digital Signature | Meeting | AI Engine                   │
└─────────────────────────────────────────────────────────────────┘
```

---

## Features

### 1. Document Management

| Feature | Description |
|---------|-------------|
| Document Library | Browse, search, and filter documents |
| Version Control | Track document versions and history |
| Metadata Management | Custom metadata fields |
| Document Templates | Create documents from templates with versioning |
| Field-Level Encryption | AES-256-GCM encryption for sensitive data |

### 2. Content Management

| Feature | Description |
|---------|-------------|
| Block Editor | Rich content editor with multiple block types |
| Real-time Collaboration | SignalR-based multi-user editing |
| Content Versioning | Track all content changes |
| Content Categories | Hierarchical organization |
| Full-text Search | Search across all content |

**Supported Block Types:**
- Paragraph, Heading (h1-h6), Quote, Code
- Image, List (ordered/unordered), Divider, Callout

### 3. AI & Analytics

| Feature | Description |
|---------|-------------|
| Document Analysis | Automatic summarization, language detection, readability scoring |
| Sentiment Analysis | Positive/negative/neutral scoring |
| Entity Extraction | Named Entity Recognition (NER) |
| Auto-Tagging | AI-suggested tags for content |
| Semantic Search | Vector-based similarity search |
| Transcription | Audio/video to text with speaker identification |
| Batch Processing | Bulk document analysis |
| Content Recommendations | ML-based related content suggestions |

### 4. Service Catalog

| Feature | Description |
|---------|-------------|
| Service Categories | Hierarchical service organization |
| Service Definitions | Define services with SLA, fields, approvers |
| Service Requests | Submit and track requests |
| Approval Workflows | Multi-step approval with delegation |
| SLA Monitoring | Track response/resolution times with breach alerts |

### 5. Meeting Management

| Feature | Description |
|---------|-------------|
| External Integration | Microsoft Teams, Outlook, Google Meet, Zoom |
| Document Linking | Link documents to meetings |
| Agenda Management | Create and reorder agenda items |
| Action Items | Assign, track, and complete meeting tasks |
| Calendar Sync | Automatic calendar synchronization |

### 6. Quality Assurance

| Feature | Description |
|---------|-------------|
| Review Workflows | Create and assign quality reviews |
| Checklists | Quality checklists for review |
| Review Comments | Threaded comments on reviews |
| Dashboard | Quality metrics and overdue tracking |

### 7. Learning Management

| Feature | Description |
|---------|-------------|
| Learning Paths | Create structured learning journeys |
| Progress Tracking | Track completion by item |
| Quizzes | Create quizzes with scoring |
| Certificates | Generate and verify certificates |

### 8. KPI Management

| Feature | Description |
|---------|-------------|
| KPI Definitions | Define KPIs with targets |
| Value Recording | Record KPI values over time |
| Assignments | Assign KPIs to users/teams |
| Reports | Date-filtered KPI reports |

### 9. Additional Features

- **Polling & Surveys** - Create polls, cast votes, view results
- **Barcode & QR Codes** - Generate, scan, and validate barcodes
- **Communities** - Create and manage collaboration communities
- **Discussions** - Threaded discussion forums
- **Lessons Learned** - Knowledge repository
- **Notifications** - Real-time notification system with preferences

---

## Technology Stack

### Backend

| Technology | Purpose |
|------------|---------|
| .NET 8.0 | Runtime framework |
| Entity Framework Core | ORM and database access |
| MassTransit | Message bus for async processing |
| Serilog | Structured logging |
| Azure AD | Authentication provider |
| Redis | Distributed caching |

### Frontend

| Technology | Purpose |
|------------|---------|
| Vue 3 | UI framework (Composition API) |
| TypeScript | Type-safe JavaScript |
| Vite | Build tool and dev server |
| Pinia | State management |
| Vue Router | Client-side routing |
| Vue-i18n | Internationalization |
| PrimeVue | UI component library |
| SignalR | Real-time communication |

### Infrastructure

| Technology | Purpose |
|------------|---------|
| SQLite | Development database |
| SQL Server | Production database |
| Redis | Caching layer |
| RabbitMQ | Message queue |

---

## Getting Started

### Prerequisites

- .NET 8.0 SDK
- Node.js 18+ and npm
- Git

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd "Asia Cup POC"
   ```

2. **Backend Setup**
   ```bash
   cd backend/src/AFC27.KMS.WebApi
   dotnet restore
   dotnet build
   ```

3. **Frontend Setup**
   ```bash
   cd frontend
   npm install
   ```

### Running the Application

1. **Start the Backend** (Port 5001)
   ```bash
   cd backend/src/AFC27.KMS.WebApi
   ASPNETCORE_ENVIRONMENT=Development ASPNETCORE_URLS="http://localhost:5001" dotnet run
   ```

2. **Start the Frontend** (Port 3000/3001)
   ```bash
   cd frontend
   npm run dev
   ```

3. **Access the Application**

   | Service | URL |
   |---------|-----|
   | Frontend | http://localhost:3001 |
   | Backend API | http://localhost:5001 |
   | Swagger UI | http://localhost:5001/swagger |

### Development Mode Features

- SQLite database (auto-created)
- Database seeding with demo data
- Authentication disabled (mock users)
- In-memory caching
- Mock AI services

---

## Configuration

### Backend Configuration

**appsettings.json**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=AFC27_KMS;...",
    "Redis": "localhost:6379"
  },
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "TenantId": "your-tenant-id",
    "ClientId": "your-client-id"
  },
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "http://localhost:3001"
    ]
  }
}
```

**Development Settings (appsettings.Development.json)**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=afc27_kms.db"
  },
  "UseInMemoryCache": true,
  "SeedDatabase": true,
  "DisableAuth": true
}
```

### Frontend Configuration

**.env.development**
```env
VITE_API_URL=http://localhost:5001/api
VITE_APP_TITLE=Intalio KMS
```

### Development vs Production

| Feature | Development | Production |
|---------|-------------|------------|
| Database | SQLite | SQL Server |
| Cache | In-Memory | Redis |
| Auth | Disabled | Azure AD |
| Data | Seeded demo data | Empty/Migrated |

---

## API Reference

### API Endpoints by Module

| Module | Base Route | Endpoints |
|--------|------------|-----------|
| AI Analysis | `/api/ai` | 20 |
| Service Catalog | `/api/services` | 24 |
| Meetings | `/api/meetings` | 18 |
| Templates | `/api/templates` | 14 |
| Quality Assurance | `/api/quality` | 15 |
| KPI Management | `/api/kpis` | 12 |
| Learning | `/api/learning` | 14 |
| Polling | `/api/polls` | 10 |
| Barcodes | `/api/barcodes` | 7 |

### Key Endpoints

**AI Analysis**
```
POST /api/ai/summarize              - Summarize text
POST /api/ai/entities/extract       - Extract named entities
POST /api/ai/search/semantic        - Semantic search
POST /api/ai/transcribe             - Start transcription
GET  /api/ai/dashboard              - AI insights dashboard
```

**Service Catalog**
```
GET  /api/services/catalog          - List services
POST /api/services/{id}/request     - Submit service request
GET  /api/services/requests/{id}    - Get request details
PUT  /api/services/requests/{id}/status - Update request status
```

**Meetings**
```
GET  /api/meetings                  - List meetings
POST /api/meetings                  - Create meeting
POST /api/meetings/{id}/agenda      - Add agenda item
POST /api/meetings/{id}/actions     - Add action item
```

---

## Frontend Structure

### Views (38 Total)

| Category | Views |
|----------|-------|
| Authentication | Login, SSO Callback |
| Dashboard | Main Dashboard |
| Content | List, Detail, Editor |
| Documents | Library, Management |
| Collaboration | Communities, Discussions, Lessons Learned |
| Media | Gallery, Video Editor |
| Workflow | Task Inbox, Service Catalog |
| AI Services | AI Hub, Chat, Semantic Search |
| Admin | Users, Roles, Groups, Audit Log, Legal Hold |

### State Management (Pinia Stores)

**Auth Store**
- User session management
- Token refresh
- Role/permission checking
- SSO support

**UI Store**
- Theme (light/dark)
- Locale (en/ar)
- Sidebar state
- RTL support

### Composables

| Composable | Purpose |
|------------|---------|
| useScrollReveal | Intersection Observer animations |
| useRipple | Material Design ripple effect |
| useDirection | RTL/LTR detection |
| useReducedMotion | Accessibility preference |
| useCountUp | Animated number counter |
| useCollaboration | Real-time collaboration |

---

## Security

### Authentication

- **Azure AD SSO** - Enterprise single sign-on
- **Email/Password** - Traditional authentication
- **Token Refresh** - Automatic token renewal

### Authorization

- **Role-based Access Control (RBAC)** - Administrator, User, etc.
- **Permission-based Features** - Granular permissions

### Authorization Policies

| Policy | Description |
|--------|-------------|
| CanManageContent | Content management operations |
| CanPublishContent | Template publishing |
| CanManageUsers | User administration |

### Data Security

- **Field-Level Encryption** - AES-256-GCM for sensitive data
- **Key Management** - Key rotation, versioning, lifecycle tracking
- **Audit Logging** - All security operations logged

---

## Internationalization

### Supported Languages

| Language | Code | Direction |
|----------|------|-----------|
| English | en | LTR |
| Arabic | ar | RTL |

### Features

- Full RTL support for Arabic
- Localized date/time formatting
- Localized number formatting
- Translation keys organized by feature

### Usage

```typescript
import { useI18n } from 'vue-i18n'

const { t, locale } = useI18n()

// Switch language
locale.value = 'ar'

// Use translations
t('nav.dashboard')  // "Dashboard" or "لوحة المعلومات"
```

---

## Integrations

### OCR Service

```json
{
  "Integration": {
    "Ocr": {
      "IsEnabled": false,
      "BaseUrl": "https://ocr-service.intalio.com/api/v1/",
      "TimeoutSeconds": 60,
      "RetryCount": 3
    }
  }
}
```

**Capabilities:**
- Async document submission
- PDF, PNG, JPG, TIFF, BMP support
- Max file size: 50 MB
- Webhook notifications

### Digital Signature Service

```json
{
  "Integration": {
    "DigitalSignature": {
      "IsEnabled": false,
      "BaseUrl": "https://signature-service.intalio.com/api/v1/",
      "SignatureValidityDays": 365
    }
  }
}
```

**Capabilities:**
- Signature request creation
- Multi-signer support
- Signature verification
- Signed document download

### Meeting Service

```json
{
  "Integration": {
    "Meeting": {
      "IsEnabled": false,
      "BaseUrl": "https://meeting-service.intalio.com/api/v1/",
      "SyncIntervalMinutes": 15,
      "EnableTeamsIntegration": true
    }
  }
}
```

**Capabilities:**
- Create/update/cancel meetings
- Recording access
- Action item tracking
- Calendar synchronization

### AI Engine

```json
{
  "Integration": {
    "AiEngine": {
      "IsEnabled": false,
      "BaseUrl": "https://ai-engine.intalio.com/api/v1/",
      "EnableSentimentAnalysis": true,
      "EnableEntityExtraction": true,
      "EnableAutoTagging": true
    }
  }
}
```

**Capabilities:**
- Document classification
- Content summarization
- Semantic search
- Entity extraction

---

## Database Entities

### Core Modules

| Module | Entities |
|--------|----------|
| Service Catalog | ServiceCategory, CatalogService, ServiceRequest, RequestApproval, RequestComment, RequestAttachment, RequestStatusHistory |
| Meetings | ExternalMeetingLink, MeetingDocumentLink, MeetingAttendee, MeetingAgendaItem, MeetingActionItem |
| AI Analysis | DocumentAnalysis, ExtractedEntity, BatchAnalysisJob, TranscriptionResult, ContentRecommendation |
| Security | EncryptionKey, EncryptionAuditLog, EncryptedFieldReference |

---

## Project Structure

```
Asia Cup POC/
├── backend/
│   └── src/
│       └── AFC27.KMS.WebApi/
│           ├── Data/                 # Database context and entities
│           ├── Features/             # Feature modules
│           │   ├── AIAnalysis/       # AI and analytics
│           │   ├── Barcodes/         # Barcode generation
│           │   ├── KpiManagement/    # KPI tracking
│           │   ├── Learning/         # Learning management
│           │   ├── Meetings/         # Meeting integration
│           │   ├── Polling/          # Polls and surveys
│           │   ├── QualityAssurance/ # Quality reviews
│           │   ├── ServiceCatalog/   # Service requests
│           │   └── Templates/        # Document templates
│           ├── Integration/          # External service integrations
│           └── Security/             # Encryption and security
├── frontend/
│   └── src/
│       ├── assets/                   # Static assets and styles
│       ├── components/               # Reusable Vue components
│       ├── composables/              # Vue composables
│       ├── design-system/            # Design tokens and themes
│       ├── locales/                  # i18n translation files
│       ├── plugins/                  # Vue plugins
│       ├── router/                   # Vue Router configuration
│       ├── services/                 # API services
│       ├── stores/                   # Pinia stores
│       ├── types/                    # TypeScript types
│       └── views/                    # Page components
└── docs/                             # Documentation
```

---

## Scripts

### Backend

```bash
# Build
dotnet build

# Run
dotnet run

# Run with environment
ASPNETCORE_ENVIRONMENT=Development dotnet run

# Add migration
dotnet ef migrations add <MigrationName>

# Update database
dotnet ef database update
```

### Frontend

```bash
# Development
npm run dev

# Build
npm run build

# Type check
npm run type-check

# Lint
npm run lint
```

---

## Documentation

Additional documentation is available in the `/docs` directory:

- [Local Development Setup](docs/LOCAL_DEVELOPMENT_SETUP.md)
- [Configuration Reference](docs/CONFIGURATION_REFERENCE.md)
- [Implementation Plan](docs/IMPLEMENTATION-PLAN.md)
- [Gap Closure Roadmap](docs/GAP-CLOSURE-ROADMAP.md)

---

## License

Copyright 2025 AFC Asian Cup 2027 Organizing Committee. All rights reserved.

---

## Support

For support and questions, please contact the development team.
