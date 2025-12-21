<script setup lang="ts">
/**
 * Avatar Component
 *
 * A flexible avatar component with image, initials fallback, and status indicator.
 * Supports multiple sizes, shapes, and group layouts.
 *
 * @example
 * <Avatar image="/path/to/photo.jpg" name="John Doe" size="lg" />
 *
 * @example
 * <Avatar name="Jane Smith" status="online" />
 *
 * @example
 * <Avatar icon="pi-user" shape="square" />
 */
import { computed, ref } from 'vue'
import { useI18n } from 'vue-i18n'

export interface AvatarProps {
  /** Image URL */
  image?: string
  /** User's name (for initials fallback and alt text) */
  name?: string
  /** Size variant */
  size?: 'xs' | 'sm' | 'md' | 'lg' | 'xl' | '2xl'
  /** Shape variant */
  shape?: 'circle' | 'square'
  /** Status indicator */
  status?: 'online' | 'offline' | 'away' | 'busy' | 'none'
  /** Show border around avatar */
  bordered?: boolean
  /** Custom background color */
  bgColor?: string
  /** Custom text color */
  textColor?: string
  /** Icon class (when no image or name) */
  icon?: string
  /** Loading state */
  loading?: boolean
  /** Show initials only (ignore image) */
  initialsOnly?: boolean
}

const props = withDefaults(defineProps<AvatarProps>(), {
  size: 'md',
  shape: 'circle',
  status: 'none',
  bordered: false,
  loading: false,
  initialsOnly: false
})

const { locale } = useI18n()

const imageError = ref(false)

const isRTL = computed(() => locale.value === 'ar')

const showImage = computed(() => {
  return props.image && !imageError.value && !props.initialsOnly
})

const showInitials = computed(() => {
  return !showImage.value && props.name
})

const showIcon = computed(() => {
  return !showImage.value && !showInitials.value
})

const initials = computed(() => {
  if (!props.name) return ''

  const parts = props.name.trim().split(/\s+/)
  if (parts.length === 1) {
    return parts[0].charAt(0).toUpperCase()
  }
  return (parts[0].charAt(0) + parts[parts.length - 1].charAt(0)).toUpperCase()
})

const altText = computed(() => {
  if (props.name) {
    return isRTL.value ? `صورة ${props.name}` : `${props.name}'s avatar`
  }
  return isRTL.value ? 'صورة المستخدم' : 'User avatar'
})

// Generate consistent color from name
const generatedBgColor = computed(() => {
  if (props.bgColor) return props.bgColor
  if (!props.name) return undefined

  const colors = [
    '#0891B2', // cyan-600
    '#0D9488', // teal-600
    '#059669', // emerald-600
    '#16A34A', // green-600
    '#CA8A04', // yellow-600
    '#EA580C', // orange-600
    '#DC2626', // red-600
    '#DB2777', // pink-600
    '#9333EA', // purple-600
    '#4F46E5', // indigo-600
    '#2563EB', // blue-600
  ]

  let hash = 0
  for (let i = 0; i < props.name.length; i++) {
    hash = props.name.charCodeAt(i) + ((hash << 5) - hash)
  }
  return colors[Math.abs(hash) % colors.length]
})

const handleImageError = () => {
  imageError.value = true
}
</script>

<template>
  <div
    class="avatar"
    :class="[
      `avatar--${size}`,
      `avatar--${shape}`,
      {
        'avatar--bordered': bordered,
        'avatar--loading': loading,
        'avatar--has-status': status !== 'none'
      }
    ]"
    :style="{
      '--avatar-bg': generatedBgColor,
      '--avatar-text': textColor
    }"
  >
    <!-- Image -->
    <img
      v-if="showImage"
      :src="image"
      :alt="altText"
      class="avatar__image"
      @error="handleImageError"
    />

    <!-- Initials Fallback -->
    <span v-else-if="showInitials" class="avatar__initials">
      {{ initials }}
    </span>

    <!-- Icon Fallback -->
    <i
      v-else-if="showIcon"
      :class="['pi', icon || 'pi-user', 'avatar__icon']"
      aria-hidden="true"
    />

    <!-- Loading Skeleton -->
    <div v-if="loading" class="avatar__loading" />

    <!-- Status Indicator -->
    <span
      v-if="status !== 'none'"
      :class="['avatar__status', `avatar__status--${status}`]"
      :aria-label="status"
    />
  </div>
