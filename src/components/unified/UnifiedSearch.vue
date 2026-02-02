<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useI18n } from 'vue-i18n'
import { useAIServicesStore } from '@/stores/aiServices'

const { t } = useI18n()
const aiStore = useAIServicesStore()

// Props
interface Props {
  modelValue?: string
  placeholder?: string
  showAiButton?: boolean
  aiButtonText?: string
  showKeyboardShortcut?: boolean
  keyboardShortcut?: string
  size?: 'sm' | 'md' | 'lg'
  variant?: 'default' | 'minimal' | 'bordered'
  showAISuggestions?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: '',
  showAiButton: true,
  showKeyboardShortcut: true,
  keyboardShortcut: '⌘K',
  size: 'md',
  variant: 'default',
  showAISuggestions: true
})

// Emits
const emit = defineEmits<{
  'update:modelValue': [value: string]
  'search': [query: string]
  'ai-click': []
}>()

// Local state
const localQuery = ref(props.modelValue)
const isFocused = ref(false)
const showSuggestions = ref(false)
const isLoadingSuggestions = ref(false)
const selectedSuggestionIndex = ref(-1)

// AI Suggestion Interfaces
interface AISuggestion {
  id: string
  text: string
  type: 'autocomplete' | 'ai_enhanced' | 'recent' | 'trending'
  icon: string
  confidence?: number
}

interface QueryCategory {
  name: string
  icon: string
  color: string
}

// AI State
const aiSuggestions = ref<AISuggestion[]>([])
const detectedCategory = ref<QueryCategory | null>(null)

// Mock AI Suggestions Data
const mockTrendingSuggestions: AISuggestion[] = [
  { id: 't1', text: 'AFC Asian Cup 2027 schedule', type: 'trending', icon: 'fas fa-fire' },
  { id: 't2', text: 'Tournament regulations', type: 'trending', icon: 'fas fa-fire' },
  { id: 't3', text: 'Venue information', type: 'trending', icon: 'fas fa-fire' }
]

const mockRecentSearches: AISuggestion[] = [
  { id: 'r1', text: 'Team registration process', type: 'recent', icon: 'fas fa-clock' },
  { id: 'r2', text: 'Media accreditation', type: 'recent', icon: 'fas fa-clock' }
]

// Computed
const inputValue = computed({
  get: () => props.modelValue || localQuery.value,
  set: (value: string) => {
    localQuery.value = value
    emit('update:modelValue', value)
  }
})

const sizeClasses = computed(() => {
  switch (props.size) {
    case 'sm':
      return 'py-2 text-xs'
    case 'lg':
      return 'py-3.5 text-base'
    default:
      return 'py-2.5 text-sm'
  }
})

const visibleSuggestions = computed(() => {
  if (!showSuggestions.value || !props.showAISuggestions) return []
  return aiSuggestions.value.slice(0, 8)
})

// Watch for input changes to generate suggestions
let debounceTimeout: ReturnType<typeof setTimeout> | null = null

watch(inputValue, (newValue) => {
  if (debounceTimeout) clearTimeout(debounceTimeout)

  if (!newValue || newValue.length < 2) {
    // Show trending and recent when query is empty/short
    aiSuggestions.value = [...mockRecentSearches, ...mockTrendingSuggestions]
    return
  }

  debounceTimeout = setTimeout(() => {
    generateAISuggestions(newValue)
  }, 300)
})

// AI Functions
async function generateAISuggestions(query: string) {
  isLoadingSuggestions.value = true

  try {
    await new Promise(resolve => setTimeout(resolve, 200))

    // Detect query category
    detectQueryCategory(query)

    // Generate contextual suggestions
    const suggestions: AISuggestion[] = []

    // AI-enhanced suggestions
    suggestions.push({
      id: 'ai1',
      text: `${query} - detailed guide`,
      type: 'ai_enhanced',
      icon: 'fas fa-wand-magic-sparkles',
      confidence: 0.95
    })
    suggestions.push({
      id: 'ai2',
      text: `${query} best practices`,
      type: 'ai_enhanced',
      icon: 'fas fa-wand-magic-sparkles',
      confidence: 0.88
    })

    // Autocomplete suggestions
    const autocompleteSuggestions = generateAutocompleteSuggestions(query)
    suggestions.push(...autocompleteSuggestions)

    // Add some trending/recent
    suggestions.push(...mockTrendingSuggestions.slice(0, 2))

    aiSuggestions.value = suggestions
  } catch (error) {
    console.error('Failed to generate suggestions:', error)
  } finally {
    isLoadingSuggestions.value = false
  }
}

