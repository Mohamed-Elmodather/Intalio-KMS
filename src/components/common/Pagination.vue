<script setup lang="ts">
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

interface Props {
  currentPage: number
  totalItems: number
  itemsPerPage: number
  itemsPerPageOptions?: number[]
  showInfo?: boolean
  showPerPageSelector?: boolean
  maxVisiblePages?: number
}

const props = withDefaults(defineProps<Props>(), {
  itemsPerPageOptions: () => [10, 20, 50, 100],
  showInfo: true,
  showPerPageSelector: true,
  maxVisiblePages: 5
})

const emit = defineEmits<{
  'update:currentPage': [page: number]
  'update:itemsPerPage': [perPage: number]
}>()

// Computed properties
const totalPages = computed(() => Math.ceil(props.totalItems / props.itemsPerPage) || 1)

const startItem = computed(() => {
  if (props.totalItems === 0) return 0
  return (props.currentPage - 1) * props.itemsPerPage + 1
})

const endItem = computed(() => Math.min(props.currentPage * props.itemsPerPage, props.totalItems))

const visiblePages = computed(() => {
  const pages: (number | 'ellipsis')[] = []
  const total = totalPages.value
  const current = props.currentPage

  if (total <= 7) {
    // Show all pages if 7 or fewer
    for (let i = 1; i <= total; i++) {
      pages.push(i)
    }
  } else {
    // Always show first page
    pages.push(1)

    if (current > 3) {
      pages.push('ellipsis')
    }

    // Show pages around current
    const start = Math.max(2, current - 1)
    const end = Math.min(total - 1, current + 1)

    for (let i = start; i <= end; i++) {
      pages.push(i)
    }

    if (current < total - 2) {
      pages.push('ellipsis')
    }

    // Always show last page
    if (total > 1) {
      pages.push(total)
    }
  }

  return pages
})

const isFirstPage = computed(() => props.currentPage === 1)
const isLastPage = computed(() => props.currentPage === totalPages.value)

// Methods
function goToPage(page: number) {
  if (page >= 1 && page <= totalPages.value && page !== props.currentPage) {
    emit('update:currentPage', page)
  }
}

function prevPage() {
  if (!isFirstPage.value) {
    emit('update:currentPage', props.currentPage - 1)
  }
}

function nextPage() {
  if (!isLastPage.value) {
    emit('update:currentPage', props.currentPage + 1)
  }
}

function changePerPage(event: Event) {
  const value = Number((event.target as HTMLSelectElement).value)
  emit('update:itemsPerPage', value)
  // Reset to first page when changing items per page
  emit('update:currentPage', 1)
}
</script>

<template>
  <div class="pagination">
    <div class="pagination__content">
      <!-- Left: Stats & Items Per Page -->
      <div class="pagination__left">
        <!-- Info -->
        <span v-if="showInfo && totalItems > 0" class="pagination__info">
          {{ t('common.showing') }} {{ startItem }}-{{ endItem }} {{ t('common.of') }} {{ totalItems }}
        </span>

        <!-- Items Per Page Selector -->
        <div v-if="showPerPageSelector" class="pagination__per-page">
          <span class="pagination__per-page-label">{{ t('common.show') }}:</span>
          <select
            :value="itemsPerPage"
            @change="changePerPage"
            class="pagination__select"
          >
            <option v-for="option in itemsPerPageOptions" :key="option" :value="option">
              {{ option }}
            </option>
          </select>
          <span class="pagination__per-page-label">{{ t('common.perPage') || 'per page' }}</span>
        </div>
      </div>

      <!-- Right: Pagination Controls -->
      <div class="pagination__controls">
        <!-- Previous Button -->
        <button
          @click="prevPage"
          :disabled="isFirstPage"
          :class="['pagination__btn', { 'pagination__btn--disabled': isFirstPage }]"
        >
          <i class="fas fa-chevron-left pagination__btn-icon"></i>
          <span class="pagination__btn-text">{{ t('common.previous') || 'Previous' }}</span>
        </button>

        <!-- Page Numbers -->
        <div class="pagination__pages">
          <template v-for="(page, index) in visiblePages" :key="index">
            <span v-if="page === 'ellipsis'" class="pagination__ellipsis">...</span>
            <button
              v-else
              @click="goToPage(page)"
              :class="['pagination__page', { 'pagination__page--active': page === currentPage }]"
            >
              {{ page }}
            </button>
          </template>
        </div>

        <!-- Next Button -->
        <button
          @click="nextPage"
          :disabled="isLastPage"
          :class="['pagination__btn', { 'pagination__btn--disabled': isLastPage }]"
        >
          <span class="pagination__btn-text">{{ t('common.next') || 'Next' }}</span>
          <i class="fas fa-chevron-right pagination__btn-icon"></i>
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.pagination {
  padding: 12px 16px;
  background: white;
  border-radius: 16px;
  border: 1px solid #f1f5f9;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.04);
}

.pagination__content {
  display: flex;
  align-items: center;
  justify-content: space-between;
  flex-wrap: wrap;
  gap: 12px;
}

.pagination__left {
  display: flex;
  align-items: center;
  gap: 16px;
  flex-wrap: wrap;
}

.pagination__info {
  font-size: 12px;
  color: #64748b;
}

.pagination__per-page {
  display: flex;
  align-items: center;
  gap: 8px;
}

.pagination__per-page-label {
  font-size: 12px;
  color: #64748b;
}

.pagination__select {
  font-size: 12px;
  padding: 6px 8px;
  border-radius: 8px;
  border: 1px solid #e2e8f0;
  background: white;
  color: #374151;
  cursor: pointer;
  outline: none;
  transition: all 0.2s ease;
}

.pagination__select:focus {
  border-color: #14b8a6;
  box-shadow: 0 0 0 2px rgba(20, 184, 166, 0.1);
}

.pagination__controls {
  display: flex;
  align-items: center;
  gap: 8px;
}

.pagination__btn {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 6px 12px;
  font-size: 12px;
  font-weight: 500;
  color: #64748b;
  background: white;
  border: 1px solid #e2e8f0;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.2s ease;
}

.pagination__btn:hover:not(.pagination__btn--disabled) {
  color: #374151;
  background: #f8fafc;
  border-color: #cbd5e1;
}

.pagination__btn--disabled {
  color: #cbd5e1;
  border-color: #f1f5f9;
  cursor: not-allowed;
}

.pagination__btn-icon {
  font-size: 10px;
}

.pagination__btn-text {
  display: none;
}

@media (min-width: 640px) {
  .pagination__btn-text {
    display: inline;
  }
}

.pagination__pages {
  display: flex;
  align-items: center;
  gap: 4px;
}

.pagination__page {
  width: 32px;
  height: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 12px;
  font-weight: 500;
  color: #64748b;
  background: transparent;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.2s ease;
}

.pagination__page:hover:not(.pagination__page--active) {
  color: #374151;
  background: #f1f5f9;
}

.pagination__page--active {
  color: #0d9488;
  background: #f0fdfa;
}

.pagination__ellipsis {
  padding: 0 4px;
  font-size: 12px;
  color: #94a3b8;
}

/* Responsive adjustments */
@media (max-width: 640px) {
  .pagination__content {
    justify-content: center;
  }

  .pagination__left {
    width: 100%;
    justify-content: center;
  }

  .pagination__info {
    display: none;
  }

  .pagination__per-page {
    display: none;
  }

  .pagination__controls {
    width: 100%;
    justify-content: center;
  }
}
</style>
