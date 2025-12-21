<script setup lang="ts">
/**
 * BaseInput Component
 *
 * A flexible input component with multiple variants and states.
 * Supports text, search, password, and textarea modes.
 * Uses design system tokens for consistent styling.
 * Enhanced with floating label animation and focus ring effects.
 *
 * @example
 * <BaseInput
 *   v-model="email"
 *   variant="elevated"
 *   label="Email Address"
 *   placeholder="Enter your email"
 *   icon="pi pi-envelope"
 *   :error="emailError"
 *   floating-label
 * />
 */
import { ref, computed, useAttrs } from 'vue'

export interface BaseInputProps {
  /** v-model binding */
  modelValue?: string | number
  /** Input style variant */
  variant?: 'default' | 'elevated' | 'filled' | 'ghost'
  /** Input size */
  size?: 'sm' | 'md' | 'lg'
  /** Input type */
  type?: 'text' | 'password' | 'email' | 'number' | 'tel' | 'url' | 'search'
  /** Label text */
  label?: string
  /** Placeholder text */
  placeholder?: string
  /** Helper text below input */
  helperText?: string
  /** Error message */
  error?: string
  /** Success state */
  success?: boolean
  /** Left icon (PrimeIcons class) */
  icon?: string
  /** Right icon (PrimeIcons class) */
  iconRight?: string
  /** Disabled state */
  disabled?: boolean
  /** Readonly state */
  readonly?: boolean
  /** Required field */
  required?: boolean
  /** Show clear button */
  clearable?: boolean
  /** Use as textarea */
  multiline?: boolean
  /** Textarea rows */
  rows?: number
  /** Full width */
  block?: boolean
  /** Enable floating label animation */
  floatingLabel?: boolean
  /** Enable expanding focus ring */
  focusRingExpand?: boolean
  /** Show animated success checkmark */
  showSuccessIcon?: boolean
}

const props = withDefaults(defineProps<BaseInputProps>(), {
  modelValue: '',
  variant: 'default',
  size: 'md',
  type: 'text',
  rows: 3,
  disabled: false,
  readonly: false,
  required: false,
  clearable: false,
  multiline: false,
  block: true,
  floatingLabel: false,
  focusRingExpand: false,
  showSuccessIcon: false
})

const emit = defineEmits<{
  'update:modelValue': [value: string | number]
  focus: [event: FocusEvent]
  blur: [event: FocusEvent]
  clear: []
}>()

const attrs = useAttrs()

const isFocused = ref(false)
const showPassword = ref(false)

const inputValue = computed({
  get: () => props.modelValue,
  set: (value) => emit('update:modelValue', value)
})

const computedType = computed(() => {
  if (props.type === 'password') {
    return showPassword.value ? 'text' : 'password'
  }
  return props.type
})

const hasValue = computed(() => {
  return props.modelValue !== '' && props.modelValue !== null && props.modelValue !== undefined
})

const isLabelFloated = computed(() => {
  return props.floatingLabel && (isFocused.value || hasValue.value)
})

const showClearButton = computed(() => {
  return props.clearable && hasValue.value && !props.disabled && !props.readonly
})

const handleFocus = (event: FocusEvent) => {
  isFocused.value = true
  emit('focus', event)
}

const handleBlur = (event: FocusEvent) => {
  isFocused.value = false
  emit('blur', event)
}

const handleClear = () => {
  emit('update:modelValue', '')
  emit('clear')
}

const togglePassword = () => {
  showPassword.value = !showPassword.value
}
</script>

