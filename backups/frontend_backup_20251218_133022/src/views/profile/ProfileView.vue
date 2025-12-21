<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useI18n } from 'vue-i18n'
import { useReducedMotion } from '@/composables/useReducedMotion'
import { PageHeader, Avatar } from '@/components/base'
import type { BreadcrumbItem } from '@/components/base/PageHeader.vue'
import ErrorState from '@/components/base/ErrorState.vue'
import Button from 'primevue/button'
import InputText from 'primevue/inputtext'
import Dropdown from 'primevue/dropdown'
import Textarea from 'primevue/textarea'
import Dialog from 'primevue/dialog'
import Password from 'primevue/password'
import InputSwitch from 'primevue/inputswitch'
import Skeleton from 'primevue/skeleton'

const { locale } = useI18n()
const prefersReducedMotion = useReducedMotion()

// RTL support
const isRTL = computed(() => locale.value === 'ar')

// Animation control
const isContentVisible = ref(false)
const shouldAnimate = computed(() => !prefersReducedMotion.value)
const LOADING_DELAY = 600

// Breadcrumbs for PageHeader
const breadcrumbs = computed<BreadcrumbItem[]>(() => [
  { label: 'Home', labelArabic: 'الرئيسية', to: '/dashboard' },
  { label: isRTL.value ? 'الملف الشخصي' : 'Profile' }
])

// Loading and error state
const loading = ref(true)
const error = ref<Error | null>(null)
const saving = ref(false)
const activeTab = ref<'profile' | 'security' | 'preferences' | 'activity'>('profile')
const showPasswordDialog = ref(false)
const showAvatarDialog = ref(false)

// Type configurations
const ACTIVITY_CONFIG: Record<string, { icon: string; color: string; bgColor: string; label: string; labelAr: string }> = {
  document: { icon: 'pi pi-file', color: '#10b981', bgColor: 'rgba(16, 185, 129, 0.1)', label: 'Document', labelAr: 'مستند' },
  article: { icon: 'pi pi-file-edit', color: '#3b82f6', bgColor: 'rgba(59, 130, 246, 0.1)', label: 'Article', labelAr: 'مقال' },
  task: { icon: 'pi pi-check-circle', color: '#8b5cf6', bgColor: 'rgba(139, 92, 246, 0.1)', label: 'Task', labelAr: 'مهمة' },
  comment: { icon: 'pi pi-comment', color: '#f59e0b', bgColor: 'rgba(245, 158, 11, 0.1)', label: 'Comment', labelAr: 'تعليق' },
  community: { icon: 'pi pi-users', color: '#ec4899', bgColor: 'rgba(236, 72, 153, 0.1)', label: 'Community', labelAr: 'مجتمع' }
}

const ACTION_CONFIG: Record<string, { label: string; labelAr: string }> = {
  uploaded: { label: 'Uploaded', labelAr: 'رفع' },
  published: { label: 'Published', labelAr: 'نشر' },
  completed: { label: 'Completed', labelAr: 'أكمل' },
  added: { label: 'Added comment on', labelAr: 'أضاف تعليقاً على' },
  joined: { label: 'Joined', labelAr: 'انضم إلى' }
}

// Interfaces
interface Profile {
  displayName: string
  displayNameAr: string
  email: string
  jobTitle: string
  jobTitleAr: string
  department: string
  departmentAr: string
  phone: string
  location: string
  locationAr: string
  bio: string
  bioAr: string
  avatarUrl?: string
  skills: string[]
  preferredLanguage: string
  joinedAt: Date
  lastActive: Date
}

interface Activity {
  id: string
  type: string
  action: string
  title: string
  titleAr?: string
  createdAt: Date
}

interface Session {
  id: string
  device: string
  browser: string
  location: string
  lastActive: Date
  isCurrent: boolean
}

// Profile data
const profile = ref<Profile>({
  displayName: '',
  displayNameAr: '',
  email: '',
  jobTitle: '',
  jobTitleAr: '',
  department: '',
  departmentAr: '',
  phone: '',
  location: '',
  locationAr: '',
  bio: '',
  bioAr: '',
  skills: [],
  preferredLanguage: 'en',
  joinedAt: new Date(),
  lastActive: new Date()
})

// Password form
const passwordForm = ref({
  currentPassword: '',
  newPassword: '',
  confirmPassword: ''
})

// Activities
const activities = ref<Activity[]>([])

// Sessions
const sessions = ref<Session[]>([])

// Notification settings
const notificationSettings = ref({
  emailNotifications: true,
  pushNotifications: true,
  taskAssignments: true,
  mentions: true,
  weeklyDigest: false
})

