<script setup lang="ts">
import { computed } from 'vue'

interface Props {
  to?: string
  count?: number
  variant?: 'primary' | 'subtle' | 'text'
  size?: 'sm' | 'md'
  iconPosition?: 'end' | 'start'
  label?: string
}

const props = withDefaults(defineProps<Props>(), {
  variant: 'primary',
  size: 'md',
  iconPosition: 'end'
})

const emit = defineEmits<{
  click: []
}>()

const isLink = computed(() => !!props.to)

const buttonClasses = computed(() => {
  const base = [
    'view-all-btn',
    `view-all-btn--${props.variant}`,
    `view-all-btn--${props.size}`
  ]

  if (props.iconPosition === 'start') {
    base.push('view-all-btn--icon-start')
  }

  return base
})

function handleClick() {
  if (!isLink.value) {
    emit('click')
  }
}
</script>

<template>
  <component
    :is="isLink ? 'router-link' : 'button'"
    :to="to"
    :class="buttonClasses"
    @click="handleClick"
  >
    <i v-if="iconPosition === 'start'" class="fas fa-arrow-right view-all-btn__icon"></i>
    <span class="view-all-btn__text">
      <slot>{{ label || $t('common.viewAll') }}</slot>
      <span v-if="count !== undefined" class="view-all-btn__count">({{ count }})</span>
    </span>
    <i v-if="iconPosition === 'end'" class="fas fa-arrow-right view-all-btn__icon"></i>
  </component>
</template>

<style scoped>
.view-all-btn {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  font-weight: 500;
  text-decoration: none;
  border: none;
  cursor: pointer;
  transition: all 0.2s ease;
  white-space: nowrap;
}

.view-all-btn--icon-start {
  flex-direction: row-reverse;
}

/* Size variants */
.view-all-btn--sm {
  padding: 4px 8px;
  font-size: 12px;
  border-radius: 6px;
}

.view-all-btn--sm .view-all-btn__icon {
  font-size: 9px;
}

.view-all-btn--md {
  padding: 6px 12px;
  font-size: 13px;
  border-radius: 8px;
}

.view-all-btn--md .view-all-btn__icon {
  font-size: 10px;
}

@media (min-width: 640px) {
  .view-all-btn--sm {
    padding: 6px 10px;
    font-size: 13px;
  }

  .view-all-btn--sm .view-all-btn__icon {
    font-size: 10px;
  }

  .view-all-btn--md {
    padding: 8px 14px;
    font-size: 14px;
  }

  .view-all-btn--md .view-all-btn__icon {
    font-size: 11px;
  }
}

/* Primary variant - filled background */
.view-all-btn--primary {
  background: #f0fdfa;
  color: #0d9488;
}

.view-all-btn--primary:hover {
  background: #14b8a6;
  color: white;
  gap: 8px;
}

/* Subtle variant - transparent background */
.view-all-btn--subtle {
  background: transparent;
  color: #0d9488;
  padding-inline-start: 0;
  padding-inline-end: 0;
}

.view-all-btn--subtle:hover {
  color: #0f766e;
  gap: 10px;
}

/* Text variant - minimal styling */
.view-all-btn--text {
  background: transparent;
  color: #14b8a6;
  padding: 0;
  font-weight: 600;
}

.view-all-btn--text:hover {
  color: #0f766e;
  gap: 10px;
}

/* Icon styling */
.view-all-btn__icon {
  transition: transform 0.2s ease;
}

[dir="rtl"] .view-all-btn__icon {
  transform: scaleX(-1);
}

.view-all-btn:hover .view-all-btn__icon {
  transform: translateX(2px);
}

[dir="rtl"] .view-all-btn:hover .view-all-btn__icon {
  transform: scaleX(-1) translateX(2px);
}

/* Count badge */
.view-all-btn__count {
  opacity: 0.8;
  font-weight: 600;
}
</style>
