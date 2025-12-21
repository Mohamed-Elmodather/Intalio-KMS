<script setup lang="ts">
/**
 * SidebarNavigation Component
 *
 * A reusable sidebar navigation component with support for
 * collapsible state, active route detection, and grouped items.
 * Enhanced with staggered entrance animations and micro-interactions.
 *
 * @example
 * <SidebarNavigation
 *   :items="navigationItems"
 *   :collapsed="isCollapsed"
 *   animated
 *   @navigate="handleNavigate"
 * />
 */
import { computed, ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useReducedMotion } from '@/composables/useReducedMotion'

export interface NavItem {
  /** Unique identifier */
  id: string
  /** Display label */
  label: string
  /** Arabic label for RTL */
  labelArabic?: string
  /** Icon class (PrimeIcons) */
  icon: string
  /** Route path */
  route: string
  /** Use exact match for active state */
  exact?: boolean
  /** Badge count */
  badge?: number
  /** Badge variant */
  badgeVariant?: 'primary' | 'success' | 'warning' | 'error'
  /** Is this item disabled */
  disabled?: boolean
}

export interface NavGroup {
  /** Group identifier */
  id: string
  /** Group header label */
  label?: string
  /** Arabic group label */
  labelArabic?: string
  /** Items in this group */
  items: NavItem[]
}

export interface SidebarNavigationProps {
  /** Navigation items or groups */
  items: (NavItem | NavGroup)[]
  /** Collapsed state */
  collapsed?: boolean
  /** Currently active route (optional, auto-detected if not provided) */
  activeRoute?: string
  /** Enable entrance animations */
  animated?: boolean
  /** Stagger delay between items (ms) */
  staggerDelay?: number
}

const props = withDefaults(defineProps<SidebarNavigationProps>(), {
  collapsed: false,
  animated: true,
  staggerDelay: 50
})

const prefersReducedMotion = useReducedMotion()
const isVisible = ref(false)

onMounted(() => {
  if (props.animated && !prefersReducedMotion.value) {
    requestAnimationFrame(() => {
      isVisible.value = true
    })
  } else {
    isVisible.value = true
  }
})

const emit = defineEmits<{
  navigate: [item: NavItem]
}>()

const route = useRoute()
const router = useRouter()

// Check if item is a group
function isNavGroup(item: NavItem | NavGroup): item is NavGroup {
  return 'items' in item
}

// Check if route is active
function isActiveRoute(item: NavItem): boolean {
  if (props.activeRoute) {
    return props.activeRoute === item.route
  }

  if (item.exact) {
    return route.path === item.route
  }
  return route.path.startsWith(item.route)
}

// Handle navigation
function handleNavigation(item: NavItem) {
  if (item.disabled) return

  emit('navigate', item)
  router.push(item.route)
}

// Get badge class based on variant
function getBadgeClass(variant?: string): string {
  const classes: Record<string, string> = {
    primary: 'nav-badge--primary',
    success: 'nav-badge--success',
    warning: 'nav-badge--warning',
    error: 'nav-badge--error'
  }
  return classes[variant || 'primary'] || classes.primary
}

// Flatten items for simpler rendering if no groups
const flattenedItems = computed(() => {
  const result: { group?: NavGroup; items: NavItem[] }[] = []

  props.items.forEach(item => {
    if (isNavGroup(item)) {
      result.push({ group: item, items: item.items })
    } else {
      // Find or create ungrouped section
      const ungrouped = result.find(r => !r.group)
      if (ungrouped) {
        ungrouped.items.push(item)
      } else {
        result.unshift({ items: [item] })
      }
    }
  })

  return result
})

// Get global item index for stagger animation
function getItemIndex(sectionIndex: number, itemIndex: number): number {
  let globalIndex = 0
  for (let s = 0; s < sectionIndex; s++) {
    globalIndex += flattenedItems.value[s].items.length
  }
  return globalIndex + itemIndex
}

// Check if should animate (based on props and reduced motion preference)
const shouldAnimate = computed(() => props.animated && !prefersReducedMotion.value)
</script>

