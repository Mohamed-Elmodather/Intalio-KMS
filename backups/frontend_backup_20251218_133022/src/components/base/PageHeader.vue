<script setup lang="ts">
/**
 * PageHeader Component
 *
 * A consistent page header with breadcrumb, title, description, and actions.
 * Provides the gradient background pattern used across all pages.
 * Enhanced with multiple background variants and entrance animations.
 *
 * @example
 * <PageHeader
 *   title="Document Library"
 *   description="Browse and manage all documents"
 *   background-variant="gradient"
 *   animated
 *   :breadcrumbs="[
 *     { label: 'Home', to: '/dashboard' },
 *     { label: 'Documents' }
 *   ]"
 * >
 *   <template #actions>
 *     <BaseButton variant="secondary">Export</BaseButton>
 *     <BaseButton variant="primary" icon="pi pi-plus">Upload</BaseButton>
 *   </template>
 * </PageHeader>
 */
import { computed, ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useReducedMotion } from '@/composables/useReducedMotion'

export interface BreadcrumbItem {
  label: string
  labelArabic?: string
  to?: string
  icon?: string
}

export interface PageHeaderProps {
  /** Page title */
  title: string
  /** Arabic title */
  titleArabic?: string
  /** Page description */
  description?: string
  /** Arabic description */
  descriptionArabic?: string
  /** Breadcrumb navigation items */
  breadcrumbs?: BreadcrumbItem[]
  /** Show home icon in breadcrumb */
  showHomeIcon?: boolean
  /** Custom padding bottom (for stats bar overlap) */
  paddingBottom?: 'sm' | 'md' | 'lg' | 'xl'
  /** Hide background pattern */
  noPattern?: boolean
  /** Background variant */
  backgroundVariant?: 'solid' | 'gradient' | 'mesh' | 'vivid'
  /** Enable entrance animations */
  animated?: boolean
}

