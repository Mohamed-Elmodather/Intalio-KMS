<script setup lang="ts">
import { computed } from 'vue'
import CategoryBadge from './CategoryBadge.vue'
import StatusBadge from './StatusBadge.vue'
import TagBadge from './TagBadge.vue'

type ContentType = 'article' | 'document' | 'media' | 'event' | 'course' | 'generic'
type CardVariant = 'grid' | 'list' | 'compact' | 'featured'

interface Author {
  name: string
  initials?: string
  avatar?: string
}

interface Stats {
  views?: number
  likes?: number
  comments?: number
  downloads?: number
  duration?: string
  readTime?: string
  fileSize?: string
}

interface Props {
  // Content identification
  contentType?: ContentType
  variant?: CardVariant

  // Main content
  title: string
  subtitle?: string
  excerpt?: string
  image?: string
  imageAlt?: string
  icon?: string

  // Category & Status
  category?: string
  categoryId?: string
  categoryIcon?: string
  categoryColor?: string
  status?: string
  statusLabel?: string
  tags?: string[]
  maxTags?: number

  // Meta information
  date?: string
  readTime?: string
  duration?: string
  author?: Author
  stats?: Stats

  // Event-specific
  eventDate?: { day: string | number; month: string }
  eventTime?: string
  eventLocation?: string
  isVirtual?: boolean
  attendeesCount?: number
  attendees?: Array<{ initials: string; color: string }>

  // Document-specific
  fileType?: string
  fileSize?: string

  // Course-specific
  progress?: number
  rating?: number
  lessonsCount?: number
  instructor?: string

  // States
  featured?: boolean
  bookmarked?: boolean
  liked?: boolean

  // Behavior
  clickable?: boolean
  hoverScale?: boolean
  showActions?: boolean
  actionsOnHover?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  contentType: 'generic',
  variant: 'grid',
  maxTags: 3,
  clickable: true,
  hoverScale: true,
  showActions: true,
  actionsOnHover: true
})

const emit = defineEmits<{
  click: [event: MouseEvent]
  bookmark: [event: MouseEvent]
  like: [event: MouseEvent]
  share: [event: MouseEvent]
  action: [type: string, event: MouseEvent]
}>()

// Card wrapper classes
const cardClasses = computed(() => {
  const base = [
    'content-card group relative bg-white rounded-2xl overflow-hidden transition-all duration-300 border'
  ]

  if (props.clickable) {
    base.push('cursor-pointer')
  }

  if (props.featured) {
    base.push('border-teal-200 shadow-md hover:shadow-lg hover:border-teal-300')
  } else {
    base.push('border-gray-100 shadow-sm hover:shadow-md hover:border-teal-200')
  }

  // Variant-specific classes
  if (props.variant === 'list') {
    base.push('flex gap-4 p-4')
  } else if (props.variant === 'compact') {
    base.push('flex gap-3 p-3')
  } else if (props.variant === 'featured') {
    base.push('min-h-[280px]')
  }

  return base.join(' ')
})

// Image container classes based on content type and variant
const imageContainerClasses = computed(() => {
  const base = ['relative overflow-hidden']

  if (props.variant === 'list') {
    base.push('w-32 h-24 flex-shrink-0 rounded-xl')
  } else if (props.variant === 'compact') {
    base.push('w-20 h-16 flex-shrink-0 rounded-lg')
  } else {
    // Grid variant - aspect ratio based on content type
    switch (props.contentType) {
      case 'article':
        base.push('aspect-[16/10]')
        break
      case 'document':
        base.push('aspect-[4/3]')
        break
      case 'media':
        base.push('aspect-video')
        break
      case 'event':
        base.push('aspect-[16/10]')
        break
      case 'course':
        base.push('aspect-video')
        break
      default:
        base.push('aspect-video')
    }
  }

  return base.join(' ')
})

// Gradient fallback based on content type
const fallbackGradient = computed(() => {
  switch (props.contentType) {
    case 'article':
      return 'from-teal-500 to-cyan-600'
    case 'document':
      return 'from-blue-500 to-indigo-600'
    case 'media':
      return 'from-purple-500 to-pink-600'
    case 'event':
      return 'from-orange-500 to-red-600'
    case 'course':
      return 'from-green-500 to-emerald-600'
    default:
      return 'from-gray-500 to-gray-600'
  }
})

