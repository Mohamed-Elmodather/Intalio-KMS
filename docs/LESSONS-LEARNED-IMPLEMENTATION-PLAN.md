# Lessons Learned Enhancement & ISO 30401 Compliance — Implementation Plan

A detailed, code-level implementation plan for enhancing the AFC27 KMS Lessons Learned subsystem to industry best practices and ISO 30401 compliance.

---

## Table of Contents

1. [Current State Summary](#1-current-state-summary)
2. [Target State Vision](#2-target-state-vision)
3. [Architecture Decisions](#3-architecture-decisions)
4. [Phase 1 — Action Tracking Foundation](#4-phase-1--action-tracking-foundation)
5. [Phase 2 — Extended Workflow & Accountability](#5-phase-2--extended-workflow--accountability)
6. [Phase 3 — Enhanced Classification & Content](#6-phase-3--enhanced-classification--content)
7. [Phase 4 — Dissemination & Knowledge Reuse](#7-phase-4--dissemination--knowledge-reuse)
8. [Phase 5 — AI-Powered Lesson Intelligence](#8-phase-5--ai-powered-lesson-intelligence)
9. [Phase 6 — Analytics & Dashboards](#9-phase-6--analytics--dashboards)
10. [Phase 7 — ISO 30401 System-Level Compliance](#10-phase-7--iso-30401-system-level-compliance)
11. [Database Migration Plan](#11-database-migration-plan)
12. [API Changes Summary](#12-api-changes-summary)
13. [Frontend Changes Summary](#13-frontend-changes-summary)
14. [Gap Traceability](#14-gap-traceability)

---

## 1. Current State Summary

### What Exists (Verified via Source Code)

**Entity** (`LessonLearned` in `Collaboration/Domain/Entities/Follow.cs`):
- 7 bilingual content field pairs (Title, Description, Context, Challenge, Solution, Outcome, Recommendations)
- `LessonCategory` enum (10 values): Process, Technical, Communication, TeamManagement, RiskManagement, StakeholderManagement, BudgetManagement, QualityAssurance, VendorManagement, Other
- `LessonImpact` enum (4 values): Low, Medium, High, Critical
- `LessonStatus` enum (6 values): Draft, PendingReview, Approved, Rejected, Published, Archived
- `AuthorId`, `AuthorName`, `CommunityId`, `ProjectId`, `ProjectName`, `OccurredAt`
- `ViewCount`, `UsefulCount` analytics
- `LessonTag` collection (bilingual)
- `Comments` navigation (via `CommentableType.LessonLearned`)
- Domain methods: `Create()`, `Update()`, `Submit()`, `Approve()`, `Reject()`, `Publish()`, `Archive()`

**Controller** (`LessonsLearnedController` — 13 endpoints):
- Full CRUD + workflow transitions + useful marking + related lessons + categories listing

**Integrations**:
- `SearchableContentType.LessonLearned` — full-text search with bilingual indexing
- `SearchScope.LessonsLearned` — semantic search with vector embeddings (320 indexed)
- `NotificationType.LessonLearnedApproved` — single notification type
- `CommentableType.LessonLearned` — threaded comments with mentions
- `CanApproveLessons` authorization policy

### What Does NOT Exist

- No `LessonAction` / action item entity
- No process owner assignment
- No root cause analysis field
- No action verification or closure tracking
- No escalation mechanism
- No extended workflow states beyond Published/Archived
- No document/procedure linkage
- No proactive dissemination
- No anonymous submission
- No lesson export
- No project phase or impact type classification
- No "What Went Well" field
- `LessonLearned` NOT in `FollowableType` enum
- Only 1 of ~8 needed notification types exists
- No lessons learned dashboard/analytics
- No AI-assisted lesson capture
- No integration with WorkflowDefinition engine

---

## 2. Target State Vision

### Enhanced Workflow

```
    ┌─────────┐  Submit   ┌────────────────┐
    │  DRAFT  │ ────────→ │ PENDING REVIEW  │
    └─────────┘           └───────┬────────┘
                                  │
                           ┌──────┴──────┐
                           │             │
                        Approve       Reject
                           │             │
                           ↓             ↓
                    ┌───────────┐  ┌──────────┐
                    │ APPROVED  │  │ REJECTED │ ─→ Edit ─→ Re-submit
                    └─────┬─────┘  └──────────┘
                          │
                       Publish
                          │
                          ↓
                    ┌───────────────┐
                    │   PUBLISHED   │ → Actions auto-created from recommendations
                    └───────┬───────┘
                            │
                     Actions assigned
                            │
                            ↓
                    ┌───────────────┐
                    │ACTIONS PENDING│ → Process owners executing changes
                    └───────┬───────┘
                            │
                     All actions done
                            │
                            ↓
                    ┌───────────────┐
                    │   COMPLETED   │ → Actions implemented, pending verification
                    └───────┬───────┘
                            │
                      Verified effective
                            │
                            ↓
                    ┌───────────────┐
                    │   VERIFIED    │ → Lesson embedded in organizational practice
                    └───────┬───────┘
                            │
                       Auto-archive
                            │
                            ↓
                    ┌───────────────┐
                    │   ARCHIVED    │ → Historical reference
                    └───────────────┘
```

### Target Capabilities

| Capability | Description |
|---|---|
| Action item tracking | Every lesson has structured, trackable actions with assignees and due dates |
| Process owner accountability | Named individuals responsible for implementing changes |
| Root cause analysis | Structured root cause capture with method selection |
| Verification & closure | Actions verified as implemented and effective |
| Escalation | Auto-escalation when actions are overdue |
| Proactive dissemination | AI-powered push to affected teams and projects |
| Project kickoff integration | Surface relevant past lessons for new projects |
| Anonymous submission | Encourage blame-free failure reporting |
| AI-assisted capture | AI helps structure observations and suggest root causes |
| Comprehensive analytics | Dashboard showing lesson health, action rates, and organizational learning |
| ISO 30401 compliance | Knowledge lifecycle management with continuous improvement |

---

## 3. Architecture Decisions

### Decision 1: LessonAction as a New Entity in Collaboration Module

**Rationale**: The `WorkflowTask` entity in the Workflow module is purpose-built for service request workflows and tightly coupled to `ServiceRequest`. Rather than adding coupling between modules, create a `LessonAction` entity within the Collaboration module that follows the `WorkflowTask` pattern but is purpose-built for lessons learned.

**Pattern reference**: `WorkflowTask` has `TaskStatus` (Pending, InProgress, Completed, Rejected, Cancelled, Expired), `TaskPriority`, assignment, delegation, due dates, and completion tracking. `LessonAction` will follow this proven pattern.

### Decision 2: Extend LessonStatus Enum (Not WorkflowDefinition)

**Rationale**: The lesson approval workflow is simple and linear. The `WorkflowDefinition` engine (Sequential, Parallel, Conditional, StateMachine) is designed for complex, configurable workflows. Extending the `LessonStatus` enum with 4 additional states is simpler, more maintainable, and doesn't create cross-module coupling. The WorkflowDefinition engine can be integrated later for organizations needing configurable approval chains.

### Decision 3: All Changes Additive

**Rationale**: Existing entities, enums, DTOs, and API endpoints are extended with new fields and values. No existing fields are modified or removed. All database migrations add nullable columns or new tables. Existing API consumers continue working without modification.

### Decision 4: Notification Types Added to Existing Enum

**Rationale**: `NotificationType` already has 30 values. Adding 7 more for lesson lifecycle events is consistent with the existing pattern and leverages the full notification infrastructure (6 channels, preferences, templates, broadcast).

---

## 4. Phase 1 — Action Tracking Foundation

**Duration**: Weeks 1–4
**Priority**: P0 — Critical
**Gaps addressed**: LL-1, LL-2, LL-4, LL-8

### 1.1 New Entity: `LessonAction`

**File**: `Collaboration/Domain/Entities/LessonAction.cs` (new file)

```csharp
public class LessonAction : AuditableEntity
{
    public Guid Id { get; private set; }
    public Guid LessonLearnedId { get; private set; }

    // Action content (bilingual)
    public string Description { get; private set; }
    public string? DescriptionArabic { get; private set; }

    // Assignment
    public Guid AssigneeId { get; private set; }
    public string AssigneeName { get; private set; }
    public Guid? DelegatedToId { get; private set; }
    public string? DelegatedToName { get; private set; }

    // Tracking
    public LessonActionStatus Status { get; private set; }
    public LessonActionPriority Priority { get; private set; }
    public DateTime DueDate { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public Guid? CompletedById { get; private set; }
    public string? CompletionNotes { get; private set; }
    public string? CompletionNotesArabic { get; private set; }

    // Verification
    public DateTime? VerifiedAt { get; private set; }
    public Guid? VerifiedById { get; private set; }
    public string? VerifiedByName { get; private set; }
    public string? VerificationNotes { get; private set; }

    // Affected document/procedure
    public Guid? AffectedDocumentId { get; private set; }
    public string? AffectedDocumentTitle { get; private set; }
    public string? AffectedDocumentType { get; private set; } // Article, Document, Policy, Procedure

    // Escalation
    public DateTime? EscalatedAt { get; private set; }
    public Guid? EscalatedToId { get; private set; }
    public string? EscalatedToName { get; private set; }
    public int ReminderCount { get; private set; }

    // Ordering
    public int SortOrder { get; private set; }

    // Navigation
    public virtual LessonLearned LessonLearned { get; private set; }

    // Factory method
    public static LessonAction Create(
        Guid lessonLearnedId,
        string description,
        string? descriptionArabic,
        Guid assigneeId,
        string assigneeName,
        DateTime dueDate,
        LessonActionPriority priority,
        Guid? affectedDocumentId = null,
        string? affectedDocumentTitle = null,
        string? affectedDocumentType = null)
    {
        return new LessonAction
        {
            Id = Guid.NewGuid(),
            LessonLearnedId = lessonLearnedId,
            Description = description,
            DescriptionArabic = descriptionArabic,
            AssigneeId = assigneeId,
            AssigneeName = assigneeName,
            DueDate = dueDate,
            Priority = priority,
            Status = LessonActionStatus.Open,
            AffectedDocumentId = affectedDocumentId,
            AffectedDocumentTitle = affectedDocumentTitle,
            AffectedDocumentType = affectedDocumentType,
            SortOrder = 0,
            ReminderCount = 0
        };
    }

    // Domain methods
    public void StartProgress()
    {
        if (Status != LessonActionStatus.Open) return;
        Status = LessonActionStatus.InProgress;
        StartedAt = DateTime.UtcNow;
    }

    public void Complete(Guid completedById, string? notes, string? notesArabic)
    {
        Status = LessonActionStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        CompletedById = completedById;
        CompletionNotes = notes;
        CompletionNotesArabic = notesArabic;
    }

    public void Verify(Guid verifiedById, string verifiedByName, string? notes)
    {
        if (Status != LessonActionStatus.Completed) return;
        Status = LessonActionStatus.Verified;
        VerifiedAt = DateTime.UtcNow;
        VerifiedById = verifiedById;
        VerifiedByName = verifiedByName;
        VerificationNotes = notes;
    }

    public void Cancel()
    {
        Status = LessonActionStatus.Cancelled;
    }

    public void Escalate(Guid escalatedToId, string escalatedToName)
    {
        EscalatedAt = DateTime.UtcNow;
        EscalatedToId = escalatedToId;
        EscalatedToName = escalatedToName;
    }

    public void IncrementReminder()
    {
        ReminderCount++;
    }

    public void Delegate(Guid delegatedToId, string delegatedToName)
    {
        DelegatedToId = delegatedToId;
        DelegatedToName = delegatedToName;
    }

    public void UpdateDueDate(DateTime newDueDate)
    {
        DueDate = newDueDate;
    }

    public void LinkDocument(Guid documentId, string documentTitle, string documentType)
    {
        AffectedDocumentId = documentId;
        AffectedDocumentTitle = documentTitle;
        AffectedDocumentType = documentType;
    }

    public bool IsOverdue => Status is LessonActionStatus.Open or LessonActionStatus.InProgress
                             && DueDate < DateTime.UtcNow;
}

public enum LessonActionStatus
{
    Open,
    InProgress,
    Completed,
    Verified,
    Cancelled
}

public enum LessonActionPriority
{
    Low,
    Normal,
    High,
    Urgent
}
```

### 1.2 Extend `LessonLearned` Entity

**File**: `Collaboration/Domain/Entities/Follow.cs` — modify `LessonLearned` class

**New properties to add:**

```csharp
// Process owner (accountable for implementing changes)
public Guid? ProcessOwnerId { get; private set; }
public string? ProcessOwnerName { get; private set; }

// Root cause analysis
public string? RootCause { get; private set; }
public string? RootCauseArabic { get; private set; }
public RootCauseMethod? RootCauseMethod { get; private set; }

// Navigation to actions
public virtual ICollection<LessonAction> Actions { get; private set; } = new List<LessonAction>();
```

**New domain methods to add:**

```csharp
public void AssignProcessOwner(Guid ownerId, string ownerName)
{
    ProcessOwnerId = ownerId;
    ProcessOwnerName = ownerName;
}

public void SetRootCause(string rootCause, string? rootCauseArabic, RootCauseMethod? method)
{
    RootCause = rootCause;
    RootCauseArabic = rootCauseArabic;
    RootCauseMethod = method;
}

public void MarkActionsPending()
{
    if (Status != LessonStatus.Published) return;
    Status = LessonStatus.ActionsPending;
}

public void MarkActionsComplete()
{
    if (Status != LessonStatus.ActionsPending) return;
    Status = LessonStatus.ActionsComplete;
}

public void MarkVerified()
{
    if (Status != LessonStatus.ActionsComplete) return;
    Status = LessonStatus.Verified;
}
```

### 1.3 Extend `LessonStatus` Enum

**File**: `Collaboration/Domain/Entities/Follow.cs`

```csharp
public enum LessonStatus
{
    Draft,            // Initial creation
    PendingReview,    // Submitted for SME review
    Approved,         // Reviewed and approved
    Rejected,         // Reviewed and rejected
    Published,        // Published to knowledge base
    ActionsPending,   // NEW: Actions created, awaiting completion
    ActionsComplete,  // NEW: All actions done, pending verification
    Verified,         // NEW: Actions verified as effective
    Archived          // Historical reference
}
```

### 1.4 New Enum: `RootCauseMethod`

```csharp
public enum RootCauseMethod
{
    FiveWhys,
    Fishbone,        // Ishikawa diagram
    FaultTree,
    ParetoCausual,
    Other
}
```

### 1.5 New API Endpoints

**Add to `LessonsLearnedController`:**

```csharp
// === Action Items ===

// GET /api/collaboration/lessons-learned/{id}/actions
[HttpGet("{id:guid}/actions")]
public async Task<ActionResult<IReadOnlyList<LessonActionDto>>> GetActions(Guid id)

// POST /api/collaboration/lessons-learned/{id}/actions
[HttpPost("{id:guid}/actions")]
public async Task<ActionResult<LessonActionDto>> CreateAction(
    Guid id, [FromBody] CreateLessonActionRequest request)

// PUT /api/collaboration/lessons-learned/{id}/actions/{actionId}
[HttpPut("{id:guid}/actions/{actionId:guid}")]
public async Task<ActionResult> UpdateAction(
    Guid id, Guid actionId, [FromBody] UpdateLessonActionRequest request)

// POST /api/collaboration/lessons-learned/{id}/actions/{actionId}/start
[HttpPost("{id:guid}/actions/{actionId:guid}/start")]
public async Task<ActionResult> StartAction(Guid id, Guid actionId)

// POST /api/collaboration/lessons-learned/{id}/actions/{actionId}/complete
[HttpPost("{id:guid}/actions/{actionId:guid}/complete")]
public async Task<ActionResult> CompleteAction(
    Guid id, Guid actionId, [FromBody] CompleteActionRequest request)

// POST /api/collaboration/lessons-learned/{id}/actions/{actionId}/verify
[HttpPost("{id:guid}/actions/{actionId:guid}/verify")]
[Authorize(Policy = "CanApproveLessons")]
public async Task<ActionResult> VerifyAction(
    Guid id, Guid actionId, [FromBody] VerifyActionRequest request)

// POST /api/collaboration/lessons-learned/{id}/actions/{actionId}/cancel
[HttpPost("{id:guid}/actions/{actionId:guid}/cancel")]
public async Task<ActionResult> CancelAction(Guid id, Guid actionId)

// POST /api/collaboration/lessons-learned/{id}/actions/{actionId}/delegate
[HttpPost("{id:guid}/actions/{actionId:guid}/delegate")]
public async Task<ActionResult> DelegateAction(
    Guid id, Guid actionId, [FromBody] DelegateActionRequest request)

// POST /api/collaboration/lessons-learned/{id}/actions/{actionId}/link-document
[HttpPost("{id:guid}/actions/{actionId:guid}/link-document")]
public async Task<ActionResult> LinkDocument(
    Guid id, Guid actionId, [FromBody] LinkDocumentRequest request)

// === Process Owner ===

// POST /api/collaboration/lessons-learned/{id}/assign-process-owner
[HttpPost("{id:guid}/assign-process-owner")]
public async Task<ActionResult> AssignProcessOwner(
    Guid id, [FromBody] AssignProcessOwnerRequest request)

// === Root Cause ===

// PUT /api/collaboration/lessons-learned/{id}/root-cause
[HttpPut("{id:guid}/root-cause")]
public async Task<ActionResult> SetRootCause(
    Guid id, [FromBody] SetRootCauseRequest request)

// === Workflow Transitions ===

// POST /api/collaboration/lessons-learned/{id}/mark-actions-pending
[HttpPost("{id:guid}/mark-actions-pending")]
public async Task<ActionResult> MarkActionsPending(Guid id)

// POST /api/collaboration/lessons-learned/{id}/mark-actions-complete
[HttpPost("{id:guid}/mark-actions-complete")]
public async Task<ActionResult> MarkActionsComplete(Guid id)

// POST /api/collaboration/lessons-learned/{id}/verify
[HttpPost("{id:guid}/verify")]
[Authorize(Policy = "CanApproveLessons")]
public async Task<ActionResult> VerifyLesson(Guid id)
```

### 1.6 New DTOs

```csharp
// Action item DTOs
public record LessonActionDto
{
    public Guid Id { get; init; }
    public Guid LessonLearnedId { get; init; }
    public string Description { get; init; }
    public string? DescriptionArabic { get; init; }
    public Guid AssigneeId { get; init; }
    public string AssigneeName { get; init; }
    public string? AssigneeAvatarUrl { get; init; }
    public Guid? DelegatedToId { get; init; }
    public string? DelegatedToName { get; init; }
    public string Status { get; init; }
    public string Priority { get; init; }
    public DateTime DueDate { get; init; }
    public DateTime? StartedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
    public string? CompletionNotes { get; init; }
    public string? CompletionNotesArabic { get; init; }
    public DateTime? VerifiedAt { get; init; }
    public string? VerifiedByName { get; init; }
    public string? VerificationNotes { get; init; }
    public Guid? AffectedDocumentId { get; init; }
    public string? AffectedDocumentTitle { get; init; }
    public string? AffectedDocumentType { get; init; }
    public bool IsOverdue { get; init; }
    public int ReminderCount { get; init; }
    public DateTime? EscalatedAt { get; init; }
}

public record CreateLessonActionRequest
{
    public string Description { get; init; }
    public string? DescriptionArabic { get; init; }
    public Guid AssigneeId { get; init; }
    public DateTime DueDate { get; init; }
    public string Priority { get; init; } = "Normal";
    public Guid? AffectedDocumentId { get; init; }
    public string? AffectedDocumentTitle { get; init; }
    public string? AffectedDocumentType { get; init; }
}

public record CompleteActionRequest
{
    public string? Notes { get; init; }
    public string? NotesArabic { get; init; }
}

public record VerifyActionRequest
{
    public string? Notes { get; init; }
}

public record AssignProcessOwnerRequest
{
    public Guid ProcessOwnerId { get; init; }
}

public record SetRootCauseRequest
{
    public string RootCause { get; init; }
    public string? RootCauseArabic { get; init; }
    public string? Method { get; init; }  // FiveWhys, Fishbone, FaultTree, Other
}
```

### 1.7 Extend Existing DTOs

**Modify `LessonLearnedDto`** — add:
```csharp
public Guid? ProcessOwnerId { get; init; }
public string? ProcessOwnerName { get; init; }
public string? RootCause { get; init; }
public string? RootCauseArabic { get; init; }
public string? RootCauseMethod { get; init; }
public IReadOnlyList<LessonActionDto> Actions { get; init; }
public int TotalActions { get; init; }
public int CompletedActions { get; init; }
public int OverdueActions { get; init; }
```

**Modify `CreateLessonLearnedRequest`** — add:
```csharp
public string? RootCause { get; init; }
public string? RootCauseArabic { get; init; }
public string? RootCauseMethod { get; init; }
public Guid? ProcessOwnerId { get; init; }
```

### 1.8 Frontend Changes

**`LessonDetailView.vue`** — add:
- Action items section with table (description, assignee, due date, status, priority)
- "Add Action" button opening creation dialog
- Per-action status buttons (Start, Complete, Verify, Cancel)
- Document link for each action
- Process owner assignment card
- Root cause analysis section with method selector
- Progress bar showing action completion percentage
- Overdue indicators with visual alerts

**`LessonsLearnedView.vue`** — add:
- Action completion percentage column
- Filter by action status (Has overdue actions, All actions complete, etc.)
- Process owner filter

---

## 5. Phase 2 — Extended Workflow & Accountability

**Duration**: Weeks 3–6
**Priority**: P1 — High
**Gaps addressed**: LL-3, LL-14, LL-17

### 2.1 Extended Notification Types

**File**: `Notifications/Domain/Entities/Notification.cs` — extend `NotificationType` enum

```csharp
// Add to existing enum:
LessonSubmitted,           // Author submits for review → notify reviewers
LessonRejected,            // Reviewer rejects → notify author with reason
LessonPublished,           // Lesson published → notify relevant teams
LessonActionAssigned,      // Action created → notify assignee
LessonActionOverdue,       // Action past due → notify assignee + manager
LessonActionCompleted,     // Action completed → notify process owner
LessonActionsAllComplete,  // All actions done → notify KM admin for verification
```

### 2.2 Background Job: Action Escalation

**File**: `Workers/AFC27.KMS.NotificationWorker/Consumers/LessonActionEscalationConsumer.cs` (new)

Or add to an existing scheduled job runner:

```csharp
public class LessonActionEscalationJob
{
    // Runs daily at 09:00
    // Queries LessonAction where:
    //   Status == Open or InProgress
    //   AND DueDate < DateTime.UtcNow
    //   AND (EscalatedAt == null OR EscalatedAt < DateTime.UtcNow.AddDays(-3))

    // For each overdue action:
    // 1. Send LessonActionOverdue notification to assignee
    // 2. If overdue > 7 days, escalate to assignee's manager (User.ManagerId)
    // 3. If overdue > 14 days, escalate to process owner
    // 4. Increment ReminderCount
    // 5. Record EscalatedAt and EscalatedToId

    // Additionally:
    // When ALL actions for a lesson reach Completed status:
    // 1. Auto-transition lesson to ActionsComplete status
    // 2. Send LessonActionsAllComplete notification to KM admin
}
```

### 2.3 Auto-Transition Logic

**Add to `LessonsLearnedController` or a domain service:**

When a `LessonAction` is completed:
1. Check if ALL actions for that lesson are now `Completed` or `Verified` or `Cancelled`
2. If yes, auto-transition lesson from `ActionsPending` → `ActionsComplete`
3. Publish `LessonActionsAllComplete` notification

When a lesson is published:
1. If it has actions already defined, auto-transition to `ActionsPending`
2. Send `LessonActionAssigned` notification to each assignee

---

## 6. Phase 3 — Enhanced Classification & Content

**Duration**: Weeks 5–8
**Priority**: P1/P2
**Gaps addressed**: LL-5, LL-6, LL-7, LL-12, LL-18, LL-19

### 3.1 New Enums

```csharp
public enum ProjectPhase
{
    Initiation,
    Planning,
    Execution,
    Monitoring,
    Closure,
    Operations
}

public enum ImpactType
{
    Cost,
    Schedule,
    Quality,
    Safety,
    Stakeholder,
    Reputation,
    Compliance
}
```

### 3.2 Extend `LessonLearned` Entity

```csharp
// Positive lessons
public string? WhatWentWell { get; private set; }
public string? WhatWentWellArabic { get; private set; }

// Enhanced classification
public ProjectPhase? ProjectPhase { get; private set; }
public ImpactType? ImpactType { get; private set; }

// Anonymous submission
public bool IsAnonymous { get; private set; }

// Rejection tracking
public string? RejectionReason { get; private set; }
public DateTime? RejectedAt { get; private set; }
public Guid? RejectedById { get; private set; }
```

### 3.3 Extend `CreateLessonLearnedRequest`

```csharp
public string? WhatWentWell { get; init; }
public string? WhatWentWellArabic { get; init; }
public string? ProjectPhase { get; init; }
public string? ImpactType { get; init; }
public bool IsAnonymous { get; init; }
```

### 3.4 Add `LessonLearned` to `FollowableType`

**File**: `Collaboration/Domain/Entities/Follow.cs`

```csharp
public enum FollowableType
{
    User, Community, Discussion, Article, Document, Tag, Category,
    LessonLearned  // NEW
}
```

### 3.5 Lesson Export

**New endpoint:**

```csharp
// GET /api/collaboration/lessons-learned/{id}/export?format=pdf|docx|md&language=en|ar|both
[HttpGet("{id:guid}/export")]
public async Task<IActionResult> ExportLesson(
    Guid id,
    [FromQuery] string format = "pdf",
    [FromQuery] string language = "en")
```

**Implementation**: Compose lesson content (all fields, actions, root cause) into structured HTML → convert to DOCX (using existing OpenXml pattern from `SummarizationController.ExportMeetingMinutes`) or PDF.

### 3.6 Bulk Export

```csharp
// POST /api/collaboration/lessons-learned/export
[HttpPost("export")]
public async Task<IActionResult> BulkExport(
    [FromBody] BulkExportRequest request)  // category, impact, dateRange, format

// Returns ZIP containing individual lesson exports + summary CSV
```

### 3.7 Frontend Changes

- "What Went Well" section in lesson creation/detail forms
- Project Phase dropdown selector
- Impact Type dropdown selector
- Anonymous submission toggle (hides author name from published view)
- Export button with format/language selection
- Follow/Watch button on lesson detail page
- Rejection reason display when status is Rejected

---

## 7. Phase 4 — Dissemination & Knowledge Reuse

**Duration**: Weeks 7–10
**Priority**: P1/P2
**Gaps addressed**: LL-9, LL-10, LL-13

### 4.1 Proactive Dissemination

**When a lesson is published**, analyze its metadata and push notifications:

```csharp
public class LessonDisseminationService
{
    public async Task DisseminateLessonAsync(Guid lessonId)
    {
        var lesson = await GetLessonAsync(lessonId);

        // 1. Category-based: notify communities matching lesson category
        var communities = await FindMatchingCommunities(lesson.Category, lesson.Tags);

        // 2. Project-based: notify members of related projects
        if (lesson.ProjectId.HasValue)
            await NotifyProjectTeam(lesson.ProjectId.Value);

        // 3. Tag-based: notify users following matching tags
        var tagFollowers = await GetTagFollowers(lesson.Tags);

        // 4. AI similarity: find active projects similar to this lesson's context
        var relatedProjects = await SemanticSearch.FindSimilarProjects(lesson.Context);

        // 5. Broadcast via configured channels (Slack, Teams, Email)
        await BroadcastLesson(lesson, communities, tagFollowers, relatedProjects);
    }
}
```

### 4.2 Project Kickoff Integration

**New endpoint:**

```csharp
// GET /api/collaboration/lessons-learned/for-project?projectName=&category=&tags=
[HttpGet("for-project")]
public async Task<ActionResult<IReadOnlyList<LessonLearnedSummaryDto>>> GetLessonsForProject(
    [FromQuery] string? projectName,
    [FromQuery] string? category,
    [FromQuery] string? tags,
    [FromQuery] int limit = 10)
```

**Implementation**: Combine keyword search (project name), category match, tag overlap, and AI semantic similarity to find the most relevant past lessons for a new project.

### 4.3 Cross-Project Reuse Tracking

**New entity**: `LessonApplication`

```csharp
public class LessonApplication
{
    public Guid Id { get; set; }
    public Guid LessonLearnedId { get; set; }
    public Guid AppliedByProjectId { get; set; }
    public string AppliedByProjectName { get; set; }
    public Guid AppliedById { get; set; }
    public string AppliedByName { get; set; }
    public DateTime AppliedAt { get; set; }
    public string? Notes { get; set; }
    public string? NotesArabic { get; set; }
    public virtual LessonLearned LessonLearned { get; set; }
}
```

**New endpoints:**

```csharp
// POST /api/collaboration/lessons-learned/{id}/apply
[HttpPost("{id:guid}/apply")]
public async Task<ActionResult> RecordApplication(
    Guid id, [FromBody] RecordApplicationRequest request)

// GET /api/collaboration/lessons-learned/{id}/applications
[HttpGet("{id:guid}/applications")]
public async Task<ActionResult<IReadOnlyList<LessonApplicationDto>>> GetApplications(Guid id)
```

### 4.4 "Pause and Learn" Session Support

**New endpoint:**

```csharp
// POST /api/collaboration/lessons-learned/capture-session
[HttpPost("capture-session")]
public async Task<ActionResult<IReadOnlyList<LessonLearnedDto>>> CreateCaptureSession(
    [FromBody] CaptureSessionRequest request)

// request.ProjectId, request.SessionType (AAR, PauseAndLearn, Retrospective, PostMortem)
// request.Observations (list of raw observations)
// Returns: draft lessons auto-created from observations (using AI in Phase 5)
```

---

## 8. Phase 5 — AI-Powered Lesson Intelligence

**Duration**: Weeks 9–12
**Priority**: P2
**Gaps addressed**: LL-16

### 5.1 AI-Assisted Lesson Capture

**New endpoint in AI module:**

```csharp
// POST /api/ai/lessons/assist-capture
[HttpPost("lessons/assist-capture")]
public async Task<ActionResult<LessonCaptureAssistanceDto>> AssistCapture(
    [FromBody] AssistCaptureRequest request)
```

**Request**: raw observation text (what happened)
**Response**:
```csharp
public record LessonCaptureAssistanceDto
{
    public string SuggestedTitle { get; init; }
    public string SuggestedTitleArabic { get; init; }
    public string SuggestedContext { get; init; }
    public string SuggestedChallenge { get; init; }
    public string SuggestedSolution { get; init; }
    public string SuggestedOutcome { get; init; }
    public string SuggestedRootCause { get; init; }
    public string SuggestedRootCauseMethod { get; init; }
    public string SuggestedCategory { get; init; }
    public string SuggestedImpact { get; init; }
    public IReadOnlyList<string> SuggestedTags { get; init; }
    public IReadOnlyList<string> SuggestedActions { get; init; }
    public IReadOnlyList<SimilarLessonDto> SimilarPastLessons { get; init; }
    public IReadOnlyList<AffectedDocumentSuggestionDto> SuggestedAffectedDocuments { get; init; }
}
```

### 5.2 AI Root Cause Suggestion

```csharp
// POST /api/ai/lessons/suggest-root-cause
[HttpPost("lessons/suggest-root-cause")]
public async Task<ActionResult<RootCauseSuggestionDto>> SuggestRootCause(
    [FromBody] RootCauseAnalysisRequest request)

// Accepts: challenge text, context, and optional method preference
// Returns: structured root cause analysis with 5-Whys chain or fishbone categories
```

### 5.3 AI Document Impact Analysis

```csharp
// POST /api/ai/lessons/find-affected-documents
[HttpPost("lessons/find-affected-documents")]
public async Task<ActionResult<IReadOnlyList<AffectedDocumentSuggestionDto>>> FindAffectedDocuments(
    [FromBody] FindAffectedDocsRequest request)

// Uses RAG to search existing documents/articles that the lesson's recommendations
// would require updating. Returns ranked list with relevance scores.
```

### 5.4 AI Translation for Lessons

Extend the AI translation capability (from main roadmap Phase 2A) to lessons learned:

```csharp
// POST /api/ai/lessons/{id}/translate
[HttpPost("lessons/{id:guid}/translate")]
public async Task<ActionResult> TranslateLesson(
    Guid id,
    [FromQuery] string targetLanguage)  // "en" or "ar"

// Translates all content fields to the target language
```

---

## 9. Phase 6 — Analytics & Dashboards

**Duration**: Weeks 11–14
**Priority**: P1
**Gaps addressed**: LL-15

### 6.1 Analytics Endpoints

```csharp
// GET /api/collaboration/lessons-learned/analytics/overview
[HttpGet("analytics/overview")]
public async Task<ActionResult<LessonsAnalyticsOverviewDto>> GetAnalyticsOverview(
    [FromQuery] DateTime? from,
    [FromQuery] DateTime? to)
```

**Response:**

```csharp
public record LessonsAnalyticsOverviewDto
{
    // Volume metrics
    public int TotalLessons { get; init; }
    public int LessonsCreatedInPeriod { get; init; }
    public int LessonsPublishedInPeriod { get; init; }

    // Status distribution
    public Dictionary<string, int> LessonsByStatus { get; init; }

    // Category distribution
    public Dictionary<string, int> LessonsByCategory { get; init; }

    // Impact distribution
    public Dictionary<string, int> LessonsByImpact { get; init; }

    // Action tracking
    public int TotalActions { get; init; }
    public int OpenActions { get; init; }
    public int CompletedActions { get; init; }
    public int VerifiedActions { get; init; }
    public int OverdueActions { get; init; }
    public double ActionCompletionRate { get; init; }
    public double AverageTimeToCompleteActionDays { get; init; }

    // Engagement
    public int TotalViews { get; init; }
    public int TotalUsefulVotes { get; init; }
    public double AverageUsefulScore { get; init; }

    // Contributors
    public IReadOnlyList<ContributorDto> TopContributors { get; init; }

    // Trends
    public IReadOnlyList<TrendDataPointDto> MonthlyLessonsTrend { get; init; }
    public IReadOnlyList<TrendDataPointDto> MonthlyActionCompletionTrend { get; init; }

    // Overdue items needing attention
    public IReadOnlyList<OverdueActionDto> OverdueActionsList { get; init; }

    // Lessons without process owners
    public int LessonsWithoutProcessOwner { get; init; }

    // Cross-project application
    public int TotalApplicationsAcrossProjects { get; init; }
    public IReadOnlyList<MostAppliedLessonDto> MostAppliedLessons { get; init; }
}
```

### 6.2 Additional Analytics Endpoints

```csharp
// GET /api/collaboration/lessons-learned/analytics/by-project
// GET /api/collaboration/lessons-learned/analytics/by-department
// GET /api/collaboration/lessons-learned/analytics/overdue-actions
// GET /api/collaboration/lessons-learned/analytics/action-completion-by-owner
// GET /api/collaboration/lessons-learned/analytics/export?format=csv|xlsx
```

### 6.3 Frontend Dashboard

**New view**: `LessonsLearnedDashboardView.vue`

Sections:
1. **Summary Cards** — Total lessons, Published this month, Open actions, Overdue actions, Completion rate
2. **Status Distribution** — Pie chart of lessons by status
3. **Category Breakdown** — Bar chart of lessons by category
4. **Action Completion Rate** — Gauge showing overall completion %
5. **Monthly Trend** — Line chart of lessons created and actions completed over time
6. **Overdue Actions Table** — Sortable list with assignee, lesson title, days overdue, escalation status
7. **Top Contributors** — Leaderboard
8. **Most Applied Lessons** — Lessons referenced by the most projects
9. **Lessons Without Process Owner** — Items needing attention
10. **Time to Implement** — Average days from lesson publication to action completion

---

## 10. Phase 7 — ISO 30401 System-Level Compliance

**Duration**: Weeks 13–18
**Priority**: P2
**Gaps addressed**: ISO 30401 clauses 4.4, 5.2, 6.1, 6.2, 7.2, 8.2, 8.6, 9.1, 9.3, 10.2

### 7.1 Knowledge Asset Registry (Clause 8.2)

**New entity** (Admin module or new KnowledgeGovernance module):

```csharp
public class KnowledgeAsset
{
    public Guid Id { get; set; }
    public string Name { get; set; }                    // e.g., "Tournament Logistics Expertise"
    public string NameArabic { get; set; }
    public string Description { get; set; }
    public string DescriptionArabic { get; set; }
    public KnowledgeDomain Domain { get; set; }          // Operations, Technical, Commercial, etc.
    public KnowledgeCriticality Criticality { get; set; } // Critical, Important, Supporting
    public Guid? PrimaryHolderId { get; set; }           // Main expert
    public string? PrimaryHolderName { get; set; }
    public List<Guid> SecondaryHolderIds { get; set; }   // Backup experts
    public KnowledgeRiskLevel RiskLevel { get; set; }    // High, Medium, Low
    public string? RiskDescription { get; set; }         // e.g., "Single point of failure — key person leaving"
    public string? MitigationPlan { get; set; }          // e.g., "Cross-training program initiated"
    public Guid? RelatedSpaceId { get; set; }            // Link to Space containing this knowledge
    public DateTime LastAssessedAt { get; set; }
    public DateTime NextAssessmentDue { get; set; }
}

public enum KnowledgeDomain
{
    Operations, Technical, Commercial, Legal, HR, Finance, Communication, Safety, Innovation
}

public enum KnowledgeCriticality
{
    Critical,   // Loss would severely impact operations
    Important,  // Loss would significantly affect quality
    Supporting  // Loss would cause inconvenience
}

public enum KnowledgeRiskLevel
{
    High,       // Single holder, upcoming departure, no documentation
    Medium,     // Limited holders, partial documentation
    Low         // Multiple holders, well documented
}
```

### 7.2 KM Objectives & Measurement (Clause 6.2, 9.1)

**New entity:**

```csharp
public class KMObjective
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string TitleArabic { get; set; }
    public string Description { get; set; }
    public string DescriptionArabic { get; set; }
    public string Metric { get; set; }               // e.g., "Lesson action completion rate"
    public double TargetValue { get; set; }           // e.g., 90.0 (percent)
    public double? CurrentValue { get; set; }         // e.g., 72.5
    public string Unit { get; set; }                  // "percent", "count", "days"
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public Guid OwnerId { get; set; }
    public string OwnerName { get; set; }
    public KMObjectiveStatus Status { get; set; }     // OnTrack, AtRisk, Behind, Achieved
    public DateTime LastMeasuredAt { get; set; }
}
```

### 7.3 KM Policy Management (Clause 5.2)

**New entity:**

```csharp
public class KMPolicy
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string TitleArabic { get; set; }
    public string Content { get; set; }               // Rich text policy content
    public string ContentArabic { get; set; }
    public int Version { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public Guid ApprovedById { get; set; }
    public string ApprovedByName { get; set; }
    public KMPolicyStatus Status { get; set; }        // Draft, Active, Superseded, Expired
    public bool RequiresAcknowledgment { get; set; }
}

public class KMPolicyAcknowledgment
{
    public Guid PolicyId { get; set; }
    public Guid UserId { get; set; }
    public DateTime AcknowledgedAt { get; set; }
}
```

### 7.4 Knowledge Culture Enablers (Clause 4.4)

**Contributor recognition:**
- Add `ContributionScore` computed property to User profiles based on: lessons authored, actions completed, articles published, documents shared, helpful votes received
- Contributor leaderboard endpoint
- Monthly "Knowledge Champion" recognition (configurable)

**Gamification elements:**
- Badges for milestones: "First Lesson," "10 Lessons," "100 Useful Votes," "Action Closer"
- Knowledge sharing streaks
- Department contribution rankings

### 7.5 Management Review Report (Clause 9.3)

**New endpoint:**

```csharp
// GET /api/admin/km-review-report?period=quarterly|annual&date=2026-03-01
[HttpGet("km-review-report")]
public async Task<ActionResult<KMReviewReportDto>> GenerateManagementReviewReport(
    [FromQuery] string period,
    [FromQuery] DateTime date)
```

**Report contents**: KM objectives status, lesson metrics, action completion rates, knowledge asset health, content freshness, user adoption, AI usage, recommendations for improvement.

---

## 11. Database Migration Plan

### Migration 1: `AddLessonActions` (Phase 1)

```
NEW TABLE: LessonAction
  - Id, LessonLearnedId (FK), Description, DescriptionArabic
  - AssigneeId, AssigneeName, DelegatedToId, DelegatedToName
  - Status, Priority, DueDate, StartedAt, CompletedAt, CompletedById
  - CompletionNotes, CompletionNotesArabic
  - VerifiedAt, VerifiedById, VerifiedByName, VerificationNotes
  - AffectedDocumentId, AffectedDocumentTitle, AffectedDocumentType
  - EscalatedAt, EscalatedToId, EscalatedToName, ReminderCount
  - SortOrder, CreatedAt, CreatedBy, ModifiedAt, ModifiedBy, IsDeleted

ALTER TABLE LessonLearned:
  + ProcessOwnerId (Guid, nullable)
  + ProcessOwnerName (nvarchar, nullable)
  + RootCause (nvarchar(max), nullable)
  + RootCauseArabic (nvarchar(max), nullable)
  + RootCauseMethod (int, nullable)
```

### Migration 2: `ExtendLessonStatus` (Phase 1)

```
-- LessonStatus enum extended with 3 new values:
-- ActionsPending = 5, ActionsComplete = 6, Verified = 7
-- Existing values unchanged: Draft=0, PendingReview=1, Approved=2, Rejected=3, Published=4, Archived=8
```

### Migration 3: `AddLessonClassification` (Phase 3)

```
ALTER TABLE LessonLearned:
  + WhatWentWell (nvarchar(max), nullable)
  + WhatWentWellArabic (nvarchar(max), nullable)
  + ProjectPhase (int, nullable)
  + ImpactType (int, nullable)
  + IsAnonymous (bit, default 0)
  + RejectionReason (nvarchar(max), nullable)
  + RejectedAt (datetime2, nullable)
  + RejectedById (uniqueidentifier, nullable)
```

### Migration 4: `AddLessonApplication` (Phase 4)

```
NEW TABLE: LessonApplication
  - Id, LessonLearnedId (FK), AppliedByProjectId, AppliedByProjectName
  - AppliedById, AppliedByName, AppliedAt, Notes, NotesArabic
```

### Migration 5: `AddKnowledgeGovernance` (Phase 7)

```
NEW TABLE: KnowledgeAsset
NEW TABLE: KMObjective
NEW TABLE: KMPolicy
NEW TABLE: KMPolicyAcknowledgment
```

All migrations are **additive** — nullable columns and new tables only.

---

## 12. API Changes Summary

### New Endpoints (by Phase)

| Phase | Endpoints Added | Total |
|---|---|---|
| Phase 1 | 12 action endpoints + 3 workflow + 2 assignment/root cause | 17 |
| Phase 2 | 0 (background job + notifications) | 0 |
| Phase 3 | 2 export + 1 follow | 3 |
| Phase 4 | 4 dissemination + kickoff + applications + capture session | 4 |
| Phase 5 | 4 AI assistance endpoints | 4 |
| Phase 6 | 6 analytics endpoints | 6 |
| Phase 7 | 8 governance endpoints | 8 |
| **Total** | | **42** |

### Modified Endpoints

| Endpoint | Changes |
|---|---|
| `GET /lessons-learned` | Add filters: actionStatus, processOwnerId, projectPhase, impactType |
| `GET /lessons-learned/{id}` | Response includes Actions[], ProcessOwner, RootCause, new fields |
| `POST /lessons-learned` | Request accepts RootCause, ProcessOwnerId, WhatWentWell, ProjectPhase, ImpactType, IsAnonymous |
| `PUT /lessons-learned/{id}` | Same extensions as POST |
| `GET /lessons-learned/categories` | Add ProjectPhase and ImpactType category listings |

---

## 13. Frontend Changes Summary

### New Views

| View | Phase | Description |
|---|---|---|
| `LessonsLearnedDashboardView.vue` | 6 | Analytics dashboard with charts and tables |
| `KnowledgeAssetsView.vue` | 7 | Knowledge asset registry management |
| `KMObjectivesView.vue` | 7 | KM objectives tracking |
| `KMPoliciesView.vue` | 7 | KM policy management |
| `KMReviewReportView.vue` | 7 | Management review report generation |

### Modified Views

| View | Changes |
|---|---|
| `LessonDetailView.vue` | Action items section, process owner card, root cause section, "What Went Well" section, project phase/impact type badges, follow button, export button, application tracking, progress indicators |
| `LessonsLearnedView.vue` | Action status filters, completion percentage column, process owner filter, project phase filter, dashboard link, bulk export |

### New Components

| Component | Purpose |
|---|---|
| `LessonActionsList.vue` | Table of action items with status controls |
| `LessonActionDialog.vue` | Create/edit action item form |
| `RootCauseEditor.vue` | Root cause input with method selector (5-Whys wizard, fishbone template) |
| `LessonProgressBar.vue` | Visual action completion percentage |
| `LessonStatusBadge.vue` | Extended status badge with new states |
| `AILessonAssistPanel.vue` | AI-assisted lesson capture sidebar |
| `LessonExportDialog.vue` | Format and language selection for export |
| `ProjectKickoffLessons.vue` | Recommended lessons for project kickoff |

---

## 14. Gap Traceability

| Gap # | Description | Phase | Status After |
|---|---|---|---|
| LL-1 | No action items tracking | 1 | ✅ Resolved |
| LL-2 | No process owner assignment | 1 | ✅ Resolved |
| LL-3 | No action verification | 2 | ✅ Resolved |
| LL-4 | No extended workflow states | 1 | ✅ Resolved |
| LL-5 | No root cause analysis field | 1 | ✅ Resolved |
| LL-6 | No project phase classification | 3 | ✅ Resolved |
| LL-7 | No impact type classification | 3 | ✅ Resolved |
| LL-8 | No document/procedure linkage | 1 | ✅ Resolved |
| LL-9 | No proactive dissemination | 4 | ✅ Resolved |
| LL-10 | No project kickoff integration | 4 | ✅ Resolved |
| LL-11 | No periodic review sessions | 4 | ✅ Resolved |
| LL-12 | No anonymous submission | 3 | ✅ Resolved |
| LL-13 | No cross-project reuse tracking | 4 | ✅ Resolved |
| LL-14 | No escalation for stale actions | 2 | ✅ Resolved |
| LL-15 | No analytics dashboard | 6 | ✅ Resolved |
| LL-16 | No AI-assisted capture | 5 | ✅ Resolved |
| LL-17 | Limited notification types | 2 | ✅ Resolved |
| LL-18 | No "What Went Well" field | 3 | ✅ Resolved |
| LL-19 | No Follow/Watch support | 3 | ✅ Resolved |
| LL-20 | No export capability | 3 | ✅ Resolved |

### ISO 30401 Compliance After Implementation

| Clause | Before | After | Key Enabler |
|---|---|---|---|
| 4.4 KMS Culture | 🟡 | ✅ | Contributor recognition, gamification (Phase 7) |
| 5.2 KM Policy | ❌ | ✅ | KMPolicy entity (Phase 7) |
| 6.1 Risks & Opportunities | ❌ | ✅ | KnowledgeAsset risk assessment (Phase 7) |
| 6.2 KM Objectives | ❌ | ✅ | KMObjective tracking (Phase 7) |
| 7.2 Competence | ❌ | 🟡 | Training tracking possible via KM objectives |
| 8.2 Knowledge Identification | ❌ | ✅ | KnowledgeAsset registry (Phase 7) |
| 8.3 Knowledge Capture | 🟡 | ✅ | AI-assisted capture, anonymous submission, structured root cause (Phases 1, 3, 5) |
| 8.5 Knowledge Application | 🟡 | ✅ | Action tracking, project kickoff integration, cross-project reuse (Phases 1, 4) |
| 8.6 Knowledge Retention | 🟡 | ✅ | Knowledge asset registry with risk scoring and succession indicators (Phase 7) |
| 9.1 Monitoring | 🟡 | ✅ | Analytics dashboard, action completion metrics (Phase 6) |
| 9.3 Management Review | ❌ | ✅ | Auto-generated management review report (Phase 7) |
| 10.2 Improvement | 🟡 | ✅ | Action tracking with verification, knowledge gap analysis, continuous improvement cycle |

**Overall ISO 30401 compliance**: 33% → **85%+**
