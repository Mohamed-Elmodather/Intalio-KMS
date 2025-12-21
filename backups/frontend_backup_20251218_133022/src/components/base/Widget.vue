<script setup lang="ts">
/**
 * Widget Component
 *
 * A dashboard widget wrapper with header, body, and footer sections.
 * Provides consistent styling for dashboard widgets with tabs and actions.
 *
 * @example
 * <Widget
 *   title="Recent Activity"
 *   icon="pi pi-history"
 *   :tabs="[{ label: '6 Months', value: '6m' }, { label: 'Year', value: 'year' }]"
 *   v-model:activeTab="selectedTab"
 * >
 *   <template #action>
 *     <BaseButton variant="text" size="sm">View All</BaseButton>
 *   </template>
 *   Widget content here
 * </Widget>
 */
import { computed } from 'vue'

export interface WidgetTab {
  label: string
  value: string
}

export interface WidgetProps {
  /** Widget title */
  title: string
  /** Title icon (PrimeIcons class) */
  icon?: string
  /** Tab options */
  tabs?: WidgetTab[]
  /** Active tab value (v-model) */
  activeTab?: string
  /** Show loading skeleton */
  loading?: boolean
  /** Grid column span (1-12) */
  span?: number | 'auto'
  /** Responsive span on medium screens */
  spanMd?: number
  /** Responsive span on small screens */
  spanSm?: number
  /** Custom body padding */
  noPadding?: boolean
  /** Collapse widget body */
  collapsed?: boolean
  /** Allow collapsing */
  collapsible?: boolean
}

const props = withDefaults(defineProps<WidgetProps>(), {
  span: 'auto',
  loading: false,
  noPadding: false,
  collapsed: false,
  collapsible: false
})

const emit = defineEmits<{
  'update:activeTab': [value: string]
  'update:collapsed': [value: boolean]
}>()

const isCollapsed = computed({
  get: () => props.collapsed,
  set: (value) => emit('update:collapsed', value)
})

const handleTabClick = (tab: WidgetTab) => {
  emit('update:activeTab', tab.value)
}

const toggleCollapse = () => {
  if (props.collapsible) {
    isCollapsed.value = !isCollapsed.value
  }
}

const gridStyles = computed(() => {
  const styles: Record<string, string> = {}
  if (props.span !== 'auto') {
    styles['--widget-span'] = String(props.span)
  }
  if (props.spanMd) {
    styles['--widget-span-md'] = String(props.spanMd)
  }
  if (props.spanSm) {
    styles['--widget-span-sm'] = String(props.spanSm)
  }
  return styles
})
</script>

<template>
  <div
    class="widget"
    :class="{
      'widget--collapsed': isCollapsed,
      'widget--loading': loading,
      'widget--span-auto': span === 'auto'
    }"
    :style="gridStyles"
  >
    <!-- Header -->
    <div class="widget__header">
      <div class="widget__title" @click="toggleCollapse">
        <!-- Collapse Toggle -->
        <button v-if="collapsible" class="widget__collapse-btn">
          <i class="pi" :class="isCollapsed ? 'pi-chevron-down' : 'pi-chevron-up'" />
        </button>

        <!-- Icon -->
        <i v-if="icon" :class="icon" class="widget__icon" />

        <!-- Title Text -->
        <h3>{{ title }}</h3>

        <!-- Title Slot (for badges, etc.) -->
        <slot name="title-extra" />
      </div>

      <!-- Tabs -->
      <div v-if="tabs && tabs.length > 0" class="widget__tabs">
        <button
          v-for="tab in tabs"
          :key="tab.value"
          class="widget__tab"
          :class="{ 'widget__tab--active': activeTab === tab.value }"
          @click="handleTabClick(tab)"
        >
          {{ tab.label }}
        </button>
      </div>

      <!-- Header Actions -->
      <div v-if="$slots.action" class="widget__actions">
        <slot name="action" />
      </div>
    </div>

    <!-- Body -->
    <Transition name="expand">
      <div v-show="!isCollapsed" class="widget__body" :class="{ 'widget__body--no-padding': noPadding }">
        <!-- Loading State -->
        <div v-if="loading" class="widget__loading">
          <div class="skeleton skeleton--block" />
          <div class="skeleton skeleton--block skeleton--short" />
        </div>

        <!-- Content -->
        <slot v-else />
      </div>
    </Transition>

    <!-- Footer -->
    <div v-if="$slots.footer && !isCollapsed" class="widget__footer">
      <slot name="footer" />
    </div>
  </div>
