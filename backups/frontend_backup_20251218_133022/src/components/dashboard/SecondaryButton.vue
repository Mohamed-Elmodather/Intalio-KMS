<script setup lang="ts">
/**
 * SecondaryButton Component
 *
 * A secondary action button with subtle styling.
 * Used for less prominent actions alongside primary buttons.
 *
 * @example
 * <SecondaryButton @click="handleCancel">Cancel</SecondaryButton>
 * <SecondaryButton icon="pi pi-eye">View All</SecondaryButton>
 */
import { computed } from 'vue'

export interface SecondaryButtonProps {
  /** Button type attribute */
  type?: 'button' | 'submit' | 'reset'
  /** Icon class (PrimeIcons) */
  icon?: string
  /** Icon position */
  iconPos?: 'left' | 'right'
  /** Show loading spinner */
  loading?: boolean
  /** Disable the button */
  disabled?: boolean
  /** Full width button */
  block?: boolean
  /** Button size */
  size?: 'sm' | 'md' | 'lg'
  /** Ghost variant (text only, no background) */
  ghost?: boolean
}

const props = withDefaults(defineProps<SecondaryButtonProps>(), {
  type: 'button',
  iconPos: 'left',
  loading: false,
  disabled: false,
  block: false,
  size: 'md',
  ghost: false
})

const emit = defineEmits<{
  click: [event: MouseEvent]
}>()

const isDisabled = computed(() => props.disabled || props.loading)

function handleClick(event: MouseEvent) {
  if (!isDisabled.value) {
    emit('click', event)
  }
}
</script>

<template>
  <button
    :type="type"
    class="secondary-btn"
    :class="[
      `secondary-btn--${size}`,
      {
        'secondary-btn--block': block,
        'secondary-btn--loading': loading,
        'secondary-btn--ghost': ghost,
        'secondary-btn--icon-right': iconPos === 'right'
      }
    ]"
    :disabled="isDisabled"
    @click="handleClick"
  >
    <!-- Loading Spinner -->
    <span v-if="loading" class="secondary-btn__spinner">
      <i class="pi pi-spinner pi-spin"></i>
    </span>

    <!-- Icon Left -->
    <i
      v-else-if="icon && iconPos === 'left'"
      :class="icon"
      class="secondary-btn__icon"
    ></i>

    <!-- Label -->
    <span class="secondary-btn__label">
      <slot />
    </span>

    <!-- Icon Right -->
    <i
      v-if="icon && iconPos === 'right' && !loading"
      :class="icon"
      class="secondary-btn__icon"
    ></i>
  </button>
</template>

<style scoped lang="scss">
// Note: Tokens available via global injection from vite.config.ts
@use '@/design-system/mixins' as *;

.secondary-btn {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: $spacing-2;
  padding: $spacing-3 $spacing-5;
  background: white;
  color: $color-text-primary;
  font-family: inherit;
  font-size: $font-size-sm;
  font-weight: $font-weight-semibold;
  line-height: 1;
  border: 1px solid $color-border-default;
  border-radius: $radius-lg;
  cursor: pointer;
  white-space: nowrap;
  transition:
    background-color $duration-fast $ease-default,
    border-color $duration-fast $ease-default,
    color $duration-fast $ease-default,
    transform $duration-fast $ease-default,
    box-shadow $duration-fast $ease-default;

  &:hover:not(:disabled) {
    background: $color-bg-secondary;
    border-color: $color-border-dark;
    transform: translateY(-1px);
  }

  &:active:not(:disabled) {
    transform: translateY(0);
    background: $color-bg-tertiary;
  }

  &:focus-visible {
    outline: none;
    box-shadow: $shadow-focus-ring;
  }

  &:disabled {
    opacity: 0.6;
    cursor: not-allowed;
  }

  // Sizes
  &--sm {
    padding: $spacing-2 $spacing-3;
    font-size: $font-size-xs;
    gap: $spacing-1-5;
  }

  &--lg {
    padding: $spacing-4 $spacing-6;
    font-size: $font-size-base;
    gap: $spacing-3;
  }

  // Block
  &--block {
    width: 100%;
  }

  // Loading
  &--loading {
    pointer-events: none;
  }

  // Ghost variant
  &--ghost {
    background: transparent;
    border-color: transparent;
    color: $color-brand-primary;

    &:hover:not(:disabled) {
      background: $color-bg-brand-subtle;
      border-color: transparent;
      color: $color-brand-primary-dark;
    }

    &:active:not(:disabled) {
      background: rgba($color-brand-primary, 0.15);
    }
  }

  // Icon right
  &--icon-right {
    flex-direction: row-reverse;
  }

  // Icon
  &__icon {
    font-size: 1em;
    flex-shrink: 0;
    color: $color-text-secondary;
    transition: color $duration-fast $ease-default;

    .secondary-btn:hover:not(:disabled) & {
      color: $color-text-primary;
    }

    .secondary-btn--ghost & {
      color: $color-brand-primary;
    }
  }

  // Spinner
  &__spinner {
    display: flex;
    align-items: center;
    justify-content: center;

    i {
      font-size: 1em;
    }
  }

  // Label
  &__label {
    display: inline-flex;
    align-items: center;
  }
}
</style>
