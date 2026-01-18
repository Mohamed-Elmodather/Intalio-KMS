<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import type { AIOperationType } from '@/types/ai'

const { t } = useI18n()

export interface AITool {
  id: AIOperationType
  name: string
  description: string
  icon: string
  shortcut?: string
  category: 'analysis' | 'content' | 'extraction' | 'search'
  color: string
  requiresInput: boolean
}

interface Props {
  modelValue: boolean
}

const props = defineProps<Props>()

const emit = defineEmits<{
  'update:modelValue': [value: boolean]
  'select': [tool: AITool]
}>()

const searchQuery = ref('')
const selectedIndex = ref(0)
const searchInputRef = ref<HTMLInputElement | null>(null)

// AI Tools configuration
const aiTools = computed<AITool[]>(() => [
  {
    id: 'extract-entities',
    name: t('ai.aiTools.entityRecognition'),
    description: t('ai.aiTools.entityRecognitionDesc'),
    icon: 'fas fa-tags',
    shortcut: 'Ctrl+E',
    category: 'analysis',
    color: 'blue',
    requiresInput: true
  },
  {
    id: 'analyze-sentiment',
    name: t('ai.aiTools.sentimentAnalysis'),
    description: t('ai.aiTools.sentimentAnalysisDesc'),
    icon: 'fas fa-smile',
    shortcut: 'Ctrl+S',
    category: 'analysis',
    color: 'pink',
    requiresInput: true
  },
  {
    id: 'summarize',
    name: t('ai.aiTools.summarize'),
    description: t('ai.aiTools.summarizeDesc'),
    icon: 'fas fa-compress-alt',
    shortcut: 'Ctrl+M',
    category: 'content',
    color: 'teal',
    requiresInput: true
  },
  {
    id: 'classify',
    name: t('ai.aiTools.classifyContent'),
    description: t('ai.aiTools.classifyContentDesc'),
    icon: 'fas fa-folder-tree',
    shortcut: 'Ctrl+C',
    category: 'analysis',
    color: 'purple',
    requiresInput: true
  },
  {
    id: 'ocr',
    name: t('ai.aiTools.ocrExtractText'),
    description: t('ai.aiTools.ocrExtractTextDesc'),
    icon: 'fas fa-file-image',
    shortcut: 'Ctrl+O',
    category: 'extraction',
    color: 'amber',
    requiresInput: false
  },
  {
    id: 'translate',
    name: t('ai.aiTools.translate'),
    description: t('ai.aiTools.translateDesc'),
    icon: 'fas fa-language',
    shortcut: 'Ctrl+T',
    category: 'content',
    color: 'green',
    requiresInput: true
  },
  {
    id: 'generate-title',
    name: t('ai.aiTools.generateTitles'),
    description: t('ai.aiTools.generateTitlesDesc'),
    icon: 'fas fa-heading',
    category: 'content',
    color: 'indigo',
    requiresInput: true
  },
  {
    id: 'auto-tag',
    name: t('ai.aiTools.autoTag'),
    description: t('ai.aiTools.autoTagDesc'),
    icon: 'fas fa-hashtag',
    category: 'extraction',
    color: 'cyan',
    requiresInput: true
  },
  {
    id: 'smart-search',
    name: t('ai.aiTools.smartSearch'),
    description: t('ai.aiTools.smartSearchDesc'),
    icon: 'fas fa-search',
    shortcut: 'Ctrl+F',
    category: 'search',
    color: 'orange',
    requiresInput: true
  }
])

// Category labels
const categories = computed(() => ({
  analysis: { label: t('ai.categories.analysis'), icon: 'fas fa-chart-bar' },
  content: { label: t('ai.categories.content'), icon: 'fas fa-file-alt' },
  extraction: { label: t('ai.categories.extraction'), icon: 'fas fa-magic' },
  search: { label: t('ai.categories.searchDiscovery'), icon: 'fas fa-compass' }
}))

// Filter tools based on search query
const filteredTools = computed(() => {
  if (!searchQuery.value) return aiTools.value

  const query = searchQuery.value.toLowerCase()
  return aiTools.value.filter(tool =>
    tool.name.toLowerCase().includes(query) ||
    tool.description.toLowerCase().includes(query) ||
    tool.category.toLowerCase().includes(query)
  )
})