</template>

<style scoped lang="scss">
// Note: Tokens available via global injection from vite.config.ts
@use '@/design-system/mixins' as *;

.widget {
  background: white;
  border-radius: $radius-xl;
  border: 1px solid $slate-100;
  box-shadow: $shadow-card;
  display: flex;
  flex-direction: column;
  transition: box-shadow $duration-normal $ease-default;

  // Grid span
  grid-column: span var(--widget-span, 1);

  @media (max-width: $breakpoint-lg) {
    grid-column: span var(--widget-span-md, var(--widget-span, 1));
  }

  @media (max-width: $breakpoint-md) {
    grid-column: span var(--widget-span-sm, 1);
  }

  &--span-auto {
    grid-column: auto;
  }

  &:hover {
    box-shadow: $shadow-card-hover;
  }

  // Header
  &__header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    gap: $spacing-4;
    padding: $spacing-5;
    border-bottom: 1px solid $slate-100;
    flex-wrap: wrap;
  }

  &__title {
    display: flex;
    align-items: center;
    gap: $spacing-3;
    flex: 1;
    min-width: 0;

    h3 {
      font-size: $font-size-base;
      font-weight: $font-weight-semibold;
      color: $slate-900;
      margin: 0;
      white-space: nowrap;
    }
  }

  &__icon {
    font-size: 1.25rem;
    color: $brand-500;
    flex-shrink: 0;
  }

  &__collapse-btn {
    display: flex;
    align-items: center;
    justify-content: center;
    width: $spacing-6;
    height: $spacing-6;
    background: transparent;
    border: none;
    border-radius: $radius-md;
    color: $slate-400;
    cursor: pointer;
    transition: all $duration-fast $ease-default;
    margin-inline-end: $spacing-1;

    &:hover {
      background: $slate-100;
      color: $slate-600;
    }

    i {
      font-size: $font-size-xs;
    }
  }

  // Tabs
  &__tabs {
    display: flex;
    gap: $spacing-1;
    background: $slate-100;
    padding: 3px;
    border-radius: $radius-lg;
  }

  &__tab {
    padding: $spacing-2 $spacing-3;
    font-size: $font-size-xs;
    font-weight: $font-weight-medium;
    color: $slate-600;
    background: transparent;
    border: none;
    border-radius: $radius-md;
    cursor: pointer;
    transition: all $duration-fast $ease-default;
    white-space: nowrap;

    &:hover:not(&--active) {
      color: $slate-800;
    }

    &--active {
      background: white;
      color: $slate-900;
      box-shadow: $shadow-sm;
    }
  }

  // Actions
  &__actions {
    display: flex;
    align-items: center;
    gap: $spacing-2;
  }

  // Body
  &__body {
    padding: $spacing-5;
    flex: 1;

    &--no-padding {
      padding: 0;
    }
  }

  // Footer
  &__footer {
    padding: $spacing-4 $spacing-5;
    border-top: 1px solid $slate-100;
    background: $slate-50;
    border-radius: 0 0 $radius-xl $radius-xl;
  }

  // Loading
  &--loading {
    pointer-events: none;
  }

  &__loading {
    display: flex;
    flex-direction: column;
    gap: $spacing-4;
    padding: $spacing-2;
  }

  // Collapsed state
  &--collapsed {
    .widget__header {
      border-bottom-color: transparent;
    }

    .widget__title {
      cursor: pointer;
    }
  }
}

// Skeleton
.skeleton {
  background: linear-gradient(
    90deg,
    $slate-100 0%,
    $slate-200 50%,
    $slate-100 100%
  );
  background-size: 200% 100%;
  animation: shimmer 1.5s infinite;
  border-radius: $radius-lg;

  &--block {
    height: 120px;
    width: 100%;
  }

  &--short {
    height: 80px;
    width: 70%;
  }
}

@keyframes shimmer {
  0% {
    background-position: 200% 0;
  }
  100% {
    background-position: -200% 0;
  }
}

// Expand transition
.expand-enter-active,
.expand-leave-active {
  transition:
    height $duration-normal $ease-default,
    opacity $duration-normal $ease-default;
  overflow: hidden;
}

.expand-enter-from,
.expand-leave-to {
  height: 0;
  opacity: 0;
}
</style>
