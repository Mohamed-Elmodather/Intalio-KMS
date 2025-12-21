<script setup lang="ts">
/**
 * KnowledgeCard Component
 *
 * A card component for displaying knowledge content snippets.
 * Ideal for documents, articles, notes, and other content items.
 *
 * @example
 * <KnowledgeCard
 *   title="Project Report Q4"
 *   description="Quarterly analysis of project performance and key metrics."
 *   type="document"
 *   :meta="{ author: 'John Doe', date: '2 hours ago' }"
 *   @click="handleClick"
 * />
 */
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'

export interface KnowledgeCardMeta {
  author?: string
  authorAvatar?: string
  date?: string
  views?: number
  comments?: number
  category?: string
}

export interface KnowledgeCardProps {
  /** Card title */
  title: string
  /** Arabic title for RTL */
  titleArabic?: string
  /** Card description/snippet */
  description?: string
  /** Content type */
  type?: 'document' | 'article' | 'note' | 'event' | 'discussion' | 'media'
  /** Custom icon (overrides type icon) */
  icon?: string
  /** Thumbnail image URL */
  thumbnail?: string
  /** Metadata information */
  meta?: KnowledgeCardMeta
  /** Show bookmark button */
  bookmarkable?: boolean
  /** Is bookmarked */
  bookmarked?: boolean
  /** Card status badge */
  status?: 'draft' | 'published' | 'archived' | 'pending'
  /** Loading state */
  loading?: boolean
  /** Compact variant */
  compact?: boolean
}

const props = withDefaults(defineProps<KnowledgeCardProps>(), {
  type: 'document',
  bookmarkable: false,
  bookmarked: false,
  loading: false,
  compact: false
})

const emit = defineEmits<{
  click: [event: MouseEvent]
  bookmark: [bookmarked: boolean]
}>()

const { locale } = useI18n()

const isRTL = computed(() => locale.value === 'ar')

const displayTitle = computed(() => {
  return isRTL.value && props.titleArabic ? props.titleArabic : props.title
})

// Type configuration
const typeConfig = computed(() => {
  const configs: Record<string, { icon: string; color: string; bgColor: string; label: string }> = {
    document: {
      icon: 'pi pi-file',
      color: '#3b82f6',
      bgColor: 'rgba(59, 130, 246, 0.1)',
      label: 'Document'
    },
    article: {
      icon: 'pi pi-file-edit',
      color: '#8b5cf6',
      bgColor: 'rgba(139, 92, 246, 0.1)',
      label: 'Article'
    },
    note: {
      icon: 'pi pi-bookmark',
      color: '#f59e0b',
      bgColor: 'rgba(245, 158, 11, 0.1)',
      label: 'Note'
    },
    event: {
      icon: 'pi pi-calendar',
      color: '#10b981',
      bgColor: 'rgba(16, 185, 129, 0.1)',
      label: 'Event'
    },
    discussion: {
      icon: 'pi pi-comments',
      color: '#ec4899',
      bgColor: 'rgba(236, 72, 153, 0.1)',
      label: 'Discussion'
    },
    media: {
      icon: 'pi pi-image',
      color: '#06b6d4',
      bgColor: 'rgba(6, 182, 212, 0.1)',
      label: 'Media'
    }
  }
  return configs[props.type] || configs.document
})

// Status configuration
const statusConfig = computed(() => {
  if (!props.status) return null

  const configs: Record<string, { color: string; bgColor: string; label: string }> = {
    draft: { color: '#64748b', bgColor: '#f1f5f9', label: 'Draft' },
    published: { color: '#10b981', bgColor: '#ecfdf5', label: 'Published' },
    archived: { color: '#94a3b8', bgColor: '#f8fafc', label: 'Archived' },
    pending: { color: '#f59e0b', bgColor: '#fffbeb', label: 'Pending' }
  }
  return configs[props.status]
})

function handleClick(event: MouseEvent) {
  if (!props.loading) {
    emit('click', event)
  }
}

function handleKeypress(event: KeyboardEvent) {
  if (!props.loading) {
    // Create a synthetic mouse event for consistency
    emit('click', event as unknown as MouseEvent)
  }
}

function handleBookmark(event: MouseEvent) {
  event.stopPropagation()
  emit('bookmark', !props.bookmarked)
}

const iconToUse = computed(() => props.icon || typeConfig.value.icon)
</script>

