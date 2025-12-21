<script setup lang="ts">
/**
 * BaseButton Component
 *
 * A flexible button component with multiple variants, sizes, and states.
 * Uses design system tokens for consistent styling.
 * Enhanced with ripple effect and micro-interactions.
 *
 * @example
 * <BaseButton variant="primary" size="md" icon="pi pi-plus">
 *   Create New
 * </BaseButton>
 */
import { computed, ref } from 'vue'
import { useRipple } from '@/composables/useRipple'
import { useReducedMotion } from '@/composables/useReducedMotion'

export interface BaseButtonProps {
  /** Button style variant */
  variant?: 'primary' | 'secondary' | 'ghost' | 'outline' | 'danger' | 'success' | 'text'
  /** Button size */
  size?: 'sm' | 'md' | 'lg'
  /** PrimeIcons icon class (e.g., 'pi pi-plus') */
  icon?: string
  /** Icon position */
  iconPos?: 'left' | 'right'
  /** Icon-only button (no text) */
  iconOnly?: boolean
  /** Rounded corners */
  rounded?: boolean
  /** Pill shape (fully rounded) */
  pill?: boolean
  /** Full width button */
  block?: boolean
  /** Loading state */
  loading?: boolean
  /** Disabled state */
  disabled?: boolean
  /** HTML button type */
  type?: 'button' | 'submit' | 'reset'
  /** HTML tag to render */
  tag?: string
  /** Enable ripple effect */
  ripple?: boolean
  /** Custom ripple color */
  rippleColor?: string
  /** Loading variant */
  loadingVariant?: 'spinner' | 'dots' | 'pulse'
}

const props = withDefaults(defineProps<BaseButtonProps>(), {
  variant: 'primary',
  size: 'md',
  iconPos: 'left',
  iconOnly: false,
  rounded: false,
  pill: false,
  block: false,
  loading: false,
  disabled: false,
  type: 'button',
  tag: 'button',
  ripple: true,
  loadingVariant: 'spinner'
})

const emit = defineEmits<{
  click: [event: MouseEvent]
}>()

const buttonRef = ref<HTMLElement | null>(null)
const prefersReducedMotion = useReducedMotion()

// Determine ripple color based on variant
const rippleColorComputed = computed(() => {
  if (props.rippleColor) return props.rippleColor
  switch (props.variant) {
    case 'primary':
    case 'danger':
    case 'success':
      return 'rgba(255, 255, 255, 0.3)'
    default:
      return 'rgba(0, 174, 141, 0.2)'
  }
})

// Setup ripple effect
const { createRipple } = useRipple(buttonRef, {
  color: rippleColorComputed.value,
  disabled: !props.ripple || prefersReducedMotion.value
})

const isDisabled = computed(() => props.disabled || props.loading)

const handleClick = (event: MouseEvent) => {
  if (!isDisabled.value) {
    if (props.ripple && !prefersReducedMotion.value) {
      createRipple(event)
    }
    emit('click', event)
  }
}
</script>

<template>
  <component
    :is="tag"
    ref="buttonRef"
    :type="tag === 'button' ? type : undefined"
    class="base-btn"
    :class="[
      `base-btn--${variant}`,
      `base-btn--${size}`,
      {
        'base-btn--icon-only': iconOnly,
        'base-btn--rounded': rounded,
        'base-btn--pill': pill,
        'base-btn--block': block,
        'base-btn--loading': loading,
        'base-btn--disabled': isDisabled,
        'base-btn--ripple': ripple
      }
    ]"
    :disabled="isDisabled"
    :aria-busy="loading"
    :aria-disabled="isDisabled"
    @click="handleClick"
  >
    <!-- Loading Spinner -->
    <i v-if="loading" class="pi pi-spinner pi-spin base-btn__loader" />

    <!-- Left Icon -->
    <i
      v-else-if="icon && iconPos === 'left'"
      :class="icon"
      class="base-btn__icon"
    />

    <!-- Button Text -->
    <span v-if="!iconOnly && $slots.default" class="base-btn__text">
      <slot />
    </span>

    <!-- Right Icon -->
    <i
      v-if="icon && iconPos === 'right' && !loading"
      :class="icon"
      class="base-btn__icon"
    />
  </component>
</template>

<style scoped lang="scss">
// Note: Tokens available via global injection from vite.config.ts
@use '@/design-system/mixins' as *;

