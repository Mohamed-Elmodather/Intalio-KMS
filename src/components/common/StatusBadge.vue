<script setup lang="ts">
import { computed } from 'vue'

type StatusType =
  | 'featured'
  | 'new'
  | 'hot'
  | 'trending'
  | 'popular'
  | 'recommended'
  | 'ai-powered'
  | 'virtual'
  | 'in-person'
  | 'draft'
  | 'published'
  | 'completed'
  | 'in-progress'
  | 'pending'

interface Props {
  status: StatusType | string
  size?: 'xs' | 'sm' | 'md' | 'lg'
  variant?: 'solid' | 'gradient' | 'outline' | 'glass'
  showIcon?: boolean
  label?: string
}

const props = withDefaults(defineProps<Props>(), {
  size: 'sm',
  variant: 'solid',
  showIcon: true
})

// Status configurations
const statusConfig: Record<string, {
  icon: string
  bg: string
  text: string
  gradient?: string
  border?: string
}> = {
  'featured': {
    icon: 'fas fa-star',
    bg: '#f59e0b',
    text: '#ffffff',
    gradient: 'linear-gradient(135deg, #fbbf24 0%, #f59e0b 100%)'
  },
  'new': {
    icon: 'fas fa-sparkles',
    bg: '#14b8a6',
    text: '#ffffff',
    gradient: 'linear-gradient(135deg, #2dd4bf 0%, #14b8a6 100%)'
  },
  'hot': {
    icon: 'fas fa-fire',
    bg: '#ef4444',
    text: '#ffffff',
    gradient: 'linear-gradient(135deg, #f87171 0%, #ef4444 100%)'
  },
  'trending': {
    icon: 'fas fa-arrow-trend-up',
    bg: '#8b5cf6',
    text: '#ffffff',
    gradient: 'linear-gradient(135deg, #a78bfa 0%, #8b5cf6 100%)'
  },
  'popular': {
    icon: 'fas fa-fire-flame-curved',
    bg: '#f97316',
    text: '#ffffff',
    gradient: 'linear-gradient(135deg, #fb923c 0%, #f97316 100%)'
  },
  'recommended': {
    icon: 'fas fa-thumbs-up',
    bg: '#3b82f6',
    text: '#ffffff',
    gradient: 'linear-gradient(135deg, #60a5fa 0%, #3b82f6 100%)'
  },
  'ai-powered': {
    icon: 'fas fa-wand-magic-sparkles',
    bg: '#8b5cf6',
    text: '#ffffff',
    gradient: 'linear-gradient(135deg, #a78bfa 0%, #8b5cf6 100%)'
  },
  'virtual': {
    icon: 'fas fa-video',
    bg: '#6366f1',
    text: '#ffffff',
    gradient: 'linear-gradient(135deg, #818cf8 0%, #6366f1 100%)'
  },
  'in-person': {
    icon: 'fas fa-location-dot',
    bg: '#10b981',
    text: '#ffffff',
    gradient: 'linear-gradient(135deg, #34d399 0%, #10b981 100%)'
  },
  'draft': {
    icon: 'fas fa-file-pen',
    bg: '#f1f5f9',
    text: '#64748b',
    border: '#e2e8f0'
  },
  'published': {
    icon: 'fas fa-check-circle',
    bg: '#dcfce7',
    text: '#16a34a',
    border: '#bbf7d0'
  },
  'completed': {
    icon: 'fas fa-circle-check',
    bg: '#dcfce7',
    text: '#16a34a',
    border: '#bbf7d0'
  },
  'in-progress': {
    icon: 'fas fa-spinner',
    bg: '#dbeafe',
    text: '#2563eb',
    border: '#bfdbfe'
  },
  'pending': {
    icon: 'fas fa-clock',
    bg: '#fef3c7',
    text: '#d97706',
    border: '#fde68a'
  }
}

const config = computed(() => {
  return statusConfig[props.status] || {
    icon: 'fas fa-tag',
    bg: '#f1f5f9',
    text: '#64748b',
    border: '#e2e8f0'
  }
})

const displayLabel = computed(() => {
  if (props.label) return props.label
  // Convert status to display label
  return props.status.split('-').map(word =>
    word.charAt(0).toUpperCase() + word.slice(1)
  ).join(' ')
})

const sizeClasses = computed(() => {
  switch (props.size) {
    case 'xs':
      return 'px-1.5 py-0.5 text-[9px] gap-0.5'
    case 'md':
      return 'px-2.5 py-1 text-[11px] gap-1.5'
    case 'lg':
      return 'px-3 py-1.5 text-xs gap-1.5'
    case 'sm':
    default:
      return 'px-2 py-0.5 text-[10px] gap-1'
  }
})

const variantStyles = computed(() => {
  const c = config.value
  switch (props.variant) {
    case 'gradient':
      return {
        background: c.gradient || c.bg,
        color: c.text,
        border: 'none',
        boxShadow: '0 2px 4px rgba(0,0,0,0.1)'
      }
    case 'outline':
      return {
        background: 'transparent',
        color: c.bg,
        border: `1.5px solid ${c.bg}`
      }
    case 'glass':
      return {
        background: `${c.bg}20`,
        color: c.text === '#ffffff' ? c.bg : c.text,
        border: `1px solid ${c.bg}30`,
        backdropFilter: 'blur(4px)'
      }
    case 'solid':
    default:
      return {
        background: c.bg,
        color: c.text,
        border: c.border ? `1px solid ${c.border}` : 'none'
      }
  }
})
</script>

<template>
  <span
    class="status-badge"
    :class="sizeClasses"
    :style="variantStyles"
  >
    <i v-if="showIcon" :class="[config.icon, 'text-[0.8em]']"></i>
    <span>{{ displayLabel }}</span>
  </span>
</template>

<style scoped>
.status-badge {
  display: inline-flex;
  align-items: center;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.03em;
  border-radius: 9999px;
  white-space: nowrap;
  transition: all 0.2s ease;
}

.status-badge:hover {
  transform: translateY(-1px);
  filter: brightness(1.05);
}
</style>
