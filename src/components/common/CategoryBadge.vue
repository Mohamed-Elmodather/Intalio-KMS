<script setup lang="ts">
import { computed } from 'vue'

interface Props {
  category: string
  categoryId?: string
  icon?: string
  size?: 'xs' | 'sm' | 'md'
  variant?: 'solid' | 'outline' | 'gradient'
  color?: string
}

const props = withDefaults(defineProps<Props>(), {
  size: 'sm',
  variant: 'solid'
})

// Predefined category colors
const categoryColors: Record<string, { bg: string; text: string; border: string }> = {
  // Articles categories
  'getting-started': { bg: '#ccfbf1', text: '#0d9488', border: '#99f6e4' },
  'tutorials': { bg: '#ede9fe', text: '#7c3aed', border: '#ddd6fe' },
  'best-practices': { bg: '#fef3c7', text: '#d97706', border: '#fde68a' },
  'policies': { bg: '#e0e7ff', text: '#4f46e5', border: '#c7d2fe' },
  'tech': { bg: '#dbeafe', text: '#2563eb', border: '#bfdbfe' },
  'technology': { bg: '#dbeafe', text: '#2563eb', border: '#bfdbfe' },
  'hr': { bg: '#fce7f3', text: '#db2777', border: '#fbcfe8' },
  'security': { bg: '#fee2e2', text: '#dc2626', border: '#fecaca' },
  // Events categories
  'meeting': { bg: '#dbeafe', text: '#1e40af', border: '#bfdbfe' },
  'training': { bg: '#d1fae5', text: '#065f46', border: '#a7f3d0' },
  'social': { bg: '#fce7f3', text: '#9d174d', border: '#fbcfe8' },
  'review': { bg: '#fef3c7', text: '#92400e', border: '#fde68a' },
  'webinar': { bg: '#e0e7ff', text: '#3730a3', border: '#c7d2fe' },
  'workshop': { bg: '#d1fae5', text: '#065f46', border: '#a7f3d0' },
  'conference': { bg: '#fef3c7', text: '#d97706', border: '#fde68a' },
  // Media categories
  'videos': { bg: '#fee2e2', text: '#dc2626', border: '#fecaca' },
  'highlights': { bg: '#fef3c7', text: '#d97706', border: '#fde68a' },
  'interviews': { bg: '#dbeafe', text: '#2563eb', border: '#bfdbfe' },
  'behind': { bg: '#fce7f3', text: '#db2777', border: '#fbcfe8' },
  'galleries': { bg: '#e0e7ff', text: '#4f46e5', border: '#c7d2fe' },
  'teams': { bg: '#d1fae5', text: '#065f46', border: '#a7f3d0' },
  'venues': { bg: '#f3e8ff', text: '#7c3aed', border: '#e9d5ff' },
  'fans': { bg: '#fce7f3', text: '#ec4899', border: '#fbcfe8' },
  'matches': { bg: '#ccfbf1', text: '#0d9488', border: '#99f6e4' },
  // Document libraries
  'tournament-docs': { bg: '#dbeafe', text: '#2563eb', border: '#bfdbfe' },
  'media-assets': { bg: '#fce7f3', text: '#db2777', border: '#fbcfe8' },
  'venue-info': { bg: '#d1fae5', text: '#065f46', border: '#a7f3d0' },
  'team-resources': { bg: '#fef3c7', text: '#d97706', border: '#fde68a' },
  'operations': { bg: '#e0e7ff', text: '#4f46e5', border: '#c7d2fe' },
  'accreditation': { bg: '#f3e8ff', text: '#7c3aed', border: '#e9d5ff' },
  // Learning categories
  'development': { bg: '#dbeafe', text: '#2563eb', border: '#bfdbfe' },
  'design': { bg: '#fce7f3', text: '#db2777', border: '#fbcfe8' },
  'business': { bg: '#d1fae5', text: '#065f46', border: '#a7f3d0' },
  'marketing': { bg: '#fef3c7', text: '#d97706', border: '#fde68a' },
  // Default
  'default': { bg: '#f1f5f9', text: '#475569', border: '#e2e8f0' }
}

const normalizedCategoryId = computed(() => {
  if (props.categoryId) return props.categoryId.toLowerCase()
  return props.category.toLowerCase().replace(/\s+/g, '-').split(' ')[0]
})

const colors = computed(() => {
  if (props.color) {
    return {
      bg: `${props.color}15`,
      text: props.color,
      border: `${props.color}30`
    }
  }
  return categoryColors[normalizedCategoryId.value] || categoryColors['default']
})

const sizeClasses = computed(() => {
  switch (props.size) {
    case 'xs':
      return 'px-1.5 py-0.5 text-[9px] gap-1'
    case 'md':
      return 'px-3 py-1.5 text-xs gap-1.5'
    case 'sm':
    default:
      return 'px-2 py-0.5 text-[10px] gap-1'
  }
})

const variantStyles = computed(() => {
  switch (props.variant) {
    case 'outline':
      return {
        background: 'transparent',
        color: colors.value.text,
        border: `1px solid ${colors.value.border}`
      }
    case 'gradient':
      return {
        background: `linear-gradient(135deg, ${colors.value.bg} 0%, ${colors.value.border} 100%)`,
        color: colors.value.text,
        border: 'none'
      }
    case 'solid':
    default:
      return {
        background: colors.value.bg,
        color: colors.value.text,
        border: `1px solid ${colors.value.border}`
      }
  }
})
</script>

<template>
  <span
    class="category-badge"
    :class="sizeClasses"
    :style="variantStyles"
  >
    <i v-if="icon" :class="[icon, 'text-[0.6em]']"></i>
    <span>{{ category }}</span>
  </span>
</template>

<style scoped>
.category-badge {
  display: inline-flex;
  align-items: center;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.025em;
  border-radius: 0.375rem;
  white-space: nowrap;
  transition: all 0.2s ease;
}

.category-badge:hover {
  filter: brightness(0.95);
}
</style>
