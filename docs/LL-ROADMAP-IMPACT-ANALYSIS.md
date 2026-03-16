# Impact Analysis: Lessons Learned Enhancement on Main Implementation Roadmap

An analysis of how the Lessons Learned & ISO 30401 implementation plan affects the main AFC27 KMS Implementation Roadmap — identifying overlaps, synergies, conflicts, shared infrastructure, new gaps, and a revised consolidated timeline.

---

## Table of Contents

1. [Impact Overview](#1-impact-overview)
2. [Shared Infrastructure & Synergies](#2-shared-infrastructure--synergies)
3. [File-Level Conflicts & Merge Points](#3-file-level-conflicts--merge-points)
4. [New Scope Not in Original Roadmap](#4-new-scope-not-in-original-roadmap)
5. [Dependency Chain Effects](#5-dependency-chain-effects)
6. [Timeline Impact](#6-timeline-impact)
7. [Effort Impact](#7-effort-impact)
8. [Migration Sequence Impact](#8-migration-sequence-impact)
9. [Phase-by-Phase Cross-Reference](#9-phase-by-phase-cross-reference)
10. [Risk Analysis](#10-risk-analysis)
11. [Consolidated Roadmap Recommendation](#11-consolidated-roadmap-recommendation)

---

## 1. Impact Overview

### Scale of Change

| Metric | Main Roadmap (Original) | LL Enhancement Plan | Combined |
|---|---|---|---|
| Total gaps addressed | 49 | 20 (LL) + ISO 30401 | 69 (some overlap) |
| New entities | ~15 | 5 (LessonAction, LessonApplication, KnowledgeAsset, KMObjective, KMPolicy) | ~20 |
| New API endpoints | 42 | 42 | 84 (some shared) |
| Database migrations | 13 | 5 | 18 (sequencing needed) |
| Modified existing entities | ~12 | 3 (LessonLearned, LessonStatus, FollowableType) | ~15 |
| Backend effort (days) | 255 | ~80 | ~320 |
| Frontend effort (days) | 230 | ~55 | ~270 |
| Calendar weeks | 40 | 18 | 44–48 (with integration overhead) |

### Impact Assessment: **Moderate-High**

The LL plan does **not** fundamentally restructure the main roadmap but introduces:
- 5 entirely new entities not accounted for in the original 49 gaps
- ISO 30401 governance features (KnowledgeAsset, KMObjective, KMPolicy) that are a new scope area
- Significant additional work on the `Notification.cs`, `Follow.cs`, and `Collaboration` module files that the main roadmap also modifies
- Additional background jobs competing for worker infrastructure

---

## 2. Shared Infrastructure & Synergies

### 2.1 Notification System Extensions

**Conflict zone**: Both plans extend the `NotificationType` enum in `Notification.cs`

| Source | Notification Types Added |
|---|---|
| Main Roadmap Phase 1B | `VerificationDue`, `VerificationOverdue`, `VerificationCompleted` |
| LL Plan Phase 2 | `LessonSubmitted`, `LessonRejected`, `LessonPublished`, `LessonActionAssigned`, `LessonActionOverdue`, `LessonActionCompleted`, `LessonActionsAllComplete` |

**Synergy**: Both add to the same enum. No technical conflict — just coordinate the enum value ordering. Both leverage the same 6-channel delivery infrastructure (InApp, Email, Push, SMS, Teams, Slack) and BroadcastNotification system.

**Recommendation**: Combine all notification type additions into a single migration. Apply article verification types AND lesson lifecycle types together.

---

### 2.2 `Follow.cs` File Modifications

**Conflict zone**: Both plans modify the same file — `Collaboration/Domain/Entities/Follow.cs`

| Source | Changes to Follow.cs |
|---|---|
| Main Roadmap Phase 3C | Wire existing `Follow` entity to frontend (Favorites UI) — no entity changes needed |
| LL Plan Phase 3 | Add `LessonLearned` to `FollowableType` enum |
| LL Plan Phase 1 | Extend `LessonLearned` class with `ProcessOwnerId`, `RootCause`, `Actions` navigation, new domain methods |
| LL Plan Phase 3 | Extend `LessonLearned` class with `WhatWentWell`, `ProjectPhase`, `ImpactType`, `IsAnonymous` |

**Synergy**: The main roadmap's Favorites UI (Phase 3C) becomes more valuable once `LessonLearned` is added to `FollowableType` — users can bookmark lessons. No technical conflict, but all changes to `Follow.cs` should be coordinated to avoid merge conflicts.

**Recommendation**: Batch all `Follow.cs` entity changes into a single development sprint. Add `LessonLearned` to `FollowableType` at the same time as the Favorites UI work.

---

### 2.3 AI Writing & Translation Infrastructure

**Synergy**: The main roadmap's Phase 1A (`AIWritingAssistantController`) builds the exact infrastructure that the LL plan's Phase 5 (AI-assisted lesson capture) needs.

| Main Roadmap | LL Plan | Relationship |
|---|---|---|
| Phase 1A: `AIWritingAssistantController` with generate, improve, translate, stream endpoints | LL Phase 5: AI-assisted lesson capture, root cause suggestion, document impact analysis | LL Phase 5 can **reuse** the AIWritingAssistantController's generate/improve/translate endpoints for lesson content |
| Phase 2A: `TranslationController` for article/document translation | LL Phase 5.4: AI translation for lessons | LL translation is a direct consumer of the TranslationController — extend it to accept `LessonLearned` as a source type |

**Recommendation**: LL Phase 5 (AI intelligence) should be scheduled AFTER main roadmap Phase 1A + 2A are complete. The LL AI features become a thin layer on top of already-built AI infrastructure rather than duplicating it.

---

### 2.4 Export Infrastructure

**Synergy**: The main roadmap's Phase 3B (`ArticleExportService` for PDF/DOCX/Markdown) builds the export pipeline that lesson export (LL Phase 3.5) needs.

| Main Roadmap | LL Plan | Relationship |
|---|---|---|
| Phase 3B: `ArticleExportService` with HTML→DOCX (OpenXml), HTML→PDF, HTML→Markdown | LL Phase 3.5: Lesson export to PDF/DOCX/Markdown | LL export reuses the same conversion pipeline — just needs a `LessonExportService` that renders lesson content to HTML, then feeds into the same converters |

**Recommendation**: Build export infrastructure generically in Phase 3B (accept any structured content, not just articles). Lesson export becomes a trivial extension.

---

### 2.5 Background Jobs & Workers

Both plans add background jobs that need scheduling:

| Source | Background Job | Worker |
|---|---|---|
| Main Roadmap Phase 1B | `KnowledgeVerificationReminderJob` (daily) | NotificationWorker or new SchedulingWorker |
| Main Roadmap Phase 2F | `ContentHealthCalculationJob` (weekly) | New or AIWorker |
| LL Plan Phase 2 | `LessonActionEscalationJob` (daily) | NotificationWorker |
| LL Plan Phase 6 | Analytics aggregation | Scheduled job |
| LL Plan Phase 7 | KM objective measurement | Scheduled job |

**Recommendation**: Create a unified `SchedulingWorker` project (or extend `NotificationWorker`) to host all scheduled/cron jobs. Don't create a separate worker per job.

---

### 2.6 Analytics Dashboards

Both plans create analytics dashboards:

| Source | Dashboard | Scope |
|---|---|---|
| Main Roadmap Phase 2G | Knowledge Health Dashboard | Article freshness, verification, content health scores |
| Main Roadmap Phase 6C | Unified Adoption Analytics | Active users, content creation, search/AI usage |
| Main Roadmap Phase 7C | Unified Admin Dashboard | System health, user mgmt, audit, storage |
| LL Plan Phase 6 | Lessons Learned Dashboard | Lesson volume, action completion, contributors |
| LL Plan Phase 7 | KM Review Report | KM objectives, knowledge assets, organizational learning |

**Synergy**: These dashboards share the same frontend patterns (PrimeVue charts, data tables, filters) and backend patterns (aggregation endpoints). The Lessons Learned dashboard can be a tab or section within the Knowledge Health Dashboard rather than a standalone view.

**Recommendation**: Design a modular dashboard framework in Phase 2G that supports pluggable widgets. Each subsequent dashboard (LL, adoption, admin) adds widgets to the framework rather than building from scratch.

---

### 2.7 Verification & Freshness Patterns

**Strong synergy**: The main roadmap's article verification (Phase 1B) and the LL plan's action verification (Phase 1) follow nearly identical patterns.

| Concept | Article Verification (Main) | Action Verification (LL) |
|---|---|---|
| Owner assignment | `Article.OwnerId` | `LessonAction.AssigneeId` |
| Status tracking | `VerificationStatus` enum | `LessonActionStatus` enum |
| Due date tracking | `NextVerificationDue` | `DueDate` |
| Overdue detection | `IsOverdue` computed | `IsOverdue` computed |
| Reminder/escalation | `KnowledgeVerificationReminderJob` | `LessonActionEscalationJob` |
| Completion record | `VerificationRecord` entity | `CompletedAt`, `CompletionNotes` |
| Manager escalation | `User.ManagerId` | `User.ManagerId` |

**Recommendation**: Abstract the common "tracked accountability item" pattern into shared infrastructure:
- Shared `IEscalatable` interface with `DueDate`, `IsOverdue`, `EscalatedAt`
- Shared `EscalationService` that both article verification and lesson action escalation use
- Single background job `AccountabilityReminderJob` that processes both types

---

## 3. File-Level Conflicts & Merge Points

### Critical Files Modified by Both Plans

| File | Main Roadmap Changes | LL Plan Changes | Conflict Risk |
|---|---|---|---|
| `Collaboration/Domain/Entities/Follow.cs` | No entity changes (Favorites is frontend-only) | Extend `LessonLearned` entity + add to `FollowableType` | **Low** — no overlapping modifications |
| `Notifications/Domain/Entities/Notification.cs` | Add 3 verification notification types | Add 7 lesson lifecycle notification types | **Low** — both add to same enum, coordinate ordering |
| `Collaboration/Application/DTOs/CommentDto.cs` | No changes | Extend `LessonLearnedDto`, `CreateLessonLearnedRequest` | **Low** — LL plan owns these DTOs |
| `Collaboration/Presentation/Controllers/LessonsLearnedController.cs` | No changes | Add 17+ new endpoints | **None** — LL plan is sole modifier |
| `Collaboration/CollaborationModule.cs` | No changes | Register new services, policies | **Low** — additive |
| `Search/Domain/Entities/SearchDocument.cs` | No changes | No changes | **None** |
| `AI/Presentation/Controllers/SemanticSearchController.cs` | No changes | Extend for lesson-specific AI features | **Low** — new endpoints, not modifications |
| `WebApi/Features/Templates/Models/DocumentTemplateModels.cs` | Phase 3D: Add `ReviewIntervalDays`, `DefaultOwnerId` | LL references these for template governance | **Synergy** — same change serves both plans |

### Verdict: No High-Risk Merge Conflicts

Both plans modify different areas of the codebase with minimal overlap. The only coordination needed is on shared enums (`NotificationType`, `FollowableType`) where both plans add values.

---

## 4. New Scope Not in Original Roadmap

The LL plan introduces **5 new scope items** not accounted for in the original 49-gap roadmap:

### 4.1 ISO 30401 Governance Entities (Entirely New)

| Entity | Purpose | Original Roadmap Coverage |
|---|---|---|
| `KnowledgeAsset` | Critical knowledge registry with risk scoring, holders, and succession planning | **None** — not in any gap |
| `KMObjective` | KM objectives with targets, measurement, and status tracking | **None** — not in any gap |
| `KMPolicy` | KM policy documents with versioning and acknowledgment tracking | **None** — not in any gap |
| `KMPolicyAcknowledgment` | Track user acknowledgment of KM policies | **None** — not in any gap |

**Impact**: These are entirely new features that add **~25 backend days + ~15 frontend days = ~40 days** to the total effort. They should be added as a new phase or appended to the main roadmap's Phase 7 (Admin, Compliance & External).

### 4.2 `LessonAction` Entity (New)

The main roadmap did not identify lesson action tracking as a gap because the gap analysis focused on platform-wide KM capabilities, not on enhancing specific existing features.

**Impact**: ~20 backend days + ~15 frontend days = ~35 days. This is the largest single deliverable in the LL plan.

### 4.3 `LessonApplication` Entity (New)

Cross-project lesson reuse tracking was not in the original gap analysis.

**Impact**: ~5 days. Small entity with simple endpoints.

### 4.4 Contributor Recognition / Gamification

The LL plan's Phase 7 introduces contributor scoring, badges, and leaderboards for KM culture enablers (ISO 30401 Clause 4.4). The main roadmap mentions no gamification.

**Impact**: ~10 backend days + ~10 frontend days = ~20 days.

### Total New Scope: ~100 effort-days

---

## 5. Dependency Chain Effects

### 5.1 LL Plan Dependencies on Main Roadmap

| LL Phase | Depends On | Main Roadmap Phase | Impact |
|---|---|---|---|
| LL Phase 4 (Dissemination) | Semantic search for finding related projects | Main Phase 2E (Permission-Aware AI) | LL Phase 4 cannot start until main Phase 2E is done. **Delays LL Phase 4 to week 12+ minimum.** |
| LL Phase 5 (AI Intelligence) | AI writing infrastructure | Main Phase 1A (Inline AI Writing) | LL Phase 5 should follow main Phase 1A. **Delays LL Phase 5 to week 8+ minimum.** |
| LL Phase 5.4 (AI Translation) | Translation controller | Main Phase 2A (General AI Translation) | LL translation is an extension of main translation. **Delays to week 10+.** |
| LL Phase 3.5 (Export) | Export infrastructure | Main Phase 3B (Article Export) | LL export reuses article export pipeline. **Delays to week 12+.** |
| LL Phase 7 (ISO 30401) | Spaces for knowledge asset scoping | Main Phase 0A (Spaces) | LL Phase 7 needs Spaces for `KnowledgeAsset.RelatedSpaceId`. **Delays to week 4+.** |

### 5.2 Main Roadmap Dependencies on LL Plan

| Main Phase | Benefits From | LL Phase | Impact |
|---|---|---|---|
| Main Phase 2G (Knowledge Health Dashboard) | Lesson action completion rates as a health metric | LL Phase 1 (Action Tracking) | The health dashboard is more complete if lesson action metrics are available. **Positive synergy if LL Phase 1 is done first.** |
| Main Phase 6C (Adoption Analytics) | Lesson contribution as an adoption metric | LL Phase 6 (Analytics) | Adoption dashboard can include lesson creation/usefulness metrics. **Additive, not blocking.** |
| Main Phase 7C (Unified Admin Dashboard) | KM governance metrics | LL Phase 7 (ISO 30401) | Admin dashboard can include KM objectives, knowledge assets. **Additive, not blocking.** |

### 5.3 Revised Dependency Graph

```
Main Phase 0A: Spaces ──────────────────┬──► Main Phase 4A-D
                                        ├──► Main Phase 2E: Permission-Aware AI ──► LL Phase 4: Dissemination
                                        ├──► Main Phase 5A: Knowledge Agents
                                        └──► LL Phase 7: ISO 30401 (KnowledgeAsset.RelatedSpaceId)

Main Phase 0B: Workflow Engine ─────────┬──► Main Phase 1B: Verification
                                        ├──► Main Phase 8E: Automation Rules
                                        └──► LL Phase 1: Action Tracking (optional integration)

Main Phase 1A: Inline AI Writing ───────┬──► Main Phase 2A: AI Translation ──► LL Phase 5.4: Lesson Translation
                                        ├──► Main Phase 2B: AI Generation
                                        └──► LL Phase 5: AI Lesson Intelligence

Main Phase 3B: Article Export ──────────└──► LL Phase 3.5: Lesson Export

LL Phase 1: Action Tracking ────────────┬──► LL Phase 2: Escalation & Notifications
                                        ├──► LL Phase 6: Analytics Dashboard
                                        └──► Main Phase 2G: Health Dashboard (enrichment)
```

---

## 6. Timeline Impact

### Original Timeline

| Period | Main Roadmap Phases |
|---|---|
| Weeks 1–4 | Phase 0: Foundation |
| Weeks 3–8 | Phase 1: Critical (P0) |
| Weeks 5–12 | Phase 2: AI & Search |
| Weeks 9–16 | Phase 3: Collaboration |
| Weeks 13–20 | Phase 4: Access Control |
| Weeks 17–24 | Phase 5: Advanced AI |
| Weeks 21–28 | Phase 6: Analytics |
| Weeks 25–32 | Phase 7: Admin & Compliance |
| Weeks 29–36 | Phase 8: UX Enhancements |
| Weeks 33–40 | Phase 9: External Channels |
| **Total**: ~40 weeks | |

### LL Plan Insertion Points

| LL Phase | Optimal Insertion | Rationale |
|---|---|---|
| LL Phase 1 (Action Tracking) | **Weeks 1–4** (parallel with Main Phase 0) | No dependencies on main roadmap. Can run independently. P0 priority justifies immediate start. |
| LL Phase 2 (Escalation) | **Weeks 3–6** (follows LL Phase 1) | Depends only on LL Phase 1. |
| LL Phase 3 (Classification) | **Weeks 5–8** (parallel with Main Phase 1) | Mostly independent. Export work waits for Main Phase 3B. |
| LL Phase 4 (Dissemination) | **Weeks 13–16** (after Main Phase 2E) | Depends on permission-aware AI from main roadmap. |
| LL Phase 5 (AI Intelligence) | **Weeks 9–12** (after Main Phase 1A) | Depends on AI writing infrastructure. |
| LL Phase 6 (Analytics) | **Weeks 13–16** (after LL Phase 1+2) | Depends on action tracking data being in production. |
| LL Phase 7 (ISO 30401) | **Weeks 25–32** (aligns with Main Phase 7) | ISO 30401 governance fits naturally with Admin & Compliance phase. |

### Revised Combined Timeline

| Weeks | Main Roadmap | LL Enhancement | Parallel? |
|---|---|---|---|
| 1–4 | Phase 0: Foundation (Spaces, Workflow) | **LL Phase 1: Action Tracking** | Yes |
| 3–6 | Phase 0 tail | **LL Phase 2: Escalation & Notifications** | Yes |
| 3–8 | Phase 1: Critical (Inline AI, Verification) | **LL Phase 3: Classification & Content** | Partial |
| 5–12 | Phase 2: AI & Search | LL Phase 3 tail (export waits for 3B) | Yes |
| 9–12 | Phase 2 continues | **LL Phase 5: AI Intelligence** | Yes |
| 9–16 | Phase 3: Collaboration | — | — |
| 13–16 | Phase 3 continues | **LL Phase 4: Dissemination** + **LL Phase 6: Analytics** | Yes |
| 13–20 | Phase 4: Access Control | — | — |
| 17–24 | Phase 5: Advanced AI | — | — |
| 21–28 | Phase 6: Analytics & Integration | — | — |
| 25–32 | Phase 7: Admin & Compliance | **LL Phase 7: ISO 30401 Governance** | Yes |
| 29–36 | Phase 8: UX Enhancements | — | — |
| 33–40 | Phase 9: External Channels | — | — |
| 37+ | Phase 10: Low Priority | — | — |

### Net Timeline Impact: **+4 to 8 weeks**

The LL enhancement adds approximately 4–8 weeks to the overall calendar, primarily because:
- LL Phases 1–3 can run fully in parallel with Main Phases 0–1 (separate module, separate files)
- LL Phase 7 (ISO 30401) adds 4 weeks of new work to the Admin/Compliance phase
- Some LL phases are blocked by main roadmap dependencies (dissemination waits for AI, export waits for export infrastructure)

**Revised total**: ~44–48 weeks (from original ~40 weeks)

---

## 7. Effort Impact

### Additional Effort from LL Plan

| LL Phase | Backend (days) | Frontend (days) | Total |
|---|---|---|---|
| Phase 1: Action Tracking | 15 | 15 | 30 |
| Phase 2: Escalation | 8 | 3 | 11 |
| Phase 3: Classification | 10 | 10 | 20 |
| Phase 4: Dissemination | 8 | 5 | 13 |
| Phase 5: AI Intelligence | 10 | 8 | 18 |
| Phase 6: Analytics | 8 | 12 | 20 |
| Phase 7: ISO 30401 | 20 | 15 | 35 |
| **LL Total** | **79** | **68** | **147** |

### Effort Reduction (Synergies)

| Synergy | Effort Saved | Reason |
|---|---|---|
| LL export reuses Article Export infrastructure | -5 days | Same DOCX/PDF pipeline |
| LL AI features reuse AIWritingAssistantController | -8 days | Extend existing endpoints, don't rebuild |
| LL notifications reuse same enum + infrastructure | -3 days | No new delivery channel work |
| LL escalation shares pattern with verification reminders | -4 days | Shared EscalationService |
| LL analytics widget reuses dashboard framework | -5 days | Pluggable widget, not standalone view |
| **Total saved** | | **-25 days** |

### Net Additional Effort

| Metric | Original | LL Addition | Synergy Savings | Revised |
|---|---|---|---|---|
| Backend days | 255 | +79 | -15 | **319** |
| Frontend days | 230 | +68 | -10 | **288** |
| **Total days** | **485** | **+147** | **-25** | **607** |

**Net increase: +122 effort-days (+25%)**

---

## 8. Migration Sequence Impact

### Original Migration Sequence (13 migrations)

```
0A: AddSpaces → 0B: GeneralizeWorkflow → 1B: AddVerification → 2F: AddHealthScore
→ 3A: AddInlineComments → 3D: ExtendTemplateMetadata → 4B: AddGuestUsers
→ 5A: AddKnowledgeAgents → 6E: ExtendTags → 6F: AddContentLinks
→ 7B: AddDLP → 7D: AddShareableLinks → 8E: AddAutomationRules
```

### LL Plan Migrations (5 migrations)

```
LL-1: AddLessonActions → LL-2: ExtendLessonStatus → LL-3: AddLessonClassification
→ LL-4: AddLessonApplication → LL-5: AddKnowledgeGovernance
```

### Revised Combined Sequence (18 migrations)

```
Week 1:  0A: AddSpaces
Week 1:  LL-1: AddLessonActions + AddLessonRootCause (can run parallel — different tables)
Week 2:  0B: GeneralizeWorkflow
Week 2:  LL-2: ExtendLessonStatus (enum change — coordinate with codebase)
Week 4:  1B: AddVerification
Week 5:  LL-3: AddLessonClassification
Week 8:  2F: AddHealthScore
Week 10: 3A: AddInlineComments
Week 10: 3D: ExtendTemplateMetadata
Week 13: LL-4: AddLessonApplication
Week 15: 4B: AddGuestUsers
Week 18: 5A: AddKnowledgeAgents
Week 22: 6E: ExtendTags
Week 22: 6F: AddContentLinks
Week 26: 7B: AddDLP
Week 26: 7D: AddShareableLinks
Week 28: LL-5: AddKnowledgeGovernance
Week 30: 8E: AddAutomationRules
```

**Impact**: Migration count increases from 13 to 18. No conflicts — all migrations are additive (new tables + nullable columns). LL migrations touch the `LessonLearned` table exclusively (except `LL-5` which creates new governance tables), so they don't conflict with main roadmap migrations that touch `Article`, `Comment`, `Tag`, `User`, etc.

---

## 9. Phase-by-Phase Cross-Reference

### Detailed Interaction Matrix

| Main Phase | LL Phase | Interaction Type | Action Required |
|---|---|---|---|
| **0A: Spaces** | LL-7: ISO 30401 | LL needs `SpaceId` on `KnowledgeAsset` | Add `RelatedSpaceId` to KnowledgeAsset |
| **0B: Workflow** | LL-1: Action Tracking | LL chose enum extension over workflow engine | No conflict — LL actions are self-contained. Optional future integration point. |
| **1A: Inline AI** | LL-5: AI Intelligence | LL AI features build on top of AI Writing infrastructure | Schedule LL-5 after Main 1A. Reuse `AIWritingAssistantController`. |
| **1B: Verification** | LL-1: Action Tracking | Both implement accountability patterns (owner, due date, escalation) | Share `EscalationService`. Coordinate background job scheduling. |
| **2A: Translation** | LL-5.4: Lesson Translation | LL translation extends article translation | Design `TranslationController` to accept entity type parameter, not just articles. |
| **2D: AI Flow-of-Work** | LL-5: AI Intelligence | The global AI panel should be able to assist with lesson capture | When AI panel detects user is on lessons page, offer lesson-specific quick actions. |
| **2E: Permission AI** | LL-4: Dissemination | LL dissemination uses semantic search to find related projects | LL Phase 4 depends on this. Schedule accordingly. |
| **2F: Health Metrics** | LL-6: Analytics | Lesson action rates are a health metric input | Include `LessonActionCompletionRate` in content health scoring algorithm. |
| **2G: Health Dashboard** | LL-6: Analytics | Both create analytics dashboards | Lessons dashboard should be a tab/section within the Knowledge Health Dashboard. |
| **3B: Article Export** | LL-3.5: Lesson Export | Both need DOCX/PDF/MD export pipeline | Build export generically. `IContentExportService` interface with `ArticleExportService` and `LessonExportService` implementations. |
| **3C: Favorites UI** | LL-3.4: FollowableType | LL adds `LessonLearned` to `FollowableType` | Coordinate enum change. Favorites UI automatically supports lessons once enum is extended. |
| **3D: Template Governance** | LL-1B: Template Review | Both add `ReviewIntervalDays` and `DefaultOwnerId` to templates | **Same change** — serves both article verification and lesson governance. Implement once. |
| **5A: Knowledge Agents** | LL-5: AI Intelligence | A "Lessons Learned Agent" is a natural agent type | Add a pre-configured "Lessons Learned Agent" scoped to LessonsLearned search scope. |
| **6B: Activity Feed** | LL-4: Dissemination | Lesson publication should appear in activity feed | Add `LessonPublished`, `LessonActionCompleted` to `ActivityType` enum. |
| **6C: Adoption Analytics** | LL-6: Analytics | Lesson contributions are an adoption metric | Include lesson authoring, action completion in adoption metrics. |
| **7C: Admin Dashboard** | LL-7: ISO 30401 | KM governance widgets for admin dashboard | Add KM objectives status widget and knowledge asset health widget to admin dashboard. |

---

## 10. Risk Analysis

### Risk 1: Scope Creep from ISO 30401

**Risk**: ISO 30401 governance features (KnowledgeAsset, KMObjective, KMPolicy) are entirely new scope. They could expand further as requirements are refined.

**Mitigation**: Timebox ISO 30401 implementation to 4 weeks. Implement minimum viable versions of each entity. Defer advanced features (risk scoring algorithms, succession planning workflows) to Phase 10.

**Impact on roadmap**: +4 weeks, +40 days effort.

### Risk 2: Collaboration Module Overloading

**Risk**: The Collaboration module already contains Community, Discussion, Comment, Follow, Mention, Like, AND LessonLearned. Adding LessonAction, LessonApplication, and extending LessonLearned significantly increases this module's size and complexity.

**Mitigation**: Consider extracting Lessons Learned into its own module (`AFC27.KMS.LessonsLearned`) if the entity count exceeds 6–8 in the Collaboration module. The current plan adds 2 new entities (LessonAction, LessonApplication) to an already-large module.

**Impact on roadmap**: 2–3 days if extraction is needed. No timeline impact if module stays as-is.

### Risk 3: Background Job Contention

**Risk**: Multiple daily/weekly background jobs (verification reminders, action escalation, health calculation, AI retention, analytics aggregation) competing for worker resources.

**Mitigation**: Stagger job schedules. Use the existing MassTransit delayed message scheduling rather than cron-based polling. Consider a dedicated `ScheduledWorker` project.

**Impact on roadmap**: 2–3 days for scheduling infrastructure. No timeline impact.

### Risk 4: Notification Fatigue

**Risk**: Adding 10 new notification types (3 verification + 7 lesson lifecycle) risks overwhelming users with notifications.

**Mitigation**: All new types must respect per-channel/per-category notification preferences. Default lesson action notifications to email-only with daily digest. Default verification reminders to in-app only. Allow users to mute specific types.

**Impact on roadmap**: No additional effort — existing preference system handles this.

### Risk 5: LL Plan Phase 5 Dependency on Unbuilt AI Infrastructure

**Risk**: LL Phase 5 (AI-assisted lesson capture) depends on Main Phase 1A (AI Writing) which may be delayed.

**Mitigation**: LL Phases 1–3 have zero dependencies on AI infrastructure. Start there. If Main Phase 1A is delayed, LL Phase 5 simply slides without affecting the P0/P1 lesson improvements.

**Impact on roadmap**: None if dependency is respected. Risk only if LL Phase 5 is forced early.

---

## 11. Consolidated Roadmap Recommendation

### Recommended Approach: Interleave, Don't Append

The LL enhancement should be **interleaved** into the main roadmap, not appended as a separate track. This:
- Avoids duplicate infrastructure builds
- Maximizes synergies (shared escalation, shared export, shared dashboards)
- Delivers LL P0 gaps (action tracking) in the same timeframe as main P0 gaps (AI writing, verification)
- Aligns ISO 30401 governance with the admin/compliance phase

### Revised Phase Structure

| Phase | Weeks | Original Scope | Added LL Scope |
|---|---|---|---|
| **0: Foundation** | 1–4 | Spaces, Workflow Engine | **+ LL Action Tracking (entity, endpoints, DTOs)** |
| **1: Critical** | 3–8 | Inline AI Writing, Verification | **+ LL Escalation, LL Classification, LL FollowableType** |
| **2: AI & Search** | 5–12 | Translation, Generation, Q&A Search, Permission AI, Health | **+ LL AI Intelligence (after 1A complete)** |
| **3: Collaboration** | 9–16 | Inline Comments, Export, Favorites, Templates | **+ LL Export (reuse infrastructure), LL Dissemination** |
| **4: Access Control** | 13–20 | Space Permissions, Guest Access, Delegation | (no LL items) |
| **5: Advanced AI** | 17–24 | Knowledge Agents, Research Mode, Data Retention | **+ "Lessons Learned Agent" as default agent** |
| **6: Analytics** | 21–28 | Federated Search, Activity Feed, Adoption, Tags, Backlinks | **+ LL Analytics Dashboard (as widget in Health Dashboard)** |
| **7: Admin & Compliance** | 25–32 | SCIM, DLP, Admin Dashboard, Shareable Links | **+ ISO 30401: KnowledgeAsset, KMObjective, KMPolicy, Contributor Recognition** |
| **8–10** | 29+ | UX, External, Low Priority | (no LL items) |

### Key Modifications to Original Roadmap

1. **Phase 0 expanded** (+1 week): Add LessonAction entity build alongside Spaces. Both are foundational entity work on different modules — fully parallelizable.

2. **Phase 1 expanded** (+1 week): Add LL notification types and classification fields alongside article verification. Same pattern, different entity.

3. **Phase 3B made generic**: `ArticleExportService` becomes `ContentExportService` with `IExportableContent` interface. Serves articles AND lessons.

4. **Phase 3D shared**: Template governance changes (`ReviewIntervalDays`, `DefaultOwnerId`) serve BOTH article verification and lesson governance. Implement once.

5. **Phase 5A enriched**: Add pre-configured "Lessons Learned Agent" as one of the default knowledge agents.

6. **Phase 6 enriched**: LL Analytics becomes a tab in the Knowledge Health Dashboard rather than a separate view.

7. **Phase 7 expanded** (+4 weeks): ISO 30401 governance entities added. This is genuinely new scope with no overlap.

### Revised Effort Summary

| Phase | Original (days) | LL Addition (days) | Synergy Savings (days) | Revised (days) |
|---|---|---|---|---|
| 0: Foundation | 35 | +30 (LessonAction) | 0 | **65** |
| 1: Critical | 55 | +20 (Escalation, Classification) | -3 (shared notifications) | **72** |
| 2: AI & Search | 55 | +18 (AI Intelligence) | -8 (reuse AI writing) | **65** |
| 3: Collaboration | 45 | +13 (Export, Dissemination) | -5 (shared export) | **53** |
| 4: Access Control | 40 | 0 | 0 | **40** |
| 5: Advanced AI | 45 | +3 (LL Agent) | 0 | **48** |
| 6: Analytics | 40 | +15 (LL Dashboard) | -5 (widget reuse) | **50** |
| 7: Admin & Compliance | 40 | +35 (ISO 30401) | -4 (shared escalation) | **71** |
| 8: UX | 45 | 0 | 0 | **45** |
| 9: External | 45 | 0 | 0 | **45** |
| 10: Low Priority | 40 | 0 | 0 | **40** |
| **Total** | **485** | **+134** | **-25** | **594** |

### Final Metrics

| Metric | Original Roadmap | With LL Enhancement | Delta |
|---|---|---|---|
| Total gaps | 49 | 69 | +20 |
| Total effort (days) | 485 | 594 | +109 (+22%) |
| Calendar weeks | ~40 | ~44–48 | +4–8 weeks |
| Database migrations | 13 | 18 | +5 |
| New API endpoints | 42 | ~75 | +33 |
| New entities | ~15 | ~20 | +5 |
| ISO 30401 compliance | Not targeted | 85%+ | New capability |
| Lessons Learned maturity | Basic (capture + approve) | Full lifecycle (capture → action → verify → archive) | Transformational |
