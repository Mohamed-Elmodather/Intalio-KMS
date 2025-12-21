# AFC Asian Cup 2027 KMS - Enhancement Implementation Plan

> **Last Updated**: December 18, 2024
> **Status**: Approved - Ready for Implementation
> **Priority**: Build frontend/backend first, prepare for AI integration later

---

## Executive Summary

Enhance the existing KMS POC to meet the Enterprise AI-Enhanced Knowledge Management Platform specifications. The current system has solid domain models and API scaffolding but lacks actual service implementations, database migrations, background workers, and Intalio AI integration.

---

## Key Decisions

| Question | Decision |
|----------|----------|
| **Priority** | Build frontend/backend first, prepare for AI integration later |
| **Storage** | Local Storage for development and on-premises deployment |
| **Live Collaboration** | CRDT-based real-time editing is MVP requirement |
| **AI Integration** | Mock implementation first, real Intalio when credentials available |

---

## Gap Analysis Summary

| Category | Current State | Required | Priority |
|----------|--------------|----------|----------|
| **Database** | Entities defined, no migrations | Full EF Core setup | Critical |
| **Services** | Controllers with mock data | Real service layer | Critical |
| **AI Integration** | Options configured, no client | Full Intalio integration | Critical |
| **Streaming Chat** | Returns complete responses | SSE streaming + typewriter | High |
| **ACL System** | Library-level only | Folder/File granular ACLs | High |
| **Permission Inheritance** | None | Parent-to-child with override | High |
| **Background Workers** | Directory empty | Media, AI, Notification workers | High |
| **Ingestion Pipeline** | None | Event-driven text extraction | High |
| **Live Collaboration** | None | CRDT + SignalR | Medium |
| **Block Editor** | Basic HTML editor | Block-based (Notion-like) | Medium |
| **Legal Hold/Quarantine** | None | Full compliance features | Medium |
| **Multi-part Upload** | Max 500MB | Chunked up to 5GB | Medium |
| **Audit Logging** | Per-entity fields | Central immutable log | Medium |
| **User Lifecycle** | Active/Inactive only | Invite/Suspend/Promote | Low |
| **Impersonation** | None | Admin debugging feature | Low |

---

## Implementation Order

```
Phase 1: Infrastructure Foundation
    |   - Database (EF Core migrations)
    |   - Local Storage Service
    |   - MassTransit Message Queue
    |   - Background Workers scaffolding
    |
    +-> Phase 2: Core DMS + ACL System
    |       - Document/Library/Folder services
    |       - Granular permissions (Folder/File level)
    |       - Permission inheritance
    |       - Secure preview
    |
    +-> Phase 3: Content Studio + Live Collaboration (MVP)
    |       - Block-based editor (Notion-like)
    |       - SignalR real-time hub
    |       - CRDT implementation
    |       - Cursor tracking & presence
    |
    +-> Phase 4: Admin & Governance
    |       - User lifecycle (Invite/Suspend)
    |       - Impersonation
    |       - Central audit logging
    |       - Legal Hold/Quarantine
    |       - Metrics dashboard
    |
    +-> Phase 5: AI Integration (Mock -> Real)
            - Mock Intalio client interfaces
            - Streaming chat with SSE (mock responses)
            - RAG service with ACL injection (prepared)
            - Ingestion pipeline structure
            - -> Replace with real Intalio when credentials available
```

---

## Phase 1: Infrastructure Foundation

**Goal**: Establish database, messaging, and storage infrastructure.

### 1.1 Database Setup
**Files to Create**:
```
backend/src/Shared/AFC27.KMS.Infrastructure/
+-- Persistence/
|   +-- ApplicationDbContext.cs
|   +-- Configurations/
|   |   +-- UserConfiguration.cs
|   |   +-- DocumentConfiguration.cs
|   |   +-- ArticleConfiguration.cs
|   |   +-- ... (per entity)
|   +-- Interceptors/
|       +-- AuditableEntityInterceptor.cs
|       +-- DomainEventDispatcherInterceptor.cs
|       +-- SoftDeleteInterceptor.cs
```

**Key Implementation**:
- Single `ApplicationDbContext` with all entity configurations
- Global query filter for soft delete: `.HasQueryFilter(e => !e.IsDeleted)`
- Audit interceptor for CreatedAt/ModifiedAt/DeletedAt
- Domain event dispatcher for async processing triggers

