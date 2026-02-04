<script setup lang="ts">
import { ref, computed } from 'vue'
import { useI18n } from 'vue-i18n'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'
import { useAIServicesStore } from '@/stores/aiServices'
import { AILoadingIndicator, AIConfidenceBar } from '@/components/ai'

const { t } = useI18n()
const aiStore = useAIServicesStore()

// Loading state
const isLoading = ref(false)

// Modal states
const showChangePassword = ref(false)

// Active tab
const activeTab = ref('account')

// Current user
const currentUser = ref({
  name: 'Ahmed Imam',
  role: 'Product Director',
  initials: 'AI',
  email: 'ahmed.imam@intalio.com'
})

// Tabs configuration
const tabs = computed(() => [
  { id: 'account', label: t('settings.tabs.account'), icon: 'fas fa-user' },
  { id: 'ai', label: t('settings.tabs.ai'), icon: 'fas fa-wand-magic-sparkles' },
  { id: 'notifications', label: t('settings.tabs.notifications'), icon: 'fas fa-bell' },
  { id: 'privacy', label: t('settings.tabs.privacy'), icon: 'fas fa-shield-alt' },
  { id: 'appearance', label: t('settings.tabs.appearance'), icon: 'fas fa-palette' },
  { id: 'language', label: t('settings.tabs.language'), icon: 'fas fa-globe' },
  { id: 'security', label: t('settings.tabs.security'), icon: 'fas fa-lock' },
  { id: 'apps', label: t('settings.tabs.apps'), icon: 'fas fa-plug' },
  { id: 'shortcuts', label: t('settings.tabs.shortcuts'), icon: 'fas fa-keyboard' }
])

// Account settings
const account = ref({
  firstName: 'Ahmed',
  lastName: 'Imam',
  email: 'ahmed.imam@intalio.com',
  phone: '+971 50 123 4567',
  timezone: 'UTC+4'
})

// Notification settings
interface NotificationItem {
  id: string
  label: string
  description: string
  email: boolean
  push: boolean
}

interface NotificationCategory {
  id: string
  title: string
  items: NotificationItem[]
}

const notificationSettings = ref({
  comments: { email: true, push: true },
  mentions: { email: true, push: true },
  reactions: { email: false, push: true },
  news: { email: true, push: true },
  events: { email: true, push: false },
  courses: { email: true, push: false }
})

const notifications = computed<NotificationCategory[]>(() => [
  {
    id: 'activity',
    title: t('settings.notificationCategories.activity'),
    items: [
      { id: 'comments', label: t('settings.notificationItems.comments'), description: t('settings.notificationItems.commentsDesc'), email: notificationSettings.value.comments.email, push: notificationSettings.value.comments.push },
      { id: 'mentions', label: t('settings.notificationItems.mentions'), description: t('settings.notificationItems.mentionsDesc'), email: notificationSettings.value.mentions.email, push: notificationSettings.value.mentions.push },
      { id: 'reactions', label: t('settings.notificationItems.reactions'), description: t('settings.notificationItems.reactionsDesc'), email: notificationSettings.value.reactions.email, push: notificationSettings.value.reactions.push }
    ]
  },
  {
    id: 'updates',
    title: t('settings.notificationCategories.updates'),
    items: [
      { id: 'news', label: t('settings.notificationItems.news'), description: t('settings.notificationItems.newsDesc'), email: notificationSettings.value.news.email, push: notificationSettings.value.news.push },
      { id: 'events', label: t('settings.notificationItems.events'), description: t('settings.notificationItems.eventsDesc'), email: notificationSettings.value.events.email, push: notificationSettings.value.events.push },
      { id: 'courses', label: t('settings.notificationItems.courses'), description: t('settings.notificationItems.coursesDesc'), email: notificationSettings.value.courses.email, push: notificationSettings.value.courses.push }
    ]
  }
])

// Privacy settings
const privacySettingsState = ref({
  profile: true,
  activity: true,
  contributions: true,
  search: true,
  analytics: false
})

const privacySettings = computed(() => [
  { id: 'profile', label: t('settings.privacyOptions.publicProfile'), description: t('settings.privacyOptions.publicProfileDesc'), enabled: privacySettingsState.value.profile },
  { id: 'activity', label: t('settings.privacyOptions.showActivity'), description: t('settings.privacyOptions.showActivityDesc'), enabled: privacySettingsState.value.activity },
  { id: 'contributions', label: t('settings.privacyOptions.showContributions'), description: t('settings.privacyOptions.showContributionsDesc'), enabled: privacySettingsState.value.contributions },
  { id: 'search', label: t('settings.privacyOptions.appearInSearch'), description: t('settings.privacyOptions.appearInSearchDesc'), enabled: privacySettingsState.value.search },
  { id: 'analytics', label: t('settings.privacyOptions.usageAnalytics'), description: t('settings.privacyOptions.usageAnalyticsDesc'), enabled: privacySettingsState.value.analytics }
])

// Appearance settings
const appearance = ref({
  theme: 'light',
  accent: 'teal',
  fontSize: 14,
  compact: false,
  animations: true
})

// Themes
const themes = computed(() => [
  { id: 'light', label: t('settings.themes.light'), icon: 'fas fa-sun' },
  { id: 'dark', label: t('settings.themes.dark'), icon: 'fas fa-moon' },
  { id: 'system', label: t('settings.themes.system'), icon: 'fas fa-laptop' }
])

// Accent colors
const accentColors = ref([
  { id: 'teal', value: '#14b8a6' },
  { id: 'blue', value: '#3b82f6' },
  { id: 'purple', value: '#8b5cf6' },
  { id: 'pink', value: '#ec4899' },
  { id: 'orange', value: '#f97316' },
  { id: 'green', value: '#22c55e' }
])

// Language settings
const language = ref({
  display: 'en',
  dateFormat: 'dmy',
  timeFormat: '12',
  firstDay: 'sunday'
})

// Security settings
const security = ref({
  twoFactor: false
})

// Sessions
const sessions = ref([
  { id: 1, device: 'Chrome on MacOS', location: 'Dubai, UAE', lastActive: 'Now', icon: 'fab fa-chrome', current: true },
  { id: 2, device: 'Safari on iPhone', location: 'Dubai, UAE', lastActive: '2 hours ago', icon: 'fab fa-safari', current: false },
  { id: 3, device: 'Firefox on Windows', location: 'Abu Dhabi, UAE', lastActive: 'Yesterday', icon: 'fab fa-firefox', current: false }
])

// Connected apps
const connectedApps = ref([
  { id: 1, name: 'Google Workspace', icon: 'fab fa-google', bg: 'bg-red-100', color: 'text-red-600', connectedDate: 'Oct 2024' },
  { id: 2, name: 'Microsoft 365', icon: 'fab fa-microsoft', bg: 'bg-blue-100', color: 'text-blue-600', connectedDate: 'Sep 2024' },
  { id: 3, name: 'Slack', icon: 'fab fa-slack', bg: 'bg-purple-100', color: 'text-purple-600', connectedDate: 'Aug 2024' }
])

// Keyboard shortcuts
const keyboardShortcuts = computed(() => [
  {
    category: t('settings.shortcutCategories.navigation'),
    shortcuts: [
      { keys: ['G', 'H'], description: t('settings.shortcuts.goToHome') },
      { keys: ['G', 'A'], description: t('settings.shortcuts.goToArticles') },
      { keys: ['G', 'D'], description: t('settings.shortcuts.goToDocuments') },
      { keys: ['G', 'E'], description: t('settings.shortcuts.goToEvents') },
      { keys: ['G', 'P'], description: t('settings.shortcuts.goToProfile') },
      { keys: ['G', 'S'], description: t('settings.shortcuts.goToSettings') }
    ]
  },
  {
    category: t('settings.shortcutCategories.searchActions'),
    shortcuts: [
      { keys: ['/', ''], description: t('settings.shortcuts.focusSearch') },
      { keys: ['Ctrl', 'K'], description: t('settings.shortcuts.openCommandPalette') },
      { keys: ['N'], description: t('settings.shortcuts.createNewItem') },
      { keys: ['Esc'], description: t('settings.shortcuts.closeModal') }
    ]
  },
  {
    category: t('settings.shortcutCategories.content'),
    shortcuts: [
      { keys: ['E'], description: t('settings.shortcuts.editItem') },
      { keys: ['B'], description: t('settings.shortcuts.bookmarkItem') },
      { keys: ['S'], description: t('settings.shortcuts.shareItem') },
      { keys: ['Ctrl', 'Enter'], description: t('settings.shortcuts.submitForm') }
    ]
  },
  {
    category: t('settings.shortcutCategories.aiFeatures'),
    shortcuts: [
      { keys: ['Ctrl', 'Shift', 'A'], description: t('settings.shortcuts.openAIAssistant') },
      { keys: ['Ctrl', 'Shift', 'S'], description: t('settings.shortcuts.aiSummarize') },
      { keys: ['Ctrl', 'Shift', 'T'], description: t('settings.shortcuts.aiTranslate') }
    ]
  }
])