// Group tools by category
const groupedTools = computed(() => {
  const groups: Record<string, AITool[]> = {}

  for (const tool of filteredTools.value) {
    if (!groups[tool.category]) {
      groups[tool.category] = []
    }
    groups[tool.category].push(tool)
  }

  return groups
})

// Get all tools in flat list for keyboard navigation
const flatTools = computed(() => {
  const flat: AITool[] = []
  for (const category of Object.keys(categories.value)) {
    if (groupedTools.value[category]) {
      flat.push(...groupedTools.value[category])
    }
  }
  return flat
})

// Watch for model value changes
watch(() => props.modelValue, (isOpen) => {
  if (isOpen) {
    searchQuery.value = ''
    selectedIndex.value = 0
    // Focus search input
    setTimeout(() => {
      searchInputRef.value?.focus()
    }, 100)
  }
})

// Keyboard navigation
function handleKeydown(e: KeyboardEvent) {
  if (!props.modelValue) return

  switch (e.key) {
    case 'Escape':
      close()
      break
    case 'ArrowDown':
      e.preventDefault()
      selectedIndex.value = Math.min(selectedIndex.value + 1, flatTools.value.length - 1)
      break
    case 'ArrowUp':
      e.preventDefault()
      selectedIndex.value = Math.max(selectedIndex.value - 1, 0)
      break
    case 'Enter':
      e.preventDefault()
      if (flatTools.value[selectedIndex.value]) {
        selectTool(flatTools.value[selectedIndex.value])
      }
      break
  }
}

function close() {
  emit('update:modelValue', false)
}

function selectTool(tool: AITool) {
  emit('select', tool)
  close()
}

function getToolColor(color: string): string {
  const colors: Record<string, string> = {
    blue: 'bg-blue-100 text-blue-600 group-hover:bg-blue-200',
    pink: 'bg-pink-100 text-pink-600 group-hover:bg-pink-200',
    teal: 'bg-teal-100 text-teal-600 group-hover:bg-teal-200',
    purple: 'bg-purple-100 text-purple-600 group-hover:bg-purple-200',
    amber: 'bg-amber-100 text-amber-600 group-hover:bg-amber-200',
    green: 'bg-green-100 text-green-600 group-hover:bg-green-200',
    indigo: 'bg-indigo-100 text-indigo-600 group-hover:bg-indigo-200',
    cyan: 'bg-cyan-100 text-cyan-600 group-hover:bg-cyan-200',
    orange: 'bg-orange-100 text-orange-600 group-hover:bg-orange-200'
  }
  return colors[color] || 'bg-gray-100 text-gray-600 group-hover:bg-gray-200'
}

function isSelected(tool: AITool): boolean {
  return flatTools.value[selectedIndex.value]?.id === tool.id
}
</script>

