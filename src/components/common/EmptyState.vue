<script setup lang="ts">
interface Props {
  icon?: string
  title: string
  description?: string
  actionLabel?: string
  actionIcon?: string
  size?: 'sm' | 'md' | 'lg'
}

const props = withDefaults(defineProps<Props>(), {
  icon: 'fas fa-inbox',
  size: 'md'
})

const emit = defineEmits<{
  action: []
}>()

function handleAction() {
  emit('action')
}
</script>

<template>
  <div :class="['empty-state', `empty-state--${size}`]">
    <!-- Icon -->
    <div class="empty-state__icon">
      <div class="empty-state__icon-bg"></div>
      <i :class="icon"></i>
    </div>

    <!-- Title -->
    <h3 class="empty-state__title">{{ title }}</h3>

    <!-- Description -->
    <p v-if="description" class="empty-state__description">{{ description }}</p>

    <!-- Slot for custom content -->
    <slot></slot>

    <!-- Action Button -->
    <button
      v-if="actionLabel"
      @click="handleAction"
      class="empty-state__action"
    >
      <i v-if="actionIcon" :class="actionIcon"></i>
      {{ actionLabel }}
    </button>
  </div>
</template>

<style scoped>
.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  text-align: center;
  width: 100%;
}

/* Size variants */
.empty-state--sm {
  padding: 1.5rem 1rem;
}

.empty-state--md {
  padding: 2.5rem 1.5rem;
}

.empty-state--lg {
  padding: 4rem 2rem;
}

/* Icon */
.empty-state__icon {
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 1rem;
}

.empty-state--sm .empty-state__icon {
  width: 48px;
  height: 48px;
}

.empty-state--md .empty-state__icon {
  width: 64px;
  height: 64px;
}

.empty-state--lg .empty-state__icon {
  width: 80px;
  height: 80px;
}

.empty-state__icon-bg {
  position: absolute;
  inset: 0;
  background: linear-gradient(135deg, #f0fdfa 0%, #e0f2fe 100%);
  border-radius: 50%;
  opacity: 0.8;
}

.empty-state__icon i {
  position: relative;
  z-index: 1;
  color: #14b8a6;
}

.empty-state--sm .empty-state__icon i {
  font-size: 1.25rem;
}

.empty-state--md .empty-state__icon i {
  font-size: 1.5rem;
}

.empty-state--lg .empty-state__icon i {
  font-size: 2rem;
}

/* Title */
.empty-state__title {
  font-weight: 600;
  color: #1e293b;
  margin: 0 0 0.5rem;
}

.empty-state--sm .empty-state__title {
  font-size: 0.9375rem;
}

.empty-state--md .empty-state__title {
  font-size: 1.0625rem;
}

.empty-state--lg .empty-state__title {
  font-size: 1.25rem;
}

/* Description */
.empty-state__description {
  color: #64748b;
  margin: 0 0 1.25rem;
  max-width: 320px;
}

.empty-state--sm .empty-state__description {
  font-size: 0.8125rem;
}

.empty-state--md .empty-state__description {
  font-size: 0.875rem;
}

.empty-state--lg .empty-state__description {
  font-size: 0.9375rem;
}

/* Action Button */
.empty-state__action {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.625rem 1.25rem;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
  border: none;
  border-radius: 0.5rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
}

.empty-state--sm .empty-state__action {
  padding: 0.5rem 1rem;
  font-size: 0.8125rem;
}

.empty-state--md .empty-state__action {
  padding: 0.625rem 1.25rem;
  font-size: 0.875rem;
}

.empty-state--lg .empty-state__action {
  padding: 0.75rem 1.5rem;
  font-size: 0.9375rem;
}

.empty-state__action:hover {
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.3);
}

.empty-state__action i {
  font-size: 0.875em;
}
</style>