const shortcutsEnabled = ref(true)

// ==================== AI SETTINGS ====================

// AI Settings State
const showAITestModal = ref(false)
const isTestingAI = ref(false)
const aiTestResults = ref<AITestResult | null>(null)

// AI Settings Interfaces
interface AITestResult {
  service: string
  status: 'success' | 'error'
  responseTime: number
  message: string
}

interface AIFeatureToggle {
  id: string
  label: string
  description: string
  enabled: boolean
  icon: string
}

interface AITranslationLanguage {
  code: string
  name: string
  nativeName: string
}

// AI Feature Toggles State
const aiFeatureStates = ref({
  suggestions: true,
  summarization: true,
  translation: true,
  chatbot: true,
  classification: true,
  ocr: true,
  sentiment: false,
  recommendations: true
})

// AI Feature Toggles
const aiFeatureToggles = computed<AIFeatureToggle[]>(() => [
  {
    id: 'suggestions',
    label: t('settings.aiFeatureToggles.smartSuggestions'),
    description: t('settings.aiFeatureToggles.smartSuggestionsDesc'),
    enabled: aiFeatureStates.value.suggestions,
    icon: 'fas fa-lightbulb'
  },
  {
    id: 'summarization',
    label: t('settings.aiFeatureToggles.autoSummarization'),
    description: t('settings.aiFeatureToggles.autoSummarizationDesc'),
    enabled: aiFeatureStates.value.summarization,
    icon: 'fas fa-compress-alt'
  },
  {
    id: 'translation',
    label: t('settings.aiFeatureToggles.aiTranslation'),
    description: t('settings.aiFeatureToggles.aiTranslationDesc'),
    enabled: aiFeatureStates.value.translation,
    icon: 'fas fa-language'
  },
  {
    id: 'chatbot',
    label: t('settings.aiFeatureToggles.aiAssistantChat'),
    description: t('settings.aiFeatureToggles.aiAssistantChatDesc'),
    enabled: aiFeatureStates.value.chatbot,
    icon: 'fas fa-robot'
  },
  {
    id: 'classification',
    label: t('settings.aiFeatureToggles.autoClassification'),
    description: t('settings.aiFeatureToggles.autoClassificationDesc'),
    enabled: aiFeatureStates.value.classification,
    icon: 'fas fa-tags'
  },
  {
    id: 'ocr',
    label: t('settings.aiFeatureToggles.documentOCR'),
    description: t('settings.aiFeatureToggles.documentOCRDesc'),
    enabled: aiFeatureStates.value.ocr,
    icon: 'fas fa-file-image'
  },
  {
    id: 'sentiment',
    label: t('settings.aiFeatureToggles.sentimentAnalysis'),
    description: t('settings.aiFeatureToggles.sentimentAnalysisDesc'),
    enabled: aiFeatureStates.value.sentiment,
    icon: 'fas fa-smile'
  },
  {
    id: 'recommendations',
    label: t('settings.aiFeatureToggles.personalizedRecommendations'),
    description: t('settings.aiFeatureToggles.personalizedRecommendationsDesc'),
    enabled: aiFeatureStates.value.recommendations,
    icon: 'fas fa-star'
  }
])

// AI Translation Languages
const aiTranslationLanguages = ref<AITranslationLanguage[]>([
  { code: 'en', name: 'English', nativeName: 'English' },
  { code: 'ar', name: 'Arabic', nativeName: 'العربية' },
  { code: 'fr', name: 'French', nativeName: 'Français' },
  { code: 'es', name: 'Spanish', nativeName: 'Español' },
  { code: 'de', name: 'German', nativeName: 'Deutsch' },
  { code: 'zh', name: 'Chinese', nativeName: '中文' },
  { code: 'ja', name: 'Japanese', nativeName: '日本語' },
  { code: 'ko', name: 'Korean', nativeName: '한국어' },
  { code: 'pt', name: 'Portuguese', nativeName: 'Português' },
  { code: 'ru', name: 'Russian', nativeName: 'Русский' }
])

// AI Preferences
const aiPreferences = ref({
  defaultTranslationLanguage: 'ar',
  summaryLength: 'medium',
  confidenceThreshold: 0.7,
  autoApplySuggestions: false,
  showConfidenceScores: true,
  dataUsageConsent: true
})

// Summary Length Options
const summaryLengthOptions = computed(() => [
  { id: 'brief', label: t('settings.summaryLengthOptions.brief'), description: t('settings.summaryLengthOptions.briefDesc') },
  { id: 'medium', label: t('settings.summaryLengthOptions.medium'), description: t('settings.summaryLengthOptions.mediumDesc') },
  { id: 'detailed', label: t('settings.summaryLengthOptions.detailed'), description: t('settings.summaryLengthOptions.detailedDesc') }
])

// AI Usage Stats (Mock)
const aiUsageStats = ref({
  summariesGenerated: 156,
  translationsPerformed: 89,
  documentsScanned: 42,
  chatMessages: 324,
  suggestionsAccepted: 78,
  lastUsed: '2 hours ago'
})

// AI Functions
async function testAIConnection() {
  isTestingAI.value = true
  showAITestModal.value = true
  aiTestResults.value = null

  try {
    await new Promise(resolve => setTimeout(resolve, 1500))
    aiTestResults.value = {
      service: 'Intalio AI Engine',
      status: 'success',
      responseTime: 245,
      message: 'All AI services are operational and responding normally.'
    }
  } catch (error) {
    aiTestResults.value = {
      service: 'Intalio AI Engine',
      status: 'error',
      responseTime: 0,
      message: 'Failed to connect to AI services. Please try again later.'
    }
  } finally {
    isTestingAI.value = false
  }
}

function saveAISettings() {
  alert('AI settings saved successfully!')
}

function resetAISettings() {
  aiFeatureToggles.value.forEach(toggle => toggle.enabled = true)
  aiPreferences.value = {
    defaultTranslationLanguage: 'ar',
    summaryLength: 'medium',
    confidenceThreshold: 0.7,
    autoApplySuggestions: false,
    showConfidenceScores: true,
    dataUsageConsent: true
  }
  alert('AI settings reset to defaults!')
}

function clearAIHistory() {
  if (confirm('Are you sure you want to clear your AI interaction history? This action cannot be undone.')) {
    alert('AI history cleared successfully!')
  }
}

// Methods
function changePassword() {
  showChangePassword.value = false
  alert('Password updated successfully!')
}
</script>

