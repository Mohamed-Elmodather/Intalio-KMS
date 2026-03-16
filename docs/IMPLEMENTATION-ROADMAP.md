# AFC27 KMS — Implementation Roadmap for Best Practice Alignment

A phased implementation plan to close the 49 identified gaps and elevate the AFC27 KMS to industry best practice standards. Each phase is designed to maximize value delivery by leveraging existing infrastructure, respecting module boundaries, and maintaining backward compatibility.

---

## Table of Contents

1. [Design Principles](#design-principles)
2. [Classification Methodology](#classification-methodology)
3. [Dependency Graph](#dependency-graph)
4. [Phase 0 — Foundation](#phase-0--foundation-weeks-14)
5. [Phase 1 — Critical Gaps](#phase-1--critical-gaps-weeks-38)
6. [Phase 2 — AI & Search Integration](#phase-2--ai--search-integration-weeks-512)
7. [Phase 3 — Collaboration & Content Export](#phase-3--collaboration--content-export-weeks-916)
8. [Phase 4 — Spaces, Permissions & Access](#phase-4--spaces-permissions--access-weeks-1320)
9. [Phase 5 — Advanced AI](#phase-5--advanced-ai-weeks-1724)
10. [Phase 6 — Analytics & Integration](#phase-6--analytics--integration-weeks-2128)
11. [Phase 7 — Admin, Compliance & External](#phase-7--admin-compliance--external-weeks-2532)
12. [Phase 8 — UX Enhancements](#phase-8--ux-enhancements-weeks-2936)
13. [Phase 9 — External Channels](#phase-9--external-channels-weeks-3340)
14. [Phase 10 — Low Priority & Continuous](#phase-10--low-priority--continuous-weeks-37)
15. [Database Migration Strategy](#database-migration-strategy)
16. [Module Boundary Decisions](#module-boundary-decisions)
17. [Effort Summary](#effort-summary)
18. [Gap Traceability Matrix](#gap-traceability-matrix)

---

## Design Principles

1. **Extend before build**: Leverage existing entities, services, and controllers wherever possible. The codebase has significant hidden depth (Follow entity, SmartSuggestion, WorkflowDefinition, TextExtractionService) that should be wired before building from scratch.
2. **Additive migrations only**: All database changes are additive (new tables, nullable columns). No breaking schema changes. Old API consumers continue working.
3. **Module boundaries respected**: New features belong in the module whose domain they naturally extend. Cross-cutting features go in SharedKernel or WebApi Features.
4. **Backend-first for infrastructure, frontend-first for wiring**: Infrastructure gaps (entities, services, queues) need backend work. "Wire" gaps where the backend exists but the frontend doesn't surface it are frontend-only tasks.
5. **Parallel phase execution**: Many phases can overlap. Dependencies are tracked explicitly.

---

## Classification Methodology

Each gap is classified as one of three types:

| Classification | Definition | Example |
|---|---|---|
| **Extend** | Existing entity/service can be augmented with new fields or methods | Add `OwnerId` to `Article` entity |
| **Wire** | Backend capability exists but is not connected to frontend or other modules | `Follow` entity exists but Favorites UI does not surface it |
| **Build** | New entities, services, controllers, or frontend views required from scratch | `Space` entity, `KnowledgeAgent` controller |

---

## Dependency Graph

```
Phase 0A: Spaces ─────────────────┬──► Phase 4A: Space Permissions
                                  ├──► Phase 4B: Delegated Admin
                                  ├──► Phase 4C: Guest Access
                                  ├──► Phase 4D: Personal Workspace
                                  ├──► Phase 2E: Permission-Aware AI
                                  └──► Phase 5A: Knowledge Agents

Phase 0B: Generalize Workflow ────┬──► Phase 1B: Verification Lifecycle
                                  └──► Phase 8F: Automation Rules

Phase 1A: Inline AI Writing ──────┬──► Phase 2A: AI Translation
                                  └──► Phase 2B: AI Content Generation

Phase 1B: Verification Lifecycle ─┬──► Phase 2F: Content Freshness
                                  ├──► Phase 2G: Health Dashboard
                                  └──► Phase 3E: Template Review Cycles

Phase 2E: Permission-Aware AI ────┬──► Phase 5A: Knowledge Agents
                                  └──► Phase 5B: Research Mode

Phase 3C: Article Export ─────────└──► Phase 9C: Bulk Export
```

---

## Phase 0 — Foundation (Weeks 1–4)

**Goal**: Establish structural primitives that many later features depend on.

### 0A. Spaces / Teamspaces Concept

**Gap**: #16 (P1) — No unified container grouping content across modules
**Classification**: Build
**Rationale**: Spaces are the #1 structural dependency. Gaps #15, #39, #11, and #20 all require a Space entity.

#### Backend (Content Module)

**New entities:**
```
Space
├── Id: Guid
├── Name: LocalizedString (EN/AR)
├── Description: LocalizedString
├── SpaceType: enum (Personal, Team, Department, Project, Public)
├── OwnerId: Guid
├── IconName: string
├── Color: string
├── IsPublic: bool
├── ParentSpaceId: Guid? (hierarchy)
├── SpacePermissions: ICollection<SpacePermission>
└── SpaceMembers: ICollection<SpaceMember>

SpacePermission
├── SpaceId, UserId?, GroupId?, RoleId?
└── AccessLevel: enum (Read, Write, Manage, Admin)

SpaceMember
├── SpaceId, UserId
├── Role: enum (Member, Editor, Admin, Owner)
└── JoinedAt: DateTime
```

**Entity modifications** (additive, nullable FK):
- `Article` → add `Guid? SpaceId`
- `DocumentLibrary` → add `Guid? SpaceId`
- `Community` → add `Guid? SpaceId`

**New controller**: `SpacesController` — CRUD, membership, content listing per space

#### Frontend

- `SpacesListView` — browseable space directory with type filters
- `SpaceDetailView` — tabbed layout (Overview, Articles, Documents, Discussions, Members, Settings)
- Sidebar restructured to show Spaces hierarchy
- Space selector on content creation forms

**Dependencies**: None (foundational)

---

### 0B. Generalize Workflow Engine

**Gap**: #14 (P1) — Workflow engine limited to service requests
**Classification**: Extend
**Rationale**: The `WorkflowDefinition` already supports Sequential, Parallel, Conditional, and StateMachine types with step assignment (Role, User, Group, RoundRobin, LeastBusy) and timeout escalation. It only needs to be decoupled from `ServiceRequestEntity`.

#### Backend (Workflow Module)

**New entities:**
```
WorkflowInstance
├── Id: Guid
├── WorkflowDefinitionId: Guid
├── TargetEntityType: string (Article, Document, ServiceRequest)
├── TargetEntityId: Guid
├── CurrentStepId: Guid?
├── Status: enum (Active, Completed, Cancelled, TimedOut)
├── StartedAt, CompletedAt
└── StepInstances: ICollection<WorkflowStepInstance>

WorkflowStepInstance
├── WorkflowInstanceId, StepDefinitionId
├── AssigneeId, AssigneeName
├── Status: enum (Pending, InProgress, Completed, Skipped, Escalated)
├── StartedAt, CompletedAt
├── Outcome: string
└── Comments: string
```

**Modifications:**
- Add `WorkflowScope` enum to `WorkflowDefinition`: ServiceRequest, ContentApproval, DocumentReview, KnowledgeVerification, Custom
- New `WorkflowEngineService` abstracting step progression, assignment resolution, timeout handling, and notification publishing
- Refactor `ServiceRequestEntity` to delegate to `WorkflowInstance` (backward-compatible — existing API unchanged)
- New MassTransit consumer: `WorkflowStepTimeoutConsumer` for delayed message scheduling

#### Frontend

- Reusable workflow builder component (visual step editor)
- Workflow status tracker component (reusable across content, documents, service requests)

**Dependencies**: None (foundational)

---

## Phase 1 — Critical Gaps (Weeks 3–8)

**Goal**: Close the 2 P0 gaps that fundamentally undermine KM value.

### 1A. Inline AI Writing Assistance in Content Editor

**Gap**: #1 / #7.1 (P0) — No AI writing assistant in the editor
**Classification**: Build
**Leverage**: Existing `IIntalioAIClient` (ChatAsync, ChatStreamAsync), `AIPromptTemplate`, `BlockEditorService`, `QASessionType.WritingAssistant`

#### Backend (AI Module)

**New controller**: `AIWritingAssistantController`

| Endpoint | Purpose |
|---|---|
| `POST /api/ai/writing/suggest` | Given block content + surrounding context, return improvement suggestions |
| `POST /api/ai/writing/generate` | Generate new block content from prompt (also addresses Gap #7) |
| `POST /api/ai/writing/improve` | Improve grammar, clarity, structure |
| `POST /api/ai/writing/change-tone` | Rewrite for different audience (formal, casual, simplified) |
| `POST /api/ai/writing/translate` | Translate block content EN↔AR (partially addresses Gap #6) |
| `POST /api/ai/writing/summarize` | Summarize selected blocks |
| `POST /api/ai/writing/continue` | Continue writing from current cursor position |
| `POST /api/ai/writing/stream` | SSE streaming for real-time output |

**Implementation details:**
- Create `AIPromptTemplate` records for each operation type with optimized system prompts
- Reuse existing `IIntalioAIClient.ChatStreamAsync` for streaming responses
- Track usage via existing `AIJob` entity with new `AIJobType.WritingAssistance`
- Respect `QASession` tracking for conversation continuity within editing session

#### Frontend

- **`AIWritingToolbar`** component — floating toolbar appearing on text selection with actions: Improve, Simplify, Translate, Change Tone, Expand, Summarize
- **Slash command** (`/ai`) in block editor — type `/ai` followed by prompt for inline generation
- **`AIWritingSidebar`** panel — slide-over for full article operations (translate entire article, rewrite, outline generation)
- **Streaming rendering** — AI output streams token-by-token into the editor
- **Keyboard shortcuts**: `Ctrl+Space` (suggestions), `Ctrl+Shift+T` (translate block)
- **Accept/reject UI** — AI suggestions shown as diff with accept/reject buttons

**Dependencies**: Existing `IIntalioAIClient`, `BlockEditorService`

---

### 1B. Knowledge Verification Lifecycle

**Gap**: #2 / #4.1 (P0) — No content verification system
**Classification**: Build + Extend
**Leverage**: Existing `Article` entity, `NotificationType` enum, notification worker, `ArticleStatus` workflow

#### Backend (Content Module)

**Entity modifications to `Article`:**
```
+ OwnerId: Guid?
+ OwnerName: string?
+ LastVerifiedAt: DateTime?
+ NextVerificationDue: DateTime?
+ VerificationStatus: enum (Unverified, Verified, DueSoon, Overdue)
+ ReviewIntervalDays: int? (default 90)
```

**New entity:**
```
VerificationRecord
├── Id: Guid
├── ArticleId: Guid
├── VerifiedById: Guid
├── VerifiedByName: string
├── VerifiedAt: DateTime
├── Notes: string?
├── PreviousStatus: VerificationStatus
├── NewStatus: VerificationStatus
└── ExpiresAt: DateTime?
```

**New endpoints on `ArticlesController`:**
| Endpoint | Purpose |
|---|---|
| `POST /api/content/articles/{id}/verify` | Mark as verified, set next due date |
| `POST /api/content/articles/{id}/assign-owner` | Assign/reassign content owner |
| `GET /api/content/articles/verification-dashboard` | Aggregated verification metrics |
| `GET /api/content/articles/overdue` | All overdue articles |
| `GET /api/content/articles/due-soon` | Articles due for verification within 7 days |

**Background job:**
- `KnowledgeVerificationReminderJob` — runs daily, queries articles where `NextVerificationDue <= DateTime.UtcNow + 7days`
- Publishes `VerificationReminderMessage` to notification queue
- New `NotificationType` values: `VerificationDue`, `VerificationOverdue`, `VerificationCompleted`
- Escalation: if owner doesn't verify within 7 days of due date, notify owner's manager (using `User.ManagerId`)

**Integration with Workflow Engine (Phase 0B):**
- Optional: verification can trigger a review workflow (`WorkflowScope.KnowledgeVerification`)
- Configurable per template: some content types auto-verify, others require formal review

#### Frontend

- **Verification badge component** — green ✓ (verified), yellow ⚠ (due soon), red ✗ (overdue), gray ○ (unverified)
- Badge displayed: on article cards, in search results, on article detail header, in navigation
- **Owner assignment dialog** — user search with role suggestions
- **One-click verify button** — on article detail page for the assigned owner
- **Verification dashboard view** — filterable by status, department, owner, space
- **Template integration** — templates can preset `ReviewIntervalDays` and default owner (connects to Gap #13)

**Dependencies**: Phase 0B (workflow generalization)

---

## Phase 2 — AI & Search Integration (Weeks 5–12)

**Goal**: Make AI pervasive throughout the application rather than siloed in dedicated views.

### 2A. General AI Translation for Articles/Documents

**Gap**: #6 (P1)
**Classification**: Extend
**Leverage**: Existing `TranscriptionController /translate` (EN↔AR), `BlockEditorService`, `IIntalioAIClient`

#### Backend (AI Module)

**New controller**: `TranslationController`

| Endpoint | Purpose |
|---|---|
| `POST /api/ai/translation/article/{id}` | Translate all blocks of an article EN→AR or AR→EN |
| `POST /api/ai/translation/blocks` | Translate a set of content blocks |
| `POST /api/ai/translation/text` | Translate arbitrary text |

**Implementation:**
- Iterate through `ContentBlock` collection, translate each block's content using `IIntalioAIClient`
- Populate the corresponding bilingual field (`Content` ↔ `ContentArabic`)
- Use `AIPromptTemplate` with translation-specific system prompts that preserve formatting (headings, lists, code blocks unchanged)
- Track block translation status: Original / Machine-Translated / Human-Verified
- New `AIJobType.Translation` value

#### Frontend

- "Translate to Arabic/English" button on article editor toolbar
- Per-block translation status indicator
- Side-by-side preview mode (original | translated)
- Batch translate option for space-wide translation

**Dependencies**: Phase 1A (writing assistant provides per-block infrastructure)

---

### 2B. AI Content Generation from Prompts

**Gap**: #7 (P1)
**Classification**: Extend Phase 1A
**Leverage**: `AIWritingAssistantController.GenerateAsync` from Phase 1A

#### Backend (AI Module)

- Extend `AIWritingAssistantController`: `POST /api/ai/writing/generate-article` — given topic/prompt/outline + optional template, generate a complete article with ContentBlocks
- Integrate with `BlockEditorService.CreateBlockAsync` to persist generated blocks
- Support template-aware generation: given a `DocumentTemplate`, fill in template structure with generated content

#### Frontend

- "New from AI" button on article creation page
- Multi-step wizard: Topic input → Outline preview/edit → Generate → Edit result
- Template selector for structured generation

**Dependencies**: Phase 1A

---

### 2C. Q&A Integrated into Main Search Bar

**Gap**: #5 (P1)
**Classification**: Wire
**Leverage**: `SearchController`, `SemanticSearchController.AskQuestion()` (already returns answers with citations, confidence, follow-ups)

#### Backend (Search Module)

- Extend `SearchController`: add `POST /api/search/unified` accepting query + `includeAIAnswer: bool`
- When `includeAIAnswer` is true and query looks like a question (heuristic: contains question words or ends with `?`), also call `IRAGService.QueryAsync`
- Return unified response: `{ aiAnswer: QuestionAnsweringResponse?, searchResults: PagedResult<SearchResultDto> }`

#### Frontend

- Detect question-like queries in search input
- Display AI answer card above search results with: answer text, confidence score, source citations (clickable), "Was this helpful?" feedback
- "Ask AI" toggle button in search bar
- Smooth fallback to standard search when AI answer confidence is low

**Dependencies**: Existing `IRAGService`, `SearchController`

---

### 2D. AI Accessible in the Flow of Work

**Gap**: #8 (P1)
**Classification**: Build (frontend-heavy)
**Leverage**: Existing `AIController` chat endpoints

#### Frontend

- **Global AI button** — floating bottom-right button on every page
- **`AIAssistantPanel`** — slide-over chat panel with context awareness:
  - When opened from article page: auto-injects article content as context
  - When opened from document view: includes document metadata and extracted text
  - When opened from search: includes current search query
- **Quick actions** based on context:
  - On article: "Summarize this page", "Translate", "Find related content", "Explain"
  - On document: "What's in this document?", "Summarize", "Extract key points"
  - On dashboard: "What's new since yesterday?", "My overdue tasks"
- **Keyboard shortcut**: `Ctrl+J` to open/close AI panel
- **Pinnable**: can be pinned open as a sidebar

**Dependencies**: Existing `AIController`

---

### 2E. Permission-Aware AI / RAG Verification

**Gap**: #9 (P1)
**Classification**: Extend
**Leverage**: `IndexDocumentRequest.AllowedRoleIds/GroupIds/UserIds`, `PermissionService.HasDocumentAccessAsync()`

#### Backend (AI Module)

- In `RAGService.QueryAsync`, after retrieving candidate document chunks:
  1. Extract `SourceEntityId` and `SourceEntityType` from each `VectorEmbedding`
  2. For documents: call `IPermissionService.HasDocumentAccessAsync(userId, documentId)` to filter
  3. For articles in spaces: check `SpacePermission` (from Phase 0A)
  4. Remove unauthorized chunks before passing to LLM context
- Add metadata to `VectorEmbedding` entity: `SpaceId`, `LibraryId`, `IsPublic` for pre-filtering during vector similarity search
- Re-index existing embeddings with permission metadata (one-time migration script)

**Dependencies**: Phase 0A (Spaces provide SpaceId for permission scoping)

---

### 2F. Content Freshness Tracking + Content Health Metrics

**Gaps**: #3 (P1), #17 (P1)
**Classification**: Extend
**Leverage**: `Article.CreatedAt`, `ModifiedAt`, `PublishedAt`, `ViewCount`, Phase 1B verification status

#### Backend (Content Module)

**Entity modification to `Article`:**
```
+ HealthScore: double? (0.0 – 1.0)
+ HealthScoreCalculatedAt: DateTime?
```

**Health score algorithm** (computed by background job):
```
HealthScore = weighted average of:
  - Freshness (0.3): based on DaysSinceLastUpdate vs ReviewIntervalDays
  - Verification (0.3): Verified=1.0, DueSoon=0.7, Overdue=0.2, Unverified=0.0
  - Engagement (0.2): ViewCount trend (increasing=1.0, flat=0.5, declining=0.2)
  - Completeness (0.1): has owner, has tags, has category, has bilingual content
  - Quality (0.1): QA review score if available
```

**Background job**: `ContentHealthCalculationJob` — runs weekly, calculates and caches scores

**New endpoints:**
| Endpoint | Purpose |
|---|---|
| `GET /api/content/health` | Health metrics aggregated by space/department/category |
| `GET /api/content/health/stale` | Articles with HealthScore < 0.4 |
| `GET /api/content/health/top` | Highest-performing content |

**Dependencies**: Phase 1B (verification status)

---

### 2G. Knowledge Health Dashboard

**Gap**: #4 (P1)
**Classification**: Build
**Leverage**: Phase 2F health scores, existing `KpiService`, search analytics

#### Frontend

- **`KnowledgeHealthDashboardView`** with sections:
  - **Verification Status** — pie chart: Verified / Due Soon / Overdue / Unverified
  - **Content Freshness** — heatmap by age (0–30d green, 30–90d yellow, 90d+ red)
  - **Stale Content Table** — sortable list of lowest health score articles with owner and last update
  - **Coverage Gaps** — categories/spaces with no recent content
  - **Top Contributors** — leaderboard of active content creators/verifiers
  - **Orphaned Content** — articles with no owner assignment
  - **Health Trend** — line chart showing overall health score over time

**Dependencies**: Phase 2F

---

## Phase 3 — Collaboration & Content Export (Weeks 9–16)

### 3A. Inline Comments on Article Text

**Gap**: #12 (P1)
**Classification**: Extend
**Leverage**: Existing `Comment` entity, `CollaborationHub` (SignalR)

#### Backend (Collaboration Module)

**Entity modification to `Comment`:**
```
+ AnchorBlockId: Guid? (references ContentBlock.Id)
+ AnchorStartOffset: int? (character offset within block text)
+ AnchorEndOffset: int?
+ AnchorQuotedText: string? (the highlighted text — for resilience if block changes)
```

**New endpoints on `CommentsController`:**
| Endpoint | Purpose |
|---|---|
| `GET /api/collaboration/comments/inline/{articleId}` | Returns comments grouped by block |
| `POST /api/collaboration/comments/inline` | Create inline comment with anchor data |

**Real-time**: Broadcast inline comment creation via existing `CollaborationHub`

#### Frontend

- Text selection in article body triggers "Comment" popover
- Comment thread sidebar anchored to highlighted text with connecting line
- Yellow highlight on commented text ranges
- Resolve/unresolve inline comments (using existing `ReviewComment.IsResolved` pattern)
- Inline comment count badge on article cards

**Dependencies**: Existing `Comment` entity, `CollaborationHub`

---

### 3B. Article Export to PDF/DOCX/Markdown

**Gap**: #19 (P1)
**Classification**: Extend
**Leverage**: `SummarizationController.ExportMeetingMinutes()` (DOCX), `BlockEditorService`, existing OpenXml dependency

#### Backend (Content Module)

**New service**: `ArticleExportService`
- `ExportToHtmlAsync(articleId, language)` — uses existing `BlockEditorService` rendering
- `ExportToMarkdownAsync(articleId, language)` — new `RenderToMarkdownAsync` method on BlockEditorService
- `ExportToDocxAsync(articleId, language)` — convert HTML to DOCX using `DocumentFormat.OpenXml` (same approach as meeting minutes export)
- `ExportToPdfAsync(articleId, language)` — HTML-to-PDF via `QuestPDF` or `PuppeteerSharp`
- Support RTL layout for Arabic exports

**New endpoint**: `GET /api/content/articles/{id}/export?format=pdf|docx|md&language=en|ar|both`

#### Frontend

- Export dropdown button on article detail and editor views
- Format and language selection dialog
- Download progress indicator

**Dependencies**: Existing `BlockEditorService`, OpenXml NuGet

---

### 3C. Favorites/Bookmarks UI

**Gap**: #21 (P2)
**Classification**: Wire (frontend-only)
**Leverage**: Existing `Follow` entity with `FollowableType` (Article, Document, Discussion, Community, Tag, Category, User)

#### Backend

- Add dedicated endpoint: `GET /api/collaboration/follows/my-favorites` — aggregates followed items with title, type, thumbnail, last updated

#### Frontend

- Star/bookmark icon on article cards, document cards, discussion cards
- "My Favorites" section in sidebar
- Favorites page with type filtering and sorting

**Dependencies**: Existing `Follow` entity

---

### 3D. Templates with Review Cycles and Default Owners

**Gap**: #13 (P1)
**Classification**: Extend
**Leverage**: Existing `TemplateMetadata` (ComplianceLevel, RequiresApproval, ApprovalRoles)

#### Backend (WebApi Templates Feature)

**Extend `TemplateMetadata`:**
```
+ ReviewIntervalDays: int?
+ DefaultOwnerId: Guid?
+ DefaultOwnerRole: string? (e.g., "Department Head" — resolves dynamically)
+ VerificationWorkflowId: Guid? (link to WorkflowDefinition)
```

When creating article from template: auto-populate `Article.ReviewIntervalDays`, `Article.OwnerId` from template defaults

#### Frontend

- Template editor: review cycle configuration section
- Owner assignment field with user search or role-based dynamic assignment

**Dependencies**: Phase 1B (Article verification fields)

---

## Phase 4 — Spaces, Permissions & Access (Weeks 13–20)

### 4A. Space-Level Permissions

**Gap**: #15 (P1)
**Classification**: Build
**Leverage**: Phase 0A `SpacePermission` entity

#### Backend

- New `SpaceAuthorizationHandler` implementing `IAuthorizationHandler`
- Extend `ArticlesController`, `DocumentsController`, `DiscussionsController` to check space-level permissions before content operations
- Permission inheritance: Space permission cascades to all content within unless explicitly overridden

#### Frontend

- Space settings page: member management with role assignment (Member, Editor, Admin)
- Permission matrix UI

**Dependencies**: Phase 0A

---

### 4B. Guest/External User Access

**Gap**: #11 (P1)
**Classification**: Build
**Leverage**: Existing `User` entity, `DocumentLibrary.IsPublic`

#### Backend (Identity Module)

**Entity modification to `User`:**
```
+ UserType: enum (Internal, Guest, External, ServiceAccount)
+ GuestExpiresAt: DateTime?
+ InvitedById: Guid?
+ MaxAccessLevel: enum (View, Comment, Edit)
```

**New controller**: `GuestAccessController`
| Endpoint | Purpose |
|---|---|
| `POST /api/identity/guests/invite` | Generate invite with scoped permissions |
| `GET /api/identity/guests` | List active guests |
| `DELETE /api/identity/guests/{id}` | Revoke guest access |

**New auth scheme**: `GuestTokenHandler` — short-lived JWT with restricted claims (SpaceIds, DocumentIds, MaxAccessLevel)

#### Frontend

- "Share with external" dialog with email input, permission level, expiration
- Guest landing page with limited navigation
- Admin view: guest management

**Dependencies**: Phase 0A (Spaces for scoping)

---

### 4C. Delegated Space Administration

**Gap**: #39 (P2)
**Classification**: Extend
**Leverage**: Phase 0A `SpaceMember.Role` (Admin)

#### Backend

- Space admins (SpaceMember.Role == Admin) can: manage space members, update space settings, manage space-level content permissions, view space analytics
- Restrict to space scope — cannot affect global settings

**Dependencies**: Phase 4A

---

### 4D. Personal Workspace

**Gap**: #20 (P2)
**Classification**: Extend Phase 0A

- Auto-create a `Space` with `SpaceType.Personal` on user creation
- Personal space: private by default, not visible in space directory
- Contains: draft articles, bookmarked items, personal notes, recent activity

**Dependencies**: Phase 0A, Phase 3C (favorites)

---

## Phase 5 — Advanced AI (Weeks 17–24)

### 5A. AI Knowledge Agents

**Gap**: #10 (P1)
**Classification**: Build
**Leverage**: Existing `IRAGService`, `QASession`, `AIPromptTemplate`

#### Backend (AI Module)

**New entity:**
```
KnowledgeAgent
├── Id: Guid
├── Name: LocalizedString
├── Description: LocalizedString
├── SystemPrompt: string
├── AvatarUrl: string?
├── ScopedSpaceIds: List<Guid>?
├── ScopedLibraryIds: List<Guid>?
├── ScopedEntityTypes: List<string>?
├── AllowedRoleIds: List<Guid>?
├── IsActive: bool
├── CreatedById: Guid
└── UsageCount: int
```

**New controller**: `KnowledgeAgentsController`
| Endpoint | Purpose |
|---|---|
| `GET /api/ai/agents` | List available agents |
| `POST /api/ai/agents` | Create agent (admin) |
| `POST /api/ai/agents/{id}/chat` | Chat with agent (scoped RAG) |
| `POST /api/ai/agents/{id}/chat/stream` | Streaming chat |
| `GET /api/ai/agents/{id}/analytics` | Agent usage and performance |

**Implementation**:
- In `RAGService.QueryAsync`, accept optional `KnowledgeAgentId` to restrict embedding search scope
- Use agent's `SystemPrompt` as the AI system message
- Enforce agent's `AllowedRoleIds` and `ScopedSpaceIds` for permission filtering
- Track all interactions in `QASession` with `AgentId` field

**Suggested default agents:**
| Agent | Scope | Purpose |
|---|---|---|
| Tournament Operations | Tournament space | Answer questions about tournament planning, logistics, scheduling |
| HR Assistant | HR space | Policies, benefits, onboarding procedures |
| IT Helpdesk | IT space | Technical troubleshooting, system guides |
| Document Finder | All documents | Help find specific documents across libraries |

#### Frontend

- Agent gallery view with department-specific agent cards
- Chat interface per agent (reuse existing AI chat components)
- Agent creation/configuration admin page
- Agent performance dashboard

**Dependencies**: Phase 0A (Spaces for scoping), Phase 2E (permission-aware RAG)

---

### 5B. AI Research Mode

**Gap**: #29 (P2)
**Classification**: Build
**Leverage**: `QASession`, `SmartSuggestion`

#### Backend (AI Module)

- New `ResearchSession` entity extending `QASession` with: `ResearchTopic`, `List<ResearchSource>`, `SynthesizedReport`, `Status` (Gathering, Analyzing, Synthesizing, Complete)
- `POST /api/ai/research/start` — initiates multi-step research
- Background processing: gather relevant chunks across spaces, cross-reference, synthesize structured report with sections and citations
- `POST /api/ai/research/{id}/save-as-article` — convert research report to draft article

**Dependencies**: Phase 2E

---

### 5C. AI Data Retention Policy Documentation

**Gap**: #18 (P1)
**Classification**: Extend

#### Backend (Admin Module)

- New entity: `AIDataRetentionPolicy` with `ConversationRetentionDays`, `EmbeddingRetentionDays`, `AutoDeleteEnabled`
- New background job: `AIDataRetentionJob` — purges old conversations and AI job records per policy
- Admin endpoint: `GET/PUT /api/admin/ai-retention-policy`
- Document Intalio AI service data handling in API documentation

**Dependencies**: Existing `Conversation`, `AIJob` entities

---

## Phase 6 — Analytics & Integration (Weeks 21–28)

### 6A. Federated Search Activation

**Gap**: #28 (P2)
**Classification**: Wire
**Leverage**: Existing `M365Controller` (SharePoint, OneDrive), `IntegrationConnector`

- New `FederatedSearchService` dispatching parallel search to configured connectors
- Extend `SearchController.Search` with `includeExternal: bool` parameter
- Normalize external results into `SearchResultDto` with source indicator

---

### 6B. Cross-Module Recent Activity Feed

**Gap**: #22 (P2)
**Classification**: Build

**New entity**: `ActivityEntry` — `ActorId`, `ActivityType`, `EntityType`, `EntityId`, `Description`, `OccurredAt`, `SpaceId`

**Implementation**: MassTransit consumers listen to existing domain events (ArticlePublished, DocumentUploaded, CommentAdded, etc.) and persist `ActivityEntry` records

**New endpoint**: `GET /api/activity/feed?spaceId=&userId=&limit=`

---

### 6C. Unified Adoption Analytics Dashboard

**Gap**: #35 (P2)
**Classification**: Extend
**Leverage**: `AuditLogEntry`, `SearchAnalyticsDto`, `AIAnalyticsDto`, `ViewCount`

- New endpoint: `GET /api/admin/analytics/adoption` — aggregates active users, content created/updated, search usage, AI usage, top contributors, department activity
- Frontend: Analytics dashboard with charts, tables, and drill-down

---

### 6D. Analytics Export/API + Scheduled Reports

**Gaps**: #36, scheduled reports (P2, P3)
- Add `GET /api/admin/analytics/export?format=csv|xlsx` endpoints
- Add scheduled email delivery using existing notification worker cron capability

---

### 6E. Controlled Vocabulary for Tags

**Gap**: #25 (P2)
**Classification**: Extend
**Leverage**: Existing `Tag` entity, `TagsController`

**Entity modification to `Tag`:**
```
+ IsApproved: bool (default true for admin-created, false for user-created)
+ ApprovedById: Guid?
+ Synonyms: List<string>?
+ ParentTagId: Guid? (hierarchical taxonomy)
```

Admin tag governance endpoints. Suggested tags during content creation.

---

### 6F. Bi-directional Backlinks

**Gap**: #26 (P2)
**Classification**: Build

**New entity**: `ContentLink` — `SourceEntityId`, `SourceEntityType`, `TargetEntityId`, `TargetEntityType`, `LinkType`

**Implementation**: When saving article blocks, parse for internal links → upsert `ContentLink` records

**New endpoint**: `GET /api/content/{type}/{id}/backlinks`

**Frontend**: "Linked from" section on article detail page

---

## Phase 7 — Admin, Compliance & External (Weeks 25–32)

### 7A. SCIM Endpoint

**Gap**: #37 (P2)
**Classification**: Build (Identity Module)

- New `ScimController` implementing SCIM 2.0 protocol (`/scim/v2/Users`, `/scim/v2/Groups`)
- Map SCIM schema to existing `User` and `Group` entities
- Bearer token authentication for SCIM clients

---

### 7B. DLP / Sensitive Content Detection

**Gap**: #38 (P2)
**Classification**: Build
**Leverage**: Existing `DocumentQuarantine`, `IIntalioAIClient`

- New `DLPService` using AI to scan content for PII, credentials, financial data
- New entities: `DLPPolicy`, `DLPScanResult`
- Integration: auto-quarantine on policy violation (using existing quarantine workflow)
- Background consumer: `DLPScanConsumer` triggered on document upload/article publish

---

### 7C. Unified Admin Dashboard

**Gap**: #40 (P2)
**Classification**: Wire (frontend-heavy)

- Consolidate: user management, audit logs, system health, knowledge health (Phase 2G), AI usage, storage metrics, security alerts
- Single-page dashboard with widget layout

---

### 7D. Per-Item Shareable Links

**Gaps**: #32, #48 (P2)
**Classification**: Build

**New entity:**
```
ShareableLink
├── EntityId: Guid
├── EntityType: string
├── Token: string (URL-safe unique)
├── ExpiresAt: DateTime?
├── CreatedById: Guid
├── Permission: enum (View, Comment)
├── RequiresAuth: bool
├── Password: string? (hashed)
└── AccessCount: int
```

**New controller**: `ShareController` — `POST /create-link`, `GET /shared/{token}`

**Dependencies**: Phase 4B (guest access for external links)

---

## Phase 8 — UX Enhancements (Weeks 29–36)

### 8A. @Mentions in Article Body Editor

**Gap**: #31 (P2)
**Classification**: Extend
**Leverage**: Existing `Mention` entity

- Make `Mention.CommentId` nullable, add `Mention.BlockId: Guid?` and `MentionableType` enum
- `@` trigger in block editor opens user search popup
- Mention rendered as linked chip within block text
- Notification published on mention

---

### 8B. Smart Link Previews

**Gap**: #27 (P2)
**Classification**: Build

- New endpoint: `GET /api/content/link-preview?entityType=&entityId=` returning title, snippet, thumbnail, type, author
- Frontend: hover popover component on internal links

---

### 8C. Proactive Recommendation Surfacing

**Gap**: #44 (P2)
**Classification**: Wire
**Leverage**: Existing `SmartSuggestion` entity

- Background job computing personalized suggestions based on user activity
- `GET /api/ai/suggestions/for-me` endpoint
- "Recommended for you" section on dashboard
- "Related content" sidebar on article detail pages

---

### 8D. Version Rollback UX

**Gap**: #30 (P2)
- New endpoint: `POST /api/content/articles/{id}/rollback/{versionNumber}`
- Frontend: version comparison view with diff highlighting, one-click rollback

---

### 8E. User-Configurable Automation Rules

**Gap**: #33 (P2)
**Classification**: Build
**Leverage**: Existing 22 webhook event types, notification delivery channels

**New entity:**
```
AutomationRule
├── Id, OwnerId, SpaceId?
├── Name: string
├── TriggerEvent: string (from webhook events)
├── Conditions: List<RuleCondition> (property, operator, value)
├── Actions: List<RuleAction> (type, config)
├── IsActive: bool
└── ExecutionCount: int
```

`AutomationEngineService` evaluates rules when domain events fire.

**Dependencies**: Phase 0B (workflow engine)

---

### 8F. Org Chart Visualization

**Gap**: #45 (P2)
**Classification**: Wire
**Leverage**: Existing `User.ManagerId`, `User.DirectReports`, `Department` hierarchy

- New endpoint: `GET /api/identity/org-chart?rootDepartmentId=` returning tree structure
- Frontend: interactive org chart (PrimeVue OrganizationChart or D3.js)

---

### 8G. Action Buttons in Content Views

**Gap**: #34 (P2)
**Classification**: Wire
**Leverage**: Existing `NotificationAction` pattern

- Contextual action bar on article detail views based on user role and content status
- Actions: Verify, Submit for Review, Approve, Assign Owner, Translate, Export

---

## Phase 9 — External Channels (Weeks 33–40)

### 9A. Teams/Slack Q&A Bot

**Gap**: #43 (P2)
**Classification**: Build
**Leverage**: Existing `IRAGService`, Slack/Teams notification channels

- New `BotController` handling incoming messages from Teams/Slack
- Route questions to `IRAGService.QueryAsync`, return answers with citations
- Webhook registration endpoints for bot frameworks
- The AI and delivery channel infrastructure both exist — this is primarily a connection/integration task

---

### 9B. Native Mobile / PWA

**Gap**: #41 (P2)

- PWA approach: add `workbox` service worker to existing Vue.js frontend
- Manifest file, offline caching, push notification registration
- Native apps: consider Capacitor/Ionic wrapper around Vue.js frontend

---

### 9C. Browser Extension

**Gap**: #42 (P2)

- Chrome/Edge extension with popup search and AI chat
- Connect to existing search and AI endpoints
- Knowledge triggers based on URL patterns

---

### 9D. Article Import Beyond HTML + Bulk Export

**Gaps**: #46, #47 (P2)
**Classification**: Extend
**Leverage**: Existing `BlockEditorController.ImportFromHtml()`, `ArticleExportService` from Phase 3B

- DOCX import: parse with `DocumentFormat.OpenXml` → convert to blocks
- Markdown import: parse with `Markdig` NuGet → convert to blocks
- Bulk export: batch the Phase 3B export for multiple articles (ZIP)

---

## Phase 10 — Low Priority & Continuous (Weeks 37+)

| Gap | Feature | Approach | Effort |
|---|---|---|---|
| 2.3 | Whiteboard/Canvas | New drawing module or embed third-party (Excalidraw) | Large |
| 2.4 | General Form Builder | Extend existing `Form` entity in Workflow module | Medium |
| 3.2 | Synced Blocks | Extend `ContentBlock` with `SyncSourceBlockId` | Medium |
| 5.4 | Time-weighted Trending | Extend search analytics scoring | Small |
| 13.1 | Quiet Publish Mode | Add `IsQuietPublish` flag to `Article.Publish()` | Small |
| 9.3 | Emoji Reactions Expansion | Extend `ReactionType` from string "like" to emoji set | Small |
| 17.1 | Embeddable Widgets | New public-facing widget endpoints with CORS | Medium |
| 20.3 | Offline Access | Service worker with IndexedDB sync | Medium |
| 18.5 | SIEM Connectors | Serilog sinks for Splunk/Datadog | Small |
| 18.4 | IP Allowlisting | Middleware for IP filtering | Small |
| 11.3 | Recurring Templates | Cron-based template instantiation job | Small |
| 11.2 | Template Contextual Buttons | "Create from Template" buttons on pages | Small |
| 2.1 | Knowledge Cards | Lightweight card entity or article subtype | Medium |
| 2.2 | User-Facing Databases | Major feature — structured data with views | Large |
| 10.2 | AI Comment Analysis | Apply sentiment analysis to comment threads | Small |
| 10.3 | Comments Sidebar Panel | Unified filterable comments panel | Small |
| 15.4 | Scheduled Analytics Reports | Cron-based email delivery using notification worker | Small |

---

## Database Migration Strategy

### Principles

1. **All migrations are additive** — new tables and nullable columns only
2. **No breaking schema changes** — existing API consumers unaffected
3. **Migrations are idempotent** — safe to re-run
4. **Data migrations run separately** — re-indexing embeddings (Phase 2E) runs as a background job, not a schema migration

### Migration Sequence

| Phase | Migration | Changes |
|---|---|---|
| 0A | `AddSpaces` | New tables: `Space`, `SpacePermission`, `SpaceMember`. Add nullable `SpaceId` FK to `Article`, `DocumentLibrary`, `Community`. |
| 0B | `GeneralizeWorkflow` | New tables: `WorkflowInstance`, `WorkflowStepInstance`. Add `WorkflowScope` to `WorkflowDefinition`. |
| 1B | `AddVerification` | Add to `Article`: `OwnerId`, `OwnerName`, `LastVerifiedAt`, `NextVerificationDue`, `VerificationStatus`, `ReviewIntervalDays`. New table: `VerificationRecord`. |
| 2F | `AddHealthScore` | Add to `Article`: `HealthScore`, `HealthScoreCalculatedAt`. |
| 3A | `AddInlineComments` | Add to `Comment`: `AnchorBlockId`, `AnchorStartOffset`, `AnchorEndOffset`, `AnchorQuotedText`. |
| 3D | `ExtendTemplateMetadata` | Add to `TemplateMetadata`: `ReviewIntervalDays`, `DefaultOwnerId`, `DefaultOwnerRole`, `VerificationWorkflowId`. |
| 4B | `AddGuestUsers` | Add to `User`: `UserType`, `GuestExpiresAt`, `InvitedById`, `MaxAccessLevel`. |
| 5A | `AddKnowledgeAgents` | New table: `KnowledgeAgent`. Add `AgentId` to `QASession`. |
| 6E | `ExtendTags` | Add to `Tag`: `IsApproved`, `ApprovedById`, `Synonyms`, `ParentTagId`. |
| 6F | `AddContentLinks` | New table: `ContentLink`. |
| 7B | `AddDLP` | New tables: `DLPPolicy`, `DLPScanResult`. |
| 7D | `AddShareableLinks` | New table: `ShareableLink`. |
| 8E | `AddAutomationRules` | New table: `AutomationRule`, `RuleCondition`, `RuleAction`. |

---

## Module Boundary Decisions

| Feature | Module | Rationale |
|---|---|---|
| Spaces | Content | Spaces are the primary content container |
| Inline AI Writing | AI | AI service boundary |
| Knowledge Verification | Content | Article lifecycle extension |
| Translation | AI | AI-powered service |
| Inline Comments | Collaboration | Comments are collaboration domain |
| Knowledge Agents | AI | AI domain |
| Activity Feed | WebApi Feature | Cross-cutting, not module-specific |
| DLP | Admin | Compliance/governance domain |
| Automation Rules | Workflow | Workflow automation domain |
| SCIM | Identity | User provisioning domain |
| Shareable Links | Content | Content sharing domain |
| Guest Access | Identity | User identity domain |
| Export | Content | Content output |

---

## Effort Summary

| Phase | Weeks | Backend (days) | Frontend (days) | Total |
|---|---|---|---|---|
| 0: Foundation | 1–4 | 20 | 15 | 35 |
| 1: Critical | 3–8 | 25 | 30 | 55 |
| 2: AI & Search | 5–12 | 30 | 25 | 55 |
| 3: Collaboration | 9–16 | 20 | 25 | 45 |
| 4: Access Control | 13–20 | 25 | 15 | 40 |
| 5: Advanced AI | 17–24 | 25 | 20 | 45 |
| 6: Analytics | 21–28 | 20 | 20 | 40 |
| 7: Admin & Compliance | 25–32 | 25 | 15 | 40 |
| 8: UX Enhancements | 29–36 | 20 | 25 | 45 |
| 9: External | 33–40 | 25 | 20 | 45 |
| 10: Low Priority | 37+ | 20 | 20 | 40 |
| **Total** | **~40 weeks** | **~255 days** | **~230 days** | **~485 days** |

> **Note**: Phases overlap significantly. With parallel execution, the total calendar time is approximately 40 weeks, not the sum of all phase durations.

---

## Gap Traceability Matrix

| Gap # | Description | Priority | Phase | Classification |
|---|---|---|---|---|
| 1 | Inline AI writing in editor | P0 | 1A | Build |
| 2 | Knowledge verification lifecycle | P0 | 1B | Build + Extend |
| 3 | Content freshness tracking | P1 | 2F | Extend |
| 4 | Knowledge health dashboard | P1 | 2G | Build |
| 5 | Q&A in main search bar | P1 | 2C | Wire |
| 6 | General AI translation (EN↔AR) | P1 | 2A | Extend |
| 7 | AI content generation | P1 | 2B | Extend |
| 8 | AI in flow of work | P1 | 2D | Build (FE) |
| 9 | Permission-aware AI | P1 | 2E | Extend |
| 10 | AI knowledge agents | P1 | 5A | Build |
| 11 | Guest/external access | P1 | 4B | Build |
| 12 | Inline comments on text | P1 | 3A | Extend |
| 13 | Template review cycles/owners | P1 | 3D | Extend |
| 14 | Generalize workflow engine | P1 | 0B | Extend |
| 15 | Space-level permissions | P1 | 4A | Build |
| 16 | Unified spaces concept | P1 | 0A | Build |
| 17 | Content health metrics | P1 | 2F | Extend |
| 18 | AI data retention policy | P1 | 5C | Extend |
| 19 | Article export (PDF/DOCX/MD) | P1 | 3B | Extend |
| 20 | Personal workspace | P2 | 4D | Extend |
| 21 | Favorites UI | P2 | 3C | Wire |
| 22 | Cross-module activity feed | P2 | 6B | Build |
| 23 | Knowledge cards format | P2 | 10 | Build |
| 24 | User-facing databases | P2 | 10 | Build |
| 25 | Controlled tag vocabulary | P2 | 6E | Extend |
| 26 | Bi-directional backlinks | P2 | 6F | Build |
| 27 | Smart link previews | P2 | 8B | Build |
| 28 | Federated search activation | P2 | 6A | Wire |
| 29 | AI research mode | P2 | 5B | Build |
| 30 | Version rollback UX | P2 | 8D | Wire |
| 31 | @mentions in article body | P2 | 8A | Extend |
| 32 | Per-item shareable links | P2 | 7D | Build |
| 33 | User-configurable automation | P2 | 8E | Build |
| 34 | Action buttons in content views | P2 | 8G | Wire |
| 35 | Unified adoption analytics | P2 | 6C | Extend |
| 36 | Analytics export/API | P2 | 6D | Extend |
| 37 | SCIM endpoint | P2 | 7A | Build |
| 38 | DLP/sensitive content detection | P2 | 7B | Build |
| 39 | Delegated space admin | P2 | 4C | Extend |
| 40 | Unified admin dashboard | P2 | 7C | Wire |
| 41 | Mobile PWA | P2 | 9B | Build |
| 42 | Browser extension | P2 | 9C | Build |
| 43 | Teams/Slack Q&A bot | P2 | 9A | Build |
| 44 | Proactive recommendations | P2 | 8C | Wire |
| 45 | Org chart visualization | P2 | 8F | Wire |
| 46 | Article import (DOCX/MD) | P2 | 9D | Extend |
| 47 | Bulk export | P2 | 9D | Extend |
| 48 | External shareable links | P2 | 7D | Build |
| 49 | Template buttons/recurring | P3 | 10 | Extend |