</template>

<style scoped lang="scss">
// Note: Tokens available via global injection from vite.config.ts
@use '@/design-system/mixins' as *;

.avatar {
  position: relative;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  background: var(--avatar-bg, #{$gray-200});
  color: var(--avatar-text, white);
  font-weight: $font-weight-semibold;
  overflow: hidden;
  user-select: none;

  // Sizes
  &--xs {
    --avatar-size: 24px;
    --avatar-font: #{$font-size-xs};
    --avatar-icon: 12px;
    --status-size: 6px;
    --status-offset: 0px;
  }

  &--sm {
    --avatar-size: 32px;
    --avatar-font: #{$font-size-sm};
    --avatar-icon: 14px;
    --status-size: 8px;
    --status-offset: 1px;
  }

  &--md {
    --avatar-size: 40px;
    --avatar-font: #{$font-size-base};
    --avatar-icon: 18px;
    --status-size: 10px;
    --status-offset: 2px;
  }

  &--lg {
    --avatar-size: 48px;
    --avatar-font: #{$font-size-lg};
    --avatar-icon: 20px;
    --status-size: 12px;
    --status-offset: 2px;
  }

  &--xl {
    --avatar-size: 64px;
    --avatar-font: #{$font-size-xl};
    --avatar-icon: 24px;
    --status-size: 14px;
    --status-offset: 3px;
  }

  &--2xl {
    --avatar-size: 80px;
    --avatar-font: #{$font-size-2xl};
    --avatar-icon: 32px;
    --status-size: 16px;
    --status-offset: 4px;
  }

  width: var(--avatar-size);
  height: var(--avatar-size);
  font-size: var(--avatar-font);

  // Shapes
  &--circle {
    border-radius: $radius-full;
  }

  &--square {
    border-radius: $radius-lg;
  }

  // Border
  &--bordered {
    border: 2px solid white;
    box-shadow: 0 0 0 1px $gray-200;
  }

  // ================================
  // Elements
  // ================================

  &__image {
    width: 100%;
    height: 100%;
    object-fit: cover;
  }

  &__initials {
    display: flex;
    align-items: center;
    justify-content: center;
    width: 100%;
    height: 100%;
    text-transform: uppercase;
    letter-spacing: 0.02em;
  }

  &__icon {
    font-size: var(--avatar-icon);
    color: var(--avatar-text, #{$gray-500});
  }

  // Loading skeleton
  &__loading {
    position: absolute;
    inset: 0;
    background: linear-gradient(
      90deg,
      $gray-200 0%,
      $gray-300 50%,
      $gray-200 100%
    );
    background-size: 200% 100%;
    animation: avatarShimmer 1.5s infinite;
  }

  // Status indicator
  &__status {
    position: absolute;
    bottom: var(--status-offset);
    right: var(--status-offset);
    width: var(--status-size);
    height: var(--status-size);
    border-radius: 50%;
    border: 2px solid white;

    &--online {
      background: $success-500;
    }

    &--offline {
      background: $gray-400;
    }

    &--away {
      background: $warning-500;
    }

    &--busy {
      background: $error-500;
    }
  }
}

// ================================
// Avatar Group (utility class)
// ================================

:global(.avatar-group) {
  display: inline-flex;
  flex-direction: row-reverse;

  .avatar {
    margin-inline-start: -$spacing-2;
    border: 2px solid white;
    transition: transform $duration-fast $ease-default;

    &:hover {
      transform: translateY(-2px);
      z-index: 1;
    }

    &:last-child {
      margin-inline-start: 0;
    }
  }

  // RTL support
  &[dir="rtl"],
  .rtl & {
    flex-direction: row;

    .avatar {
      margin-inline-start: 0;
      margin-inline-end: -$spacing-2;

      &:last-child {
        margin-inline-end: 0;
      }
    }
  }
}

// ================================
// Keyframe Animations
// ================================

@keyframes avatarShimmer {
  0% {
    background-position: 200% 0;
  }
  100% {
    background-position: -200% 0;
  }
}

// ================================
// Reduced Motion
// ================================

@media (prefers-reduced-motion: reduce) {
  .avatar {
    &__loading {
      animation: none !important;
    }
  }

  :global(.avatar-group) .avatar:hover {
    transform: none !important;
  }
}
</style>