### 1.2 Message Queue with MassTransit
**Files to Create**:
```
backend/src/Shared/AFC27.KMS.Infrastructure/
+-- Messaging/
    +-- MassTransitConfiguration.cs
    +-- Messages/
    |   +-- DocumentUploadedMessage.cs
    |   +-- AIIngestionRequestMessage.cs
    |   +-- ThumbnailGenerationMessage.cs
    |   +-- TranscodingRequestMessage.cs
    +-- Consumers/
        +-- BaseConsumer.cs
```

### 1.3 Storage Service
**Files to Create**:
```
backend/src/Shared/AFC27.KMS.Infrastructure/
+-- Storage/
    +-- IStorageService.cs
    +-- LocalStorageService.cs
    +-- MultipartUploadHandler.cs
```

**Interface**:
```csharp
public interface IStorageService
{
    Task<string> InitiateMultipartUploadAsync(string fileName, string contentType);
    Task<string> UploadPartAsync(string uploadId, int partNumber, Stream content);
    Task<string> CompleteMultipartUploadAsync(string uploadId, List<string> partETags);
    Task<Stream> GetFileStreamAsync(string path);
    Task<string> GenerateSecurePreviewUrlAsync(string path, TimeSpan expiry);
}
```

### 1.4 Background Workers
**Projects to Create**:
```
backend/src/Workers/
+-- AFC27.KMS.MediaWorker/
|   +-- Program.cs
|   +-- Consumers/
|   |   +-- ThumbnailGenerationConsumer.cs
|   |   +-- VideoTranscodingConsumer.cs
|   +-- Services/
|       +-- FFmpegService.cs
|       +-- ImageProcessingService.cs
+-- AFC27.KMS.AIWorker/
|   +-- Program.cs
|   +-- Consumers/
|   |   +-- IngestionConsumer.cs
|   |   +-- EmbeddingConsumer.cs
|   +-- Services/
|       +-- TextExtractionService.cs
+-- AFC27.KMS.NotificationWorker/
    +-- Program.cs
    +-- Consumers/
        +-- NotificationConsumer.cs
```

---

## Phase 2: Core DMS Implementation

**Goal**: Implement document management with granular ACLs.

### 2.1 Document Services
**Files to Create**:
```
backend/src/Modules/AFC27.KMS.Documents/
+-- Application/
    +-- Interfaces/
    |   +-- IDocumentService.cs
    |   +-- ILibraryService.cs
    |   +-- IPermissionService.cs
    +-- Services/
        +-- DocumentService.cs
        +-- LibraryService.cs
        +-- FolderService.cs
        +-- DocumentVersionService.cs
```

**Files to Modify**:
- `DocumentsController.cs` - Replace mock data with service calls
- `DocumentsModule.cs` - Register services in DI

### 2.2 Granular ACL System
**Files to Create**:
```
backend/src/Modules/AFC27.KMS.Documents/
+-- Domain/Entities/
|   +-- FolderPermission.cs
|   +-- DocumentPermission.cs
+-- Application/Services/
    +-- PermissionService.cs
    +-- PermissionInheritanceService.cs
```

**Permission Model**:
```csharp
public class FolderPermission : Entity
{
    public Guid FolderId { get; private set; }
    public Guid? UserId { get; private set; }
    public Guid? GroupId { get; private set; }
    public PermissionLevel Level { get; private set; }
    public bool InheritFromParent { get; private set; } = true;
    public bool PropagateToChildren { get; private set; } = true;
}

public enum PermissionLevel
{
    None = 0, Read = 1, Write = 2, Delete = 4, Share = 8, Manage = 16,
    FullControl = Read | Write | Delete | Share | Manage
}
```

### 2.3 Secure Preview
**Files to Create**:
```
backend/src/Modules/AFC27.KMS.Documents/
+-- Application/Services/
|   +-- PreviewService.cs
+-- Presentation/Controllers/
    +-- PreviewController.cs
```

---

## Phase 3: Content Studio + Live Collaboration (MVP)

**Goal**: Implement block-based editor with CRDT real-time collaboration.

### 3.1 SignalR Real-time Infrastructure
**Files to Create**:
```
backend/src/Modules/AFC27.KMS.Content/
+-- Infrastructure/
|   +-- Hubs/
|       +-- CollaborationHub.cs
+-- Application/Services/
    +-- CollaborationService.cs
    +-- PresenceService.cs
```

