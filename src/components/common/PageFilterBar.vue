<script setup lang="ts">
import { ref, computed } from 'vue'
import { useI18n } from 'vue-i18n'

const { t } = useI18n()

// Props
interface SortOption {
  value: string
  label: string
  icon: string
}

interface Props {
  // Search
  searchQuery?: string
  searchPlaceholder?: string
  aiSearchPlaceholder?: string
  showAIToggle?: boolean
  isAIMode?: boolean
  isProcessingAI?: boolean
  aiSuggestions?: string[]
  searchSuggestions?: string[]
  showSearchSuggestions?: boolean

  // Sort
  sortOptions: SortOption[]
  sortBy: string
  sortOrder?: 'asc' | 'desc'
  sortByLabel?: string
  ascendingLabel?: string
  descendingLabel?: string

  // View Toggle
  showViewToggle?: boolean
  viewMode?: 'grid' | 'list'
  gridViewLabel?: string
  listViewLabel?: string

  // Labels
  clearAllLabel?: string
  applyLabel?: string
  aiSearchingLabel?: string
  analyzingQueryLabel?: string
  tryAskingLabel?: string
  aiSuggestionsLabel?: string
}

const props = withDefaults(defineProps<Props>(), {
  searchQuery: '',
  searchPlaceholder: '',
  aiSearchPlaceholder: '',
  showAIToggle: false,
  isAIMode: false,
  isProcessingAI: false,
  aiSuggestions: () => [],
  searchSuggestions: () => [],
  showSearchSuggestions: false,
  sortOrder: 'desc',
  sortByLabel: '',
  ascendingLabel: '',
  descendingLabel: '',
  showViewToggle: true,
  viewMode: 'grid',
  gridViewLabel: '',
  listViewLabel: '',
  clearAllLabel: '',
  applyLabel: '',
  aiSearchingLabel: '',
  analyzingQueryLabel: '',
  tryAskingLabel: '',
  aiSuggestionsLabel: ''
})

// Emits
const emit = defineEmits<{
  'update:searchQuery': [value: string]
  'update:isAIMode': [value: boolean]
  'update:sortBy': [value: string]
  'update:sortOrder': [value: 'asc' | 'desc']
  'update:viewMode': [value: 'grid' | 'list']
  'search': [query: string]
  'clearSearch': []
  'selectSuggestion': [suggestion: string]
}>()

// Local state
const localSearchQuery = ref(props.searchQuery)
const showAISuggestions = ref(false)
const showSortDropdown = ref(false)

// Computed
const currentSortOption = computed(() => {
  return props.sortOptions.find(opt => opt.value === props.sortBy) ?? props.sortOptions[0]
})

// Methods
function updateSearchQuery(value: string) {
  localSearchQuery.value = value
  emit('update:searchQuery', value)
}

function toggleAIMode() {
  emit('update:isAIMode', !props.isAIMode)
}

function handleSearch() {
  emit('search', localSearchQuery.value)
}

function clearSearch() {
  localSearchQuery.value = ''
  emit('update:searchQuery', '')
  emit('clearSearch')
}

function selectSortOption(value: string) {
  emit('update:sortBy', value)
  showSortDropdown.value = false
}

function toggleSortOrder() {
  emit('update:sortOrder', props.sortOrder === 'asc' ? 'desc' : 'asc')
}

function setViewMode(mode: 'grid' | 'list') {
  emit('update:viewMode', mode)
}

function selectAISuggestion(suggestion: string) {
  localSearchQuery.value = suggestion
  emit('update:searchQuery', suggestion)
  emit('selectSuggestion', suggestion)
  showAISuggestions.value = false
}

function handleSearchFocus() {
  if (props.isAIMode && !localSearchQuery.value) {
    showAISuggestions.value = true
  }
}

function handleSearchBlur() {
  setTimeout(() => {
    showAISuggestions.value = false
  }, 200)
}
</script>