<template>
  <nav
    class="sidebar-nav"
    :class="{
      'sidebar-nav--collapsed': collapsed,
      'sidebar-nav--animated': shouldAnimate,
      'sidebar-nav--visible': isVisible
    }"
  >
    <template v-for="(section, sectionIndex) in flattenedItems" :key="section.group?.id || 'main'">
      <!-- Group Header -->
      <div
        v-if="section.group?.label && !collapsed"
        class="nav-group-header"
      >
        <span>{{ section.group.label }}</span>
      </div>

      <!-- Divider for collapsed state -->
      <div
        v-else-if="section.group && collapsed"
        class="nav-group-divider"
      ></div>

      <!-- Navigation Items -->
      <div class="nav-group">
        <button
          v-for="(item, itemIndex) in section.items"
          :key="item.id"
          type="button"
          class="nav-item"
          :class="{
            'nav-item--active': isActiveRoute(item),
            'nav-item--disabled': item.disabled
          }"
          :style="shouldAnimate ? { '--item-index': getItemIndex(sectionIndex, itemIndex) } : undefined"
          :disabled="item.disabled"
          :title="collapsed ? item.label : undefined"
          @click="handleNavigation(item)"
        >
          <!-- Icon -->
          <span class="nav-item__icon">
            <i :class="item.icon"></i>
          </span>

          <!-- Label -->
          <span v-if="!collapsed" class="nav-item__label">
            {{ item.label }}
          </span>

          <!-- Badge -->
          <span
            v-if="item.badge !== undefined && item.badge > 0 && !collapsed"
            class="nav-item__badge"
            :class="[getBadgeClass(item.badgeVariant), { 'nav-item__badge--pulse': item.badge > 0 }]"
          >
            {{ item.badge > 99 ? '99+' : item.badge }}
          </span>

          <!-- Badge dot for collapsed -->
          <span
            v-if="item.badge !== undefined && item.badge > 0 && collapsed"
            class="nav-item__badge-dot"
            :class="[getBadgeClass(item.badgeVariant), { 'nav-item__badge-dot--pulse': true }]"
          ></span>

          <!-- Active Indicator -->
          <span v-if="isActiveRoute(item)" class="nav-item__indicator"></span>
        </button>
      </div>
    </template>
  </nav>
</template>

<style scoped lang="scss">
// Note: Tokens available via global injection from vite.config.ts
@use '@/design-system/mixins' as *;

.sidebar-nav {
  display: flex;
  flex-direction: column;
  padding: $spacing-4;
  gap: $spacing-1;

  // Collapsed state
  &--collapsed {
    padding: $spacing-3;

    .nav-item {
      justify-content: center;
      padding: $spacing-3;

      &__label {
        display: none;
      }
    }
  }

  // Animation state - items hidden initially
  &--animated {
    .nav-item {
      opacity: 0;
      transform: translateX(-10px);
    }

    // When visible, animate items in with stagger
    &.sidebar-nav--visible {
      .nav-item {
        animation: navItemFadeIn 0.4s ease-out forwards;
        animation-delay: calc(var(--item-index, 0) * 50ms);
      }
    }
  }
}

// Group header
.nav-group-header {
  padding: $spacing-2 $spacing-4;
  margin-top: $spacing-4;
  margin-bottom: $spacing-2;

  &:first-child {
    margin-top: 0;
  }

  span {
    font-size: $font-size-xs;
    font-weight: $font-weight-semibold;
    color: $color-text-muted;
    text-transform: uppercase;
    letter-spacing: 0.08em;
  }
}

// Group divider (collapsed state)
.nav-group-divider {
  height: 1px;
  background: $color-border-light;
  margin: $spacing-3 $spacing-2;
}

// Navigation group
.nav-group {
  display: flex;
  flex-direction: column;
  gap: $spacing-1;
}