### 3.2 Block Editor Backend
**Files to Create**:
```
backend/src/Modules/AFC27.KMS.Content/
+-- Domain/Entities/
|   +-- ContentBlock.cs
+-- Domain/ValueObjects/
|   +-- BlockContent.cs
+-- Application/Services/
    +-- BlockEditorService.cs
    +-- CRDTService.cs
```

**Block Types**:
```csharp
public enum BlockType
{
    Paragraph, Heading1, Heading2, Heading3,
    BulletList, NumberedList, Quote, Code,
    Image, Video, Embed, Table, Divider, Callout, Toggle
}
```

### 3.3 Frontend Block Editor
**Files to Create**:
```
frontend/src/
+-- components/content/
|   +-- BlockEditor.vue
|   +-- BlockToolbar.vue
|   +-- blocks/
|       +-- ParagraphBlock.vue
|       +-- HeadingBlock.vue
|       +-- ListBlock.vue
|       +-- CodeBlock.vue
|       +-- ImageBlock.vue
|       +-- EmbedBlock.vue
+-- composables/
    +-- useCollaboration.ts
    +-- useCRDT.ts
    +-- usePresence.ts
```

---

## Phase 4: Admin & Governance

**Goal**: Implement user lifecycle, audit logging, and compliance features.

### 4.1 Central Audit Log
**Files to Create**:
```
backend/src/Shared/AFC27.KMS.Infrastructure/
+-- Auditing/
    +-- IAuditLogService.cs
    +-- AuditLogService.cs
    +-- AuditLogEntry.cs
    +-- DomainEventAuditHandler.cs
```

### 4.2 Legal Hold & Quarantine
**Files to Create**:
```
backend/src/Modules/AFC27.KMS.Documents/
+-- Domain/Entities/
|   +-- LegalHold.cs
|   +-- QuarantinedDocument.cs
+-- Application/Services/
    +-- LegalHoldService.cs
```

### 4.3 User Lifecycle Enhancement
**Files to Modify**:
- `backend/src/Modules/AFC27.KMS.Identity/Domain/Entities/User.cs`
  - Add `UserStatus` enum: Invited, Active, Suspended, Inactive
  - Add `Suspend()`, `Unsuspend()`, `Invite()` methods

**Files to Create**:
```
backend/src/Modules/AFC27.KMS.Identity/
+-- Application/Services/
|   +-- UserLifecycleService.cs
|   +-- ImpersonationService.cs
+-- Domain/Entities/
    +-- ImpersonationSession.cs
```

---

## Phase 5: AI Integration (Mock Implementation)

**Goal**: Prepare AI interfaces with mock implementations for future Intalio integration.

### 5.1 Mock Intalio AI Client
**Files to Create**:
```
backend/src/Modules/AFC27.KMS.AI/
+-- Infrastructure/
    +-- Clients/
    |   +-- IIntalioAIClient.cs          # Interface (production-ready)
    |   +-- MockIntalioAIClient.cs       # Mock implementation
    |   +-- IntalioAIClient.cs           # Real implementation (later)
    +-- Resilience/
        +-- IntalioCircuitBreaker.cs
```

### 5.2 ACL-Aware RAG Service
**Files to Create**:
```
backend/src/Modules/AFC27.KMS.AI/
+-- Application/
    +-- Services/
    |   +-- ACLAwareRAGService.cs
    |   +-- ContextBuilderService.cs
    |   +-- CitationService.cs
    +-- EventHandlers/
        +-- DocumentCreatedHandler.cs
        +-- DocumentUpdatedHandler.cs
```

### 5.3 Frontend Chat with Streaming
**Files to Create/Modify**:
```
frontend/src/
+-- views/ai/
|   +-- AIChatView.vue (modify for streaming)
+-- components/ai/
|   +-- ChatMessage.vue
|   +-- StreamingText.vue
|   +-- CitationFootnote.vue
|   +-- SourceSplitView.vue
+-- composables/
|   +-- useStreamingChat.ts
+-- services/
    +-- ai.service.ts
```

---

## Database Schema Additions

