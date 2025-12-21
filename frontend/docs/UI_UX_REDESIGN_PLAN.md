# AFC Asian Cup 2027 Knowledge Portal - UI/UX Redesign Plan

## Executive Summary

A comprehensive redesign to transform the portal into a **modern, elegant, vivid, and engaging** experience while maintaining the **Intalio Teal (#00ae8d)** corporate identity. The redesign follows a **"Balanced & Modern"** animation philosophy with thoughtful visual effects.

**Current State:** Visual engagement 6/10 | **Target State:** 9/10

---

## Design Philosophy

### Animation Intensity: Balanced & Modern
- Noticeable but not distracting animations
- Engaging hover effects and micro-interactions
- Smooth page transitions
- Modern SaaS feel (think Linear, Stripe, Notion)

### Visual Effects Strategy
| Effect | Usage | Purpose |
|--------|-------|---------|
| **Glassmorphism** | Header, modals, floating panels | Modern depth and elegance |
| **Gradient Accents** | Hero sections, buttons, text highlights | Brand vibrancy |
| **Layered Shadows** | Cards, dropdowns, FABs | Visual hierarchy |
| **Subtle Glow** | Focus states, brand elements | Premium feel |

---

## Implementation Phases (8 Weeks)

### Phase 1: Design System Enhancement (Week 1-2)

#### 1.1 New Color Tokens
**File:** `src/design-system/tokens/_colors-extended.scss` (NEW)

Add vibrant accent colors while keeping Intalio Teal primary:
- **Coral** (#ff5252) - Warm accent for alerts, highlights
- **Purple** (#a855f7) - Creative accent for special features
- **Gold** (#eab308) - Premium highlights, achievements
- **Ocean** (#06b6d4) - Cool accent for info, links

Add semantic glow colors:
```scss
$color-glow-brand: rgba($brand-400, 0.5);
$color-glow-accent: rgba($purple-400, 0.4);
```

#### 1.2 Visual Effects Tokens
**File:** `src/design-system/tokens/_effects.scss` (NEW)

**Glassmorphism Presets:**
- `$glass-light-subtle` - 8px blur, 60% white
- `$glass-light-medium` - 12px blur, 75% white
- `$glass-light-strong` - 20px blur, 90% white
- `$glass-dark-medium` - 12px blur, 80% dark
- `$glass-brand-subtle` - Brand-tinted glass

**Premium Gradients:**
- `$gradient-hero-vivid` - Multi-color hero (teal → ocean → purple)
- `$gradient-hero-warm` - Warm variant (teal → gold → coral)
- `$gradient-animated-mesh` - Layered radial gradients for backgrounds
- `$gradient-shimmer` - Loading shimmer effect

**Glow Effects:**
- `$glow-brand-subtle/medium/strong/intense`
- `$glow-hover-card` - Combined shadow + brand glow
- `$glow-hover-button` - Button hover state

#### 1.3 Effects Mixins
**File:** `src/design-system/mixins/_effects.scss` (NEW)

```scss
@mixin glass($variant, $with-border)     // Glassmorphism
@mixin glass-card($blur)                  // Card-specific glass
@mixin glow($color, $intensity)           // Glow effects
@mixin glow-hover($color)                 // Interactive glow
@mixin gradient-text($gradient)           // Gradient text
@mixin gradient-border($gradient, $width) // Gradient borders
@mixin gradient-shimmer-effect            // Shimmer on hover
@mixin depth($level)                      // Depth hierarchy
@mixin depth-hover($from, $to)            // Progressive depth
```

#### 1.4 Enhanced Shadow System
**Update:** `src/design-system/tokens/_shadows.scss`

Add dramatic layered shadows:
- `$shadow-hero` - Multi-layer for featured content
- `$shadow-glass` - With ambient glow
- `$shadow-fab` - Floating action buttons
- `$shadow-ambient-brand` - Soft colored shadows
- `$shadow-long-drop` - Dramatic depth

---

### Phase 2: Animation Library (Week 2-3)

#### 2.1 New Keyframes
**Update:** `src/design-system/animations/_keyframes.scss`

**Entrance Animations (12 new):**
- `fadeInScale`, `fadeInBlur`, `slideInBlur`
- `expandIn`, `revealUp`, `revealDown`
- Exit variants for each

**Micro-Interactions (8 new):**
- `buttonPress`, `buttonRipple`
- `iconPop`, `heartBeat`, `wiggle`, `rubber`

**Loading States (6 new):**
- `spinnerGrow`, `barLoader`, `dotWave`
- `pulseRing`, `skeletonWave`

**Scroll-Triggered (5 new):**
- `scrollFadeIn`, `scrollScaleIn`
- `scrollSlideInLeft/Right`, `parallaxFloat`

**Page Transitions (4 new):**
- `pageEnterScale`, `pageEnterSlide`
- `pageLeaveScale`, `pageLeaveSlide`

**Ambient/Decorative (5 new):**
- `gradientFlow`, `colorShift`, `borderGlow`
- `morphBlob`

#### 2.2 Enhanced Vue Transitions
**Update:** `src/design-system/animations/_transitions.scss`

New transition classes:
- `.page-blur-*` - Blur fade page transition
- `.page-scale-*` - Scale page transition
- `.page-forward/backward-*` - Directional transitions
- `.stagger-list-*` - Staggered list animations
- `.stagger-grid-*` - Grid item animations
- `.panel-slide/fade-*` - Drawer/panel transitions
- `.notification-*` - Toast animations
- `.card-flip/morph-*` - Card transitions
- `.collapse-smooth-*` - Accordion transitions

#### 2.3 Animation Utility Classes
**Update:** `src/design-system/animations/_utility-classes.scss`

**Scroll-Triggered:**
```scss
.scroll-animate        // Base class
.scroll-fade-in        // Fade in on scroll
.scroll-scale-in       // Scale in on scroll
.scroll-slide-left/right
```

**Stagger System:**
```scss
.stagger-children      // Parent container
.stagger-fast          // 50ms delay
.stagger-normal        // 75ms delay
.stagger-slow          // 100ms delay
```

**Micro-Interactions:**
```scss
.micro-press           // Scale on active
.micro-pop             // Scale on hover
.micro-wiggle          // Wiggle on hover
.micro-heartbeat       // Pulse on hover
```

**Loading States:**
```scss
.loading-skeleton      // Shimmer skeleton
.loading-pulse         // Pulse animation
.loading-spinner       // Spin animation
.loading-dots          // Bouncing dots
```

**Hover Effects:**
```scss
.hover-lift            // Translate up
.hover-lift-shadow     // With shadow
.hover-glow-brand      // Brand glow
.hover-scale           // Scale up
.hover-shimmer         // Shimmer sweep
```

---

### Phase 3: Component Enhancements (Week 3-4)

#### 3.1 BaseButton.vue Enhancements
**File:** `src/components/base/BaseButton.vue`

**New Props:**
```typescript
ripple?: boolean              // Enable ripple effect (default: true)
rippleColor?: string          // Custom ripple color
loadingVariant?: 'spinner' | 'dots' | 'pulse'
```

**Features:**
- Click ripple effect using CSS pseudo-element
- Enhanced loading states with variants
- Reduced motion support
- ARIA improvements (`aria-busy`, `aria-disabled`)

#### 3.2 BaseCard.vue Enhancements
**File:** `src/components/base/BaseCard.vue`

**New Props:**
```typescript
hover3d?: boolean             // 3D tilt on hover
glassBlur?: 'sm' | 'md' | 'lg'
skeletonVariant?: 'simple' | 'content' | 'media'
```

**Features:**
- 3D perspective transform on hover
- Enhanced glassmorphism with configurable blur
- Improved skeleton with shimmer wave
- Gradient border option

#### 3.3 BaseInput.vue Enhancements
**File:** `src/components/base/BaseInput.vue`

**New Props:**
```typescript
floatingLabel?: boolean       // Floating label animation
focusRingExpand?: boolean     // Expanding focus ring
```

**Features:**
- Floating label transition (placeholder → label)
- Animated focus ring expansion
- Icon color transition on focus
- Enhanced error state animation

#### 3.4 New Animation Components

**AnimatedCounter.vue** (NEW)
```typescript
interface Props {
  value: number
  duration?: number           // Default: 1000ms
  easing?: 'linear' | 'easeOut' | 'spring'
  decimals?: number
  prefix?: string             // e.g., '$'
  suffix?: string             // e.g., '%'
  separator?: string          // Thousands separator
  startOnVisible?: boolean    // Intersection Observer
}
```

**AnimatedList.vue** (NEW)
```typescript
interface Props {
  items: any[]
  staggerDelay?: number       // Default: 100ms
  animation?: 'fadeUp' | 'fadeIn' | 'slideIn' | 'scaleIn'
  startOnVisible?: boolean
}
// Slot: #item="{ item, index }"
```

**AnimatedReveal.vue** (NEW)
```typescript
interface Props {
  animation?: 'fadeUp' | 'fadeIn' | 'slideLeft' | 'scaleIn' | 'blur'
  duration?: number
  delay?: number
  threshold?: number          // Intersection threshold
  once?: boolean              // Animate only once
}
```

**Skeleton Components** (NEW - `src/components/base/skeleton/`)
- `SkeletonBase.vue` - Foundation with shimmer
- `SkeletonText.vue` - Lines with configurable count
- `SkeletonImage.vue` - Aspect ratio presets
- `SkeletonCard.vue` - Card layout skeleton
- `SkeletonAvatar.vue` - Circle/square variants

#### 3.5 New Composables

**useRipple.ts** (NEW)
```typescript
function useRipple(elementRef, options?) {
  // Returns: { createRipple: (event) => void }
}
```

**useReducedMotion.ts** (NEW)
```typescript
function useReducedMotion(): ComputedRef<boolean>
```

**useScrollReveal.ts** (NEW)
```typescript
function useScrollReveal(options?) {
  // Intersection Observer wrapper
}
```

**useCountUp.ts** (NEW)
```typescript
function useCountUp(target, options?) {
  // Animated counter logic
}
```

---

### Phase 4: Layout & Navigation (Week 4-5)

#### 4.1 MainLayout.vue Redesign
**File:** `src/views/layouts/MainLayout.vue`

**Sidebar Enhancements:**
- Spring-based collapse/expand: `cubic-bezier(0.34, 1.56, 0.64, 1)`
- Nav item hover: scale + background transition
- Active indicator: animated slide bar
- Collapsed tooltips: fade + scale entrance

**Header Glassmorphism:**
```scss
.header {
  background: linear-gradient(
    135deg,
    rgba(255, 255, 255, 0.95) 0%,
    rgba(255, 255, 255, 0.85) 100%
  );
  backdrop-filter: blur(20px);
  border-bottom: 1px solid rgba(255, 255, 255, 0.3);
  box-shadow: 0 4px 30px rgba(0, 0, 0, 0.05);
}
```

**Page Transitions:**
- Forward navigation: slide-in from right
- Backward navigation: slide-in from left
- Default: scale + fade

#### 4.2 PageHeader.vue Enhancement
**File:** `src/components/base/PageHeader.vue`

**New Props:**
```typescript
backgroundVariant?: 'solid' | 'gradient' | 'mesh' | 'image'
animated?: boolean            // Entrance animation
```

**Features:**
- Animated gradient mesh background option
- Staggered breadcrumb animation
- Action button stagger entrance

#### 4.3 SidebarNavigation.vue Enhancement
**File:** `src/components/dashboard/SidebarNavigation.vue`

**Features:**
- Staggered item entrance on mount
- Active indicator slide animation
- Badge pulse for notifications
- Icon hover micro-animation

---

### Phase 5: Dashboard Flagship (Week 5-6)

#### 5.1 DashboardView.vue Complete Redesign
**File:** `src/views/dashboard/DashboardView.vue`

**Hero Section:**
- Animated gradient mesh background
- Floating decorative elements (morphBlob)
- Personalized greeting with subtle animation
- Time/date with gentle pulse

**Stats Section:**
- AnimatedCounter for all numeric values
- Staggered card entrance (100ms delay each)
- Trend indicators with bounce animation
- Hover: lift + brand glow

**Widgets Grid:**
- Staggered widget entrance (150ms delay each)
- Loading: shimmer skeleton
- Collapse/expand: smooth transition
- Tab switching: fade animation

**Activity Feed:**
- Staggered list entrance (75ms delay)
- New items: slide-in animation
- Avatar: subtle scale on hover

**Charts:**
- Line drawing animation on mount
- Point appearance animation
- Tooltip: fade + scale

**Tasks Section:**
- Swipe gesture hints
- Completion: checkmark animation
- Overdue: pulse glow effect
- Priority: colored indicator glow

---

### Phase 6: View-by-View Enhancement (Week 6-7)

#### Priority 1: High-Traffic Views

| View | Key Enhancements |
|------|------------------|
| **ContentListView** | Card grid stagger, filter slide-down, view mode morph |
| **DocumentLibraryView** | Tree expand animation, upload progress, drag feedback |
| **TaskInboxView** | Panel slide-in, completion confetti, priority pulse |

#### Priority 2: Collaboration Views

| View | Key Enhancements |
|------|------------------|
| **CommunitiesView** | Card hover depth, avatar stack expand, join success |
| **CalendarView** | View transition, event drag feedback, date selection |
| **DiscussionView** | Comment cascade, reaction pop, thread expand |

#### Priority 3: Media & AI Views

| View | Key Enhancements |
|------|------------------|
| **MediaGalleryView** | Masonry load animation, lightbox zoom, upload progress |
| **AIServicesView** | AI thinking orbs, result streaming, relevance bar fill |
| **SemanticSearchView** | Source cascade reveal, loading branded animation |

#### Priority 4: Admin Views

| View | Key Enhancements |
|------|------------------|
| **UsersView/RolesView/GroupsView** | Table row hover, edit transition, batch progress |

---

### Phase 7: Polish & Accessibility (Week 7-8)

#### 7.1 RTL Animation Support
All animations must work correctly in RTL:
- Reverse `translateX` animations
- Mirror sidebar collapse direction
- Adjust slide directions
- Test all directional animations

#### 7.2 Reduced Motion Support
```scss
@media (prefers-reduced-motion: reduce) {
  *,
  *::before,
  *::after {
    animation-duration: 0.01ms !important;
    animation-iteration-count: 1 !important;
    transition-duration: 0.01ms !important;
  }
}
```

#### 7.3 Performance Optimization
- Use `transform` and `opacity` only (GPU accelerated)
- Strategic `will-change` hints
- Debounce scroll animations
- Lazy load heavy animations

#### 7.4 Accessibility Audit
- All interactive elements focusable
- Visible focus states
- ARIA attributes on animated elements
- Screen reader announcements for state changes

---

## File Structure After Redesign

```
src/design-system/
├── tokens/
│   ├── _colors.scss (existing)
│   ├── _colors-extended.scss (NEW)
│   ├── _effects.scss (NEW)
│   ├── _shadows.scss (updated)
│   └── ...
├── mixins/
│   ├── _effects.scss (NEW)
│   ├── _interactive.scss (updated)
│   └── ...
├── animations/
│   ├── _keyframes.scss (major update)
│   ├── _transitions.scss (updated)
│   └── _utility-classes.scss (major update)
└── utilities/
    └── _effects.scss (NEW)

src/components/base/
├── AnimatedCounter.vue (NEW)
├── AnimatedList.vue (NEW)
├── AnimatedReveal.vue (NEW)
├── EmptyState.vue (NEW)
├── ShimmerLoader.vue (NEW)
├── skeleton/ (NEW directory)
│   ├── SkeletonBase.vue
│   ├── SkeletonText.vue
│   ├── SkeletonImage.vue
│   ├── SkeletonCard.vue
│   ├── SkeletonAvatar.vue
│   └── index.ts
├── BaseButton.vue (enhanced)
├── BaseCard.vue (enhanced)
├── BaseInput.vue (enhanced)
├── PageHeader.vue (enhanced)
├── StatsBar.vue (enhanced)
└── Widget.vue (enhanced)

src/composables/
├── useRipple.ts (NEW)
├── useReducedMotion.ts (NEW)
├── useScrollReveal.ts (NEW)
├── useCountUp.ts (NEW)
└── usePageTransition.ts (NEW)

src/directives/
└── v-scroll-reveal.ts (NEW)
```

---

## Critical Files Summary

### New Files (15)
1. `src/design-system/tokens/_colors-extended.scss`
2. `src/design-system/tokens/_effects.scss`
3. `src/design-system/mixins/_effects.scss`
4. `src/design-system/utilities/_effects.scss`
5. `src/components/base/AnimatedCounter.vue`
6. `src/components/base/AnimatedList.vue`
7. `src/components/base/AnimatedReveal.vue`
8. `src/components/base/EmptyState.vue`
9. `src/components/base/ShimmerLoader.vue`
10. `src/components/base/skeleton/*.vue` (5 files)
11. `src/composables/useRipple.ts`
12. `src/composables/useReducedMotion.ts`
13. `src/composables/useScrollReveal.ts`
14. `src/composables/useCountUp.ts`
15. `src/directives/v-scroll-reveal.ts`

### Major Updates (12)
1. `src/design-system/animations/_keyframes.scss` - 40+ new keyframes
2. `src/design-system/animations/_transitions.scss` - New Vue transitions
3. `src/design-system/animations/_utility-classes.scss` - Interaction utilities
4. `src/design-system/tokens/_shadows.scss` - Enhanced shadow system
5. `src/design-system/mixins/_interactive.scss` - New interaction mixins
6. `src/components/base/BaseButton.vue` - Ripple, loading variants
7. `src/components/base/BaseCard.vue` - 3D hover, glassmorphism
8. `src/components/base/BaseInput.vue` - Floating label, focus ring
9. `src/components/base/StatsBar.vue` - AnimatedCounter integration
10. `src/views/layouts/MainLayout.vue` - Sidebar, header, transitions
11. `src/views/dashboard/DashboardView.vue` - Complete redesign
12. `src/components/dashboard/SidebarNavigation.vue` - Stagger, active indicator

---

## Success Metrics

| Metric | Current | Target |
|--------|---------|--------|
| Visual Engagement Score | 6/10 | 9/10 |
| Animation Coverage | ~20% | ~80% |
| Micro-interactions | Minimal | Comprehensive |
| Loading State Quality | Basic | Premium |
| Accessibility (Motion) | None | Full support |
| RTL Animation Support | Partial | Complete |

---

## Notes

- All enhancements maintain backwards compatibility
- Corporate identity (Intalio Teal) preserved and enhanced
- Performance optimized with GPU-accelerated properties
- RTL support built into all animations
- Reduced motion preference respected throughout