// Default icon based on content type
const defaultIcon = computed(() => {
  if (props.icon) return props.icon
  switch (props.contentType) {
    case 'article': return 'fas fa-newspaper'
    case 'document': return 'fas fa-file-alt'
    case 'media': return 'fas fa-play-circle'
    case 'event': return 'fas fa-calendar-alt'
    case 'course': return 'fas fa-graduation-cap'
    default: return 'fas fa-file'
  }
})

// Format numbers
function formatNumber(num: number): string {
  if (num >= 1000000) return (num / 1000000).toFixed(1) + 'M'
  if (num >= 1000) return (num / 1000).toFixed(1) + 'K'
  return num.toString()
}

// Handle card click
function handleClick(event: MouseEvent) {
  if (props.clickable) {
    emit('click', event)
  }
}

// Handle action clicks
function handleAction(type: string, event: MouseEvent) {
  event.stopPropagation()
  emit('action', type, event)

  // Also emit specific events
  if (type === 'bookmark') emit('bookmark', event)
  if (type === 'like') emit('like', event)
  if (type === 'share') emit('share', event)
}
</script>

<template>
  <article :class="cardClasses" @click="handleClick">
    <!-- ========== GRID VARIANT ========== -->
    <template v-if="variant === 'grid' || variant === 'featured'">
      <!-- Image Section -->
      <div :class="imageContainerClasses">
        <!-- Image or Gradient Fallback -->
        <img
          v-if="image"
          :src="image"
          :alt="imageAlt || title"
          class="absolute inset-0 w-full h-full object-cover transition-transform duration-500"
          :class="{ 'group-hover:scale-105': hoverScale }"
        >
        <div
          v-else
          class="absolute inset-0 bg-gradient-to-br flex items-center justify-center"
          :class="fallbackGradient"
        >
          <i :class="[defaultIcon, 'text-white text-3xl opacity-80']"></i>
        </div>

        <!-- Dark overlay on hover -->
        <div class="absolute inset-0 bg-black/0 group-hover:bg-black/10 transition-colors"></div>

        <!-- Top Section: Badges & Actions -->
        <div class="absolute top-3 inset-x-3 flex items-start justify-between">
          <!-- Left: Badges -->
          <div class="flex flex-wrap gap-1.5">
            <slot name="header-badges">
              <StatusBadge v-if="featured" status="featured" size="xs" variant="gradient" />
              <StatusBadge v-if="status" :status="status" :label="statusLabel" size="xs" />
            </slot>
          </div>

          <!-- Right: Action Buttons -->
          <div
            v-if="showActions"
            class="flex items-center gap-1.5"
            :class="{ 'opacity-0 group-hover:opacity-100 transition-opacity': actionsOnHover }"
          >
            <slot name="header-actions">
              <!-- Default action buttons -->
              <button
                v-if="bookmarked !== undefined"
                @click="handleAction('bookmark', $event)"
                :class="[
                  'w-7 h-7 bg-white/90 backdrop-blur-sm rounded-full flex items-center justify-center transition-all shadow-sm',
                  bookmarked ? 'text-teal-600' : 'text-gray-400 hover:text-teal-600 hover:bg-white'
                ]"
                title="Bookmark"
              >
                <i :class="bookmarked ? 'fas fa-bookmark' : 'far fa-bookmark'" class="text-xs"></i>
              </button>
              <button
                @click="handleAction('share', $event)"
                class="w-7 h-7 bg-white/90 backdrop-blur-sm rounded-full flex items-center justify-center transition-all shadow-sm text-gray-400 hover:text-blue-500 hover:bg-white"
                title="Share"
              >
                <i class="fas fa-share-alt text-xs"></i>
              </button>
            </slot>
          </div>
        </div>

        <!-- Event Date Badge (for events) -->
        <div v-if="eventDate" class="absolute top-3 end-3 flex flex-col items-center bg-white rounded-lg shadow-lg px-2 py-1">
          <span class="text-[10px] font-bold text-gray-500 uppercase">{{ eventDate.month }}</span>
          <span class="text-lg font-bold text-gray-800 leading-none">{{ eventDate.day }}</span>
        </div>

        <!-- Bottom Section: Category Badge & Extras -->
        <div class="absolute bottom-3 inset-x-3 flex items-end justify-between">
          <slot name="image-footer-left">
            <CategoryBadge
              v-if="category"
              :category="category"
              :category-id="categoryId"
              :icon="categoryIcon"
              :color="categoryColor"
              size="xs"
            />
          </slot>

          <slot name="image-footer-right">
            <!-- Read time / Duration badge -->
            <span
              v-if="readTime || duration"
              class="px-2 py-0.5 rounded bg-black/70 text-white text-xs font-medium backdrop-blur-sm"
            >
              {{ readTime || duration }}
            </span>
          </slot>
        </div>

        <!-- Progress bar (for courses) -->
        <div v-if="progress !== undefined && progress > 0" class="absolute bottom-0 left-0 right-0 h-1 bg-gray-200">
          <div
            class="h-full bg-gradient-to-r from-teal-500 to-cyan-500 transition-all"
            :style="{ width: `${progress}%` }"
          ></div>
        </div>
      </div>

      <!-- Content Section -->
      <div class="p-4">
        <slot name="content">
          <!-- Meta Info -->
          <div v-if="date || readTime || eventTime" class="flex items-center gap-3 text-[11px] text-gray-400 mb-2">
            <span v-if="readTime && !duration" class="flex items-center gap-1">
              <i class="fas fa-clock text-[9px]"></i>
              {{ readTime }}
            </span>
            <span v-if="eventTime" class="flex items-center gap-1">
              <i class="fas fa-clock text-[9px]"></i>
              {{ eventTime }}
            </span>
            <span v-if="date" class="flex items-center gap-1">
              <i class="fas fa-calendar text-[9px]"></i>
              {{ date }}
            </span>
            <span v-if="eventLocation" class="flex items-center gap-1">
              <i :class="isVirtual ? 'fas fa-video' : 'fas fa-map-marker-alt'" class="text-[9px]"></i>
              {{ eventLocation }}
            </span>
          </div>

          <!-- Title -->
          <h3 class="text-sm font-semibold text-gray-800 mb-2 line-clamp-2 group-hover:text-teal-600 transition-colors leading-snug">
            {{ title }}
          </h3>

          <!-- Subtitle (for courses - instructor name) -->
          <p v-if="subtitle" class="text-xs text-gray-500 mb-2">
            {{ subtitle }}
          </p>

          <!-- Excerpt -->
          <p v-if="excerpt" class="text-xs text-gray-500 mb-3 line-clamp-2 leading-relaxed">
            {{ excerpt }}
          </p>

          <!-- Tags -->
          <div v-if="tags && tags.length > 0" class="flex flex-wrap gap-1.5 mb-3">
            <TagBadge
              v-for="tag in tags.slice(0, maxTags)"
              :key="tag"
              :tag="tag"
              size="xs"
              clickable
              class="!rounded-full"
            />
            <span v-if="tags.length > maxTags" class="text-[10px] text-gray-400 self-center">
              +{{ tags.length - maxTags }}
            </span>
          </div>

          <!-- Course Rating -->
          <div v-if="rating !== undefined" class="flex items-center gap-2 mb-3">
            <div class="flex items-center">
              <i v-for="i in 5" :key="i" :class="[
                'text-xs',
                i <= Math.round(rating) ? 'fas fa-star text-amber-400' : 'far fa-star text-gray-300'
              ]"></i>
            </div>
            <span class="text-xs text-gray-500">{{ rating.toFixed(1) }}</span>
            <span v-if="lessonsCount" class="text-xs text-gray-400">
              · {{ lessonsCount }} lessons
            </span>
          </div>
        </slot>

        <!-- Footer -->
        <slot name="footer">
          <div class="flex items-center justify-between pt-3 border-t border-gray-100">
            <!-- Author (for articles) -->
            <div v-if="author" class="flex items-center gap-2">
              <div
                v-if="author.avatar"
                class="w-7 h-7 rounded-full overflow-hidden"
              >
                <img :src="author.avatar" :alt="author.name" class="w-full h-full object-cover">
              </div>
              <div
                v-else
                class="w-7 h-7 rounded-full bg-gradient-to-br from-teal-400 to-cyan-500 flex items-center justify-center text-white text-[10px] font-semibold shadow-sm"
              >
                {{ author.initials || author.name.charAt(0) }}
              </div>
              <span class="text-xs text-gray-600 font-medium">{{ author.name }}</span>
            </div>

            <!-- Attendees (for events) -->
            <div v-else-if="attendees && attendees.length > 0" class="flex items-center gap-2">
              <div class="flex -space-x-2">
                <div
                  v-for="(attendee, i) in attendees.slice(0, 3)"
                  :key="i"
                  class="w-6 h-6 rounded-full flex items-center justify-center text-white text-[9px] font-semibold border-2 border-white"
                  :style="{ backgroundColor: attendee.color }"
                >
                  {{ attendee.initials }}
                </div>
              </div>
              <span v-if="attendeesCount" class="text-xs text-gray-500">
                {{ attendeesCount }} going
              </span>
            </div>

            <!-- File info (for documents) -->
            <div v-else-if="fileType || fileSize" class="flex items-center gap-2 text-xs text-gray-500">
              <span v-if="fileType" class="uppercase font-medium">{{ fileType }}</span>
              <span v-if="fileSize">{{ fileSize }}</span>
            </div>

            <!-- Empty spacer if no left content -->
            <div v-else></div>

            <!-- Stats -->
            <div v-if="stats" class="flex items-center gap-3 text-[11px] text-gray-400">
              <span v-if="stats.views !== undefined" class="flex items-center gap-1 hover:text-teal-500 transition-colors">
                <i class="fas fa-eye text-[9px]"></i>
                {{ formatNumber(stats.views) }}
              </span>
              <span v-if="stats.likes !== undefined" class="flex items-center gap-1 hover:text-red-400 transition-colors">
                <i class="fas fa-heart text-[9px]"></i>
                {{ formatNumber(stats.likes) }}
              </span>
              <span v-if="stats.downloads !== undefined" class="flex items-center gap-1 hover:text-blue-500 transition-colors">
                <i class="fas fa-download text-[9px]"></i>
                {{ formatNumber(stats.downloads) }}
              </span>
              <span v-if="stats.comments !== undefined" class="flex items-center gap-1 hover:text-purple-500 transition-colors">
                <i class="fas fa-comment text-[9px]"></i>
                {{ formatNumber(stats.comments) }}
              </span>
            </div>
          </div>
        </slot>
      </div>
    </template>

    <!-- ========== LIST VARIANT ========== -->
    <template v-else-if="variant === 'list'">
      <!-- Thumbnail -->
      <div :class="imageContainerClasses" class="bg-gradient-to-br from-teal-50 to-cyan-50">
        <img
          v-if="image"
          :src="image"
          :alt="imageAlt || title"
          class="w-full h-full object-cover transition-transform duration-300 group-hover:scale-110"
        >
        <div v-else class="absolute inset-0 flex items-center justify-center">
          <i :class="[defaultIcon, 'text-2xl', `text-${contentType === 'article' ? 'teal' : 'gray'}-300`]"></i>
        </div>
        <!-- Featured Star -->
        <div v-if="featured" class="absolute top-1.5 start-1.5">
          <span class="w-5 h-5 bg-amber-500 rounded-full flex items-center justify-center shadow-sm">
            <i class="fas fa-star text-white text-[8px]"></i>
          </span>
        </div>
      </div>

      <!-- Content -->
      <div class="flex-1 min-w-0">
        <slot name="content">
          <!-- Header Badges -->
          <div class="flex flex-wrap items-center gap-2 mb-1.5">
            <CategoryBadge
              v-if="category"
              :category="category"
              :category-id="categoryId"
              size="xs"
            />
            <StatusBadge v-if="status" :status="status" :label="statusLabel" size="xs" />
            <span v-if="readTime" class="text-[10px] text-gray-400 flex items-center gap-1">
              <i class="fas fa-clock text-[8px]"></i>
              {{ readTime }}
            </span>
          </div>

          <!-- Title -->
          <h3 class="text-sm font-semibold text-gray-800 mb-1 truncate group-hover:text-teal-600 transition-colors">
            {{ title }}
          </h3>

          <!-- Excerpt -->
          <p v-if="excerpt" class="text-xs text-gray-500 mb-2 line-clamp-1">
            {{ excerpt }}
          </p>

          <!-- Footer -->
          <div class="flex items-center justify-between">
            <!-- Author & Date -->
            <div v-if="author" class="flex items-center gap-2">
              <div
                class="w-6 h-6 rounded-full bg-gradient-to-br from-teal-400 to-cyan-500 flex items-center justify-center text-white text-[9px] font-semibold"
              >
                {{ author.initials || author.name.charAt(0) }}
              </div>
              <span class="text-[11px] text-gray-500">
                {{ author.name }}
                <span v-if="date" class="text-gray-300"> · </span>
                <span v-if="date">{{ date }}</span>
              </span>
            </div>

            <!-- Stats & Tags -->
            <div class="flex items-center gap-3">
              <div v-if="tags && tags.length > 0" class="hidden sm:flex items-center gap-1.5">
                <TagBadge
                  v-for="tag in tags.slice(0, 2)"
                  :key="tag"
                  :tag="tag"
                  size="xs"
                  class="!rounded-full"
                />
              </div>
              <div v-if="stats" class="flex items-center gap-2 text-[11px] text-gray-400">
                <span v-if="stats.views !== undefined" class="flex items-center gap-1">
                  <i class="fas fa-eye text-[9px]"></i>
                  {{ formatNumber(stats.views) }}
                </span>
                <span v-if="stats.likes !== undefined" class="flex items-center gap-1">
                  <i class="fas fa-heart text-[9px]"></i>
                  {{ formatNumber(stats.likes) }}
                </span>
              </div>
            </div>
          </div>
        </slot>
      </div>

      <!-- Actions Column -->
      <div v-if="showActions" class="flex flex-col gap-2">
        <slot name="actions">
          <div class="flex flex-col gap-2 opacity-0 group-hover:opacity-100 transition-opacity">
            <button
              v-if="bookmarked !== undefined"
              @click="handleAction('bookmark', $event)"
              :class="[
                'w-8 h-8 rounded-lg flex items-center justify-center transition-colors',
                bookmarked ? 'bg-teal-50 text-teal-600' : 'bg-gray-50 text-gray-400 hover:bg-teal-50 hover:text-teal-600'
              ]"
              title="Bookmark"
            >
              <i :class="bookmarked ? 'fas fa-bookmark' : 'far fa-bookmark'" class="text-xs"></i>
            </button>
            <button
              @click="handleAction('share', $event)"
              class="w-8 h-8 rounded-lg bg-gray-50 text-gray-400 hover:bg-teal-50 hover:text-teal-600 flex items-center justify-center transition-colors"
              title="Share"
            >
              <i class="fas fa-share-alt text-xs"></i>
            </button>
          </div>
        </slot>
      </div>
    </template>

    <!-- ========== COMPACT VARIANT ========== -->
    <template v-else-if="variant === 'compact'">
      <!-- Thumbnail -->
      <div :class="imageContainerClasses" class="bg-gray-100">
        <img
          v-if="image"
          :src="image"
          :alt="imageAlt || title"
          class="w-full h-full object-cover"
        >
        <div v-else class="absolute inset-0 flex items-center justify-center">
          <i :class="[defaultIcon, 'text-lg text-gray-300']"></i>
        </div>
      </div>

      <!-- Content -->
      <div class="flex-1 min-w-0 py-0.5">
        <h4 class="text-sm font-medium text-gray-800 truncate group-hover:text-teal-600 transition-colors">
          {{ title }}
        </h4>
        <p class="text-xs text-gray-500 truncate">
          <span v-if="category">{{ category }}</span>
          <span v-if="category && date" class="text-gray-300"> · </span>
          <span v-if="date">{{ date }}</span>
        </p>
      </div>
    </template>
  </article>
</template>

<style scoped>
.content-card {
  transition: all 0.3s ease;
}

.content-card:hover {
  transform: translateY(-2px);
}

/* Prevent transform on list variant */
.content-card.flex:hover {
  transform: none;
}

/* Action button hover effects */
.content-card button:hover:not(:disabled) {
  transform: translateY(-1px);
}

.content-card button:active:not(:disabled) {
  transform: translateY(0);
}
</style>
