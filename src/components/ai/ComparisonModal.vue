<script setup lang="ts">
import { computed, watch, onMounted, onUnmounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { useComparisonStore } from '@/stores/comparison'
import ComparisonSideBySide from './ComparisonSideBySide.vue'
import ComparisonInsights from './ComparisonInsights.vue'

const { t } = useI18n()
const comparisonStore = useComparisonStore()

const tabs = computed(() => [
  { id: 'side-by-side', label: t('comparison.sideBySide'), icon: 'fas fa-columns' },
  { id: 'insights', label: t('comparison.aiInsights'), icon: 'fas fa-brain' },
  { id: 'entities', label: t('comparison.entities'), icon: 'fas fa-tags' },
  { id: 'sentiment', label: t('comparison.sentiment'), icon: 'fas fa-smile' },
  { id: 'topics', label: t('comparison.topics'), icon: 'fas fa-lightbulb' },
] as const)

// Handle ESC key
function handleKeydown(e: KeyboardEvent) {
  if (e.key === 'Escape' && comparisonStore.isComparing) {
    comparisonStore.closeComparison()
  }
}

// Only lock body scroll when modal is actually open
watch(() => comparisonStore.isComparing, (isOpen) => {
  if (isOpen) {
    document.body.style.overflow = 'hidden'
  } else {
    document.body.style.overflow = ''
  }
})

onMounted(() => {
  document.addEventListener('keydown', handleKeydown)
})

onUnmounted(() => {
  document.removeEventListener('keydown', handleKeydown)
  document.body.style.overflow = ''
})
</script>

<template>
  <Teleport to="body">
    <Transition name="modal">
      <div
        v-if="comparisonStore.isComparing"
        class="fixed inset-0 z-[60] flex items-center justify-center p-4"
      >
        <!-- Backdrop -->
        <div
          class="absolute inset-0 bg-black/60 backdrop-blur-sm"
          @click="comparisonStore.closeComparison()"
        />

        <!-- Modal Content -->
        <div class="relative bg-white rounded-2xl shadow-2xl w-full max-w-7xl max-h-[90vh] flex flex-col overflow-hidden">
          <!-- Header -->
          <div class="flex items-center justify-between px-6 py-4 border-b border-gray-100 bg-gradient-to-r from-gray-50 to-white">
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 rounded-xl bg-teal-100 flex items-center justify-center">
                <i class="fas fa-chart-bar text-teal-600" />
              </div>
              <div>
                <h2 class="text-lg font-semibold text-gray-900">
                  {{ $t('comparison.compareItems', { count: comparisonStore.itemCount }) }}
                </h2>
                <p class="text-sm text-gray-500">
                  {{ $t('comparison.analyzeAndCompare') }}
                </p>
              </div>
            </div>

            <button
              class="p-2 text-gray-400 hover:text-gray-600 hover:bg-gray-100 rounded-lg transition-colors"
              @click="comparisonStore.closeComparison()"
            >
              <i class="fas fa-times text-lg" />
            </button>
          </div>

          <!-- Tabs -->
          <div class="flex items-center gap-1 px-6 py-3 border-b border-gray-100 bg-gray-50/50 overflow-x-auto">
            <button
              v-for="tab in tabs"
              :key="tab.id"
              class="flex items-center gap-2 px-4 py-2 rounded-lg font-medium text-sm whitespace-nowrap transition-all"
              :class="comparisonStore.activeTab === tab.id
                ? 'bg-teal-500 text-white shadow-sm'
                : 'text-gray-600 hover:bg-gray-100'"
              @click="comparisonStore.setActiveTab(tab.id)"
            >
              <i :class="tab.icon" />
              <span>{{ tab.label }}</span>
            </button>

            <!-- Generate Analysis Button -->
            <button
              v-if="!comparisonStore.analysisResults && comparisonStore.activeTab !== 'side-by-side'"
              class="ml-auto flex items-center gap-2 px-4 py-2 rounded-lg font-medium text-sm bg-purple-500 text-white hover:bg-purple-600 transition-colors"
              :disabled="comparisonStore.isAnalyzing"
              @click="comparisonStore.generateAnalysis()"
            >
              <i :class="comparisonStore.isAnalyzing ? 'fas fa-spinner fa-spin' : 'fas fa-magic'" />
              <span>{{ comparisonStore.isAnalyzing ? $t('comparison.analyzing') : $t('comparison.generateAnalysis') }}</span>
            </button>
          </div>

          <!-- Content Area -->
          <div class="flex-1 overflow-auto p-6">
            <!-- Side by Side View -->
            <ComparisonSideBySide v-if="comparisonStore.activeTab === 'side-by-side'" />

            <!-- AI Insights View -->
            <ComparisonInsights
              v-else-if="comparisonStore.activeTab === 'insights'"
              section="summary"
            />

            <!-- Entities View -->
            <ComparisonInsights
              v-else-if="comparisonStore.activeTab === 'entities'"
              section="entities"
            />

            <!-- Sentiment View -->
            <ComparisonInsights
              v-else-if="comparisonStore.activeTab === 'sentiment'"
              section="sentiment"
            />

            <!-- Topics View -->
            <ComparisonInsights
              v-else-if="comparisonStore.activeTab === 'topics'"
              section="topics"
            />
          </div>

          <!-- Footer -->
          <div class="flex items-center justify-between px-6 py-4 border-t border-gray-100 bg-gray-50/50">
            <div class="text-sm text-gray-500">
              <i class="fas fa-info-circle me-1" />
              {{ $t('comparison.aiAnalysisNote') }}
            </div>
            <div class="flex gap-2">
              <button
                class="px-4 py-2 text-gray-600 hover:bg-gray-100 rounded-lg transition-colors"
                @click="comparisonStore.clearSelection()"
              >
                {{ $t('comparison.clearSelection') }}
              </button>
              <button
                class="px-4 py-2 bg-gray-900 text-white rounded-lg hover:bg-gray-800 transition-colors"
                @click="comparisonStore.closeComparison()"
              >
                {{ $t('common.done') }}
              </button>
            </div>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<style scoped>
.modal-enter-active,
.modal-leave-active {
  transition: all 0.3s ease;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}

.modal-enter-from .relative,
.modal-leave-to .relative {
  transform: scale(0.95);
}
</style>