// Tabs
const tabs = computed(() => [
  { key: 'profile', label: isRTL.value ? 'الملف الشخصي' : 'Profile', icon: 'pi pi-user' },
  { key: 'security', label: isRTL.value ? 'الأمان' : 'Security', icon: 'pi pi-shield' },
  { key: 'preferences', label: isRTL.value ? 'التفضيلات' : 'Preferences', icon: 'pi pi-cog' },
  { key: 'activity', label: isRTL.value ? 'النشاط' : 'Activity', icon: 'pi pi-clock' }
])

// Language options
const languageOptions = computed(() => [
  { label: 'English', value: 'en' },
  { label: 'العربية', value: 'ar' }
])

// Stats
const stats = computed(() => [
  { value: 47, label: isRTL.value ? 'مساهمة' : 'Contributions', icon: 'pi pi-file-edit', color: '#3b82f6' },
  { value: 12, label: isRTL.value ? 'مهمة مكتملة' : 'Tasks Done', icon: 'pi pi-check-circle', color: '#10b981' },
  { value: 3, label: isRTL.value ? 'مجتمع' : 'Communities', icon: 'pi pi-users', color: '#8b5cf6' }
])

// Helper functions
const getActivityConfig = (type: string) => ACTIVITY_CONFIG[type] || ACTIVITY_CONFIG.document
const getActionConfig = (action: string) => ACTION_CONFIG[action] || ACTION_CONFIG.uploaded

const getName = () => isRTL.value && profile.value.displayNameAr ? profile.value.displayNameAr : profile.value.displayName
const getJobTitle = () => isRTL.value && profile.value.jobTitleAr ? profile.value.jobTitleAr : profile.value.jobTitle
const getDepartment = () => isRTL.value && profile.value.departmentAr ? profile.value.departmentAr : profile.value.department
const getLocation = () => isRTL.value && profile.value.locationAr ? profile.value.locationAr : profile.value.location

const formatDate = (date: Date) => {
  return new Date(date).toLocaleDateString(isRTL.value ? 'ar-SA' : 'en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  })
}

const formatRelativeTime = (date: Date) => {
  const now = new Date()
  const diffMs = now.getTime() - new Date(date).getTime()
  const diffMins = Math.floor(diffMs / 60000)
  const diffHours = Math.floor(diffMins / 60)
  const diffDays = Math.floor(diffHours / 24)

  if (diffMins < 60) {
    return isRTL.value ? `منذ ${diffMins} دقيقة` : `${diffMins}m ago`
  } else if (diffHours < 24) {
    return isRTL.value ? `منذ ${diffHours} ساعة` : `${diffHours}h ago`
  } else if (diffDays < 7) {
    return isRTL.value ? `منذ ${diffDays} يوم` : `${diffDays}d ago`
  } else {
    return formatDate(date)
  }
}

// Actions
const saveProfile = async () => {
  saving.value = true
  await new Promise(resolve => setTimeout(resolve, 1000))
  saving.value = false
}

const changePassword = async () => {
  saving.value = true
  await new Promise(resolve => setTimeout(resolve, 1000))
  showPasswordDialog.value = false
  passwordForm.value = { currentPassword: '', newPassword: '', confirmPassword: '' }
  saving.value = false
}

const revokeSession = async (sessionId: string) => {
  sessions.value = sessions.value.filter(s => s.id !== sessionId)
}

const uploadAvatar = () => {
  showAvatarDialog.value = true
}

// Load data
async function loadProfile() {
  try {
    error.value = null
    loading.value = true

    await new Promise(resolve => setTimeout(resolve, LOADING_DELAY))

    profile.value = {
      displayName: 'Mohammed Al-Rashid',
      displayNameAr: 'محمد الراشد',
      email: 'mohammed.alrashid@afc2027.qa',
      jobTitle: 'Operations Director',
      jobTitleAr: 'مدير العمليات',
      department: 'Operations',
      departmentAr: 'العمليات',
      phone: '+974 5512 3456',
      location: 'Doha, Qatar',
      locationAr: 'الدوحة، قطر',
      bio: 'Leading the operations team for AFC Asian Cup 2027. Passionate about delivering world-class sporting events with over 15 years of experience in event management.',
      bioAr: 'قيادة فريق العمليات لكأس آسيا 2027. شغوف بتقديم فعاليات رياضية عالمية المستوى مع أكثر من 15 عاماً من الخبرة في إدارة الفعاليات.',
      avatarUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Mohammed',
      skills: ['Event Management', 'Operations', 'Team Leadership', 'Strategic Planning', 'Budget Management'],
      preferredLanguage: 'en',
      joinedAt: new Date(Date.now() - 180 * 24 * 60 * 60 * 1000),
      lastActive: new Date(Date.now() - 5 * 60 * 1000)
    }

    activities.value = [
      { id: '1', type: 'document', action: 'uploaded', title: 'Operations Manual v2.1', titleAr: 'دليل العمليات الإصدار 2.1', createdAt: new Date(Date.now() - 2 * 60 * 60 * 1000) },
      { id: '2', type: 'article', action: 'published', title: 'Venue Preparation Update', titleAr: 'تحديث إعداد الملعب', createdAt: new Date(Date.now() - 24 * 60 * 60 * 1000) },
      { id: '3', type: 'task', action: 'completed', title: 'Review security protocols', titleAr: 'مراجعة بروتوكولات الأمن', createdAt: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000) },
      { id: '4', type: 'comment', action: 'added', title: 'Stadium Layout Discussion', titleAr: 'نقاش تخطيط الملعب', createdAt: new Date(Date.now() - 3 * 24 * 60 * 60 * 1000) },
      { id: '5', type: 'community', action: 'joined', title: 'Volunteer Coordinators', titleAr: 'منسقو المتطوعين', createdAt: new Date(Date.now() - 5 * 24 * 60 * 60 * 1000) }
    ]

    sessions.value = [
      { id: '1', device: 'Desktop', browser: 'Chrome on Windows', location: 'Doha, QA', lastActive: new Date(), isCurrent: true },
      { id: '2', device: 'Mobile', browser: 'Safari on iPhone', location: 'Doha, QA', lastActive: new Date(Date.now() - 2 * 60 * 60 * 1000), isCurrent: false },
      { id: '3', device: 'Tablet', browser: 'Chrome on iPad', location: 'Riyadh, SA', lastActive: new Date(Date.now() - 24 * 60 * 60 * 1000), isCurrent: false }
    ]

    loading.value = false

    if (shouldAnimate.value) {
      requestAnimationFrame(() => {
        isContentVisible.value = true
      })
    } else {
      isContentVisible.value = true
    }
  } catch (e) {
    error.value = e as Error
    loading.value = false
  }
}