function generateAutocompleteSuggestions(query: string): AISuggestion[] {
  const queryLower = query.toLowerCase()
  const suggestions: AISuggestion[] = []

  const commonCompletions: Record<string, string[]> = {
    'match': ['match schedule', 'match results', 'match tickets', 'match venue'],
    'team': ['team registration', 'team schedule', 'team roster', 'team standings'],
    'venue': ['venue information', 'venue map', 'venue capacity', 'venue facilities'],
    'ticket': ['ticket booking', 'ticket prices', 'ticket availability'],
    'media': ['media accreditation', 'media guidelines', 'media center'],
    'tournament': ['tournament rules', 'tournament schedule', 'tournament format'],
    'document': ['document templates', 'document guidelines', 'document submission'],
    'policy': ['policy guidelines', 'policy updates', 'policy documents']
  }

  for (const [key, completions] of Object.entries(commonCompletions)) {
    if (queryLower.includes(key)) {
      completions.slice(0, 3).forEach((completion, idx) => {
        suggestions.push({
          id: `auto_${key}_${idx}`,
          text: completion,
          type: 'autocomplete',
          icon: 'fas fa-search'
        })
      })
      break
    }
  }

  return suggestions
}

function detectQueryCategory(query: string) {
  const queryLower = query.toLowerCase()

  const categories: Record<string, QueryCategory> = {
    'document|file|pdf|template': { name: t('nav.documents'), icon: 'fas fa-file-alt', color: 'bg-blue-100 text-blue-700' },
    'article|news|blog|post': { name: t('nav.articles'), icon: 'fas fa-newspaper', color: 'bg-purple-100 text-purple-700' },
    'video|media|watch': { name: t('nav.mediaCenter'), icon: 'fas fa-video', color: 'bg-red-100 text-red-700' },
    'course|learn|training': { name: t('nav.learning'), icon: 'fas fa-graduation-cap', color: 'bg-green-100 text-green-700' },
    'event|schedule|calendar': { name: t('nav.events'), icon: 'fas fa-calendar', color: 'bg-amber-100 text-amber-700' },
    'policy|rule|regulation': { name: t('nav.policies'), icon: 'fas fa-gavel', color: 'bg-indigo-100 text-indigo-700' }
  }

  for (const [pattern, category] of Object.entries(categories)) {
    if (new RegExp(pattern).test(queryLower)) {
      detectedCategory.value = category
      return
    }
  }

  detectedCategory.value = null
}

function getSuggestionTypeColor(type: string): string {
  switch (type) {
    case 'ai_enhanced': return 'text-teal-600'
    case 'recent': return 'text-gray-500'
    case 'trending': return 'text-orange-500'
    default: return 'text-gray-400'
  }
}

function getSuggestionTypeBg(type: string): string {
  switch (type) {
    case 'ai_enhanced': return 'bg-teal-50'
    case 'recent': return 'bg-gray-50'
    case 'trending': return 'bg-orange-50'
    default: return 'bg-gray-50'
  }
}

// Methods
function handleSearch() {
  showSuggestions.value = false
  if (inputValue.value.trim()) {
    emit('search', inputValue.value.trim())
  }
}

function handleAiClick() {
  emit('ai-click')
}

function handleFocus() {
  isFocused.value = true
  if (props.showAISuggestions) {
    showSuggestions.value = true
    if (aiSuggestions.value.length === 0) {
      aiSuggestions.value = [...mockRecentSearches, ...mockTrendingSuggestions]
    }
  }
}