<template>
  <div class="min-h-screen px-6 py-6">
    <!-- Loading State -->
    <div v-if="isLoading" class="flex items-center justify-center min-h-[60vh]">
      <LoadingSpinner size="lg" :text="$t('settings.loadingSettings')" />
    </div>

    <div v-else>
      <!-- Back Button & Breadcrumb -->
      <div class="flex items-center gap-4 mb-6">
        <button @click="$router.back()" class="w-10 h-10 rounded-xl bg-white border border-gray-200 flex items-center justify-center text-gray-600 hover:bg-gray-50 hover:border-gray-300 transition-all shadow-sm">
          <i class="fas fa-arrow-left"></i>
        </button>
        <nav class="flex items-center gap-2 text-sm text-gray-500">
          <router-link to="/" class="hover:text-teal-600 transition-colors">
            <i class="fas fa-home"></i>
          </router-link>
          <i class="fas fa-chevron-right text-xs"></i>
          <span class="text-gray-900 font-medium">{{ $t('settings.title') }}</span>
        </nav>
      </div>

      <!-- Enhanced Header with User Card -->
      <div class="card-animated rounded-2xl overflow-hidden mb-6 fade-in-up">
        <div class="bg-gradient-to-r from-teal-500 via-teal-600 to-emerald-600 p-6">
          <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
            <!-- User Info -->
            <div class="flex items-center gap-4">
              <div class="relative">
                <div class="w-16 h-16 rounded-2xl bg-white/20 backdrop-blur flex items-center justify-center text-white text-2xl font-bold border-2 border-white/30">
                  {{ currentUser.initials }}
                </div>
                <span class="absolute -bottom-1 -right-1 w-5 h-5 bg-green-400 rounded-full border-2 border-white"></span>
              </div>
              <div>
                <h1 class="text-xl font-bold text-white">{{ currentUser.name }}</h1>
                <p class="text-teal-100">{{ currentUser.role }}</p>
                <p class="text-teal-200 text-sm">{{ currentUser.email }}</p>
              </div>
            </div>

            <!-- Quick Actions -->
            <div class="flex items-center gap-2">
              <router-link to="/profile" class="px-4 py-2 bg-white/20 backdrop-blur text-white rounded-xl font-medium text-sm hover:bg-white/30 transition-all flex items-center gap-2">
                <i class="fas fa-user"></i>
                {{ $t('user.viewProfile') }}
              </router-link>
              <button class="px-4 py-2 bg-white text-teal-600 rounded-xl font-medium text-sm hover:bg-teal-50 transition-all flex items-center gap-2 shadow-lg">
                <i class="fas fa-download"></i>
                {{ $t('settings.dataExport') }}
              </button>
            </div>
          </div>

          <!-- Quick Stats -->
          <div class="grid grid-cols-2 sm:grid-cols-4 gap-3 mt-6">
            <div class="bg-white/10 backdrop-blur rounded-xl p-3 text-center">
              <p class="text-2xl font-bold text-white">89%</p>
              <p class="text-xs text-teal-100">{{ $t('settings.profileComplete') }}</p>
            </div>
            <div class="bg-white/10 backdrop-blur rounded-xl p-3 text-center">
              <p class="text-2xl font-bold text-white">12</p>
              <p class="text-xs text-teal-100">{{ $t('settings.connectedApps') }}</p>
            </div>
            <div class="bg-white/10 backdrop-blur rounded-xl p-3 text-center">
              <p class="text-2xl font-bold text-white">3</p>
              <p class="text-xs text-teal-100">{{ $t('settings.activeSessions') }}</p>
            </div>
            <div class="bg-white/10 backdrop-blur rounded-xl p-3 text-center">
              <p class="text-2xl font-bold text-white flex items-center justify-center gap-1">
                <i class="fas fa-shield-check text-green-300 text-lg"></i>
              </p>
              <p class="text-xs text-teal-100">{{ $t('settings.securityStrong') }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Mobile Tab Navigation -->
      <div class="settings-mobile-tabs mb-6 overflow-x-auto hide-scrollbar">
        <div class="flex gap-2 pb-2">
          <button
            v-for="tab in tabs"
            :key="tab.id"
            @click="activeTab = tab.id"
            :class="[
              'flex items-center gap-2 px-4 py-2.5 rounded-xl font-medium text-sm whitespace-nowrap transition-all',
              activeTab === tab.id
                ? 'bg-teal-500 text-white shadow-lg'
                : 'bg-white text-teal-700 border border-teal-100 hover:border-teal-300'
            ]"
          >
            <i :class="tab.icon"></i>
            {{ tab.label }}
          </button>
        </div>
      </div>

      <div class="flex gap-6">
        <!-- Settings Navigation -->
        <aside class="settings-sidebar w-72 flex-shrink-0 fade-in-up" style="animation-delay: 0.1s;">
          <div class="sticky top-24">
            <!-- Sidebar Header -->
            <div class="sidebar-header rounded-t-2xl p-5 bg-gradient-to-br from-teal-500 via-teal-600 to-emerald-600">
              <div class="flex items-center gap-3">
                <div class="w-10 h-10 rounded-xl bg-white/20 backdrop-blur flex items-center justify-center">
                  <i class="fas fa-cog text-white text-lg"></i>
                </div>
                <div>
                  <h3 class="text-white font-semibold">{{ $t('settings.title') }}</h3>
                  <p class="text-teal-100 text-xs">{{ $t('settings.managePreferences') }}</p>
                </div>
              </div>
            </div>

            <!-- Navigation Items -->
            <div class="bg-white rounded-b-2xl shadow-lg border border-gray-100 border-t-0 overflow-hidden">
              <nav class="p-3 space-y-1">
                <button
                  v-for="(tab, index) in tabs"
                  :key="tab.id"
                  @click="activeTab = tab.id"
                  :class="[
                    'sidebar-nav-item w-full flex items-center gap-3 px-4 py-3 rounded-xl text-start transition-all',
                    activeTab === tab.id
                      ? 'bg-gradient-to-r from-teal-500 to-emerald-500 text-white shadow-md shadow-teal-200'
                      : 'text-gray-700 hover:bg-gray-50'
                  ]"
                  :style="{ animationDelay: `${0.2 + index * 0.05}s` }"
                >
                  <div :class="[
                    'w-9 h-9 rounded-lg flex items-center justify-center transition-all',
                    activeTab === tab.id
                      ? 'bg-white/20'
                      : 'bg-gray-100 group-hover:bg-teal-100'
                  ]">
                    <i :class="[tab.icon, 'text-sm', activeTab === tab.id ? 'text-white' : 'text-teal-600']"></i>
                  </div>
                  <span class="font-medium text-sm">{{ tab.label }}</span>
                  <i v-if="activeTab === tab.id" class="fas fa-chevron-right ms-auto text-xs text-white/70"></i>
                </button>
              </nav>

              <!-- Sidebar Footer -->
              <div class="p-4 border-t border-gray-100 bg-gray-50/50">
                <div class="flex items-center gap-2 text-xs text-gray-500">
                  <i class="fas fa-shield-check text-teal-500"></i>
                  <span>{{ $t('settings.secureSettings') }}</span>
                </div>
              </div>
            </div>
          </div>
        </aside>

        <!-- Settings Content -->
        <div class="flex-1">
          <!-- Account Settings -->
          <div v-if="activeTab === 'account'" class="card-animated rounded-2xl overflow-hidden fade-in-up" style="animation-delay: 0.2s;">
            <div class="settings-tab-header p-5 bg-gradient-to-r from-teal-500 to-emerald-500">
              <div class="flex items-center gap-4">
                <div class="w-12 h-12 bg-white/20 backdrop-blur rounded-xl flex items-center justify-center">
                  <i class="fas fa-user text-white text-xl"></i>
                </div>
                <div>
                  <h2 class="text-lg font-semibold text-white">{{ $t('settings.accountSettings') }}</h2>
                  <p class="text-sm text-teal-100">{{ $t('settings.manageAccountInfo') }}</p>
                </div>
              </div>
            </div>
            <div class="p-6 space-y-6">
              <div class="flex items-center gap-6">
                <div class="relative">
                  <div class="w-20 h-20 rounded-2xl bg-gradient-to-br from-teal-400 to-teal-600 flex items-center justify-center text-white text-2xl font-bold">
                    {{ currentUser.initials }}
                  </div>
                  <button class="absolute -bottom-1 -right-1 w-8 h-8 rounded-full bg-teal-500 text-white flex items-center justify-center hover:bg-teal-600 ripple">
                    <i class="fas fa-camera text-sm icon-vibrant"></i>
                  </button>
                </div>
                <div>
                  <p class="font-medium text-teal-900">{{ $t('settings.profilePhoto') }}</p>
                  <p class="text-sm text-teal-500 mb-2">{{ $t('settings.photoHint') }}</p>
                  <button class="btn btn-secondary btn-sm ripple">{{ $t('settings.uploadPhoto') }}</button>
                </div>
              </div>

              <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <label class="block text-sm font-medium text-teal-700 mb-2">{{ $t('settings.firstName') }}</label>
                  <input type="text" v-model="account.firstName" class="input">
                </div>
                <div>
                  <label class="block text-sm font-medium text-teal-700 mb-2">{{ $t('settings.lastName') }}</label>
                  <input type="text" v-model="account.lastName" class="input">
                </div>
              </div>

              <div>
                <label class="block text-sm font-medium text-teal-700 mb-2">{{ $t('settings.emailAddress') }}</label>
                <input type="email" v-model="account.email" class="input">
              </div>

              <div>
                <label class="block text-sm font-medium text-teal-700 mb-2">{{ $t('settings.phoneNumber') }}</label>
                <input type="tel" v-model="account.phone" class="input">
              </div>

              <div>
                <label class="block text-sm font-medium text-teal-700 mb-2">{{ $t('settings.timeZone') }}</label>
                <select v-model="account.timezone" class="input">
                  <option value="UTC+4">Gulf Standard Time (UTC+4)</option>
                  <option value="UTC+0">UTC</option>
                  <option value="UTC-5">Eastern Time (UTC-5)</option>
                  <option value="UTC-8">Pacific Time (UTC-8)</option>
                </select>
              </div>

              <div class="pt-4 border-t border-teal-100 flex justify-end gap-3">
                <button class="btn btn-secondary ripple">{{ $t('settings.cancel') }}</button>
                <button class="btn-vibrant ripple">{{ $t('settings.saveChanges') }}</button>
              </div>
            </div>
          </div>

          <!-- AI Settings -->
          <div v-if="activeTab === 'ai'" class="space-y-6">
            <!-- AI Settings Header Card -->
            <div class="card-animated rounded-2xl overflow-hidden fade-in-up" style="animation-delay: 0.2s;">
              <div class="settings-tab-header p-5 bg-gradient-to-r from-teal-500 to-emerald-500">
                <div class="flex items-center justify-between">
                  <div class="flex items-center gap-4">
                    <div class="w-12 h-12 bg-white/20 backdrop-blur rounded-xl flex items-center justify-center">
                      <i class="fas fa-wand-magic-sparkles text-white text-xl"></i>
                    </div>
                    <div>
                      <h2 class="text-lg font-semibold text-white">{{ $t('settings.aiSettings') }}</h2>
                      <p class="text-sm text-teal-100">{{ $t('settings.manageAIFeatures') }}</p>
                    </div>
                  </div>
                  <button @click="testAIConnection"
                          class="px-4 py-2 bg-white/20 backdrop-blur text-white rounded-xl font-medium text-sm hover:bg-white/30 transition-all flex items-center gap-2">
                    <i class="fas fa-plug"></i>
                    {{ $t('settings.testConnection') }}
                  </button>
                </div>
              </div>
            </div>

            <!-- AI Feature Toggles -->
            <div class="card-animated rounded-2xl overflow-hidden fade-in-up" style="animation-delay: 0.3s;">
              <div class="p-6 border-b border-teal-100">
                <h3 class="text-lg font-semibold text-teal-900">{{ $t('settings.aiFeatures') }}</h3>
                <p class="text-sm text-teal-500">{{ $t('settings.enableDisableAI') }}</p>
              </div>
              <div class="p-6 space-y-4">
                <div v-for="feature in aiFeatureToggles" :key="feature.id"
                     class="flex items-center justify-between py-3 border-b border-teal-100 last:border-0 list-item-animated">
                  <div class="flex items-center gap-4">
                    <div class="w-10 h-10 rounded-xl bg-teal-100 flex items-center justify-center">
                      <i :class="[feature.icon, 'text-teal-600']"></i>
                    </div>
                    <div>
                      <p class="font-medium text-teal-800">{{ feature.label }}</p>
                      <p class="text-sm text-teal-500">{{ feature.description }}</p>
                    </div>
                  </div>
                  <label class="toggle">
                    <input type="checkbox" v-model="feature.enabled">
                    <span class="toggle-slider"></span>
                  </label>
                </div>
              </div>
            </div>

            <!-- Translation Preferences -->
            <div class="card-animated rounded-2xl overflow-hidden fade-in-up" style="animation-delay: 0.4s;">
              <div class="p-6 border-b border-teal-100">
                <h3 class="text-lg font-semibold text-teal-900">{{ $t('settings.translationPreferences') }}</h3>
                <p class="text-sm text-teal-500">{{ $t('settings.configureLanguages') }}</p>
              </div>
              <div class="p-6 space-y-6">
                <div>
                  <label class="block text-sm font-medium text-teal-700 mb-2">{{ $t('settings.defaultTranslationLanguage') }}</label>
                  <select v-model="aiPreferences.defaultTranslationLanguage" class="input">
                    <option v-for="lang in aiTranslationLanguages" :key="lang.code" :value="lang.code">
                      {{ lang.name }} ({{ lang.nativeName }})
                    </option>
                  </select>
                  <p class="text-xs text-teal-500 mt-1">{{ $t('settings.translationLanguageHint') }}</p>
                </div>

                <div>
                  <label class="block text-sm font-medium text-teal-700 mb-3">{{ $t('settings.preferredSummaryLength') }}</label>
                  <div class="grid grid-cols-3 gap-4">
                    <label v-for="option in summaryLengthOptions" :key="option.id"
                           :class="[
                             'p-4 rounded-xl border-2 cursor-pointer transition-all text-center ripple',
                             aiPreferences.summaryLength === option.id ? 'border-teal-500 bg-teal-50' : 'border-teal-100 hover:border-teal-200'
                           ]">
                      <input type="radio" v-model="aiPreferences.summaryLength" :value="option.id" class="hidden">
                      <p class="font-medium" :class="aiPreferences.summaryLength === option.id ? 'text-teal-700' : 'text-teal-500'">{{ option.label }}</p>
                      <p class="text-xs text-teal-400 mt-1">{{ option.description }}</p>
                    </label>
                  </div>
                </div>
              </div>
            </div>

            <!-- AI Behavior Preferences -->
            <div class="card-animated rounded-2xl overflow-hidden fade-in-up" style="animation-delay: 0.5s;">
              <div class="p-6 border-b border-teal-100">
                <h3 class="text-lg font-semibold text-teal-900">{{ $t('settings.aiBehavior') }}</h3>
                <p class="text-sm text-teal-500">{{ $t('settings.customizeAIBehavior') }}</p>
              </div>
              <div class="p-6 space-y-6">
                <!-- Confidence Threshold -->
                <div>
                  <div class="flex items-center justify-between mb-2">
                    <label class="text-sm font-medium text-teal-700">{{ $t('settings.confidenceThreshold') }}</label>
                    <span class="text-sm font-medium text-teal-600">{{ Math.round(aiPreferences.confidenceThreshold * 100) }}%</span>
                  </div>
                  <input type="range" v-model.number="aiPreferences.confidenceThreshold" min="0.5" max="0.95" step="0.05"
                         class="w-full h-2 bg-teal-100 rounded-lg appearance-none cursor-pointer">
                  <p class="text-xs text-teal-500 mt-1">{{ $t('settings.confidenceThresholdHint') }}</p>
                </div>

                <!-- Toggle Preferences -->
                <div class="space-y-4">
                  <div class="flex items-center justify-between py-3 border-b border-teal-100">
                    <div>
                      <p class="font-medium text-teal-800">{{ $t('settings.showConfidenceScores') }}</p>
                      <p class="text-sm text-teal-500">{{ $t('settings.showConfidenceScoresDesc') }}</p>
                    </div>
                    <label class="toggle">
                      <input type="checkbox" v-model="aiPreferences.showConfidenceScores">
                      <span class="toggle-slider"></span>
                    </label>
                  </div>

                  <div class="flex items-center justify-between py-3 border-b border-teal-100">
                    <div>
                      <p class="font-medium text-teal-800">{{ $t('settings.autoApplySuggestions') }}</p>
                      <p class="text-sm text-teal-500">{{ $t('settings.autoApplySuggestionsDesc') }}</p>
                    </div>
                    <label class="toggle">
                      <input type="checkbox" v-model="aiPreferences.autoApplySuggestions">
                      <span class="toggle-slider"></span>
                    </label>
                  </div>

                  <div class="flex items-center justify-between py-3">
                    <div>
                      <p class="font-medium text-teal-800">{{ $t('settings.aiDataUsageConsent') }}</p>
                      <p class="text-sm text-teal-500">{{ $t('settings.aiDataUsageConsentDesc') }}</p>
                    </div>
                    <label class="toggle">
                      <input type="checkbox" v-model="aiPreferences.dataUsageConsent">
                      <span class="toggle-slider"></span>
                    </label>
                  </div>
                </div>
              </div>
            </div>

            <!-- Data & Privacy -->
            <div class="card-animated rounded-2xl overflow-hidden fade-in-up" style="animation-delay: 0.6s;">
              <div class="p-6 border-b border-teal-100">
                <h3 class="text-lg font-semibold text-teal-900">{{ $t('settings.aiDataPrivacy') }}</h3>
                <p class="text-sm text-teal-500">{{ $t('settings.manageAIData') }}</p>
              </div>
              <div class="p-6 space-y-4">
                <div class="p-4 bg-teal-50 rounded-xl">
                  <div class="flex items-start gap-4">
                    <div class="w-10 h-10 rounded-xl bg-teal-100 flex items-center justify-center flex-shrink-0">
                      <i class="fas fa-database text-teal-600"></i>
                    </div>
                    <div class="flex-1">
                      <p class="font-medium text-teal-900">{{ $t('settings.aiInteractionHistory') }}</p>
                      <p class="text-sm text-teal-600 mb-3">{{ $t('settings.aiInteractionHistoryDesc') }}</p>
                      <div class="flex gap-3">
                        <button class="px-4 py-2 text-sm font-medium text-teal-600 bg-white rounded-lg hover:bg-teal-100 transition-colors">
                          <i class="fas fa-download mr-2"></i>{{ $t('settings.exportData') }}
                        </button>
                        <button @click="clearAIHistory"
                                class="px-4 py-2 text-sm font-medium text-red-600 bg-red-50 rounded-lg hover:bg-red-100 transition-colors">
                          <i class="fas fa-trash mr-2"></i>{{ $t('settings.clearHistory') }}
                        </button>
                      </div>
                    </div>
                  </div>
                </div>

                <div class="p-4 bg-blue-50 rounded-xl border border-blue-200">
                  <div class="flex items-start gap-3">
                    <i class="fas fa-info-circle text-blue-500 mt-0.5"></i>
                    <div>
                      <p class="text-sm text-blue-800 font-medium">{{ $t('settings.aboutAIData') }}</p>
                      <p class="text-sm text-blue-600 mt-1">{{ $t('settings.aboutAIDataDesc') }}</p>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- Actions -->
            <div class="flex justify-end gap-3 pt-4">
              <button @click="resetAISettings" class="btn btn-secondary ripple">
                <i class="fas fa-undo mr-2"></i>{{ $t('settings.resetToDefaults') }}
              </button>
              <button @click="saveAISettings" class="btn-vibrant ripple">
                <i class="fas fa-check mr-2"></i>{{ $t('settings.saveAISettings') }}
              </button>
            </div>
          </div>

          <!-- Notifications -->
          <div v-if="activeTab === 'notifications'" class="card-animated rounded-2xl overflow-hidden fade-in-up" style="animation-delay: 0.2s;">
            <div class="settings-tab-header p-5 bg-gradient-to-r from-teal-500 to-emerald-500">
              <div class="flex items-center gap-4">
                <div class="w-12 h-12 bg-white/20 backdrop-blur rounded-xl flex items-center justify-center">
                  <i class="fas fa-bell text-white text-xl"></i>
                </div>
                <div>
                  <h2 class="text-lg font-semibold text-white">{{ $t('settings.notificationPreferences') }}</h2>
                  <p class="text-sm text-teal-100">{{ $t('settings.chooseNotifications') }}</p>
                </div>
              </div>
            </div>
            <div class="p-6 space-y-6">
              <div v-for="category in notifications" :key="category.id" class="border-b border-teal-100 last:border-0 pb-6 last:pb-0">
                <h3 class="font-medium text-teal-900 mb-4">{{ category.title }}</h3>
                <div class="space-y-4">
                  <div
                    v-for="item in category.items"
                    :key="item.id"
                    class="flex items-center justify-between py-2 list-item-animated"
                  >
                    <div>
                      <p class="text-teal-800">{{ item.label }}</p>
                      <p class="text-sm text-teal-500">{{ item.description }}</p>
                    </div>
                    <div class="flex items-center gap-4">
                      <label class="toggle">
                        <input type="checkbox" v-model="notificationSettings[item.id as keyof typeof notificationSettings].email">
                        <span class="toggle-slider"></span>
                      </label>
                      <span class="text-xs text-teal-500 w-12">{{ $t('settings.email') }}</span>
                      <label class="toggle">
                        <input type="checkbox" v-model="notificationSettings[item.id as keyof typeof notificationSettings].push">
                        <span class="toggle-slider"></span>
                      </label>
                      <span class="text-xs text-teal-500 w-12">{{ $t('settings.push') }}</span>
                    </div>
                  </div>
                </div>
              </div>

              <div class="pt-4 flex justify-end gap-3">
                <button class="btn btn-secondary ripple">{{ $t('settings.resetToDefault') }}</button>
                <button class="btn-vibrant ripple">{{ $t('settings.savePreferences') }}</button>
              </div>
            </div>
          </div>

          <!-- Privacy -->
          <div v-if="activeTab === 'privacy'" class="card-animated rounded-2xl overflow-hidden fade-in-up" style="animation-delay: 0.2s;">
            <div class="settings-tab-header p-5 bg-gradient-to-r from-teal-500 to-emerald-500">
              <div class="flex items-center gap-4">
                <div class="w-12 h-12 bg-white/20 backdrop-blur rounded-xl flex items-center justify-center">
                  <i class="fas fa-shield-alt text-white text-xl"></i>
                </div>
                <div>
                  <h2 class="text-lg font-semibold text-white">{{ $t('settings.privacySettings') }}</h2>
                  <p class="text-sm text-teal-100">{{ $t('settings.controlPrivacy') }}</p>
                </div>
              </div>
            </div>
            <div class="p-6 space-y-6">
              <div
                v-for="setting in privacySettings"
                :key="setting.id"
                class="flex items-center justify-between py-3 border-b border-teal-100 last:border-0 list-item-animated"
              >
                <div>
                  <p class="font-medium text-teal-800">{{ setting.label }}</p>
                  <p class="text-sm text-teal-500">{{ setting.description }}</p>
                </div>
                <label class="toggle">
                  <input type="checkbox" v-model="privacySettingsState[setting.id as keyof typeof privacySettingsState]">
                  <span class="toggle-slider"></span>
                </label>
              </div>

              <div class="p-4 bg-red-50 rounded-xl border border-red-200">
                <h4 class="font-medium text-red-800 mb-2"><i class="fas fa-exclamation-triangle mr-2 icon-soft"></i>{{ $t('settings.dangerZone') }}</h4>
                <p class="text-sm text-red-600 mb-3">{{ $t('settings.deleteAccountWarning') }}</p>
                <button class="btn bg-red-500 hover:bg-red-600 text-white btn-sm ripple">{{ $t('settings.deleteAccount') }}</button>
              </div>
            </div>
          </div>

          <!-- Appearance -->
          <div v-if="activeTab === 'appearance'" class="card-animated rounded-2xl overflow-hidden fade-in-up" style="animation-delay: 0.2s;">
            <div class="settings-tab-header p-5 bg-gradient-to-r from-teal-500 to-emerald-500">
              <div class="flex items-center gap-4">
                <div class="w-12 h-12 bg-white/20 backdrop-blur rounded-xl flex items-center justify-center">
                  <i class="fas fa-palette text-white text-xl"></i>
                </div>
                <div>
                  <h2 class="text-lg font-semibold text-white">{{ $t('settings.appearanceSettings') }}</h2>
                  <p class="text-sm text-teal-100">{{ $t('settings.customizeAppearance') }}</p>
                </div>
              </div>
            </div>
            <div class="p-6 space-y-6">
              <div>
                <label class="block text-sm font-medium text-teal-700 mb-3">{{ $t('settings.theme') }}</label>
                <div class="grid grid-cols-3 gap-4">
                  <label
                    v-for="theme in themes"
                    :key="theme.id"
                    :class="[
                      'p-4 rounded-xl border-2 cursor-pointer transition-all text-center ripple',
                      appearance.theme === theme.id ? 'border-teal-500 bg-teal-50' : 'border-teal-100 hover:border-teal-200'
                    ]"
                  >
                    <input type="radio" v-model="appearance.theme" :value="theme.id" class="hidden">
                    <i :class="[theme.icon, 'text-2xl mb-2', appearance.theme === theme.id ? 'text-teal-600 icon-vibrant' : 'text-teal-400 icon-soft']"></i>
                    <p class="text-sm font-medium" :class="appearance.theme === theme.id ? 'text-teal-700' : 'text-teal-500'">{{ theme.label }}</p>
                  </label>
                </div>
              </div>

              <div>
                <label class="block text-sm font-medium text-teal-700 mb-3">{{ $t('settings.accentColor') }}</label>
                <div class="flex gap-3">
                  <button
                    v-for="color in accentColors"
                    :key="color.id"
                    @click="appearance.accent = color.id"
                    :class="[
                      'w-10 h-10 rounded-xl transition-transform hover:scale-110 ripple',
                      appearance.accent === color.id ? 'ring-2 ring-offset-2 ring-teal-500' : ''
                    ]"
                    :style="{ backgroundColor: color.value }"
                  ></button>
                </div>
              </div>

              <div>
                <label class="block text-sm font-medium text-teal-700 mb-3">{{ $t('settings.fontSize') }}</label>
                <div class="flex items-center gap-4">
                  <span class="text-sm text-teal-500">A</span>
                  <input type="range" v-model="appearance.fontSize" min="12" max="20" class="flex-1 h-2 bg-teal-100 rounded-lg appearance-none cursor-pointer">
                  <span class="text-lg text-teal-500">A</span>
                  <span class="text-sm text-teal-600 font-medium w-12">{{ appearance.fontSize }}px</span>
                </div>
              </div>

              <div class="flex items-center justify-between py-3">
                <div>
                  <p class="font-medium text-teal-800">{{ $t('settings.compactMode') }}</p>
                  <p class="text-sm text-teal-500">{{ $t('settings.compactModeDesc') }}</p>
                </div>
                <label class="toggle">
                  <input type="checkbox" v-model="appearance.compact">
                  <span class="toggle-slider"></span>
                </label>
              </div>

              <div class="flex items-center justify-between py-3">
                <div>
                  <p class="font-medium text-teal-800">{{ $t('settings.animations') }}</p>
                  <p class="text-sm text-teal-500">{{ $t('settings.animationsDesc') }}</p>
                </div>
                <label class="toggle">
                  <input type="checkbox" v-model="appearance.animations">
                  <span class="toggle-slider"></span>
                </label>
              </div>

              <div class="pt-4 border-t border-teal-100 flex justify-end gap-3">
                <button class="btn btn-secondary ripple">{{ $t('settings.resetToDefault') }}</button>
                <button class="btn-vibrant ripple">{{ $t('settings.saveChanges') }}</button>
              </div>
            </div>
          </div>

          <!-- Language -->
          <div v-if="activeTab === 'language'" class="card-animated rounded-2xl overflow-hidden fade-in-up" style="animation-delay: 0.2s;">
            <div class="settings-tab-header p-5 bg-gradient-to-r from-teal-500 to-emerald-500">
              <div class="flex items-center gap-4">
                <div class="w-12 h-12 bg-white/20 backdrop-blur rounded-xl flex items-center justify-center">
                  <i class="fas fa-globe text-white text-xl"></i>
                </div>
                <div>
                  <h2 class="text-lg font-semibold text-white">{{ $t('settings.languageRegion') }}</h2>
                  <p class="text-sm text-teal-100">{{ $t('settings.languagePreferences') }}</p>
                </div>
              </div>
            </div>
            <div class="p-6 space-y-6">
              <div>
                <label class="block text-sm font-medium text-teal-700 mb-2">{{ $t('settings.displayLanguage') }}</label>
                <select v-model="language.display" class="input">
                  <option value="en">English (US)</option>
                  <option value="ar">Arabic</option>
                  <option value="fr">French</option>
                  <option value="es">Spanish</option>
                  <option value="de">German (Deutsch)</option>
                </select>
              </div>

              <div>
                <label class="block text-sm font-medium text-teal-700 mb-2">{{ $t('settings.dateFormat') }}</label>
                <select v-model="language.dateFormat" class="input">
                  <option value="mdy">MM/DD/YYYY</option>
                  <option value="dmy">DD/MM/YYYY</option>
                  <option value="ymd">YYYY-MM-DD</option>
                </select>
              </div>

              <div>
                <label class="block text-sm font-medium text-teal-700 mb-2">{{ $t('settings.timeFormat') }}</label>
                <div class="flex gap-4">
                  <label
                    :class="[
                      'flex-1 p-3 rounded-xl border-2 cursor-pointer text-center ripple',
                      language.timeFormat === '12' ? 'border-teal-500 bg-teal-50' : 'border-teal-100'
                    ]"
                  >
                    <input type="radio" v-model="language.timeFormat" value="12" class="hidden">
                    <span class="font-medium" :class="language.timeFormat === '12' ? 'text-teal-700' : 'text-teal-500'">{{ $t('settings.timeFormat12') }}</span>
                  </label>
                  <label
                    :class="[
                      'flex-1 p-3 rounded-xl border-2 cursor-pointer text-center ripple',
                      language.timeFormat === '24' ? 'border-teal-500 bg-teal-50' : 'border-teal-100'
                    ]"
                  >
                    <input type="radio" v-model="language.timeFormat" value="24" class="hidden">
                    <span class="font-medium" :class="language.timeFormat === '24' ? 'text-teal-700' : 'text-teal-500'">{{ $t('settings.timeFormat24') }}</span>
                  </label>
                </div>
              </div>

              <div>
                <label class="block text-sm font-medium text-teal-700 mb-2">{{ $t('settings.firstDayOfWeek') }}</label>
                <select v-model="language.firstDay" class="input">
                  <option value="sunday">{{ $t('settings.weekDays.sunday') }}</option>
                  <option value="monday">{{ $t('settings.weekDays.monday') }}</option>
                  <option value="saturday">{{ $t('settings.weekDays.saturday') }}</option>
                </select>
              </div>

              <div class="pt-4 border-t border-teal-100 flex justify-end gap-3">
                <button class="btn btn-secondary ripple">{{ $t('settings.cancel') }}</button>
                <button class="btn-vibrant ripple">{{ $t('settings.saveChanges') }}</button>
              </div>
            </div>
          </div>

          <!-- Security -->
          <div v-if="activeTab === 'security'" class="card-animated rounded-2xl overflow-hidden fade-in-up" style="animation-delay: 0.2s;">
            <div class="settings-tab-header p-5 bg-gradient-to-r from-teal-500 to-emerald-500">
              <div class="flex items-center gap-4">
                <div class="w-12 h-12 bg-white/20 backdrop-blur rounded-xl flex items-center justify-center">
                  <i class="fas fa-lock text-white text-xl"></i>
                </div>
                <div>
                  <h2 class="text-lg font-semibold text-white">{{ $t('settings.securitySettings') }}</h2>
                  <p class="text-sm text-teal-100">{{ $t('settings.manageSecuritySettings') }}</p>
                </div>
              </div>
            </div>
            <div class="p-6 space-y-6">
              <!-- Password -->
              <div class="p-4 rounded-xl bg-teal-50">
                <div class="flex items-center justify-between mb-4">
                  <div>
                    <h4 class="font-medium text-teal-900">{{ $t('settings.password') }}</h4>
                    <p class="text-sm text-teal-500">{{ $t('settings.lastChanged', { time: '3 months ago' }) }}</p>
                  </div>
                  <button @click="showChangePassword = true" class="btn btn-secondary btn-sm ripple">{{ $t('settings.changePassword') }}</button>
                </div>
              </div>

              <!-- 2FA -->
              <div class="p-4 rounded-xl bg-teal-50">
                <div class="flex items-center justify-between">
                  <div>
                    <h4 class="font-medium text-teal-900">{{ $t('settings.twoFactorAuthentication') }}</h4>
                    <p class="text-sm text-teal-500">{{ $t('settings.addExtraSecurityLayer') }}</p>
                  </div>
                  <label class="toggle">
                    <input type="checkbox" v-model="security.twoFactor">
                    <span class="toggle-slider"></span>
                  </label>
                </div>
              </div>

              <!-- Sessions -->
              <div>
                <h4 class="font-medium text-teal-900 mb-4">{{ $t('settings.activeSessions') }}</h4>
                <div class="space-y-3">
                  <div
                    v-for="session in sessions"
                    :key="session.id"
                    class="flex items-center justify-between p-4 rounded-xl bg-teal-50 list-item-animated"
                  >
                    <div class="flex items-center gap-4">
                      <div class="w-10 h-10 rounded-lg bg-teal-100 flex items-center justify-center">
                        <i :class="[session.icon, 'text-teal-600 icon-soft']"></i>
                      </div>
                      <div>
                        <p class="font-medium text-teal-900">{{ session.device }}</p>
                        <p class="text-sm text-teal-500">{{ session.location }} - {{ session.lastActive }}</p>
                      </div>
                    </div>
                    <button v-if="!session.current" class="text-red-500 hover:text-red-700 text-sm font-medium">
                      {{ $t('settings.revoke') }}
                    </button>
                    <span v-else class="badge badge-green">{{ $t('settings.current') }}</span>
                  </div>
                </div>
                <button class="w-full mt-3 py-2 text-center text-red-500 hover:text-red-700 font-medium">
                  {{ $t('settings.signOutAllSessions') }}
                </button>
              </div>
            </div>
          </div>

          <!-- Connected Apps -->
          <div v-if="activeTab === 'apps'" class="card-animated rounded-2xl overflow-hidden fade-in-up" style="animation-delay: 0.2s;">
            <div class="settings-tab-header p-5 bg-gradient-to-r from-teal-500 to-emerald-500">
              <div class="flex items-center gap-4">
                <div class="w-12 h-12 bg-white/20 backdrop-blur rounded-xl flex items-center justify-center">
                  <i class="fas fa-plug text-white text-xl"></i>
                </div>
                <div>
                  <h2 class="text-lg font-semibold text-white">{{ $t('settings.connectedAppsTitle') }}</h2>
                  <p class="text-sm text-teal-100">{{ $t('settings.manageIntegrations') }}</p>
                </div>
              </div>
            </div>
            <div class="p-6 space-y-4">
              <div
                v-for="app in connectedApps"
                :key="app.id"
                class="flex items-center justify-between p-4 rounded-xl bg-teal-50 list-item-animated"
              >
                <div class="flex items-center gap-4">
                  <div :class="['w-12 h-12 rounded-xl flex items-center justify-center', app.bg]">
                    <i :class="[app.icon, app.color, 'text-xl icon-soft']"></i>
                  </div>
                  <div>
                    <p class="font-medium text-teal-900">{{ app.name }}</p>
                    <p class="text-sm text-teal-500">{{ $t('settings.connected') }} {{ app.connectedDate }}</p>
                  </div>
                </div>
                <button class="btn btn-secondary btn-sm text-red-500 hover:text-red-700 ripple">{{ $t('settings.disconnect') }}</button>
              </div>

              <button class="w-full p-4 rounded-xl border-2 border-dashed border-teal-200 text-teal-500 hover:border-teal-400 hover:text-teal-700 transition-colors ripple">
                <i class="fas fa-plus mr-2 icon-soft"></i>{{ $t('settings.connectNewApp') }}
              </button>
            </div>
          </div>

          <!-- Keyboard Shortcuts -->
          <div v-if="activeTab === 'shortcuts'" class="space-y-6">
            <!-- Shortcuts Header -->
            <div class="card-animated rounded-2xl overflow-hidden fade-in-up" style="animation-delay: 0.2s;">
              <div class="settings-tab-header p-5 bg-gradient-to-r from-teal-500 to-emerald-500">
                <div class="flex items-center justify-between">
                  <div class="flex items-center gap-4">
                    <div class="w-12 h-12 bg-white/20 backdrop-blur rounded-xl flex items-center justify-center">
                      <i class="fas fa-keyboard text-white text-xl"></i>
                    </div>
                    <div>
                      <h2 class="text-lg font-semibold text-white">{{ $t('settings.keyboardShortcuts') }}</h2>
                      <p class="text-sm text-teal-100">{{ $t('settings.speedUpWorkflow') }}</p>
                    </div>
                  </div>
                  <div class="flex items-center gap-3">
                    <span class="text-sm text-teal-100">{{ $t('settings.enableShortcuts') }}</span>
                    <label class="toggle">
                      <input type="checkbox" v-model="shortcutsEnabled">
                      <span class="toggle-slider"></span>
                    </label>
                  </div>
                </div>
              </div>
            </div>

            <!-- Shortcuts Categories -->
            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div
                v-for="(category, index) in keyboardShortcuts"
                :key="category.category"
                class="card-animated rounded-2xl overflow-hidden fade-in-up"
                :style="{ animationDelay: `${0.3 + index * 0.1}s` }"
              >
                <div class="p-4 border-b border-teal-100 bg-teal-50">
                  <h3 class="font-semibold text-teal-900">{{ category.category }}</h3>
                </div>
                <div class="p-4 space-y-3">
                  <div
                    v-for="shortcut in category.shortcuts"
                    :key="shortcut.description"
                    class="flex items-center justify-between py-2"
                  >
                    <span class="text-sm text-teal-700">{{ shortcut.description }}</span>
                    <div class="flex items-center gap-1">
                      <template v-for="(key, keyIndex) in shortcut.keys" :key="keyIndex">
                        <kbd v-if="key" class="px-2 py-1 bg-gray-100 border border-gray-300 rounded-md text-xs font-mono text-gray-700 shadow-sm">
                          {{ key }}
                        </kbd>
                        <span v-if="key && keyIndex < shortcut.keys.length - 1 && shortcut.keys[keyIndex + 1]" class="text-gray-400 text-xs">+</span>
                      </template>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- Shortcut Tips -->
            <div class="card-animated rounded-2xl p-6 fade-in-up" style="animation-delay: 0.7s;">
              <div class="flex items-start gap-4">
                <div class="w-10 h-10 rounded-xl bg-blue-100 flex items-center justify-center flex-shrink-0">
                  <i class="fas fa-lightbulb text-blue-600"></i>
                </div>
                <div>
                  <h4 class="font-medium text-teal-900 mb-2">{{ $t('settings.proTips') }}</h4>
                  <ul class="space-y-2 text-sm text-teal-600">
                    <li class="flex items-start gap-2">
                      <i class="fas fa-check text-teal-500 mt-0.5"></i>
                      <span>{{ $t('settings.proTipsList.tip1') }}</span>
                    </li>
                    <li class="flex items-start gap-2">
                      <i class="fas fa-check text-teal-500 mt-0.5"></i>
                      <span>{{ $t('settings.proTipsList.tip2') }}</span>
                    </li>
                    <li class="flex items-start gap-2">
                      <i class="fas fa-check text-teal-500 mt-0.5"></i>
                      <span>{{ $t('settings.proTipsList.tip3') }}</span>
                    </li>
                  </ul>
                </div>
              </div>
            </div>

            <!-- Print Shortcuts Button -->
            <div class="flex justify-end">
              <button class="px-4 py-2 bg-teal-100 text-teal-700 rounded-xl font-medium text-sm hover:bg-teal-200 transition-all flex items-center gap-2">
                <i class="fas fa-print"></i>
                {{ $t('settings.printShortcuts') }}
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Change Password Modal -->
    <div v-if="showChangePassword" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
      <div class="card-animated rounded-2xl w-full max-w-md fade-in-up">
        <div class="p-5 border-b border-teal-100 flex items-center justify-between">
          <h2 class="text-xl font-semibold text-teal-900">{{ $t('settings.changePassword') }}</h2>
          <button @click="showChangePassword = false" class="p-2 rounded-lg hover:bg-teal-100 text-teal-500 ripple">
            <i class="fas fa-times icon-soft"></i>
          </button>
        </div>
        <form @submit.prevent="changePassword" class="p-6 space-y-4">
          <div>
            <label class="block text-sm font-medium text-teal-700 mb-2">{{ $t('settings.currentPassword') }}</label>
            <input type="password" class="input" required>
          </div>
          <div>
            <label class="block text-sm font-medium text-teal-700 mb-2">{{ $t('settings.newPassword') }}</label>
            <input type="password" class="input" required>
          </div>
          <div>
            <label class="block text-sm font-medium text-teal-700 mb-2">{{ $t('settings.confirmNewPassword') }}</label>
            <input type="password" class="input" required>
          </div>
          <div class="flex gap-3 pt-4">
            <button type="button" @click="showChangePassword = false" class="btn btn-secondary flex-1 ripple">{{ $t('settings.cancel') }}</button>
            <button type="submit" class="btn-vibrant flex-1 ripple">{{ $t('settings.updatePassword') }}</button>
          </div>
        </form>
      </div>
    </div>

    <!-- AI Connection Test Modal -->
    <Teleport to="body">
      <div v-if="showAITestModal" class="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
        <div class="bg-white rounded-2xl shadow-2xl w-full max-w-md overflow-hidden">
          <div class="p-6 border-b border-gray-100">
            <div class="flex items-center justify-between">
              <div class="flex items-center gap-3">
                <div class="w-10 h-10 bg-gradient-to-br from-teal-500 to-emerald-500 rounded-xl flex items-center justify-center">
                  <i class="fas fa-plug text-white"></i>
                </div>
                <div>
                  <h3 class="text-lg font-semibold text-gray-900">{{ $t('settings.aiConnectionTest') }}</h3>
                  <p class="text-sm text-gray-500">{{ $t('settings.testingConnection') }}</p>
                </div>
              </div>
              <button @click="showAITestModal = false" class="p-2 hover:bg-gray-100 rounded-lg transition-colors">
                <i class="fas fa-times text-gray-400"></i>
              </button>
            </div>
          </div>

          <div class="p-6">
            <div v-if="isTestingAI" class="text-center py-8">
              <div class="w-16 h-16 mx-auto mb-4 relative">
                <div class="absolute inset-0 border-4 border-teal-200 rounded-full"></div>
                <div class="absolute inset-0 border-4 border-teal-500 rounded-full border-t-transparent animate-spin"></div>
              </div>
              <p class="text-gray-600">{{ $t('settings.testingAIServices') }}</p>
            </div>

            <div v-else-if="aiTestResults" class="space-y-4">
              <!-- Status -->
              <div :class="[
                'p-4 rounded-xl flex items-center gap-4',
                aiTestResults.status === 'success' ? 'bg-green-50 border border-green-200' : 'bg-red-50 border border-red-200'
              ]">
                <div :class="[
                  'w-12 h-12 rounded-full flex items-center justify-center',
                  aiTestResults.status === 'success' ? 'bg-green-100' : 'bg-red-100'
                ]">
                  <i :class="[
                    'text-xl',
                    aiTestResults.status === 'success' ? 'fas fa-check text-green-600' : 'fas fa-times text-red-600'
                  ]"></i>
                </div>
                <div>
                  <p :class="[
                    'font-semibold',
                    aiTestResults.status === 'success' ? 'text-green-800' : 'text-red-800'
                  ]">
                    {{ aiTestResults.status === 'success' ? $t('settings.connectionSuccessful') : $t('settings.connectionFailed') }}
                  </p>
                  <p :class="aiTestResults.status === 'success' ? 'text-green-600' : 'text-red-600'" class="text-sm">
                    {{ aiTestResults.message }}
                  </p>
                </div>
              </div>

              <!-- Details -->
              <div v-if="aiTestResults.status === 'success'" class="space-y-3">
                <div class="flex items-center justify-between p-3 bg-gray-50 rounded-lg">
                  <span class="text-sm text-gray-600">{{ $t('settings.service') }}</span>
                  <span class="text-sm font-medium text-gray-900">{{ aiTestResults.service }}</span>
                </div>
                <div class="flex items-center justify-between p-3 bg-gray-50 rounded-lg">
                  <span class="text-sm text-gray-600">{{ $t('settings.responseTime') }}</span>
                  <span class="text-sm font-medium text-green-600">{{ aiTestResults.responseTime }}ms</span>
                </div>
                <div class="flex items-center justify-between p-3 bg-gray-50 rounded-lg">
                  <span class="text-sm text-gray-600">{{ $t('common.status') }}</span>
                  <span class="px-2 py-1 text-xs font-medium bg-green-100 text-green-700 rounded-full">{{ $t('settings.operational') }}</span>
                </div>
              </div>
            </div>
          </div>

          <div class="p-4 border-t border-gray-100 flex justify-end gap-3">
            <button @click="testAIConnection"
                    class="px-4 py-2 text-teal-600 hover:bg-teal-50 rounded-lg transition-colors flex items-center gap-2">
              <i class="fas fa-rotate"></i> {{ $t('settings.testAgain') }}
            </button>
            <button @click="showAITestModal = false"
                    class="px-4 py-2 bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 transition-colors">
              {{ $t('common.close') }}
            </button>
          </div>
        </div>
      </div>
    </Teleport>
  </div>
