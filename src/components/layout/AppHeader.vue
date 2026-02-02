<script setup lang="ts">
import { ref, computed, onMounted, onBeforeUnmount } from 'vue'
import { useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useAuthStore } from '@/stores/auth'
import { useUIStore } from '@/stores/ui'
import { useNotificationsStore } from '@/stores/notifications'
import ViewAllButton from '@/components/common/ViewAllButton.vue'

const { t } = useI18n()
const router = useRouter()
const authStore = useAuthStore()
const uiStore = useUIStore()
const notificationsStore = useNotificationsStore()

// State
const searchQuery = ref('')
const showCreateMenu = ref(false)
const showNotifications = ref(false)
const showUserMenu = ref(false)

// Computed
const user = computed(() => authStore.user)
const userInitials = computed(() => {
  if (!user.value) return 'U'
  const names = user.value.displayName?.split(' ') || [user.value.email]
  return names.map(n => n[0]).join('').toUpperCase().slice(0, 2)
})

const unreadCount = computed(() => notificationsStore.unreadCount)
const recentNotifications = computed(() => notificationsStore.recentNotifications)

// Methods
function handleSearch() {
  if (searchQuery.value.trim()) {
    router.push({ name: 'SearchResults', query: { q: searchQuery.value } })
    searchQuery.value = ''
  }
}

function openAIAssistant() {
  router.push({ name: 'AIAssistant' })
}

function toggleCreate() {
  showCreateMenu.value = !showCreateMenu.value
  showNotifications.value = false
  showUserMenu.value = false
}

function toggleNotifications() {
  showNotifications.value = !showNotifications.value
  showCreateMenu.value = false
  showUserMenu.value = false
}

function toggleUserMenu() {
  showUserMenu.value = !showUserMenu.value
  showCreateMenu.value = false
  showNotifications.value = false
}

function closeDropdowns(e: MouseEvent) {
  if (!(e.target as Element).closest('.header-dropdown-trigger')) {
    showCreateMenu.value = false
    showNotifications.value = false
    showUserMenu.value = false
  }
}

function navigateTo(name: string) {
  router.push({ name })
  showCreateMenu.value = false
  showUserMenu.value = false
}

async function handleLogout() {
  await authStore.logout()
  router.push({ name: 'Login' })
}

function markAllRead() {
  notificationsStore.markAllAsRead()
}

function viewAllNotifications() {
  showNotifications.value = false
  // Navigate to dashboard where notifications can be viewed
  // In a future update, this could link to a dedicated notifications page
  router.push({ name: 'Dashboard' })
}

function openMessages() {
  router.push({ name: 'Collaboration' })
}

function handleNotificationClick(notif: { id: string; link?: string; isRead: boolean }) {
  // Mark as read
  if (!notif.isRead) {
    notificationsStore.markAsRead(notif.id)
  }
  // Navigate if link exists
  if (notif.link) {
    showNotifications.value = false
    router.push(notif.link)
  }
}

onMounted(() => {
  document.addEventListener('click', closeDropdowns)
  notificationsStore.fetchUnreadCount()
})

onBeforeUnmount(() => {
  document.removeEventListener('click', closeDropdowns)
})
</script>