```sql
-- Granular Permissions
CREATE TABLE FolderPermissions (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    FolderId UNIQUEIDENTIFIER NOT NULL,
    UserId UNIQUEIDENTIFIER NULL,
    GroupId UNIQUEIDENTIFIER NULL,
    PermissionLevel INT NOT NULL,
    InheritFromParent BIT DEFAULT 1,
    PropagateToChildren BIT DEFAULT 1
);

CREATE TABLE DocumentPermissions (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    DocumentId UNIQUEIDENTIFIER NOT NULL,
    UserId UNIQUEIDENTIFIER NULL,
    GroupId UNIQUEIDENTIFIER NULL,
    PermissionLevel INT NOT NULL
);

-- Immutable Audit Log
CREATE TABLE AuditLogs (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    EventType NVARCHAR(100) NOT NULL,
    EntityType NVARCHAR(100),
    EntityId UNIQUEIDENTIFIER,
    UserId UNIQUEIDENTIFIER NOT NULL,
    Action NVARCHAR(100) NOT NULL,
    OldValues NVARCHAR(MAX),
    NewValues NVARCHAR(MAX),
    IpAddress NVARCHAR(45),
    Timestamp DATETIME2 NOT NULL
);

-- Legal Hold
CREATE TABLE LegalHolds (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    CaseNumber NVARCHAR(100),
    Status INT NOT NULL,
    StartDate DATETIME2 NOT NULL,
    EndDate DATETIME2 NULL
);

-- Vector Embeddings
CREATE TABLE VectorEmbeddings (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    SourceEntityId UNIQUEIDENTIFIER NOT NULL,
    SourceEntityType NVARCHAR(50) NOT NULL,
    ChunkIndex INT,
    ChunkText NVARCHAR(MAX),
    EmbeddingModel NVARCHAR(100),
    IndexedAt DATETIME2 NOT NULL
);

-- Impersonation
CREATE TABLE ImpersonationSessions (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    AdminUserId UNIQUEIDENTIFIER NOT NULL,
    TargetUserId UNIQUEIDENTIFIER NOT NULL,
    Reason NVARCHAR(500) NOT NULL,
    StartedAt DATETIME2 NOT NULL,
    EndedAt DATETIME2 NULL
);
```

---

## Configuration Updates

### appsettings.json Additions

```json
{
  "AI": {
    "DefaultProvider": "IntalioAI",
    "UseMockClient": true,
    "Intalio": {
      "Endpoint": "https://ai.intalio.com/api",
      "ApiKey": "",
      "TenantId": "afc27",
      "EmbeddingModel": "intalio-embed-v1",
      "GenerationModel": "intalio-generate-v1"
    },
    "SemanticSearch": {
      "ChunkSize": 500,
      "ChunkOverlap": 50
    },
    "CircuitBreaker": {
      "FailureThreshold": 5,
      "BreakDuration": "00:01:00"
    }
  },
  "Storage": {
    "Provider": "Local",
    "BasePath": "/storage/documents",
    "MaxFileSizeMB": 5120,
    "ChunkSizeMB": 5
  },
  "MassTransit": {
    "Host": "rabbitmq://localhost",
    "Queues": {
      "DocumentProcessing": "document-processing",
      "AIIngestion": "ai-ingestion",
      "MediaTranscoding": "media-transcoding"
    }
  }
}
```

---

## Critical Files Summary

| File | Purpose |
|------|---------|
| `Infrastructure/Persistence/ApplicationDbContext.cs` | EF Core database context |
| `Infrastructure/Messaging/MassTransitConfiguration.cs` | Message bus setup |
| `Documents/Application/Services/PermissionService.cs` | ACL enforcement |
| `AI/Infrastructure/Clients/IIntalioAIClient.cs` | Intalio interface |
| `AI/Infrastructure/Clients/MockIntalioAIClient.cs` | Mock AI for development |
| `AI/Application/Services/ACLAwareRAGService.cs` | RAG with permission filtering |
| `Content/Infrastructure/Hubs/CollaborationHub.cs` | Real-time SignalR hub |
| `Workers/AFC27.KMS.AIWorker/Program.cs` | AI background processing |

---

## Related Documents

- [MASTER-PROGRESS.md](../MASTER-PROGRESS.md) - Implementation progress tracker
- [LOCAL_DEVELOPMENT_SETUP.md](./LOCAL_DEVELOPMENT_SETUP.md) - Development environment setup
- [CONFIGURATION_REFERENCE.md](./CONFIGURATION_REFERENCE.md) - Configuration options
