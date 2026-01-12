<script setup lang="ts">
import { computed } from 'vue'

interface Props {
  label: string
  confidence?: number
  icon?: string
  variant?: 'default' | 'primary' | 'success' | 'warning'
  removable?: boolean
  selected?: boolean
  disabled?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  variant: 'default',
  removable: false,
  selected: false,
  disabled: false,
})

const emit = defineEmits<{
  click: []
  remove: []
}>()

const variantClasses = computed(() => {
  if (props.selected) {
    return 'bg-teal-100 text-teal-700 border-teal-300 ring-2 ring-teal-200'
  }
  switch (props.variant) {
    case 'primary':
      return 'bg-teal-50 text-teal-700 border-teal-200 hover:bg-teal-100'
    case 'success':
      return 'bg-green-50 text-green-700 border-green-200 hover:bg-green-100'
    case 'warning':
      return 'bg-amber-50 text-amber-700 border-amber-200 hover:bg-amber-100'
    default:
      return 'bg-gray-50 text-gray-700 border-gray-200 hover:bg-gray-100'
  }
})

function handleClick() {
  if (!props.disabled) {
    emit('click')
  }
}

function handleRemove(e: Event) {
  e.stopPropagation()
  if (!props.disabled) {
    emit('remove')
  }
}
</script>

<template>
  <button
    type="button"
    class="ai-suggestion-chip inline-flex items-center gap-1.5 px-3 py-1.5 rounded-full border text-sm font-medium transition-all duration-200"
    :class="[
      variantClasses,
      disabled ? 'opacity-50 cursor-not-allowed' : 'cursor-pointer',
    ]"
    :disabled="disabled"
    @click="handleClick"
  >
    <!-- AI Icon -->
    <i v-if="icon" :class="icon" class="text-xs" />
    <i v-else class="fas fa-wand-magic-sparkles text-xs opacity-70" />

    <!-- Label -->
    <span>{{ label }}</span>

    <!-- Confidence badge -->
    <span
      v-if="confidence !== undefined"
      class="ml-1 px-1.5 py-0.5 text-xs rounded bg-black/5"
    >
      {{ Math.round(confidence * 100) }}%
    </span>

    <!-- Remove button -->
    <button
      v-if="removable"
      type="button"
      class="ml-1 -mr-1 p-0.5 rounded-full hover:bg-black/10 transition-colors"
      @click="handleRemove"
    >
      <i class="fas fa-times text-xs" />
    </button>
  </button>
</template>

<style scoped>
.ai-suggestion-chip:hover:not(:disabled) {
  transform: translateY(-1px);
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.ai-suggestion-chip:active:not(:disabled) {
  transform: translateY(0);
}
</style>