<template>
  <article
    class="knowledge-card"
    :class="{
      'knowledge-card--compact': compact,
      'knowledge-card--loading': loading,
      'knowledge-card--has-thumbnail': thumbnail
    }"
    @click="handleClick"
    role="button"
    tabindex="0"
    @keypress.enter="handleKeypress"
  >
    <!-- Loading Skeleton -->
    <template v-if="loading">
      <div class="knowledge-card__skeleton">
        <div class="skeleton-icon"></div>
        <div class="skeleton-content">
          <div class="skeleton-title"></div>
          <div class="skeleton-desc"></div>
          <div class="skeleton-meta"></div>
        </div>
      </div>
    </template>

    <!-- Content -->
    <template v-else>
      <!-- Thumbnail (if provided) -->
      <div v-if="thumbnail" class="knowledge-card__thumbnail">
        <img :src="thumbnail" :alt="displayTitle" />
        <div class="thumbnail-overlay"></div>
      </div>

      <!-- Body -->
      <div class="knowledge-card__body">
        <!-- Type Icon -->
        <div
          class="knowledge-card__icon"
          :style="{
            backgroundColor: typeConfig.bgColor,
            color: typeConfig.color
          }"
        >
          <i :class="iconToUse"></i>
        </div>

        <!-- Content -->
        <div class="knowledge-card__content">
          <!-- Header -->
          <div class="knowledge-card__header">
            <h3 class="knowledge-card__title">{{ displayTitle }}</h3>

            <!-- Status Badge -->
            <span
              v-if="statusConfig"
              class="knowledge-card__status"
              :style="{
                backgroundColor: statusConfig.bgColor,
                color: statusConfig.color
              }"
            >
              {{ statusConfig.label }}
            </span>
          </div>

          <!-- Description -->
          <p v-if="description && !compact" class="knowledge-card__description">
            {{ description }}
          </p>

          <!-- Meta -->
          <div v-if="meta" class="knowledge-card__meta">
            <!-- Author -->
            <span v-if="meta.author" class="meta-item">
              <i class="pi pi-user"></i>
              {{ meta.author }}
            </span>

            <!-- Date -->
            <span v-if="meta.date" class="meta-item">
              <i class="pi pi-clock"></i>
              {{ meta.date }}
            </span>

            <!-- Category -->
            <span v-if="meta.category" class="meta-item meta-item--category">
              {{ meta.category }}
            </span>

            <!-- Views -->
            <span v-if="meta.views !== undefined" class="meta-item">
              <i class="pi pi-eye"></i>
              {{ meta.views }}
            </span>

            <!-- Comments -->
            <span v-if="meta.comments !== undefined" class="meta-item">
              <i class="pi pi-comment"></i>
              {{ meta.comments }}
            </span>
          </div>
        </div>

        <!-- Actions -->
        <div class="knowledge-card__actions">
          <!-- Bookmark Button -->
          <button
            v-if="bookmarkable"
            type="button"
            class="action-btn bookmark-btn"
            :class="{ 'is-bookmarked': bookmarked }"
            @click="handleBookmark"
            :title="bookmarked ? 'Remove bookmark' : 'Add bookmark'"
          >
            <i :class="bookmarked ? 'pi pi-bookmark-fill' : 'pi pi-bookmark'"></i>
          </button>

          <!-- Chevron -->
          <i class="pi pi-chevron-right action-chevron"></i>
        </div>
      </div>
    </template>
  </article>
</template>

<style scoped lang="scss">
// Note: Tokens available via global injection from vite.config.ts
@use '@/design-system/mixins' as *;