<template>
  <div
    class="base-input"
    :class="[
      `base-input--${variant}`,
      `base-input--${size}`,
      {
        'base-input--focused': isFocused,
        'base-input--error': error,
        'base-input--success': success,
        'base-input--disabled': disabled,
        'base-input--has-icon': icon,
        'base-input--has-icon-right': iconRight || type === 'password' || clearable,
        'base-input--block': block,
        'base-input--floating': floatingLabel,
        'base-input--floating-active': isLabelFloated,
        'base-input--focus-ring-expand': focusRingExpand
      }
    ]"
  >
    <!-- Standard Label (non-floating) -->
    <label v-if="label && !floatingLabel" class="base-input__label">
      {{ label }}
      <span v-if="required" class="base-input__required">*</span>
    </label>

    <!-- Input Wrapper -->
    <div class="base-input__wrapper">
      <!-- Floating Label -->
      <label
        v-if="label && floatingLabel"
        class="base-input__floating-label"
        :class="{ 'base-input__floating-label--active': isLabelFloated }"
      >
        {{ label }}
        <span v-if="required" class="base-input__required">*</span>
      </label>
      <!-- Left Icon -->
      <i v-if="icon" :class="icon" class="base-input__icon base-input__icon--left" />

      <!-- Textarea -->
      <textarea
        v-if="multiline"
        v-model="inputValue"
        class="base-input__field base-input__field--textarea"
        :placeholder="placeholder"
        :disabled="disabled"
        :readonly="readonly"
        :required="required"
        :rows="rows"
        v-bind="attrs"
        @focus="handleFocus"
        @blur="handleBlur"
      />

      <!-- Input -->
      <input
        v-else
        v-model="inputValue"
        :type="computedType"
        class="base-input__field"
        :placeholder="placeholder"
        :disabled="disabled"
        :readonly="readonly"
        :required="required"
        v-bind="attrs"
        @focus="handleFocus"
        @blur="handleBlur"
      />

      <!-- Clear Button -->
      <button
        v-if="showClearButton"
        type="button"
        class="base-input__action"
        tabindex="-1"
        @click="handleClear"
      >
        <i class="pi pi-times" />
      </button>

      <!-- Password Toggle -->
      <button
        v-else-if="type === 'password'"
        type="button"
        class="base-input__action"
        tabindex="-1"
        @click="togglePassword"
      >
        <i :class="showPassword ? 'pi pi-eye-slash' : 'pi pi-eye'" />
      </button>

      <!-- Right Icon -->
      <i
        v-else-if="iconRight"
        :class="iconRight"
        class="base-input__icon base-input__icon--right"
      />

      <!-- Search Icon (for search type) -->
      <i
        v-else-if="type === 'search' && !icon"
        class="pi pi-search base-input__icon base-input__icon--right"
      />
    </div>

    <!-- Helper/Error Text -->
    <span v-if="error" class="base-input__helper base-input__helper--error">
      <i class="pi pi-exclamation-circle" />
      {{ error }}
    </span>
    <span v-else-if="helperText" class="base-input__helper">
      {{ helperText }}
    </span>
  </div>
</template>

<style scoped lang="scss">
// Note: Tokens available via global injection from vite.config.ts
@use '@/design-system/mixins' as *;

