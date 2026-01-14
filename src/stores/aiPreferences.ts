import { defineStore } from 'pinia'
import { ref, computed, watch } from 'vue'
import type { SupportedLanguage, SummaryType } from '@/types/ai'

// ============================================================================
// Types
// ============================================================================

export interface AIPreferences {
  // Response preferences
  responseStyle: 'concise' | 'detailed' | 'conversational'
  preferredLanguage: SupportedLanguage
  autoShowSources: boolean

  // Feature preferences
  autoSuggestFollowUps: boolean
  showConfidenceScores: boolean
  enableVoiceInput: boolean
  enableKeyboardShortcuts: boolean

  // AI Tool defaults
  defaultSummaryStyle: SummaryType
  defaultTitleStyle: 'formal' | 'casual' | 'seo' | 'creative'
  defaultTranslateLanguage: SupportedLanguage
  autoRunAnalysis: boolean

  // History preferences
  saveConversationHistory: boolean
  historyRetentionDays: number

  // UI preferences
  showContextPanel: boolean
  showToolbar: boolean
  compactMode: boolean
  showWelcomeScreen: boolean
}

export interface UsageStats {
  operationCounts: Record<string, number>
  totalOperations: number
  preferredOperations: string[]
  languageUsage: Record<string, number>
  activeHours: Record<number, number>
  lastActiveDate: string
  streakDays: number
}

export interface LearnedBehavior {
  preferredSummaryStyle: SummaryType | null
  preferredTranslateLanguage: SupportedLanguage | null
  commonQueryPatterns: string[]
  frequentTopics: string[]
}

// ============================================================================
// Default Values
// ============================================================================

const DEFAULT_PREFERENCES: AIPreferences = {
  responseStyle: 'detailed',
  preferredLanguage: 'en',
  autoShowSources: true,
  autoSuggestFollowUps: true,
  showConfidenceScores: true,
  enableVoiceInput: true,
  enableKeyboardShortcuts: true,
  defaultSummaryStyle: 'detailed',
  defaultTitleStyle: 'formal',
  defaultTranslateLanguage: 'ar',
  autoRunAnalysis: false,
  saveConversationHistory: true,
  historyRetentionDays: 30,
  showContextPanel: true,
  showToolbar: true,
  compactMode: false,
  showWelcomeScreen: true
}

const DEFAULT_STATS: UsageStats = {
  operationCounts: {},
  totalOperations: 0,
  preferredOperations: [],
  languageUsage: {},
  activeHours: {},
  lastActiveDate: '',
  streakDays: 0
}

const DEFAULT_LEARNED: LearnedBehavior = {
  preferredSummaryStyle: null,
  preferredTranslateLanguage: null,
  commonQueryPatterns: [],
  frequentTopics: []
}

// ============================================================================
// Store
// ============================================================================

