# Portal Redesign Plan
## AFC Asian Cup 2027 - Knowledge Management System

**Document Version**: 1.2
**Created**: December 11, 2024
**Last Updated**: December 11, 2024
**Status**: In Progress

---

## Recent Progress (December 11, 2024)

### Session 3: Communities Page Redesign

**CommunitiesView Complete Redesign:**
1. ✅ Added page header with gradient background and breadcrumbs
2. ✅ Added floating stats bar with community metrics (animated)
3. ✅ Premium toolbar with search, type toggle, filters panel
4. ✅ Redesigned community cards with type-based colors and icons
5. ✅ Added grid/list view toggle
6. ✅ Staggered animations for cards and stats
7. ✅ Full RTL support and responsive design

**Design Features:**
- Type toggle: All Communities / My Communities
- Filter panel: Type (General, Project, Department, Working Group) + Visibility (Public, Private, Secret)
- Active filters tags with clear all
- Community cards with:
  - Color-coded headers by type
  - Member badge for joined communities
  - Visibility icons (public/private/secret)
  - Recent members avatars
  - Join/View buttons
  - Hover lift effect with icon scale

---

### Session 2: Task Management System

**Task Inbox & Dashboard Enhancement:**
1. ✅ Created comprehensive Task type system (`types/task.types.ts`)
2. ✅ Built TaskDetailPanel slide-over component with full task info display
3. ✅ Redesigned TaskInboxView with premium design system styling
4. ✅ Enhanced Dashboard task widget with smart sorting algorithm
5. ✅ Added Tasks link to sidebar navigation
6. ✅ Added routes for `/workflow/tasks` and `/workflow/services`
7. ✅ Added translations (English & Arabic) for navigation

**Task System Features:**
- Smart sorting: overdue → due today → priority → due date
- 5 task types: approval, review, assignment, mention, reminder
- 4 priority levels: urgent, high, medium, low
- 5 status states: pending, in_progress, completed, deferred, cancelled
- Quick action buttons for primary actions
- Slide-over detail panel (no page navigation needed)
- Progress tracking for assignment tasks
- Comments section with input
- Related item links to documents/communities/events
- Full RTL support and responsive design

---

### Session 1: Design System Unification

**Unified Design System Implementation:**
1. ✅ Analyzed ContentListView.vue as the reference implementation (gold standard)
2. ✅ Updated global SCSS mixins (`_mixins.scss`) with Content page standards
3. ✅ Applied unified styles to DocumentLibraryView.vue - now matches Content page exactly
4. ✅ Updated PAGE_LAYOUT_GUIDELINES.md with comprehensive documentation
5. ✅ Fixed container background issue in Documents page (removed `$gradient-background`)

**Key Changes Made:**
- Documents page now uses global mixins for toolbar, pagination, filters
- Page header uses layered background pattern (`header-background`, `bg-gradient`, `bg-pattern`)
- Stats bar simplified to 3 core stats with dividers
- Removed redundant background styling from page container
- All responsive breakpoints aligned with Content page

**Backup Created:** `backups/backup_20251211_145541`

---

## Overview

This document outlines the plan for redesigning all frontend views to match the premium design system documented in `frontend/docs/UI-UX-DESIGN-GUIDELINES.md`. The redesign focuses on creating a world-class, enterprise-grade user experience with full bilingual (English/Arabic RTL) support.

---

## Design System Reference

### Brand Colors
- **Primary**: Intalio Teal (`#00ae8d`)
- **Neutrals**: Slate Gray spectrum (`#f8fafc` to `#020617`)
- **Semantic**: Success (green), Warning (amber), Error (red), Info (blue)

### Key Design Patterns
1. **Page Header**: Gradient background with mesh overlay, breadcrumbs, title, description, action buttons
2. **Stats Bar**: White card with icon boxes and metrics
3. **Toolbar**: Search input, filters, view toggle (grid/list)
4. **Content Cards**: Hover lift effect, selection states, semantic colors
5. **Animations**: Staggered entrance, spring transitions, hover effects

### CSS Guidelines
- Use CSS logical properties for RTL support (`margin-inline-start` vs `margin-left`)
- 4px-based spacing grid
- `$radius-xl` (16px) for cards, `$radius-lg` (12px) for buttons/inputs
- Multi-layer shadows for depth

---

## Completion Status

### Phase 1: Core Pages (COMPLETED)

