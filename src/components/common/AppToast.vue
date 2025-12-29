<script setup lang="ts">
import { useUIStore, type Toast } from '@/stores/ui'

const uiStore = useUIStore()

function getIcon(type: Toast['type']): string {
  const icons = {
    success: 'fas fa-check-circle',
    error: 'fas fa-exclamation-circle',
    warning: 'fas fa-exclamation-triangle',
    info: 'fas fa-info-circle',
  }
  return icons[type]
}

function getStyles(type: Toast['type']): string {
  const styles = {
    success: 'bg-emerald-50 border-emerald-200 text-emerald-800',
    error: 'bg-red-50 border-red-200 text-red-800',
    warning: 'bg-amber-50 border-amber-200 text-amber-800',
    info: 'bg-blue-50 border-blue-200 text-blue-800',
  }
  return styles[type]
}

function getIconStyles(type: Toast['type']): string {
  const styles = {
    success: 'text-emerald-500',
    error: 'text-red-500',
    warning: 'text-amber-500',
    info: 'text-blue-500',
  }
  return styles[type]
}
</script>

<template>
  <div class="fixed top-20 right-4 z-[100] space-y-3 pointer-events-none">
    <TransitionGroup name="toast">
      <div
        v-for="toast in uiStore.toasts"
        :key="toast.id"
        class="pointer-events-auto max-w-sm w-full rounded-xl border shadow-lg p-4"
        :class="getStyles(toast.type)"
      >
        <div class="flex items-start gap-3">
          <i :class="[getIcon(toast.type), getIconStyles(toast.type)]" class="text-lg mt-0.5"></i>
          <div class="flex-1 min-w-0">
            <p class="font-semibold text-sm">{{ toast.title }}</p>
            <p v-if="toast.message" class="text-sm opacity-80 mt-0.5">{{ toast.message }}</p>
          </div>
          <button
            @click="uiStore.removeToast(toast.id)"
            class="text-current opacity-50 hover:opacity-100 transition-opacity"
          >
            <i class="fas fa-times"></i>
          </button>
        </div>
      </div>
    </TransitionGroup>
  </div>
</template>

<style scoped>
.toast-enter-active {
  animation: slideIn 0.3s ease-out;
}

.toast-leave-active {
  animation: slideOut 0.3s ease-in;
}

@keyframes slideIn {
  from {
    opacity: 0;
    transform: translateX(100%);
  }
  to {
    opacity: 1;
    transform: translateX(0);
  }
}

@keyframes slideOut {
  from {
    opacity: 1;
    transform: translateX(0);
  }
  to {
    opacity: 0;
    transform: translateX(100%);
  }
}
</style>