.base-btn {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: $spacing-2;
  font-weight: $font-weight-medium;
  font-family: inherit;
  line-height: 1;
  border: none;
  outline: none;
  cursor: pointer;
  transition:
    background $duration-fast $ease-default,
    color $duration-fast $ease-default,
    border-color $duration-fast $ease-default,
    box-shadow $duration-fast $ease-default,
    transform $duration-fast $ease-default;
  white-space: nowrap;
  user-select: none;

  &:focus-visible {
    box-shadow: $shadow-focus-ring;
  }

  // Sizes
  &--sm {
    font-size: $font-size-xs;
    padding: $spacing-2 $spacing-3;
    border-radius: $radius-md;
    min-height: $spacing-8;

    &.base-btn--icon-only {
      width: $spacing-8;
      padding: $spacing-2;
    }
  }

  &--md {
    font-size: $font-size-sm;
    padding: $spacing-3 $spacing-5;
    border-radius: $radius-lg;
    min-height: $spacing-10;

    &.base-btn--icon-only {
      width: $spacing-10;
      padding: $spacing-3;
    }
  }

  &--lg {
    font-size: $font-size-base;
    padding: $spacing-4 $spacing-6;
    border-radius: $radius-lg;
    min-height: $spacing-12;

    &.base-btn--icon-only {
      width: $spacing-12;
      padding: $spacing-4;
    }
  }

  // Variants
  &--primary {
    background: $gradient-primary;
    color: white;
    box-shadow: $shadow-teal-sm;

    &:hover:not(.base-btn--disabled) {
      background: $gradient-primary-hover;
      box-shadow: $shadow-teal-md;
      transform: translateY(-1px);
    }

    &:active:not(.base-btn--disabled) {
      transform: translateY(0);
      box-shadow: $shadow-teal-sm;
    }
  }

  &--secondary {
    background: white;
    color: $slate-700;
    border: 1px solid $slate-200;
    box-shadow: $shadow-sm;

    &:hover:not(.base-btn--disabled) {
      background: $slate-50;
      border-color: $slate-300;
      box-shadow: $shadow-md;
    }

    &:active:not(.base-btn--disabled) {
      background: $slate-100;
    }
  }

  &--ghost {
    background: transparent;
    color: $slate-600;

    &:hover:not(.base-btn--disabled) {
      background: $slate-100;
      color: $slate-900;
    }

    &:active:not(.base-btn--disabled) {
      background: $slate-200;
    }
  }

  &--outline {
    background: transparent;
    color: $brand-600;
    border: 1.5px solid $brand-500;

    &:hover:not(.base-btn--disabled) {
      background: $brand-50;
      border-color: $brand-600;
    }

    &:active:not(.base-btn--disabled) {
      background: $brand-100;
    }
  }

  &--danger {
    background: $error-500;
    color: white;

    &:hover:not(.base-btn--disabled) {
      background: $error-600;
      transform: translateY(-1px);
    }

    &:active:not(.base-btn--disabled) {
      background: $error-700;
      transform: translateY(0);
    }
  }

  &--success {
    background: $success-500;
    color: white;

    &:hover:not(.base-btn--disabled) {
      background: $success-600;
      transform: translateY(-1px);
    }

    &:active:not(.base-btn--disabled) {
      background: $success-700;
      transform: translateY(0);
    }
  }

  &--text {
    background: transparent;
    color: $brand-600;
    padding-inline: $spacing-2;

    &:hover:not(.base-btn--disabled) {
      color: $brand-700;
      text-decoration: underline;
    }
  }

  // Modifiers
  &--rounded {
    border-radius: $radius-xl !important;
  }

  &--pill {
    border-radius: $radius-full !important;
  }

  &--block {
    width: 100%;
  }

  &--disabled {
    opacity: 0.6;
    cursor: not-allowed;
    pointer-events: none;
  }

  &--loading {
    pointer-events: none;

    .base-btn__text {
      opacity: 0.7;
    }
  }

  // Ripple enabled (adds overflow hidden)
  &--ripple {
    position: relative;
    overflow: hidden;
  }

  // Elements
  &__icon {
    font-size: 1em;
    flex-shrink: 0;
    transition: transform $duration-fast $ease-default;
  }

  &__loader {
    font-size: 1em;
  }

  &__text {
    flex: 0 0 auto;
  }

  // Icon hover micro-interaction
  &:hover:not(.base-btn--disabled) {
    .base-btn__icon {
      transform: scale(1.1);
    }
  }

  // Active press animation
  &:active:not(.base-btn--disabled) {
    animation: buttonPress 0.15s ease-out;
  }
}

// Reduced motion support
@media (prefers-reduced-motion: reduce) {
  .base-btn {
    transition: none !important;
    animation: none !important;

    &:hover:not(.base-btn--disabled),
    &:active:not(.base-btn--disabled) {
      transform: none !important;
    }

    .base-btn__icon {
      transition: none !important;
    }
  }
}
</style>
