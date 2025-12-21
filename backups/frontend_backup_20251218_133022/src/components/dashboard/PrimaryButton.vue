<script setup lang="ts">
/**
 * PrimaryButton Component
 *
 * A primary action button styled with the brand gradient.
 * Uses the design system tokens for consistent styling.
 *
 * @example
 * <PrimaryButton @click="handleClick">Create New</PrimaryButton>
 * <PrimaryButton icon="pi pi-plus" :loading="isLoading">Add Item</PrimaryButton>
 */
import { computed } from 'vue'

export interface PrimaryButtonProps {
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
}

const props = withDefaults(defineProps<PrimaryButtonProps>(), {
  type: 'button',
  iconPos: 'left',
  loading: false,
  disabled: false,
  block: false,
  size: 'md'
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
    class="primary-btn"
    :class="[
      `primary-btn--${size}`,
      {
        'primary-btn--block': block,
        'primary-btn--loading': loading,
        'primary-btn--icon-right': iconPos === 'right'
      }
    ]"
    :disabled="isDisabled"
    @click="handleClick"
  >
    <!-- Loading Spinner -->
    <span v-if="loading" class="primary-btn__spinner">
      <i class="pi pi-spinner pi-spin"></i>
    </span>

    <!-- Icon Left -->
    <i
      v-else-if="icon && iconPos === 'left'"
      :class="icon"
      class="primary-btn__icon"
    ></i>

    <!-- Label -->
    <span class="primary-btn__label">
      <slot />
    </span>

    <!-- Icon Right -->
    <i
      v-if="icon && iconPos === 'right' && !loading"
      :class="icon"
      class="primary-btn__icon"
    ></i>
  </button>
</template>

<style scoped lang="scss">
// Note: Tokens available via global injection from vite.config.ts
@use '@/design-system/mixins' as *;

.primary-btn {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: $spacing-2;
  padding: $spacing-3 $spacing-5;
  background: $gradient-brand-primary;
  color: white;
  font-family: inherit;
  font-size: $font-size-sm;
  font-weight: $font-weight-semibold;
  line-height: 1;
  border: none;
  border-radius: $radius-lg;
  cursor: pointer;
  white-space: nowrap;
  transition:
    transform $duration-fast $ease-default,
    box-shadow $duration-fast $ease-default,
    opacity $duration-fast $ease-default;

  &:hover:not(:disabled) {
    background: $gradient-brand-primary-hover;
    transform: translateY(-1px);
    box-shadow: $shadow-brand-sm;
  }

  &:active:not(:disabled) {
    transform: translateY(0);
    box-shadow: none;
  }

  &:focus-visible {
    outline: none;
    box-shadow: $shadow-focus-ring-brand;
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

  // Icon right
  &--icon-right {
    flex-direction: row-reverse;
  }

  // Icon
  &__icon {
    font-size: 1em;
    flex-shrink: 0;
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
