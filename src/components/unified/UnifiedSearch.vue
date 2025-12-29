<script setup lang="ts">
import { ref, computed } from 'vue'

// Props
interface Props {
  modelValue?: string
  placeholder?: string
  showAiButton?: boolean
  aiButtonText?: string
  showKeyboardShortcut?: boolean
  keyboardShortcut?: string
  size?: 'sm' | 'md' | 'lg'
  variant?: 'default' | 'minimal' | 'bordered'
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: '',
  placeholder: 'Search anything...',
  showAiButton: true,
  aiButtonText: 'Ask AI',
  showKeyboardShortcut: true,
  keyboardShortcut: '⌘K',
  size: 'md',
  variant: 'default'
})

// Emits
const emit = defineEmits<{
  'update:modelValue': [value: string]
  'search': [query: string]
  'ai-click': []
}>()

// Local state
const localQuery = ref(props.modelValue)
const isFocused = ref(false)

// Computed
const inputValue = computed({
  get: () => props.modelValue || localQuery.value,
  set: (value: string) => {
    localQuery.value = value
    emit('update:modelValue', value)
  }
})

const sizeClasses = computed(() => {
  switch (props.size) {
    case 'sm':
      return 'py-2 text-xs'
    case 'lg':
      return 'py-3.5 text-base'
    default:
      return 'py-2.5 text-sm'
  }
})

// Methods
function handleSearch() {
  if (inputValue.value.trim()) {
    emit('search', inputValue.value.trim())
  }
}

function handleAiClick() {
  emit('ai-click')
}

function handleFocus() {
  isFocused.value = true
}

function handleBlur() {
  isFocused.value = false
}
</script>

<template>
  <div class="unified-search" :class="[`unified-search--${variant}`, { 'unified-search--focused': isFocused }]">
    <div class="unified-search__glow"></div>
    <div class="unified-search__inner">
      <div class="unified-search__content">
        <!-- Search Icon -->
        <i class="fas fa-search unified-search__icon"></i>

        <!-- Input -->
        <input
          type="text"
          v-model="inputValue"
          :placeholder="placeholder"
          :class="['unified-search__input', sizeClasses]"
          @keyup.enter="handleSearch"
          @focus="handleFocus"
          @blur="handleBlur"
        >

        <!-- Actions -->
        <div class="unified-search__actions">
          <!-- Keyboard Shortcut -->
          <kbd v-if="showKeyboardShortcut" class="unified-search__kbd">
            {{ keyboardShortcut }}
          </kbd>

          <!-- Divider -->
          <div v-if="showAiButton && showKeyboardShortcut" class="unified-search__divider"></div>

          <!-- AI Button -->
          <button
            v-if="showAiButton"
            @click="handleAiClick"
            class="unified-search__ai-btn"
            type="button"
          >
            <i class="fas fa-wand-magic-sparkles"></i>
            <span>{{ aiButtonText }}</span>
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.unified-search {
  position: relative;
  width: 100%;
}

.unified-search__glow {
  position: absolute;
  inset: 0;
  background: linear-gradient(135deg, rgba(20, 184, 166, 0.15), rgba(13, 148, 136, 0.1));
  border-radius: 14px;
  opacity: 0;
  filter: blur(8px);
  transition: opacity 0.3s ease;
  pointer-events: none;
}

.unified-search--focused .unified-search__glow {
  opacity: 1;
}

.unified-search__inner {
  position: relative;
  background: #f9fafb;
  border: 1px solid #e5e7eb;
  border-radius: 14px;
  transition: all 0.3s ease;
}

.unified-search--focused .unified-search__inner {
  background: white;
  border-color: #14b8a6;
  box-shadow: 0 0 0 3px rgba(20, 184, 166, 0.1);
}

.unified-search--minimal .unified-search__inner {
  background: transparent;
  border: none;
}

.unified-search--bordered .unified-search__inner {
  background: white;
  border: 2px solid #e5e7eb;
}

.unified-search__content {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 0 16px;
}

.unified-search__icon {
  color: #9ca3af;
  transition: color 0.3s ease;
  flex-shrink: 0;
}

.unified-search--focused .unified-search__icon {
  color: #14b8a6;
}

.unified-search__input {
  flex: 1;
  background: transparent;
  color: #111827;
  outline: none;
  border: none;
  min-width: 0;
}

.unified-search__input::placeholder {
  color: #9ca3af;
}

.unified-search__actions {
  display: flex;
  align-items: center;
  gap: 8px;
  flex-shrink: 0;
}

.unified-search__kbd {
  display: none;
  align-items: center;
  gap: 4px;
  padding: 4px 8px;
  font-size: 11px;
  color: #9ca3af;
  background: #f3f4f6;
  border-radius: 6px;
  border: 1px solid #e5e7eb;
  font-family: inherit;
}

@media (min-width: 640px) {
  .unified-search__kbd {
    display: inline-flex;
  }
}

.unified-search__divider {
  width: 1px;
  height: 20px;
  background: #e5e7eb;
}

.unified-search__ai-btn {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 6px 12px;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
  border: none;
  border-radius: 8px;
  font-size: 12px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
  white-space: nowrap;
}

.unified-search__ai-btn:hover {
  transform: scale(1.02);
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.3);
}

.unified-search__ai-btn i {
  font-size: 11px;
}
</style>
