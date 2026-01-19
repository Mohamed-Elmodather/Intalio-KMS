<script setup lang="ts">
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { useComparisonStore } from '@/stores/comparison'
import type { ComparisonItem } from '@/types'

const { t, locale } = useI18n()
const comparisonStore = useComparisonStore()

// Type icons and colors
const typeConfig: Record<string, { icon: string; bg: string; text: string; border: string }> = {
  document: {
    icon: 'fas fa-file-alt',
    bg: 'bg-blue-50',
    text: 'text-blue-600',
    border: 'border-blue-200',
  },
  article: {
    icon: 'fas fa-newspaper',
    bg: 'bg-purple-50',
    text: 'text-purple-600',
    border: 'border-purple-200',
  },
  media: {
    icon: 'fas fa-photo-video',
    bg: 'bg-pink-50',
    text: 'text-pink-600',
    border: 'border-pink-200',
  },
  event: {
    icon: 'fas fa-calendar-alt',
    bg: 'bg-orange-50',
    text: 'text-orange-600',
    border: 'border-orange-200',
  },
}

function getTypeConfig(type: string) {
  return typeConfig[type] || typeConfig.document
}

function formatSize(size?: number): string {
  if (!size) return '-'
  if (size < 1024) return `${size} B`
  if (size < 1024 * 1024) return `${(size / 1024).toFixed(1)} KB`
  return `${(size / (1024 * 1024)).toFixed(1)} MB`
}

function formatDuration(duration?: number): string {
  if (!duration) return '-'
  const mins = Math.floor(duration / 60)
  const secs = duration % 60
  return `${mins}:${secs.toString().padStart(2, '0')}`
}

function formatDate(date?: string): string {
  if (!date) return '-'
  return new Date(date).toLocaleDateString(locale.value === 'ar' ? 'ar-SA' : 'en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
  })
}
</script>

<template>
  <div class="comparison-side-by-side">
    <!-- Horizontal Scroll Container -->
    <div class="flex gap-4 overflow-x-auto pb-4 -mx-2 px-2">
      <!-- Item Cards -->
      <div
        v-for="item in comparisonStore.selectedItems"
        :key="item.id"
        class="flex-shrink-0 w-72 rounded-xl border overflow-hidden bg-white shadow-sm hover:shadow-md transition-shadow"
        :class="getTypeConfig(item.type).border"
      >
        <!-- Card Header with Type Badge -->
        <div
          class="px-4 py-3 flex items-center gap-3"
          :class="getTypeConfig(item.type).bg"
        >
          <div
            class="w-10 h-10 rounded-lg flex items-center justify-center"
            :class="[getTypeConfig(item.type).bg, getTypeConfig(item.type).text]"
            style="background: rgba(255,255,255,0.7)"
          >
            <i :class="getTypeConfig(item.type).icon" class="text-lg" />
          </div>
          <div class="flex-1 min-w-0">
            <span
              class="text-xs font-medium uppercase tracking-wide"
              :class="getTypeConfig(item.type).text"
            >
              {{ item.type }}
            </span>
          </div>
          <button
            class="p-1.5 rounded-lg hover:bg-white/50 transition-colors"
            :class="getTypeConfig(item.type).text"
            @click="comparisonStore.removeItem(item.id)"
          >
            <i class="fas fa-times text-sm" />
          </button>
        </div>

        <!-- Thumbnail or Icon -->
        <div class="h-32 bg-gray-100 flex items-center justify-center overflow-hidden">
          <img
            v-if="item.thumbnail"
            :src="item.thumbnail"
            :alt="item.title"
            class="w-full h-full object-cover"
          />
          <i
            v-else
            :class="getTypeConfig(item.type).icon"
            class="text-4xl text-gray-300"
          />
        </div>

        <!-- Content -->
        <div class="p-4">
          <!-- Title -->
          <h3 class="font-semibold text-gray-900 mb-2 line-clamp-2">
            {{ item.title }}
          </h3>

          <!-- Description -->
          <p
            v-if="item.description"
            class="text-sm text-gray-500 mb-3 line-clamp-2"
          >
            {{ item.description }}
          </p>

          <!-- Metadata -->
          <div class="space-y-2">
            <!-- Author -->
            <div v-if="item.metadata.author" class="flex items-center gap-2 text-sm">
              <i class="fas fa-user text-gray-400 w-4" />
              <span class="text-gray-600">{{ item.metadata.author }}</span>
            </div>

            <!-- Date -->
            <div v-if="item.metadata.date" class="flex items-center gap-2 text-sm">
              <i class="fas fa-calendar text-gray-400 w-4" />
              <span class="text-gray-600">{{ formatDate(item.metadata.date) }}</span>
            </div>

            <!-- Size (for documents) -->
            <div v-if="item.metadata.size" class="flex items-center gap-2 text-sm">
              <i class="fas fa-file text-gray-400 w-4" />
              <span class="text-gray-600">{{ formatSize(item.metadata.size) }}</span>
            </div>

            <!-- Duration (for media) -->
            <div v-if="item.metadata.duration" class="flex items-center gap-2 text-sm">
              <i class="fas fa-clock text-gray-400 w-4" />
              <span class="text-gray-600">{{ formatDuration(item.metadata.duration) }}</span>
            </div>

            <!-- Category -->
            <div v-if="item.metadata.category" class="flex items-center gap-2 text-sm">
              <i class="fas fa-folder text-gray-400 w-4" />
              <span class="text-gray-600">{{ item.metadata.category }}</span>
            </div>
          </div>

          <!-- Tags -->
          <div v-if="item.metadata.tags?.length" class="mt-3 flex flex-wrap gap-1">
            <span
              v-for="tag in item.metadata.tags.slice(0, 3)"
              :key="tag"
              class="px-2 py-0.5 bg-gray-100 text-gray-600 text-xs rounded-full"
            >
              {{ tag }}
            </span>
            <span
              v-if="item.metadata.tags.length > 3"
              class="px-2 py-0.5 bg-gray-100 text-gray-500 text-xs rounded-full"
            >
              +{{ item.metadata.tags.length - 3 }}
            </span>
          </div>
        </div>
      </div>
    </div>

    <!-- Empty State -->
    <div
      v-if="comparisonStore.selectedItems.length === 0"
      class="text-center py-12"
    >
      <div class="w-16 h-16 rounded-full bg-gray-100 mx-auto mb-4 flex items-center justify-center">
        <i class="fas fa-columns text-2xl text-gray-400" />
      </div>
      <h3 class="text-lg font-medium text-gray-700 mb-1">{{ $t('comparison.noItemsSelected') }}</h3>
      <p class="text-gray-500">{{ $t('comparison.selectItemsToCompare') }}</p>
    </div>

    <!-- Comparison Hint -->
    <div
      v-if="comparisonStore.selectedItems.length >= 2"
      class="mt-6 p-4 bg-teal-50 rounded-xl border border-teal-100"
    >
      <div class="flex items-start gap-3">
        <div class="w-8 h-8 rounded-lg bg-teal-100 flex items-center justify-center flex-shrink-0">
          <i class="fas fa-lightbulb text-teal-600" />
        </div>
        <div>
          <h4 class="font-medium text-teal-800 mb-1">{{ $t('comparison.quickTips.title') }}</h4>
          <ul class="text-sm text-teal-700 space-y-1">
            <li>{{ $t('comparison.quickTips.tip1') }}</li>
            <li>{{ $t('comparison.quickTips.tip2') }}</li>
            <li>{{ $t('comparison.quickTips.tip3') }}</li>
          </ul>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
</style>
