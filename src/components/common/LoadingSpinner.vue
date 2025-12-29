<script setup lang="ts">
defineProps<{
  size?: 'sm' | 'md' | 'lg'
  text?: string
}>()
</script>

<template>
  <div class="flex flex-col items-center justify-center gap-3">
    <div
      class="spinner"
      :class="{
        'w-5 h-5 border-2': size === 'sm',
        'w-8 h-8 border-3': size === 'md' || !size,
        'w-12 h-12 border-4': size === 'lg'
      }"
    ></div>
    <p v-if="text" class="text-sm text-gray-500">{{ text }}</p>
  </div>
</template>

<style scoped>
.spinner {
  border-radius: 50%;
  border-color: #e5e7eb;
  border-top-color: #14b8a6;
  animation: spin 0.8s linear infinite;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}
</style>