<template>
  <div class="px-4 py-2 bg-gray-50/50 flex flex-wrap items-center gap-3">
    <!-- Search Section -->
    <div class="relative flex-1 max-w-xl">
      <div class="flex items-stretch">
        <!-- AI Mode Toggle -->
        <button
          v-if="showAIToggle"
          @click="toggleAIMode"
          :class="[
            'px-3 rounded-s-lg border border-e-0 flex items-center gap-1.5 text-xs font-medium transition-all',
            isAIMode
              ? 'bg-gradient-to-r from-teal-500 to-cyan-500 border-teal-500 text-white'
              : 'bg-gray-100 border-gray-200 text-gray-500 hover:bg-gray-200'
          ]"
        >
          <i class="fas fa-wand-magic-sparkles"></i>
          <span class="hidden sm:inline">AI</span>
        </button>

        <!-- Search Input -->
        <div class="relative flex-1">
          <i :class="[
            'absolute start-3 top-1/2 -translate-y-1/2 text-sm transition-colors',
            isAIMode ? 'fas fa-brain text-teal-500' : 'fas fa-search text-gray-400'
          ]"></i>
          <input
            :value="localSearchQuery"
            @input="updateSearchQuery(($event.target as HTMLInputElement).value)"
            type="text"
            :placeholder="isAIMode ? aiSearchPlaceholder : searchPlaceholder"
            @keyup.enter="handleSearch"
            @focus="handleSearchFocus"
            @blur="handleSearchBlur"
            :class="[
              'w-full ps-9 pe-20 py-2 text-sm focus:outline-none transition-all',
              showAIToggle ? 'rounded-e-lg' : 'rounded-lg',
              isAIMode
                ? 'bg-gradient-to-r from-teal-50 to-cyan-50 border border-teal-200 focus:ring-2 focus:ring-teal-400 focus:border-transparent placeholder:text-teal-400'
                : 'bg-white border border-gray-200 focus:ring-2 focus:ring-teal-500 focus:border-transparent',
              !showAIToggle && 'rounded-s-lg'
            ]"
          >
          <!-- Clear & Search Buttons -->
          <div class="absolute end-2 top-1/2 -translate-y-1/2 flex items-center gap-1">
            <button
              v-if="localSearchQuery"
              @click="clearSearch"
              :class="['p-1 rounded transition-colors', isAIMode ? 'text-teal-400 hover:text-teal-600' : 'text-gray-400 hover:text-gray-600']"
            >
              <i class="fas fa-times text-xs"></i>
            </button>
            <button
              v-if="isAIMode && localSearchQuery"
              @click="handleSearch"
              :disabled="isProcessingAI"
              class="p-1 rounded text-teal-500 hover:text-teal-600 disabled:opacity-50"
            >
              <i :class="isProcessingAI ? 'fas fa-spinner animate-spin' : 'fas fa-arrow-right'" class="text-sm"></i>
            </button>
          </div>
        </div>
      </div>

      <!-- AI Search Suggestions Dropdown -->
      <div
        v-if="showAIToggle && isAIMode && showAISuggestions && !localSearchQuery && aiSuggestions.length > 0"
        class="absolute start-0 top-full mt-2 w-full bg-white rounded-xl shadow-lg border border-teal-100 py-2 z-50"
      >
        <div class="px-3 py-1.5 text-xs font-semibold text-teal-500 flex items-center gap-2">
          <i class="fas fa-lightbulb"></i>
          {{ tryAskingLabel }}
        </div>
        <button
          v-for="suggestion in aiSuggestions"
          :key="suggestion"
          @click="selectAISuggestion(suggestion)"
          class="w-full px-3 py-2 text-left text-sm text-gray-700 hover:bg-teal-50 flex items-center gap-2"
        >
          <i class="fas fa-search text-teal-400 text-xs"></i>
          {{ suggestion }}
        </button>
      </div>

      <!-- Regular Search Suggestions Dropdown -->
      <div
        v-if="!isAIMode && showSearchSuggestions && searchSuggestions.length > 0"
        class="absolute start-0 top-full mt-2 w-full bg-white rounded-xl shadow-lg border border-gray-100 py-2 z-50"
      >
        <div class="px-3 py-1.5 text-xs font-semibold text-teal-500 flex items-center gap-2">
          <i class="fas fa-wand-magic-sparkles"></i>
          {{ aiSuggestionsLabel }}
        </div>
        <button
          v-for="suggestion in searchSuggestions"
          :key="suggestion"
          @click="selectAISuggestion(suggestion)"
          class="w-full px-3 py-2 text-left text-sm text-gray-700 hover:bg-gray-50 flex items-center gap-2 group"
        >
          <i class="fas fa-search text-gray-400 text-xs"></i>
          <span class="flex-1">{{ suggestion }}</span>
          <i class="fas fa-arrow-right text-teal-500 text-xs opacity-0 group-hover:opacity-100"></i>
        </button>
      </div>

      <!-- AI Processing Indicator -->
      <div
        v-if="isProcessingAI"
        class="absolute start-0 top-full mt-2 w-full bg-gradient-to-r from-teal-50 to-cyan-50 rounded-xl shadow-lg border border-teal-100 p-4 z-50"
      >
        <div class="flex items-center gap-3">
          <div class="w-8 h-8 rounded-lg bg-gradient-to-br from-teal-500 to-cyan-500 flex items-center justify-center">
            <i class="fas fa-brain text-white text-sm animate-pulse"></i>
          </div>
          <div>
            <div class="text-sm font-medium text-teal-700">{{ aiSearchingLabel }}</div>
            <div class="text-xs text-teal-500">{{ analyzingQueryLabel }}</div>
          </div>
        </div>
      </div>
    </div>

    <!-- Filter Slots -->
    <slot name="filters"></slot>

    <!-- Sort Options with Order Toggle -->
    <div class="relative ms-auto flex items-center">
      <button
        @click="showSortDropdown = !showSortDropdown"
        class="flex items-center gap-2 px-3 py-1.5 rounded-s-lg text-xs font-medium transition-all border border-e-0 bg-white border-gray-200 text-gray-600 hover:bg-gray-50"
      >
        <i :class="[currentSortOption?.icon, 'text-sm text-teal-500']"></i>
        <span>{{ currentSortOption?.label }}</span>
        <i :class="showSortDropdown ? 'fas fa-chevron-up' : 'fas fa-chevron-down'" class="text-[10px] ms-1"></i>
      </button>
      <button
        @click="toggleSortOrder"
        class="flex items-center justify-center w-8 h-8 rounded-e-lg text-xs font-medium transition-all border bg-white border-gray-200 text-gray-600 hover:bg-gray-50 hover:text-teal-600"
        :title="sortOrder === 'asc' ? ascendingLabel : descendingLabel"
      >
        <i :class="sortOrder === 'asc' ? 'fas fa-arrow-up' : 'fas fa-arrow-down'" class="text-sm text-teal-500"></i>
      </button>

      <!-- Sort Dropdown Menu -->
      <div
        v-if="showSortDropdown"
        class="absolute start-0 top-full mt-2 w-48 bg-white rounded-xl shadow-lg border border-gray-100 py-2 z-50"
      >
        <div class="px-3 py-1.5 text-xs font-semibold text-gray-400 uppercase tracking-wider">{{ sortByLabel }}</div>
        <div class="max-h-64 overflow-y-auto">
          <button
            v-for="option in sortOptions"
            :key="option.value"
            @click="selectSortOption(option.value)"
            :class="[
              'w-full px-3 py-2 text-left text-sm flex items-center gap-3 transition-colors',
              sortBy === option.value ? 'bg-teal-50 text-teal-700' : 'text-gray-700 hover:bg-gray-50'
            ]"
          >
            <i :class="[option.icon, 'text-sm w-4', sortBy === option.value ? 'text-teal-500' : 'text-gray-400']"></i>
            <span class="flex-1">{{ option.label }}</span>
            <i v-if="sortBy === option.value" class="fas fa-check text-teal-500 text-xs"></i>
          </button>
        </div>
      </div>
      <div v-if="showSortDropdown" @click="showSortDropdown = false" class="fixed inset-0 z-40"></div>
    </div>

    <!-- View Toggle -->
    <div v-if="showViewToggle" class="flex items-center gap-0.5 bg-white border border-gray-200 rounded-lg p-1">
      <button
        @click="setViewMode('grid')"
        :class="['px-2.5 py-1 rounded-md transition-all', viewMode === 'grid' ? 'bg-teal-500 text-white' : 'text-gray-500 hover:bg-gray-100']"
        :title="gridViewLabel"
      >
        <i class="fas fa-th-large text-xs"></i>
      </button>
      <button
        @click="setViewMode('list')"
        :class="['px-2.5 py-1 rounded-md transition-all', viewMode === 'list' ? 'bg-teal-500 text-white' : 'text-gray-500 hover:bg-gray-100']"
        :title="listViewLabel"
      >
        <i class="fas fa-list text-xs"></i>
      </button>
    </div>
  </div>
</template>
