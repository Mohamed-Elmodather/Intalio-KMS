<script setup lang="ts">
import { computed } from 'vue'

type SkeletonType = 'card' | 'list-item' | 'text' | 'avatar' | 'thumbnail' | 'badge' | 'button'
type ContentType = 'article' | 'document' | 'media' | 'event' | 'course' | 'generic'

interface Props {
  type?: SkeletonType
  contentType?: ContentType
  count?: number
  columns?: 1 | 2 | 3 | 4
  animated?: boolean
  showImage?: boolean
  showBadge?: boolean
  showAvatar?: boolean
  lines?: number
}

const props = withDefaults(defineProps<Props>(), {
  type: 'card',
  contentType: 'generic',
  count: 3,
  columns: 3,
  animated: true,
  showImage: true,
  showBadge: true,
  showAvatar: true,
  lines: 2
})

const animationClass = computed(() => props.animated ? 'animate-pulse' : '')

const gridClasses = computed(() => {
  switch (props.columns) {
    case 1: return 'grid-cols-1'
    case 2: return 'grid-cols-1 sm:grid-cols-2'
    case 3: return 'grid-cols-1 sm:grid-cols-2 lg:grid-cols-3'
    case 4: return 'grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4'
    default: return 'grid-cols-1 sm:grid-cols-2 lg:grid-cols-3'
  }
})

// Content type specific configurations
const imageAspect = computed(() => {
  switch (props.contentType) {
    case 'article': return 'aspect-[16/9]'
    case 'document': return 'aspect-square'
    case 'media': return 'aspect-video'
    case 'event': return 'aspect-[4/3]'
    case 'course': return 'aspect-video'
    default: return 'aspect-video'
  }
})
</script>

