<script setup lang="ts">
import { ref, computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { useSharing } from '@/composables/useSharing'

const { t } = useI18n()

const props = withDefaults(defineProps<{
  title: string
  description?: string
  url?: string
  layout?: 'horizontal' | 'vertical'
  size?: 'sm' | 'md' | 'lg'
  showLabels?: boolean
}>(), {
  layout: 'horizontal',
  size: 'md',
  showLabels: false
})

const { copyLink, shareToTwitter, shareToLinkedIn, shareToFacebook, shareViaEmail, copySuccess } = useSharing()

const shareData = {
  title: props.title,
  description: props.description,
  url: props.url || window.location.href
}

const shareOptions = computed(() => [
  {
    id: 'twitter',
    name: 'Twitter',
    icon: 'fab fa-twitter',
    color: 'hover:bg-sky-100 hover:text-sky-500',
    bgColor: 'bg-sky-500',
    action: () => shareToTwitter(shareData)
  },
  {
    id: 'linkedin',
    name: 'LinkedIn',
    icon: 'fab fa-linkedin-in',
    color: 'hover:bg-blue-100 hover:text-blue-600',
    bgColor: 'bg-blue-600',
    action: () => shareToLinkedIn(shareData)
  },
  {
    id: 'facebook',
    name: 'Facebook',
    icon: 'fab fa-facebook-f',
    color: 'hover:bg-indigo-100 hover:text-indigo-600',
    bgColor: 'bg-indigo-600',
    action: () => shareToFacebook(shareData)
  },
  {
    id: 'email',
    name: t('common.email'),
    icon: 'fas fa-envelope',
    color: 'hover:bg-gray-200 hover:text-gray-700',
    bgColor: 'bg-gray-600',
    action: () => shareViaEmail(shareData)
  }
])

const sizeClasses = {
  sm: 'w-8 h-8 text-sm',
  md: 'w-10 h-10 text-base',
  lg: 'w-12 h-12 text-lg'
}

function handleCopyLink() {
  copyLink(shareData.url)
}
</script>

<template>
  <div
    class="social-share-buttons flex items-center"
    :class="[
      layout === 'vertical' ? 'flex-col gap-2' : 'flex-row gap-3'
    ]"
  >
    <!-- Social Buttons -->
    <button
      v-for="option in shareOptions"
      :key="option.id"
      @click="option.action"
      :title="$t('common.shareOn', { name: option.name })"
      :class="[
        'rounded-full flex items-center justify-center transition-all duration-200 text-gray-500',
        sizeClasses[size],
        option.color
      ]"
    >
      <i :class="option.icon"></i>
      <span v-if="showLabels" class="ms-2 text-sm">{{ option.name }}</span>
    </button>

    <!-- Divider -->
    <div
      :class="[
        'bg-gray-200',
        layout === 'vertical' ? 'w-8 h-px' : 'w-px h-8'
      ]"
    ></div>

    <!-- Copy Link Button -->
    <button
      @click="handleCopyLink"
      :title="copySuccess ? $t('common.copied') : $t('common.copyLink')"
      :class="[
        'rounded-full flex items-center justify-center transition-all duration-200',
        sizeClasses[size],
        copySuccess
          ? 'bg-green-100 text-green-600'
          : 'text-gray-500 hover:bg-teal-100 hover:text-teal-600'
      ]"
    >
      <i :class="copySuccess ? 'fas fa-check' : 'fas fa-link'"></i>
      <span v-if="showLabels" class="ms-2 text-sm">{{ copySuccess ? $t('common.copied') : $t('common.copy') }}</span>
    </button>
  </div>
</template>

<style scoped>
.social-share-buttons button {
  border: 1px solid transparent;
}

.social-share-buttons button:hover {
  border-color: currentColor;
  transform: translateY(-2px);
}
</style>