async function handleRetry() {
  await loadProfile()
}

onMounted(() => {
  loadProfile()
})
</script>

<template>
  <div class="profile-view" :class="{ rtl: isRTL }">
    <!-- Page Header -->
    <PageHeader
      :title="isRTL ? 'الملف الشخصي' : 'My Profile'"
      :description="isRTL ? 'إدارة معلوماتك الشخصية والإعدادات' : 'Manage your personal information and settings'"
      :breadcrumbs="breadcrumbs"
      padding-bottom="xl"
      background-variant="gradient"
      :animated="shouldAnimate"
    />

    <!-- Profile Header Card -->
    <div class="profile-header-section">
      <div class="profile-header-card">
          <div v-if="loading" class="loading-header">
            <Skeleton shape="circle" size="120px" />
            <div class="loading-info">
              <Skeleton width="200px" height="32px" class="mb-2" />
              <Skeleton width="150px" height="20px" class="mb-4" />
              <Skeleton width="300px" height="16px" />
            </div>
          </div>

          <template v-else>
            <div class="avatar-section">
              <div class="avatar-wrapper">
                <Avatar
                  :image="profile.avatarUrl"
                  :name="profile.displayName"
                  size="2xl"
                  shape="circle"
                  class="profile-avatar"
                />
                <button class="avatar-edit-btn" @click="uploadAvatar">
                  <i class="pi pi-camera"></i>
                </button>
              </div>
            </div>

            <div class="profile-info">
              <h1>{{ getName() }}</h1>
              <p class="job-title">{{ getJobTitle() }}</p>
              <div class="profile-badges">
                <span class="badge department">
                  <i class="pi pi-building"></i>
                  {{ getDepartment() }}
                </span>
                <span class="badge location">
                  <i class="pi pi-map-marker"></i>
                  {{ getLocation() }}
                </span>
              </div>
              <div class="contact-row">
                <span class="contact-item">
                  <i class="pi pi-envelope"></i>
                  {{ profile.email }}
                </span>
                <span class="contact-item">
                  <i class="pi pi-phone"></i>
                  {{ profile.phone }}
                </span>
              </div>
            </div>

            <div class="profile-stats">
              <div v-for="stat in stats" :key="stat.label" class="stat-box">
                <div class="stat-icon" :style="{ background: stat.color + '15', color: stat.color }">
                  <i :class="stat.icon"></i>
                </div>
                <div class="stat-info">
                  <span class="stat-value">{{ stat.value }}</span>
                  <span class="stat-label">{{ stat.label }}</span>
                </div>
              </div>
            </div>
          </template>
      </div>
    </div>

    <!-- Main Content -->
    <div class="main-content">
      <!-- Tabs Navigation -->
      <div class="tabs-nav">
        <button
          v-for="tab in tabs"
          :key="tab.key"
          class="tab-btn"
          :class="{ active: activeTab === tab.key }"
          @click="activeTab = tab.key as typeof activeTab"
        >
          <i :class="tab.icon"></i>
          <span>{{ tab.label }}</span>
        </button>
      </div>

      <!-- Tab Content -->
      <div class="tab-content">
        <!-- Loading State -->
        <div v-if="loading" class="loading-content">
          <Skeleton width="100%" height="300px" borderRadius="16px" />
        </div>

        <!-- Error State -->
        <ErrorState
          v-else-if="error"
          :error="error"
          :title="isRTL ? 'فشل تحميل الملف الشخصي' : 'Failed to load profile'"
          show-retry
          @retry="handleRetry"
        />

        <!-- Profile Tab -->
        <div v-else-if="activeTab === 'profile'" class="profile-tab">
          <div class="content-card">
            <div class="card-header">
              <h2>{{ isRTL ? 'المعلومات الشخصية' : 'Personal Information' }}</h2>
            </div>
            <div class="card-body">
              <div class="form-grid">
                <div class="form-field">
                  <label>{{ isRTL ? 'الاسم (إنجليزي)' : 'Name (English)' }}</label>
                  <InputText v-model="profile.displayName" />
                </div>
                <div class="form-field">
                  <label>{{ isRTL ? 'الاسم (عربي)' : 'Name (Arabic)' }}</label>
                  <InputText v-model="profile.displayNameAr" dir="rtl" />
                </div>
                <div class="form-field">
                  <label>{{ isRTL ? 'البريد الإلكتروني' : 'Email' }}</label>
                  <InputText v-model="profile.email" disabled />
                </div>
                <div class="form-field">
                  <label>{{ isRTL ? 'رقم الهاتف' : 'Phone' }}</label>
                  <InputText v-model="profile.phone" />
                </div>
                <div class="form-field">
                  <label>{{ isRTL ? 'المسمى الوظيفي' : 'Job Title' }}</label>
                  <InputText v-model="profile.jobTitle" />
                </div>
                <div class="form-field">
                  <label>{{ isRTL ? 'الموقع' : 'Location' }}</label>
                  <InputText v-model="profile.location" />
                </div>
                <div class="form-field full-width">
                  <label>{{ isRTL ? 'نبذة عني' : 'Bio' }}</label>
                  <Textarea v-model="profile.bio" rows="4" autoResize />
                </div>
              </div>
            </div>
          </div>

          <div class="content-card">
            <div class="card-header">
              <h2>{{ isRTL ? 'المهارات' : 'Skills & Expertise' }}</h2>
            </div>
            <div class="card-body">
              <div class="skills-grid">
                <div v-for="skill in profile.skills" :key="skill" class="skill-tag">
                  {{ skill }}
                </div>
                <button class="add-skill-btn">
                  <i class="pi pi-plus"></i>
                  {{ isRTL ? 'إضافة مهارة' : 'Add Skill' }}
                </button>
              </div>
            </div>
          </div>

          <div class="form-actions">
            <Button
              :label="isRTL ? 'حفظ التغييرات' : 'Save Changes'"
              icon="pi pi-check"
              :loading="saving"
              @click="saveProfile"
            />
          </div>
        </div>

        <!-- Security Tab -->
        <div v-else-if="activeTab === 'security'" class="security-tab">
          <div class="content-card">
            <div class="card-header">
              <h2>{{ isRTL ? 'كلمة المرور' : 'Password' }}</h2>
            </div>
            <div class="card-body">
              <p class="card-description">
                {{ isRTL ? 'قم بتغيير كلمة المرور الخاصة بك لتأمين حسابك' : 'Change your password to secure your account' }}
              </p>
              <Button
                :label="isRTL ? 'تغيير كلمة المرور' : 'Change Password'"
                icon="pi pi-lock"
                severity="secondary"
                @click="showPasswordDialog = true"
              />
            </div>
          </div>

          <div class="content-card">
            <div class="card-header">
              <h2>{{ isRTL ? 'المصادقة الثنائية' : 'Two-Factor Authentication' }}</h2>
              <span class="status-badge disabled">{{ isRTL ? 'معطل' : 'Disabled' }}</span>
            </div>
            <div class="card-body">
              <p class="card-description">
                {{ isRTL ? 'أضف طبقة إضافية من الأمان إلى حسابك' : 'Add an extra layer of security to your account' }}
              </p>
              <Button
                :label="isRTL ? 'تفعيل' : 'Enable'"
                icon="pi pi-shield"
                severity="secondary"
              />
            </div>
          </div>

          <div class="content-card">
            <div class="card-header">
              <h2>{{ isRTL ? 'الجلسات النشطة' : 'Active Sessions' }}</h2>
            </div>
            <div class="card-body">
              <div class="sessions-list">
                <div
                  v-for="session in sessions"
                  :key="session.id"
                  class="session-item"
                  :class="{ current: session.isCurrent }"
                >
                  <div class="session-icon">
                    <i :class="session.device === 'Desktop' ? 'pi pi-desktop' : session.device === 'Mobile' ? 'pi pi-mobile' : 'pi pi-tablet'"></i>
                  </div>
                  <div class="session-info">
                    <span class="session-device">{{ session.browser }}</span>
                    <span class="session-meta">
                      {{ session.location }}
                      <span v-if="session.isCurrent" class="current-badge">{{ isRTL ? 'الجلسة الحالية' : 'Current' }}</span>
                      <span v-else>{{ formatRelativeTime(session.lastActive) }}</span>
                    </span>
                  </div>
                  <Button
                    v-if="!session.isCurrent"
                    :label="isRTL ? 'إنهاء' : 'Revoke'"
                    severity="danger"
                    text
                    size="small"
                    @click="revokeSession(session.id)"
                  />
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Preferences Tab -->
        <div v-else-if="activeTab === 'preferences'" class="preferences-tab">
          <div class="content-card">
            <div class="card-header">
              <h2>{{ isRTL ? 'اللغة والمنطقة' : 'Language & Region' }}</h2>
            </div>
            <div class="card-body">
              <div class="form-field">
                <label>{{ isRTL ? 'اللغة المفضلة' : 'Preferred Language' }}</label>
                <Dropdown
                  v-model="profile.preferredLanguage"
                  :options="languageOptions"
                  optionLabel="label"
                  optionValue="value"
                  class="language-dropdown"
                />
              </div>
            </div>
          </div>

          <div class="content-card">
            <div class="card-header">
              <h2>{{ isRTL ? 'الإشعارات' : 'Notifications' }}</h2>
            </div>
            <div class="card-body">
              <div class="preferences-list">
                <div class="preference-item">
                  <div class="preference-info">
                    <span class="preference-title">{{ isRTL ? 'إشعارات البريد الإلكتروني' : 'Email Notifications' }}</span>
                    <span class="preference-desc">{{ isRTL ? 'تلقي تحديثات مهمة عبر البريد الإلكتروني' : 'Receive important updates via email' }}</span>
                  </div>
                  <InputSwitch v-model="notificationSettings.emailNotifications" />
                </div>
                <div class="preference-item">
                  <div class="preference-info">
                    <span class="preference-title">{{ isRTL ? 'الإشعارات الفورية' : 'Push Notifications' }}</span>
                    <span class="preference-desc">{{ isRTL ? 'تلقي إشعارات في المتصفح' : 'Receive browser notifications' }}</span>
                  </div>
                  <InputSwitch v-model="notificationSettings.pushNotifications" />
                </div>
                <div class="preference-item">
                  <div class="preference-info">
                    <span class="preference-title">{{ isRTL ? 'تعيين المهام' : 'Task Assignments' }}</span>
                    <span class="preference-desc">{{ isRTL ? 'إشعار عند تعيين مهام جديدة' : 'Notify when new tasks are assigned' }}</span>
                  </div>
                  <InputSwitch v-model="notificationSettings.taskAssignments" />
                </div>
                <div class="preference-item">
                  <div class="preference-info">
                    <span class="preference-title">{{ isRTL ? 'الإشارات' : 'Mentions' }}</span>
                    <span class="preference-desc">{{ isRTL ? 'إشعار عند الإشارة إليك' : 'Notify when you are mentioned' }}</span>
                  </div>
                  <InputSwitch v-model="notificationSettings.mentions" />
                </div>
                <div class="preference-item">
                  <div class="preference-info">
                    <span class="preference-title">{{ isRTL ? 'الملخص الأسبوعي' : 'Weekly Digest' }}</span>
                    <span class="preference-desc">{{ isRTL ? 'ملخص أسبوعي لنشاطك' : 'Weekly summary of your activity' }}</span>
                  </div>
                  <InputSwitch v-model="notificationSettings.weeklyDigest" />
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Activity Tab -->
        <div v-else-if="activeTab === 'activity'" class="activity-tab">
          <div class="content-card">
            <div class="card-header">
              <h2>{{ isRTL ? 'النشاط الأخير' : 'Recent Activity' }}</h2>
            </div>
            <div class="card-body">
              <div class="activity-timeline">
                <div
                  v-for="(activity, index) in activities"
                  :key="activity.id"
                  class="activity-item"
                  :style="{ '--animation-order': index }"
                >
                  <div class="activity-icon" :style="{ background: getActivityConfig(activity.type).bgColor, color: getActivityConfig(activity.type).color }">
                    <i :class="getActivityConfig(activity.type).icon"></i>
                  </div>
                  <div class="activity-content">
                    <p class="activity-text">
                      <strong>{{ isRTL ? getActionConfig(activity.action).labelAr : getActionConfig(activity.action).label }}</strong>
                      {{ isRTL && activity.titleAr ? activity.titleAr : activity.title }}
                    </p>
                    <span class="activity-time">{{ formatRelativeTime(activity.createdAt) }}</span>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div class="content-card">
            <div class="card-header">
              <h2>{{ isRTL ? 'معلومات الحساب' : 'Account Info' }}</h2>
            </div>
            <div class="card-body">
              <div class="account-info">
                <div class="info-row">
                  <span class="info-label">{{ isRTL ? 'تاريخ الانضمام' : 'Joined' }}</span>
                  <span class="info-value">{{ formatDate(profile.joinedAt) }}</span>
                </div>
                <div class="info-row">
                  <span class="info-label">{{ isRTL ? 'آخر نشاط' : 'Last Active' }}</span>
                  <span class="info-value">{{ formatRelativeTime(profile.lastActive) }}</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Password Change Dialog -->
    <Dialog
      v-model:visible="showPasswordDialog"
      :header="isRTL ? 'تغيير كلمة المرور' : 'Change Password'"
      :modal="true"
      :style="{ width: '450px' }"
      class="password-dialog"
    >
      <div class="password-form">
        <div class="form-field">
          <label>{{ isRTL ? 'كلمة المرور الحالية' : 'Current Password' }}</label>
          <Password v-model="passwordForm.currentPassword" :feedback="false" toggleMask />
        </div>
        <div class="form-field">
          <label>{{ isRTL ? 'كلمة المرور الجديدة' : 'New Password' }}</label>
          <Password v-model="passwordForm.newPassword" toggleMask />
        </div>
        <div class="form-field">
          <label>{{ isRTL ? 'تأكيد كلمة المرور' : 'Confirm Password' }}</label>
          <Password v-model="passwordForm.confirmPassword" :feedback="false" toggleMask />
        </div>
      </div>
      <template #footer>
        <Button :label="isRTL ? 'إلغاء' : 'Cancel'" text @click="showPasswordDialog = false" />
        <Button :label="isRTL ? 'تغيير' : 'Change'" icon="pi pi-check" :loading="saving" @click="changePassword" />
      </template>
    </Dialog>

    <!-- Avatar Upload Dialog -->
    <Dialog
      v-model:visible="showAvatarDialog"
      :header="isRTL ? 'تغيير الصورة الشخصية' : 'Change Profile Picture'"
      :modal="true"
      :style="{ width: '450px' }"
      class="avatar-dialog"
    >
      <div class="avatar-upload-area">
        <div class="upload-zone">
          <i class="pi pi-cloud-upload"></i>
          <p>{{ isRTL ? 'اسحب صورة هنا أو انقر للاختيار' : 'Drag an image here or click to select' }}</p>
          <span>{{ isRTL ? 'PNG, JPG حتى 5MB' : 'PNG, JPG up to 5MB' }}</span>
        </div>
      </div>
      <template #footer>
        <Button :label="isRTL ? 'إلغاء' : 'Cancel'" text @click="showAvatarDialog = false" />
        <Button :label="isRTL ? 'رفع' : 'Upload'" icon="pi pi-upload" />
      </template>
    </Dialog>
  </div>