</template>

<style scoped>
/* Settings navigation responsive visibility */
.settings-mobile-tabs {
  display: block;
}

.settings-sidebar {
  display: none;
}

@media (min-width: 1024px) {
  .settings-mobile-tabs {
    display: none;
  }

  .settings-sidebar {
    display: block;
  }
}

/* Sidebar navigation item styles */
.sidebar-nav-item {
  position: relative;
  overflow: hidden;
}

.sidebar-nav-item::before {
  content: '';
  position: absolute;
  inset-inline-start: 0;
  top: 50%;
  transform: translateY(-50%) scaleY(0);
  width: 3px;
  height: 60%;
  background: linear-gradient(180deg, #14b8a6, #10b981);
  border-radius: 0 4px 4px 0;
  transition: transform 0.3s ease;
}

.sidebar-nav-item:not(.bg-gradient-to-r):hover::before {
  transform: translateY(-50%) scaleY(1);
}

.sidebar-nav-item:not(.bg-gradient-to-r):hover {
  padding-inline-start: 1.25rem;
  background: linear-gradient(90deg, rgba(20, 184, 166, 0.08), transparent);
}

/* Sidebar header glow effect */
.sidebar-header,
.settings-tab-header {
  position: relative;
  overflow: hidden;
}

.sidebar-header::before,
.settings-tab-header::before {
  content: '';
  position: absolute;
  top: -50%;
  right: -50%;
  width: 100%;
  height: 100%;
  background: radial-gradient(circle, rgba(255,255,255,0.15) 0%, transparent 60%);
  animation: headerGlow 8s ease-in-out infinite;
}

@keyframes headerGlow {
  0%, 100% { transform: translate(0, 0); }
  50% { transform: translate(-20%, 20%); }
}

/* Settings tab header unified styling */
.settings-tab-header {
  border-bottom: none;
}

/* Card animations */
.card-animated {
  background: #ffffff;
  border: 1px solid rgba(20, 184, 166, 0.1);
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.04), 0 1px 2px rgba(0, 0, 0, 0.02);
  transition: all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275);
  animation: cardEntrance 0.6s ease-out backwards;
}