function handleBlur() {
  // Delay to allow click on suggestions
  setTimeout(() => {
    isFocused.value = false
    showSuggestions.value = false
  }, 200)
}

function selectSuggestion(suggestion: AISuggestion) {
  inputValue.value = suggestion.text
  showSuggestions.value = false
  handleSearch()
}

function handleKeyDown(event: KeyboardEvent) {
  if (!showSuggestions.value || visibleSuggestions.value.length === 0) return

  switch (event.key) {
    case 'ArrowDown':
      event.preventDefault()
      selectedSuggestionIndex.value = Math.min(
        selectedSuggestionIndex.value + 1,
        visibleSuggestions.value.length - 1
      )
      break
    case 'ArrowUp':
      event.preventDefault()
      selectedSuggestionIndex.value = Math.max(selectedSuggestionIndex.value - 1, -1)
      break
    case 'Enter':
      if (selectedSuggestionIndex.value >= 0) {
        const selectedSuggestion = visibleSuggestions.value[selectedSuggestionIndex.value]
        if (selectedSuggestion) {
          event.preventDefault()
          selectSuggestion(selectedSuggestion)
        }
      }
      break
    case 'Escape':
      showSuggestions.value = false
      selectedSuggestionIndex.value = -1
      break
  }
}
</script>

<template>
  <div class="unified-search" :class="[`unified-search--${variant}`, { 'unified-search--focused': isFocused }]">
    <div class="unified-search__glow"></div>
    <div class="unified-search__inner">
      <div class="unified-search__content">
        <!-- Search Icon -->
        <i class="fas fa-search unified-search__icon"></i>

        <!-- Detected Category Badge -->
        <span v-if="detectedCategory && isFocused"
              :class="['unified-search__category', detectedCategory.color]">
          <i :class="detectedCategory.icon"></i>
          {{ detectedCategory.name }}
        </span>

        <!-- Input -->
        <input
          type="text"
          v-model="inputValue"
          :placeholder="placeholder || $t('common.searchPlaceholder')"
          :class="['unified-search__input', sizeClasses]"
          @keyup.enter="handleSearch"
          @keydown="handleKeyDown"
          @focus="handleFocus"
          @blur="handleBlur"
        >

        <!-- Loading Indicator -->
        <i v-if="isLoadingSuggestions" class="fas fa-circle-notch fa-spin unified-search__loading"></i>

        <!-- Actions -->
        <div class="unified-search__actions">
          <!-- Keyboard Shortcut -->
          <kbd v-if="showKeyboardShortcut" class="unified-search__kbd">
            {{ keyboardShortcut }}
          </kbd>

          <!-- Divider -->
          <div v-if="showAiButton && showKeyboardShortcut" class="unified-search__divider"></div>

          <!-- AI Button -->
          <button
            v-if="showAiButton"
            @click="handleAiClick"
            class="unified-search__ai-btn"
            type="button"
          >
            <i class="fas fa-wand-magic-sparkles"></i>
            <span>{{ aiButtonText || $t('header.askAI') }}</span>
          </button>
        </div>
      </div>
    </div>

    <!-- AI Suggestions Dropdown -->
    <Transition name="suggestions">
      <div v-if="showSuggestions && visibleSuggestions.length > 0" class="unified-search__suggestions">
        <div class="unified-search__suggestions-header">
          <div class="flex items-center gap-2">
            <i class="fas fa-wand-magic-sparkles text-teal-500"></i>
            <span class="text-xs font-medium text-gray-500">{{ $t('search.aiPoweredSuggestions') }}</span>
          </div>
        </div>

        <div class="unified-search__suggestions-list">
          <button
            v-for="(suggestion, index) in visibleSuggestions"
            :key="suggestion.id"
            @click="selectSuggestion(suggestion)"
            :class="['unified-search__suggestion-item',
                     { 'unified-search__suggestion-item--selected': index === selectedSuggestionIndex }]"
          >
            <div :class="['unified-search__suggestion-icon', getSuggestionTypeBg(suggestion.type)]">
              <i :class="[suggestion.icon, getSuggestionTypeColor(suggestion.type)]"></i>
            </div>
            <div class="unified-search__suggestion-content">
              <span class="unified-search__suggestion-text">{{ suggestion.text }}</span>
              <span v-if="suggestion.confidence" class="unified-search__suggestion-confidence">
                {{ $t('search.matchPercent', { percent: Math.round(suggestion.confidence * 100) }) }}
              </span>
            </div>
            <span :class="['unified-search__suggestion-type', getSuggestionTypeColor(suggestion.type)]">
              {{ suggestion.type === 'ai_enhanced' ? 'AI' : suggestion.type === 'trending' ? $t('common.trending') : suggestion.type === 'recent' ? $t('search.recent') : '' }}
            </span>
          </button>
        </div>

        <div class="unified-search__suggestions-footer">
          <span class="text-xs text-gray-400">
            <i class="fas fa-keyboard me-1"></i>
            {{ $t('search.keyboardHint') }}
          </span>
        </div>
      </div>
    </Transition>
  </div>