</template>

<style scoped lang="scss">
@use '@/assets/styles/_variables.scss' as *;
@use '@/assets/styles/_mixins.scss' as *;

// ============================================
// PROFILE VIEW - MAIN CONTAINER
// Following Universal Spacing Guidelines from _mixins.scss
// ============================================

.profile-view {
  @include page-view;
}

// Profile Header Section (positioned below PageHeader)
.profile-header-section {
  margin: -3rem 2rem 2rem;
  position: relative;
  z-index: 10;
}

// Profile Header Card
.profile-header-card {
  display: flex;
  gap: $spacing-8;
  padding: $spacing-8;
  background: #fff;
  border-radius: $radius-2xl;
  box-shadow: $shadow-lg;
  margin-top: $spacing-8;

  @media (max-width: 768px) {
    flex-direction: column;
    align-items: center;
    text-align: center;
  }
}

.loading-header {
  display: flex;
  gap: $spacing-8;
  align-items: center;
  width: 100%;

  .loading-info {
    flex: 1;
  }
}

.avatar-section {
  flex-shrink: 0;
}

.avatar-wrapper {
  position: relative;

  .profile-avatar {
    width: 120px !important;
    height: 120px !important;
    font-size: $text-4xl;
    background: linear-gradient(135deg, $intalio-teal-500, $intalio-teal-600);
    color: #fff;
  }

  .avatar-edit-btn {
    position: absolute;
    bottom: 0;
    right: 0;
    width: 36px;
    height: 36px;
    background: $intalio-teal-500;
    border: 3px solid #fff;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    cursor: pointer;
    transition: all $transition-fast;

    i {
      color: #fff;
      font-size: $text-sm;
    }

    &:hover {
      background: $intalio-teal-600;
      transform: scale(1.1);
    }

    .rtl & {
      right: auto;
      left: 0;
    }
  }
}

