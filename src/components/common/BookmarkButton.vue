<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useBookmarks } from '@/composables/useBookmarks'
import type { Bookmark } from '@/types/detail-pages'

const props = withDefaults(defineProps<{
  contentId: string
  contentType: Bookmark['contentType']
  size?: 'sm' | 'md' | 'lg'
  showLabel?: boolean
  variant?: 'icon' | 'button'
}>(), {
  size: 'md',
  showLabel: false,
  variant: 'icon'
})

const emit = defineEmits<{
  'bookmarked': [isBookmarked: boolean]
}>()

const { isBookmarked, toggleBookmark, loadBookmarks } = useBookmarks()

const isLoading = ref(false)
const showTooltip = ref(false)

const bookmarked = computed(() => isBookmarked(props.contentId, props.contentType))

const sizeClasses = computed(() => {
  switch (props.size) {
    case 'sm':
      return props.variant === 'icon' ? 'w-8 h-8 text-sm' : 'px-3 py-1.5 text-sm'
    case 'lg':
      return props.variant === 'icon' ? 'w-12 h-12 text-xl' : 'px-5 py-3 text-base'
    default:
      return props.variant === 'icon' ? 'w-10 h-10 text-base' : 'px-4 py-2 text-sm'
  }
})

onMounted(() => {
  loadBookmarks()
})

async function handleToggle() {
  isLoading.value = true
  try {
    const result = await toggleBookmark(props.contentId, props.contentType)
    emit('bookmarked', result)

    // Show tooltip briefly
    showTooltip.value = true
    setTimeout(() => {
      showTooltip.value = false
    }, 2000)
  } finally {
    isLoading.value = false
  }
}
</script>

<template>
  <!-- Icon Variant -->
  <div v-if="variant === 'icon'" class="relative inline-block">
    <button
      @click="handleToggle"
      :disabled="isLoading"
      :title="bookmarked ? 'Remove from saved' : 'Save for later'"
      :class="[
        'rounded-full flex items-center justify-center transition-all duration-200',
        sizeClasses,
        bookmarked
          ? 'bg-teal-100 text-teal-600 hover:bg-teal-200'
          : 'bg-gray-100 text-gray-500 hover:bg-gray-200 hover:text-gray-700'
      ]"
    >
      <i v-if="isLoading" class="fas fa-spinner fa-spin"></i>
      <i v-else :class="bookmarked ? 'fas fa-bookmark' : 'far fa-bookmark'"></i>
    </button>

    <!-- Tooltip -->
    <transition name="fade">
      <div
        v-if="showTooltip"
        class="absolute bottom-full left-1/2 -translate-x-1/2 mb-2 px-3 py-1.5 bg-gray-900 text-white text-xs rounded-lg whitespace-nowrap"
      >
        {{ bookmarked ? 'Saved!' : 'Removed from saved' }}
        <div class="absolute top-full left-1/2 -translate-x-1/2 -mt-1 border-4 border-transparent border-t-gray-900"></div>
      </div>
    </transition>
  </div>

  <!-- Button Variant -->
  <button
    v-else
    @click="handleToggle"
    :disabled="isLoading"
    :class="[
      'rounded-lg font-medium flex items-center gap-2 transition-all duration-200',
      sizeClasses,
      bookmarked
        ? 'bg-teal-100 text-teal-700 hover:bg-teal-200'
        : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
    ]"
  >
    <i v-if="isLoading" class="fas fa-spinner fa-spin"></i>
    <i v-else :class="bookmarked ? 'fas fa-bookmark' : 'far fa-bookmark'"></i>
    <span v-if="showLabel">{{ bookmarked ? 'Saved' : 'Save' }}</span>
  </button>
</template>

<style scoped>
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.2s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>
