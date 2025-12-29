<script setup lang="ts">
defineProps<{
  title: string
  value: string | number
  icon: string
  iconClass?: string
  trend?: {
    value: number
    label: string
    direction: 'up' | 'down'
  }
}>()
</script>

<template>
  <div class="card p-6">
    <div class="flex items-start justify-between">
      <div>
        <p class="text-sm font-medium text-gray-500">{{ title }}</p>
        <p class="text-3xl font-bold text-gray-900 mt-2">{{ value }}</p>
        <div v-if="trend" class="flex items-center gap-1 mt-3 text-sm" :class="trend.direction === 'up' ? 'text-emerald-600' : 'text-red-600'">
          <i :class="trend.direction === 'up' ? 'fas fa-arrow-up' : 'fas fa-arrow-down'" class="text-xs"></i>
          <span>{{ trend.value }}%</span>
          <span class="text-gray-400 ml-1">{{ trend.label }}</span>
        </div>
      </div>
      <div class="icon-box icon-box-lg" :class="iconClass || 'icon-box-primary-soft'">
        <i :class="icon"></i>
      </div>
    </div>
  </div>
</template>
