# AFC Asian Cup 2027 - Knowledge Management System Implementation Plan

## Project Overview
Enterprise Knowledge Management System for the Local Organising Committee (LOC AFC27) supporting 10,000+ users with bilingual Arabic/English support and world-class UI/UX.

## Technology Stack (Confirmed)
| Layer | Technology |
|-------|------------|
| Frontend | Vue.js 3 + PrimeVue |
| Backend | .NET 8 (C#) |
| Database | SQL Server + Redis |
| Search | Elasticsearch |
| Message Queue | RabbitMQ + MassTransit |
| AI | Intalio AI Engine |
| Deployment | On-Premises (Kubernetes) |
| Integrations | M365/SharePoint + SAP/Oracle ERP |

---

## Architecture Decision: Modular Monolith

**Rationale:**
- Reduced operational complexity for on-premises deployment
- Faster MVP delivery
- Clear path to extract microservices as needed (Media, AI, Search workers)

```
                    LOAD BALANCER (HAProxy/Nginx)
                              |
        +---------------------+---------------------+
        |                     |                     |
   API Gateway           Vue.js SPA           Static Assets
     (YARP)              (PrimeVue)              (CDN)
        |
        |  JWT/OAuth2
        |
   .NET 8 MODULAR MONOLITH
   +--------------------------------------------------+
   | Identity | Content | Documents | Search | Collab |
   | Media | Workflow | Calendar | Notifications | AI |
   +--------------------------------------------------+
        |              |              |
   SQL Server      Redis         Elasticsearch

   MESSAGE BUS (RabbitMQ + MassTransit)
        |              |              |
   Media Worker   AI Worker   Notification Worker
```

---

## Project Structure

### Backend (.NET 8)
```
AFC27.KMS/
├── src/
│   ├── AFC27.KMS.ApiGateway/          # YARP API Gateway
│   ├── AFC27.KMS.WebApi/              # Main API Host
│   ├── Modules/
│   │   ├── AFC27.KMS.Identity/        # Auth, Users, Roles, Groups
│   │   ├── AFC27.KMS.Content/         # Articles, News, Blogs, Pages
│   │   ├── AFC27.KMS.Documents/       # Document Library, Versioning
│   │   ├── AFC27.KMS.Search/          # Elasticsearch Integration
│   │   ├── AFC27.KMS.Collaboration/   # Communities, Forums, Comments
│   │   ├── AFC27.KMS.Media/           # Image/Video, Editor
│   │   ├── AFC27.KMS.Workflow/        # Service Catalog, Forms
│   │   ├── AFC27.KMS.Calendar/        # Events, RSVP
│   │   ├── AFC27.KMS.Notifications/   # Email, Push, In-App
│   │   ├── AFC27.KMS.AI/              # Intalio Integration
│   │   └── AFC27.KMS.Integration/     # M365, ERP Connectors
│   ├── Shared/
│   │   ├── AFC27.KMS.SharedKernel/    # Base entities, Domain primitives
│   │   ├── AFC27.KMS.Contracts/       # Shared DTOs
│   │   └── AFC27.KMS.Infrastructure/  # Cross-cutting concerns
│   └── Workers/
│       ├── AFC27.KMS.MediaWorker/     # FFmpeg processing
│       ├── AFC27.KMS.AIWorker/        # AI processing
│       └── AFC27.KMS.NotificationWorker/
├── tests/
└── docker/
```

### Frontend (Vue.js 3)
```
afc27-kms-frontend/
├── src/
│   ├── assets/styles/
│   │   ├── themes/afc27-theme.ts      # PrimeVue preset
│   │   └── _rtl.scss                  # RTL utilities
│   ├── components/
│   │   ├── common/                    # Header, Sidebar, Footer
│   │   ├── widgets/                   # Dashboard widgets
│   │   ├── content/                   # Article editor/viewer
│   │   ├── documents/                 # Document library
│   │   ├── collaboration/             # Forums, comments
│   │   ├── media/                     # Gallery, video editor
│   │   ├── search/                    # Global search
│   │   ├── workflow/                  # Task inbox, forms
│   │   ├── calendar/                  # Events
│   │   └── admin/                     # User/role management
│   ├── composables/                   # useAuth, useDirection, etc.
│   ├── stores/                        # Pinia stores
│   ├── services/                      # API services
│   ├── router/
│   ├── views/
│   ├── locales/                       # en.json, ar.json
│   └── plugins/
│       ├── primevue.ts
│       └── i18n.ts
```

---

## Module Priority & Dependencies

| Priority | Module | Dependencies | Description |
|----------|--------|--------------|-------------|
| P0 | Identity | SharedKernel | SSO, RBAC, User profiles, Org chart |
| P0 | Content | Identity, Search | Articles, News, Blogs, Templates |
| P0 | Documents | Identity, Search | Libraries, Versioning, Audit |
| P0 | Search | Content, Documents | Elasticsearch, Facets |
| P0 | Notifications | Identity | Email, Push, In-app |
| P1 | Collaboration | Identity, Content, Notifications | Communities, Forums |
| P1 | Media | Documents, Search | Gallery, Video Editor (FFmpeg) |
| P1 | Workflow | Identity, Notifications | Service Catalog, Forms |
| P1 | Calendar | Identity, Notifications | Events, RSVP, Outlook sync |
| P2 | AI | Content, Documents, Media | Transcription, Summarization |
| P2 | Integration | Documents, Workflow | M365, SharePoint, ERP |

---

## Key Technical Decisions

### 1. Authentication
- Azure AD / ADFS SSO via OpenID Connect
- JWT tokens with refresh
- Resource-based authorization for documents

### 2. RTL/LTR Support
- CSS Logical Properties (`padding-inline-start`, etc.)
- Vue composable `useDirection()` for dynamic switching
- PrimeVue built-in RTL support

### 3. Search Strategy
- Elasticsearch for full-text search
- SQL Server for relational queries
- Redis for caching search results

### 4. File Storage
- Hot storage (NVMe SSD) for recent files
- Warm storage (SAS HDD) for archives
- MinIO/NetApp for object storage gateway

### 5. Video Processing
- Server-side FFmpeg via background workers
- Async job queue (RabbitMQ)
- Transcoding to web-optimized formats

---

## Implementation Phases

### Phase 0: Foundation (Weeks 1-4)
- [ ] Dev environment setup (Docker, local K8s)
- [ ] CI/CD pipeline (Azure DevOps)
- [ ] Project scaffolding
- [ ] Database schema & migrations
- [ ] Azure AD integration
- [ ] API Gateway (YARP) setup
- [ ] Logging/monitoring baseline

### Phase 1: MVP Core Platform (Weeks 5-12)
- [ ] Identity Module (SSO, RBAC, profiles, directory)
- [ ] Content Module (WYSIWYG editor, articles, news)
- [ ] Search Module (Elasticsearch, basic search)
- [ ] Notifications Module (in-app, email)
- [ ] UI Framework (dashboard, navigation, RTL/LTR, theming)

### Phase 2: Document Management (Weeks 13-18)
- [ ] Document libraries & folders
- [ ] Version control (major/minor)
- [ ] Check-in/check-out
- [ ] Custom metadata
- [ ] In-browser preview
- [ ] Audit trail
- [ ] Granular permissions
- [ ] Full-text indexing (PDF, Office)

### Phase 3: Collaboration (Weeks 19-24)
- [ ] Community workspaces
- [ ] Discussion forums
- [ ] Commenting system
- [ ] @Mentions
- [ ] Follow functionality
- [ ] Lessons Learned

### Phase 4: Multimedia (Weeks 25-30)
- [ ] Image/video gallery
- [ ] HTML5 video player
- [ ] Thumbnail generation
- [ ] Video transcoding
- [ ] Video editor (trimming, overlays)
- [ ] FFmpeg worker

### Phase 5: Workflow & Calendar (Weeks 31-36)
- [ ] Service catalog
- [ ] Dynamic forms
- [ ] Approval workflows
- [ ] Task inbox
- [ ] Voting/polling
- [ ] Calendar views
- [ ] Event management & RSVP
- [ ] Outlook/Google sync

### Phase 6: AI Integration (Weeks 37-42)
- [ ] Intalio AI integration
- [ ] Audio transcription
- [ ] Document summarization
- [ ] Meeting minutes generation
- [ ] Document classification

### Phase 7: Enterprise Integration (Weeks 43-48)
- [ ] M365/SharePoint document sync
- [ ] Calendar sync
- [ ] User directory sync
- [ ] SAP/Oracle ERP connectors
- [ ] Service catalog integration

### Phase 8: Polish & Hardening (Weeks 49-54)
- [ ] Auto-generated org chart
- [ ] Analytics dashboard
- [ ] Performance optimization
- [ ] Security audit & penetration testing
- [ ] Load testing

### Phase 9: UAT & Go-Live (Weeks 55-60)
- [ ] User acceptance testing
- [ ] Training materials
- [ ] Documentation
- [ ] Production deployment
- [ ] 24/7 support setup

---

## Database Core Entities

### Identity
- `Users`, `Roles`, `Permissions`, `UserRoles`
- `Departments`, `Groups`, `GroupMembers`

### Content
- `Articles`, `ArticleVersions`, `Categories`, `Tags`
- `Announcements`, `Templates`

### Documents
- `DocumentLibraries`, `Folders`, `Documents`
- `DocumentVersions`, `DocumentMetadata`, `DocumentAudit`

### Collaboration
- `Communities`, `CommunityMembers`
- `Discussions`, `Comments`, `Mentions`
- `Follows`, `LessonsLearned`

### Media
- `MediaGalleries`, `MediaItems`, `MediaThumbnails`
- `VideoEditJobs`

### Workflow
- `Services`, `ServiceRequests`, `Tasks`
- `Forms`, `FormSubmissions`, `Polls`, `Votes`

### Calendar
- `Events`, `EventAttendees`, `RSVPs`

---

## API Endpoints (Key Examples)

```
# Identity
POST   /api/identity/auth/login
GET    /api/identity/users
GET    /api/identity/org-chart

# Content
GET    /api/content/articles
POST   /api/content/articles
POST   /api/content/articles/{id}/publish

# Documents
GET    /api/documents/libraries
POST   /api/documents/upload
GET    /api/documents/{id}/versions
POST   /api/documents/{id}/checkout

# Search
GET    /api/search?q={query}
GET    /api/search/suggest

# Collaboration
GET    /api/collaboration/communities
POST   /api/collaboration/comments
POST   /api/collaboration/follow/{type}/{id}

# Media
POST   /api/media/upload
POST   /api/media/video/{id}/edit
GET    /api/media/{id}/stream

# Workflow
GET    /api/workflow/services
GET    /api/workflow/tasks
POST   /api/workflow/tasks/{id}/approve

# Calendar
GET    /api/calendar/events
POST   /api/calendar/events/{id}/rsvp
```

---

## High Availability Requirements

| Component | Strategy |
|-----------|----------|
| SQL Server | Always On (Primary + Secondary) |
| Redis | 6-node cluster with Sentinel |
| Elasticsearch | 3-node cluster |
| Application | 3+ replicas with rolling updates |
| Load Balancer | HAProxy/Nginx with health checks |
| File Storage | RAID + replication |

**SLA Target: 99.95% uptime**

---

## Security Checklist

- [ ] TLS 1.3 for all traffic
- [ ] SQL Server TDE (Transparent Data Encryption)
- [ ] Field-level encryption for PII
- [ ] Azure Key Vault / HashiCorp Vault for secrets
- [ ] OWASP Top 10 compliance
- [ ] Regular security audits
- [ ] Rate limiting on API Gateway
- [ ] Input validation & sanitization
- [ ] CORS configuration
- [ ] CSP headers

---

## Monitoring & Observability

| Tool | Purpose |
|------|---------|
| Prometheus + Grafana | Metrics & dashboards |
| ELK Stack | Centralized logging |
| Jaeger | Distributed tracing |
| Health endpoints | `/health/live`, `/health/ready` |

---

## Team Composition (Recommended)

| Role | Count | Focus |
|------|-------|-------|
| Backend Developers | 4-5 | .NET modules, APIs |
| Frontend Developers | 3-4 | Vue.js, PrimeVue |
| DevOps Engineer | 1-2 | CI/CD, K8s, monitoring |
| QA Engineers | 2 | Testing, automation |
| UI/UX Designer | 1 | Design system, branding |
| Tech Lead | 1 | Architecture, code review |
| Project Manager | 1 | Coordination, stakeholders |

---

## Success Criteria

- [ ] SSO working with Azure AD
- [ ] Bilingual UI (English + Arabic RTL)
- [ ] Document upload with version control
- [ ] Full-text search across all content
- [ ] Video editing capabilities
- [ ] 99.95% uptime in production
- [ ] < 2s page load time
- [ ] Mobile-responsive design
- [ ] Accessibility (WCAG 2.1 AA)
