<script setup lang="ts">
import { computed } from 'vue'

interface Props {
  label?: string
  icon?: string
  loading?: boolean
  disabled?: boolean
  variant?: 'primary' | 'secondary' | 'ghost' | 'outline'
  size?: 'sm' | 'md' | 'lg'
  showSparkle?: boolean
  tooltip?: string
}

const props = withDefaults(defineProps<Props>(), {
  icon: 'fas fa-wand-magic-sparkles',
  loading: false,
  disabled: false,
  variant: 'primary',
  size: 'md',
  showSparkle: true,
})

const emit = defineEmits<{
  click: []
}>()

const variantClasses = computed(() => {
  switch (props.variant) {
    case 'primary':
      return 'bg-gradient-to-r from-teal-500 to-teal-600 text-white hover:from-teal-600 hover:to-teal-700 shadow-sm hover:shadow'
    case 'secondary':
      return 'bg-teal-50 text-teal-700 hover:bg-teal-100 border border-teal-200'
    case 'outline':
      return 'bg-white text-teal-600 border border-teal-300 hover:bg-teal-50'
    case 'ghost':
      return 'bg-transparent text-teal-600 hover:bg-teal-50'
    default:
      return ''
  }
})

const sizeClasses = computed(() => {
  switch (props.size) {
    case 'sm':
      return {
        button: 'px-2.5 py-1.5 text-xs gap-1.5',
        icon: 'text-xs',
      }
    case 'lg':
      return {
        button: 'px-5 py-3 text-base gap-2.5',
        icon: 'text-base',
      }
    default:
      return {
        button: 'px-3.5 py-2 text-sm gap-2',
        icon: 'text-sm',
      }
  }
})

function handleClick() {
  if (!props.disabled && !props.loading) {
    emit('click')
  }
}
</script>

<template>
  <button
    type="button"
    class="ai-action-button inline-flex items-center justify-center font-medium rounded-lg transition-all duration-200"
    :class="[
      variantClasses,
      sizeClasses.button,
      (disabled || loading) ? 'opacity-60 cursor-not-allowed' : 'cursor-pointer',
    ]"
    :disabled="disabled || loading"
    :title="tooltip"
    @click="handleClick"
  >
    <!-- Loading spinner -->
    <svg
      v-if="loading"
      class="animate-spin"
      :class="sizeClasses.icon"
      viewBox="0 0 24 24"
      fill="none"
    >
      <circle
        class="opacity-25"
        cx="12"
        cy="12"
        r="10"
        stroke="currentColor"
        stroke-width="3"
      />
      <path
        class="opacity-75"
        fill="currentColor"
        d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
      />
    </svg>

    <!-- Icon -->
    <i v-else-if="showSparkle || icon" :class="[icon, sizeClasses.icon, 'ai-sparkle-icon']" />

    <!-- Label -->
    <span v-if="label">{{ label }}</span>
    <slot v-else />
  </button>
</template>

<style scoped>
.ai-action-button:not(:disabled):hover .ai-sparkle-icon {
  animation: sparkle 0.6s ease-in-out;
}

@keyframes sparkle {
  0%, 100% {
    transform: scale(1) rotate(0deg);
  }
  25% {
    transform: scale(1.2) rotate(-10deg);
  }
  50% {
    transform: scale(1.1) rotate(10deg);
  }
  75% {
    transform: scale(1.15) rotate(-5deg);
  }
}

.ai-action-button:not(:disabled):hover {
  transform: translateY(-1px);
}

.ai-action-button:not(:disabled):active {
  transform: translateY(0);
}
</style>