| Page | File | Status | Notes |
|------|------|--------|-------|
| Login | `views/auth/LoginView.vue` | ✅ Complete | Split layout, glass-morphism, bilingual |
| Main Layout | `views/layouts/MainLayout.vue` | ✅ Complete | Collapsible sidebar, premium nav |
| Dashboard | `views/dashboard/DashboardView.vue` | ✅ Complete | Stats widgets, quick actions, timeline, task widget |
| Content List | `views/content/ContentListView.vue` | ✅ Complete | Grid/list toggle, filters, pagination |
| Content Detail | `views/content/ContentDetailView.vue` | ✅ Complete | Hero image, author card, related articles |
| Document Library | `views/documents/DocumentLibraryView.vue` | ✅ Complete | Drag-drop upload, file icons, bulk actions |
| Task Inbox | `views/workflow/TaskInboxView.vue` | ✅ Complete | Smart sorting, quick actions, slide panel |
| Task Detail Panel | `components/tasks/TaskDetailPanel.vue` | ✅ Complete | Slide-over panel, actions, comments |
| Task Types | `types/task.types.ts` | ✅ Complete | Full type system, helper functions |

---

### Phase 2: Collaboration Module (PARTIAL)

| Page | File | Status | Priority | Complexity |
|------|------|--------|----------|------------|
| Communities List | `views/collaboration/CommunitiesView.vue` | ✅ Complete | High | Medium |
| Community Detail | `views/collaboration/CommunityDetailView.vue` | 🔲 Pending | High | High |
| Discussion Thread | `views/collaboration/DiscussionView.vue` | 🔲 Pending | High | High |
| Lessons Learned | `views/collaboration/LessonsLearnedView.vue` | 🔲 Pending | Medium | Medium |
| Lesson Detail | `views/collaboration/LessonDetailView.vue` | 🔲 Pending | Medium | Medium |

**Design Requirements:**
- Community cards with member avatars, discussion counts
- Discussion threads with voting, replies, @mentions
- Rich text content with bilingual support
- Member management and role badges

---

### Phase 3: Calendar Module (PENDING)

| Page | File | Status | Priority | Complexity |
|------|------|--------|----------|------------|
| Calendar View | `views/calendar/CalendarView.vue` | 🔲 Pending | High | High |

**Design Requirements:**
- Month/week/day views with event cards
- Mini calendar sidebar
- Event creation dialog
- Color-coded calendar categories
- RSVP status indicators
- Upcoming events panel

---

### Phase 4: Search Module (PENDING)

| Page | File | Status | Priority | Complexity |
|------|------|--------|----------|------------|
| Search Page | `views/search/SearchView.vue` | 🔲 Pending | High | Medium |
| Search Results | `views/search/SearchResultsView.vue` | 🔲 Pending | High | High |

**Design Requirements:**
- Command palette style search input
- Faceted filters (type, date, author, tags)
- Result cards with highlighting
- Saved searches panel
- Search analytics/trending

---

### Phase 5: Workflow Module (PARTIAL)

| Page | File | Status | Priority | Complexity |
|------|------|--------|----------|------------|
| Service Catalog | `views/workflow/ServiceCatalogView.vue` | 🔲 Pending | High | Medium |
| Task Inbox | `views/workflow/TaskInboxView.vue` | ✅ Complete | High | High |

**Completed Features (Task Inbox):**
- Smart sorting (overdue → due today → priority → date)
- Task type icons with color coding (approval, review, assignment, mention, reminder)
- Quick action buttons (approve/reject, complete, defer)
- Slide-over detail panel with full task info
- Comments section with input
- Related item links
- Progress tracking for assignments
- Filter by status, type, priority
- Search functionality
- List and grid view toggle
- Responsive design with RTL support

**Pending (Service Catalog):**
- Service cards with icons and categories
- Service request workflow

---

### Phase 6: AI Module (PENDING)

| Page | File | Status | Priority | Complexity |
|------|------|--------|----------|------------|
| AI Services | `views/ai/AIServicesView.vue` | 🔲 Pending | Medium | Medium |
| Semantic Search | `views/ai/SemanticSearchView.vue` | 🔲 Pending | Medium | Medium |

**Design Requirements:**
- AI service cards (transcription, summarization, Q&A)
- Chat-style interface for AI assistant
- Semantic search with relevance scores
- Processing status indicators
- Usage statistics

---

### Phase 7: Notifications Module (PENDING)

| Page | File | Status | Priority | Complexity |
|------|------|--------|----------|------------|
| Notifications List | `views/notifications/NotificationsView.vue` | 🔲 Pending | Medium | Medium |
| Notification Preferences | `views/notifications/NotificationPreferencesView.vue` | 🔲 Pending | Low | Low |

**Design Requirements:**
- Notification cards with icons and timestamps
- Read/unread states
- Group by date
- Mark all as read
- Channel toggles (email, push, in-app, SMS)
- Frequency settings (immediate, digest)

---

### Phase 8: Profile Module (PENDING)

