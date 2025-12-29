<script setup lang="ts">
import { ref } from 'vue'
import LoadingSpinner from '@/components/common/LoadingSpinner.vue'

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
const tabs = ref([
  { id: 'account', label: 'Account', icon: 'fas fa-user' },
  { id: 'notifications', label: 'Notifications', icon: 'fas fa-bell' },
  { id: 'privacy', label: 'Privacy', icon: 'fas fa-shield-alt' },
  { id: 'appearance', label: 'Appearance', icon: 'fas fa-palette' },
  { id: 'language', label: 'Language', icon: 'fas fa-globe' },
  { id: 'security', label: 'Security', icon: 'fas fa-lock' },
  { id: 'apps', label: 'Connected Apps', icon: 'fas fa-plug' }
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

const notifications = ref<NotificationCategory[]>([
  {
    id: 'activity',
    title: 'Activity',
    items: [
      { id: 'comments', label: 'Comments on my content', description: 'When someone comments on your articles or posts', email: true, push: true },
      { id: 'mentions', label: 'Mentions', description: 'When someone mentions you', email: true, push: true },
      { id: 'reactions', label: 'Reactions', description: 'When someone reacts to your content', email: false, push: true }
    ]
  },
  {
    id: 'updates',
    title: 'Updates',
    items: [
      { id: 'news', label: 'Company News', description: 'Important announcements and updates', email: true, push: true },
      { id: 'events', label: 'Events', description: 'Upcoming events and reminders', email: true, push: false },
      { id: 'courses', label: 'Learning Updates', description: 'New courses and learning opportunities', email: true, push: false }
    ]
  }
])

// Privacy settings
const privacySettings = ref([
  { id: 'profile', label: 'Public Profile', description: 'Allow others to view your profile', enabled: true },
  { id: 'activity', label: 'Show Activity Status', description: "Display when you're online", enabled: true },
  { id: 'contributions', label: 'Show Contributions', description: 'Display your articles and comments publicly', enabled: true },
  { id: 'search', label: 'Appear in Search', description: 'Allow your profile to appear in search results', enabled: true },
  { id: 'analytics', label: 'Usage Analytics', description: 'Help improve the platform with anonymous usage data', enabled: false }
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
const themes = ref([
  { id: 'light', label: 'Light', icon: 'fas fa-sun' },
  { id: 'dark', label: 'Dark', icon: 'fas fa-moon' },
  { id: 'system', label: 'System', icon: 'fas fa-laptop' }
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

// Methods
function changePassword() {
  showChangePassword.value = false
  alert('Password updated successfully!')
}
</script>

<template>
  <div class="min-h-screen">
    <!-- Loading State -->
    <div v-if="isLoading" class="flex items-center justify-center min-h-[60vh]">
      <LoadingSpinner size="lg" text="Loading settings..." />
    </div>

    <div v-else>
      <!-- Page Header -->
      <div class="flex flex-col lg:flex-row lg:items-center lg:justify-between gap-4 mb-10 fade-in-up">
        <div>
          <h1 class="text-3xl font-bold text-teal-900 mb-2">Settings</h1>
          <p class="text-teal-600">Manage your account preferences and settings</p>
        </div>
      </div>

      <div class="flex gap-6">
        <!-- Settings Navigation -->
        <aside class="w-64 flex-shrink-0 hidden lg:block fade-in-up" style="animation-delay: 0.1s;">
          <div class="card-animated rounded-2xl p-4 sticky top-24">
            <nav class="space-y-1">
              <button
                v-for="(tab, index) in tabs"
                :key="tab.id"
                @click="activeTab = tab.id"
                :class="[
                  'w-full flex items-center gap-3 px-4 py-3 rounded-xl text-left transition-all ripple list-item-animated',
                  activeTab === tab.id ? 'bg-teal-500 text-white' : 'text-teal-700 hover:bg-teal-50'
                ]"
                :style="{ animationDelay: `${0.2 + index * 0.05}s` }"
              >
                <i :class="[tab.icon, activeTab === tab.id ? 'icon-vibrant' : 'icon-soft']"></i>
                <span class="font-medium">{{ tab.label }}</span>
              </button>
            </nav>
          </div>
        </aside>

        <!-- Settings Content -->
        <div class="flex-1">
          <!-- Account Settings -->
          <div v-if="activeTab === 'account'" class="card-animated rounded-2xl overflow-hidden fade-in-up" style="animation-delay: 0.2s;">
            <div class="p-6 border-b border-teal-100">
              <h2 class="text-lg font-semibold text-teal-900">Account Settings</h2>
              <p class="text-sm text-teal-500">Manage your account information</p>
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
                  <p class="font-medium text-teal-900">Profile Photo</p>
                  <p class="text-sm text-teal-500 mb-2">JPG, PNG or GIF. Max 2MB</p>
                  <button class="btn btn-secondary btn-sm ripple">Upload Photo</button>
                </div>
              </div>

              <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <label class="block text-sm font-medium text-teal-700 mb-2">First Name</label>
                  <input type="text" v-model="account.firstName" class="input">
                </div>
                <div>
                  <label class="block text-sm font-medium text-teal-700 mb-2">Last Name</label>
                  <input type="text" v-model="account.lastName" class="input">
                </div>
              </div>

              <div>
                <label class="block text-sm font-medium text-teal-700 mb-2">Email Address</label>
                <input type="email" v-model="account.email" class="input">
              </div>

              <div>
                <label class="block text-sm font-medium text-teal-700 mb-2">Phone Number</label>
                <input type="tel" v-model="account.phone" class="input">
              </div>

              <div>
                <label class="block text-sm font-medium text-teal-700 mb-2">Time Zone</label>
                <select v-model="account.timezone" class="input">
                  <option value="UTC+4">Gulf Standard Time (UTC+4)</option>
                  <option value="UTC+0">UTC</option>
                  <option value="UTC-5">Eastern Time (UTC-5)</option>
                  <option value="UTC-8">Pacific Time (UTC-8)</option>
                </select>
              </div>

              <div class="pt-4 border-t border-teal-100 flex justify-end gap-3">
                <button class="btn btn-secondary ripple">Cancel</button>
                <button class="btn-vibrant ripple">Save Changes</button>
              </div>
            </div>
          </div>

          <!-- Notifications -->
          <div v-if="activeTab === 'notifications'" class="card-animated rounded-2xl overflow-hidden fade-in-up" style="animation-delay: 0.2s;">
            <div class="p-6 border-b border-teal-100">
              <h2 class="text-lg font-semibold text-teal-900">Notification Preferences</h2>
              <p class="text-sm text-teal-500">Choose how you want to be notified</p>
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
                        <input type="checkbox" v-model="item.email">
                        <span class="toggle-slider"></span>
                      </label>
                      <span class="text-xs text-teal-500 w-12">Email</span>
                      <label class="toggle">
                        <input type="checkbox" v-model="item.push">
                        <span class="toggle-slider"></span>
                      </label>
                      <span class="text-xs text-teal-500 w-12">Push</span>
                    </div>
                  </div>
                </div>
              </div>

              <div class="pt-4 flex justify-end gap-3">
                <button class="btn btn-secondary ripple">Reset to Default</button>
                <button class="btn-vibrant ripple">Save Preferences</button>
              </div>
            </div>
          </div>

          <!-- Privacy -->
          <div v-if="activeTab === 'privacy'" class="card-animated rounded-2xl overflow-hidden fade-in-up" style="animation-delay: 0.2s;">
            <div class="p-6 border-b border-teal-100">
              <h2 class="text-lg font-semibold text-teal-900">Privacy Settings</h2>
              <p class="text-sm text-teal-500">Control your privacy and visibility</p>
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
                  <input type="checkbox" v-model="setting.enabled">
                  <span class="toggle-slider"></span>
                </label>
              </div>

              <div class="p-4 bg-red-50 rounded-xl border border-red-200">
                <h4 class="font-medium text-red-800 mb-2"><i class="fas fa-exclamation-triangle mr-2 icon-soft"></i>Danger Zone</h4>
                <p class="text-sm text-red-600 mb-3">Permanently delete your account and all data</p>
                <button class="btn bg-red-500 hover:bg-red-600 text-white btn-sm ripple">Delete Account</button>
              </div>
            </div>
          </div>

          <!-- Appearance -->
          <div v-if="activeTab === 'appearance'" class="card-animated rounded-2xl overflow-hidden fade-in-up" style="animation-delay: 0.2s;">
            <div class="p-6 border-b border-teal-100">
              <h2 class="text-lg font-semibold text-teal-900">Appearance</h2>
              <p class="text-sm text-teal-500">Customize how the app looks</p>
            </div>
            <div class="p-6 space-y-6">
              <div>
                <label class="block text-sm font-medium text-teal-700 mb-3">Theme</label>
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
                <label class="block text-sm font-medium text-teal-700 mb-3">Accent Color</label>
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
                <label class="block text-sm font-medium text-teal-700 mb-3">Font Size</label>
                <div class="flex items-center gap-4">
                  <span class="text-sm text-teal-500">A</span>
                  <input type="range" v-model="appearance.fontSize" min="12" max="20" class="flex-1 h-2 bg-teal-100 rounded-lg appearance-none cursor-pointer">
                  <span class="text-lg text-teal-500">A</span>
                  <span class="text-sm text-teal-600 font-medium w-12">{{ appearance.fontSize }}px</span>
                </div>
              </div>

              <div class="flex items-center justify-between py-3">
                <div>
                  <p class="font-medium text-teal-800">Compact Mode</p>
                  <p class="text-sm text-teal-500">Reduce spacing and padding</p>
                </div>
                <label class="toggle">
                  <input type="checkbox" v-model="appearance.compact">
                  <span class="toggle-slider"></span>
                </label>
              </div>

              <div class="flex items-center justify-between py-3">
                <div>
                  <p class="font-medium text-teal-800">Animations</p>
                  <p class="text-sm text-teal-500">Enable interface animations</p>
                </div>
                <label class="toggle">
                  <input type="checkbox" v-model="appearance.animations">
                  <span class="toggle-slider"></span>
                </label>
              </div>

              <div class="pt-4 border-t border-teal-100 flex justify-end gap-3">
                <button class="btn btn-secondary ripple">Reset to Default</button>
                <button class="btn-vibrant ripple">Save Changes</button>
              </div>
            </div>
          </div>

          <!-- Language -->
          <div v-if="activeTab === 'language'" class="card-animated rounded-2xl overflow-hidden fade-in-up" style="animation-delay: 0.2s;">
            <div class="p-6 border-b border-teal-100">
              <h2 class="text-lg font-semibold text-teal-900">Language & Region</h2>
              <p class="text-sm text-teal-500">Set your language and regional preferences</p>
            </div>
            <div class="p-6 space-y-6">
              <div>
                <label class="block text-sm font-medium text-teal-700 mb-2">Display Language</label>
                <select v-model="language.display" class="input">
                  <option value="en">English (US)</option>
                  <option value="ar">Arabic</option>
                  <option value="fr">French</option>
                  <option value="es">Spanish</option>
                  <option value="de">German (Deutsch)</option>
                </select>
              </div>

              <div>
                <label class="block text-sm font-medium text-teal-700 mb-2">Date Format</label>
                <select v-model="language.dateFormat" class="input">
                  <option value="mdy">MM/DD/YYYY</option>
                  <option value="dmy">DD/MM/YYYY</option>
                  <option value="ymd">YYYY-MM-DD</option>
                </select>
              </div>

              <div>
                <label class="block text-sm font-medium text-teal-700 mb-2">Time Format</label>
                <div class="flex gap-4">
                  <label
                    :class="[
                      'flex-1 p-3 rounded-xl border-2 cursor-pointer text-center ripple',
                      language.timeFormat === '12' ? 'border-teal-500 bg-teal-50' : 'border-teal-100'
                    ]"
                  >
                    <input type="radio" v-model="language.timeFormat" value="12" class="hidden">
                    <span class="font-medium" :class="language.timeFormat === '12' ? 'text-teal-700' : 'text-teal-500'">12-hour (2:30 PM)</span>
                  </label>
                  <label
                    :class="[
                      'flex-1 p-3 rounded-xl border-2 cursor-pointer text-center ripple',
                      language.timeFormat === '24' ? 'border-teal-500 bg-teal-50' : 'border-teal-100'
                    ]"
                  >
                    <input type="radio" v-model="language.timeFormat" value="24" class="hidden">
                    <span class="font-medium" :class="language.timeFormat === '24' ? 'text-teal-700' : 'text-teal-500'">24-hour (14:30)</span>
                  </label>
                </div>
              </div>

              <div>
                <label class="block text-sm font-medium text-teal-700 mb-2">First Day of Week</label>
                <select v-model="language.firstDay" class="input">
                  <option value="sunday">Sunday</option>
                  <option value="monday">Monday</option>
                  <option value="saturday">Saturday</option>
                </select>
              </div>

              <div class="pt-4 border-t border-teal-100 flex justify-end gap-3">
                <button class="btn btn-secondary ripple">Cancel</button>
                <button class="btn-vibrant ripple">Save Changes</button>
              </div>
            </div>
          </div>

          <!-- Security -->
          <div v-if="activeTab === 'security'" class="card-animated rounded-2xl overflow-hidden fade-in-up" style="animation-delay: 0.2s;">
            <div class="p-6 border-b border-teal-100">
              <h2 class="text-lg font-semibold text-teal-900">Security</h2>
              <p class="text-sm text-teal-500">Manage your security settings</p>
            </div>
            <div class="p-6 space-y-6">
              <!-- Password -->
              <div class="p-4 rounded-xl bg-teal-50">
                <div class="flex items-center justify-between mb-4">
                  <div>
                    <h4 class="font-medium text-teal-900">Password</h4>
                    <p class="text-sm text-teal-500">Last changed 3 months ago</p>
                  </div>
                  <button @click="showChangePassword = true" class="btn btn-secondary btn-sm ripple">Change Password</button>
                </div>
              </div>

              <!-- 2FA -->
              <div class="p-4 rounded-xl bg-teal-50">
                <div class="flex items-center justify-between">
                  <div>
                    <h4 class="font-medium text-teal-900">Two-Factor Authentication</h4>
                    <p class="text-sm text-teal-500">Add an extra layer of security</p>
                  </div>
                  <label class="toggle">
                    <input type="checkbox" v-model="security.twoFactor">
                    <span class="toggle-slider"></span>
                  </label>
                </div>
              </div>

              <!-- Sessions -->
              <div>
                <h4 class="font-medium text-teal-900 mb-4">Active Sessions</h4>
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
                      Revoke
                    </button>
                    <span v-else class="badge badge-green">Current</span>
                  </div>
                </div>
                <button class="w-full mt-3 py-2 text-center text-red-500 hover:text-red-700 font-medium">
                  Sign out all other sessions
                </button>
              </div>
            </div>
          </div>

          <!-- Connected Apps -->
          <div v-if="activeTab === 'apps'" class="card-animated rounded-2xl overflow-hidden fade-in-up" style="animation-delay: 0.2s;">
            <div class="p-6 border-b border-teal-100">
              <h2 class="text-lg font-semibold text-teal-900">Connected Apps</h2>
              <p class="text-sm text-teal-500">Manage third-party app integrations</p>
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
                    <p class="text-sm text-teal-500">Connected {{ app.connectedDate }}</p>
                  </div>
                </div>
                <button class="btn btn-secondary btn-sm text-red-500 hover:text-red-700 ripple">Disconnect</button>
              </div>

              <button class="w-full p-4 rounded-xl border-2 border-dashed border-teal-200 text-teal-500 hover:border-teal-400 hover:text-teal-700 transition-colors ripple">
                <i class="fas fa-plus mr-2 icon-soft"></i>Connect New App
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
          <h2 class="text-xl font-semibold text-teal-900">Change Password</h2>
          <button @click="showChangePassword = false" class="p-2 rounded-lg hover:bg-teal-100 text-teal-500 ripple">
            <i class="fas fa-times icon-soft"></i>
          </button>
        </div>
        <form @submit.prevent="changePassword" class="p-6 space-y-4">
          <div>
            <label class="block text-sm font-medium text-teal-700 mb-2">Current Password</label>
            <input type="password" class="input" required>
          </div>
          <div>
            <label class="block text-sm font-medium text-teal-700 mb-2">New Password</label>
            <input type="password" class="input" required>
          </div>
          <div>
            <label class="block text-sm font-medium text-teal-700 mb-2">Confirm New Password</label>
            <input type="password" class="input" required>
          </div>
          <div class="flex gap-3 pt-4">
            <button type="button" @click="showChangePassword = false" class="btn btn-secondary flex-1 ripple">Cancel</button>
            <button type="submit" class="btn-vibrant flex-1 ripple">Update Password</button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<style scoped>
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
</style>
