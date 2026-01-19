<script setup lang="ts">
import { computed } from 'vue'
import { useI18n } from 'vue-i18n'
import { useComparisonStore } from '@/stores/comparison'

const { t } = useI18n()
const comparisonStore = useComparisonStore()

// Type icons mapping
const typeIcons: Record<string, string> = {
  document: 'fas fa-file-alt',
  article: 'fas fa-newspaper',
  media: 'fas fa-photo-video',
  event: 'fas fa-calendar-alt',
}

const typeColors: Record<string, string> = {
  document: 'bg-blue-100 text-blue-600',
  article: 'bg-purple-100 text-purple-600',
  media: 'bg-pink-100 text-pink-600',
  event: 'bg-orange-100 text-orange-600',
}

const displayedItems = computed(() => comparisonStore.selectedItems.slice(0, 5))
const extraCount = computed(() => Math.max(0, comparisonStore.selectedItems.length - 5))
</script>

<template>
  <Teleport to="body">
    <Transition name="slide-up">
      <div
        v-if="comparisonStore.hasItems"
        class="fixed bottom-6 right-6 z-50"
      >
        <!-- Minimized State -->
        <div
          v-if="comparisonStore.isPanelMinimized"
          class="bg-gray-900 text-white rounded-full px-4 py-3 shadow-2xl cursor-pointer hover:bg-gray-800 transition-colors flex items-center gap-2"
          @click="comparisonStore.togglePanelMinimized()"
        >
          <i class="fas fa-layer-group" />
          <span class="font-medium">{{ comparisonStore.itemCount }}</span>
          <i class="fas fa-chevron-up text-xs opacity-60" />
        </div>

        <!-- Expanded Panel -->
        <div
          v-else
          class="bg-white rounded-2xl shadow-2xl border border-gray-200 overflow-hidden w-80"
        >
          <!-- Header -->
          <div class="bg-gradient-to-r from-teal-500 to-teal-600 text-white px-4 py-3 flex items-center justify-between">
            <div class="flex items-center gap-2">
              <i class="fas fa-layer-group" />
              <span class="font-semibold">{{ $t('comparison.compare') }}</span>
              <span class="bg-white/20 rounded-full px-2 py-0.5 text-sm">
                {{ $t('comparison.itemsCount', { count: comparisonStore.itemCount }) }}
              </span>
            </div>
            <button
              class="p-1 hover:bg-white/20 rounded transition-colors"
              @click="comparisonStore.togglePanelMinimized()"
            >
              <i class="fas fa-minus" />
            </button>
          </div>

          <!-- Selected Items Preview -->
          <div class="p-3 space-y-2 max-h-64 overflow-y-auto">
            <div
              v-for="item in displayedItems"
              :key="item.id"
              class="flex items-center gap-3 p-2 bg-gray-50 rounded-lg group hover:bg-gray-100 transition-colors"
            >
              <!-- Type Icon -->
              <div
                class="w-8 h-8 rounded-lg flex items-center justify-center flex-shrink-0"
                :class="typeColors[item.type]"
              >
                <i :class="typeIcons[item.type]" class="text-sm" />
              </div>

              <!-- Item Info -->
              <div class="flex-1 min-w-0">
                <p class="text-sm font-medium text-gray-800 truncate">
                  {{ item.title }}
                </p>
                <p class="text-xs text-gray-500 capitalize">
                  {{ item.type }}
                </p>
              </div>

              <!-- Remove Button -->
              <button
                class="p-1.5 text-gray-400 hover:text-red-500 hover:bg-red-50 rounded-lg opacity-0 group-hover:opacity-100 transition-all"
                @click.stop="comparisonStore.removeItem(item.id)"
              >
                <i class="fas fa-times text-xs" />
              </button>
            </div>

            <!-- More Items Indicator -->
            <div
              v-if="extraCount > 0"
              class="text-center text-sm text-gray-500 py-1"
            >
              {{ $t('comparison.moreItems', { count: extraCount }) }}
            </div>
          </div>

          <!-- Type Breakdown -->
          <div class="px-3 pb-2">
            <div class="flex flex-wrap gap-1">
              <span
                v-for="(count, type) in comparisonStore.typeBreakdown"
                :key="type"
                class="text-xs px-2 py-0.5 rounded-full"
                :class="typeColors[type]"
              >
                {{ count }} {{ type }}{{ count > 1 ? 's' : '' }}
              </span>
            </div>
          </div>

          <!-- Actions -->
          <div class="p-3 pt-0 flex gap-2">
            <button
              class="flex-1 px-4 py-2.5 bg-gray-100 text-gray-700 rounded-xl font-medium hover:bg-gray-200 transition-colors"
              @click="comparisonStore.clearSelection()"
            >
              {{ $t('common.clearAll') }}
            </button>
            <button
              class="flex-1 px-4 py-2.5 rounded-xl font-medium transition-all flex items-center justify-center gap-2"
              :class="comparisonStore.canCompare
                ? 'bg-teal-500 text-white hover:bg-teal-600'
                : 'bg-gray-200 text-gray-400 cursor-not-allowed'"
              :disabled="!comparisonStore.canCompare"
              @click="comparisonStore.startComparison()"
            >
              <i class="fas fa-chart-bar" />
              <span>{{ $t('comparison.compare') }}</span>
            </button>
          </div>

          <!-- Minimum Items Hint -->
          <div
            v-if="!comparisonStore.canCompare"
            class="px-3 pb-3"
          >
            <p class="text-xs text-center text-gray-400">
              {{ $t('comparison.selectAtLeast2') }}
            </p>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<style scoped>
.slide-up-enter-active,
.slide-up-leave-active {
  transition: all 0.3s ease;
}

.slide-up-enter-from,
.slide-up-leave-to {
  opacity: 0;
  transform: translateY(20px);
}
</style>