| Page | File | Status | Priority | Complexity |
|------|------|--------|----------|------------|
| User Profile | `views/profile/ProfileView.vue` | 🔲 Pending | Medium | Medium |

**Design Requirements:**
- Profile header with avatar and cover image
- Contact information cards
- Activity timeline
- Skills/expertise tags
- Department and reporting structure
- Edit profile dialog

---

### Phase 9: Admin Module (PENDING)

| Page | File | Status | Priority | Complexity |
|------|------|--------|----------|------------|
| Users Management | `views/admin/UsersView.vue` | 🔲 Pending | Medium | High |
| Roles Management | `views/admin/RolesView.vue` | 🔲 Pending | Medium | Medium |
| Groups Management | `views/admin/GroupsView.vue` | 🔲 Pending | Medium | Medium |

**Design Requirements:**
- Data tables with sorting, filtering, pagination
- Bulk actions (activate, deactivate, delete)
- Role assignment dialogs
- Permission matrix view
- Import/export functionality
- Audit trail indicators

---

### Phase 10: Utility Pages (PENDING)

| Page | File | Status | Priority | Complexity |
|------|------|--------|----------|------------|
| 404 Not Found | `views/errors/NotFoundView.vue` | 🔲 Pending | Low | Low |
| Integration Dashboard | `views/integration/IntegrationDashboardView.vue` | 🔲 Pending | Low | Medium |

**Design Requirements:**
- 404: Friendly illustration, search suggestions, navigation links
- Integration: Connection status cards, sync indicators, logs

---

## Implementation Guidelines

### Reference Documents

Before implementing any page redesign, review these documents:

1. **`frontend/docs/UI-UX-DESIGN-GUIDELINES.md`** - Complete design system (colors, typography, spacing)
2. **`frontend/docs/PAGE_LAYOUT_GUIDELINES.md`** - Standard page layout patterns and specifications
3. **`frontend/src/assets/styles/_mixins.scss`** - Reusable SCSS mixins for page layouts

### Using SCSS Mixins

New pages should use the standardized mixins from `_mixins.scss`:

```scss
@use '@/assets/styles/_variables.scss' as *;
@use '@/assets/styles/_mixins.scss' as *;

.my-page-view {
  padding: $spacing-6;
}

.page-header {
  @include page-header;
}

.header-content {
  @include header-content;
}

.breadcrumb {
  @include breadcrumb;

  .breadcrumb-link { @include breadcrumb-link; }
  .breadcrumb-separator { @include breadcrumb-separator; }
  .breadcrumb-current { @include breadcrumb-current; }
}

h1 {
  @include page-title;
}

.page-description {
  @include page-description;
}

.header-actions {
  @include header-actions;

  .btn-primary { @include header-btn-primary; }
  .btn-secondary { @include header-btn-secondary; }
}

.stats-bar {
  @include stats-bar;

  .stat-item { @include stat-item; }
  .stat-icon { @include stat-icon; @include stat-icon-primary; }
  .stat-value { @include stat-value; }
  .stat-label { @include stat-label; }
  .stat-divider { @include stat-divider; }
}

.toolbar {
  @include toolbar;

  .toolbar-left { @include toolbar-left; }
  .toolbar-right { @include toolbar-right; }
}

.search-box {
  @include search-box;

  .search-input { @include search-input; }
}

.view-toggle {
  @include view-toggle;
}

.content-area {
  @include content-area;
}

.empty-state {
  @include empty-state;

  .empty-icon { @include empty-state-icon; }
  h3 { @include empty-state-title; }
  p { @include empty-state-text; }
}
```

### Standard Page Structure

Every redesigned page should follow this structure:

```vue
<template>
  <div class="page-name" :class="{ rtl: isRTL }">
    <!-- Page Header with Gradient -->
    <header class="page-header">
      <div class="header-content">
        <div class="header-left">
          <nav class="breadcrumbs">...</nav>
          <h1 class="page-title">{{ title }}</h1>
          <p class="page-description">{{ description }}</p>
        </div>
        <div class="header-actions">
          <Button ... />
        </div>
      </div>
    </header>

    <!-- Stats Bar -->
    <section class="stats-bar">
      <div class="stat-item" v-for="stat in stats">...</div>
    </section>

    <!-- Toolbar -->
    <div class="toolbar">
      <div class="toolbar-left"><!-- Search, Filters --></div>
      <div class="toolbar-right"><!-- View Toggle --></div>
    </div>

    <!-- Content Area -->
    <main class="content-area">
      <!-- Loading State -->
      <div v-if="loading" class="loading-grid">...</div>

      <!-- Empty State -->
      <div v-else-if="!items.length" class="empty-state">...</div>

      <!-- Data Display -->
      <div v-else class="data-grid">...</div>
    </main>

    <!-- Pagination -->
    <div class="pagination-wrapper">...</div>
  </div>
</template>
```