.profile-info {
  flex: 1;

  h1 {
    margin: 0;
    font-size: $text-2xl;
    font-weight: $font-weight-bold;
    color: $slate-900;
  }

  .job-title {
    margin: $spacing-1 0;
    font-size: $text-lg;
    color: $slate-600;
  }

  .profile-badges {
    display: flex;
    flex-wrap: wrap;
    gap: $spacing-2;
    margin: $spacing-3 0;

    .badge {
      display: inline-flex;
      align-items: center;
      gap: $spacing-1;
      padding: $spacing-1 $spacing-3;
      background: $slate-100;
      border-radius: $radius-full;
      font-size: $text-sm;
      color: $slate-700;

      i {
        font-size: $text-xs;
      }
    }
  }

  .contact-row {
    display: flex;
    flex-wrap: wrap;
    gap: $spacing-4;
    margin-top: $spacing-2;

    .contact-item {
      display: flex;
      align-items: center;
      gap: $spacing-2;
      font-size: $text-sm;
      color: $slate-500;

      i {
        color: $slate-400;
      }
    }
  }
}

.profile-stats {
  display: flex;
  flex-direction: column;
  gap: $spacing-3;

  @media (max-width: 768px) {
    flex-direction: row;
    justify-content: center;
  }
}

.stat-box {
  display: flex;
  align-items: center;
  gap: $spacing-3;
  padding: $spacing-3 $spacing-4;
  background: $slate-50;
  border-radius: $radius-xl;

  .stat-icon {
    width: 40px;
    height: 40px;
    border-radius: $radius-lg;
    display: flex;
    align-items: center;
    justify-content: center;

    i {
      font-size: $text-lg;
    }
  }

  .stat-info {
    display: flex;
    flex-direction: column;

    .stat-value {
      font-size: $text-xl;
      font-weight: $font-weight-bold;
      color: $slate-900;
    }

    .stat-label {
      font-size: $text-xs;
      color: $slate-500;
    }
  }
}

