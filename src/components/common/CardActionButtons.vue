<script setup lang="ts">
import { computed } from 'vue'

type ActionType = 'like' | 'bookmark' | 'save' | 'share' | 'download' | 'star' | 'compare' | 'ai-analyze' | 'calendar' | 'reminder'

interface ActionConfig {
  type: ActionType
  active?: boolean
  disabled?: boolean
  tooltip?: string
  hidden?: boolean
}

interface Props {
  actions: ActionConfig[]
  size?: 'xs' | 'sm' | 'md' | 'lg'
  variant?: 'circular' | 'rounded' | 'square'
  layout?: 'horizontal' | 'vertical'
  showOnHover?: boolean
  backdrop?: boolean
  position?: 'top-left' | 'top-right' | 'bottom-left' | 'bottom-right'
}

const props = withDefaults(defineProps<Props>(), {
  size: 'sm',
  variant: 'rounded',
  layout: 'horizontal',
  showOnHover: true,
  backdrop: true,
  position: 'top-right'
})

const emit = defineEmits<{
  action: [type: ActionType, event: MouseEvent]
}>()

// Action configurations with icons and colors
const actionConfigs: Record<ActionType, {
  icon: string
  activeIcon: string
  activeClass: string
  hoverClass: string
  label: string
}> = {
  'like': {
    icon: 'far fa-heart',
    activeIcon: 'fas fa-heart',
    activeClass: 'bg-red-50 text-red-500 border-red-200',
    hoverClass: 'hover:bg-red-500 hover:text-white hover:border-red-500',
    label: 'Like'
  },
  'bookmark': {
    icon: 'far fa-bookmark',
    activeIcon: 'fas fa-bookmark',
    activeClass: 'bg-teal-500 text-white border-teal-500',
    hoverClass: 'hover:bg-teal-500 hover:text-white hover:border-teal-500',
    label: 'Bookmark'
  },
  'save': {
    icon: 'far fa-bookmark',
    activeIcon: 'fas fa-bookmark',
    activeClass: 'bg-amber-100 text-amber-600 border-amber-200',
    hoverClass: 'hover:bg-amber-500 hover:text-white hover:border-amber-500',
    label: 'Save'
  },
  'share': {
    icon: 'fas fa-share-alt',
    activeIcon: 'fas fa-share-alt',
    activeClass: 'bg-blue-500 text-white border-blue-500',
    hoverClass: 'hover:bg-blue-500 hover:text-white hover:border-blue-500',
    label: 'Share'
  },
  'download': {
    icon: 'fas fa-download',
    activeIcon: 'fas fa-download',
    activeClass: 'bg-teal-500 text-white border-teal-500',
    hoverClass: 'hover:bg-teal-500 hover:text-white hover:border-teal-500',
    label: 'Download'
  },
  'star': {
    icon: 'far fa-star',
    activeIcon: 'fas fa-star',
    activeClass: 'bg-amber-400 text-white border-amber-400',
    hoverClass: 'hover:bg-amber-400 hover:text-white hover:border-amber-400',
    label: 'Star'
  },
  'compare': {
    icon: 'fas fa-code-compare',
    activeIcon: 'fas fa-code-compare',
    activeClass: 'bg-purple-500 text-white border-purple-500',
    hoverClass: 'hover:bg-purple-500 hover:text-white hover:border-purple-500',
    label: 'Compare'
  },
  'ai-analyze': {
    icon: 'fas fa-wand-magic-sparkles',
    activeIcon: 'fas fa-wand-magic-sparkles',
    activeClass: 'bg-violet-500 text-white border-violet-500',
    hoverClass: 'hover:bg-violet-500 hover:text-white hover:border-violet-500',
    label: 'AI Analyze'
  },
  'calendar': {
    icon: 'fas fa-calendar-plus',
    activeIcon: 'fas fa-calendar-check',
    activeClass: 'bg-green-500 text-white border-green-500',
    hoverClass: 'hover:bg-green-500 hover:text-white hover:border-green-500',
    label: 'Add to Calendar'
  },
  'reminder': {
    icon: 'far fa-bell',
    activeIcon: 'fas fa-bell',
    activeClass: 'bg-orange-500 text-white border-orange-500',
    hoverClass: 'hover:bg-orange-500 hover:text-white hover:border-orange-500',
    label: 'Set Reminder'
  }
}

const sizeClasses = computed(() => {
  switch (props.size) {
    case 'xs': return 'w-6 h-6 text-[10px]'
    case 'sm': return 'w-7 h-7 text-xs'
    case 'md': return 'w-8 h-8 text-xs'
    case 'lg': return 'w-10 h-10 text-sm'
    default: return 'w-7 h-7 text-xs'
  }
})

const variantClasses = computed(() => {
  switch (props.variant) {
    case 'circular': return 'rounded-full'
    case 'square': return 'rounded-md'
    case 'rounded':
    default: return 'rounded-lg'
  }
})

const layoutClasses = computed(() => {
  return props.layout === 'vertical' ? 'flex-col' : 'flex-row'
})

const positionClasses = computed(() => {
  switch (props.position) {
    case 'top-left': return 'top-2 left-2'
    case 'top-right': return 'top-2 right-2'
    case 'bottom-left': return 'bottom-2 left-2'
    case 'bottom-right': return 'bottom-2 right-2'
    default: return 'top-2 right-2'
  }
})

const visibleActions = computed(() => {
  return props.actions.filter(action => !action.hidden)
})

function getActionConfig(type: ActionType) {
  return actionConfigs[type] || actionConfigs['share']
}

function getIcon(action: ActionConfig) {
  const config = getActionConfig(action.type)
  return action.active ? config.activeIcon : config.icon
}

function getButtonClasses(action: ActionConfig) {
  const config = getActionConfig(action.type)
  const baseClasses = [
    'flex items-center justify-center',
    'border transition-all duration-200',
    'cursor-pointer disabled:opacity-50 disabled:cursor-not-allowed',
    sizeClasses.value,
    variantClasses.value
  ]

  if (props.backdrop) {
    baseClasses.push('backdrop-blur-sm shadow-sm')
  }

  if (action.active) {
    baseClasses.push(config.activeClass)
  } else {
    baseClasses.push('bg-white/95 text-gray-500 border-gray-200/50')
    baseClasses.push(config.hoverClass)
  }

  return baseClasses.join(' ')
}

function handleAction(action: ActionConfig, event: MouseEvent) {
  event.stopPropagation()
  if (!action.disabled) {
    emit('action', action.type, event)
  }
}
</script>

<template>
  <div
    class="card-action-buttons absolute flex gap-1.5 z-20"
    :class="[
      layoutClasses,
      positionClasses,
      showOnHover ? 'opacity-0 group-hover:opacity-100 transition-opacity duration-200' : ''
    ]"
  >
    <button
      v-for="action in visibleActions"
      :key="action.type"
      :class="getButtonClasses(action)"
      :disabled="action.disabled"
      :title="action.tooltip || getActionConfig(action.type).label"
      @click="handleAction(action, $event)"
    >
      <i :class="getIcon(action)"></i>
    </button>
  </div>
</template>

<style scoped>
.card-action-buttons button:hover:not(:disabled) {
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.card-action-buttons button:active:not(:disabled) {
  transform: translateY(0);
}
</style>
