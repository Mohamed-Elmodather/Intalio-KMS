<script setup lang="ts">
import { computed } from 'vue'
import { useComparisonStore } from '@/stores/comparison'
import type { ComparisonItem, ComparisonItemType } from '@/types'

interface Props {
  item: {
    id: string | number
    title: string
    thumbnail?: string
    description?: string
    author?: string
    date?: string
    size?: number
    duration?: number
    category?: string
    tags?: string[]
    [key: string]: any
  }
  type: ComparisonItemType
  size?: 'sm' | 'md' | 'lg'
  variant?: 'overlay' | 'solid' | 'outline'
}

const props = withDefaults(defineProps<Props>(), {
  size: 'sm',
  variant: 'overlay'
})

const comparisonStore = useComparisonStore()

const itemId = computed(() => String(props.item.id))

const isInComparison = computed(() => comparisonStore.isItemSelected(itemId.value))

const buttonTitle = computed(() =>
  isInComparison.value ? 'Remove from Compare' : 'Add to Compare'
)

function toggleComparison() {
  if (isInComparison.value) {
    comparisonStore.removeItem(itemId.value)
  } else {
    const comparisonItem: ComparisonItem = {
      id: itemId.value,
      type: props.type,
      title: props.item.title,
      thumbnail: props.item.thumbnail,
      description: props.item.description,
      metadata: {
        author: props.item.author,
        date: props.item.date,
        size: props.item.size,
        duration: props.item.duration,
        category: props.item.category,
        tags: props.item.tags
      }
    }
    comparisonStore.addItem(comparisonItem)
  }
}
</script>

<template>
  <button
    @click.stop="toggleComparison"
    :class="[
      'comparison-btn',
      `comparison-btn--${size}`,
      `comparison-btn--${variant}`,
      { 'comparison-btn--active': isInComparison }
    ]"
    :title="buttonTitle"
  >
    <i class="fas fa-layer-group comparison-btn__icon"></i>
  </button>
</template>

<style scoped>
.comparison-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 0.5rem;
  transition: all 0.2s ease;
  cursor: pointer;
  border: none;
}

/* Size variants */
.comparison-btn--sm {
  width: 1.75rem;
  height: 1.75rem;
}

.comparison-btn--sm .comparison-btn__icon {
  font-size: 0.625rem;
}

.comparison-btn--md {
  width: 2rem;
  height: 2rem;
}

.comparison-btn--md .comparison-btn__icon {
  font-size: 0.75rem;
}

.comparison-btn--lg {
  width: 2.25rem;
  height: 2.25rem;
}

.comparison-btn--lg .comparison-btn__icon {
  font-size: 0.875rem;
}

/* Overlay variant (for card image overlays) */
.comparison-btn--overlay {
  background: rgba(255, 255, 255, 0.9);
  backdrop-filter: blur(4px);
  color: #9ca3af;
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.05);
}

.comparison-btn--overlay:hover {
  background: #f3e8ff;
  color: #9333ea;
}

.comparison-btn--overlay.comparison-btn--active {
  background: #9333ea;
  color: white;
  box-shadow: 0 0 0 2px rgba(147, 51, 234, 0.3);
}

/* Solid variant (for sidebars/lists) */
.comparison-btn--solid {
  background: #f3f4f6;
  color: #6b7280;
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.05);
}

.comparison-btn--solid:hover {
  background: #ede9fe;
  color: #7c3aed;
}

.comparison-btn--solid.comparison-btn--active {
  background: #9333ea;
  color: white;
}

/* Outline variant */
.comparison-btn--outline {
  background: white;
  color: #9ca3af;
  border: 1px solid #e5e7eb;
}

.comparison-btn--outline:hover {
  background: #faf5ff;
  color: #9333ea;
  border-color: #d8b4fe;
}

.comparison-btn--outline.comparison-btn--active {
  background: #9333ea;
  color: white;
  border-color: #9333ea;
  box-shadow: 0 0 0 2px rgba(147, 51, 234, 0.3);
}

/* Round variant for overlay */
.comparison-btn--overlay.comparison-btn--sm {
  border-radius: 9999px;
}
</style>
