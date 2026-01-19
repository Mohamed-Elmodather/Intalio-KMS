<script setup lang="ts">
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

interface Props {
  size?: 'sm' | 'md' | 'lg'
  variant?: 'spinner' | 'dots' | 'pulse' | 'shimmer'
  text?: string
  showText?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  size: 'md',
  variant: 'spinner',
  showText: true,
})

const sizeClasses = computed(() => {
  switch (props.size) {
    case 'sm': return 'w-4 h-4'
    case 'lg': return 'w-8 h-8'
    default: return 'w-6 h-6'
  }
})

const textSizeClass = computed(() => {
  switch (props.size) {
    case 'sm': return 'text-xs'
    case 'lg': return 'text-base'
    default: return 'text-sm'
  }
})
</script>

<template>
  <div class="ai-loading-indicator flex items-center gap-2">
    <!-- Spinner variant -->
    <div v-if="variant === 'spinner'" :class="[sizeClasses, 'ai-spinner']">
      <svg class="animate-spin" viewBox="0 0 24 24" fill="none">
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
    </div>

    <!-- Dots variant -->
    <div v-else-if="variant === 'dots'" class="flex items-center gap-1">
      <span
        v-for="i in 3"
        :key="i"
        class="ai-dot rounded-full bg-teal-500"
        :class="size === 'sm' ? 'w-1.5 h-1.5' : size === 'lg' ? 'w-3 h-3' : 'w-2 h-2'"
        :style="{ animationDelay: `${(i - 1) * 150}ms` }"
      />
    </div>

    <!-- Pulse variant -->
    <div v-else-if="variant === 'pulse'" class="relative">
      <div
        :class="[sizeClasses, 'rounded-full bg-teal-500/30 animate-ping absolute']"
      />
      <div
        :class="[sizeClasses, 'rounded-full bg-teal-500 relative']"
      >
        <i class="fas fa-wand-magic-sparkles text-white absolute inset-0 flex items-center justify-center" :class="size === 'sm' ? 'text-[8px]' : size === 'lg' ? 'text-sm' : 'text-xs'" />
      </div>
    </div>

    <!-- Shimmer variant -->
    <div v-else-if="variant === 'shimmer'" class="ai-shimmer rounded" :class="sizeClasses" />

    <!-- Text -->
    <span v-if="showText && text" :class="[textSizeClass, 'text-gray-500 font-medium']">
      {{ text }}
    </span>
    <span v-else-if="showText" :class="[textSizeClass, 'text-gray-500 font-medium']">
      {{ $t('ai.thinking') }}
    </span>
  </div>
</template>

<style scoped>
.ai-spinner {
  color: #14b8a6;
}

.ai-dot {
  animation: dotBounce 1.4s ease-in-out infinite both;
}

@keyframes dotBounce {
  0%, 80%, 100% {
    transform: scale(0.6);
    opacity: 0.5;
  }
  40% {
    transform: scale(1);
    opacity: 1;
  }
}

.ai-shimmer {
  background: linear-gradient(
    90deg,
    rgba(20, 184, 166, 0.1) 25%,
    rgba(20, 184, 166, 0.3) 50%,
    rgba(20, 184, 166, 0.1) 75%
  );
  background-size: 200% 100%;
  animation: shimmer 1.5s infinite;
}

@keyframes shimmer {
  0% {
    background-position: 200% 0;
  }
  100% {
    background-position: -200% 0;
  }
}
</style>
