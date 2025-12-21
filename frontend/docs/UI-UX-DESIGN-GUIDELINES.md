# Intalio KMS - UI/UX Design Guidelines
## World-Class Design System Documentation

---

## Table of Contents

1. [Brand Identity](#1-brand-identity)
2. [Color System](#2-color-system)
3. [Typography](#3-typography)
4. [Spacing System](#4-spacing-system)
5. [Border Radius](#5-border-radius)
6. [Shadows & Elevation](#6-shadows--elevation)
7. [Gradients](#7-gradients)
8. [Animations & Transitions](#8-animations--transitions)
9. [Page Layout Patterns](#9-page-layout-patterns)
10. [Component Patterns](#10-component-patterns)
11. [Responsive Design](#11-responsive-design)
12. [RTL Support](#12-rtl-support)
13. [Accessibility](#13-accessibility)
14. [Completed Pages](#14-completed-pages)
15. [Remaining Pages](#15-remaining-pages)

---

## 1. Brand Identity

### Logo Assets
| Asset | File | Usage |
|-------|------|-------|
| Primary Logo (Black) | `Intalio.png` | Light backgrounds, main navigation |
| Primary Logo (White) | `Intalio_White.png` | Dark backgrounds, hero sections, login page |

### Brand Values
- **Professional**: Enterprise-grade, trustworthy
- **Modern**: Clean lines, contemporary aesthetics
- **Efficient**: Focus on usability and productivity
- **Bilingual**: Full Arabic (RTL) and English (LTR) support

---

## 2. Color System

### Primary Brand Colors - Intalio Teal

```scss
// Full Teal Spectrum
$intalio-teal-50:  #e6faf6;   // Hover backgrounds
$intalio-teal-100: #b3f0e6;   // Light accents
$intalio-teal-200: #80e6d5;   // Disabled states
$intalio-teal-300: #4dcbb4;   // Light variant
$intalio-teal-400: #26d4b8;   // Interactive elements
$intalio-teal-500: #00ae8d;   // PRIMARY BRAND COLOR
$intalio-teal-600: #009a7d;   // Hover state
$intalio-teal-700: #008670;   // Active/pressed state
$intalio-teal-800: #006b59;   // Dark accents
$intalio-teal-900: #004d40;   // Deep teal
```

### Neutral Colors - Slate Gray

```scss
$slate-50:  #f8fafc;   // Page backgrounds
$slate-100: #f1f5f9;   // Card backgrounds, alternating rows
$slate-200: #e2e8f0;   // Borders, dividers
$slate-300: #cbd5e1;   // Disabled text
$slate-400: #94a3b8;   // Placeholder text
$slate-500: #64748b;   // Secondary text
$slate-600: #475569;   // Body text
$slate-700: #334155;   // Headings
$slate-800: #1e293b;   // Primary text
$slate-900: #0f172a;   // Deep dark
$slate-950: #020617;   // Near black
```

### Semantic Colors

```scss
// Success
$success-50:  #ecfdf5;
$success-500: #10b981;
$success-600: #059669;

// Warning
$warning-50:  #fffbeb;
$warning-500: #f59e0b;
$warning-600: #d97706;

// Error/Danger
$error-50:  #fef2f2;
$error-500: #ef4444;
$error-600: #dc2626;

// Info
$info-50:  #eff6ff;
$info-500: #3b82f6;
$info-600: #2563eb;
```

### File Type Colors

```scss
$file-colors: (
  folder: #f59e0b,    // Amber
  pdf: #ef4444,       // Red
  docx: #3b82f6,      // Blue
  xlsx: #22c55e,      // Green
  pptx: #f97316,      // Orange
  image: #8b5cf6,     // Purple
  video: #ec4899,     // Pink
  archive: #6b7280    // Gray
);
```

---

## 3. Typography

### Font Family

```scss
$font-family-primary: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
$font-family-arabic: 'Noto Sans Arabic', 'Inter', sans-serif;
```

### Font Sizes (rem-based)

```scss
$text-xs:   0.75rem;    // 12px - Captions, labels
$text-sm:   0.875rem;   // 14px - Secondary text
$text-base: 1rem;       // 16px - Body text
$text-lg:   1.125rem;   // 18px - Emphasized text
$text-xl:   1.25rem;    // 20px - Small headings
$text-2xl:  1.5rem;     // 24px - Section headings
$text-3xl:  1.875rem;   // 30px - Page titles
$text-4xl:  2.25rem;    // 36px - Hero titles
```

### Font Weights

```scss
$font-weight-light:    300;
$font-weight-normal:   400;
$font-weight-medium:   500;
$font-weight-semibold: 600;
$font-weight-bold:     700;
```

### Typography Usage

| Element | Size | Weight | Color |
|---------|------|--------|-------|
| Hero Title | `$text-4xl` | Bold | White / $slate-900 |
| Page Title | `$text-3xl` | Bold | White (header) / $slate-900 |
| Section Heading | `$text-2xl` | Semibold | $slate-800 |
| Card Title | `$text-xl` | Semibold | $slate-800 |
| Body Text | `$text-base` | Normal | $slate-600 |
| Secondary Text | `$text-sm` | Normal | $slate-500 |
| Caption/Label | `$text-xs` | Medium | $slate-500 |

---

## 4. Spacing System

Based on 4px grid:

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

### Usage Guidelines

| Context | Spacing |
|---------|---------|
| Component internal padding | `$spacing-4` - `$spacing-5` |
| Card padding | `$spacing-5` - `$spacing-6` |
| Page padding | `$spacing-6` |
| Section margin | `$spacing-6` - `$spacing-8` |
| Gap between cards | `$spacing-4` |
| Gap between elements | `$spacing-2` - `$spacing-3` |

---

## 5. Border Radius

```scss
$radius-sm:   0.25rem;  // 4px - Buttons, inputs
$radius-md:   0.5rem;   // 8px - Small cards, chips
$radius-lg:   0.75rem;  // 12px - Cards
$radius-xl:   1rem;     // 16px - Large cards, modals
$radius-2xl:  1.5rem;   // 24px - Hero sections
$radius-full: 9999px;   // Circles, pills
```

### Usage Guidelines

| Component | Radius |
|-----------|--------|
| Buttons | `$radius-lg` |
| Input fields | `$radius-lg` |
| Cards | `$radius-xl` |
| Modals | `$radius-xl` - `$radius-2xl` |
| Tags/Chips | `$radius-md` |
| Avatars | `$radius-full` |
| Icon containers | `$radius-lg` - `$radius-xl` |

---

## 6. Shadows & Elevation

### Standard Shadows

```scss
$shadow-xs:  0 1px 2px 0 rgba(0, 0, 0, 0.03);
$shadow-sm:  0 1px 3px 0 rgba(0, 0, 0, 0.04), 0 1px 2px -1px rgba(0, 0, 0, 0.04);
$shadow-md:  0 4px 6px -1px rgba(0, 0, 0, 0.05), 0 2px 4px -2px rgba(0, 0, 0, 0.05);
$shadow-lg:  0 10px 15px -3px rgba(0, 0, 0, 0.06), 0 4px 6px -4px rgba(0, 0, 0, 0.06);
$shadow-xl:  0 20px 25px -5px rgba(0, 0, 0, 0.08), 0 8px 10px -6px rgba(0, 0, 0, 0.06);
$shadow-2xl: 0 25px 50px -12px rgba(0, 0, 0, 0.15);
```

### Card Shadows (Multi-layer)

```scss
$shadow-card:        0 1px 3px rgba(0, 0, 0, 0.04), 0 4px 12px rgba(0, 0, 0, 0.04);
$shadow-card-hover:  0 4px 12px rgba(0, 0, 0, 0.06), 0 12px 24px rgba(0, 0, 0, 0.08);
$shadow-card-active: 0 2px 8px rgba(0, 0, 0, 0.08), 0 8px 16px rgba(0, 0, 0, 0.1);
```

### Colored Shadows (Brand accent)

```scss
$shadow-teal-sm:   0 4px 14px -3px rgba($intalio-teal-500, 0.25);
$shadow-teal-md:   0 8px 20px -4px rgba($intalio-teal-500, 0.3);
$shadow-teal-glow: 0 0 40px -10px rgba($intalio-teal-400, 0.4);
```

### Focus Ring

```scss
$shadow-focus-ring: 0 0 0 3px rgba($intalio-teal-500, 0.15);
```

---

## 7. Gradients

### Primary Gradients

```scss
$gradient-primary: linear-gradient(135deg, #00ae8d 0%, #26d4b8 50%, #00c9a7 100%);
$gradient-primary-hover: linear-gradient(135deg, #009a7d 0%, #00ae8d 50%, #26d4b8 100%);
$gradient-primary-subtle: linear-gradient(135deg, #e6faf6 0%, rgba(#b3f0e6, 0.5) 100%);
```

### Hero/Header Gradients

```scss
$gradient-hero: linear-gradient(135deg, #009a7d 0%, #00ae8d 35%, #00c9a7 100%);
$gradient-hero-dark: linear-gradient(135deg, #004d40 0%, #006b59 50%, #008670 100%);
```

### Background Gradients

```scss
$gradient-background: linear-gradient(180deg, #f8fafc 0%, #ffffff 100%);
$gradient-card: linear-gradient(180deg, #ffffff 0%, #f8fafc 100%);
```

### Glass Effect

```scss
$gradient-glass: linear-gradient(135deg, rgba(255,255,255,0.9) 0%, rgba(255,255,255,0.7) 100%);
```

### Mesh Gradient (Background decoration)

```scss
$gradient-mesh: radial-gradient(at 40% 20%, rgba($intalio-teal-100, 0.3) 0px, transparent 50%),
                radial-gradient(at 80% 0%, rgba($intalio-teal-50, 0.4) 0px, transparent 50%),
                radial-gradient(at 0% 50%, rgba($intalio-teal-100, 0.2) 0px, transparent 50%);
```

---

## 8. Animations & Transitions

### Timing Functions

```scss
$ease-out-expo:     cubic-bezier(0.16, 1, 0.3, 1);      // Primary ease
$ease-out-quart:    cubic-bezier(0.25, 1, 0.5, 1);      // Smooth deceleration
$ease-in-out-quart: cubic-bezier(0.76, 0, 0.24, 1);     // Symmetrical
$ease-spring:       cubic-bezier(0.34, 1.56, 0.64, 1);  // Bouncy
$ease-bounce:       cubic-bezier(0.68, -0.6, 0.32, 1.6); // Playful bounce
```

### Duration Scale

```scss
$duration-instant:  75ms;
$duration-fast:     150ms;
$duration-base:     200ms;
$duration-moderate: 300ms;
$duration-slow:     400ms;
$duration-slower:   500ms;
```

### Composed Transitions

```scss
$transition-fast:   150ms $ease-out-quart;
$transition-base:   200ms $ease-out-quart;
$transition-slow:   300ms $ease-out-expo;
$transition-spring: 300ms $ease-spring;
```

### Standard Animations

```scss
// Fade In with Slide
@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

// Staggered entrance (apply to cards/items)
@for $i from 1 through 20 {
  .item:nth-child(#{$i}) {
    animation-delay: #{$i * 0.05}s;
  }
}
```

### Hover Effects

```scss
// Card hover lift
.card:hover {
  transform: translateY(-2px);
  box-shadow: $shadow-card-hover;
}

// Button scale
.button:hover {
  transform: scale(1.02);
}

// Icon rotation (refresh/loading)
.icon:hover {
  transform: rotate(15deg);
}
```

---

## 9. Page Layout Patterns

### Standard Page Structure

```
┌────────────────────────────────────────────────────┐
│  PAGE HEADER (gradient background)                 │
│  ├─ Breadcrumbs                                    │
│  ├─ Page Title                                     │
│  ├─ Page Description                               │
│  └─ Action Buttons                                 │
├────────────────────────────────────────────────────┤
│  STATS BAR (white card)                            │
│  └─ Stat Items with Icons                          │
├────────────────────────────────────────────────────┤
│  TOOLBAR (white card)                              │
│  ├─ Search Input                                   │
│  ├─ Filters (Dropdowns)                            │
│  └─ View Toggle (List/Grid)                        │
├────────────────────────────────────────────────────┤
│  CONTENT AREA                                      │
│  ├─ Loading State (Skeletons)                      │
│  ├─ Data View (Table/Grid)                         │
│  └─ Empty State                                    │
├────────────────────────────────────────────────────┤
│  PAGINATION                                        │
└────────────────────────────────────────────────────┘
```

### Page Header Pattern

```vue
<header class="page-header">
  <div class="header-content">
    <div class="header-left">
      <nav class="breadcrumbs">...</nav>
      <h1 class="page-title">...</h1>
      <p class="page-description">...</p>
    </div>
    <div class="header-actions">
      <Button class="action-btn secondary" outlined />
      <Button class="action-btn primary" />
    </div>
  </div>
</header>
```

```scss
.page-header {
  margin: -$spacing-6 -$spacing-6 $spacing-6;
  padding: $spacing-8 $spacing-6;
  background: $gradient-hero;
  position: relative;

  &::before {
    content: '';
    position: absolute;
    inset: 0;
    background: $gradient-mesh;
    opacity: 0.5;
  }
}
```

### Stats Bar Pattern

```vue
<section class="stats-bar">
  <div class="stat-item" v-for="stat in stats">
    <div class="stat-icon" :class="stat.type">
      <i :class="stat.icon"></i>
    </div>
    <div class="stat-content">
      <span class="stat-value">{{ stat.value }}</span>
      <span class="stat-label">{{ stat.label }}</span>
    </div>
  </div>
</section>
```

### Toolbar Pattern

```vue
<div class="toolbar">
  <div class="toolbar-left">
    <div class="search-box">
      <i class="pi pi-search"></i>
      <InputText v-model="search" class="search-input" />
    </div>
    <Dropdown v-model="filter" class="filter-dropdown" />
  </div>
  <div class="toolbar-right">
    <div class="view-toggle">
      <button :class="{ active: view === 'list' }">
        <i class="pi pi-list"></i>
      </button>
      <button :class="{ active: view === 'grid' }">
        <i class="pi pi-th-large"></i>
      </button>
    </div>
  </div>
</div>
```

---

## 10. Component Patterns

### Card Component

```scss
.card {
  background: #fff;
  border-radius: $radius-xl;
  padding: $spacing-5;
  box-shadow: $shadow-card;
  border: 2px solid transparent;
  transition: all $transition-base;
  cursor: pointer;

  &:hover {
    box-shadow: $shadow-card-hover;
    transform: translateY(-2px);
  }

  &.selected {
    border-color: $intalio-teal-400;
    background: $intalio-teal-50;
  }
}
```

### Button Styles

```scss
// Primary Button
.btn-primary {
  background: $gradient-primary;
  border: none;
  color: #fff;
  font-weight: $font-weight-semibold;
  padding: $spacing-3 $spacing-5;
  border-radius: $radius-lg;

  &:hover {
    background: $gradient-primary-hover;
  }
}

// Secondary Button (on dark bg)
.btn-secondary {
  background: rgba(255, 255, 255, 0.15);
  border: 1px solid rgba(255, 255, 255, 0.3);
  color: #fff;

  &:hover {
    background: rgba(255, 255, 255, 0.25);
  }
}

// Ghost Button
.btn-ghost {
  background: transparent;
  border: 1px solid $slate-200;
  color: $slate-600;

  &:hover {
    border-color: $intalio-teal-400;
    color: $intalio-teal-600;
    background: $intalio-teal-50;
  }
}
```

### Input Fields

```scss
.input {
  height: 44px;
  padding: $spacing-3 $spacing-4;
  border: 1px solid $slate-200;
  border-radius: $radius-lg;
  background: $slate-50;
  font-size: $text-base;
  transition: all $transition-fast;

  &:focus {
    border-color: $intalio-teal-400;
    box-shadow: $shadow-focus-ring;
    background: #fff;
    outline: none;
  }

  &::placeholder {
    color: $slate-400;
  }
}
```

### Tags/Badges

```scss
.tag {
  display: inline-flex;
  align-items: center;
  gap: $spacing-1;
  padding: $spacing-1 $spacing-2;
  font-size: $text-xs;
  font-weight: $font-weight-medium;
  border-radius: $radius-md;

  &.success {
    background: $success-50;
    color: $success-600;
  }
  &.warning {
    background: $warning-50;
    color: $warning-600;
  }
  &.error {
    background: $error-50;
    color: $error-600;
  }
  &.info {
    background: $info-50;
    color: $info-600;
  }
}
```

### Avatar

```scss
.avatar {
  width: 40px;
  height: 40px;
  border-radius: $radius-full;
  overflow: hidden;
  flex-shrink: 0;

  img {
    width: 100%;
    height: 100%;
    object-fit: cover;
  }

  // Fallback initial
  &.initial {
    display: flex;
    align-items: center;
    justify-content: center;
    background: $intalio-teal-100;
    color: $intalio-teal-700;
    font-weight: $font-weight-semibold;
  }
}
```

### Icon Containers

```scss
.icon-box {
  width: 44px;
  height: 44px;
  border-radius: $radius-lg;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.25rem;

  // Semantic variants
  &.primary {
    background: rgba($intalio-teal-500, 0.1);
    color: $intalio-teal-500;
  }
  &.success {
    background: rgba($success-500, 0.1);
    color: $success-500;
  }
  &.warning {
    background: rgba($warning-500, 0.1);
    color: $warning-500;
  }
  &.error {
    background: rgba($error-500, 0.1);
    color: $error-500;
  }
}
```

---

## 11. Responsive Design

### Breakpoints

```scss
$breakpoint-sm:  640px;   // Mobile landscape
$breakpoint-md:  768px;   // Tablet portrait
$breakpoint-lg:  1024px;  // Tablet landscape / Small desktop
$breakpoint-xl:  1280px;  // Desktop
$breakpoint-2xl: 1536px;  // Large desktop
```

### Responsive Patterns

```scss
// Mobile-first approach
.container {
  padding: $spacing-4;

  @media (min-width: $breakpoint-md) {
    padding: $spacing-6;
  }
}

// Grid responsive
.grid {
  display: grid;
  gap: $spacing-4;
  grid-template-columns: 1fr;

  @media (min-width: $breakpoint-md) {
    grid-template-columns: repeat(2, 1fr);
  }

  @media (min-width: $breakpoint-lg) {
    grid-template-columns: repeat(3, 1fr);
  }

  @media (min-width: $breakpoint-xl) {
    grid-template-columns: repeat(4, 1fr);
  }
}

// Auto-fill grid
.auto-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(240px, 1fr));
  gap: $spacing-4;
}
```

---

## 12. RTL Support

### CSS Logical Properties

Always use logical properties instead of directional ones:

```scss
// DON'T
margin-left: $spacing-4;
padding-right: $spacing-4;
text-align: left;
border-left: 1px solid;

// DO
margin-inline-start: $spacing-4;
padding-inline-end: $spacing-4;
text-align: start;
border-inline-start: 1px solid;
```

### RTL Class Toggle

```vue
<div class="page" :class="{ rtl: locale === 'ar' }">
```

```scss
.page {
  &.rtl {
    direction: rtl;

    // Rotate directional icons
    .separator,
    .chevron {
      transform: rotate(180deg);
    }
  }
}
```

### Computed RTL Check

```typescript
const isRTL = computed(() => locale.value === 'ar')

const getTitle = (item: Item) => {
  return isRTL.value && item.titleArabic ? item.titleArabic : item.title
}
```

---

## 13. Accessibility

### Focus States

```scss
// All interactive elements
button, a, input, select, [tabindex] {
  &:focus-visible {
    outline: none;
    box-shadow: $shadow-focus-ring;
  }
}
```

### Keyboard Navigation

```vue
<div
  @click="handleClick"
  @keydown.enter="handleClick"
  role="button"
  tabindex="0"
>
```

### ARIA Labels

```vue
<button :aria-label="isRTL ? 'خيارات' : 'Options'">
  <i class="pi pi-ellipsis-v"></i>
</button>
```

### Color Contrast

- All text must meet WCAG AA (4.5:1 for normal text, 3:1 for large text)
- Primary teal (#00ae8d) meets AA on white backgrounds
- Use $slate-600+ for body text on white

---

## 14. Completed Pages

### Login Page
- **File**: `src/views/auth/LoginView.vue`
- **Features**:
  - Split layout with branding panel and form panel
  - Gradient hero background with mesh overlay
  - Glass-morphism form container
  - Bilingual support (EN/AR)
  - Social login buttons
  - Remember me checkbox
  - Password visibility toggle

### Main Layout
- **File**: `src/views/layouts/MainLayout.vue`
- **Features**:
  - Collapsible sidebar with smooth animation
  - Premium navigation items with hover states
  - User profile dropdown
  - Notification bell with badge
  - Language toggle
  - Search command palette trigger
  - RTL support

### Dashboard
- **File**: `src/views/dashboard/DashboardView.vue`
- **Features**:
  - Page header with gradient and mesh background
  - Welcome message with user greeting
  - Stats widgets with semantic colors
  - Quick actions grid
  - Recent activity timeline
  - Upcoming events calendar preview
  - Announcements card

### Content List View
- **File**: `src/views/content/ContentListView.vue`
- **Features**:
  - Premium page header with gradient
  - Stats bar with article counts
  - Filter toolbar with PrimeVue Dropdowns
  - Grid/List view toggle
  - Article cards with hover effects
  - Category and type filters
  - Working pagination
  - Clickable cards with navigation
  - Staggered entrance animations

### Content Detail View
- **File**: `src/views/content/ContentDetailView.vue`
- **Features**:
  - Hero section with full-width featured image
  - Author card with avatar and info
  - Rich content styling (typography)
  - Related articles sidebar
  - Share buttons
  - Tags display
  - Reading time estimate
  - Back navigation

### Document Library View
- **File**: `src/views/documents/DocumentLibraryView.vue`
- **Features**:
  - Premium page header with gradient
  - Stats bar (documents, folders, size, recent, shared, pending)
  - Drag & drop upload zone with progress
  - Search with bulk selection
  - List and Grid view toggle
  - Folders and documents display
  - File type icons with semantic colors
  - Status tags (Published, Draft, Review, Archived)
  - Version badges
  - Lock indicators
  - Action menu (Open, Download, Share, Version History, Properties, Delete)
  - Multi-select with bulk actions
  - Empty state

---

## 15. Remaining Pages

### Documents Module
- [ ] DocumentDetailView.vue - Document preview and properties

### Media Module
- [ ] MediaGalleryView.vue - Gallery listing
- [ ] MediaGalleryDetailView.vue - Gallery detail with items
- [ ] VideoEditorView.vue - Video editor interface

### Collaboration Module
- [ ] CommunitiesView.vue - Communities listing
- [ ] CommunityDetailView.vue - Community detail with discussions
- [ ] DiscussionView.vue - Discussion thread
- [ ] LessonsLearnedView.vue - Lessons learned listing
- [ ] LessonDetailView.vue - Lesson detail

### Calendar Module
- [ ] CalendarView.vue - Calendar with events

### Search Module
- [ ] SearchView.vue - Search page
- [ ] SearchResultsView.vue - Search results

### Workflow Module
- [ ] ServiceCatalogView.vue - Service catalog
- [ ] TaskInboxView.vue - Task inbox

### AI Module
- [ ] AIAssistantView.vue - AI assistant chat
- [ ] TranscriptionView.vue - Audio/video transcription

### Notifications Module
- [ ] NotificationsView.vue - Notifications list
- [ ] NotificationSettingsView.vue - Notification preferences

### Profile Module
- [ ] ProfileView.vue - User profile

### Admin Module
- [ ] UsersView.vue - User management
- [ ] RolesView.vue - Role management
- [ ] GroupsView.vue - Group management
- [ ] SettingsView.vue - System settings

### Error Pages
- [ ] NotFoundView.vue - 404 page

### Integration Module
- [ ] IntegrationView.vue - Integration settings

---

## Appendix: Quick Reference

### Z-Index Scale

```scss
$z-dropdown:       1000;
$z-sticky:         1020;
$z-fixed:          1030;
$z-modal-backdrop: 1040;
$z-modal:          1050;
$z-popover:        1060;
$z-tooltip:        1070;
```

### Layout Dimensions

```scss
$sidebar-width:           280px;
$sidebar-collapsed-width: 80px;
$header-height:           64px;
$footer-height:           48px;
```

### SCSS Import

```scss
@use '@/assets/styles/_variables.scss' as *;
```

---

*Document Version: 1.0*
*Last Updated: December 2024*
*Design System: Intalio KMS v2.0 - Elevated Modern*
