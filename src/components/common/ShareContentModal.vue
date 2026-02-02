<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import { useSharing } from '@/composables/useSharing'

const { t } = useI18n()
const { copyLink, shareToTwitter, shareToLinkedIn, shareToFacebook, shareViaEmail, copySuccess } = useSharing()

interface Props {
  modelValue: boolean
  title: string
  description?: string
  url?: string
  image?: string
  contentType?: string
}

const props = withDefaults(defineProps<Props>(), {
  contentType: 'content'
})

const emit = defineEmits<{
  'update:modelValue': [value: boolean]
}>()

const showQR = ref(false)

const isOpen = computed({
  get: () => props.modelValue,
  set: (value) => emit('update:modelValue', value)
})

const shareUrl = computed(() => props.url || window.location.href)

const shareData = computed(() => ({
  title: props.title,
  description: props.description,
  url: shareUrl.value
}))

// QR Code URL using a free API
const qrCodeUrl = computed(() => {
  return `https://api.qrserver.com/v1/create-qr-code/?size=200x200&data=${encodeURIComponent(shareUrl.value)}`
})

const shareOptions = computed(() => [
  {
    id: 'twitter',
    name: 'Twitter',
    icon: 'fab fa-x-twitter',
    bgColor: 'bg-black',
    hoverColor: 'hover:bg-gray-800',
    action: () => shareToTwitter(shareData.value)
  },
  {
    id: 'linkedin',
    name: 'LinkedIn',
    icon: 'fab fa-linkedin-in',
    bgColor: 'bg-blue-600',
    hoverColor: 'hover:bg-blue-700',
    action: () => shareToLinkedIn(shareData.value)
  },
  {
    id: 'facebook',
    name: 'Facebook',
    icon: 'fab fa-facebook-f',
    bgColor: 'bg-indigo-600',
    hoverColor: 'hover:bg-indigo-700',
    action: () => shareToFacebook(shareData.value)
  },
  {
    id: 'whatsapp',
    name: 'WhatsApp',
    icon: 'fab fa-whatsapp',
    bgColor: 'bg-green-500',
    hoverColor: 'hover:bg-green-600',
    action: () => shareToWhatsApp()
  },
  {
    id: 'email',
    name: t('common.email'),
    icon: 'fas fa-envelope',
    bgColor: 'bg-gray-600',
    hoverColor: 'hover:bg-gray-700',
    action: () => shareViaEmail(shareData.value)
  }
])

function close() {
  isOpen.value = false
  showQR.value = false
}

function handleCopyLink() {
  copyLink(shareUrl.value)
}

function shareToWhatsApp() {
  const text = encodeURIComponent(`${props.title}\n\n${shareUrl.value}`)
  window.open(`https://wa.me/?text=${text}`, '_blank')
}

function toggleQR() {
  showQR.value = !showQR.value
}

// Reset QR view when modal closes
watch(isOpen, (value) => {
  if (!value) {
    showQR.value = false
  }
})
</script>