<template>
  <!-- Card Grid Skeleton -->
  <div v-if="type === 'card'" class="grid gap-5" :class="gridClasses">
    <div
      v-for="i in count"
      :key="i"
      class="skeleton-card bg-white rounded-2xl overflow-hidden border border-gray-100 shadow-sm"
    >
      <!-- Image Placeholder -->
      <div
        v-if="showImage"
        class="bg-gray-200 relative"
        :class="[imageAspect, animationClass]"
      >
        <!-- Badge Placeholder -->
        <div v-if="showBadge" class="absolute top-3 left-3">
          <div class="h-5 w-16 bg-gray-300 rounded-full" :class="animationClass"></div>
        </div>
        <!-- Action Buttons Placeholder -->
        <div class="absolute top-3 right-3 flex gap-1.5">
          <div class="w-7 h-7 bg-gray-300 rounded-full" :class="animationClass"></div>
          <div class="w-7 h-7 bg-gray-300 rounded-full" :class="animationClass"></div>
        </div>
      </div>

      <!-- Content -->
      <div class="p-4 space-y-3">
        <!-- Meta Line -->
        <div class="flex items-center gap-2">
          <div class="h-3 w-16 bg-gray-200 rounded" :class="animationClass"></div>
          <div class="h-3 w-20 bg-gray-200 rounded" :class="animationClass"></div>
        </div>

        <!-- Title -->
        <div class="h-5 w-full bg-gray-200 rounded" :class="animationClass"></div>
        <div class="h-5 w-3/4 bg-gray-200 rounded" :class="animationClass"></div>

        <!-- Description Lines -->
        <div class="space-y-2">
          <div v-for="l in lines" :key="l" class="h-3 bg-gray-100 rounded" :class="[animationClass, l === lines ? 'w-2/3' : 'w-full']"></div>
        </div>

        <!-- Tags -->
        <div class="flex gap-1.5 pt-1">
          <div class="h-5 w-14 bg-gray-100 rounded-full" :class="animationClass"></div>
          <div class="h-5 w-16 bg-gray-100 rounded-full" :class="animationClass"></div>
          <div class="h-5 w-12 bg-gray-100 rounded-full" :class="animationClass"></div>
        </div>

        <!-- Footer -->
        <div class="flex items-center justify-between pt-3 border-t border-gray-100">
          <!-- Avatar & Name -->
          <div v-if="showAvatar" class="flex items-center gap-2">
            <div class="w-7 h-7 bg-gray-200 rounded-full" :class="animationClass"></div>
            <div class="h-3 w-20 bg-gray-200 rounded" :class="animationClass"></div>
          </div>
          <!-- Stats -->
          <div class="flex gap-3">
            <div class="h-3 w-10 bg-gray-100 rounded" :class="animationClass"></div>
            <div class="h-3 w-10 bg-gray-100 rounded" :class="animationClass"></div>
          </div>
        </div>
      </div>
    </div>
  </div>

  <!-- List Item Skeleton -->
  <div v-else-if="type === 'list-item'" class="space-y-3">
    <div
      v-for="i in count"
      :key="i"
      class="skeleton-list-item flex gap-4 p-4 bg-white rounded-xl border border-gray-100"
    >
      <!-- Thumbnail -->
      <div
        v-if="showImage"
        class="flex-shrink-0 w-24 h-20 bg-gray-200 rounded-lg"
        :class="animationClass"
      ></div>

      <!-- Content -->
      <div class="flex-1 min-w-0 space-y-2">
        <!-- Badges -->
        <div class="flex gap-2">
          <div class="h-4 w-16 bg-gray-200 rounded" :class="animationClass"></div>
          <div class="h-4 w-12 bg-gray-200 rounded" :class="animationClass"></div>
        </div>
        <!-- Title -->
        <div class="h-4 w-3/4 bg-gray-200 rounded" :class="animationClass"></div>
        <!-- Description -->
        <div class="h-3 w-full bg-gray-100 rounded" :class="animationClass"></div>
        <!-- Footer -->
        <div class="flex items-center gap-4 pt-1">
          <div v-if="showAvatar" class="flex items-center gap-2">
            <div class="w-5 h-5 bg-gray-200 rounded-full" :class="animationClass"></div>
            <div class="h-3 w-16 bg-gray-100 rounded" :class="animationClass"></div>
          </div>
          <div class="h-3 w-12 bg-gray-100 rounded" :class="animationClass"></div>
        </div>
      </div>

      <!-- Actions -->
      <div class="flex flex-col gap-2">
        <div class="w-8 h-8 bg-gray-100 rounded-lg" :class="animationClass"></div>
        <div class="w-8 h-8 bg-gray-100 rounded-lg" :class="animationClass"></div>
      </div>
    </div>
  </div>

  <!-- Text Lines Skeleton -->
  <div v-else-if="type === 'text'" class="space-y-2">
    <div
      v-for="i in count"
      :key="i"
      class="h-4 bg-gray-200 rounded"
      :class="[animationClass, i === count ? 'w-2/3' : 'w-full']"
    ></div>
  </div>

  <!-- Avatar Skeleton -->
  <div v-else-if="type === 'avatar'" class="flex items-center gap-3">
    <div
      v-for="i in count"
      :key="i"
      class="w-10 h-10 bg-gray-200 rounded-full"
      :class="animationClass"
    ></div>
  </div>

  <!-- Thumbnail Skeleton -->
  <div v-else-if="type === 'thumbnail'" class="grid gap-3" :class="gridClasses">
    <div
      v-for="i in count"
      :key="i"
      class="bg-gray-200 rounded-xl"
      :class="[imageAspect, animationClass]"
    ></div>
  </div>

  <!-- Badge Skeleton -->
  <div v-else-if="type === 'badge'" class="flex flex-wrap gap-2">
    <div
      v-for="i in count"
      :key="i"
      class="h-6 bg-gray-200 rounded-full"
      :class="animationClass"
      :style="{ width: `${50 + Math.random() * 30}px` }"
    ></div>
  </div>

  <!-- Button Skeleton -->
  <div v-else-if="type === 'button'" class="flex gap-2">
    <div
      v-for="i in count"
      :key="i"
      class="h-10 bg-gray-200 rounded-lg"
      :class="animationClass"
      :style="{ width: `${80 + Math.random() * 40}px` }"
    ></div>
  </div>
</template>

<style scoped>
.skeleton-card,
.skeleton-list-item {
  transition: opacity 0.3s ease;
}
</style>