</template>

<style scoped>
.unified-search {
  position: relative;
  width: 100%;
}

.unified-search__glow {
  position: absolute;
  inset: 0;
  background: linear-gradient(135deg, rgba(20, 184, 166, 0.15), rgba(13, 148, 136, 0.1));
  border-radius: 10px;
  opacity: 0;
  filter: blur(8px);
  transition: opacity 0.3s ease;
  pointer-events: none;
}

@media (min-width: 640px) {
  .unified-search__glow {
    border-radius: 14px;
  }
}

.unified-search--focused .unified-search__glow {
  opacity: 1;
}

.unified-search__inner {
  position: relative;
  background: #f9fafb;
  border: 1px solid #e5e7eb;
  border-radius: 10px;
  transition: all 0.3s ease;
}

@media (min-width: 640px) {
  .unified-search__inner {
    border-radius: 14px;
  }
}

.unified-search--focused .unified-search__inner {
  background: white;
  border-color: #14b8a6;
  box-shadow: 0 0 0 3px rgba(20, 184, 166, 0.1);
}

.unified-search--minimal .unified-search__inner {
  background: transparent;
  border: none;
}

.unified-search--bordered .unified-search__inner {
  background: white;
  border: 2px solid #e5e7eb;
}

.unified-search__content {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 0 10px;
}

@media (min-width: 640px) {
  .unified-search__content {
    gap: 12px;
    padding: 0 16px;
  }
}

.unified-search__icon {
  color: #9ca3af;
  transition: color 0.3s ease;
  flex-shrink: 0;
}

.unified-search--focused .unified-search__icon {
  color: #14b8a6;
}

.unified-search__input {
  flex: 1;
  background: transparent;
  color: #111827;
  outline: none;
  border: none;
  min-width: 0;
}

.unified-search__input::placeholder {
  color: #9ca3af;
}

.unified-search__actions {
  display: flex;
  align-items: center;
  gap: 8px;
  flex-shrink: 0;
}

.unified-search__kbd {
  display: none;
  align-items: center;
  gap: 4px;
  padding: 4px 8px;
  font-size: 11px;
  color: #9ca3af;
  background: #f3f4f6;
  border-radius: 6px;
  border: 1px solid #e5e7eb;
  font-family: inherit;
}

@media (min-width: 640px) {
  .unified-search__kbd {
    display: inline-flex;
  }
}

.unified-search__divider {
  display: none;
  width: 1px;
  height: 20px;
  background: #e5e7eb;
}

@media (min-width: 640px) {
  .unified-search__divider {
    display: block;
  }
}

.unified-search__ai-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0;
  padding: 6px 8px;
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  color: white;
  border: none;
  border-radius: 8px;
  font-size: 12px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
  white-space: nowrap;
}

.unified-search__ai-btn span {
  display: none;
}