<template>
  <Teleport to="body">
    <Transition name="palette">
      <div
        v-if="modelValue"
        class="fixed inset-0 z-[100] flex items-start justify-center pt-[10vh]"
        @keydown="handleKeydown"
      >
        <!-- Backdrop -->
        <div
          class="absolute inset-0 bg-black/50 backdrop-blur-sm"
          @click="close"
        ></div>

        <!-- Palette Content -->
        <div class="relative w-full max-w-2xl bg-white rounded-2xl shadow-2xl overflow-hidden">
          <!-- Search Header -->
          <div class="px-4 py-3 border-b border-gray-100">
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 rounded-xl bg-gradient-to-br from-teal-500 to-emerald-500 flex items-center justify-center">
                <i class="fas fa-wand-magic-sparkles text-white"></i>
              </div>
              <div class="flex-1">
                <input
                  ref="searchInputRef"
                  v-model="searchQuery"
                  type="text"
                  :placeholder="$t('ai.searchTools')"
                  class="w-full text-lg bg-transparent border-none outline-none text-gray-800 placeholder-gray-400"
                />
              </div>
              <button
                @click="close"
                class="w-8 h-8 rounded-lg text-gray-400 hover:text-gray-600 hover:bg-gray-100 flex items-center justify-center transition-colors"
              >
                <i class="fas fa-times"></i>
              </button>
            </div>
          </div>

          <!-- Tools List -->
          <div class="max-h-[60vh] overflow-y-auto p-3">
            <template v-for="(catKey) in Object.keys(categories)" :key="catKey">
              <div v-if="groupedTools[catKey]?.length" class="mb-4">
                <!-- Category Header -->
                <div class="flex items-center gap-2 px-2 py-1.5 text-xs font-semibold text-gray-400 uppercase tracking-wider">
                  <i :class="(categories as any)[catKey].icon"></i>
                  <span>{{ (categories as any)[catKey].label }}</span>
                </div>

                <!-- Tools Grid -->
                <div class="grid grid-cols-1 gap-1">
                  <button
                    v-for="tool in groupedTools[catKey]"
                    :key="tool.id"
                    @click="selectTool(tool)"
                    @mouseenter="selectedIndex = flatTools.findIndex(t => t.id === tool.id)"
                    :class="[
                      'group flex items-center gap-3 p-3 rounded-xl text-left transition-all',
                      isSelected(tool)
                        ? 'bg-teal-50 ring-2 ring-teal-500'
                        : 'hover:bg-gray-50'
                    ]"
                  >
                    <!-- Icon -->
                    <div :class="[
                      'w-10 h-10 rounded-xl flex items-center justify-center transition-colors',
                      getToolColor(tool.color)
                    ]">
                      <i :class="tool.icon"></i>
                    </div>

                    <!-- Info -->
                    <div class="flex-1 min-w-0">
                      <div class="flex items-center gap-2">
                        <span class="font-medium text-gray-800">{{ tool.name }}</span>
                        <span
                          v-if="tool.shortcut"
                          class="px-1.5 py-0.5 bg-gray-100 text-gray-500 text-[10px] font-mono rounded"
                        >
                          {{ tool.shortcut }}
                        </span>
                      </div>
                      <p class="text-sm text-gray-500 truncate">{{ tool.description }}</p>
                    </div>

                    <!-- Arrow -->
                    <i class="fas fa-chevron-right text-gray-300 group-hover:text-gray-400 transition-colors"></i>
                  </button>
                </div>
              </div>
            </template>

            <!-- No Results -->
            <div v-if="filteredTools.length === 0" class="py-12 text-center">
              <div class="w-16 h-16 rounded-full bg-gray-100 flex items-center justify-center mx-auto mb-4">
                <i class="fas fa-search text-2xl text-gray-400"></i>
              </div>
              <p class="text-gray-500">{{ $t('ai.noToolsFound', { query: searchQuery }) }}</p>
            </div>
          </div>

          <!-- Footer -->
          <div class="px-4 py-3 border-t border-gray-100 bg-gray-50/50 flex items-center justify-between text-xs text-gray-400">
            <div class="flex items-center gap-4">
              <span class="flex items-center gap-1">
                <kbd class="px-1.5 py-0.5 bg-gray-200 rounded text-[10px]">↑↓</kbd>
                {{ $t('ai.navigate') }}
              </span>
              <span class="flex items-center gap-1">
                <kbd class="px-1.5 py-0.5 bg-gray-200 rounded text-[10px]">Enter</kbd>
                {{ $t('common.select') }}
              </span>
              <span class="flex items-center gap-1">
                <kbd class="px-1.5 py-0.5 bg-gray-200 rounded text-[10px]">Esc</kbd>
                {{ $t('common.close') }}
              </span>
            </div>
            <span>{{ $t('ai.toolsAvailable', { count: filteredTools.length }) }}</span>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<style scoped>
.palette-enter-active,
.palette-leave-active {
  transition: all 0.2s ease;
}

.palette-enter-from,
.palette-leave-to {
  opacity: 0;
}

.palette-enter-from .relative,
.palette-leave-to .relative {
  transform: scale(0.95) translateY(-20px);
}

/* Custom scrollbar */
.max-h-\[60vh\]::-webkit-scrollbar {
  width: 6px;
}

.max-h-\[60vh\]::-webkit-scrollbar-track {
  background: transparent;
}

.max-h-\[60vh\]::-webkit-scrollbar-thumb {
  background: #e5e7eb;
  border-radius: 3px;
}

.max-h-\[60vh\]::-webkit-scrollbar-thumb:hover {
  background: #d1d5db;
}
</style>
