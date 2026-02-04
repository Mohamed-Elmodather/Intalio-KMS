import { describe, it, expect, vi, beforeEach } from 'vitest'
import { mount, flushPromises } from '@vue/test-utils'
import { createPinia, setActivePinia } from 'pinia'
import SettingsPage from '@/pages/settings/SettingsPage.vue'

// Mock vue-i18n
vi.mock('vue-i18n', () => ({
  useI18n: () => ({
    t: (key: string) => {
      const translations: Record<string, string> = {
        'settings.tabs.account': 'Account',
        'settings.tabs.ai': 'AI Features',
        'settings.tabs.notifications': 'Notifications',
        'settings.tabs.privacy': 'Privacy',
        'settings.tabs.appearance': 'Appearance',
        'settings.tabs.language': 'Language',
        'settings.tabs.security': 'Security',
        'settings.tabs.apps': 'Connected Apps',
        'settings.tabs.shortcuts': 'Shortcuts',
        'settings.notificationCategories.activity': 'Activity',
        'settings.notificationCategories.updates': 'Updates',
        'settings.notificationItems.comments': 'Comments',
        'settings.notificationItems.commentsDesc': 'When someone comments on your content',
        'settings.notificationItems.mentions': 'Mentions',
        'settings.notificationItems.mentionsDesc': 'When someone mentions you',
        'settings.notificationItems.reactions': 'Reactions',
        'settings.notificationItems.reactionsDesc': 'When someone reacts to your content',
        'settings.notificationItems.news': 'News',
        'settings.notificationItems.newsDesc': 'News updates',
        'settings.notificationItems.events': 'Events',
        'settings.notificationItems.eventsDesc': 'Event notifications',
        'settings.notificationItems.courses': 'Courses',
        'settings.notificationItems.coursesDesc': 'Course updates',
        'settings.privacyOptions.publicProfile': 'Public Profile',
        'settings.privacyOptions.publicProfileDesc': 'Make your profile public',
        'settings.privacyOptions.showActivity': 'Show Activity',
        'settings.privacyOptions.showActivityDesc': 'Show your activity status',
        'settings.privacyOptions.showContributions': 'Show Contributions',
        'settings.privacyOptions.showContributionsDesc': 'Show your contributions',
        'settings.privacyOptions.appearInSearch': 'Appear in Search',
        'settings.privacyOptions.appearInSearchDesc': 'Allow others to find you',
        'settings.privacyOptions.usageAnalytics': 'Usage Analytics',
        'settings.privacyOptions.usageAnalyticsDesc': 'Help improve the platform',
        'settings.themes.light': 'Light',
        'settings.themes.dark': 'Dark',
        'settings.themes.system': 'System',
        'settings.shortcutCategories.navigation': 'Navigation',
        'settings.shortcutCategories.searchActions': 'Search & Actions',
        'settings.shortcutCategories.content': 'Content',
        'settings.shortcutCategories.aiFeatures': 'AI Features',
        'settings.shortcuts.goToHome': 'Go to Home',
        'settings.shortcuts.goToArticles': 'Go to Articles',
        'settings.shortcuts.goToDocuments': 'Go to Documents',
        'settings.shortcuts.goToEvents': 'Go to Events',
        'settings.shortcuts.goToProfile': 'Go to Profile',
        'settings.shortcuts.goToSettings': 'Go to Settings',
        'settings.shortcuts.focusSearch': 'Focus Search',
        'settings.shortcuts.openCommandPalette': 'Open Command Palette',
        'settings.shortcuts.createNewItem': 'Create New Item',
        'settings.shortcuts.closeModal': 'Close Modal',
        'settings.shortcuts.editItem': 'Edit Item',
        'settings.shortcuts.bookmarkItem': 'Bookmark Item',
        'settings.shortcuts.shareItem': 'Share Item',
        'settings.shortcuts.submitForm': 'Submit Form',
        'settings.shortcuts.openAIAssistant': 'Open AI Assistant',
        'settings.shortcuts.aiSummarize': 'AI Summarize',
        'settings.shortcuts.aiTranslate': 'AI Translate',
        'settings.aiFeatureToggles.smartSuggestions': 'Smart Suggestions',
        'settings.aiFeatureToggles.smartSuggestionsDesc': 'Get AI-powered suggestions',
        'settings.aiFeatureToggles.autoSummarization': 'Auto Summarization',
        'settings.aiFeatureToggles.autoSummarizationDesc': 'Automatically summarize content',
        'settings.aiFeatureToggles.aiTranslation': 'AI Translation',
        'settings.aiFeatureToggles.aiTranslationDesc': 'Translate content using AI',
        'settings.aiFeatureToggles.aiAssistantChat': 'AI Assistant Chat',
        'settings.aiFeatureToggles.aiAssistantChatDesc': 'Chat with AI assistant',
        'settings.aiFeatureToggles.autoClassification': 'Auto Classification',
        'settings.aiFeatureToggles.autoClassificationDesc': 'Auto-classify documents',
        'settings.aiFeatureToggles.documentOCR': 'Document OCR',
        'settings.aiFeatureToggles.documentOCRDesc': 'Extract text from images',
        'settings.aiFeatureToggles.sentimentAnalysis': 'Sentiment Analysis',
        'settings.aiFeatureToggles.sentimentAnalysisDesc': 'Analyze content sentiment',
        'settings.aiFeatureToggles.personalizedRecommendations': 'Personalized Recommendations',
        'settings.aiFeatureToggles.personalizedRecommendationsDesc': 'Get personalized content',
        'settings.summaryLengthOptions.brief': 'Brief',
        'settings.summaryLengthOptions.briefDesc': '1-2 sentences',
        'settings.summaryLengthOptions.medium': 'Medium',
        'settings.summaryLengthOptions.mediumDesc': '1 paragraph',
        'settings.summaryLengthOptions.detailed': 'Detailed',
        'settings.summaryLengthOptions.detailedDesc': 'Multiple paragraphs',
      }
      return translations[key] || key
    },
  }),
}))