.card-animated:hover {
  transform: translateY(-6px) scale(1.01);
  box-shadow: 0 20px 40px rgba(20, 184, 166, 0.15), 0 0 0 1px rgba(20, 184, 166, 0.1);
  border-color: rgba(20, 184, 166, 0.3);
}

@keyframes cardEntrance {
  from {
    opacity: 0;
    transform: translateY(30px) scale(0.95);
  }
  to {
    opacity: 1;
    transform: translateY(0) scale(1);
  }
}

/* Vibrant button */
.btn-vibrant {
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 50%, #0f766e 100%);
  background-size: 200% 200%;
  animation: buttonGradient 3s ease infinite;
  box-shadow: 0 4px 15px rgba(20, 184, 166, 0.4);
  transition: all 0.3s ease;
  color: white;
  font-weight: 500;
  padding: 0.625rem 1.25rem;
  border-radius: 0.75rem;
  border: none;
  cursor: pointer;
}

.btn-vibrant:hover {
  transform: translateY(-2px) scale(1.02);
  box-shadow: 0 8px 25px rgba(20, 184, 166, 0.5);
}

.btn-vibrant:active {
  transform: translateY(0) scale(0.98);
}

@keyframes buttonGradient {
  0%, 100% { background-position: 0% 50%; }
  50% { background-position: 100% 50%; }
}

