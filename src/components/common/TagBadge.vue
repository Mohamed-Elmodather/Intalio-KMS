<script setup lang="ts">
import { computed } from 'vue'

interface Props {
  tag: string
  size?: 'xs' | 'sm' | 'md'
  variant?: 'default' | 'outlined' | 'gradient' | 'colored'
  color?: string
  clickable?: boolean
  removable?: boolean
  showHash?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  size: 'sm',
  variant: 'default',
  clickable: false,
  removable: false,
  showHash: false
})

const emit = defineEmits<{
  click: [tag: string]
  remove: [tag: string]
}>()

const sizeClasses = computed(() => {
  switch (props.size) {
    case 'xs':
      return 'px-1.5 py-0.5 text-[9px] gap-0.5'
    case 'md':
      return 'px-2.5 py-1 text-xs gap-1.5'
    case 'sm':
    default:
      return 'px-2 py-0.5 text-[10px] gap-1'
  }
})

const variantStyles = computed(() => {
  if (props.color) {
    return {
      background: `${props.color}15`,
      color: props.color,
      border: `1px solid ${props.color}30`
    }
  }

  switch (props.variant) {
    case 'outlined':
      return {
        background: 'transparent',
        color: '#64748b',
        border: '1px solid #e2e8f0'
      }
    case 'gradient':
      return {
        background: 'linear-gradient(135deg, #f1f5f9 0%, #e2e8f0 100%)',
        color: '#475569',
        border: '1px solid #e2e8f0'
      }
    case 'colored':
      return {
        background: 'linear-gradient(135deg, #ccfbf1 0%, #99f6e4 100%)',
        color: '#0d9488',
        border: '1px solid #5eead4'
      }
    case 'default':
    default:
      return {
        background: '#f1f5f9',
        color: '#64748b',
        border: '1px solid #e2e8f0'
      }
  }
})

function handleClick() {
  if (props.clickable) {
    emit('click', props.tag)
  }
}

function handleRemove(e: Event) {
  e.stopPropagation()
  emit('remove', props.tag)
}
</script>

<template>
  <span
    class="tag-badge"
    :class="[
      sizeClasses,
      { 'tag-badge--clickable': clickable }
    ]"
    :style="variantStyles"
    @click="handleClick"
  >
    <span v-if="showHash" class="tag-hash">#</span>
    <span class="tag-text">{{ tag }}</span>
    <button
      v-if="removable"
      type="button"
      class="tag-remove"
      @click="handleRemove"
    >
      <i class="fas fa-times"></i>
    </button>
  </span>
</template>

<style scoped>
.tag-badge {
  display: inline-flex;
  align-items: center;
  font-weight: 500;
  border-radius: 0.375rem;
  white-space: nowrap;
  transition: all 0.2s ease;
}

.tag-badge--clickable {
  cursor: pointer;
}

.tag-badge--clickable:hover {
  background: linear-gradient(135deg, #ccfbf1 0%, #99f6e4 100%) !important;
  color: #0d9488 !important;
  border-color: #5eead4 !important;
}

.tag-hash {
  opacity: 0.6;
}

.tag-text {
  line-height: 1;
}

.tag-remove {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 14px;
  height: 14px;
  margin-inline-start: 2px;
  padding: 0;
  background: transparent;
  border: none;
  border-radius: 50%;
  color: inherit;
  opacity: 0.6;
  cursor: pointer;
  transition: all 0.15s ease;
}

.tag-remove:hover {
  opacity: 1;
  background: rgba(0, 0, 0, 0.1);
}

.tag-remove i {
  font-size: 8px;
}
</style>
