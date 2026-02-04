import { describe, it, expect, vi, beforeEach } from 'vitest'
import { setActivePinia, createPinia } from 'pinia'

describe('AI Preferences Store', () => {
  let store: any

  beforeEach(async () => {
    // Reset modules to get a fresh store for each test
    vi.resetModules()
    vi.clearAllMocks()
    localStorage.clear()

    // Create fresh Pinia instance to ensure clean state
    const pinia = createPinia()
    setActivePinia(pinia)

    // Dynamically import store to get fresh instance
    const { useAIPreferencesStore } = await import('@/stores/aiPreferences')
    store = useAIPreferencesStore()
  })

  describe('Initial State', () => {
    it('should have default preferences', () => {
      expect(store.preferences.responseStyle).toBe('detailed')
      expect(store.preferences.preferredLanguage).toBe('en')
      expect(store.preferences.autoShowSources).toBe(true)
    })

    it('should have empty usage stats', () => {
      expect(store.usageStats.totalOperations).toBe(0)
      expect(store.usageStats.operationCounts).toEqual({})
    })

    it('should have empty learned behavior', () => {
      expect(store.learnedBehavior.preferredSummaryStyle).toBeNull()
      expect(store.learnedBehavior.frequentTopics).toEqual([])
    })
  })

  describe('updatePreference', () => {
    it('should update a single preference', () => {
      store.updatePreference('responseStyle', 'concise')

      expect(store.preferences.responseStyle).toBe('concise')
    })

    it('should save to localStorage', () => {
      store.updatePreference('responseStyle', 'conversational')

      const saved = localStorage.getItem('ai_preferences')
      expect(saved).not.toBeNull()
      expect(JSON.parse(saved!).responseStyle).toBe('conversational')
    })
  })

  describe('updatePreferences', () => {
    it('should update multiple preferences at once', () => {
      store.updatePreferences({
        responseStyle: 'concise',
        preferredLanguage: 'ar',
        autoShowSources: false,
      })

      expect(store.preferences.responseStyle).toBe('concise')
      expect(store.preferences.preferredLanguage).toBe('ar')
      expect(store.preferences.autoShowSources).toBe(false)
    })

    it('should preserve non-updated preferences', () => {
      const originalVoice = store.preferences.enableVoiceInput

      store.updatePreferences({ responseStyle: 'concise' })

      expect(store.preferences.enableVoiceInput).toBe(originalVoice)
    })
  })

  describe('resetPreferences', () => {
    it('should reset all preferences to defaults', () => {
      store.updatePreferences({
        responseStyle: 'concise',
        preferredLanguage: 'ar',
        autoShowSources: false,
      })

      store.resetPreferences()

      expect(store.preferences.responseStyle).toBe('detailed')
      expect(store.preferences.preferredLanguage).toBe('en')
      expect(store.preferences.autoShowSources).toBe(true)
    })
  })

  describe('trackOperation', () => {
    it('should increment operation count', () => {
      store.trackOperation('summarize')

      expect(store.usageStats.operationCounts.summarize).toBe(1)
      expect(store.usageStats.totalOperations).toBe(1)
    })

    it('should accumulate operation counts', () => {
      store.trackOperation('summarize')
      store.trackOperation('summarize')
      store.trackOperation('translate')

      expect(store.usageStats.operationCounts.summarize).toBe(2)
      expect(store.usageStats.operationCounts.translate).toBe(1)
      expect(store.usageStats.totalOperations).toBe(3)
    })

    it('should track active hours', () => {
      const currentHour = new Date().getHours()

      store.trackOperation('summarize')

      expect(store.usageStats.activeHours[currentHour]).toBe(1)
    })

    it('should update streak on first operation', () => {
      store.trackOperation('summarize')

      expect(store.usageStats.streakDays).toBe(1)
    })
  })

  describe('trackLanguageUsage', () => {
    it('should track language usage', () => {
      store.trackLanguageUsage('ar')

      expect(store.usageStats.languageUsage.ar).toBe(1)
    })

    it('should learn preferred language after 5 uses', () => {
      for (let i = 0; i < 5; i++) {
        store.trackLanguageUsage('ar')
      }

      expect(store.learnedBehavior.preferredTranslateLanguage).toBe('ar')
    })

    it('should not set preferred language before 5 uses', () => {
      for (let i = 0; i < 4; i++) {
        store.trackLanguageUsage('ar')
      }

      expect(store.learnedBehavior.preferredTranslateLanguage).toBeNull()
    })
  })

  describe('trackQueryPattern', () => {
    it('should extract and store word patterns', () => {
      store.trackQueryPattern('What is machine learning about')

      expect(store.learnedBehavior.commonQueryPatterns).toContain('machine')
      expect(store.learnedBehavior.commonQueryPatterns).toContain('learning')
      expect(store.learnedBehavior.commonQueryPatterns).toContain('about')
    })

    it('should ignore short words', () => {
      store.trackQueryPattern('How to do it now')

      // Words with 4 or fewer characters should be excluded
      expect(store.learnedBehavior.commonQueryPatterns).not.toContain('how')
      expect(store.learnedBehavior.commonQueryPatterns).not.toContain('to')
    })

    it('should limit patterns to 20', () => {
      for (let i = 0; i < 25; i++) {
        store.trackQueryPattern(`unique${i}word`)
      }

      expect(store.learnedBehavior.commonQueryPatterns.length).toBeLessThanOrEqual(20)
    })
  })

  describe('trackTopic', () => {
    it('should add topic to frequent topics', () => {
      store.trackTopic('Machine Learning')

      expect(store.learnedBehavior.frequentTopics).toContain('Machine Learning')
    })

    it('should move repeated topic to front', () => {
      store.trackTopic('Topic A')
      store.trackTopic('Topic B')
      store.trackTopic('Topic A')

      expect(store.learnedBehavior.frequentTopics[0]).toBe('Topic A')
    })

    it('should limit topics to 10', () => {
      for (let i = 0; i < 15; i++) {
        store.trackTopic(`Topic ${i}`)
      }

      expect(store.learnedBehavior.frequentTopics.length).toBeLessThanOrEqual(10)
    })
  })

  describe('learnFromSummaryChoice', () => {
    it('should track summarize operation', () => {
      store.learnFromSummaryChoice('brief')

      expect(store.usageStats.operationCounts.summarize).toBe(1)
    })

    it('should learn preferred style after 5 uses', () => {
      for (let i = 0; i < 5; i++) {
        store.learnFromSummaryChoice('brief')
      }

      expect(store.learnedBehavior.preferredSummaryStyle).toBe('brief')
    })
  })

  describe('getPersonalizedSuggestions', () => {
    it('should return empty array with no usage', () => {
      const suggestions = store.getPersonalizedSuggestions()

      expect(suggestions).toEqual([])
    })

    it('should suggest based on preferred operations', () => {
      // Track operations to build preferred list
      for (let i = 0; i < 5; i++) {
        store.trackOperation('summarize')
      }

      const suggestions = store.getPersonalizedSuggestions()

      expect(suggestions.some((s) => s.includes('Summarize'))).toBe(true)
    })

    it('should suggest based on frequent topics', () => {
      store.trackTopic('Football')

      const suggestions = store.getPersonalizedSuggestions()

      expect(suggestions.some((s) => s.includes('Football'))).toBe(true)
    })

    it('should limit suggestions to 4', () => {
      for (let i = 0; i < 10; i++) {
        store.trackTopic(`Topic ${i}`)
      }

      const suggestions = store.getPersonalizedSuggestions()

      expect(suggestions.length).toBeLessThanOrEqual(4)
    })
  })

  describe('Computed Properties', () => {
    it('should compute top operations', () => {
      store.trackOperation('summarize')
      store.trackOperation('summarize')
      store.trackOperation('translate')

      const top = store.topOperations

      expect(top[0].operation).toBe('summarize')
      expect(top[0].count).toBe(2)
    })

    it('should compute estimated time saved', () => {
      // Track 30 operations (30 * 2 = 60 minutes = 1 hour)
      for (let i = 0; i < 30; i++) {
        store.trackOperation('test')
      }

      const timeSaved = store.estimatedTimeSaved

      expect(timeSaved.hours).toBe(1)
      expect(timeSaved.minutes).toBe(0)
    })

    it('should compute most active hour', () => {
      const currentHour = new Date().getHours()
      store.trackOperation('test')
      store.trackOperation('test')

      expect(store.mostActiveHour).toBe(currentHour)
    })
  })

  describe('Persistence', () => {
    it('should load saved preferences on initialize', () => {
      localStorage.setItem(
        'ai_preferences',
        JSON.stringify({ responseStyle: 'concise' })
      )

      store.initialize()

      expect(store.preferences.responseStyle).toBe('concise')
    })

    it('should load saved usage stats on initialize', () => {
      localStorage.setItem(
        'ai_usage_stats',
        JSON.stringify({ totalOperations: 50 })
      )

      store.initialize()

      expect(store.usageStats.totalOperations).toBe(50)
    })

    it('should clear all data', () => {
      store.updatePreference('responseStyle', 'concise')
      store.trackOperation('test')

      store.clearAllData()

      expect(store.preferences.responseStyle).toBe('detailed')
      expect(store.usageStats.totalOperations).toBe(0)
      expect(localStorage.getItem('ai_preferences')).toBeNull()
    })
  })
})
