<script setup lang="ts">
/**
 * SearchInput Component
 *
 * A search input with icon, focus states, and keyboard shortcut hint.
 * Designed for consistent search experiences across the application.
 *
 * @example
 * <SearchInput
 *   v-model="searchQuery"
 *   placeholder="Search documents..."
 *   @search="handleSearch"
 * />
 */
import { ref, computed, watch } from 'vue'
import { useI18n } from 'vue-i18n'

export interface SearchInputProps {
  /** v-model value */
  modelValue?: string
  /** Placeholder text */
  placeholder?: string
  /** Show keyboard shortcut hint */
  showShortcut?: boolean
  /** Shortcut key display (e.g., '⌘K') */
  shortcutKey?: string
  /** Debounce delay in ms */
  debounce?: number
  /** Enable clearable button */
  clearable?: boolean
  /** Disabled state */
  disabled?: boolean
  /** Size variant */
  size?: 'sm' | 'md' | 'lg'
  /** Full width */
  block?: boolean
}

const props = withDefaults(defineProps<SearchInputProps>(), {
  modelValue: '',
  placeholder: '',
  showShortcut: true,
  shortcutKey: '⌘K',
  debounce: 0,
  clearable: true,
  disabled: false,
  size: 'md',
  block: false
})

const emit = defineEmits<{
  'update:modelValue': [value: string]
  search: [value: string]
  focus: [event: FocusEvent]
  blur: [event: FocusEvent]
  clear: []
}>()

const { t } = useI18n()
const inputRef = ref<HTMLInputElement | null>(null)
const isFocused = ref(false)
const localValue = ref(props.modelValue)

// Sync local value with prop
watch(() => props.modelValue, (newValue) => {
  localValue.value = newValue
})

// Debounced emit
let debounceTimer: ReturnType<typeof setTimeout> | null = null

function handleInput(event: Event) {
  const target = event.target as HTMLInputElement
  localValue.value = target.value

  if (debounceTimer) {
    clearTimeout(debounceTimer)
  }

  if (props.debounce > 0) {
    debounceTimer = setTimeout(() => {
      emit('update:modelValue', localValue.value)
    }, props.debounce)
  } else {
    emit('update:modelValue', localValue.value)
  }
}

function handleFocus(event: FocusEvent) {
  isFocused.value = true
  emit('focus', event)
}

function handleBlur(event: FocusEvent) {
  isFocused.value = false
  emit('blur', event)
}

function handleKeydown(event: KeyboardEvent) {
  if (event.key === 'Enter') {
    emit('search', localValue.value)
  }
  if (event.key === 'Escape') {
    inputRef.value?.blur()
  }
}

function handleClear() {
  localValue.value = ''
  emit('update:modelValue', '')
  emit('clear')
  inputRef.value?.focus()
}

const hasValue = computed(() => localValue.value.length > 0)

const placeholderText = computed(() => {
  return props.placeholder || t('common.search', 'Search...')
})

// Expose focus method
defineExpose({
  focus: () => inputRef.value?.focus(),
  blur: () => inputRef.value?.blur()
})
</script>

<template>
  <div
    class="search-input"
    :class="[
      `search-input--${size}`,
      {
        'search-input--focused': isFocused,
        'search-input--has-value': hasValue,
        'search-input--disabled': disabled,
        'search-input--block': block
      }
    ]"
  >
    <!-- Search Icon -->
    <div class="search-input__icon">
      <i class="pi pi-search"></i>
    </div>

    <!-- Input -->
    <input
      ref="inputRef"
      type="text"
      class="search-input__field"
      :value="localValue"
      :placeholder="placeholderText"
      :disabled="disabled"
      @input="handleInput"
      @focus="handleFocus"
      @blur="handleBlur"
      @keydown="handleKeydown"
    />

    <!-- Clear Button -->
    <button
      v-if="clearable && hasValue && !disabled"
      type="button"
      class="search-input__clear"
      @click="handleClear"
      tabindex="-1"
    >
      <i class="pi pi-times"></i>
    </button>

    <!-- Keyboard Shortcut -->
    <kbd
      v-if="showShortcut && !isFocused && !hasValue"
      class="search-input__shortcut"
    >
      {{ shortcutKey }}
    </kbd>
  </div>
</template>