### Required SCSS Imports

```scss
<style lang="scss" scoped>
// All pages should define these CSS custom properties or use the design system variables

.page-header {
  margin: -1.5rem -1.5rem 1.5rem;
  padding: 2rem 1.5rem;
  background: linear-gradient(135deg, #009a7d 0%, #00ae8d 35%, #00c9a7 100%);
  position: relative;

  &::before {
    content: '';
    position: absolute;
    inset: 0;
    background: radial-gradient(at 40% 20%, rgba(#b3f0e6, 0.3) 0px, transparent 50%),
                radial-gradient(at 80% 0%, rgba(#e6faf6, 0.4) 0px, transparent 50%);
    opacity: 0.5;
  }
}

// ... more standard styles
</style>
```

### RTL Support Checklist

- [ ] Use `margin-inline-start/end` instead of `margin-left/right`
- [ ] Use `padding-inline-start/end` instead of `padding-left/right`
- [ ] Use `text-align: start/end` instead of `left/right`
- [ ] Rotate directional icons (chevrons, arrows) in RTL mode
- [ ] Test with Arabic content for text overflow
- [ ] Ensure form layouts work in both directions

### Accessibility Checklist

- [ ] All interactive elements have focus states
- [ ] Color contrast meets WCAG AA (4.5:1)
- [ ] Keyboard navigation works (Tab, Enter, Escape)
- [ ] ARIA labels for icon-only buttons
- [ ] Screen reader announcements for dynamic content

---

## Progress Tracking

### Summary

| Status | Count | Percentage |
|--------|-------|------------|
| ✅ Completed | 10 | 37% |
| 🔲 Pending | 17 | 63% |
| **Total** | **27** | **100%** |

### Files Modified This Session

| File | Changes |
|------|---------|
| `frontend/src/views/documents/DocumentLibraryView.vue` | Unified with Content page design, global mixins |
| `frontend/src/views/content/ContentListView.vue` | Minor fix for unused variables |
| `frontend/src/assets/styles/_mixins.scss` | Added toolbar, pagination, filter mixins |
| `frontend/docs/PAGE_LAYOUT_GUIDELINES.md` | Added toolbar documentation, updated status |
| `docs/PORTAL_REDESIGN_PLAN.md` | Added progress tracking |
| `frontend/src/types/task.types.ts` | **NEW** - Comprehensive task type system |
| `frontend/src/components/tasks/TaskDetailPanel.vue` | **NEW** - Slide-over task detail panel |
| `frontend/src/views/workflow/TaskInboxView.vue` | **REDESIGNED** - Full task management page |
| `frontend/src/views/dashboard/DashboardView.vue` | Enhanced task widget with smart sorting |
| `frontend/src/views/layouts/MainLayout.vue` | Added Tasks link to sidebar navigation |
| `frontend/src/router/index.ts` | Added workflow/tasks and workflow/services routes |
| `frontend/src/locales/en.json` | Added nav.tasks translation |
| `frontend/src/locales/ar.json` | Added nav.tasks Arabic translation |

### Execution Order (Recommended)

1. ~~**CommunitiesView**~~ - ✅ Completed
2. **CommunityDetailView** - Complex layout with tabs
3. **DiscussionView** - Thread-based UI patterns
4. **CalendarView** - Complex calendar component
5. **SearchResultsView** - Frequently accessed
6. **ServiceCatalogView** - Workflow entry point
7. ~~**TaskInboxView**~~ - ✅ Completed
8. **NotificationsView** - Common UI pattern
9. **ProfileView** - User-facing profile
10. **UsersView** - Admin data table
11. **RolesView** - Admin permissions
12. **GroupsView** - Admin groups
13. **LessonsLearnedView** - Knowledge base
14. **LessonDetailView** - Content detail
15. **AIServicesView** - AI features showcase
16. **SemanticSearchView** - AI search
17. **NotificationPreferencesView** - Settings form
18. **SearchView** - Search landing
19. **IntegrationDashboardView** - Integration status
20. **NotFoundView** - Error page

---

## Notes

- All pages must support both English (LTR) and Arabic (RTL) layouts
- Use PrimeVue components as base, customize with design system styles
- Maintain consistent loading states with skeleton loaders
- Include empty states with helpful messaging
- Test on viewport sizes: 640px, 768px, 1024px, 1280px, 1536px

---

## Backup History

| Date | Backup Name | Contents |
|------|-------------|----------|
| 2024-12-11 14:55 | `backups/backup_20251211_145541` | Full frontend after Documents page unification |

---

*Last Updated: December 11, 2024*