.knowledge-card {
  @include card-base;
  display: flex;
  flex-direction: column;
  overflow: hidden;
  cursor: pointer;
  transition:
    box-shadow $duration-normal $ease-default,
    transform $duration-normal $ease-default,
    border-color $duration-fast $ease-default;

  &:hover {
    box-shadow: $shadow-card-hover;
    transform: translateY(-2px);
    border-color: $color-border-default;

    .knowledge-card__title {
      color: $color-brand-primary;
    }

    .action-chevron {
      opacity: 1;
      transform: translateX(0);
    }

    .thumbnail-overlay {
      opacity: 0.3;
    }
  }

  &:focus-visible {
    outline: none;
    box-shadow: $shadow-focus-ring;
  }

  &:active {
    transform: translateY(0);
  }

  // Loading state
  &--loading {
    pointer-events: none;
  }

  // Compact variant
  &--compact {
    .knowledge-card__body {
      padding: $spacing-3;
    }

    .knowledge-card__icon {
      width: 36px;
      height: 36px;

      i {
        font-size: 0.875rem;
      }
    }

    .knowledge-card__title {
      font-size: $font-size-sm;
    }

    .knowledge-card__meta {
      margin-top: $spacing-1;
    }
  }

  // Thumbnail
  &__thumbnail {
    position: relative;
    height: 140px;
    overflow: hidden;

    img {
      width: 100%;
      height: 100%;
      object-fit: cover;
      transition: transform $duration-slow $ease-out-expo;
    }

    .knowledge-card:hover & img {
      transform: scale(1.05);
    }
  }

  .thumbnail-overlay {
    position: absolute;
    inset: 0;
    background: linear-gradient(to bottom, transparent 50%, rgba(0, 0, 0, 0.6) 100%);
    opacity: 0.5;
    transition: opacity $duration-normal $ease-default;
  }

  // Body
  &__body {
    display: flex;
    align-items: flex-start;
    gap: $spacing-3;
    padding: $spacing-4;
    flex: 1;
  }

  // Icon
  &__icon {
    display: flex;
    align-items: center;
    justify-content: center;
    width: 44px;
    height: 44px;
    border-radius: $radius-lg;
    flex-shrink: 0;
    transition: transform $duration-fast $ease-default;

    i {
      font-size: 1.125rem;
    }

    .knowledge-card:hover & {
      transform: scale(1.05);
    }
  }

  // Content
  &__content {
    flex: 1;
    min-width: 0;
  }

  // Header
  &__header {
    display: flex;
    align-items: flex-start;
    gap: $spacing-2;
    margin-bottom: $spacing-1;
  }

  // Title
  &__title {
    flex: 1;
    font-size: $font-size-base;
    font-weight: $font-weight-semibold;
    color: $color-text-primary;
    margin: 0;
    line-height: 1.4;
    transition: color $duration-fast $ease-default;
    @include line-clamp(2);
  }

  // Status
  &__status {
    display: inline-flex;
    align-items: center;
    padding: 2px $spacing-2;
    font-size: $font-size-xs;
    font-weight: $font-weight-semibold;
    border-radius: $radius-full;
    white-space: nowrap;
    flex-shrink: 0;
  }

  // Description
  &__description {
    font-size: $font-size-sm;
    color: $color-text-secondary;
    line-height: 1.5;
    margin: 0 0 $spacing-2;
    @include line-clamp(2);
  }

  // Meta
  &__meta {
    display: flex;
    align-items: center;
    flex-wrap: wrap;
    gap: $spacing-3;
    margin-top: $spacing-2;
  }

  .meta-item {
    display: flex;
    align-items: center;
    gap: $spacing-1;
    font-size: $font-size-xs;
    color: $color-text-muted;

    i {
      font-size: 0.75rem;
    }

    &--category {
      padding: 2px $spacing-2;
      background: $color-bg-tertiary;
      border-radius: $radius-full;
      color: $color-text-secondary;
    }
  }

  // Actions
  &__actions {
    display: flex;
    align-items: center;
    gap: $spacing-2;
    flex-shrink: 0;
    margin-inline-start: auto;
  }

  .action-btn {
    display: flex;
    align-items: center;
    justify-content: center;
    width: 32px;
    height: 32px;
    padding: 0;
    background: transparent;
    border: none;
    border-radius: $radius-lg;
    color: $color-text-muted;
    cursor: pointer;
    transition:
      background-color $duration-fast $ease-default,
      color $duration-fast $ease-default;

    &:hover {
      background: $color-bg-tertiary;
      color: $color-text-primary;
    }

    &.is-bookmarked {
      color: $color-brand-primary;

      &:hover {
        background: $color-bg-brand-subtle;
      }
    }

    i {
      font-size: 0.875rem;
    }
  }

  .action-chevron {
    color: $color-text-muted;
    font-size: 0.75rem;
    opacity: 0;
    transform: translateX(-4px);
    transition:
      opacity $duration-fast $ease-default,
      transform $duration-fast $ease-default;
  }

  // Skeleton
  &__skeleton {
    display: flex;
    align-items: flex-start;
    gap: $spacing-3;
    padding: $spacing-4;
  }

  .skeleton-icon,
  .skeleton-title,
  .skeleton-desc,
  .skeleton-meta {
    background: linear-gradient(
      90deg,
      $color-bg-tertiary 0%,
      $color-bg-secondary 50%,
      $color-bg-tertiary 100%
    );
    background-size: 200% 100%;
    animation: shimmer 1.5s infinite;
    border-radius: $radius-md;
  }

  .skeleton-icon {
    width: 44px;
    height: 44px;
    border-radius: $radius-lg;
    flex-shrink: 0;
  }

  .skeleton-content {
    flex: 1;
    display: flex;
    flex-direction: column;
    gap: $spacing-2;
  }

  .skeleton-title {
    width: 70%;
    height: 20px;
  }

  .skeleton-desc {
    width: 100%;
    height: 16px;
  }

  .skeleton-meta {
    width: 50%;
    height: 14px;
  }
}

@keyframes shimmer {
  0% {
    background-position: 200% 0;
  }
  100% {
    background-position: -200% 0;
  }
}
</style>
