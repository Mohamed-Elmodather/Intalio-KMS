<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useRelatedContent } from '@/composables/useRelatedContent'
import type { RelatedItem } from '@/types/detail-pages'

const { t } = useI18n()

const props = withDefaults(defineProps<{
  contentType: string
  contentId: string
  title?: string
  limit?: number
  mixed?: boolean
}>(), {
  limit: 4,
  mixed: false
})

const router = useRouter()
const {
  relatedItems,
  isLoading,
  loadRelatedContent,
  loadMixedContent,
  getContentTypeIcon,
  getContentTypeColor,
  getContentTypeLabel
} = useRelatedContent(props.contentType, props.contentId)

const scrollContainer = ref<HTMLElement | null>(null)
const canScrollLeft = ref(false)
const canScrollRight = ref(true)

onMounted(async () => {
  if (props.mixed) {
    await loadMixedContent(props.limit)
  } else {
    await loadRelatedContent(props.limit)
  }
  checkScrollButtons()
})

function checkScrollButtons() {
  if (!scrollContainer.value) return
  const { scrollLeft, scrollWidth, clientWidth } = scrollContainer.value
  canScrollLeft.value = scrollLeft > 0
  canScrollRight.value = scrollLeft < scrollWidth - clientWidth - 10
}

function scrollLeft() {
  if (!scrollContainer.value) return
  scrollContainer.value.scrollBy({ left: -300, behavior: 'smooth' })
  setTimeout(checkScrollButtons, 300)
}

function scrollRight() {
  if (!scrollContainer.value) return
  scrollContainer.value.scrollBy({ left: 300, behavior: 'smooth' })
  setTimeout(checkScrollButtons, 300)
}

function navigateToItem(item: RelatedItem) {
  const routes: Record<string, string> = {
    article: 'ArticleView',
    document: 'DocumentView',
    media: 'MediaPlayer',
    course: 'CourseView',
    event: 'EventDetail',
    poll: 'PollDetail'
  }
  const routeName = routes[item.type]
  if (routeName) {
    router.push({ name: routeName, params: { id: item.id } })
  }
}
</script>

<template>
  <div class="related-content-carousel">
    <!-- Header -->
    <div class="flex items-center justify-between mb-4">
      <h3 class="text-lg font-semibold text-gray-900">{{ title || $t('common.relatedContent') }}</h3>
      <div class="flex gap-2">
        <button
          @click="scrollLeft"
          :disabled="!canScrollLeft"
          class="w-8 h-8 rounded-full bg-gray-100 flex items-center justify-center text-gray-600 hover:bg-gray-200 disabled:opacity-40 disabled:cursor-not-allowed transition-all"
        >
          <i class="fas fa-chevron-left text-sm"></i>
        </button>
        <button
          @click="scrollRight"
          :disabled="!canScrollRight"
          class="w-8 h-8 rounded-full bg-gray-100 flex items-center justify-center text-gray-600 hover:bg-gray-200 disabled:opacity-40 disabled:cursor-not-allowed transition-all"
        >
          <i class="fas fa-chevron-right text-sm"></i>
        </button>
      </div>
    </div>

    <!-- Loading State -->
    <div v-if="isLoading" class="flex gap-4 overflow-hidden">
      <div
        v-for="i in 4"
        :key="i"
        class="flex-shrink-0 w-64 bg-gray-100 rounded-xl animate-pulse"
      >
        <div class="h-36 bg-gray-200 rounded-t-xl"></div>
        <div class="p-4 space-y-2">
          <div class="h-4 bg-gray-200 rounded w-3/4"></div>
          <div class="h-3 bg-gray-200 rounded w-1/2"></div>
        </div>
      </div>
    </div>

    <!-- Content Carousel -->
    <div
      v-else-if="relatedItems.length > 0"
      ref="scrollContainer"
      @scroll="checkScrollButtons"
      class="flex gap-4 overflow-x-auto scrollbar-hide pb-2 -mx-2 px-2"
    >
      <div
        v-for="item in relatedItems"
        :key="item.id"
        @click="navigateToItem(item)"
        class="flex-shrink-0 w-64 bg-white border border-gray-200 rounded-xl overflow-hidden hover:shadow-lg hover:border-teal-300 transition-all cursor-pointer group"
      >
        <!-- Thumbnail -->
        <div class="relative h-36 bg-gray-100 overflow-hidden">
          <img
            v-if="item.thumbnail"
            :src="item.thumbnail"
            :alt="item.title"
            class="w-full h-full object-cover group-hover:scale-105 transition-transform duration-300"
          >
          <div v-else class="w-full h-full flex items-center justify-center bg-gradient-to-br from-gray-100 to-gray-200">
            <i :class="[getContentTypeIcon(item.type), 'text-4xl text-gray-400']"></i>
          </div>

          <!-- Type Badge -->
          <span
            :class="[
              'absolute top-2 left-2 px-2 py-1 rounded-full text-xs font-medium',
              getContentTypeColor(item.type)
            ]"
          >
            <i :class="[getContentTypeIcon(item.type), 'mr-1']"></i>
            {{ getContentTypeLabel(item.type) }}
          </span>
        </div>

        <!-- Content -->
        <div class="p-4">
          <h4 class="font-medium text-gray-900 line-clamp-2 group-hover:text-teal-600 transition-colors">
            {{ item.title }}
          </h4>
          <p v-if="item.description" class="text-sm text-gray-500 mt-1 line-clamp-2">
            {{ item.description }}
          </p>
          <p class="text-xs text-gray-400 mt-2">
            {{ item.metadata }}
          </p>
        </div>
      </div>
    </div>

    <!-- Empty State -->
    <div v-else class="text-center py-8 bg-gray-50 rounded-xl">
      <i class="fas fa-folder-open text-3xl text-gray-300 mb-2"></i>
      <p class="text-gray-500">{{ $t('common.noRelatedContent') }}</p>
    </div>
  </div>
</template>

<style scoped>
.scrollbar-hide {
  -ms-overflow-style: none;
  scrollbar-width: none;
}

.scrollbar-hide::-webkit-scrollbar {
  display: none;
}

.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
</style>