// ============================================
// MAIN CONTENT
// ============================================

.main-content {
  max-width: 900px;
  margin: -$spacing-10 auto 0;
  padding: 0 $spacing-6 $spacing-6;
  position: relative;
  z-index: 1;
}

// Tabs Navigation
.tabs-nav {
  display: flex;
  gap: $spacing-1;
  background: #fff;
  padding: $spacing-1;
  border-radius: $radius-xl;
  box-shadow: $shadow-sm;
  margin-bottom: $spacing-6;
  overflow-x: auto;
}

.tab-btn {
  display: flex;
  align-items: center;
  gap: $spacing-2;
  padding: $spacing-3 $spacing-5;
  background: none;
  border: none;
  border-radius: $radius-lg;
  font-size: $text-sm;
  font-weight: $font-weight-medium;
  color: $slate-600;
  cursor: pointer;
  transition: all $transition-fast;
  white-space: nowrap;

  &:hover {
    background: $slate-100;
    color: $slate-900;
  }

  &.active {
    background: $intalio-teal-500;
    color: #fff;
  }
}

// Content Cards
.content-card {
  background: #fff;
  border-radius: $radius-2xl;
  box-shadow: $shadow-sm;
  margin-bottom: $spacing-5;
  overflow: hidden;

  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: $spacing-5;
    border-bottom: 1px solid $slate-100;

    h2 {
      margin: 0;
      font-size: $text-lg;
      font-weight: $font-weight-semibold;
      color: $slate-900;
    }

    .status-badge {
      padding: $spacing-1 $spacing-3;
      border-radius: $radius-full;
      font-size: $text-xs;
      font-weight: $font-weight-medium;

      &.disabled {
        background: $slate-100;
        color: $slate-600;
      }

      &.enabled {
        background: rgba($intalio-teal-500, 0.1);
        color: $intalio-teal-600;
      }
    }
  }

  .card-body {
    padding: $spacing-5;
  }

  .card-description {
    margin: 0 0 $spacing-4;
    color: $slate-600;
    font-size: $text-sm;
  }
}

