<script setup lang="ts">
import { computed } from 'vue'

interface Props {
  to?: string
  count?: number
  size?: 'sm' | 'md'
  label?: string
}

const props = withDefaults(defineProps<Props>(), {
  size: 'sm'
})

const emit = defineEmits<{
  click: []
}>()

const isLink = computed(() => !!props.to)

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
    :class="['view-all-btn', `view-all-btn--${size}`]"
    @click="handleClick"
  >
    <span class="view-all-btn__text">
      <slot>{{ label || $t('common.viewAll') }}</slot>
      <span v-if="count !== undefined" class="view-all-btn__count">({{ count }})</span>
    </span>
    <i class="fas fa-arrow-right view-all-btn__icon"></i>
  </component>
</template>

<style scoped>
.view-all-btn {
  display: inline-flex;
  align-items: center;
  font-weight: 500;
  text-decoration: none;
  border: none;
  cursor: pointer;
  white-space: nowrap;
  background-color: #f0fdfa;
  color: #0d9488;
  transition: all 0.25s ease;
}

.view-all-btn:hover {
  background-color: #14b8a6;
  color: white;
}

/* Size: sm (default - matches dashboard) */
.view-all-btn--sm {
  padding: 4px 8px;
  font-size: 12px;
  gap: 4px;
  border-radius: 8px;
}

@media (min-width: 640px) {
  .view-all-btn--sm {
    padding: 6px 12px;
    font-size: 14px;
    gap: 6px;
  }
}

/* Size: md */
.view-all-btn--md {
  padding: 6px 12px;
  font-size: 13px;
  gap: 5px;
  border-radius: 8px;
}

@media (min-width: 640px) {
  .view-all-btn--md {
    padding: 8px 16px;
    font-size: 14px;
    gap: 6px;
  }
}

/* Icon */
.view-all-btn__icon {
  font-size: 10px;
  transition: transform 0.25s ease;
}

.view-all-btn--sm .view-all-btn__icon {
  font-size: 10px;
}

@media (min-width: 640px) {
  .view-all-btn--sm .view-all-btn__icon {
    font-size: 12px;
  }
}

.view-all-btn:hover .view-all-btn__icon {
  transform: translateX(3px);
}

/* RTL support */
[dir="rtl"] .view-all-btn__icon {
  transform: scaleX(-1);
}

[dir="rtl"] .view-all-btn:hover .view-all-btn__icon {
  transform: scaleX(-1) translateX(-3px);
}

/* Count */
.view-all-btn__count {
  margin-inline-start: 2px;
}
</style>