// Mock AI components
vi.mock('@/components/ai', () => ({
  AILoadingIndicator: { template: '<div class="ai-loading"></div>' },
  AIConfidenceBar: { template: '<div class="ai-confidence"></div>' },
}))

// Mock LoadingSpinner
vi.mock('@/components/common/LoadingSpinner.vue', () => ({
  default: { template: '<div class="loading-spinner"></div>' },
}))

// Mock AI services store
vi.mock('@/stores/aiServices', () => ({
  useAIServicesStore: () => ({
    isLoading: false,
    error: null,
  }),
}))

describe('SettingsPage', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.clearAllMocks()
  })

  describe('Rendering', () => {
    it('should render settings page', () => {
      const wrapper = mount(SettingsPage)
      expect(wrapper.exists()).toBe(true)
    })

    it('should render user info', () => {
      const wrapper = mount(SettingsPage)
      expect(wrapper.text()).toContain('Ahmed Imam')
    })
  })

  describe('Tabs', () => {
    it('should render all tabs', () => {
      const wrapper = mount(SettingsPage)
      const expectedTabs = [
        'Account',
        'AI Features',
        'Notifications',
        'Privacy',
        'Appearance',
        'Language',
        'Security',
        'Connected Apps',
        'Shortcuts',
      ]
      expectedTabs.forEach((tab) => {
        expect(wrapper.text()).toContain(tab)
      })
    })

    it('should show account tab by default', () => {
      const wrapper = mount(SettingsPage)
      // Account tab content should be visible
      expect(wrapper.text()).toContain('ahmed.imam@intalio.com')
    })

    it('should switch tabs when clicked', async () => {
      const wrapper = mount(SettingsPage)
      const tabs = wrapper.findAll('.settings-tab, [class*="tab"]')

      // Find and click the AI tab
      const aiTab = tabs.find((tab) => tab.text().includes('AI Features'))
      if (aiTab) {
        await aiTab.trigger('click')
        await wrapper.vm.$nextTick()
      }
    })
  })

  describe('Account Settings', () => {
    it('should display user information', () => {
      const wrapper = mount(SettingsPage)
      expect(wrapper.text()).toContain('Ahmed')
      expect(wrapper.text()).toContain('Imam')
      expect(wrapper.text()).toContain('ahmed.imam@intalio.com')
    })
  })

  describe('AI Settings', () => {
    it('should have AI feature toggles defined', () => {
      const wrapper = mount(SettingsPage)
      const vm = wrapper.vm as any

      expect(vm.aiFeatureToggles).toBeDefined()
      expect(Array.isArray(vm.aiFeatureToggles)).toBe(true)
      expect(vm.aiFeatureToggles.length).toBe(8)
    })

    it('should have smart suggestions feature', () => {
      const wrapper = mount(SettingsPage)
      const vm = wrapper.vm as any

      const suggestions = vm.aiFeatureToggles.find((f: any) => f.id === 'suggestions')
      expect(suggestions).toBeDefined()
      expect(suggestions.enabled).toBe(true)
    })

    it('should have translation languages', () => {
      const wrapper = mount(SettingsPage)
      const vm = wrapper.vm as any

      expect(vm.aiTranslationLanguages).toBeDefined()
      expect(vm.aiTranslationLanguages.length).toBeGreaterThan(0)
      expect(vm.aiTranslationLanguages.some((l: any) => l.code === 'en')).toBe(true)
      expect(vm.aiTranslationLanguages.some((l: any) => l.code === 'ar')).toBe(true)
    })

    it('should have AI preferences', () => {
      const wrapper = mount(SettingsPage)
      const vm = wrapper.vm as any

      expect(vm.aiPreferences).toBeDefined()
      expect(vm.aiPreferences.defaultTranslationLanguage).toBe('ar')
      expect(vm.aiPreferences.summaryLength).toBe('medium')
      expect(vm.aiPreferences.confidenceThreshold).toBe(0.7)
    })

    it('should have summary length options', () => {
      const wrapper = mount(SettingsPage)
      const vm = wrapper.vm as any

      expect(vm.summaryLengthOptions).toBeDefined()
      expect(vm.summaryLengthOptions.length).toBe(3)
    })

    it('should have AI usage stats', () => {
      const wrapper = mount(SettingsPage)
      const vm = wrapper.vm as any

      expect(vm.aiUsageStats).toBeDefined()
      expect(vm.aiUsageStats.summariesGenerated).toBe(156)
      expect(vm.aiUsageStats.translationsPerformed).toBe(89)
    })

    it('should test AI connection', async () => {
      vi.useFakeTimers()
      const wrapper = mount(SettingsPage)
      const vm = wrapper.vm as any

      vm.testAIConnection()

      expect(vm.isTestingAI).toBe(true)
      expect(vm.showAITestModal).toBe(true)

      vi.advanceTimersByTime(2000)
      await flushPromises()

      expect(vm.aiTestResults).toBeDefined()
      vi.useRealTimers()
    })

    it('should reset AI settings', () => {
      const wrapper = mount(SettingsPage)
      const vm = wrapper.vm as any

      // Mock alert
      const alertSpy = vi.spyOn(window, 'alert').mockImplementation(() => {})

      // Modify a setting
      vm.aiFeatureStates.suggestions = false
      vm.aiPreferences.summaryLength = 'detailed'

      // Reset
      vm.resetAISettings()

      expect(vm.aiFeatureStates.suggestions).toBe(true)
      expect(vm.aiPreferences.summaryLength).toBe('medium')

      alertSpy.mockRestore()
    })
  })

  describe('Notification Settings', () => {
    it('should have notification categories', () => {
      const wrapper = mount(SettingsPage)
      const vm = wrapper.vm as any

      expect(vm.notifications).toBeDefined()
      expect(vm.notifications.length).toBe(2)
      expect(vm.notifications[0].id).toBe('activity')
      expect(vm.notifications[1].id).toBe('updates')
    })

    it('should have notification items', () => {
      const wrapper = mount(SettingsPage)
      const vm = wrapper.vm as any

      const activityItems = vm.notifications[0].items
      expect(activityItems.length).toBe(3)
      expect(activityItems.some((i: any) => i.id === 'comments')).toBe(true)
      expect(activityItems.some((i: any) => i.id === 'mentions')).toBe(true)
    })

    it('should have email and push notification toggles', () => {
      const wrapper = mount(SettingsPage)
      const vm = wrapper.vm as any

      expect(vm.notificationSettings.comments.email).toBe(true)
      expect(vm.notificationSettings.comments.push).toBe(true)
    })
  })

  describe('Privacy Settings', () => {
    it('should have privacy options', () => {
      const wrapper = mount(SettingsPage)
      const vm = wrapper.vm as any

      expect(vm.privacySettings).toBeDefined()
      expect(vm.privacySettings.length).toBe(5)
    })

    it('should have profile visibility option', () => {
      const wrapper = mount(SettingsPage)
      const vm = wrapper.vm as any

      const profileOption = vm.privacySettings.find((p: any) => p.id === 'profile')
      expect(profileOption).toBeDefined()
      expect(profileOption.enabled).toBe(true)
    })

    it('should have analytics option disabled by default', () => {
      const wrapper = mount(SettingsPage)
      const vm = wrapper.vm as any

      const analyticsOption = vm.privacySettings.find((p: any) => p.id === 'analytics')
      expect(analyticsOption).toBeDefined()
      expect(analyticsOption.enabled).toBe(false)
    })
  })

  describe('Appearance Settings', () => {
    it('should have theme options', () => {
      const wrapper = mount(SettingsPage)
      const vm = wrapper.vm as any

      expect(vm.themes).toBeDefined()
      expect(vm.themes.length).toBe(3)
      expect(vm.themes.some((t: any) => t.id === 'light')).toBe(true)
      expect(vm.themes.some((t: any) => t.id === 'dark')).toBe(true)
      expect(vm.themes.some((t: any) => t.id === 'system')).toBe(true)
    })

    it('should have accent colors', () => {
      const wrapper = mount(SettingsPage)
      const vm = wrapper.vm as any

      expect(vm.accentColors).toBeDefined()
      expect(vm.accentColors.length).toBe(6)
    })

    it('should default to light theme', () => {
      const wrapper = mount(SettingsPage)
      const vm = wrapper.vm as any

      expect(vm.appearance.theme).toBe('light')
    })

    it('should default to teal accent', () => {
      const wrapper = mount(SettingsPage)
      const vm = wrapper.vm as any

      expect(vm.appearance.accent).toBe('teal')
    })

    it('should have animations enabled by default', () => {
      const wrapper = mount(SettingsPage)
      const vm = wrapper.vm as any

      expect(vm.appearance.animations).toBe(true)
    })
  })

  describe('Language Settings', () => {
    it('should have language options', () => {
      const wrapper = mount(SettingsPage)
      const vm = wrapper.vm as any

      expect(vm.language).toBeDefined()
      expect(vm.language.display).toBe('en')
    })

    it('should have date format options', () => {
      const wrapper = mount(SettingsPage)
      const vm = wrapper.vm as any

      expect(vm.language.dateFormat).toBe('dmy')
    })

    it('should have time format options', () => {
      const wrapper = mount(SettingsPage)
      const vm = wrapper.vm as any

      expect(vm.language.timeFormat).toBe('12')
    })
  })

  describe('Security Settings', () => {
    it('should have two-factor auth option', () => {
      const wrapper = mount(SettingsPage)
      const vm = wrapper.vm as any

      expect(vm.security).toBeDefined()
      expect(vm.security.twoFactor).toBe(false)
    })

    it('should have active sessions', () => {
      const wrapper = mount(SettingsPage)
      const vm = wrapper.vm as any

      expect(vm.sessions).toBeDefined()
      expect(vm.sessions.length).toBe(3)
    })

    it('should mark current session', () => {
      const wrapper = mount(SettingsPage)
      const vm = wrapper.vm as any

      const currentSession = vm.sessions.find((s: any) => s.current)
      expect(currentSession).toBeDefined()
      expect(currentSession.device).toContain('Chrome')
    })
  })

  describe('Connected Apps', () => {
    it('should have connected apps list', () => {
      const wrapper = mount(SettingsPage)
      const vm = wrapper.vm as any

      expect(vm.connectedApps).toBeDefined()
      expect(vm.connectedApps.length).toBe(3)
    })

    it('should include Google Workspace', () => {
      const wrapper = mount(SettingsPage)
      const vm = wrapper.vm as any

      const google = vm.connectedApps.find((a: any) => a.name === 'Google Workspace')
      expect(google).toBeDefined()
    })

    it('should include Microsoft 365', () => {
      const wrapper = mount(SettingsPage)
      const vm = wrapper.vm as any

      const microsoft = vm.connectedApps.find((a: any) => a.name === 'Microsoft 365')
      expect(microsoft).toBeDefined()
    })

    it('should include Slack', () => {
      const wrapper = mount(SettingsPage)
      const vm = wrapper.vm as any

      const slack = vm.connectedApps.find((a: any) => a.name === 'Slack')
      expect(slack).toBeDefined()
    })
  })

  describe('Keyboard Shortcuts', () => {
    it('should have shortcut categories', () => {
      const wrapper = mount(SettingsPage)
      const vm = wrapper.vm as any

      expect(vm.keyboardShortcuts).toBeDefined()
      expect(vm.keyboardShortcuts.length).toBe(4)
    })

    it('should have navigation shortcuts', () => {
      const wrapper = mount(SettingsPage)
      const vm = wrapper.vm as any

      const navCategory = vm.keyboardShortcuts.find((c: any) => c.category === 'Navigation')
      expect(navCategory).toBeDefined()
      expect(navCategory.shortcuts.length).toBeGreaterThan(0)
    })

    it('should have AI feature shortcuts', () => {
      const wrapper = mount(SettingsPage)
      const vm = wrapper.vm as any

      const aiCategory = vm.keyboardShortcuts.find((c: any) => c.category === 'AI Features')
      expect(aiCategory).toBeDefined()
    })

    it('should have shortcuts enabled by default', () => {
      const wrapper = mount(SettingsPage)
      const vm = wrapper.vm as any

      expect(vm.shortcutsEnabled).toBe(true)
    })
  })

  describe('Modal States', () => {
    it('should have change password modal state', () => {
      const wrapper = mount(SettingsPage)
      const vm = wrapper.vm as any

      expect(vm.showChangePassword).toBe(false)
    })

    it('should have AI test modal state', () => {
      const wrapper = mount(SettingsPage)
      const vm = wrapper.vm as any

      expect(vm.showAITestModal).toBe(false)
    })
  })
})
