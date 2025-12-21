<script setup lang="ts">
/**
 * StatsBar Component
 *
 * A floating stats bar that overlaps the page header.
 * Displays key metrics with icons, optional trends, and animated counters.
 * Enhanced with glassmorphism effect and micro-interactions.
 *
 * @example
 * <StatsBar
 *   :stats="[
 *     { icon: 'pi pi-file', value: 42, label: 'Documents', colorClass: 'primary' },
 *     { icon: 'pi pi-check', value: 18, label: 'Completed', colorClass: 'success' },
 *     { icon: 'pi pi-clock', value: 5, label: 'Pending', colorClass: 'warning' }
 *   ]"
 *   animated
 *   animateNumbers
 * />
 */
import { computed, ref, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { useReducedMotion } from '@/composables/useReducedMotion'
import AnimatedCounter from '@/components/base/AnimatedCounter.vue'

export interface StatItem {
  /** PrimeIcons icon class */
  icon: string
  /** Stat value (number or string) */
  value: number | string
  /** Stat label */
  label: string
  /** Arabic label */
  labelArabic?: string
  /** Color variant */
  colorClass?: 'primary' | 'success' | 'warning' | 'error' | 'info'
  /** Optional trend indicator */
  trend?: {
    value: string
    direction: 'up' | 'down' | 'neutral'
  }
  /** Click handler */
  onClick?: () => void
}

export interface StatsBarProps {
  /** Array of stat items to display */
  stats: StatItem[]
  /** Show loading skeleton */
  loading?: boolean
  /** Overlap amount (negative margin top) */
  overlap?: 'sm' | 'md' | 'lg'
  /** Animation stagger enabled */
  animated?: boolean
  /** Animate numeric values with counter */
  animateNumbers?: boolean
  /** Counter animation duration in ms */
  counterDuration?: number
  /** Use glass effect for container */
  glass?: boolean
}

const props = withDefaults(defineProps<StatsBarProps>(), {
  loading: false,
  overlap: 'md',
  animated: true,
  animateNumbers: true,
  counterDuration: 1000,
  glass: false
})

const prefersReducedMotion = useReducedMotion()
const isVisible = ref(false)

const { locale } = useI18n()

const isRTL = computed(() => locale.value === 'ar')

// Check if should animate
const shouldAnimate = computed(() => props.animated && !prefersReducedMotion.value)
const shouldAnimateNumbers = computed(() => props.animateNumbers && !prefersReducedMotion.value)

onMounted(() => {
  if (shouldAnimate.value) {
    requestAnimationFrame(() => {
      isVisible.value = true
    })
  } else {
    isVisible.value = true
  }
})

const getLabel = (stat: StatItem) => {
  return isRTL.value && stat.labelArabic ? stat.labelArabic : stat.label
}

const formatValue = (value: number | string) => {
  if (typeof value === 'number') {
    return value.toLocaleString()
  }
  return value
}

// Check if value is a number for AnimatedCounter
const isNumericValue = (value: number | string): value is number => {
  return typeof value === 'number'
}
</script>

<template>
  <div
    class="stats-bar"
    :class="[
      `stats-bar--overlap-${overlap}`,
      {
        'stats-bar--rtl': isRTL,
        'stats-bar--animated': shouldAnimate,
        'stats-bar--visible': isVisible,
        'stats-bar--glass': glass
      }
    ]"
  >
    <div class="stats-bar__container">
      <!-- Loading State -->
      <template v-if="loading">
        <div v-for="i in 3" :key="i" class="stats-bar__item stats-bar__item--loading">
          <div class="skeleton skeleton--icon" />
          <div class="skeleton skeleton--text" />
        </div>
      </template>

      <!-- Stats -->
      <template v-else>
        <template v-for="(stat, index) in stats" :key="stat.label">
          <div
            class="stats-bar__item"
            :class="{
              'stats-bar__item--clickable': stat.onClick,
              [`stats-bar__item--${stat.colorClass || 'primary'}`]: true
            }"
            :style="shouldAnimate ? { '--item-index': index } : undefined"
            @click="stat.onClick?.()"
          >
            <!-- Icon -->
            <div class="stats-bar__icon" :class="`stats-bar__icon--${stat.colorClass || 'primary'}`">
              <i :class="stat.icon" />
            </div>

            <!-- Content -->
            <div class="stats-bar__content">
              <!-- Animated Counter for numbers -->
              <AnimatedCounter
                v-if="isNumericValue(stat.value) && shouldAnimateNumbers"
                :value="stat.value"
                :duration="counterDuration"
                :start-on-visible="true"
                class="stats-bar__value"
              />
              <!-- Static value for strings or when animation disabled -->
              <span v-else class="stats-bar__value">{{ formatValue(stat.value) }}</span>
              <span class="stats-bar__label">{{ getLabel(stat) }}</span>
            </div>

            <!-- Trend -->
            <div v-if="stat.trend" class="stats-bar__trend" :class="`stats-bar__trend--${stat.trend.direction}`">
              <i
                class="pi"
                :class="{
                  'pi-arrow-up': stat.trend.direction === 'up',
                  'pi-arrow-down': stat.trend.direction === 'down',
                  'pi-minus': stat.trend.direction === 'neutral'
                }"
              />
              <span>{{ stat.trend.value }}</span>
            </div>
          </div>

          <!-- Divider -->
          <div v-if="index < stats.length - 1" class="stats-bar__divider" />
        </template>
      </template>
    </div>
  </div>