// Form Grid
.form-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: $spacing-5;

  @media (max-width: 640px) {
    grid-template-columns: 1fr;
  }
}

.form-field {
  display: flex;
  flex-direction: column;
  gap: $spacing-2;

  &.full-width {
    grid-column: 1 / -1;
  }

  label {
    font-size: $text-sm;
    font-weight: $font-weight-medium;
    color: $slate-700;
  }

  :deep(.p-inputtext),
  :deep(.p-dropdown),
  :deep(.p-textarea),
  :deep(.p-password) {
    width: 100%;
  }
}

// Skills Grid
.skills-grid {
  display: flex;
  flex-wrap: wrap;
  gap: $spacing-2;
}

.skill-tag {
  padding: $spacing-2 $spacing-4;
  background: $intalio-teal-50;
  border-radius: $radius-full;
  font-size: $text-sm;
  font-weight: $font-weight-medium;
  color: $intalio-teal-700;
}

.add-skill-btn {
  display: inline-flex;
  align-items: center;
  gap: $spacing-2;
  padding: $spacing-2 $spacing-4;
  background: none;
  border: 2px dashed $slate-300;
  border-radius: $radius-full;
  font-size: $text-sm;
  color: $slate-500;
  cursor: pointer;
  transition: all $transition-fast;

  &:hover {
    border-color: $intalio-teal-500;
    color: $intalio-teal-600;
  }
}