// Navigation item
.nav-item {
  position: relative;
  display: flex;
  align-items: center;
  gap: $spacing-3;
  width: 100%;
  padding: $spacing-3 $spacing-4;
  background: transparent;
  border: none;
  border-radius: $radius-xl;
  color: $color-text-secondary;
  font-family: inherit;
  font-size: $font-size-sm;
  font-weight: $font-weight-medium;
  text-align: start;
  cursor: pointer;
  transition:
    background-color $duration-fast $ease-default,
    color $duration-fast $ease-default,
    transform $duration-fast $ease-default;

  &:hover:not(:disabled) {
    background: $color-bg-tertiary;
    color: $color-text-primary;
    transform: translateX(2px);

    .nav-item__icon {
      color: $color-brand-primary;
      transform: scale(1.05);

      i {
        animation: iconPop 0.3s ease-out;
      }
    }
  }

  &:active:not(:disabled) {
    transform: translateX(2px) scale(0.98);
  }

  &:focus-visible {
    outline: none;
    box-shadow: inset 0 0 0 2px $color-brand-primary;
  }

  // Active state
  &--active {
    background: rgba($color-brand-primary, 0.1);
    color: $color-brand-primary-dark;

    .nav-item__icon {
      color: $color-brand-primary;
      background: rgba($color-brand-primary, 0.15);
    }

    &:hover:not(:disabled) {
      transform: none;
    }
  }

  // Disabled state
  &--disabled {
    opacity: 0.5;
    cursor: not-allowed;
    pointer-events: none;
  }

  // Icon
  &__icon {
    display: flex;
    align-items: center;
    justify-content: center;
    width: 36px;
    height: 36px;
    border-radius: $radius-lg;
    color: $color-text-muted;
    flex-shrink: 0;
    transition:
      color $duration-fast $ease-default,
      background-color $duration-fast $ease-default,
      transform $duration-fast $ease-default;

    i {
      font-size: 1.125rem;
    }
  }

  // Label
  &__label {
    flex: 1;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
  }

  // Badge
  &__badge {
    display: inline-flex;
    align-items: center;
    justify-content: center;
    min-width: 20px;
    height: 20px;
    padding: 0 $spacing-1-5;
    font-size: $font-size-xs;
    font-weight: $font-weight-bold;
    border-radius: $radius-full;
    flex-shrink: 0;
    transition: transform $duration-fast $ease-default;

    &--primary {
      background: $color-bg-brand-subtle;
      color: $color-brand-primary;
    }

    &--success {
      background: $color-success-light;
      color: $color-success-dark;
    }

    &--warning {
      background: $color-warning-light;
      color: $color-warning-dark;
    }

    &--error {
      background: $color-error-light;
      color: $color-error-dark;
    }

    // Pulse animation for new notifications
    &--pulse {
      animation: badgePulse 2s ease-in-out infinite;
    }
  }

  // Badge dot (for collapsed state)
  &__badge-dot {
    position: absolute;
    top: $spacing-2;
    inset-inline-end: $spacing-2;
    width: 8px;
    height: 8px;
    border-radius: $radius-full;
    border: 2px solid $color-bg-primary;

    &--primary {
      background: $color-brand-primary;
    }

    &--success {
      background: $color-success;
    }

    &--warning {
      background: $color-warning;
    }

    &--error {
      background: $color-error;
    }

    // Pulse animation for collapsed badge dot
    &--pulse {
      animation: badgeDotPulse 2s ease-in-out infinite;
    }
  }

  // Active indicator
  &__indicator {
    position: absolute;
    inset-inline-start: 0;
    top: 50%;
    transform: translateY(-50%);
    width: 3px;
    height: 24px;
    background: $color-brand-primary;
    border-radius: 0 $radius-full $radius-full 0;
    animation: indicatorSlideIn 0.3s ease-out forwards;
  }
}

// RTL support
[dir="rtl"] {
  .nav-item__indicator {
    inset-inline-start: auto;
    inset-inline-end: 0;
    border-radius: $radius-full 0 0 $radius-full;
  }

  .sidebar-nav--animated .nav-item {
    transform: translateX(10px);
  }

  .sidebar-nav--animated.sidebar-nav--visible .nav-item {
    animation-name: navItemFadeInRTL;
  }

  .nav-item:hover:not(:disabled) {
    transform: translateX(-2px);
  }
}

// ================================
// Keyframe Animations
// ================================

@keyframes navItemFadeIn {
  from {
    opacity: 0;
    transform: translateX(-10px);
  }
  to {
    opacity: 1;
    transform: translateX(0);
  }
}

@keyframes navItemFadeInRTL {
  from {
    opacity: 0;
    transform: translateX(10px);
  }
  to {
    opacity: 1;
    transform: translateX(0);
  }
}

@keyframes iconPop {
  0% {
    transform: scale(1);
  }
  50% {
    transform: scale(1.15);
  }
  100% {
    transform: scale(1);
  }
}

@keyframes badgePulse {
  0%, 100% {
    transform: scale(1);
  }
  50% {
    transform: scale(1.1);
  }
}

@keyframes badgeDotPulse {
  0%, 100% {
    transform: scale(1);
    box-shadow: 0 0 0 0 rgba($color-brand-primary, 0.4);
  }
  50% {
    transform: scale(1.1);
    box-shadow: 0 0 0 4px rgba($color-brand-primary, 0);
  }
}

@keyframes indicatorSlideIn {
  from {
    opacity: 0;
    transform: translateY(-50%) scaleY(0);
  }
  to {
    opacity: 1;
    transform: translateY(-50%) scaleY(1);
  }
}

// ================================
// Reduced Motion Support
// ================================

@media (prefers-reduced-motion: reduce) {
  .sidebar-nav {
    &--animated {
      .nav-item {
        opacity: 1;
        transform: none;
        animation: none !important;
      }
    }
  }

  .nav-item {
    transition: background-color $duration-fast $ease-default,
                color $duration-fast $ease-default;

    &:hover:not(:disabled) {
      transform: none;

      .nav-item__icon {
        transform: none;

        i {
          animation: none !important;
        }
      }
    }

    &:active:not(:disabled) {
      transform: none;
    }

    &__badge--pulse,
    &__badge-dot--pulse {
      animation: none !important;
    }

    &__indicator {
      animation: none !important;
    }
  }
}
</style>