</template>

<style scoped lang="scss">
// Note: Tokens available via global injection from vite.config.ts
@use '@/design-system/mixins' as *;

.stats-bar {
  position: relative;
  z-index: $z-dropdown;
  margin-inline: $spacing-6;
  margin-bottom: $spacing-6;

  @media (max-width: $breakpoint-md) {
    margin-inline: $spacing-4;
    margin-bottom: $spacing-4;
  }

  // Overlap variants
  &--overlap-sm {
    margin-top: -$spacing-6;
  }

  &--overlap-md {
    margin-top: -$spacing-10;
  }

  &--overlap-lg {
    margin-top: -$spacing-16;
  }

  // RTL
  &--rtl {
    direction: rtl;
  }

  // Container
  &__container {
    display: flex;
    align-items: center;
    justify-content: center;
    gap: $spacing-8;
    padding: $spacing-5 $spacing-6;
    background: white;
    border-radius: $radius-2xl;
    box-shadow: $shadow-lg;
    border: 1px solid $slate-100;
    transition: box-shadow $duration-normal $ease-default;

    @media (max-width: $breakpoint-lg) {
      gap: $spacing-6;
      padding: $spacing-4 $spacing-5;
    }

    @media (max-width: $breakpoint-md) {
      flex-wrap: wrap;
      gap: $spacing-4;
    }
  }

  // Glass variant
  &--glass &__container {
    background: rgba(255, 255, 255, 0.85);
    backdrop-filter: blur(12px);
    -webkit-backdrop-filter: blur(12px);
    border: 1px solid rgba(255, 255, 255, 0.4);
    box-shadow:
      0 4px 30px rgba(0, 0, 0, 0.08),
      0 1px 2px rgba(0, 0, 0, 0.05);
  }

  // Item
  &__item {
    display: flex;
    align-items: center;
    gap: $spacing-3;
    transition: transform $duration-fast $ease-default;

    &--clickable {
      cursor: pointer;
      padding: $spacing-2;
      margin: -$spacing-2;
      border-radius: $radius-lg;
      transition:
        background $duration-fast $ease-default,
        transform $duration-fast $ease-default,
        box-shadow $duration-fast $ease-default;

      &:hover {
        background: $slate-50;
        transform: translateY(-2px);
      }

      &:active {
        transform: translateY(0) scale(0.98);
      }
    }

    // Color-based hover glow effects
    &--primary:hover .stats-bar__icon--primary {
      box-shadow: 0 0 20px rgba($brand-500, 0.3);
    }

    &--success:hover .stats-bar__icon--success {
      box-shadow: 0 0 20px rgba($success-500, 0.3);
    }

    &--warning:hover .stats-bar__icon--warning {
      box-shadow: 0 0 20px rgba($warning-500, 0.3);
    }

    &--error:hover .stats-bar__icon--error {
      box-shadow: 0 0 20px rgba($error-500, 0.3);
    }

    &--info:hover .stats-bar__icon--info {
      box-shadow: 0 0 20px rgba($info-500, 0.3);
    }

    &--loading {
      opacity: 1;
    }

    @media (max-width: $breakpoint-md) {
      flex: 1 1 calc(50% - $spacing-4);
      min-width: 120px;
    }
  }

  // Animated state - items hidden initially
  &--animated &__item {
    opacity: 0;
    transform: translateY(15px);
  }

  // When visible, animate items in with stagger
  &--animated.stats-bar--visible &__item {
    animation: statsFadeInUp 0.5s ease-out forwards;
    animation-delay: calc(var(--item-index, 0) * 100ms + 100ms);
  }

  // Animation disabled or loading
  &:not(.stats-bar--animated) &__item,
  &--animated &__item--loading {
    opacity: 1;
    transform: none;
    animation: none;
  }

  // Icon
  &__icon {
    display: flex;
    align-items: center;
    justify-content: center;
    width: 44px;
    height: 44px;
    border-radius: $radius-xl;
    flex-shrink: 0;
    transition:
      transform $duration-fast $ease-default,
      box-shadow $duration-fast $ease-default;

    i {
      font-size: 1.25rem;
      transition: transform $duration-fast $ease-default;
    }

    &--primary {
      background: rgba($brand-500, 0.1);
      color: $brand-600;
    }

    &--success {
      background: rgba($success-500, 0.1);
      color: $success-600;
    }

    &--warning {
      background: rgba($warning-500, 0.1);
      color: $warning-600;
    }

    &--error {
      background: rgba($error-500, 0.1);
      color: $error-600;
    }

    &--info {
      background: rgba($info-500, 0.1);
      color: $info-600;
    }

    .stats-bar__item:hover & {
      transform: scale(1.08);

      i {
        transform: scale(1.05);
      }
    }
  }

  // Content
  &__content {
    display: flex;
    flex-direction: column;
    gap: $spacing-0-5;
  }

  &__value {
    font-size: $font-size-xl;
    font-weight: $font-weight-bold;
    color: $slate-900;
    line-height: 1.2;

    @media (max-width: $breakpoint-md) {
      font-size: $font-size-lg;
    }
  }

  &__label {
    font-size: $font-size-sm;
    color: $slate-500;
    white-space: nowrap;

    @media (max-width: $breakpoint-md) {
      font-size: $font-size-xs;
    }
  }

  // Trend
  &__trend {
    display: flex;
    align-items: center;
    gap: $spacing-1;
    padding: $spacing-1 $spacing-2;
    border-radius: $radius-full;
    font-size: $font-size-xs;
    font-weight: $font-weight-semibold;

    i {
      font-size: 0.625rem;
    }

    &--up {
      background: $success-50;
      color: $success-600;
    }

    &--down {
      background: $error-50;
      color: $error-600;
    }

    &--neutral {
      background: $slate-100;
      color: $slate-600;
    }
  }

  // Divider
  &__divider {
    width: 1px;
    height: 40px;
    background: $slate-200;
    flex-shrink: 0;

    @media (max-width: $breakpoint-md) {
      display: none;
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
  border-radius: $radius-md;

  &--icon {
    width: 44px;
    height: 44px;
    border-radius: $radius-xl;
  }

  &--text {
    width: 80px;
    height: $spacing-10;
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

@keyframes statsFadeInUp {
  from {
    opacity: 0;
    transform: translateY(15px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

@keyframes trendBounce {
  0%, 100% {
    transform: translateY(0);
  }
  50% {
    transform: translateY(-2px);
  }
}

// ================================
// Reduced Motion Support
// ================================

@media (prefers-reduced-motion: reduce) {
  .stats-bar {
    &--animated &__item {
      opacity: 1;
      transform: none;
      animation: none !important;
    }

    &__item--clickable {
      &:hover,
      &:active {
        transform: none;
      }
    }

    &__icon {
      transition: none;

      i {
        transition: none;
      }
    }

    .stats-bar__item:hover .stats-bar__icon {
      transform: none;

      i {
        transform: none;
      }
    }

    &__trend {
      animation: none !important;
    }
  }

  .skeleton {
    animation: none;
    background: $slate-200;
  }
}
</style>
