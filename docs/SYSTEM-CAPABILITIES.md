# AFC27 KMS — System Capabilities & Feature Reference

A comprehensive reference of all capabilities, features, and integrations implemented in the AFC Asian Cup 2027 Knowledge Management System.

---

## Table of Contents

1. [System Overview](#1-system-overview)
2. [Content Management](#2-content-management)
3. [Document Management](#3-document-management)
4. [AI & Intelligent Services](#4-ai--intelligent-services)
5. [Search](#5-search)
6. [Collaboration](#6-collaboration)
7. [Workflow & Service Catalog](#7-workflow--service-catalog)
8. [Task Management](#8-task-management)
9. [Calendar & Events](#9-calendar--events)
10. [Notifications](#10-notifications)
11. [Media Management](#11-media-management)
12. [Administration](#12-administration)
13. [Integrations](#13-integrations)
14. [Authentication & Authorization](#14-authentication--authorization)
15. [Infrastructure & Architecture](#15-infrastructure--architecture)
16. [Frontend Design System & UX](#16-frontend-design-system--ux)
17. [Summary Metrics](#17-summary-metrics)

---

## 1. System Overview

The AFC27 KMS is an enterprise-grade Knowledge Management System built with a modular monolith architecture. It consists of:

- **Backend**: .NET 8.0 ASP.NET Core with 11 feature modules, 3 background workers, and 3 shared libraries
- **Frontend**: Vue.js 3 + TypeScript with PrimeVue 4, Pinia state management, and full bilingual (EN/AR) support
- **Database**: SQL Server (production) / SQLite (development) via EF Core 8
- **Messaging**: MassTransit with RabbitMQ (production) / In-Memory (development)
- **Caching**: Redis (production) / In-Memory (development)
- **Authentication**: Azure AD / OpenID Connect with JWT Bearer tokens

### Architecture Diagram

```
AFC27 KMS
├── Main API Layer
│   ├── AFC27.KMS.WebApi          (HTTP API host)
│   │   ├── Features/             (AI Analysis, Meetings, ServiceCatalog, Barcodes, KPIs, etc.)
│   │   ├── Integration/          (AI Engine, OCR, Signature, Meeting connectors)
│   │   └── Data/                 (DbContext, Entities, Migrations)
│   └── AFC27.KMS.ApiGateway      (Request routing)
│
├── Feature Modules (11)
│   ├── AFC27.KMS.Identity        (Auth, Users, Roles, Groups, Departments)
│   ├── AFC27.KMS.Content         (Articles, Categories, Tags, Block Editor)
│   ├── AFC27.KMS.Documents       (Files, Libraries, Versions, Permissions)
│   ├── AFC27.KMS.AI              (Chat, RAG, Embeddings, Semantic Search)
│   ├── AFC27.KMS.Search          (Global search, Facets, Analytics)
│   ├── AFC27.KMS.Notifications   (Email, Push, In-app)
│   ├── AFC27.KMS.Collaboration   (Communities, Discussions, Lessons Learned)
│   ├── AFC27.KMS.Media           (Images, Videos, Galleries)
│   ├── AFC27.KMS.Calendar        (Events, Holidays)
│   ├── AFC27.KMS.Workflow        (Service Catalog, Approvals, SLAs)
│   ├── AFC27.KMS.Admin           (Audit, Legal Hold, Quarantine)
│   └── AFC27.KMS.Integration     (External system connectors)
│
├── Background Workers (3)
│   ├── AFC27.KMS.AIWorker        (Text extraction, Chunking, Embeddings)
│   ├── AFC27.KMS.MediaWorker     (Image processing, Video transcoding)
│   └── AFC27.KMS.NotificationWorker (Email/Push delivery)
│
└── Shared Libraries (3)
    ├── AFC27.KMS.SharedKernel    (Base entities, Domain events, Interfaces)
    ├── AFC27.KMS.Infrastructure  (EF Core, MassTransit, Caching, Storage)
    └── AFC27.KMS.Contracts       (Common DTOs, Response wrappers)
```

---

## 2. Content Management

**Backend Module**: `AFC27.KMS.Content`
**Frontend Views**: `ContentListView`, `ContentDetailView`, `ContentEditorView`
**API Base**: `/api/content/*`

### Capabilities

| Capability | Description |
|---|---|
| Article authoring | Rich text editor (Quill-based), draft/publish workflow, auto-save |
| Bilingual content | Full English + Arabic support with independent fields for each language |
| Content versioning | Version history with full audit trail per article |
| Categorization | Hierarchical categories for content organization |
| Tagging | Free-form tags for cross-cutting content discovery |
| Featured articles | Pin/highlight key content on the dashboard |
| Approval workflow | Content review pipeline before publication |
| Real-time collaboration | SignalR hub with CRDT-based conflict-free co-editing |
| Block editor | Block-based rich content composition |
| Content types | News, Articles, Announcements, Blog posts, Pages |
| Presence tracking | See who else is editing the same content |

### Frontend Features

- Grid/list view toggle for content listing
- Search by title with active filter chip display
- Filter by category (tournament news, announcements, features, behind-the-scenes)
- Filter by content type (news, article, announcement)
- Pagination with configurable page sizes (6, 12, 24, 48)
- Content detail page with hero image, author card, social sharing, related articles, comments
- Rich text editor with bilingual support and draft/publish toggle

### Database Entities

- `Article` (bilingual fields, status workflow, versioning)
- `Category` (hierarchical)
- `Tag`
- `ArticleTag` (many-to-many)
- `ArticleVersion` (audit trail)

---

## 3. Document Management

**Backend Module**: `AFC27.KMS.Documents`
**Frontend View**: `DocumentLibraryView`
**API Base**: `/api/documents/*`, `/api/libraries/*`, `/api/preview/*`

### Capabilities

| Capability | Description |
|---|---|
| Upload & storage | Drag-drop upload with progress tracking, up to 100MB per file |
| Versioning | Major and minor version tracking |
| Check-in/check-out | Exclusive editing locks to prevent conflicts |
| Document libraries | Organized folder and library hierarchies |
| Preview generation | Thumbnail and document preview rendering |
| Permission-based access | Document-level and folder-level access control lists |
| Status workflow | Draft → In Review → Published → Archived |
| Bulk operations | Move, copy, delete, share multiple files at once |
| File type support | PDF, Word, Excel, PowerPoint, images (with type-specific icons) |
| Audit trail | Full history of access and modification events |
| Metadata management | Custom metadata per document |
| Copy/move operations | Relocate documents across libraries |

### Frontend Features

- Drag-drop upload area with progress indicators
- Folder hierarchy navigation with breadcrumbs
- File type icons mapped to document extensions
- Search and filter by filename, type, status, file format
- Bulk selection with multi-action toolbar (delete, share, move, rename)
- List/grid view toggle
- Sortable columns: Name, Type, Status, Modified, Size, Author
- In-place rename, move, share, and delete dialogs
- Pagination with 5–100 items per page

### Database Entities

- `Document` (versioning, status, permissions)
- `DocumentVersion`
- `Folder`
- `Library`
- `FolderPermission`
- `DocumentPermission`

---

## 4. AI & Intelligent Services

**Backend Module**: `AFC27.KMS.AI` + `AFC27.KMS.AIWorker`
**Frontend Views**: `AIServicesView`, `SemanticSearchView`
**API Base**: `/api/ai/*`, `/api/search/semantic/*`

### Capabilities

| Capability | Description |
|---|---|
| AI Chat (RAG) | ChatGPT-like conversational interface with Retrieval-Augmented Generation |
| Semantic search | Vector-based similarity search across all indexed content |
| Document embedding | Background worker for text extraction, chunking, and embedding |
| Streaming responses | Server-Sent Events for real-time token-by-token AI output |
| Conversation history | Persistent chat sessions with message history |
| Citation tracking | Link AI responses back to source documents |
| Transcription | Audio/video to text conversion with language selection |
| Summarization | Configurable-length content summarization |
| Q&A | Question answering from provided document context |
| Entity extraction (NER) | Named entity recognition from documents |
| Sentiment analysis | Content sentiment scoring |
| Document classification | AI-powered automatic categorization |
| Content recommendations | AI-suggested related content |
| Quality analysis | AI-driven content quality scoring |
| Quota management | Per-user AI token usage tracking and limits |

### Background Processing (AIWorker)

| Consumer | Purpose | Queue |
|---|---|---|
| `IngestionConsumer` | Document ingestion for AI processing | `ai-ingestion` |
| `EmbeddingConsumer` | Generate embeddings for semantic search | `ai-ingestion` |

Supporting services: `TextExtractionService`, `ChunkingService`, `MockEmbeddingService`

### Frontend Features

- Tab interface for Transcription, Summarization, and Q&A services
- Service cards with quota status and usage indicators
- Job management: recent jobs listing, status tracking, details dialog
- File upload for transcription with language selection (EN/AR)
- Semantic search with relevance scores, result highlighting, and related concepts
- Saved search feature for semantic queries

### Database Entities

- `Conversation`, `Message`
- `DocumentChunk`, `Embedding`
- `DocumentAnalysisEntity`, `ExtractedEntityRecord`
- `BatchAnalysisJobEntity`
- `TranscriptionResultEntity`
- `ContentRecommendationEntity`

---

## 5. Search

**Backend Module**: `AFC27.KMS.Search`
**Frontend Views**: `SearchResultsView`
**API Base**: `/api/search/*`

### Capabilities

| Capability | Description |
|---|---|
| Global search | Unified search across 14 content types |
| Advanced/Boolean search | Complex query syntax support |
| Faceted filtering | Filter by content type, date range, author |
| Sort options | Relevance, newest, oldest, most popular |
| Saved searches | Persist queries with optional notification subscriptions |
| Search suggestions | Autocomplete and typeahead recommendations |
| Trending searches | Popular search terms surfacing |
| Similar content | "More Like This" content discovery |
| Search analytics | Click tracking, search metrics, usage dashboard |
| Search history | Per-user search history tracking |
| Index management | Rebuild and manage search indexes |

### Searchable Content Types

Articles, News, Blogs, Announcements, Pages, Documents, MediaItems, Discussions, Comments, LessonsLearned, Users, Communities, Events, Services

### Frontend Features

- Query input with execution time display
- Faceted filters: content type checkboxes, date range picker
- Sort dropdown (relevance, newest, oldest, popular)
- Result cards with type badge, title highlighting, snippet preview, author, date, view count
- Save search dialog with name, notification toggle, and frequency selector (daily/weekly/monthly)
- Saved searches panel
- Pagination with result count

---

## 6. Collaboration

**Backend Module**: `AFC27.KMS.Collaboration`
**Frontend Views**: `CommunitiesView`, `CommunityDetailView`, `DiscussionView`, `LessonsLearnedView`, `LessonDetailView`
**API Base**: `/api/collaboration/*`

### Capabilities

| Capability | Description |
|---|---|
| Communities | Create and join groups with type classification |
| Community types | General, Project, Department, Working Group |
| Visibility levels | Public, Private, Secret |
| Discussion forums | Threaded discussions with full Q&A support |
| Discussion types | Discussion, Question, Announcement, Poll |
| Lessons learned | Structured knowledge capture and sharing |
| Comments | Nested/threaded comments with @mention support |
| Likes | Content engagement tracking (like/unlike) |
| Follow/unfollow | Subscribe to discussion thread updates |
| Pin/lock | Moderator controls for important or closed discussions |
| Mark as answered | Q&A resolution tracking |
| Trending | Algorithmic hot topic surfacing |
| Unanswered queue | Pending Q&A items awaiting response |

### Frontend Features

- Community type toggle (All / My Communities)
- Filter by type and visibility
- Grid/list view with color-coded community cards
- Member count, discussion count, and recent member avatars on cards
- Community detail page with tabs: Overview, Members, Discussions, Files, Settings
- Discussion thread view with nested replies
- Lessons learned listing and detail views
- Staggered card entrance animations

### Database Entities

- `Community`
- `Discussion` (with type enum)
- `Comment` (threaded/nested)
- `Mention`
- `LessonLearned`
- `Like`

---

## 7. Workflow & Service Catalog

**Backend Module**: `AFC27.KMS.Workflow`
**Frontend View**: `ServiceCatalogView`
**API Base**: `/api/workflow/*`, `/api/services/*`

### Capabilities

| Capability | Description |
|---|---|
| Service catalog | Self-service portal with categorized service offerings |
| Service requests | Submit, track, and manage service requests |
| Multi-level approvals | Configurable approval chains per service |
| SLA tracking | Response and resolution time monitoring with breach alerts |
| Custom forms | Dynamic form definitions per service type |
| Request comments | Internal and external comments with file attachments |
| Status tracking | Full lifecycle: Draft → Submitted → PendingApproval → Approved → InProgress → Completed/Rejected/Cancelled |
| Assignee management | Route requests to appropriate teams or individuals |
| Priority levels | Configurable priority per request |
| Status history | Complete audit trail of status transitions |
| Dashboard & metrics | KPI tracking and service performance analytics |

### Database Entities

- `ServiceCategoryEntity` (hierarchical)
- `CatalogServiceEntity` (SLA config, approval requirements, custom fields, assignees)
- `ServiceRequestEntity` (status, priority, approvals)
- `RequestApprovalEntity`
- `RequestCommentEntity`
- `RequestAttachmentEntity`
- `RequestStatusHistoryEntity`

---

## 8. Task Management

**Frontend View**: `TaskInboxView` with `TaskDetailPanel`

### Capabilities

| Capability | Description |
|---|---|
| Task inbox | Centralized task view with algorithmic smart sorting |
| Task types | Approval, Review, Assignment, Mention, Reminder |
| Priority levels | Urgent, High, Medium, Low (color-coded) |
| Quick actions | Approve/reject, complete, defer — inline without navigation |
| Slide-over detail | View full task details in a side panel |
| Comments & activity | Per-task comments with @mention support |
| Progress tracking | Percentage-based progress bars for multi-step tasks |
| Overdue detection | Automatic overdue flagging with visual indicators |
| Smart sorting | Algorithm: Overdue → Due today → Priority weight → Due date |
| Related items | Navigate to associated documents, articles, events, communities |

### Task Type Color Coding

| Type | Color |
|---|---|
| Approval | Purple (#8b5cf6) |
| Review | Blue (#3b82f6) |
| Assignment | Teal (#00ae8d) |
| Mention | Pink (#ec4899) |
| Reminder | Orange (#f59e0b) |

---

## 9. Calendar & Events

**Backend Module**: `AFC27.KMS.Calendar`
**Frontend View**: `CalendarView`
**API Base**: `/api/calendar/*`, `/api/events/*`

### Capabilities

| Capability | Description |
|---|---|
| Multiple views | Month, Week, Day, Agenda display modes |
| Event CRUD | Create, read, update, delete events |
| Color-coded categories | 6 category colors (blue, green, purple, orange, red, gold) |
| RSVP management | Accepted, Declined, Maybe, Pending statuses |
| Recurring events | Repeating event configuration |
| All-day events | Full-day event toggle |
| Holiday management | Configurable holiday calendar |
| Mini calendar | Sidebar date picker for quick navigation |
| Multi-calendar | Multiple calendar overlay support |
| Event reminders | Notification-based reminder scheduling |
| Meeting links | External meeting URL integration (Teams, Outlook, Google, Zoom) |
| Agenda items | Per-meeting agenda with ordering |
| Action items | Meeting action item tracking |
| Attendee management | Invite and track attendees |

### Database Entities (WebApi)

- `ExternalMeetingLinkEntity`
- `MeetingDocumentLinkEntity`
- `MeetingAttendeeEntity`
- `MeetingAgendaItemEntity`
- `MeetingActionItemEntity`

---

## 10. Notifications

**Backend Module**: `AFC27.KMS.Notifications` + `AFC27.KMS.NotificationWorker`
**Frontend Views**: `NotificationsView`, `NotificationPreferencesView`, `NotificationDropdown`
**API Base**: `/api/notifications/*`

### Capabilities

| Capability | Description |
|---|---|
| Multi-channel delivery | In-App, Email, Push notifications |
| Categories | Workflow, Content, Collaboration, Calendar, System |
| User preferences | Per-channel and per-category toggles |
| Frequency settings | Immediate, Daily digest, Weekly digest |
| Quiet hours | Configurable do-not-disturb periods |
| Templates | Reusable notification templates |
| Bulk operations | Mark all as read, delete all read notifications |
| Priority levels | Urgent, High, Normal, Low |
| Archive | Notification archival |
| Statistics | Unread counts, category counts |
| Header dropdown | Quick-access notification panel in navigation bar |

### Notification Types

`TaskAssigned`, `TaskCompleted`, `TaskDelegated`, `ContentCommented`, `ContentApproved`, `ContentPublished`, `DocumentShared`, `DocumentUploaded`, `MeetingInvited`, `LessonLearned`, `Custom`

### Background Processing (NotificationWorker)

| Consumer | Purpose | Queue |
|---|---|---|
| `NotificationConsumer` | Process and route notifications | `notifications` |
| `EmailConsumer` | Send email notifications | `notifications` |
| `BulkNotificationConsumer` | Distribute bulk notifications | `notifications` |

---

## 11. Media Management

**Backend Module**: `AFC27.KMS.Media` + `AFC27.KMS.MediaWorker`
**Frontend Views**: `MediaGalleryView`, `MediaGalleryDetailView`, `VideoEditorView`
**API Base**: `/api/media/*`

### Capabilities

| Capability | Description |
|---|---|
| Media gallery | Image and video gallery with thumbnail display |
| Image processing | Automatic thumbnail generation and resizing |
| Video transcoding | FFmpeg-based format conversion |
| Video editor | Timeline-based trim/cut, subtitle management, export |
| Gallery management | Create, organize, and share media galleries |
| Metadata display | Per-item metadata and detailed information |
| Upload support | Multi-file upload for images and videos |

### Background Processing (MediaWorker)

| Consumer | Purpose | Queue |
|---|---|---|
| `ThumbnailGenerationConsumer` | Generate image thumbnails | `media-transcoding` |
| `VideoTranscodingConsumer` | Transcode video formats | `media-transcoding` |

---

## 12. Administration

**Backend Module**: `AFC27.KMS.Admin`
**Frontend Views**: `UsersView`, `RolesView`, `GroupsView`
**API Base**: `/api/admin/*`

### Capabilities

| Capability | Description |
|---|---|
| User management | Invite, suspend, reactivate, delete users with lifecycle tracking |
| Role management | RBAC with full permission matrix configuration |
| Group management | User groups with member add/remove |
| Audit logging | Comprehensive, searchable audit trail for all system actions |
| Legal hold | eDiscovery and legal hold placement on documents |
| Document quarantine | Quarantine workflow with admin review and release |
| Impersonation | Admin can act as any user for troubleshooting |
| Bulk operations | Activate, deactivate, delete, or assign roles to multiple users |
| Activity summaries | Per-user activity reports |

### Frontend Features

- User data table with avatar, name, email, role, department, status, last login
- Role/status/department filter toggles
- Search by name or email
- Bulk action toolbar
- User edit dialog with role assignment and status management
- Reset password and disable/enable account functions
- Role listing with permission matrix editor
- Group listing with member management

### Database Entities

- `AuditLogEntry` (entity type, action, timestamp, user, changes JSON)
- `LegalHold`
- `QuarantinedDocument` (quarantine workflow, review status)
- `ImpersonationSession`

---

## 13. Integrations

**Backend Module**: `AFC27.KMS.Integration` + WebApi Integration services
**Frontend View**: `IntegrationDashboardView`
**API Base**: `/api/integration/*`

### External System Connectors

| Integration | Capabilities |
|---|---|
| **Intalio Case (BPM)** | Get/start processes, get/claim/complete tasks, task assignment, process sync |
| **Intalio IAM** | User sync, group sync, role management |
| **Intalio DMS** | Document upload/download, document sync, folder management |
| **Intalio Correspondence** | Get incoming/outgoing correspondence, create correspondence, reference number generation, sync |
| **Microsoft 365** | Teams and Outlook integration with sync |
| **ERP** | Generic ERP connector |
| **Meeting platforms** | Microsoft Teams, Outlook, Google Calendar, Zoom |
| **OCR Service** | Async document OCR with webhook callbacks, synchronous mode with timeout |
| **Digital Signatures** | Signature request, verification, document signing completion |
| **AI Engine** | External AI service with webhook-based job completion |

### Webhook Handlers

| Controller | Purpose |
|---|---|
| `AiEngineWebhookController` | AI job completion callbacks |
| `SignatureWebhookController` | Signature completion callbacks |
| `OcrWebhookController` | OCR job completion callbacks |
| `MeetingWebhookController` | Meeting sync event callbacks |
| `IntegrationHealthController` | External service health monitoring |

### Frontend Features

- Integration connection cards with status indicators (Connected, Disconnected, Error)
- Last sync time and sync activity indicators
- Configure, test connection, and disconnect actions
- Connection logs and health dashboard

---

## 14. Authentication & Authorization

**Backend Module**: `AFC27.KMS.Identity`
**Frontend Views**: `LoginView`, `SSOCallbackView`
**API Base**: `/api/identity/auth`

### Authentication Methods

| Method | Description |
|---|---|
| Azure AD SSO | OpenID Connect via Microsoft Identity Web |
| JWT Bearer tokens | Access + refresh token flow with automatic refresh |
| Development mode | Authentication can be fully disabled for local development |

### Authorization Policies (20+)

**Content Management**: `CanManageContent`, `CanCreateContent`, `CanEditContent`, `CanDeleteContent`, `CanPublishContent`

**Document Management**: `CanManageLibraries`, `CanUploadDocuments`, `CanEditDocuments`, `CanDeleteDocuments`, `CanPublishDocuments`

**Administrative**: `CanManageUsers`, `CanViewUsers`, `AIAdmin`, `IntegrationAdmin`, `NotificationAdmin`

**Workflow**: `CanManageWorkflow`, `CanViewAllTasks`, `CanAssignRequests`, `CanApproveLessons`

**Search & Calendar**: `CanManageSearch`, `CanManageCalendar`

### Frontend Auth Features

- SSO login button with Azure AD redirect
- Email/password login form with remember-me
- Automatic token refresh on 401 responses
- Role-based route guards
- Permission-based UI element rendering
- Multi-device session management with logout controls

### Database Entities

- `User` (Email, DisplayName, DisplayNameArabic, Department, Manager, IsActive)
- `Department` (hierarchical)
- `Role`, `Group`
- `UserRole`, `GroupMember` (many-to-many)
- `Permission`, `RolePermission` (many-to-many)

---

## 15. Infrastructure & Architecture

### Message Queue System

**Framework**: MassTransit with RabbitMQ (production) / In-Memory (development)

| Queue | Purpose | Prefetch | Retry Strategy |
|---|---|---|---|
| `document-processing` | Document ingestion | 16 | 5s, 15s |
| `ai-ingestion` | AI processing | 8 | 10s, 30s |
| `media-transcoding` | Video/image processing | 4 | 30s, 1m |
| `notifications` | Notification delivery | 32 | 5s |

**Resilience**: Automatic retries with exponential backoff, circuit breaker (15 trip / 10 active threshold), dead-letter queue handling.

### Caching

| Environment | Implementation |
|---|---|
| Production | Redis (`StackExchange.Redis`, prefix: `AFC27KMS:`) |
| Development | `Microsoft.Extensions.Caching.Memory` |

Cached data: document metadata, search results, embeddings, sessions.

### Database

- **ORM**: Entity Framework Core 8.0 with Code-First migrations
- **Production**: SQL Server
- **Development**: SQLite
- **Features**: Soft delete (global query filter), auditable entities (CreatedAt/ModifiedAt/CreatedBy/ModifiedBy), domain event dispatching interceptor

### Storage

- Local file system storage via `IStorageService` abstraction
- Multipart upload handler for large files
- Configurable base path (`/storage/documents`) and max file size (100MB)

### Logging & Monitoring

- **Serilog** structured logging with Elasticsearch sink
- Enrichment: machine name, environment name, log context
- Health checks: `/health/live` (liveness), `/health/ready` (readiness)

### API Standards

- RESTful endpoints with Swagger/OpenAPI documentation
- Standard response wrapper: `ApiResponse<T>` with `Success`, `Data`, `Message`, `Errors`
- Pagination wrapper: `PagedResult<T>` with `Items`, `TotalCount`, `PageNumber`, `PageSize`, `TotalPages`
- FluentValidation for input validation
- Global exception middleware with HTTP status code mapping
- CORS configured for `localhost:3000` and `localhost:5173`

---

## 16. Frontend Design System & UX

### Technology

- **Framework**: Vue.js 3 with Composition API + TypeScript
- **UI Library**: PrimeVue 4
- **State Management**: Pinia with localStorage/sessionStorage persistence
- **Internationalization**: Vue I18n with full English and Arabic (RTL) support
- **Build Tool**: Vite
- **Styling**: SCSS with custom design system

### Design Tokens

| Token Category | Details |
|---|---|
| Primary color | Intalio Teal #00ae8d (50–900 spectrum) |
| Neutrals | Slate Gray #f8fafc to #020617 |
| Semantic colors | Success (#10b981), Warning (#f59e0b), Error (#ef4444), Info (#3b82f6) |
| Typography | Inter (EN), Noto Sans Arabic (AR), sizes xs (12px) to 4xl (36px) |
| Spacing | 4px-based grid system (spacing-1 to spacing-20) |
| Shadows | 9-level system (xs to 2xl) + elevated + colored variants |
| Border radius | 2px to 9999px |
| Breakpoints | 640px, 768px, 1024px, 1280px, 1536px |
| Transitions | Fast (150ms), Base (200ms), Moderate (300ms), Slow (400ms) |

### Themes

- **Intalio Theme**: Primary custom theme
- **AFC27 Theme**: Tournament-specific variant

### UI Patterns

| Pattern | Implementation |
|---|---|
| Page headers | Gradient backgrounds with breadcrumbs, title, description, actions |
| Stats bars | Icon-based metrics with trend indicators |
| Content cards | Hover lift effects, selection states, semantic color coding |
| Skeleton loaders | Per-component loading placeholders (card, avatar, text, image) |
| Empty states | Icon + message + suggestion + action button |
| Error states | Error icon, message, retry button |
| Slide-over panels | Side panels for detail views without navigation |
| Animations | Staggered entrance, spring transitions, scroll reveal, ripple effects |
| Responsive design | Mobile-first with 5 breakpoints, collapsible sidebar |
| Accessibility | Reduced motion support, focus states, ARIA labels, keyboard navigation |

### Composables

| Composable | Purpose |
|---|---|
| `useDirection` | RTL/LTR direction helper |
| `useReducedMotion` | Respects `prefers-reduced-motion` |
| `useCountUp` | Animated number counter |
| `useRipple` | Material Design ripple effect |
| `useScrollReveal` | Intersection observer scroll reveal |

### State Management

**Auth Store** (`auth.store.ts`): User state, tokens, login/logout/SSO, role and permission checks. Persisted to `sessionStorage`.

**UI Store** (`ui.store.ts`): Locale (en/ar), theme (light/dark), sidebar state. Persisted to `localStorage`.

### API Client

- Axios-based with request/response interceptors
- Automatic auth token injection and language header
- 401 response handling with token refresh
- Mock data fallback when backend is unavailable

---

## 17. Summary Metrics

| Metric | Count |
|---|---|
| Frontend pages/views | 32+ |
| Reusable components | 28+ custom + PrimeVue library |
| Named routes | 40+ |
| API endpoints | 150+ |
| Database entities | 40+ |
| Feature modules | 11 |
| Background workers | 3 |
| Shared libraries | 3 |
| External integrations | 10+ |
| Authorization policies | 20+ |
| Supported languages | 2 (English, Arabic with RTL) |
| Searchable content types | 14 |
| Notification categories | 5 |
| Notification channels | 3 (In-App, Email, Push) |
| Message queues | 4 |
| Design tokens | 100+ |
| SCSS mixins | 85KB+ |
| Animation types | 15+ |
| Responsive breakpoints | 5 |

---

## Additional WebApi Features

These features are implemented directly in the WebApi host rather than in separate modules:

| Feature | Description |
|---|---|
| AI Analysis | Document analysis, batch analysis jobs, entity extraction, sentiment analysis |
| Barcodes | Barcode and QR code generation |
| KPI Management | KPI tracking, metrics, and dashboard statistics |
| Learning | Learning management functions |
| Meetings | External meeting links (Teams, Outlook, Google, Zoom), agenda items, action items, attendees, calendar integration |
| Polling | Poll creation, voting, results aggregation |
| Quality Assurance | QA tracking and quality metrics |
| Templates | Document template management with versioning and template libraries |