/* List item hover effects */
.list-item-animated {
  transition: all 0.3s cubic-bezier(0.175, 0.885, 0.32, 1.275);
  position: relative;
}

.list-item-animated::before {
  content: '';
  position: absolute;
  left: 0;
  top: 0;
  bottom: 0;
  width: 3px;
  background: linear-gradient(180deg, #14b8a6, #0d9488);
  border-radius: 0 3px 3px 0;
  transform: scaleY(0);
  transition: transform 0.3s ease;
}

.list-item-animated:hover::before {
  transform: scaleY(1);
}

.list-item-animated:hover {
  transform: translateX(8px);
  background: linear-gradient(90deg, rgba(20, 184, 166, 0.08), transparent);
}

/* Icon containers with gradient */
.icon-vibrant {
  background: linear-gradient(135deg, #14b8a6 0%, #0d9488 100%);
  box-shadow: 0 4px 15px rgba(20, 184, 166, 0.3);
  transition: all 0.3s ease;
}

.icon-vibrant:hover {
  transform: scale(1.1) rotate(5deg);
  box-shadow: 0 8px 25px rgba(20, 184, 166, 0.4);
}

.icon-soft {
  background: linear-gradient(135deg, #f0fdfa 0%, #ccfbf1 100%);
  color: #0d9488;
  transition: all 0.3s ease;
}

.icon-soft:hover {
  background: linear-gradient(135deg, #ccfbf1 0%, #99f6e4 100%);
  transform: scale(1.1);
}

/* Ripple effect on click */
.ripple {
  position: relative;
  overflow: hidden;
}

.ripple::after {
  content: '';
  position: absolute;
  width: 100%;
  height: 100%;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%) scale(0);
  background: radial-gradient(circle, rgba(20, 184, 166, 0.3) 0%, transparent 70%);
  transition: transform 0.5s ease;
}

.ripple:active::after {
  transform: translate(-50%, -50%) scale(2);
}

/* Fade in up animation for content */
.fade-in-up {
  animation: fadeInUp 0.6s ease-out backwards;
}

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* Toggle switch styles */
.toggle {
  position: relative;
  display: inline-block;
  width: 44px;
  height: 24px;
}

.toggle input {
  opacity: 0;
  width: 0;
  height: 0;
}

.toggle-slider {
  position: absolute;
  cursor: pointer;
  inset: 0;
  background-color: #ccc;
  border-radius: 24px;
  transition: .3s;
}

.toggle-slider:before {
  position: absolute;
  content: "";
  height: 18px;
  width: 18px;
  left: 3px;
  bottom: 3px;
  background-color: white;
  border-radius: 50%;
  transition: .3s;
}

.toggle input:checked + .toggle-slider {
  background-color: #14b8a6;
}

.toggle input:checked + .toggle-slider:before {
  transform: translateX(20px);
}

/* Badge styles */
.badge-green {
  background-color: #dcfce7;
  color: #166534;
  padding: 0.25rem 0.75rem;
  border-radius: 9999px;
  font-size: 0.75rem;
  font-weight: 500;
}

/* Hide scrollbar for mobile tabs */
.hide-scrollbar {
  -ms-overflow-style: none;
  scrollbar-width: none;
}

.hide-scrollbar::-webkit-scrollbar {
  display: none;
}
</style>
