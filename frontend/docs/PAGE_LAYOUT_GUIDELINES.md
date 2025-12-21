# Page Layout Guidelines
## AFC Asian Cup 2027 - Knowledge Management System

**Document Version**: 1.0
**Created**: December 11, 2024
**Status**: Active

---

## Overview

This document defines the standard page layout patterns to be applied consistently across all portal pages. These guidelines ensure visual consistency, proper spacing, and a premium user experience.

---

## Table of Contents

1. [Page Structure](#1-page-structure)
2. [Page Header Pattern](#2-page-header-pattern)
3. [Stats Bar Pattern](#3-stats-bar-pattern)
4. [Toolbar Pattern](#4-toolbar-pattern)
5. [Content Area Pattern](#5-content-area-pattern)
6. [Responsive Breakpoints](#6-responsive-breakpoints)
7. [SCSS Variables Reference](#7-scss-variables-reference)
8. [Implementation Examples](#8-implementation-examples)

---

## 1. Page Structure

### Standard Page Layout

Every list/dashboard page should follow this structure:

```
┌─────────────────────────────────────────────────────────────┐
│  PAGE HEADER (gradient background, rounded bottom corners)  │
│  ├─ Breadcrumb navigation                                   │
│  ├─ Page Title (h1)                                         │
│  ├─ Page Description                                        │
│  └─ Action Buttons (aligned to bottom-right)                │
├─────────────────────────────────────────────────────────────┤
│  STATS BAR (white card, overlapping header)                 │
│  └─ Stat Items with Icons (centered, horizontal)            │
├─────────────────────────────────────────────────────────────┤
│  TOOLBAR (white card)                                       │
│  ├─ Search Input (left)                                     │
│  ├─ Filters (left, after search)                            │
│  └─ View Toggle (right)                                     │
├─────────────────────────────────────────────────────────────┤
│  CONTENT AREA                                               │
│  ├─ Loading State (Skeletons)                               │
│  ├─ Empty State (when no data)                              │
│  └─ Data Display (Table/Grid)                               │
├─────────────────────────────────────────────────────────────┤
│  PAGINATION (if applicable)                                 │
└─────────────────────────────────────────────────────────────┘
```

### HTML Structure Template

```vue
<template>
  <div class="page-view" :class="{ rtl: isRTL }">
    <!-- Page Header -->
    <header class="page-header">
      <div class="header-content">
        <div>
          <nav class="breadcrumb">...</nav>
          <h1>Page Title</h1>
          <p class="page-description">Description text</p>
        </div>
        <div class="header-actions">
          <Button ... />
        </div>
      </div>
    </header>

    <!-- Stats Bar -->
    <section class="stats-bar">
      <div class="stat-item">...</div>
    </section>

    <!-- Toolbar -->
    <div class="toolbar">
      <div class="toolbar-left">...</div>
      <div class="toolbar-right">...</div>
    </div>

    <!-- Content Area -->
    <div class="content-area">...</div>
  </div>
</template>
```

---

## 2. Page Header Pattern

### Visual Specifications

| Property | Desktop | Mobile (< 768px) |
|----------|---------|------------------|
| Margin | `-$spacing-6 -$spacing-6 0` | `-$spacing-4 -$spacing-4 0` |
| Padding | `$spacing-8 $spacing-6 5rem` | `$spacing-6 $spacing-4 4rem` |
| Background | `$gradient-hero` | Same |
| Border Radius | `0 0 $radius-2xl $radius-2xl` | Same |
| Overlay | `$gradient-mesh` at 50% opacity | Same |

### Header Content Alignment

```scss
.header-content {
  position: relative;
  z-index: 1;
  display: flex;
  justify-content: space-between;
  align-items: flex-end;        // IMPORTANT: Aligns buttons to bottom
  gap: $spacing-4;
}
```

### Breadcrumb Specifications

```scss
.breadcrumb {
  display: flex;
  align-items: center;
  gap: $spacing-2;
  margin-bottom: $spacing-3;    // Space before title

  .breadcrumb-link {
    display: flex;
    align-items: center;
    gap: $spacing-2;
    padding: $spacing-1 $spacing-2;
    color: rgba(255, 255, 255, 0.8);
    background: none;
    border: none;
    font-size: $text-sm;
    border-radius: $radius-md;
    cursor: pointer;
    transition: all $transition-fast;

    &:hover {
      color: #fff;
      background: rgba(255, 255, 255, 0.1);
    }
  }

  .breadcrumb-separator {
    font-size: 0.625rem;        // 10px
    color: rgba(255, 255, 255, 0.5);
  }

  .breadcrumb-current {
    color: white;
    font-size: $text-sm;
    font-weight: 500;
  }
}
```

### Title Specifications

```scss
h1 {
  margin: 0 0 $spacing-2 0;
  font-size: $text-3xl;         // 30px
  font-weight: 700;
  color: #fff;
  letter-spacing: -0.02em;
}

.page-description {
  font-size: $text-base;        // 16px
  color: rgba(255, 255, 255, 0.8);
  margin: 0;
}
```

### Action Buttons

```scss
.header-actions {
  display: flex;
  gap: $spacing-3;
  flex-shrink: 0;               // Prevent buttons from shrinking
}

.action-btn {
  font-weight: $font-weight-semibold;
  padding: $spacing-3 $spacing-5;
  border-radius: $radius-lg;

  &.secondary {
    background: rgba(255, 255, 255, 0.15);
    border-color: rgba(255, 255, 255, 0.3);
    color: #fff;

    &:hover {
      background: rgba(255, 255, 255, 0.25);
    }
  }

  &.primary {
    background: #fff;
    border-color: #fff;
    color: $intalio-teal-600;

    &:hover {
      background: rgba(255, 255, 255, 0.9);
    }
  }
}
```

---

## 3. Stats Bar Pattern

### Visual Specifications

The stats bar creates a "floating card" effect by overlapping the page header.

| Property | Desktop | Mobile (< 768px) |
|----------|---------|------------------|
| Margin | `-2.5rem $spacing-6 $spacing-6` | `-2rem $spacing-4 $spacing-4` |
| Padding | `$spacing-5 $spacing-6` | `$spacing-4` |
| Background | `#fff` | Same |
| Border Radius | `$radius-xl` (16px) | Same |
| Shadow | `$shadow-elevated-md` | Same |
| Z-Index | `5` | Same |

### SCSS Implementation

```scss
.stats-bar {
  position: relative;
  z-index: 5;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: $spacing-8;
  margin: -2.5rem $spacing-6 $spacing-6;
  padding: $spacing-5 $spacing-6;
  background: #fff;
  border-radius: $radius-xl;
  box-shadow: $shadow-elevated-md;
}

.stat-item {
  display: flex;
  align-items: center;
  gap: $spacing-3;

  .stat-icon {
    width: 44px;
    height: 44px;
    border-radius: $radius-lg;
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: 1.25rem;

    // Semantic color variants
    &.primary   { background: rgba($intalio-teal-500, 0.1); color: $intalio-teal-500; }
    &.info      { background: rgba($info-500, 0.1); color: $info-500; }
    &.success   { background: rgba($success-500, 0.1); color: $success-500; }
    &.warning   { background: rgba($warning-500, 0.1); color: $warning-500; }
    &.error     { background: rgba($error-500, 0.1); color: $error-500; }
  }

  .stat-value {
    font-size: $text-xl;
    font-weight: $font-weight-bold;
    color: $slate-900;
    line-height: 1.2;
  }

  .stat-label {
    font-size: $text-xs;
    color: $slate-500;
    text-transform: uppercase;
    letter-spacing: 0.5px;
  }
}

.stat-divider {
  width: 1px;
  height: 40px;
  background: $slate-200;
}
```

---

## 4. Toolbar Pattern

The toolbar is a comprehensive control bar that provides search, filtering, pagination controls, and view toggles. It supports an expandable filters panel and active filter tags.

### Structure Overview

```
┌─────────────────────────────────────────────────────────────────┐
│  TOOLBAR CONTAINER                                               │
├─────────────────────────────────────────────────────────────────┤
│  TOOLBAR ROW                                                     │
│  ┌─────────────────────────────────┬───────────────────────────┐ │
│  │  TOOLBAR LEFT                   │  TOOLBAR RIGHT            │ │
│  │  ├─ Search Box                  │  ├─ Per Page Control      │ │
│  │  ├─ Type Toggle (optional)      │  └─ View Toggle           │ │
│  │  ├─ Filter Button               │                           │ │
│  │  └─ Bulk Actions (if selected)  │                           │ │
│  └─────────────────────────────────┴───────────────────────────┘ │
├─────────────────────────────────────────────────────────────────┤
│  FILTERS PANEL (expandable, shown when Filter button clicked)    │
│  ├─ Filter Group 1 (chips)                                       │
│  ├─ Filter Group 2 (chips)                                       │
│  └─ Clear Filters Button                                         │
├─────────────────────────────────────────────────────────────────┤
│  ACTIVE FILTERS ROW (shown when filters active & panel closed)   │
│  ├─ Active filter tags (removable)                               │
│  └─ Clear All link                                               │
└─────────────────────────────────────────────────────────────────┘
```

### Visual Specifications

| Property | Value |
|----------|-------|
| Display | `flex` / `flex-direction: column` |
| Gap | `$spacing-4` |
| Margin | `0 0 $spacing-6 0` |
| Padding | `$spacing-4` |
| Background | `#fff` |
| Border Radius | `$radius-xl` (16px) |
| Shadow | `$shadow-card` |

### HTML Template

```vue
<template>
  <div class="toolbar">
    <div class="toolbar-row">
      <div class="toolbar-left">
        <!-- Search -->
        <div class="search-box">
          <i class="pi pi-search"></i>
          <InputText v-model="search" placeholder="Search..." class="search-input" />
        </div>

        <!-- Type Toggle (optional) -->
        <div class="type-toggle">
          <button v-for="opt in typeOptions" :class="{ active: type === opt.value }">
            <i :class="opt.icon"></i>
            <span>{{ opt.label }}</span>
          </button>
        </div>

        <!-- Filter Button -->
        <button class="filter-btn" :class="{ active: showFilters, 'has-filters': activeFiltersCount > 0 }">
          <i class="pi pi-filter"></i>
          <span>Filters</span>
          <span v-if="activeFiltersCount > 0" class="filter-badge">{{ activeFiltersCount }}</span>
        </button>
      </div>

      <div class="toolbar-right">
        <!-- Per Page Control -->
        <div class="per-page-control">
          <span class="per-page-label">Show</span>
          <select v-model="itemsPerPage" class="per-page-select">
            <option value="10">10</option>
            <option value="20">20</option>
          </select>
          <span class="per-page-label">items</span>
        </div>

        <!-- View Toggle -->
        <div class="view-toggle">
          <button :class="{ active: view === 'grid' }"><i class="pi pi-th-large"></i></button>
          <button :class="{ active: view === 'list' }"><i class="pi pi-list"></i></button>
        </div>
      </div>
    </div>

    <!-- Filters Panel -->
    <div v-if="showFilters" class="filters-panel">
      <div class="filter-group">
        <label class="filter-label">Category</label>
        <div class="filter-options">
          <button v-for="opt in categoryOptions" class="filter-chip" :class="{ active: ... }">
            {{ opt.label }}
          </button>
        </div>
      </div>
    </div>

    <!-- Active Filters -->
    <div v-if="activeFiltersCount > 0 && !showFilters" class="active-filters">
      <span class="active-filters-label">Active filters:</span>
      <span class="filter-tag">
        Category Name
        <button><i class="pi pi-times"></i></button>
      </span>
      <button class="clear-all-link">Clear all</button>
    </div>
  </div>
</template>
```

### SCSS Implementation Using Mixins

```scss
@use '@/assets/styles/_variables.scss' as *;
@use '@/assets/styles/_mixins.scss' as *;

.toolbar {
  @include toolbar-container;
}

.toolbar-row {
  @include toolbar-row;
}

.toolbar-left {
  @include toolbar-section-left;
}

.toolbar-right {
  @include toolbar-section-right;
}

.search-box {
  @include toolbar-search(280px);
}

.search-input {
  @include toolbar-search-input;
}

.type-toggle {
  @include type-toggle;
}

.filter-btn {
  @include filter-btn;

  .filter-badge {
    @include filter-badge;
  }
}

.per-page-control {
  @include per-page-control;
}

.per-page-label {
  @include per-page-label;
}

.per-page-select {
  @include per-page-select;
}

.view-toggle {
  @include premium-view-toggle;
}

// Filters Panel
.filters-panel {
  @include filters-panel;
}

.filter-group {
  @include filters-panel-group;
}

.filter-label {
  @include filters-panel-label;
}

.filter-options {
  @include filters-panel-options;
}

.filter-chip {
  @include filter-chip;
}

.clear-filters-btn {
  @include clear-filters-btn;
}

// Active Filters
.active-filters {
  @include active-filters-row;
}

.active-filters-label {
  @include active-filters-label;
}

.filter-tag {
  @include filter-tag;
}

.clear-all-link {
  @include clear-all-link;
}
```

### Available Mixins Reference

| Mixin | Description |
|-------|-------------|
| `toolbar-container` | Main toolbar wrapper with column layout |
| `toolbar-row` | Horizontal row for main controls |
| `toolbar-section-left` | Left section container |
| `toolbar-section-right` | Right section container |
| `toolbar-search($width)` | Search box wrapper |
| `toolbar-search-input` | Search input styling |
| `type-toggle` | Segmented control for type selection |
| `filter-btn` | Filter button with badge support |
| `filter-badge` | Count badge inside filter button |
| `per-page-control` | Per page selector wrapper |
| `per-page-label` | Label text styling |
| `per-page-select` | Select dropdown styling |
| `premium-view-toggle` | Grid/List view toggle |
| `filters-panel` | Expandable filters container |
| `filters-panel-group` | Filter group wrapper |
| `filters-panel-label` | Filter group label |
| `filters-panel-options` | Filter options container |
| `filter-chip` | Filter option pill button |
| `clear-filters-btn` | Clear filters button |
| `active-filters-row` | Active filters tags container |
| `active-filters-label` | "Active filters:" label |
| `filter-tag` | Removable filter tag |
| `clear-all-link` | Clear all link |
| `bulk-actions` | Bulk actions container |
| `selection-count` | Selection count text |

---

## 5. Content Area Pattern

### Card-Based Content

```scss
.content-area {
  background: #fff;
  border-radius: $radius-xl;
  box-shadow: $shadow-card;
  overflow: hidden;
}
```

### Grid Layout

```scss
.content-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(240px, 1fr));
  gap: $spacing-4;
  padding: $spacing-6;
}
```

### Empty State

```scss
.empty-state {
  text-align: center;
  padding: $spacing-12 $spacing-6;

  .empty-icon {
    width: 80px;
    height: 80px;
    margin: 0 auto $spacing-4;
    background: $slate-100;
    border-radius: $radius-xl;
    display: flex;
    align-items: center;
    justify-content: center;

    i {
      font-size: 2.5rem;
      color: $slate-400;
    }
  }

  h3 {
    font-size: $text-xl;
    font-weight: $font-weight-semibold;
    color: $slate-800;
    margin: 0 0 $spacing-2;
  }

  p {
    font-size: $text-base;
    color: $slate-500;
    margin: 0 0 $spacing-6;
  }
}
```

---

## 6. Responsive Breakpoints

### Mobile Adjustments (max-width: 768px)

```scss
@media (max-width: $breakpoint-md) {
  .page-view {
    padding: $spacing-4;
  }

  .page-header {
    margin: (-$spacing-4) (-$spacing-4) 0;
    padding: $spacing-6 $spacing-4 4rem;

    .header-content {
      flex-direction: column;
      align-items: flex-start;
    }

    h1 {
      font-size: $text-2xl;
    }
  }

  .stats-bar {
    margin: -2rem $spacing-4 $spacing-4;
    padding: $spacing-4;
    flex-wrap: wrap;
    gap: $spacing-4;

    .stat-divider {
      display: none;
    }
  }

  .toolbar {
    flex-direction: column;
    gap: $spacing-3;
  }

  .toolbar-left {
    width: 100%;
  }

  .search-box {
    width: 100%;
  }

  .header-actions {
    width: 100%;

    .action-btn {
      flex: 1;
      justify-content: center;
    }
  }
}
```

### Tablet Adjustments (max-width: 1024px)

```scss
@media (max-width: $breakpoint-lg) {
  .stats-bar {
    flex-wrap: wrap;

    .stat-item {
      flex: 0 0 calc(33.333% - $spacing-3);
    }
  }
}
```

---

## 7. SCSS Variables Reference

### Spacing

```scss
$spacing-1:  0.25rem;   // 4px
$spacing-2:  0.5rem;    // 8px
$spacing-3:  0.75rem;   // 12px
$spacing-4:  1rem;      // 16px
$spacing-5:  1.25rem;   // 20px
$spacing-6:  1.5rem;    // 24px
$spacing-8:  2rem;      // 32px
$spacing-10: 2.5rem;    // 40px
$spacing-12: 3rem;      // 48px
```

### Border Radius

```scss
$radius-sm:   0.25rem;  // 4px
$radius-md:   0.5rem;   // 8px
$radius-lg:   0.75rem;  // 12px
$radius-xl:   1rem;     // 16px
$radius-2xl:  1.5rem;   // 24px
$radius-full: 9999px;
```

### Shadows

```scss
$shadow-card:         0 1px 3px rgba(0, 0, 0, 0.04), 0 4px 12px rgba(0, 0, 0, 0.04);
$shadow-card-hover:   0 4px 12px rgba(0, 0, 0, 0.06), 0 12px 24px rgba(0, 0, 0, 0.08);
$shadow-elevated-md:  0 4px 6px -1px rgba(0, 0, 0, 0.05), 0 2px 4px -2px rgba(0, 0, 0, 0.05);
$shadow-focus-ring:   0 0 0 3px rgba($intalio-teal-500, 0.15);
```

### Gradients

```scss
$gradient-hero: linear-gradient(135deg, #009a7d 0%, #00ae8d 35%, #00c9a7 100%);
$gradient-mesh: radial-gradient(at 40% 20%, rgba(#b3f0e6, 0.3) 0px, transparent 50%),
                radial-gradient(at 80% 0%, rgba(#e6faf6, 0.4) 0px, transparent 50%);
```

---

## 8. Implementation Examples

### Complete Page SCSS

```scss
@use '@/assets/styles/_variables.scss' as *;

.my-page-view {
  min-height: 100vh;
  padding: $spacing-6;

  &.rtl {
    direction: rtl;

    .breadcrumb-separator {
      transform: rotate(180deg);
    }
  }
}

// Page Header
.page-header {
  margin: (-$spacing-6) (-$spacing-6) 0;
  padding: $spacing-8 $spacing-6 5rem;
  background: $gradient-hero;
  position: relative;
  overflow: hidden;
  border-radius: 0 0 $radius-2xl $radius-2xl;

  &::before {
    content: '';
    position: absolute;
    inset: 0;
    background: $gradient-mesh;
    opacity: 0.5;
  }
}

.header-content {
  position: relative;
  z-index: 1;
  display: flex;
  justify-content: space-between;
  align-items: flex-end;
  gap: $spacing-4;
}

// ... (rest of styles as documented above)
```

---

## Pages Using This Pattern

The following pages should implement these guidelines:

| Page | Status | Notes |
|------|--------|-------|
| ContentListView | ✅ Reference Implementation | Gold standard for all list pages |
| DocumentLibraryView | ✅ Complete | Uses global mixins, matches Content page |
| CommunitiesView | 🔲 Pending | High priority - collaboration module |
| CommunityDetailView | 🔲 Pending | High priority - collaboration module |
| DiscussionView | 🔲 Pending | Thread-based UI patterns |
| LessonsLearnedView | 🔲 Pending | Knowledge base list |
| CalendarView | 🔲 Pending | Complex calendar component |
| SearchResultsView | 🔲 Pending | Frequently accessed |
| ServiceCatalogView | 🔲 Pending | Workflow entry point |
| TaskInboxView | 🔲 Pending | Daily user interaction |
| NotificationsView | 🔲 Pending | Common UI pattern |
| ProfileView | 🔲 Pending | User-facing profile |
| UsersView (Admin) | 🔲 Pending | Admin data table |
| RolesView (Admin) | 🔲 Pending | Admin permissions |
| GroupsView (Admin) | 🔲 Pending | Admin groups |
| AIServicesView | 🔲 Pending | AI features showcase |
| SemanticSearchView | 🔲 Pending | AI search |
| NotificationPreferencesView | 🔲 Pending | Settings form |
| IntegrationDashboardView | 🔲 Pending | Integration status |
| NotFoundView | 🔲 Pending | Error page |

---

## Checklist for New Pages

When implementing a new page, verify:

- [ ] Page header uses correct margin: `-$spacing-6 -$spacing-6 0`
- [ ] Page header padding includes 5rem bottom for overlap
- [ ] Page header has rounded bottom corners
- [ ] Header content uses `align-items: flex-end`
- [ ] Breadcrumb uses flat structure (no `<ol><li>`)
- [ ] Breadcrumb margin-bottom is `$spacing-3`
- [ ] Title uses `letter-spacing: -0.02em`
- [ ] Stats bar uses negative top margin `-2.5rem`
- [ ] Stats bar has `z-index: 5`
- [ ] All spacing uses CSS logical properties for RTL
- [ ] Mobile breakpoint adjusts header to column layout
- [ ] Mobile breadcrumb separator rotates 180deg in RTL

---

## Appendix: Complete Mixins Reference

### Content Cards
| Mixin | Description |
|-------|-------------|
| `@include content-card` | Base interactive card with hover effects |
| `@include content-card-featured` | Featured card variant with accent border |
| `@include card-media($height)` | Card image section with zoom effect |
| `@include card-placeholder` | Placeholder for missing images |
| `@include card-badges` | Container for badges (top-left) |
| `@include featured-badge` | Gold featured star badge |
| `@include card-overlay` | Hover overlay with gradient |
| `@include card-body` | Card content area |
| `@include card-title` | Card title with line-clamp |
| `@include card-excerpt` | Card description text |
| `@include card-meta` | Category/date row |
| `@include card-footer` | Bottom section with border-top |
| `@include card-tags` | Tags container |
| `@include tag-chip` | Individual tag with dynamic color |

### Author Components
| Mixin | Description |
|-------|-------------|
| `@include author-card` | Horizontal author info card |
| `@include author-avatar($size)` | Circular avatar with gradient |
| `@include author-info` | Name and role text |
| `@include follow-btn` | Teal follow/subscribe button |

### Sidebar Components
| Mixin | Description |
|-------|-------------|
| `@include sticky-sidebar` | Sticky sidebar with max-height |
| `@include sidebar-section` | Sidebar card container |
| `@include section-title` | Section heading with icon |
| `@include related-card` | Related item with thumbnail |
| `@include quick-action-btn` | Full-width action button |

### Hero Section (Detail Pages)
| Mixin | Description |
|-------|-------------|
| `@include detail-hero($height)` | Full-width hero with image |
| `@include hero-background` | Absolute positioned image |
| `@include hero-overlay` | Dark gradient overlay |
| `@include hero-content` | Content container over hero |
| `@include hero-action-btn` | Icon button on dark bg |
| `@include hero-back-btn` | Back navigation button |
| `@include hero-stats` | Stats row (views, date, etc.) |

### Rich Content
| Mixin | Description |
|-------|-------------|
| `@include article-body` | Rich text article container with styled h2, h3, p, lists, blockquotes |

### Share Components
| Mixin | Description |
|-------|-------------|
| `@include share-section` | Share bar container |
| `@include share-btn` | Social media share button |

### Pagination
| Mixin | Description |
|-------|-------------|
| `@include pagination-wrapper` | Full pagination bar container |
| `@include pagination-info` | "Showing X to Y of Z items" text |
| `@include pagination-controls` | Page buttons container |
| `@include pagination-btn` | Page number button with active state |
| `@include pagination-ellipsis` | Ellipsis "..." separator |
| `@include pagination` | Simple pagination (deprecated) |
| `@include page-btn` | Page number button (deprecated) |
| `@include page-btn-nav` | Prev/Next button variant (deprecated) |

### Filter Components
| Mixin | Description |
|-------|-------------|
| `@include filter-group` | Filter with label container |
| `@include filter-label` | Uppercase filter label |
| `@include active-filters` | Active filters bar |
| `@include filter-pill` | Removable filter chip |

### Skeleton Loading
| Mixin | Description |
|-------|-------------|
| `@include skeleton-grid` | Grid container for skeletons |
| `@include skeleton-card` | Pulsing skeleton card |

### Layout
| Mixin | Description |
|-------|-------------|
| `@include two-column-layout($sidebar)` | Main + sidebar grid layout |

### Animations
| Mixin | Description |
|-------|-------------|
| `@include fade-in-up` | Fade in with upward motion |
| `@include staggered-children($delay, $count)` | Stagger animation for list items |

---

*Last Updated: December 11, 2024*
