<script setup lang="ts">
/**
 * SkeletonCard Component
 *
 * Skeleton for card layouts with optional image, title, and text.
 *
 * @example
 * <SkeletonCard />
 * <SkeletonCard :show-image="false" :text-lines="2" />
 */
import SkeletonBase from './SkeletonBase.vue'
import SkeletonText from './SkeletonText.vue'
import SkeletonImage from './SkeletonImage.vue'

export interface SkeletonCardProps {
  /** Show image placeholder */
  showImage?: boolean
  /** Image aspect ratio */
  imageRatio?: string
  /** Show title placeholder */
  showTitle?: boolean
  /** Number of text lines */
  textLines?: number
  /** Show footer placeholder */
  showFooter?: boolean
  /** Animation type */
  animation?: 'shimmer' | 'pulse' | 'none'
}

withDefaults(defineProps<SkeletonCardProps>(), {
  showImage: true,
  imageRatio: '16/9',
  showTitle: true,
  textLines: 2,
  showFooter: false,
  animation: 'shimmer'
})
</script>

<template>
  <div class="skeleton-card">
    <!-- Image -->
    <SkeletonImage
      v-if="showImage"
      :aspect-ratio="imageRatio"
      :animation="animation"
      :rounded="false"
      class="skeleton-card__image"
    />

    <!-- Content -->
    <div class="skeleton-card__content">
      <!-- Title -->
      <SkeletonBase
        v-if="showTitle"
        height="1.25rem"
        width="70%"
        :animation="animation"
        rounded="sm"
        class="skeleton-card__title"
      />

      <!-- Text -->
      <SkeletonText
        v-if="textLines > 0"
        :lines="textLines"
        :animation="animation"
        class="skeleton-card__text"
      />
    </div>

    <!-- Footer -->
    <div v-if="showFooter" class="skeleton-card__footer">
      <SkeletonBase
        height="2rem"
        width="100%"
        :animation="animation"
        rounded="md"
      />
    </div>
  </div>
</template>

<style scoped lang="scss">
@use '@/design-system/mixins' as *;

.skeleton-card {
  background: white;
  border-radius: $radius-xl;
  border: 1px solid $slate-100;
  overflow: hidden;

  &__image {
    border-radius: 0;
  }

  &__content {
    padding: $spacing-5;
    display: flex;
    flex-direction: column;
    gap: $spacing-3;
  }

  &__title {
    margin-bottom: $spacing-1;
  }

  &__footer {
    padding: $spacing-4 $spacing-5;
    border-top: 1px solid $slate-100;
    background: $slate-50;
  }
}
</style>