// Form Actions
.form-actions {
  display: flex;
  justify-content: flex-end;
  margin-top: $spacing-4;
}

// Sessions List
.sessions-list {
  display: flex;
  flex-direction: column;
  gap: $spacing-3;
}

.session-item {
  display: flex;
  align-items: center;
  gap: $spacing-4;
  padding: $spacing-4;
  background: $slate-50;
  border-radius: $radius-xl;
  border: 2px solid transparent;
  transition: all $transition-fast;

  &.current {
    background: rgba($intalio-teal-50, 0.5);
    border-color: $intalio-teal-200;
  }

  .session-icon {
    width: 44px;
    height: 44px;
    background: #fff;
    border-radius: $radius-lg;
    display: flex;
    align-items: center;
    justify-content: center;

    i {
      font-size: $text-xl;
      color: $slate-600;
    }
  }

  .session-info {
    flex: 1;

    .session-device {
      display: block;
      font-weight: $font-weight-medium;
      color: $slate-900;
    }

    .session-meta {
      display: flex;
      align-items: center;
      gap: $spacing-2;
      font-size: $text-sm;
      color: $slate-500;

      .current-badge {
        padding: 2px $spacing-2;
        background: $intalio-teal-100;
        border-radius: $radius-md;
        font-size: $text-xs;
        font-weight: $font-weight-medium;
        color: $intalio-teal-700;
      }
    }
  }
}

// Preferences List
.preferences-list {
  display: flex;
  flex-direction: column;
  gap: $spacing-3;
}

.preference-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: $spacing-4;
  background: $slate-50;
  border-radius: $radius-xl;

  .preference-info {
    .preference-title {
      display: block;
      font-weight: $font-weight-medium;
      color: $slate-900;
    }

    .preference-desc {
      display: block;
      font-size: $text-sm;
      color: $slate-500;
      margin-top: 2px;
    }
  }
}

.language-dropdown {
  max-width: 300px;
}

// Activity Timeline
.activity-timeline {
  display: flex;
  flex-direction: column;
  gap: $spacing-4;
}

.activity-item {
  display: flex;
  gap: $spacing-4;
  padding: $spacing-4;
  background: $slate-50;
  border-radius: $radius-xl;

  @include card-item-animation;

  .activity-icon {
    width: 44px;
    height: 44px;
    border-radius: $radius-lg;
    display: flex;
    align-items: center;
    justify-content: center;
    flex-shrink: 0;

    i {
      font-size: $text-lg;
    }
  }

  .activity-content {
    flex: 1;

    .activity-text {
      margin: 0;
      color: $slate-700;
      line-height: 1.5;

      strong {
        color: $slate-900;
      }
    }

    .activity-time {
      font-size: $text-xs;
      color: $slate-500;
    }
  }
}

// Account Info
.account-info {
  display: flex;
  flex-direction: column;
  gap: $spacing-3;

  .info-row {
    display: flex;
    justify-content: space-between;
    padding: $spacing-3;
    background: $slate-50;
    border-radius: $radius-lg;

    .info-label {
      color: $slate-600;
      font-size: $text-sm;
    }

    .info-value {
      font-weight: $font-weight-medium;
      color: $slate-900;
    }
  }
}

// ============================================
// DIALOGS
// ============================================

.password-dialog,
.avatar-dialog {
  :deep(.p-dialog-content) {
    padding: $spacing-5;
  }
}

.password-form {
  display: flex;
  flex-direction: column;
  gap: $spacing-4;
}

.avatar-upload-area {
  .upload-zone {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    padding: $spacing-10;
    border: 2px dashed $slate-300;
    border-radius: $radius-xl;
    background: $slate-50;
    text-align: center;
    cursor: pointer;
    transition: all $transition-fast;

    &:hover {
      border-color: $intalio-teal-500;
      background: rgba($intalio-teal-50, 0.5);
    }

    i {
      font-size: 48px;
      color: $slate-400;
      margin-bottom: $spacing-3;
    }

    p {
      margin: 0;
      color: $slate-700;
      font-weight: $font-weight-medium;
    }

    span {
      font-size: $text-sm;
      color: $slate-500;
      margin-top: $spacing-1;
    }
  }
}

// ============================================
// ANIMATIONS
// ============================================

@include staggered-animation-delays('.activity-item', 10, 0.05s);

// ============================================
// RTL SUPPORT
// ============================================

.rtl {
  direction: rtl;

  .breadcrumb-separator {
    transform: rotate(180deg);
  }
}
</style>