.base-input {
  display: flex;
  flex-direction: column;
  gap: $spacing-1;

  &--block {
    width: 100%;
  }

  // Label
  &__label {
    font-size: $font-size-sm;
    font-weight: $font-weight-medium;
    color: $slate-700;
    margin-bottom: $spacing-1;
  }

  &__required {
    color: $error-500;
    margin-inline-start: $spacing-0-5;
  }

  // Wrapper
  &__wrapper {
    position: relative;
    display: flex;
    align-items: center;
  }

  // Field
  &__field {
    width: 100%;
    font-family: inherit;
    font-size: $font-size-base;
    line-height: 1.5;
    color: $slate-900;
    background: white;
    border: 1.5px solid $slate-200;
    border-radius: $radius-lg;
    transition:
      border-color $duration-fast $ease-default,
      box-shadow $duration-fast $ease-default,
      background $duration-fast $ease-default;

    &::placeholder {
      color: $slate-400;
    }

    &:hover:not(:disabled):not(:focus) {
      border-color: $slate-300;
    }

    &:focus {
      outline: none;
      border-color: $brand-500;
      box-shadow: $shadow-focus-ring;
    }

    &:disabled {
      background: $slate-50;
      color: $slate-500;
      cursor: not-allowed;
    }

    &--textarea {
      resize: vertical;
      min-height: 80px;
    }
  }

  // Size variants
  &--sm {
    .base-input__field {
      font-size: $font-size-sm;
      padding: $spacing-2 $spacing-3;
    }

    &.base-input--has-icon .base-input__field {
      padding-inline-start: $spacing-8;
    }

    &.base-input--has-icon-right .base-input__field {
      padding-inline-end: $spacing-8;
    }

    .base-input__icon {
      font-size: $font-size-sm;
    }
  }

  &--md {
    .base-input__field {
      padding: $spacing-3 $spacing-4;
    }

    &.base-input--has-icon .base-input__field {
      padding-inline-start: $spacing-10;
    }

    &.base-input--has-icon-right .base-input__field {
      padding-inline-end: $spacing-10;
    }
  }

  &--lg {
    .base-input__field {
      font-size: $font-size-lg;
      padding: $spacing-4 $spacing-5;
    }

    &.base-input--has-icon .base-input__field {
      padding-inline-start: $spacing-12;
    }

    &.base-input--has-icon-right .base-input__field {
      padding-inline-end: $spacing-12;
    }

    .base-input__icon {
      font-size: $font-size-lg;
    }
  }

  // Style variants
  &--elevated {
    .base-input__field {
      background: $slate-50;
      border-color: transparent;
      box-shadow: inset 0 1px 2px rgba($slate-900, 0.05);

      &:focus {
        background: white;
        border-color: $brand-500;
        box-shadow: $shadow-focus-ring;
      }
    }
  }

  &--filled {
    .base-input__field {
      background: $slate-100;
      border-color: transparent;

      &:hover:not(:disabled):not(:focus) {
        background: $slate-200;
      }

      &:focus {
        background: white;
        border-color: $brand-500;
      }
    }
  }

  &--ghost {
    .base-input__field {
      background: transparent;
      border-color: transparent;

      &:hover:not(:disabled):not(:focus) {
        background: $slate-50;
      }

      &:focus {
        background: white;
        border-color: $brand-500;
      }
    }
  }

  // States
  &--error {
    .base-input__field {
      border-color: $error-500;

      &:focus {
        box-shadow: 0 0 0 3px rgba($error-500, 0.15);
      }
    }

    .base-input__icon {
      color: $error-500;
    }
  }

  &--success {
    .base-input__field {
      border-color: $success-500;

      &:focus {
        box-shadow: 0 0 0 3px rgba($success-500, 0.15);
      }
    }

    .base-input__icon {
      color: $success-500;
    }
  }

  &--disabled {
    opacity: 0.7;

    .base-input__label {
      color: $slate-500;
    }
  }

  // Icon
  &__icon {
    position: absolute;
    color: $slate-400;
    font-size: $font-size-base;
    pointer-events: none;
    transition: color $duration-fast $ease-default;

    &--left {
      inset-inline-start: $spacing-3;
    }

    &--right {
      inset-inline-end: $spacing-3;
    }
  }

  &--focused .base-input__icon {
    color: $brand-500;
  }

  // Action buttons (clear, password toggle)
  &__action {
    position: absolute;
    inset-inline-end: $spacing-2;
    display: flex;
    align-items: center;
    justify-content: center;
    width: $spacing-7;
    height: $spacing-7;
    background: transparent;
    border: none;
    border-radius: $radius-md;
    color: $slate-400;
    cursor: pointer;
    transition:
      background $duration-fast $ease-default,
      color $duration-fast $ease-default;

    &:hover {
      background: $slate-100;
      color: $slate-600;
    }

    i {
      font-size: $font-size-sm;
    }
  }

  // Helper text
  &__helper {
    display: flex;
    align-items: center;
    gap: $spacing-1;
    font-size: $font-size-xs;
    color: $slate-500;
    margin-top: $spacing-1;

    &--error {
      color: $error-600;

      i {
        font-size: 0.75rem;
      }
    }
  }

  // Floating label styles
  &__floating-label {
    position: absolute;
    left: $spacing-4;
    top: 50%;
    transform: translateY(-50%);
    font-size: $font-size-base;
    color: $slate-400;
    pointer-events: none;
    transition:
      top $duration-normal $ease-spring,
      transform $duration-normal $ease-spring,
      font-size $duration-normal $ease-spring,
      color $duration-normal $ease-spring,
      background $duration-normal $ease-spring,
      padding $duration-normal $ease-spring;
    z-index: 1;
    background: transparent;
    padding: 0;

    [dir='rtl'] & {
      left: auto;
      right: $spacing-4;
    }

    &--active {
      top: 0;
      transform: translateY(-50%);
      font-size: $font-size-xs;
      color: $brand-600;
      background: white;
      padding: 0 $spacing-1;
    }
  }

  // Floating label mode
  &--floating {
    .base-input__field {
      padding-top: $spacing-5;
      padding-bottom: $spacing-2;
    }

    .base-input__wrapper {
      position: relative;
    }

    &.base-input--has-icon {
      .base-input__floating-label {
        left: $spacing-10;

        [dir='rtl'] & {
          left: auto;
          right: $spacing-10;
        }
      }
    }
  }

  &--floating-active {
    .base-input__floating-label {
      top: 0;
      transform: translateY(-50%);
      font-size: $font-size-xs;
      color: $brand-600;
      background: white;
      padding: 0 $spacing-1;
    }
  }

  // Focus ring expand effect
  &--focus-ring-expand {
    .base-input__field {
      &:focus {
        animation: focusRingExpand $duration-normal $ease-spring;
      }
    }
  }

  // Error state for floating label
  &--error {
    .base-input__floating-label--active {
      color: $error-600;
    }
  }

  // Success state for floating label
  &--success {
    .base-input__floating-label--active {
      color: $success-600;
    }
  }
}

// Focus ring expand keyframes
@keyframes focusRingExpand {
  0% {
    box-shadow: 0 0 0 0 rgba($brand-500, 0.4);
  }
  50% {
    box-shadow: 0 0 0 6px rgba($brand-500, 0.2);
  }
  100% {
    box-shadow: $shadow-focus-ring;
  }
}

// Reduced motion support
@media (prefers-reduced-motion: reduce) {
  .base-input {
    &__floating-label {
      transition: none !important;
    }

    &--focus-ring-expand .base-input__field:focus {
      animation: none !important;
    }
  }
}
</style>