<style scoped lang="scss">
// Note: Tokens available via global injection from vite.config.ts
@use '@/design-system/mixins' as *;

.search-input {
  position: relative;
  display: inline-flex;
  align-items: center;
  max-width: 400px;
  width: 100%;

  // Block
  &--block {
    max-width: none;
  }

  // Focused state
  &--focused {
    .search-input__field {
      background: $color-bg-primary;
      border-color: $color-brand-primary;
      box-shadow: $shadow-focus-ring;
    }

    .search-input__icon {
      color: $color-brand-primary;
    }
  }

  // Disabled state
  &--disabled {
    opacity: 0.6;
    pointer-events: none;

    .search-input__field {
      cursor: not-allowed;
    }
  }

  // Icon
  &__icon {
    position: absolute;
    inset-inline-start: $spacing-4;
    top: 50%;
    transform: translateY(-50%);
    display: flex;
    align-items: center;
    justify-content: center;
    color: $color-text-muted;
    pointer-events: none;
    transition: color $duration-fast $ease-default;
    z-index: 1;

    i {
      font-size: 1rem;
    }
  }

  // Input field
  &__field {
    width: 100%;
    padding: $spacing-3 $spacing-4;
    padding-inline-start: calc($spacing-4 + 1.5rem);
    padding-inline-end: $spacing-10;
    font-family: inherit;
    font-size: $font-size-sm;
    color: $color-text-primary;
    background: $color-bg-tertiary;
    border: 1.5px solid transparent;
    border-radius: $radius-xl;
    outline: none;
    transition:
      background-color $duration-fast $ease-default,
      border-color $duration-fast $ease-default,
      box-shadow $duration-fast $ease-default;

    &::placeholder {
      color: $color-text-muted;
    }

    &:hover:not(:focus):not(:disabled) {
      background: $color-border-default;
    }
  }

  // Clear button
  &__clear {
    position: absolute;
    inset-inline-end: $spacing-3;
    top: 50%;
    transform: translateY(-50%);
    display: flex;
    align-items: center;
    justify-content: center;
    width: 24px;
    height: 24px;
    padding: 0;
    background: $color-bg-tertiary;
    border: none;
    border-radius: $radius-full;
    color: $color-text-muted;
    cursor: pointer;
    transition:
      background-color $duration-fast $ease-default,
      color $duration-fast $ease-default;

    &:hover {
      background: $color-border-default;
      color: $color-text-secondary;
    }

    i {
      font-size: 0.75rem;
    }
  }

  // Keyboard shortcut hint
  &__shortcut {
    position: absolute;
    inset-inline-end: $spacing-3;
    top: 50%;
    transform: translateY(-50%);
    padding: $spacing-1 $spacing-2;
    font-family: inherit;
    font-size: $font-size-xs;
    color: $color-text-muted;
    background: $color-bg-primary;
    border: 1px solid $color-border-default;
    border-radius: $radius-sm;
    transition: opacity $duration-fast $ease-default;
  }

  // Size variants
  &--sm {
    .search-input__field {
      padding: $spacing-2 $spacing-3;
      padding-inline-start: calc($spacing-3 + 1.25rem);
      padding-inline-end: $spacing-8;
      font-size: $font-size-xs;
    }

    .search-input__icon {
      inset-inline-start: $spacing-3;

      i {
        font-size: 0.875rem;
      }
    }

    .search-input__shortcut {
      inset-inline-end: $spacing-2;
      font-size: 10px;
      padding: 2px $spacing-1;
    }

    .search-input__clear {
      inset-inline-end: $spacing-2;
      width: 20px;
      height: 20px;

      i {
        font-size: 0.625rem;
      }
    }
  }

  &--lg {
    .search-input__field {
      padding: $spacing-4 $spacing-5;
      padding-inline-start: calc($spacing-5 + 1.75rem);
      padding-inline-end: $spacing-12;
      font-size: $font-size-base;
    }

    .search-input__icon {
      inset-inline-start: $spacing-5;

      i {
        font-size: 1.125rem;
      }
    }

    .search-input__shortcut {
      inset-inline-end: $spacing-4;
    }

    .search-input__clear {
      inset-inline-end: $spacing-4;
      width: 28px;
      height: 28px;

      i {
        font-size: 0.875rem;
      }
    }
  }
}
</style>