export const useAIPreferencesStore = defineStore('aiPreferences', () => {
  // State
  const preferences = ref<AIPreferences>({ ...DEFAULT_PREFERENCES })
  const usageStats = ref<UsageStats>({ ...DEFAULT_STATS })
  const learnedBehavior = ref<LearnedBehavior>({ ...DEFAULT_LEARNED })

  // ============================================================================
  // Computed
  // ============================================================================

  const topOperations = computed(() => {
    const sorted = Object.entries(usageStats.value.operationCounts)
      .sort((a, b) => b[1] - a[1])
      .slice(0, 5)
    return sorted.map(([op, count]) => ({ operation: op, count }))
  })

  const estimatedTimeSaved = computed(() => {
    // Rough estimate: 2 minutes per AI operation
    const minutes = usageStats.value.totalOperations * 2
    const hours = Math.floor(minutes / 60)
    const remainingMinutes = minutes % 60
    return { hours, minutes: remainingMinutes }
  })

  const mostActiveHour = computed(() => {
    const hours = usageStats.value.activeHours
    let maxHour = 0
    let maxCount = 0
    for (const [hour, count] of Object.entries(hours)) {
      if (count > maxCount) {
        maxCount = count
        maxHour = parseInt(hour)
      }
    }
    return maxHour
  })

  // ============================================================================
  // Actions - Preferences
  // ============================================================================

  function updatePreference<K extends keyof AIPreferences>(key: K, value: AIPreferences[K]) {
    preferences.value[key] = value
    saveToStorage()
  }

  function updatePreferences(updates: Partial<AIPreferences>) {
    preferences.value = { ...preferences.value, ...updates }
    saveToStorage()
  }

  function resetPreferences() {
    preferences.value = { ...DEFAULT_PREFERENCES }
    saveToStorage()
  }

  // ============================================================================
  // Actions - Usage Tracking
  // ============================================================================

  function trackOperation(operation: string) {
    // Increment operation count
    if (!usageStats.value.operationCounts[operation]) {
      usageStats.value.operationCounts[operation] = 0
    }
    usageStats.value.operationCounts[operation]++
    usageStats.value.totalOperations++

    // Track active hour
    const hour = new Date().getHours()
    if (!usageStats.value.activeHours[hour]) {
      usageStats.value.activeHours[hour] = 0
    }
    usageStats.value.activeHours[hour]++

    // Update streak
    updateStreak()

    // Update preferred operations
    updatePreferredOperations()

    saveUsageStats()
  }

  function trackLanguageUsage(language: string) {
    if (!usageStats.value.languageUsage[language]) {
      usageStats.value.languageUsage[language] = 0
    }
    usageStats.value.languageUsage[language]++

    // Learn preferred translate language
    const sorted = Object.entries(usageStats.value.languageUsage)
      .sort((a, b) => b[1] - a[1])
    if (sorted.length > 0 && sorted[0][1] >= 5) {
      learnedBehavior.value.preferredTranslateLanguage = sorted[0][0] as SupportedLanguage
    }

    saveUsageStats()
    saveLearnedBehavior()
  }

  function trackQueryPattern(query: string) {
    // Extract common patterns (simplified)
    const patterns = learnedBehavior.value.commonQueryPatterns
    const words = query.toLowerCase().split(/\s+/)

    for (const word of words) {
      if (word.length > 4 && !patterns.includes(word)) {
        patterns.push(word)
        if (patterns.length > 20) {
          patterns.shift()
        }
      }
    }

    saveLearnedBehavior()
  }

  function trackTopic(topic: string) {
    const topics = learnedBehavior.value.frequentTopics
    const index = topics.indexOf(topic)

    if (index !== -1) {
      // Move to front (most recent)
      topics.splice(index, 1)
    }
    topics.unshift(topic)

    // Keep only last 10 topics
    if (topics.length > 10) {
      topics.pop()
    }

    saveLearnedBehavior()
  }

  function updateStreak() {
    const today = new Date().toISOString().split('T')[0]
    const lastDate = usageStats.value.lastActiveDate

    if (lastDate === today) {
      // Already tracked today
      return
    }

    if (lastDate) {
      const lastDateObj = new Date(lastDate)
      const todayObj = new Date(today)
      const diffDays = Math.floor((todayObj.getTime() - lastDateObj.getTime()) / (1000 * 60 * 60 * 24))

      if (diffDays === 1) {
        // Consecutive day
        usageStats.value.streakDays++
      } else if (diffDays > 1) {
        // Streak broken
        usageStats.value.streakDays = 1
      }
    } else {
      // First day
      usageStats.value.streakDays = 1
    }

    usageStats.value.lastActiveDate = today
  }

  function updatePreferredOperations() {
    const sorted = Object.entries(usageStats.value.operationCounts)
      .sort((a, b) => b[1] - a[1])
      .slice(0, 3)
      .map(([op]) => op)

    usageStats.value.preferredOperations = sorted
  }

  // ============================================================================
  // Actions - Learning
  // ============================================================================

  function learnFromSummaryChoice(style: SummaryType) {
    // Track summary style preference
    trackOperation('summarize')

    // After 5 uses of same style, set as preferred
    const count = usageStats.value.operationCounts[`summary_${style}`] || 0
    usageStats.value.operationCounts[`summary_${style}`] = count + 1

    if (count + 1 >= 5) {
      learnedBehavior.value.preferredSummaryStyle = style
      saveLearnedBehavior()
    }
  }

  function getPersonalizedSuggestions(): string[] {
    const suggestions: string[] = []

    // Based on preferred operations
    if (usageStats.value.preferredOperations.includes('summarize')) {
      suggestions.push('Summarize this content')
    }
    if (usageStats.value.preferredOperations.includes('translate')) {
      const lang = learnedBehavior.value.preferredTranslateLanguage || 'Arabic'
      suggestions.push(`Translate to ${lang}`)
    }
    if (usageStats.value.preferredOperations.includes('extract-entities')) {
      suggestions.push('Extract key entities')
    }

    // Based on frequent topics
    for (const topic of learnedBehavior.value.frequentTopics.slice(0, 2)) {
      suggestions.push(`Find more about ${topic}`)
    }

    return suggestions.slice(0, 4)
  }

  // ============================================================================
  // Persistence
  // ============================================================================

  function saveToStorage() {
    try {
      localStorage.setItem('ai_preferences', JSON.stringify(preferences.value))
    } catch (error) {
      console.error('Failed to save AI preferences:', error)
    }
  }

  function saveUsageStats() {
    try {
      localStorage.setItem('ai_usage_stats', JSON.stringify(usageStats.value))
    } catch (error) {
      console.error('Failed to save usage stats:', error)
    }
  }

  function saveLearnedBehavior() {
    try {
      localStorage.setItem('ai_learned_behavior', JSON.stringify(learnedBehavior.value))
    } catch (error) {
      console.error('Failed to save learned behavior:', error)
    }
  }

  function loadFromStorage() {
    try {
      const savedPrefs = localStorage.getItem('ai_preferences')
      if (savedPrefs) {
        preferences.value = { ...DEFAULT_PREFERENCES, ...JSON.parse(savedPrefs) }
      }

      const savedStats = localStorage.getItem('ai_usage_stats')
      if (savedStats) {
        usageStats.value = { ...DEFAULT_STATS, ...JSON.parse(savedStats) }
      }

      const savedLearned = localStorage.getItem('ai_learned_behavior')
      if (savedLearned) {
        learnedBehavior.value = { ...DEFAULT_LEARNED, ...JSON.parse(savedLearned) }
      }
    } catch (error) {
      console.error('Failed to load AI preferences:', error)
    }
  }

  function clearAllData() {
    preferences.value = { ...DEFAULT_PREFERENCES }
    usageStats.value = { ...DEFAULT_STATS }
    learnedBehavior.value = { ...DEFAULT_LEARNED }
    localStorage.removeItem('ai_preferences')
    localStorage.removeItem('ai_usage_stats')
    localStorage.removeItem('ai_learned_behavior')
  }

  function initialize() {
    loadFromStorage()
  }

  // ============================================================================
  // Return
  // ============================================================================

  return {
    // State
    preferences,
    usageStats,
    learnedBehavior,

    // Computed
    topOperations,
    estimatedTimeSaved,
    mostActiveHour,

    // Preference Actions
    updatePreference,
    updatePreferences,
    resetPreferences,

    // Tracking Actions
    trackOperation,
    trackLanguageUsage,
    trackQueryPattern,
    trackTopic,

    // Learning Actions
    learnFromSummaryChoice,
    getPersonalizedSuggestions,

    // Persistence
    initialize,
    clearAllData
  }
})
