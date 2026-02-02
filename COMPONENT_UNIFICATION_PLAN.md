# Component Unification Plan

## Overview
This plan outlines the systematic unification of duplicated UI components across the KMS portal to reduce code duplication, improve maintainability, and ensure consistent user experience.

**Total Estimated Reduction:** ~2,750-3,995 lines of code

---

## Phase 1: Quick Wins (Low effort, immediate impact)

### Step 1.1: ComparisonButton Component
**Status:** Complete

**New Component:** `src/components/common/ComparisonButton.vue`

**Affected Pages:**
- `src/pages/articles/ArticlesPage.vue`
- `src/pages/documents/DocumentsPage.vue`
- `src/pages/media/MediaCenterPage.vue`
- `src/pages/events/EventsPage.vue`
- `src/pages/learning/LearningPage.vue`

**Reduction:** ~100-150 lines

---

### Step 1.2: usePagination Composable
**Status:** Complete

**New Composable:** `src/composables/usePagination.ts`

**Affected Pages:**
- `src/pages/articles/ArticlesPage.vue`
- `src/pages/documents/DocumentsPage.vue`
- `src/pages/media/MediaCenterPage.vue`
- `src/pages/events/EventsPage.vue`
- `src/pages/learning/LearningPage.vue`
- `src/pages/search/SearchResultsPage.vue`

**Reduction:** ~105-175 lines

---

### Step 1.3: Badge Components Suite
**Status:** Complete

**New Components:**
- `src/components/common/CategoryBadge.vue`
- `src/components/common/StatusBadge.vue`
- `src/components/common/TagBadge.vue`

**Affected Pages:**
- `src/pages/articles/ArticlesPage.vue`
- `src/pages/documents/DocumentsPage.vue`
- `src/pages/media/MediaCenterPage.vue`
- `src/pages/events/EventsPage.vue`
- `src/pages/learning/LearningPage.vue`

**Reduction:** ~200-360 lines

---

## Phase 2: Medium Effort Components

### Step 2.1: CardActionButtons Component
**Status:** Component Created (Application Deferred)

**New Component:** `src/components/common/CardActionButtons.vue`

**Note:** Component created with support for like, bookmark, save, share, download, star, compare, ai-analyze, calendar, and reminder actions. Each page has highly customized CSS styling for action buttons - full application deferred to allow gradual adoption in new features.

**Affected Pages:**
- `src/pages/articles/ArticlesPage.vue`
- `src/pages/documents/DocumentsPage.vue`
- `src/pages/media/MediaCenterPage.vue`
- `src/pages/events/EventsPage.vue`
- `src/pages/learning/LearningPage.vue`

**Reduction:** ~125-200 lines (when fully applied)

---

### Step 2.2: ShareContentModal Component
**Status:** Complete

**New Component:** `src/components/common/ShareContentModal.vue`

**Features:**
- Social sharing: Twitter, LinkedIn, Facebook, WhatsApp, Email
- Copy link with success feedback
- QR code generation toggle
- Content preview with image
- Built on existing useSharing composable

**Affected Pages:**
- `src/pages/articles/ArticlesPage.vue` ✓
- `src/pages/documents/DocumentsPage.vue` ✓
- `src/pages/media/MediaCenterPage.vue` ✓
- `src/pages/events/EventsPage.vue` ✓
- `src/pages/learning/LearningPage.vue` ✓

**Reduction:** ~150-300 lines

---

### Step 2.3: SkeletonLoader Component
**Status:** Complete (Component Created, Imports Added)

**New Component:** `src/components/common/SkeletonLoader.vue`

**Features:**
- Multiple skeleton types: card, list-item, text, avatar, thumbnail, badge, button
- Content-type presets: article, document, media, event, course
- Configurable columns (1-4), count, animations
- Optional image, badge, and avatar placeholders

**Affected Pages:**
- `src/pages/articles/ArticlesPage.vue` ✓
- `src/pages/documents/DocumentsPage.vue` ✓
- `src/pages/media/MediaCenterPage.vue` ✓
- `src/pages/events/EventsPage.vue` ✓
- `src/pages/learning/LearningPage.vue` ✓
- `src/pages/DashboardPage.vue`

**Note:** Component imported and ready for use. Active skeleton states to be added when API loading is implemented.

**Reduction:** ~150-250 lines

---

## Phase 3: High Impact (Most effort, biggest payoff)

### Step 3.1: ContentCard Component
**Status:** Pending

**New Component:** `src/components/common/ContentCard.vue`

**Affected Pages:**
- `src/pages/articles/ArticlesPage.vue`
- `src/pages/documents/DocumentsPage.vue`
- `src/pages/media/MediaCenterPage.vue`
- `src/pages/events/EventsPage.vue`
- `src/pages/learning/LearningPage.vue`
- `src/pages/DashboardPage.vue`
- `src/pages/collections/CollectionsPage.vue`

**Reduction:** ~1,500-2,000 lines

---

### Step 3.2: useFilters Composable + Enhanced PageFilterBar
**Status:** Pending

**New/Modified:**
- `src/composables/useFilters.ts` (new)
- `src/components/common/PageFilterBar.vue` (enhance)

**Affected Pages:**
- `src/pages/articles/ArticlesPage.vue`
- `src/pages/documents/DocumentsPage.vue`
- `src/pages/media/MediaCenterPage.vue`
- `src/pages/events/EventsPage.vue`
- `src/pages/learning/LearningPage.vue`
- `src/pages/collections/CollectionsPage.vue`
- `src/pages/polls/PollsPage.vue`

**Reduction:** ~420-560 lines

---

## Progress Tracking

| Step | Component | Status | Lines Reduced | Commit |
|------|-----------|--------|---------------|--------|
| 1.1 | ComparisonButton | Complete | ~167 lines | - |
| 1.2 | usePagination | Complete | ~150 lines | - |
| 1.3 | Badge Suite | In Progress | - | - |
| 2.1 | CardActionButtons | Pending | - | - |
| 2.2 | ShareContentModal | Pending | - | - |
| 2.3 | SkeletonLoader | Pending | - | - |
| 3.1 | ContentCard | Pending | - | - |
| 3.2 | useFilters | Pending | - | - |

---

## Previously Completed Unifications

| Component | Commit | Lines Reduced |
|-----------|--------|---------------|
| ViewAllButton | 57f6bb2 | ~200 lines |
| EmptyState | 1c21a12 | ~300 lines |
| Pagination | ab132da | ~650 lines |

---

## Notes
- Each step requires approval before committing
- Test changes in browser before marking complete
- Update this document after each commit