@media (min-width: 640px) {
  .unified-search__ai-btn {
    gap: 6px;
    padding: 6px 12px;
  }

  .unified-search__ai-btn span {
    display: inline;
  }
}

.unified-search__ai-btn:hover {
  transform: scale(1.02);
  box-shadow: 0 4px 12px rgba(20, 184, 166, 0.3);
}

.unified-search__ai-btn i {
  font-size: 11px;
}

/* Category Badge */
.unified-search__category {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  padding: 4px 8px;
  border-radius: 6px;
  font-size: 11px;
  font-weight: 500;
  flex-shrink: 0;
  animation: fadeIn 0.2s ease;
}

.unified-search__category i {
  font-size: 10px;
}

/* Loading Indicator */
.unified-search__loading {
  color: #14b8a6;
  font-size: 14px;
  flex-shrink: 0;
}

/* AI Suggestions Dropdown */
.unified-search__suggestions {
  position: absolute;
  top: 100%;
  left: 0;
  right: 0;
  margin-top: 6px;
  background: white;
  border: 1px solid #e5e7eb;
  border-radius: 10px;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.1), 0 2px 10px rgba(0, 0, 0, 0.05);
  z-index: 50;
  overflow: hidden;
}

@media (min-width: 640px) {
  .unified-search__suggestions {
    margin-top: 8px;
    border-radius: 14px;
  }
}

.unified-search__suggestions-header {
  padding: 10px 12px;
  background: linear-gradient(to right, rgba(20, 184, 166, 0.05), rgba(16, 185, 129, 0.05));
  border-bottom: 1px solid #f3f4f6;
}

@media (min-width: 640px) {
  .unified-search__suggestions-header {
    padding: 12px 16px;
  }
}

.unified-search__suggestions-list {
  max-height: 320px;
  overflow-y: auto;
}

.unified-search__suggestion-item {
  display: flex;
  align-items: center;
  gap: 10px;
  width: 100%;
  padding: 10px 12px;
  text-align: left;
  background: transparent;
  border: none;
  cursor: pointer;
  transition: all 0.15s ease;
}

@media (min-width: 640px) {
  .unified-search__suggestion-item {
    gap: 12px;
    padding: 12px 16px;
  }
}

.unified-search__suggestion-item:hover,
.unified-search__suggestion-item--selected {
  background: #f9fafb;
}

.unified-search__suggestion-item--selected {
  background: rgba(20, 184, 166, 0.05);
}

.unified-search__suggestion-icon {
  width: 28px;
  height: 28px;
  border-radius: 6px;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

@media (min-width: 640px) {
  .unified-search__suggestion-icon {
    width: 32px;
    height: 32px;
    border-radius: 8px;
  }
}

.unified-search__suggestion-icon i {
  font-size: 11px;
}

@media (min-width: 640px) {
  .unified-search__suggestion-icon i {
    font-size: 12px;
  }
}

.unified-search__suggestion-content {
  flex: 1;
  min-width: 0;
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.unified-search__suggestion-text {
  font-size: 13px;
  color: #374151;
  font-weight: 500;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

@media (min-width: 640px) {
  .unified-search__suggestion-text {
    font-size: 14px;
  }
}

.unified-search__suggestion-confidence {
  font-size: 11px;
  color: #14b8a6;
  font-weight: 500;
}

.unified-search__suggestion-type {
  font-size: 10px;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  flex-shrink: 0;
}

.unified-search__suggestions-footer {
  display: none;
  padding: 10px 16px;
  background: #f9fafb;
  border-top: 1px solid #f3f4f6;
  text-align: center;
}

@media (min-width: 640px) {
  .unified-search__suggestions-footer {
    display: block;
  }
}

/* Transition animations */
.suggestions-enter-active,
.suggestions-leave-active {
  transition: all 0.2s ease;
}

.suggestions-enter-from,
.suggestions-leave-to {
  opacity: 0;
  transform: translateY(-8px);
}

@keyframes fadeIn {
  from { opacity: 0; transform: scale(0.95); }
  to { opacity: 1; transform: scale(1); }
}
</style>