const props = withDefaults(defineProps<PageHeaderProps>(), {
  breadcrumbs: () => [],
  showHomeIcon: true,
  paddingBottom: 'lg',
  noPattern: false,
  backgroundVariant: 'gradient',
  animated: true
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

const router = useRouter()
const { locale } = useI18n()

const isRTL = computed(() => locale.value === 'ar')

const displayTitle = computed(() => {
  return isRTL.value && props.titleArabic ? props.titleArabic : props.title
})

const displayDescription = computed(() => {
  if (!props.description) return null
  return isRTL.value && props.descriptionArabic ? props.descriptionArabic : props.description
})

const navigateTo = (item: BreadcrumbItem) => {
  if (item.to) {
    router.push(item.to)
  }
}

const getLabel = (item: BreadcrumbItem) => {
  return isRTL.value && item.labelArabic ? item.labelArabic : item.label
}
</script>

<template>
  <header
    class="page-header"
    :class="[
      `page-header--padding-${paddingBottom}`,
      `page-header--bg-${backgroundVariant}`,
      {
        'page-header--rtl': isRTL,
        'page-header--animated': animated && !prefersReducedMotion,
        'page-header--visible': isVisible
      }
    ]"
  >
    <!-- Background -->
    <div class="page-header__background">
      <div class="page-header__gradient" />
      <div v-if="!noPattern" class="page-header__pattern" />
      <!-- Animated mesh orbs for vivid variant -->
      <div v-if="backgroundVariant === 'mesh' || backgroundVariant === 'vivid'" class="page-header__orbs">
        <div class="page-header__orb page-header__orb--1" />
        <div class="page-header__orb page-header__orb--2" />
        <div class="page-header__orb page-header__orb--3" />
      </div>
    </div>

    <!-- Content -->
    <div class="page-header__content">
      <div class="page-header__left">
        <!-- Breadcrumb -->
        <nav v-if="breadcrumbs.length > 0" class="page-header__breadcrumb">
          <template v-for="(item, index) in breadcrumbs" :key="index">
            <!-- Clickable breadcrumb item -->
            <button
              v-if="item.to"
              class="page-header__breadcrumb-link"
              :style="animated && !prefersReducedMotion ? { '--stagger-index': index } : undefined"
              @click="navigateTo(item)"
            >
              <i v-if="index === 0 && showHomeIcon" class="pi pi-home" />
              <i v-else-if="item.icon" :class="item.icon" />
              <span>{{ getLabel(item) }}</span>
            </button>

            <!-- Current/non-clickable item -->
            <span
              v-else
              class="page-header__breadcrumb-current"
              :style="animated && !prefersReducedMotion ? { '--stagger-index': index } : undefined"
            >
              {{ getLabel(item) }}
            </span>

            <!-- Separator -->
            <i
              v-if="index < breadcrumbs.length - 1"
              class="pi pi-chevron-right page-header__breadcrumb-sep"
              :style="animated && !prefersReducedMotion ? { '--stagger-index': index } : undefined"
            />
          </template>
        </nav>

        <!-- Title -->
        <h1 class="page-header__title">{{ displayTitle }}</h1>

        <!-- Description -->
        <p v-if="displayDescription" class="page-header__description">
          {{ displayDescription }}
        </p>

        <!-- Extra content slot -->
        <slot name="extra" />
      </div>

      <!-- Actions -->
      <div v-if="$slots.actions" class="page-header__actions">
        <slot name="actions" />
      </div>
    </div>
  </header>
</template>

<style scoped lang="scss">
// Note: Tokens available via global injection from vite.config.ts
@use '@/design-system/mixins' as *;

.page-header {
  position: relative;
  margin: (-$spacing-6) (-$spacing-6) $spacing-6;
  padding: $spacing-8 $spacing-6;
  overflow: hidden;

  @media (max-width: $breakpoint-md) {
    margin: (-$spacing-4) (-$spacing-4) $spacing-4;
    padding: $spacing-6 $spacing-4;
  }

  // Padding variants (for stats bar overlap)
  &--padding-sm {
    padding-bottom: $spacing-8;
  }

  &--padding-md {
    padding-bottom: $spacing-12;
  }

  &--padding-lg {
    padding-bottom: $spacing-16;
  }

  &--padding-xl {
    padding-bottom: $spacing-20;
  }

  // RTL
  &--rtl {
    direction: rtl;

    .page-header__breadcrumb-sep {
      transform: rotate(180deg);
    }
  }

  // Background
  &__background {
    position: absolute;
    inset: 0;
    overflow: hidden;
  }

  &__gradient {
    position: absolute;
    inset: 0;
    background: $gradient-primary;
  }

  &__pattern {
    position: absolute;
    inset: 0;
    opacity: 0.1;
    background-image:
      radial-gradient(circle at 20% 50%, rgba(white, 0.3) 0%, transparent 50%),
      radial-gradient(circle at 80% 50%, rgba(white, 0.2) 0%, transparent 40%);
    pointer-events: none;
  }

  // Content
  &__content {
    position: relative;
    z-index: 1;
    display: flex;
    justify-content: space-between;
    align-items: flex-start;
    gap: $spacing-6;

    @media (max-width: $breakpoint-lg) {
      flex-direction: column;
      align-items: stretch;
    }
  }

  &__left {
    flex: 1;
    min-width: 0;
  }

  // Breadcrumb
  &__breadcrumb {
    display: flex;
    align-items: center;
    flex-wrap: wrap;
    gap: $spacing-2;
    margin-bottom: $spacing-3;
  }

  &__breadcrumb-link {
    display: inline-flex;
    align-items: center;
    gap: $spacing-2;
    padding: $spacing-1 $spacing-2;
    background: rgba(white, 0.15);
    border: none;
    border-radius: $radius-md;
    color: rgba(white, 0.9);
    font-size: $font-size-sm;
    font-weight: $font-weight-medium;
    cursor: pointer;
    transition: all $duration-fast $ease-default;

    i {
      font-size: $font-size-sm;
    }

    &:hover {
      background: rgba(white, 0.25);
      color: white;
    }
  }

  &__breadcrumb-sep {
    color: rgba(white, 0.5);
    font-size: $font-size-xs;
  }

  &__breadcrumb-current {
    color: rgba(white, 0.7);
    font-size: $font-size-sm;
    font-weight: $font-weight-medium;
  }

  // Title
  &__title {
    font-size: $font-size-3xl;
    font-weight: $font-weight-bold;
    color: white;
    margin: 0 0 $spacing-2;
    letter-spacing: -0.02em;
    line-height: 1.2;

    @media (max-width: $breakpoint-md) {
      font-size: $font-size-2xl;
    }
  }

  // Description
  &__description {
    font-size: $font-size-base;
    color: rgba(white, 0.85);
    margin: 0;
    max-width: 600px;
    line-height: 1.5;

    @media (max-width: $breakpoint-md) {
      font-size: $font-size-sm;
    }
  }

  // Actions
  &__actions {
    display: flex;
    align-items: center;
    gap: $spacing-3;
    flex-shrink: 0;

    @media (max-width: $breakpoint-lg) {
      width: 100%;
      justify-content: flex-end;
    }

    @media (max-width: $breakpoint-sm) {
      flex-direction: column;

      :deep(.base-btn),
      :deep(button) {
        width: 100%;
      }
    }
  }

  // ================================
  // Background Variants
  // ================================

  &--bg-solid {
    .page-header__gradient {
      background: $intalio-teal-600;
    }
  }

  &--bg-gradient {
    .page-header__gradient {
      background: $gradient-primary;
    }
  }

  &--bg-mesh {
    .page-header__gradient {
      background: linear-gradient(135deg, $intalio-teal-600 0%, $intalio-teal-500 100%);
    }

    .page-header__pattern {
      opacity: 0.15;
      background:
        radial-gradient(circle at 20% 30%, rgba(white, 0.4) 0%, transparent 40%),
        radial-gradient(circle at 80% 60%, rgba(white, 0.3) 0%, transparent 50%),
        radial-gradient(circle at 50% 80%, rgba(white, 0.2) 0%, transparent 40%);
    }
  }

  &--bg-vivid {
    .page-header__gradient {
      background: linear-gradient(135deg, $intalio-teal-600 0%, $intalio-teal-500 50%, #00c9a7 100%);
    }

    .page-header__pattern {
      opacity: 0.2;
    }
  }

  // Animated orbs
  &__orbs {
    position: absolute;
    inset: 0;
    overflow: hidden;
    pointer-events: none;
  }

  &__orb {
    position: absolute;
    border-radius: 50%;
    filter: blur(60px);
    opacity: 0.3;

    &--1 {
      width: 200px;
      height: 200px;
      background: rgba(white, 0.4);
      top: -50px;
      right: 10%;
      animation: orbFloat1 8s ease-in-out infinite;
    }

    &--2 {
      width: 150px;
      height: 150px;
      background: rgba(white, 0.3);
      bottom: -30px;
      left: 20%;
      animation: orbFloat2 10s ease-in-out infinite;
    }

    &--3 {
      width: 100px;
      height: 100px;
      background: rgba(white, 0.25);
      top: 30%;
      right: 30%;
      animation: orbFloat3 12s ease-in-out infinite;
    }
  }

  // ================================
  // Animation States
  // ================================

  &--animated {
    .page-header__title,
    .page-header__description,
    .page-header__breadcrumb-link,
    .page-header__breadcrumb-current,
    .page-header__breadcrumb-sep,
    .page-header__actions {
      opacity: 0;
      transform: translateY(10px);
    }

    &.page-header--visible {
      .page-header__title {
        animation: headerFadeUp 0.5s ease-out 0.1s forwards;
      }

      .page-header__description {
        animation: headerFadeUp 0.5s ease-out 0.2s forwards;
      }

      .page-header__breadcrumb-link,
      .page-header__breadcrumb-current,
      .page-header__breadcrumb-sep {
        animation: headerFadeUp 0.4s ease-out forwards;
        animation-delay: calc(0.05s * var(--stagger-index, 0));
      }

      .page-header__actions {
        animation: headerFadeUp 0.5s ease-out 0.3s forwards;
      }
    }
  }
}

// ================================
// Keyframe Animations
// ================================

@keyframes headerFadeUp {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

@keyframes orbFloat1 {
  0%, 100% {
    transform: translate(0, 0) scale(1);
  }
  50% {
    transform: translate(-20px, 10px) scale(1.1);
  }
}

@keyframes orbFloat2 {
  0%, 100% {
    transform: translate(0, 0) scale(1);
  }
  50% {
    transform: translate(15px, -15px) scale(1.05);
  }
}

@keyframes orbFloat3 {
  0%, 100% {
    transform: translate(0, 0) scale(1);
  }
  50% {
    transform: translate(-10px, 20px) scale(0.95);
  }
}

// ================================
// Reduced Motion
// ================================

@media (prefers-reduced-motion: reduce) {
  .page-header {
    &--animated {
      .page-header__title,
      .page-header__description,
      .page-header__breadcrumb-link,
      .page-header__breadcrumb-current,
      .page-header__breadcrumb-sep,
      .page-header__actions {
        opacity: 1;
        transform: none;
        animation: none !important;
      }
    }

    &__orb {
      animation: none !important;
    }
  }
}
</style>
