# AFC Asian Cup 2027 - Knowledge Management System
## Development Progress Report

**Project**: Enterprise Knowledge Management System for LOC AFC27
**Target Users**: 10,000+
**Languages**: English & Arabic (RTL/LTR)
**Last Updated**: December 10, 2024

---

## Technology Stack

| Layer | Technology | Status |
|-------|------------|--------|
| Frontend | Vue.js 3 + PrimeVue + Pinia | In Progress |
| Backend | .NET 8 (C#) Modular Monolith | In Progress |
| Database | SQL Server + Redis | Planned |
| Search | Elasticsearch | Planned |
| Message Queue | RabbitMQ + MassTransit | Planned |
| AI | Intalio AI Engine | Planned |
| Deployment | On-Premises (Kubernetes) | Planned |

---

## Project Structure

### Backend Structure
```
backend/
├── src/
│   ├── AFC27.KMS.ApiGateway/              [Created] YARP API Gateway
│   ├── AFC27.KMS.WebApi/                  [Created] Main API Host
│   ├── Modules/
│   │   ├── AFC27.KMS.Identity/            [Created] Auth, Users, Roles
│   │   ├── AFC27.KMS.Content/             [Created] Articles, News, Blogs
│   │   ├── AFC27.KMS.Documents/           [Created] Document Library
│   │   ├── AFC27.KMS.Collaboration/       [Created] Communities, Forums
│   │   ├── AFC27.KMS.Search/              [Created] Elasticsearch
│   │   ├── AFC27.KMS.Media/               [Created] Image/Video
│   │   ├── AFC27.KMS.Workflow/            [Created] Service Catalog, Forms
│   │   ├── AFC27.KMS.Calendar/            [Created] Events, RSVP, TimeOff
│   │   ├── AFC27.KMS.Notifications/       [Created] Email, Push, In-App
│   │   ├── AFC27.KMS.AI/                  [Created] Intalio AI Integration
│   │   └── AFC27.KMS.Integration/         [Created] Intalio Suite, M365, ERP
│   ├── Shared/
│   │   ├── AFC27.KMS.SharedKernel/        [Created] Base entities
│   │   ├── AFC27.KMS.Contracts/           [Created] Shared DTOs
│   │   └── AFC27.KMS.Infrastructure/      [Created] Cross-cutting
│   └── Workers/
│       ├── AFC27.KMS.MediaWorker/         [Scaffolded]
│       ├── AFC27.KMS.AIWorker/            [Scaffolded]
│       └── AFC27.KMS.NotificationWorker/  [Scaffolded]
├── tests/
└── docker/
    └── docker-compose.yml                 [Created]
```

### Frontend Structure
```
frontend/
├── src/
│   ├── assets/
│   │   └── styles/
│   │       └── main.scss                  [Created]
│   ├── components/
│   │   └── common/                        [Created] Shared components
│   ├── composables/
│   │   └── useDirection.ts                [Created] RTL/LTR support
│   ├── locales/
│   │   ├── en.json                        [Created] English translations
│   │   └── ar.json                        [Created] Arabic translations
│   ├── plugins/
│   │   ├── primevue.ts                    [Created] PrimeVue config
│   │   └── i18n.ts                        [Created] Vue-i18n config
│   ├── router/
│   │   └── index.ts                       [Created] Vue Router
│   ├── services/
│   │   └── api.ts                         [Created] Axios client
│   ├── stores/
│   │   └── auth.store.ts                  [Created] Pinia auth store
│   └── views/
│       ├── auth/
│       │   ├── LoginView.vue              [Created]
│       │   └── SSOCallbackView.vue        [Created]
│       ├── dashboard/
│       │   └── DashboardView.vue          [Created]
│       ├── content/
│       │   ├── ContentListView.vue        [Created]
│       │   ├── ContentDetailView.vue      [Scaffolded]
│       │   └── ContentEditorView.vue      [Scaffolded]
│       ├── documents/
│       │   └── DocumentLibraryView.vue    [Created]
│       ├── collaboration/
│       │   ├── CommunitiesView.vue        [Created]
│       │   ├── CommunityDetailView.vue    [Created]
│       │   ├── DiscussionView.vue         [Created]
│       │   ├── LessonsLearnedView.vue     [Created]
│       │   └── LessonDetailView.vue       [Created]
│       ├── search/
│       │   └── SearchResultsView.vue      [Created]
│       ├── workflow/
│       │   ├── ServiceCatalogView.vue     [Created]
│       │   └── TaskInboxView.vue          [Created]
│       ├── profile/
│       │   └── ProfileView.vue            [Created]
│       ├── calendar/
│       │   └── CalendarView.vue           [Created]
│       ├── notifications/
│       │   ├── NotificationsView.vue      [Created]
│       │   └── NotificationPreferencesView.vue [Created]
│       ├── ai/
│       │   ├── AIServicesView.vue         [Created]
│       │   └── SemanticSearchView.vue     [Created]
│       ├── integration/
│       │   └── IntegrationDashboardView.vue [Created]
│       ├── admin/
│       │   ├── AdminLayout.vue            [Scaffolded]
│       │   ├── UsersView.vue              [Scaffolded]
│       │   ├── RolesView.vue              [Scaffolded]
│       │   └── GroupsView.vue             [Scaffolded]
│       ├── layouts/
│       │   └── MainLayout.vue             [Created]
│       └── errors/
│           └── NotFoundView.vue           [Created]
```

---

## Module Implementation Status

### 1. SharedKernel [COMPLETE]
Base infrastructure for all modules.

**Files:**
- `BaseEntity.cs` - Base entity with Id, domain events
- `AuditableEntity.cs` - Adds CreatedAt, CreatedBy, ModifiedAt, ModifiedBy
- `LocalizedString.cs` - Value object for bilingual text (English + Arabic)
- `IDomainEvent.cs` - Domain event interface
- `IRepository.cs` - Generic repository interface

### 2. Contracts [COMPLETE]
Shared DTOs and common types.

**Files:**
- `ApiResponse.cs` - Standard API response wrapper
- `PagedResult.cs` - Pagination support
- `Common/` - Shared enums and constants

### 3. Infrastructure [COMPLETE]
Cross-cutting concerns.

**Files:**
- `Persistence/` - EF Core configuration (scaffolded)
- `Caching/` - Redis caching (scaffolded)
- `Messaging/` - RabbitMQ/MassTransit (scaffolded)

### 4. Identity Module [COMPLETE]
User authentication, authorization, and directory.

**Domain Entities:**
- `User` - User profile with bilingual fields
- `Role` - Role with permissions
- `Permission` - Granular permission
- `Department` - Organizational structure
- `Group` - User groups

**DTOs:**
- `UserDto`, `UserSummaryDto`, `CreateUserRequest`, `UpdateUserRequest`
- `RoleDto`, `CreateRoleRequest`
- `DepartmentDto`, `CreateDepartmentRequest`
- `GroupDto`, `CreateGroupRequest`

**Controllers:**
- `UsersController` - CRUD, activate/deactivate, role assignment
- `RolesController` - CRUD, permission management
- `DepartmentsController` - CRUD, org chart
- `GroupsController` - CRUD, member management
- `AuthController` - Login, SSO, token refresh

**API Endpoints:**
```
POST   /api/identity/auth/login
POST   /api/identity/auth/sso
POST   /api/identity/auth/refresh
GET    /api/identity/users
GET    /api/identity/users/{id}
POST   /api/identity/users
PUT    /api/identity/users/{id}
POST   /api/identity/users/{id}/activate
POST   /api/identity/users/{id}/deactivate
GET    /api/identity/roles
POST   /api/identity/roles
GET    /api/identity/departments
GET    /api/identity/departments/org-chart
GET    /api/identity/groups
POST   /api/identity/groups/{id}/members
```

### 5. Content Module [COMPLETE]
Articles, news, blogs, and content management.

**Domain Entities:**
- `Article` - Main content entity with versioning
- `ArticleVersion` - Version history
- `Category` - Content categories
- `Tag` - Content tags
- `ContentType` enum - Article, News, Blog, Announcement, Page

**DTOs:**
- `ArticleDto`, `ArticleSummaryDto`, `CreateArticleRequest`, `UpdateArticleRequest`
- `CategoryDto`, `CreateCategoryRequest`
- `TagDto`

**Controllers:**
- `ArticlesController` - Full CRUD, publish workflow, versioning
- `CategoriesController` - CRUD for categories
- `TagsController` - Tag management

**API Endpoints:**
```
GET    /api/content/articles
GET    /api/content/articles/{id}
POST   /api/content/articles
PUT    /api/content/articles/{id}
DELETE /api/content/articles/{id}
POST   /api/content/articles/{id}/publish
POST   /api/content/articles/{id}/unpublish
POST   /api/content/articles/{id}/archive
GET    /api/content/articles/{id}/versions
GET    /api/content/categories
POST   /api/content/categories
GET    /api/content/tags
```

### 6. Documents Module [COMPLETE]
Document library with versioning and metadata.

**Domain Entities:**
- `Document` - Document with metadata
- `DocumentVersion` - Version history
- `DocumentLibrary` - Library container
- `Folder` - Folder structure
- `DocumentMetadata` - Custom metadata

**DTOs:**
- `DocumentDto`, `DocumentSummaryDto`, `UploadDocumentRequest`
- `DocumentLibraryDto`, `CreateLibraryRequest`
- `FolderDto`, `CreateFolderRequest`
- `DocumentVersionDto`

**Controllers:**
- `DocumentsController` - CRUD, upload, download, versioning
- `LibrariesController` - Library management
- `FoldersController` - Folder management

**API Endpoints:**
```
GET    /api/documents
GET    /api/documents/{id}
POST   /api/documents/upload
PUT    /api/documents/{id}
DELETE /api/documents/{id}
GET    /api/documents/{id}/download
POST   /api/documents/{id}/checkout
POST   /api/documents/{id}/checkin
GET    /api/documents/{id}/versions
GET    /api/documents/libraries
POST   /api/documents/libraries
GET    /api/documents/folders
POST   /api/documents/folders
```

### 7. Collaboration Module [COMPLETE]
Communities, forums, comments, and lessons learned.

**Domain Entities:**
- `Community` - Community workspace
- `CommunityMember` - Membership with roles
- `Discussion` - Forum discussion
- `DiscussionTag` - Discussion tags
- `DiscussionReaction` - Likes/reactions
- `Comment` - Comments on any entity
- `CommentReaction` - Comment likes
- `Mention` - @mentions
- `Follow` - Follow relationships
- `LessonLearned` - Knowledge capture

**DTOs:**
- `CommunityDto`, `CommunitySummaryDto`, `CreateCommunityRequest`
- `CommunityMemberDto`, `CommunityFilterRequest`
- `DiscussionDto`, `DiscussionSummaryDto`, `CreateDiscussionRequest`
- `PollOptionDto`, `PollDiscussionDto`
- `CommentDto`, `CreateCommentRequest`, `UpdateCommentRequest`
- `MentionDto`, `MentionRequest`
- `FollowDto`, `FollowRequest`
- `LessonLearnedDto`, `LessonLearnedSummaryDto`, `CreateLessonLearnedRequest`

**Controllers:**
- `CommunitiesController` - CRUD, join/leave, member management
- `DiscussionsController` - CRUD, like, follow, pin, lock
- `CommentsController` - CRUD, like, accept as answer
- `FollowsController` - Follow/unfollow, notifications
- `LessonsLearnedController` - CRUD, approval workflow, useful marking

**API Endpoints:**
```
GET    /api/collaboration/communities
GET    /api/collaboration/communities/{id}
POST   /api/collaboration/communities
PUT    /api/collaboration/communities/{id}
DELETE /api/collaboration/communities/{id}
POST   /api/collaboration/communities/{id}/join
POST   /api/collaboration/communities/{id}/leave
GET    /api/collaboration/communities/{id}/members
GET    /api/collaboration/communities/{id}/discussions

GET    /api/collaboration/discussions
GET    /api/collaboration/discussions/{id}
POST   /api/collaboration/discussions
PUT    /api/collaboration/discussions/{id}
DELETE /api/collaboration/discussions/{id}
POST   /api/collaboration/discussions/{id}/like
DELETE /api/collaboration/discussions/{id}/like
POST   /api/collaboration/discussions/{id}/follow
DELETE /api/collaboration/discussions/{id}/follow
POST   /api/collaboration/discussions/{id}/pin
POST   /api/collaboration/discussions/{id}/lock
POST   /api/collaboration/discussions/{id}/mark-answered

GET    /api/collaboration/comments
GET    /api/collaboration/comments/{id}
POST   /api/collaboration/comments
PUT    /api/collaboration/comments/{id}
DELETE /api/collaboration/comments/{id}
POST   /api/collaboration/comments/{id}/like
DELETE /api/collaboration/comments/{id}/like
POST   /api/collaboration/comments/{id}/accept-answer
GET    /api/collaboration/comments/{id}/replies

GET    /api/collaboration/follows
POST   /api/collaboration/follows
DELETE /api/collaboration/follows
PUT    /api/collaboration/follows/{id}/notifications
GET    /api/collaboration/follows/followers

GET    /api/collaboration/lessons-learned
GET    /api/collaboration/lessons-learned/{id}
POST   /api/collaboration/lessons-learned
PUT    /api/collaboration/lessons-learned/{id}
DELETE /api/collaboration/lessons-learned/{id}
POST   /api/collaboration/lessons-learned/{id}/submit
POST   /api/collaboration/lessons-learned/{id}/approve
POST   /api/collaboration/lessons-learned/{id}/reject
POST   /api/collaboration/lessons-learned/{id}/publish
POST   /api/collaboration/lessons-learned/{id}/useful
DELETE /api/collaboration/lessons-learned/{id}/useful
GET    /api/collaboration/lessons-learned/{id}/related
GET    /api/collaboration/lessons-learned/categories
```

---

## Frontend Implementation Status

### Core Infrastructure [COMPLETE]
- Vue.js 3 with Composition API
- PrimeVue component library with custom AFC27 theme
- Pinia state management
- Vue Router with auth guards
- Vue-i18n for internationalization
- Axios API client
- RTL/LTR support via useDirection composable

### Views Implemented

| View | Route | Status |
|------|-------|--------|
| Login | `/login` | Complete |
| SSO Callback | `/sso-callback` | Complete |
| Dashboard | `/` | Complete |
| Content List | `/content` | Complete |
| Content Create | `/content/create` | Scaffolded |
| Content Detail | `/content/:id` | Scaffolded |
| Documents | `/documents` | Complete |
| Search | `/search` | Complete |
| Profile | `/profile` | Complete |
| Communities | `/communities` | Complete |
| Community Detail | `/communities/:id` | Complete |
| Discussion Detail | `/discussions/:id` | Complete |
| Lessons Learned | `/lessons-learned` | Complete |
| Lesson Detail | `/lessons-learned/:id` | Complete |
| Media Gallery | `/media` | Complete |
| Gallery Detail | `/media/gallery/:id` | Complete |
| Video Editor | `/media/video-editor` | Complete |
| Service Catalog | `/workflow/services` | Complete |
| Task Inbox | `/workflow/tasks` | Complete |
| Calendar | `/calendar` | Complete |
| Admin Users | `/admin/users` | Scaffolded |
| Admin Roles | `/admin/roles` | Scaffolded |
| Admin Groups | `/admin/groups` | Scaffolded |
| 404 Not Found | `/*` | Complete |

### Translations
- English (`en.json`) - Complete for implemented modules
- Arabic (`ar.json`) - Complete for implemented modules

---

## 8. Media Module [COMPLETE]
Image and video gallery with editing capabilities.

**Domain Entities:**
- `MediaGallery` - Gallery container with type, visibility
- `GalleryTag` - Gallery categorization
- `MediaItem` - Individual media file
- `MediaTag` - Media item tags
- `MediaThumbnail` - Generated thumbnail variants
- `MediaTranscoding` - Video transcoding jobs
- `VideoEditJob` - Video editing operations

**DTOs:**
- `MediaGalleryDto`, `MediaGallerySummaryDto`, `CreateGalleryRequest`
- `MediaItemDto`, `MediaItemSummaryDto`, `UploadMediaRequest`
- `ThumbnailDto`, `TranscodingDto`, `MediaMetadataDto`
- `VideoEditJobDto`, `TrimVideoRequest`, `AddOverlaysRequest`
- `AddWatermarkRequest`, `ConvertVideoRequest`, `ExtractAudioRequest`
- `MediaStatisticsDto`, `StorageByTypeDto`

**Controllers:**
- `GalleriesController` - CRUD, cover image, archive/restore
- `MediaItemsController` - CRUD, upload, download, stream, transcode
- `VideoEditorController` - Trim, merge, overlay, watermark, convert
- `MediaStatisticsController` - Usage statistics

**API Endpoints:**
```
GET    /api/media/galleries
GET    /api/media/galleries/{id}
POST   /api/media/galleries
PUT    /api/media/galleries/{id}
DELETE /api/media/galleries/{id}
POST   /api/media/galleries/{id}/cover
POST   /api/media/galleries/{id}/archive
POST   /api/media/galleries/{id}/restore
GET    /api/media/galleries/{id}/items
GET    /api/media/galleries/types

GET    /api/media/items
GET    /api/media/items/{id}
POST   /api/media/items/upload
POST   /api/media/items/upload/bulk
PUT    /api/media/items/{id}
DELETE /api/media/items/{id}
GET    /api/media/items/{id}/download
GET    /api/media/items/{id}/stream
POST   /api/media/items/move
POST   /api/media/items/reorder
POST   /api/media/items/{id}/feature
POST   /api/media/items/{id}/thumbnails
POST   /api/media/items/{id}/transcode

GET    /api/media/video-editor/jobs
GET    /api/media/video-editor/jobs/{id}
POST   /api/media/video-editor/trim
POST   /api/media/video-editor/merge
POST   /api/media/video-editor/overlay
POST   /api/media/video-editor/watermark
POST   /api/media/video-editor/convert
POST   /api/media/video-editor/extract-audio
POST   /api/media/video-editor/jobs/{id}/cancel
POST   /api/media/video-editor/jobs/{id}/retry
GET    /api/media/video-editor/jobs/{id}/progress
GET    /api/media/video-editor/quality-presets
GET    /api/media/video-editor/formats
GET    /api/media/video-editor/watermark-positions

GET    /api/media/statistics
GET    /api/media/statistics/by-user
```

---

## 9. Search Module [COMPLETE]
Elasticsearch integration for full-text search.

**Domain Entities:**
- `SearchDocument` - Searchable document with bilingual content
- `SearchIndex` - Elasticsearch index configuration
- `SavedSearch` - User saved search queries
- `SearchQuery` - Search query analytics tracking
- `SearchTermStats` - Aggregated search term statistics
- `SearchSuggestion` - Autocomplete suggestions
- `DailySearchStats` - Daily search analytics aggregate

**DTOs:**
- `SearchRequest`, `AdvancedSearchRequest` - Search request DTOs
- `SearchResponse`, `SearchResultDto` - Search response DTOs
- `SearchFacets`, `FacetItem`, `DateRangeFacet` - Facet DTOs
- `SuggestRequest`, `SuggestResponse`, `SuggestionItem` - Suggestion DTOs
- `SavedSearchDto`, `SaveSearchRequest` - Saved search DTOs
- `IndexDocumentRequest`, `ReindexRequest` - Indexing DTOs
- `SearchAnalyticsDto`, `TopQueryDto`, `ContentTypeStatsDto` - Analytics DTOs

**Controllers:**
- `SearchController` - Global search, advanced search, similar content
- `SuggestionsController` - Autocomplete, spelling, entity suggestions
- `SavedSearchesController` - CRUD for saved searches
- `SearchIndexController` - Index management, reindexing (admin)
- `SearchAnalyticsController` - Search analytics dashboard (admin)

**API Endpoints:**
```
GET    /api/search
POST   /api/search/advanced
GET    /api/search/type/{contentType}
GET    /api/search/similar/{documentId}
POST   /api/search/click
GET    /api/search/facets
GET    /api/search/content-types
GET    /api/search/history
DELETE /api/search/history
GET    /api/search/trending

GET    /api/search/suggest
GET    /api/search/suggest/popular
GET    /api/search/suggest/entities
GET    /api/search/suggest/tags
GET    /api/search/suggest/categories
GET    /api/search/suggest/people
GET    /api/search/suggest/spelling
POST   /api/search/suggest/curated
DELETE /api/search/suggest/curated/{id}

GET    /api/search/saved
GET    /api/search/saved/{id}
POST   /api/search/saved
PUT    /api/search/saved/{id}
DELETE /api/search/saved/{id}
POST   /api/search/saved/{id}/execute
POST   /api/search/saved/{id}/notifications
GET    /api/search/saved/new-results

GET    /api/search/admin/indices
GET    /api/search/admin/indices/{name}
POST   /api/search/admin/indices/documents
POST   /api/search/admin/indices/documents/bulk
DELETE /api/search/admin/indices/documents/{contentType}/{sourceId}
POST   /api/search/admin/indices/reindex
GET    /api/search/admin/indices/reindex/{jobId}
POST   /api/search/admin/indices/reindex/{jobId}/cancel
GET    /api/search/admin/indices/reindex/active
POST   /api/search/admin/indices/{name}/optimize
POST   /api/search/admin/indices/{name}/refresh
PUT    /api/search/admin/indices/{name}/settings
GET    /api/search/admin/indices/health

GET    /api/search/admin/analytics
GET    /api/search/admin/analytics/top-queries
GET    /api/search/admin/analytics/zero-results
GET    /api/search/admin/analytics/trend
GET    /api/search/admin/analytics/by-content-type
GET    /api/search/admin/analytics/ctr
GET    /api/search/admin/analytics/performance
GET    /api/search/admin/analytics/user-behavior
GET    /api/search/admin/analytics/daily/{date}
GET    /api/search/admin/analytics/export
```

---

## 10. Workflow Module [COMPLETE]
Service catalog, dynamic forms, and approval workflows.

**Domain Entities:**
- `ServiceCategory` - Service catalog categories
- `Service` - Service definitions with SLA configuration
- `ServiceRequest` - User service requests
- `RequestComment` - Comments on requests
- `RequestAttachment` - Request attachments
- `RequestActivity` - Request activity log
- `WorkflowDefinition` - Workflow templates
- `WorkflowStepDefinition` - Workflow step definitions
- `StepOutcome` - Step outcome definitions
- `WorkflowTask` - Task assignments
- `Form` - Dynamic form definitions
- `FormSection` - Form sections
- `FormField` - Form fields (30+ types)
- `FieldOption` - Field options for select/radio/checkbox
- `FormSubmission` - Form submissions
- `FormAttachment` - Form file attachments

**DTOs:**
- `ServiceCategoryDto`, `ServiceCategorySummaryDto`, `CreateServiceCategoryRequest`
- `ServiceDto`, `ServiceSummaryDto`, `CreateServiceRequest`
- `ServiceRequestDto`, `ServiceRequestSummaryDto`, `CreateServiceRequestCommand`
- `RequestCommentDto`, `AddCommentRequest`, `RequestActivityDto`
- `WorkflowDefinitionDto`, `CreateWorkflowRequest`, `WorkflowStepDto`, `CreateWorkflowStepRequest`
- `WorkflowTaskDto`, `TaskSummaryDto`, `TaskActionRequest`, `DelegateTaskRequest`
- `FormDto`, `FormSummaryDto`, `CreateFormRequest`
- `FormSectionDto`, `CreateFormSectionRequest`, `FormFieldDto`, `CreateFormFieldRequest`
- `FormSubmissionDto`, `SubmitFormRequest`, `FormAttachmentDto`, `FieldTypeInfoDto`

**Controllers:**
- `ServiceCatalogController` - Categories and services CRUD, featured services
- `ServiceRequestsController` - Request CRUD, cancel, comments, attachments
- `TasksController` - Task inbox, approve/reject, delegate, reassign, bulk actions
- `WorkflowsController` - Workflow definition management, publish/unpublish
- `FormsController` - Dynamic form builder, sections, fields, submissions

**API Endpoints:**
```
GET    /api/workflow/catalog/categories
GET    /api/workflow/catalog/categories/{id}
POST   /api/workflow/catalog/categories
PUT    /api/workflow/catalog/categories/{id}
DELETE /api/workflow/catalog/categories/{id}
POST   /api/workflow/catalog/categories/reorder
GET    /api/workflow/catalog/services
GET    /api/workflow/catalog/services/{id}
POST   /api/workflow/catalog/services
PUT    /api/workflow/catalog/services/{id}
DELETE /api/workflow/catalog/services/{id}
POST   /api/workflow/catalog/services/{id}/publish
POST   /api/workflow/catalog/services/{id}/unpublish
GET    /api/workflow/catalog/services/featured
GET    /api/workflow/catalog/services/recent

GET    /api/workflow/requests
GET    /api/workflow/requests/{id}
POST   /api/workflow/requests
PUT    /api/workflow/requests/{id}
POST   /api/workflow/requests/{id}/cancel
GET    /api/workflow/requests/{id}/history
GET    /api/workflow/requests/{id}/comments
POST   /api/workflow/requests/{id}/comments
GET    /api/workflow/requests/{id}/attachments
POST   /api/workflow/requests/{id}/attachments
DELETE /api/workflow/requests/{id}/attachments/{attachmentId}
GET    /api/workflow/requests/my
GET    /api/workflow/requests/statistics

GET    /api/workflow/tasks
GET    /api/workflow/tasks/{id}
POST   /api/workflow/tasks/{id}/approve
POST   /api/workflow/tasks/{id}/reject
POST   /api/workflow/tasks/{id}/delegate
POST   /api/workflow/tasks/{id}/return
POST   /api/workflow/tasks/{id}/claim
POST   /api/workflow/tasks/{id}/unclaim
POST   /api/workflow/tasks/{id}/reassign
POST   /api/workflow/tasks/{id}/complete
GET    /api/workflow/tasks/{id}/history
GET    /api/workflow/tasks/my
GET    /api/workflow/tasks/delegated
GET    /api/workflow/tasks/team
POST   /api/workflow/tasks/bulk/approve
POST   /api/workflow/tasks/bulk/reject
POST   /api/workflow/tasks/bulk/reassign
GET    /api/workflow/tasks/statistics
GET    /api/workflow/tasks/overdue
GET    /api/workflow/tasks/due-today

GET    /api/workflow/definitions
GET    /api/workflow/definitions/{id}
POST   /api/workflow/definitions
PUT    /api/workflow/definitions/{id}
DELETE /api/workflow/definitions/{id}
POST   /api/workflow/definitions/{id}/steps
PUT    /api/workflow/definitions/{id}/steps/{stepId}
DELETE /api/workflow/definitions/{id}/steps/{stepId}
POST   /api/workflow/definitions/{id}/steps/reorder
POST   /api/workflow/definitions/{id}/publish
POST   /api/workflow/definitions/{id}/unpublish
POST   /api/workflow/definitions/{id}/archive
POST   /api/workflow/definitions/{id}/clone
POST   /api/workflow/definitions/{id}/set-default
GET    /api/workflow/definitions/types
GET    /api/workflow/definitions/step-types
GET    /api/workflow/definitions/assignment-types

GET    /api/workflow/forms
GET    /api/workflow/forms/{id}
POST   /api/workflow/forms
PUT    /api/workflow/forms/{id}
DELETE /api/workflow/forms/{id}
POST   /api/workflow/forms/{id}/sections
PUT    /api/workflow/forms/{id}/sections/{sectionId}
DELETE /api/workflow/forms/{id}/sections/{sectionId}
POST   /api/workflow/forms/{id}/fields
PUT    /api/workflow/forms/{id}/fields/{fieldId}
DELETE /api/workflow/forms/{id}/fields/{fieldId}
POST   /api/workflow/forms/{id}/fields/reorder
POST   /api/workflow/forms/{id}/publish
POST   /api/workflow/forms/{id}/unpublish
POST   /api/workflow/forms/{id}/clone
POST   /api/workflow/forms/{id}/submit
GET    /api/workflow/forms/{id}/submissions
GET    /api/workflow/forms/{id}/submissions/{submissionId}
PUT    /api/workflow/forms/{id}/submissions/{submissionId}
POST   /api/workflow/forms/{id}/submissions/{submissionId}/files
GET    /api/workflow/forms/field-types
GET    /api/workflow/forms/types
```

---

## 11. Calendar Module [COMPLETE]
Events, scheduling, holidays, and time off management.

**Domain Entities:**
- `Calendar` - Calendar container with sharing settings
- `CalendarShare` - Calendar sharing configuration
- `Event` - Event with recurrence, attendees, reminders
- `EventAttendee` - Attendee with RSVP status
- `EventReminder` - Event reminders
- `RecurrenceRule` - RFC 5545 compatible recurrence rules
- `Holiday` - Organization-wide holidays
- `TimeOff` - Time off / Out of office requests
- `WorkingHours` - User working hours configuration

**DTOs:**
- `CalendarDto`, `CalendarSummaryDto`, `CreateCalendarRequest`
- `CalendarShareDto`, `ShareCalendarRequest`, `CalendarSettingsDto`
- `EventDto`, `EventSummaryDto`, `CreateEventRequest`, `UpdateEventRequest`
- `RecurrenceRuleDto`, `CreateRecurrenceRequest`, `ReminderDto`
- `AttendeeDto`, `AddAttendeeRequest`, `RsvpRequest`
- `FreeBusySlotDto`, `FreeBusyRequest`, `FreeBusyResponseDto`
- `MeetingTimeSuggestionDto`, `FindMeetingTimesRequest`
- `HolidayDto`, `CreateHolidayRequest`, `TimeOffDto`, `CreateTimeOffRequest`
- `WorkingHoursDto`, `UpdateWorkingHoursRequest`
- `CalendarExportRequest`, `CalendarImportRequest`, `ImportResultDto`
- `CalendarStatisticsDto`, `AgendaItemDto`

**Controllers:**
- `CalendarsController` - Calendar CRUD, sharing, sync, import/export
- `EventsController` - Event CRUD, RSVP, attendees, reminders, free/busy
- `HolidaysController` - Holiday management
- `TimeOffController` - Time off requests and approvals
- `WorkingHoursController` - Working hours configuration

**API Endpoints:**
```
GET    /api/calendar/calendars
GET    /api/calendar/calendars/{id}
POST   /api/calendar/calendars
PUT    /api/calendar/calendars/{id}
DELETE /api/calendar/calendars/{id}
POST   /api/calendar/calendars/{id}/set-default
GET    /api/calendar/calendars/{id}/shares
POST   /api/calendar/calendars/{id}/shares
PUT    /api/calendar/calendars/{id}/shares/{shareId}
DELETE /api/calendar/calendars/{id}/shares/{shareId}
POST   /api/calendar/calendars/{id}/sync
DELETE /api/calendar/calendars/{id}/sync
POST   /api/calendar/calendars/{id}/sync/now
POST   /api/calendar/calendars/export
POST   /api/calendar/calendars/{id}/import
GET    /api/calendar/calendars/settings
PUT    /api/calendar/calendars/settings
GET    /api/calendar/calendars/statistics
GET    /api/calendar/calendars/types

GET    /api/calendar/events
GET    /api/calendar/events/{id}
POST   /api/calendar/events
PUT    /api/calendar/events/{id}
DELETE /api/calendar/events/{id}
POST   /api/calendar/events/{id}/cancel
POST   /api/calendar/events/{id}/duplicate
GET    /api/calendar/events/agenda
GET    /api/calendar/events/today
GET    /api/calendar/events/upcoming
GET    /api/calendar/events/{id}/attendees
POST   /api/calendar/events/{id}/attendees
DELETE /api/calendar/events/{id}/attendees/{attendeeId}
POST   /api/calendar/events/{id}/attendees/{attendeeId}/resend-invitation
POST   /api/calendar/events/{id}/rsvp
POST   /api/calendar/events/{id}/check-in
POST   /api/calendar/events/{id}/feedback
GET    /api/calendar/events/{id}/reminders
POST   /api/calendar/events/{id}/reminders
DELETE /api/calendar/events/{id}/reminders/{reminderId}
POST   /api/calendar/events/free-busy
POST   /api/calendar/events/find-times
GET    /api/calendar/events/types
GET    /api/calendar/events/statuses
GET    /api/calendar/events/recurrence-frequencies

GET    /api/calendar/holidays
GET    /api/calendar/holidays/{id}
POST   /api/calendar/holidays
PUT    /api/calendar/holidays/{id}
DELETE /api/calendar/holidays/{id}
GET    /api/calendar/holidays/types
POST   /api/calendar/holidays/import

GET    /api/calendar/time-off
GET    /api/calendar/time-off/team
GET    /api/calendar/time-off/pending-approvals
GET    /api/calendar/time-off/{id}
POST   /api/calendar/time-off
PUT    /api/calendar/time-off/{id}
POST   /api/calendar/time-off/{id}/cancel
POST   /api/calendar/time-off/{id}/approve
GET    /api/calendar/time-off/types
GET    /api/calendar/time-off/balance

GET    /api/calendar/working-hours
PUT    /api/calendar/working-hours
GET    /api/calendar/working-hours/organization
PUT    /api/calendar/working-hours/organization
```

---

### 12. Notifications Module [COMPLETE]
Multi-channel notification system with templates, preferences, and real-time delivery.

**Domain Entities:**
- `Notification` - Core notification with localized content
- `NotificationAction` - Action buttons for notifications
- `NotificationDelivery` - Delivery tracking per channel
- `NotificationTemplate` - Reusable notification templates
- `EmailTemplate` - Email-specific templates
- `TemplatePlaceholder` - Dynamic placeholders
- `NotificationPreferences` - User preference settings
- `CategoryPreference` - Per-category settings
- `TypePreference` - Per-notification-type settings
- `MutedEntity` - Muted entities tracking
- `DeviceRegistration` - Push notification devices
- `NotificationDigest` - Digest tracking
- `NotificationSubscription` - Entity subscriptions

**Enums:**
- `NotificationType` - 30+ types (TaskAssigned, ContentPublished, EventInvitation, MentionNotification, etc.)
- `NotificationCategory` - Workflow, Content, Collaboration, Calendar, System, Security, Admin
- `NotificationPriority` - Urgent, High, Normal, Low
- `DeliveryChannel` - InApp, Email, Push, SMS, Teams, Slack
- `DeliveryStatus` - Pending, Sent, Delivered, Failed, Bounced, Read
- `DigestFrequency` - Instant, Hourly, Daily, Weekly, None
- `DevicePlatform` - Web, iOS, Android, Windows, MacOS
- `EmailTemplateCategory` - Transactional, Marketing, Digest, System

**DTOs:**
- `NotificationDto`, `NotificationActionDto`, `RealTimeNotificationDto`
- `CreateNotificationRequest`, `SendTemplatedNotificationRequest`
- `BroadcastNotificationRequest`, `NotificationFilterRequest`
- `NotificationStatsDto`, `MarkNotificationsRequest`
- `NotificationPreferencesDto`, `CategoryPreferenceDto`, `TypePreferenceDto`
- `MutedEntityDto`, `UpdatePreferencesRequest`
- `UpdateCategoryPreferenceRequest`, `UpdateTypePreferenceRequest`
- `MuteEntityRequest`, `DeviceRegistrationDto`, `RegisterDeviceRequest`
- `SubscriptionDto`, `SubscribeRequest`, `ChannelInfoDto`
- `NotificationTemplateDto`, `TemplatePlaceholderDto`
- `CreateTemplateRequest`, `EmailTemplateDto`, `CreateEmailTemplateRequest`
- `PreviewTemplateRequest`, `TemplatePreviewDto`, `SendTestNotificationRequest`
- `DeliveryStatsDto`, `NotificationAnalyticsDto`, `DailyNotificationStatsDto`

**Controllers:**
- `NotificationsController` - User notifications management
- `PreferencesController` - User preferences, devices, subscriptions
- `TemplatesController` - Admin template management and analytics

**API Endpoints:**
```
# User Notifications
GET    /api/notifications
GET    /api/notifications/{id}
GET    /api/notifications/stats
GET    /api/notifications/unread-count
POST   /api/notifications/{id}/read
POST   /api/notifications/{id}/unread
POST   /api/notifications/read
POST   /api/notifications/read-all
POST   /api/notifications/{id}/archive
DELETE /api/notifications/{id}
DELETE /api/notifications

# Notification Types & Categories
GET    /api/notifications/types
GET    /api/notifications/categories
GET    /api/notifications/channels

# User Preferences
GET    /api/notifications/preferences
PUT    /api/notifications/preferences
PUT    /api/notifications/preferences/category
PUT    /api/notifications/preferences/type
POST   /api/notifications/preferences/reset

# Mute/Unmute
GET    /api/notifications/preferences/muted
POST   /api/notifications/preferences/mute
DELETE /api/notifications/preferences/mute/{entityType}/{entityId}

# Device Registration
GET    /api/notifications/preferences/devices
POST   /api/notifications/preferences/devices
PUT    /api/notifications/preferences/devices/{id}
DELETE /api/notifications/preferences/devices/{id}

# Subscriptions
GET    /api/notifications/preferences/subscriptions
POST   /api/notifications/preferences/subscriptions
DELETE /api/notifications/preferences/subscriptions/{entityType}/{entityId}

# Admin - Templates
GET    /api/notifications/admin/templates
GET    /api/notifications/admin/templates/{id}
GET    /api/notifications/admin/templates/key/{key}
POST   /api/notifications/admin/templates
PUT    /api/notifications/admin/templates/{id}
DELETE /api/notifications/admin/templates/{id}
POST   /api/notifications/admin/templates/{id}/activate
POST   /api/notifications/admin/templates/{id}/deactivate
POST   /api/notifications/admin/templates/preview
POST   /api/notifications/admin/templates/test
POST   /api/notifications/admin/templates/{id}/clone

# Admin - Email Templates
GET    /api/notifications/admin/templates/email
GET    /api/notifications/admin/templates/email/{id}
POST   /api/notifications/admin/templates/email
PUT    /api/notifications/admin/templates/email/{id}
DELETE /api/notifications/admin/templates/email/{id}
POST   /api/notifications/admin/templates/email/{id}/preview

# Admin - Analytics
GET    /api/notifications/admin/templates/analytics
GET    /api/notifications/admin/templates/analytics/channels
GET    /api/notifications/admin/templates/analytics/templates

# Admin - Broadcast
POST   /api/notifications/admin/templates/broadcast
GET    /api/notifications/admin/templates/broadcast/history
DELETE /api/notifications/admin/templates/broadcast/{jobId}

# Admin - Configuration
GET    /api/notifications/admin/templates/config
PUT    /api/notifications/admin/templates/config
POST   /api/notifications/admin/templates/config/test-email
POST   /api/notifications/admin/templates/config/test-push
```

**Frontend Components:**
- `NotificationsView.vue` - Full notifications page with filtering, bulk actions
- `NotificationDropdown.vue` - Header dropdown bell with real-time updates
- `NotificationPreferencesView.vue` - Comprehensive preferences settings

**Module Configuration:**
- `NotificationsModule.cs` - DI configuration with options
- `NotificationOptions` - System-wide settings (timezone, retry, grouping)
- `EmailOptions` - SMTP/SendGrid/SES configuration
- `PushOptions` - Firebase/APNS/WebPush configuration
- `SmsOptions` - Twilio/Unifonic configuration

**Translations:**
- 120+ English translations
- 120+ Arabic translations

---

## 13. AI Module [COMPLETE]
Intalio AI Engine integration with comprehensive AI services.

**Domain Entities:**
- `AIJob` - AI job tracking with 30+ job types
- `TranscriptionResult` - Transcription with segments, speakers, words
- `TranscriptionSegment`, `TranscriptionWord`, `TranscriptionSpeaker`
- `DocumentSummary` - Document summarization results
- `MeetingMinutes` - Generated meeting minutes with participants, agenda, actions
- `ContentClassification` - Content classification with labels
- `SentimentResult` - Sentiment analysis results
- `ContentSafetyResult` - Content moderation results
- `AIModelConfig` - AI model configurations
- `AIUsageQuota` - Usage quota tracking
- `VectorEmbedding` - Vector embeddings for semantic search
- `SemanticSearchQuery` - Semantic search queries
- `SemanticSearchResult` - Semantic search results
- `QASession`, `QAExchange` - Question answering sessions
- `SmartSuggestion` - AI-powered suggestions
- `KnowledgeGraphEntity`, `KnowledgeGraphRelation` - Knowledge graph
- `AIPromptTemplate` - Customizable prompt templates

**Enums:**
- `AIJobType` - 30+ types (Transcription, Summarization, MeetingMinutes, Classification, Translation, etc.)
- `AIJobStatus` - Pending, Running, Completed, Failed, Cancelled
- `AIProvider` - IntalioAI, AzureOpenAI, OpenAI, AzureCognitiveServices, GoogleCloud, AWS
- `SummaryLength` - Short, Medium, Long, Custom
- `EntityType` - Person, Organization, Location, Date, Money, etc.
- `SuggestionType` - RelatedContent, NextAction, SmartReply, AutoComplete, etc.
- `SearchScope` - All, Documents, Articles, Media, Discussions

**DTOs:**
- `AIJobDto`, `CreateAIJobRequest`, `AIUsageStatsDto`, `AIQuotaStatusDto`
- `TranscriptionRequest`, `TranscriptionResultDto`, `TranscriptionSegmentDto`
- `SummarizationRequest`, `DocumentSummaryDto`
- `GenerateMeetingMinutesRequest`, `MeetingMinutesDto`, `MeetingParticipantDto`
- `ClassifyContentRequest`, `ContentClassificationDto`, `ClassificationLabelDto`
- `TranslationRequest`, `TextGenerationRequest`
- `SemanticSearchRequest`, `SemanticSearchResponse`, `SemanticSearchResultDto`
- `QuestionAnsweringRequest`, `QuestionAnsweringResponse`, `QASessionDto`
- `SmartSuggestionDto`, `KnowledgeGraphEntityDto`, `IndexContentRequest`

**Controllers:**
- `AIJobsController` - Job management, types, providers, models, quota
- `TranscriptionController` - Audio/video transcription services
- `SummarizationController` - Document summarization, meeting minutes
- `SemanticSearchController` - Semantic search, Q&A, smart suggestions
- `AIAdminController` - Prompt templates, model configuration, analytics

**API Endpoints:**
```
# AI Jobs
GET    /api/ai/jobs
GET    /api/ai/jobs/{id}
POST   /api/ai/jobs
POST   /api/ai/jobs/{id}/cancel
DELETE /api/ai/jobs/{id}
GET    /api/ai/jobs/{id}/progress
GET    /api/ai/jobs/{id}/result
GET    /api/ai/jobs/my
GET    /api/ai/jobs/types
GET    /api/ai/jobs/providers
GET    /api/ai/jobs/models
GET    /api/ai/jobs/quota
GET    /api/ai/jobs/usage

# Transcription
POST   /api/ai/transcription/audio
POST   /api/ai/transcription/video
POST   /api/ai/transcription/url
GET    /api/ai/transcription/{jobId}/result
POST   /api/ai/transcription/{jobId}/export
GET    /api/ai/transcription/languages
GET    /api/ai/transcription/recent

# Summarization
POST   /api/ai/summarization/document
POST   /api/ai/summarization/text
POST   /api/ai/summarization/url
POST   /api/ai/summarization/multiple
GET    /api/ai/summarization/{jobId}/result
POST   /api/ai/summarization/meeting-minutes
POST   /api/ai/summarization/meeting-from-transcript
GET    /api/ai/summarization/lengths
GET    /api/ai/summarization/recent

# Semantic Search
POST   /api/ai/search/semantic
GET    /api/ai/search/suggestions
POST   /api/ai/search/qa
GET    /api/ai/search/qa/sessions
GET    /api/ai/search/qa/sessions/{sessionId}
POST   /api/ai/search/qa/sessions/{sessionId}/followup
POST   /api/ai/search/qa/sessions/{sessionId}/feedback
GET    /api/ai/search/knowledge-graph
GET    /api/ai/search/knowledge-graph/entity/{id}
GET    /api/ai/search/knowledge-graph/entity/{id}/relations
GET    /api/ai/search/scopes

# Admin
GET    /api/ai/admin/templates
GET    /api/ai/admin/templates/{id}
POST   /api/ai/admin/templates
PUT    /api/ai/admin/templates/{id}
DELETE /api/ai/admin/templates/{id}
GET    /api/ai/admin/models
POST   /api/ai/admin/models
PUT    /api/ai/admin/models/{id}
POST   /api/ai/admin/models/{id}/activate
POST   /api/ai/admin/models/{id}/deactivate
GET    /api/ai/admin/quotas
PUT    /api/ai/admin/quotas/{userId}
POST   /api/ai/admin/quotas/reset
POST   /api/ai/admin/index
POST   /api/ai/admin/reindex
GET    /api/ai/admin/analytics
GET    /api/ai/admin/analytics/usage
GET    /api/ai/admin/analytics/popular-queries
```

**Module Configuration:**
- `AIModule.cs` - DI configuration with comprehensive options
- `IntalioAIOptions` - Intalio AI Engine configuration (Arabic optimization)
- `AzureOpenAIOptions` - Azure OpenAI configuration
- `AzureCognitiveServicesOptions` - Azure Cognitive Services configuration
- `SemanticSearchOptions` - Vector search configuration
- `QuotaOptions` - Usage quota settings

**Frontend Components:**
- `AIServicesView.vue` - Transcription, summarization, Q&A services
- `SemanticSearchView.vue` - AI-powered semantic search interface

**Translations:**
- 80+ English translations for AI services
- 80+ Arabic translations for AI services

---

## 14. Integration Module [COMPLETE]
Comprehensive integration with Intalio Suite, Microsoft 365, and ERP systems.

**Domain Entities:**
- `IntegrationConnector` - Connector configuration with health tracking
- `EntityMapping` - Entity mapping between systems
- `FieldMapping` - Field-level mapping
- `SyncJob` - Synchronization job tracking
- `WebhookConfig` - Webhook configuration
- `WebhookDelivery` - Webhook delivery tracking
- `ExternalEntityRef` - External entity references
- `IntegrationEventLog` - Event logging
- `IntalioCaseConfig` - Intalio Case (BPM) configuration
- `IntalioProcessDefinition` - Process definitions
- `IntalioFormMapping` - Form field mappings
- `IntalioIAMConfig` - Intalio IAM configuration
- `IntalioDMSConfig` - Intalio DMS configuration
- `LibraryMapping`, `ContentTypeMapping` - DMS mappings
- `IntalioCorrespondenceConfig` - Correspondence configuration
- `IntalioTaskRef` - Task synchronization tracking
- `Microsoft365Config` - M365 configuration
- `SharePointLibraryMapping` - SharePoint library mappings
- `ERPConfig` - ERP configuration
- `ERPAPIMapping`, `ERPWorkflowMapping` - ERP mappings

**Enums:**
- `IntegrationProvider` - IntalioCase, IntalioIAM, IntalioDMS, IntalioCorrespondence, SharePoint, Exchange, MicrosoftTeams, SAP, Oracle, Dynamics365, etc.
- `IntegrationCategory` - Workflow, Identity, DocumentManagement, Correspondence, Calendar, Communication, ERP
- `ConnectorStatus` - Active, Inactive, Error, Configuring, Syncing
- `AuthenticationType` - OAuth2, APIKey, Basic, Certificate, Windows
- `SyncDirection` - Inbound, Outbound, Bidirectional
- `SyncJobType` - Full, Incremental, Delta, Scheduled, Manual
- `SyncJobStatus` - Pending, Running, Completed, Failed, Cancelled, Paused
- `IntalioTaskStatus` - Created, Assigned, Claimed, Completed, Cancelled, Delegated, Escalated
- `ERPSystem` - SAP_S4HANA, SAP_ECC, Oracle_EBS, Oracle_Cloud, Microsoft_Dynamics365
- `CorrespondenceType` - Incoming, Outgoing, Internal

**DTOs:**
- `ConnectorDto`, `ConnectorDetailDto`, `ConnectorStatsDto`
- `CreateConnectorRequest`, `UpdateConnectorRequest`, `TestConnectionResultDto`
- `EntityMappingDto`, `FieldMappingDto`
- `SyncJobDto`, `SyncJobDetailDto`, `CreateSyncJobRequest`, `RunSyncJobRequest`
- `SyncErrorDto`, `ExternalEntityRefDto`, `IntegrationEventLogDto`
- `WebhookConfigDto`, `CreateWebhookRequest`, `WebhookDeliveryDto`
- `IntalioProcessDto`, `IntalioTaskDto`, `StartIntalioProcessRequest`
- `CompleteIntalioTaskRequest`, `ClaimIntalioTaskRequest`
- `IntalioUserDto`, `IntalioGroupDto`, `IntalioDocumentDto`
- `UploadToIntalioDMSRequest`, `IntalioCorrespondenceDto`, `CreateCorrespondenceRequest`
- `M365SyncStatusDto`, `SharePointDocumentDto`
- `ERPEmployeeDto`, `ERPOrganizationUnitDto`, `ERPCostCenterDto`, `ERPVendorDto`
- `IntegrationDashboardDto`, `ConnectorStatusDto`, `RecentSyncJobDto`

**Controllers:**
- `ConnectorsController` - Connector CRUD, test connection, health, providers
- `IntalioController` - Intalio Case, IAM, DMS, Correspondence integration
- `SyncJobsController` - Sync job management, entity references, event logs
- `WebhooksController` - Webhook management, incoming webhooks
- `M365Controller` - SharePoint, Exchange, Teams, Azure AD integration
- `ERPController` - SAP/Oracle employee, organization, workflow integration

**API Endpoints:**
```
# Connectors
GET    /api/integration/connectors
GET    /api/integration/connectors/{id}
POST   /api/integration/connectors
PUT    /api/integration/connectors/{id}
DELETE /api/integration/connectors/{id}
POST   /api/integration/connectors/{id}/test
POST   /api/integration/connectors/test
POST   /api/integration/connectors/{id}/activate
POST   /api/integration/connectors/{id}/deactivate
GET    /api/integration/connectors/{id}/health
GET    /api/integration/connectors/{id}/stats
GET    /api/integration/connectors/providers

# Intalio Case (BPM)
GET    /api/integration/intalio/case/processes
POST   /api/integration/intalio/case/processes/start
GET    /api/integration/intalio/case/tasks
GET    /api/integration/intalio/case/tasks/{taskId}
POST   /api/integration/intalio/case/tasks/complete
POST   /api/integration/intalio/case/tasks/claim
POST   /api/integration/intalio/case/sync/processes
POST   /api/integration/intalio/case/sync/tasks

# Intalio IAM
GET    /api/integration/intalio/iam/users
GET    /api/integration/intalio/iam/groups
POST   /api/integration/intalio/iam/sync/users
POST   /api/integration/intalio/iam/sync/groups

# Intalio DMS
GET    /api/integration/intalio/dms/documents
GET    /api/integration/intalio/dms/documents/{documentId}
POST   /api/integration/intalio/dms/documents
GET    /api/integration/intalio/dms/documents/{documentId}/download
POST   /api/integration/intalio/dms/sync

# Intalio Correspondence
GET    /api/integration/intalio/correspondence
POST   /api/integration/intalio/correspondence
POST   /api/integration/intalio/correspondence/sync

# Sync Jobs
GET    /api/integration/sync-jobs
GET    /api/integration/sync-jobs/{id}
POST   /api/integration/sync-jobs
PUT    /api/integration/sync-jobs/{id}
DELETE /api/integration/sync-jobs/{id}
POST   /api/integration/sync-jobs/{id}/run
POST   /api/integration/sync-jobs/{id}/cancel
POST   /api/integration/sync-jobs/{id}/pause
POST   /api/integration/sync-jobs/{id}/resume
GET    /api/integration/sync-jobs/{id}/history
GET    /api/integration/sync-jobs/{id}/errors
POST   /api/integration/sync-jobs/{id}/retry
GET    /api/integration/sync-jobs/entity-refs
GET    /api/integration/sync-jobs/entity-refs/by-local/{entityType}/{entityId}
GET    /api/integration/sync-jobs/entity-refs/by-external/{connectorId}/{externalId}
POST   /api/integration/sync-jobs/entity-refs/link
DELETE /api/integration/sync-jobs/entity-refs/{id}
GET    /api/integration/sync-jobs/events

# Webhooks
GET    /api/integration/webhooks
GET    /api/integration/webhooks/{id}
POST   /api/integration/webhooks
PUT    /api/integration/webhooks/{id}
DELETE /api/integration/webhooks/{id}
POST   /api/integration/webhooks/{id}/activate
POST   /api/integration/webhooks/{id}/deactivate
POST   /api/integration/webhooks/{id}/test
GET    /api/integration/webhooks/{id}/deliveries
GET    /api/integration/webhooks/deliveries/{deliveryId}
POST   /api/integration/webhooks/deliveries/{deliveryId}/retry
POST   /api/integration/webhooks/receive/intalio-case
POST   /api/integration/webhooks/receive/intalio-iam
POST   /api/integration/webhooks/receive/intalio-dms
POST   /api/integration/webhooks/receive/intalio-correspondence
POST   /api/integration/webhooks/receive/{connectorKey}
GET    /api/integration/webhooks/events

# Microsoft 365
GET    /api/integration/m365/sharepoint/status
GET    /api/integration/m365/sharepoint/sites
GET    /api/integration/m365/sharepoint/sites/{siteId}/libraries
GET    /api/integration/m365/sharepoint/documents
POST   /api/integration/m365/sharepoint/sync/document/{documentId}
POST   /api/integration/m365/sharepoint/mappings
POST   /api/integration/m365/sharepoint/sync
GET    /api/integration/m365/exchange/events
POST   /api/integration/m365/exchange/sync/event/{eventId}
GET    /api/integration/m365/exchange/mailboxes
POST   /api/integration/m365/exchange/sync
GET    /api/integration/m365/teams/channels
POST   /api/integration/m365/teams/channels/{channelId}/messages
POST   /api/integration/m365/teams/meetings
GET    /api/integration/m365/azure-ad/users
GET    /api/integration/m365/azure-ad/groups
POST   /api/integration/m365/azure-ad/sync

# ERP
GET    /api/integration/erp/status
GET    /api/integration/erp/systems
GET    /api/integration/erp/employees
GET    /api/integration/erp/employees/{employeeId}
GET    /api/integration/erp/employees/by-kms-user/{userId}
POST   /api/integration/erp/employees/sync
GET    /api/integration/erp/organization
GET    /api/integration/erp/organization/hierarchy
POST   /api/integration/erp/organization/sync
GET    /api/integration/erp/cost-centers
POST   /api/integration/erp/cost-centers/sync
GET    /api/integration/erp/vendors
POST   /api/integration/erp/vendors/sync
GET    /api/integration/erp/workflows/pending
POST   /api/integration/erp/workflows/{workItemId}/approve
POST   /api/integration/erp/workflows/{workItemId}/reject
POST   /api/integration/erp/purchase-requisitions
```

**Module Configuration:**
- `IntegrationModule.cs` - DI configuration with comprehensive options
- `IntegrationOptions` - System-wide integration settings
- `IntalioCaseOptions` - Intalio Case (BPM) configuration
- `IntalioIAMOptions` - Intalio IAM configuration with SSO
- `IntalioDMSOptions` - Intalio DMS configuration
- `IntalioCorrespondenceOptions` - Correspondence configuration
- `Microsoft365Options` - SharePoint, Exchange, Teams, Azure AD settings
- `ERPOptions` - SAP/Oracle ERP configuration

**Frontend Components:**
- `IntegrationDashboardView.vue` - Integration monitoring dashboard

**Translations:**
- 60+ English translations for integrations
- 60+ Arabic translations for integrations

---

## Database Schema (Planned)

### Identity Tables
- `Users`, `Roles`, `Permissions`, `UserRoles`
- `Departments`, `Groups`, `GroupMembers`

### Content Tables
- `Articles`, `ArticleVersions`, `Categories`, `Tags`
- `ArticleTags`, `ArticleCategories`

### Documents Tables
- `DocumentLibraries`, `Folders`, `Documents`
- `DocumentVersions`, `DocumentMetadata`, `DocumentAudit`

### Collaboration Tables
- `Communities`, `CommunityMembers`
- `Discussions`, `DiscussionTags`, `DiscussionReactions`
- `Comments`, `CommentReactions`, `Mentions`
- `Follows`, `LessonsLearned`, `LessonTags`

### Media Tables
- `MediaGalleries`, `MediaItems`, `MediaThumbnails`
- `VideoEditJobs`, `MediaTags`

### Workflow Tables
- `Services`, `ServiceRequests`, `Tasks`
- `Forms`, `FormFields`, `FormSubmissions`
- `Polls`, `PollOptions`, `Votes`

### Calendar Tables
- `Events`, `EventAttendees`, `RSVPs`
- `RecurringEventRules`

### Notifications Tables
- `Notifications`, `NotificationActions`, `NotificationDeliveries`
- `NotificationTemplates`, `EmailTemplates`, `TemplatePlaceholders`
- `NotificationPreferences`, `CategoryPreferences`, `TypePreferences`
- `MutedEntities`, `DeviceRegistrations`, `NotificationDigests`
- `NotificationSubscriptions`

---

## API Gateway (YARP) Configuration

```yaml
Routes:
  - /api/identity/*    -> Identity Module
  - /api/content/*     -> Content Module
  - /api/documents/*   -> Documents Module
  - /api/collaboration/* -> Collaboration Module
  - /api/media/*       -> Media Module
  - /api/search/*      -> Search Module
  - /api/workflow/*    -> Workflow Module
  - /api/calendar/*    -> Calendar Module
  - /api/notifications/* -> Notifications Module
```

---

## Docker Services

```yaml
services:
  sqlserver:     SQL Server 2022
  redis:         Redis 7
  rabbitmq:      RabbitMQ 3 with Management
  elasticsearch: Elasticsearch 8
  kibana:        Kibana 8
  minio:         MinIO (S3-compatible storage)
```

---

## Security Checklist

- [ ] TLS 1.3 for all traffic
- [ ] JWT authentication with refresh tokens
- [ ] Azure AD SSO integration
- [ ] Role-based access control (RBAC)
- [ ] Resource-based authorization
- [ ] SQL Server TDE
- [ ] Field-level encryption for PII
- [ ] OWASP Top 10 compliance
- [ ] Rate limiting
- [ ] Input validation
- [ ] CORS configuration
- [ ] CSP headers

---

## Performance Targets

- Page load time: < 2 seconds
- API response time: < 200ms (p95)
- Search response time: < 500ms
- File upload: Up to 100MB
- Video streaming: Adaptive bitrate
- Concurrent users: 10,000+
- Uptime SLA: 99.95%

---

## Next Steps

1. **Database Implementation** - EF Core migrations and schema
2. **Authentication** - Azure AD SSO integration
3. **Service Implementation** - Implement business logic for all modules
4. **Testing** - Unit and integration tests
5. **Deployment** - Kubernetes setup
6. **Performance Optimization** - Caching, indexing, optimization
7. **Security Hardening** - Penetration testing, security audit
8. **Documentation** - API documentation, user guides