<template>
  <header class="header-enhanced fixed top-0 left-0 right-0 z-50 h-14 sm:h-16">
    <div class="h-full px-2 sm:px-4 flex items-center justify-between">
      <!-- Left: Logo & Toggle -->
      <div class="flex items-center gap-2 sm:gap-3">
        <button @click="uiStore.toggleSidebar" class="header-btn header-btn-sm sm:header-btn group" :title="$t('header.toggleSidebar')">
          <i class="fas fa-bars text-gray-500 group-hover:text-teal-600 transition-colors"></i>
        </button>
        <div class="flex items-center gap-2 sm:gap-3">
          <router-link to="/">
            <img src="/Intalio.png" alt="Intalio" class="h-6 sm:h-8 object-contain">
          </router-link>
          <div class="hidden md:flex items-center gap-2 pl-3 border-l border-gray-200">
            <span class="text-sm font-semibold text-gray-700">Knowledge Hub</span>
          </div>
        </div>
      </div>

      <!-- Center: Enhanced Search -->
      <div class="flex-1 max-w-xs sm:max-w-md lg:max-w-2xl mx-2 sm:mx-4 lg:mx-6">
        <div class="search-container group">
          <div class="search-glow"></div>
          <div class="search-inner">
            <div class="flex items-center gap-2 sm:gap-3 px-3 sm:px-4">
              <i class="fas fa-search text-gray-400 group-focus-within:text-teal-500 transition-colors text-sm"></i>
              <input
                type="text"
                v-model="searchQuery"
                @keyup.enter="handleSearch"
                :placeholder="$t('header.searchPlaceholder')"
                class="flex-1 py-2 sm:py-2.5 bg-transparent text-sm text-gray-900 placeholder-gray-400 outline-none min-w-0"
              >
              <div class="flex items-center gap-1 sm:gap-2">
                <kbd class="hidden lg:inline-flex items-center gap-1 px-2 py-1 text-xs text-gray-400 bg-gray-100 rounded-md border border-gray-200">
                  <span class="text-[10px]">⌘</span>K
                </kbd>
                <div class="hidden sm:block w-px h-5 bg-gray-200"></div>
                <button @click="openAIAssistant" class="ai-search-btn group/ai" :title="$t('header.aiPoweredSearch')">
                  <i class="fas fa-wand-magic-sparkles text-xs"></i>
                  <span class="hidden sm:inline text-xs font-medium">{{ $t('header.askAI') }}</span>
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Right: Enhanced Actions -->
      <div class="flex items-center gap-0.5 sm:gap-1">
        <!-- Quick Create -->
        <div class="relative header-dropdown-trigger">
          <button @click.stop="toggleCreate" class="header-btn header-btn-sm sm:header-btn group" :title="$t('header.createNew')">
            <i class="fas fa-plus text-gray-500 group-hover:text-teal-600 transition-colors"></i>
          </button>
          <Transition name="dropdown">
            <div v-if="showCreateMenu" class="absolute right-0 top-full mt-2 w-56 bg-white rounded-xl shadow-lg border border-gray-100 z-50">
              <div class="px-3 py-2 border-b border-gray-100">
                <p class="text-xs font-semibold text-gray-400 uppercase tracking-wider">{{ $t('header.createNew') }}</p>
              </div>
              <div class="py-1">
                <button @click="navigateTo('ArticleCreate')" class="flex items-center gap-3 w-full px-3 py-2 hover:bg-gray-50">
                  <div class="w-8 h-8 rounded-lg bg-blue-50 flex items-center justify-center">
                    <i class="fas fa-file-alt text-blue-500 text-sm"></i>
                  </div>
                  <div class="text-left">
                    <p class="text-sm font-medium text-gray-700">{{ $t('nav.articles') }}</p>
                    <p class="text-xs text-gray-400">{{ $t('header.writeNewArticle') }}</p>
                  </div>
                </button>
                <button @click="navigateTo('PollCreate')" class="flex items-center gap-3 w-full px-3 py-2 hover:bg-gray-50">
                  <div class="w-8 h-8 rounded-lg bg-purple-50 flex items-center justify-center">
                    <i class="fas fa-poll text-purple-500 text-sm"></i>
                  </div>
                  <div class="text-left">
                    <p class="text-sm font-medium text-gray-700">{{ $t('nav.polls') }}</p>
                    <p class="text-xs text-gray-400">{{ $t('header.createSurvey') }}</p>
                  </div>
                </button>
                <button @click="navigateTo('Events')" class="flex items-center gap-3 w-full px-3 py-2 hover:bg-gray-50">
                  <div class="w-8 h-8 rounded-lg bg-green-50 flex items-center justify-center">
                    <i class="fas fa-calendar-plus text-green-500 text-sm"></i>
                  </div>
                  <div class="text-left">
                    <p class="text-sm font-medium text-gray-700">{{ $t('nav.events') }}</p>
                    <p class="text-xs text-gray-400">{{ $t('header.scheduleEvent') }}</p>
                  </div>
                </button>
                <button @click="navigateTo('Documents')" class="flex items-center gap-3 w-full px-3 py-2 hover:bg-gray-50">
                  <div class="w-8 h-8 rounded-lg bg-amber-50 flex items-center justify-center">
                    <i class="fas fa-folder-plus text-amber-500 text-sm"></i>
                  </div>
                  <div class="text-left">
                    <p class="text-sm font-medium text-gray-700">{{ $t('nav.documents') }}</p>
                    <p class="text-xs text-gray-400">{{ $t('header.uploadFile') }}</p>
                  </div>
                </button>
              </div>
            </div>
          </Transition>
        </div>

        <!-- Notifications -->
        <div class="relative header-dropdown-trigger">
          <button @click.stop="toggleNotifications" class="header-btn header-btn-sm sm:header-btn group" :title="$t('header.notifications')">
            <i class="fas fa-bell text-gray-500 group-hover:text-teal-600 transition-colors"></i>
            <span v-if="unreadCount > 0" class="notification-badge">
              {{ unreadCount > 9 ? '9+' : unreadCount }}
            </span>
          </button>
          <Transition name="dropdown">
            <div v-if="showNotifications" class="absolute right-0 top-full mt-2 w-80 bg-white rounded-xl shadow-lg border border-gray-100 z-50">
              <div class="px-4 py-3 border-b border-gray-100 flex items-center justify-between">
                <p class="text-sm font-semibold text-gray-800">{{ $t('header.notifications') }}</p>
                <button @click="markAllRead" class="text-xs text-teal-600 hover:text-teal-700 font-medium">{{ $t('header.markAllRead') }}</button>
              </div>
              <div class="max-h-80 overflow-y-auto">
                <div v-if="recentNotifications.length === 0" class="px-4 py-8 text-center text-gray-400">
                  <i class="fas fa-bell-slash text-2xl mb-2"></i>
                  <p class="text-sm">{{ $t('header.noNotifications') }}</p>
                </div>
                <button v-for="notif in recentNotifications" :key="notif.id"
                   @click="handleNotificationClick(notif)"
                   class="flex items-start gap-3 px-4 py-3 hover:bg-gray-50 transition-colors w-full text-left"
                   :class="{ 'bg-teal-50/50': !notif.isRead }">
                  <div class="w-8 h-8 rounded-lg flex items-center justify-center flex-shrink-0"
                       :class="notif.type === 'success' ? 'bg-green-100 text-green-600' :
                               notif.type === 'warning' ? 'bg-amber-100 text-amber-600' :
                               notif.type === 'error' ? 'bg-red-100 text-red-600' : 'bg-blue-100 text-blue-600'">
                    <i :class="notif.icon || 'fas fa-bell'" class="text-sm"></i>
                  </div>
                  <div class="flex-1 min-w-0">
                    <p class="text-sm text-gray-800 line-clamp-2">{{ notif.title }}</p>
                    <p class="text-xs text-gray-400 mt-0.5">{{ notif.message }}</p>
                  </div>
                  <div v-if="!notif.isRead" class="w-2 h-2 bg-teal-500 rounded-full flex-shrink-0 mt-2"></div>
                </button>
              </div>
              <div class="px-4 py-3 border-t border-gray-100 bg-gray-50/50 flex justify-center">
                <ViewAllButton variant="text" :label="$t('header.viewAllNotifications')" @click="viewAllNotifications" />
              </div>
            </div>
          </Transition>
        </div>

        <!-- Messages -->
        <button @click="openMessages" class="hidden sm:flex header-btn header-btn-sm sm:header-btn group" :title="$t('header.messages')">
          <i class="fas fa-comment-dots text-gray-500 group-hover:text-teal-600 transition-colors"></i>
          <span class="notification-badge small">3</span>
        </button>

        <!-- Divider -->
        <div class="hidden sm:block w-px h-6 sm:h-8 bg-gray-200 mx-1 sm:mx-2"></div>

        <!-- User Menu -->
        <div class="relative header-dropdown-trigger">
          <button @click.stop="toggleUserMenu" class="flex items-center gap-1 sm:gap-3 px-1 sm:px-2 py-1 rounded-xl hover:bg-gray-50 transition-colors group">
            <div class="hidden md:block text-right">
              <p class="text-sm font-semibold text-gray-800 group-hover:text-teal-700 transition-colors">{{ user?.displayName || 'User' }}</p>
              <p class="text-xs text-gray-500 flex items-center justify-end gap-1">
                <span class="w-1.5 h-1.5 bg-emerald-500 rounded-full"></span>
                {{ $t('user.online') }}
              </p>
            </div>
            <div class="w-8 h-8 sm:w-10 sm:h-10 rounded-lg sm:rounded-xl bg-gradient-to-br from-teal-400 to-teal-600 flex items-center justify-center text-white font-bold text-xs sm:text-sm shadow-lg relative">
              <span v-if="!user?.avatar">{{ userInitials }}</span>
              <img v-else :src="user.avatar" :alt="user.displayName" class="w-full h-full object-cover rounded-lg sm:rounded-xl">
              <span class="absolute -bottom-0.5 -right-0.5 w-2.5 h-2.5 sm:w-3 sm:h-3 bg-emerald-500 border-2 border-white rounded-full"></span>
            </div>
            <i class="hidden sm:block fas fa-chevron-down text-[10px] text-gray-400 group-hover:text-teal-600 transition-colors"></i>
          </button>
          <Transition name="dropdown">
            <div v-if="showUserMenu" class="absolute right-0 top-full mt-2 w-64 bg-white rounded-xl shadow-lg border border-gray-100 z-50">
              <div class="px-4 py-4 border-b border-gray-100 bg-gradient-to-br from-teal-50 to-white rounded-t-xl">
                <div class="flex items-center gap-3">
                  <div class="w-12 h-12 rounded-xl bg-gradient-to-br from-teal-400 to-teal-600 flex items-center justify-center text-white font-bold text-lg shadow-lg">
                    {{ userInitials }}
                  </div>
                  <div>
                    <p class="font-semibold text-gray-800">{{ user?.displayName }}</p>
                    <p class="text-xs text-gray-500">{{ user?.jobTitle || user?.role }}</p>
                    <p class="text-xs text-teal-600 mt-0.5">{{ user?.email }}</p>
                  </div>
                </div>
              </div>
              <div class="py-2">
                <button @click="navigateTo('Profile')" class="flex items-center gap-3 w-full px-4 py-2 text-left hover:bg-gray-50">
                  <i class="fas fa-user w-5 text-gray-400"></i>
                  <span class="text-sm text-gray-700">{{ $t('user.myProfile') }}</span>
                </button>
                <button @click="navigateTo('Settings')" class="flex items-center gap-3 w-full px-4 py-2 text-left hover:bg-gray-50">
                  <i class="fas fa-cog w-5 text-gray-400"></i>
                  <span class="text-sm text-gray-700">{{ $t('nav.settings') }}</span>
                </button>
              </div>
              <div class="border-t border-gray-100 py-2">
                <button @click="handleLogout" class="flex items-center gap-3 w-full px-4 py-2 text-left hover:bg-red-50 text-red-600">
                  <i class="fas fa-sign-out-alt w-5"></i>
                  <span class="text-sm font-medium">{{ $t('auth.signOut') }}</span>
                </button>
              </div>
            </div>
          </Transition>
        </div>
      </div>
    </div>
  </header>
</template>

<style scoped>
.dropdown-enter-active,
.dropdown-leave-active {
  transition: all 0.2s ease;
}

.dropdown-enter-from,
.dropdown-leave-to {
  opacity: 0;
  transform: translateY(-10px);
}
</style>
