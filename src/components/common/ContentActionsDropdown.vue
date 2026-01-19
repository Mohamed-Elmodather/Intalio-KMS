<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

interface Props {
  showDownload?: boolean
  showCopyLink?: boolean
  showReport?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  showDownload: true,
  showCopyLink: true,
  showReport: true
})

const emit = defineEmits<{
  addToCollection: []
  share: []
  download: []
  copyLink: []
  report: []
}>()

const showDropdown = ref(false)
const dropdownRef = ref<HTMLElement | null>(null)

function toggleDropdown() {
  showDropdown.value = !showDropdown.value
}

function handleAction(action: string) {
  showDropdown.value = false
  switch (action) {
    case 'addToCollection':
      emit('addToCollection')
      break
    case 'share':
      emit('share')
      break
    case 'download':
      emit('download')
      break
    case 'copyLink':
      emit('copyLink')
      break
    case 'report':
      emit('report')
      break
  }
}

function handleClickOutside(event: MouseEvent) {
  if (dropdownRef.value && !dropdownRef.value.contains(event.target as Node)) {
    showDropdown.value = false
  }
}

onMounted(() => {
  document.addEventListener('click', handleClickOutside)
})

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside)
})
</script>

<template>
  <div ref="dropdownRef" class="content-actions-dropdown">
    <button
      @click.stop="toggleDropdown"
      class="trigger-btn"
      :class="{ 'active': showDropdown }"
      :title="$t('common.moreActions')"
    >
      <i class="fas fa-ellipsis-v"></i>
    </button>

    <Transition name="dropdown">
      <div v-if="showDropdown" class="dropdown-menu">
        <button @click.stop="handleAction('addToCollection')" class="dropdown-item">
          <i class="fas fa-layer-group"></i>
          <span>{{ $t('collections.addToCollection') }}</span>
        </button>
        <button @click.stop="handleAction('share')" class="dropdown-item">
          <i class="fas fa-share-alt"></i>
          <span>{{ $t('common.share') }}</span>
        </button>
        <button v-if="showDownload" @click.stop="handleAction('download')" class="dropdown-item">
          <i class="fas fa-download"></i>
          <span>{{ $t('common.download') }}</span>
        </button>
        <button v-if="showCopyLink" @click.stop="handleAction('copyLink')" class="dropdown-item">
          <i class="fas fa-link"></i>
          <span>{{ $t('common.copyLink') }}</span>
        </button>
        <div v-if="showReport" class="dropdown-divider"></div>
        <button v-if="showReport" @click.stop="handleAction('report')" class="dropdown-item danger">
          <i class="fas fa-flag"></i>
          <span>{{ $t('common.report') }}</span>
        </button>
      </div>
    </Transition>
  </div>
</template>

<style scoped>
.content-actions-dropdown {
  position: relative;
}

.trigger-btn {
  width: 32px;
  height: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(255, 255, 255, 0.95);
  border: none;
  border-radius: 50%;
  color: #64748b;
  font-size: 0.75rem;
  cursor: pointer;
  transition: all 0.2s;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.trigger-btn:hover,
.trigger-btn.active {
  background: #14b8a6;
  color: white;
  transform: scale(1.1);
}

.dropdown-menu {
  position: absolute;
  top: calc(100% + 4px);
  right: 0;
  min-width: 180px;
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 12px;
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.12);
  z-index: 100;
  overflow: hidden;
  padding: 0.375rem;
}

.dropdown-item {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  width: 100%;
  padding: 0.625rem 0.75rem;
  font-size: 0.8125rem;
  font-weight: 500;
  color: #475569;
  background: transparent;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.15s;
  text-align: left;
}

.dropdown-item:hover {
  background: #f0fdfa;
  color: #0f766e;
}

.dropdown-item i {
  width: 16px;
  text-align: center;
  font-size: 0.875rem;
}

.dropdown-item.danger {
  color: #dc2626;
}

.dropdown-item.danger:hover {
  background: #fef2f2;
  color: #b91c1c;
}

.dropdown-divider {
  height: 1px;
  background: #f1f5f9;
  margin: 0.375rem 0;
}

/* Dropdown Transition */
.dropdown-enter-active,
.dropdown-leave-active {
  transition: all 0.2s ease;
}

.dropdown-enter-from,
.dropdown-leave-to {
  opacity: 0;
  transform: translateY(-8px) scale(0.95);
}
</style>