<template>
  <Teleport to="body">
    <Transition name="modal">
      <div
        v-if="isOpen"
        class="fixed inset-0 bg-black/50 backdrop-blur-sm flex items-center justify-center z-50 p-4"
        @click.self="close"
      >
        <div class="modal-content bg-white rounded-2xl max-w-md w-full shadow-2xl overflow-hidden">
          <!-- Header -->
          <div class="bg-gradient-to-r from-teal-500 to-cyan-500 px-6 py-4">
            <div class="flex items-center justify-between">
              <h3 class="text-lg font-semibold text-white flex items-center gap-2">
                <i class="fas fa-share-alt"></i>
                {{ $t('common.share') }} {{ contentType }}
              </h3>
              <button
                @click="close"
                class="w-8 h-8 rounded-full bg-white/20 hover:bg-white/30 flex items-center justify-center transition-colors"
              >
                <i class="fas fa-times text-white"></i>
              </button>
            </div>
          </div>

          <!-- Content Preview -->
          <div class="px-6 pt-5 pb-4">
            <div class="flex gap-4 mb-5">
              <!-- Thumbnail -->
              <div v-if="image" class="flex-shrink-0">
                <img :src="image" :alt="title" class="w-16 h-16 rounded-lg object-cover" />
              </div>
              <div v-else class="flex-shrink-0 w-16 h-16 rounded-lg bg-gradient-to-br from-teal-100 to-cyan-100 flex items-center justify-center">
                <i class="fas fa-share-nodes text-2xl text-teal-500"></i>
              </div>
              <!-- Title & Description -->
              <div class="flex-1 min-w-0">
                <h4 class="font-semibold text-gray-900 text-sm line-clamp-2 mb-1">{{ title }}</h4>
                <p v-if="description" class="text-xs text-gray-500 line-clamp-2">{{ description }}</p>
              </div>
            </div>

            <!-- QR Code Section -->
            <Transition name="fade">
              <div v-if="showQR" class="mb-5 flex flex-col items-center">
                <div class="bg-white p-3 rounded-xl border-2 border-gray-100 shadow-inner">
                  <img :src="qrCodeUrl" alt="QR Code" class="w-40 h-40" />
                </div>
                <p class="text-xs text-gray-500 mt-2">{{ $t('common.scanToShare') || 'Scan to share' }}</p>
              </div>
            </Transition>

            <!-- Social Share Buttons -->
            <div class="mb-5">
              <p class="text-xs font-medium text-gray-500 uppercase tracking-wide mb-3">{{ $t('common.shareVia') || 'Share via' }}</p>
              <div class="flex justify-center gap-3">
                <button
                  v-for="option in shareOptions"
                  :key="option.id"
                  @click="option.action"
                  :title="option.name"
                  :class="[
                    'w-11 h-11 rounded-full flex items-center justify-center text-white transition-all duration-200 shadow-md',
                    option.bgColor,
                    option.hoverColor
                  ]"
                >
                  <i :class="[option.icon, 'text-lg']"></i>
                </button>
              </div>
            </div>

            <!-- Copy Link Section -->
            <div class="bg-gray-50 rounded-xl p-3">
              <p class="text-xs font-medium text-gray-500 uppercase tracking-wide mb-2">{{ $t('common.copyLink') || 'Copy Link' }}</p>
              <div class="flex gap-2">
                <div class="flex-1 bg-white rounded-lg border border-gray-200 px-3 py-2 text-sm text-gray-600 truncate">
                  {{ shareUrl }}
                </div>
                <button
                  @click="handleCopyLink"
                  :class="[
                    'px-4 py-2 rounded-lg font-medium text-sm transition-all duration-200 flex items-center gap-2',
                    copySuccess
                      ? 'bg-green-500 text-white'
                      : 'bg-teal-500 text-white hover:bg-teal-600'
                  ]"
                >
                  <i :class="copySuccess ? 'fas fa-check' : 'fas fa-copy'"></i>
                  {{ copySuccess ? $t('common.copied') : $t('common.copy') }}
                </button>
              </div>
            </div>
          </div>

          <!-- Footer with QR Toggle -->
          <div class="px-6 py-4 bg-gray-50 border-t border-gray-100 flex justify-between items-center">
            <button
              @click="toggleQR"
              class="text-sm text-gray-600 hover:text-teal-600 flex items-center gap-2 transition-colors"
            >
              <i :class="showQR ? 'fas fa-chevron-up' : 'fas fa-qrcode'"></i>
              {{ showQR ? $t('common.hideQR') || 'Hide QR Code' : $t('common.showQR') || 'Show QR Code' }}
            </button>
            <button
              @click="close"
              class="px-4 py-2 text-sm font-medium text-gray-600 hover:text-gray-800 transition-colors"
            >
              {{ $t('common.close') }}
            </button>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<style scoped>
.modal-enter-active,
.modal-leave-active {
  transition: opacity 0.3s ease;
}

.modal-enter-active .modal-content,
.modal-leave-active .modal-content {
  transition: transform 0.3s ease;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}

.modal-enter-from .modal-content,
.modal-leave-to .modal-content {
  transform: scale(0.95) translateY(10px);
}

.fade-enter-active,
.fade-leave-active {
  transition: all 0.2s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
  transform: translateY(-10px);
}
</style>
