<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useI18n } from 'vue-i18n'
import { useReducedMotion } from '@/composables/useReducedMotion'
import { Avatar } from '@/components/base'
import Button from 'primevue/button'
import InputText from 'primevue/inputtext'
import AvatarGroup from 'primevue/avatargroup'
import Skeleton from 'primevue/skeleton'
import Dialog from 'primevue/dialog'
import Textarea from 'primevue/textarea'
import Dropdown from 'primevue/dropdown'

// Base Components
import ErrorState from '@/components/base/ErrorState.vue'

const route = useRoute()
const router = useRouter()
const { locale } = useI18n()
const prefersReducedMotion = useReducedMotion()

const isRTL = computed(() => locale.value === 'ar')
const loading = ref(true)
const error = ref<Error | null>(null)
const isContentVisible = ref(false)
const shouldAnimate = computed(() => !prefersReducedMotion.value)
const activeTab = ref<'discussions' | 'members' | 'files' | 'about'>('discussions')
const showNewDiscussionDialog = ref(false)
const discussionSearch = ref('')

// Discussion type configuration
const DISCUSSION_TYPE_CONFIG: Record<string, { icon: string; color: string; bgColor: string; label: string; labelAr: string }> = {
  Discussion: { icon: 'pi pi-comments', color: '#3b82f6', bgColor: 'rgba(59, 130, 246, 0.1)', label: 'Discussion', labelAr: 'نقاش' },
  Question: { icon: 'pi pi-question-circle', color: '#f59e0b', bgColor: 'rgba(245, 158, 11, 0.1)', label: 'Question', labelAr: 'سؤال' },
  Announcement: { icon: 'pi pi-megaphone', color: '#ef4444', bgColor: 'rgba(239, 68, 68, 0.1)', label: 'Announcement', labelAr: 'إعلان' },
  Poll: { icon: 'pi pi-chart-bar', color: '#10b981', bgColor: 'rgba(16, 185, 129, 0.1)', label: 'Poll', labelAr: 'استطلاع' },
  Idea: { icon: 'pi pi-lightbulb', color: '#8b5cf6', bgColor: 'rgba(139, 92, 246, 0.1)', label: 'Idea', labelAr: 'فكرة' }
}

// Member role configuration
const ROLE_CONFIG: Record<string, { color: string; bgColor: string; label: string; labelAr: string }> = {
  Owner: { color: '#dc2626', bgColor: 'rgba(220, 38, 38, 0.1)', label: 'Owner', labelAr: 'مالك' },
  Moderator: { color: '#f59e0b', bgColor: 'rgba(245, 158, 11, 0.1)', label: 'Moderator', labelAr: 'مشرف' },
  Member: { color: '#3b82f6', bgColor: 'rgba(59, 130, 246, 0.1)', label: 'Member', labelAr: 'عضو' }
}

// Community type configuration
const TYPE_CONFIG: Record<string, { icon: string; color: string; bgColor: string; label: string; labelAr: string }> = {
  General: { icon: 'pi pi-globe', color: '#3b82f6', bgColor: 'rgba(59, 130, 246, 0.1)', label: 'General', labelAr: 'عام' },
  Project: { icon: 'pi pi-briefcase', color: '#10b981', bgColor: 'rgba(16, 185, 129, 0.1)', label: 'Project', labelAr: 'مشروع' },
  Department: { icon: 'pi pi-building', color: '#f59e0b', bgColor: 'rgba(245, 158, 11, 0.1)', label: 'Department', labelAr: 'قسم' },
  WorkingGroup: { icon: 'pi pi-users', color: '#8b5cf6', bgColor: 'rgba(139, 92, 246, 0.1)', label: 'Working Group', labelAr: 'فريق عمل' }
}

const VISIBILITY_CONFIG: Record<string, { icon: string; label: string; labelAr: string }> = {
  Public: { icon: 'pi pi-globe', label: 'Public', labelAr: 'عام' },
  Private: { icon: 'pi pi-lock', label: 'Private', labelAr: 'خاص' },
  Secret: { icon: 'pi pi-eye-slash', label: 'Secret', labelAr: 'سري' }
}

interface Discussion {
  id: string
  title: string
  titleArabic?: string
  contentPreview: string
  type: string
  authorName: string
  authorAvatarUrl?: string
  viewCount: number
  replyCount: number
  likeCount: number
  isPinned: boolean
  createdAt: Date
}

interface Member {
  userId: string
  displayName: string
  displayNameArabic?: string
  jobTitle: string
  jobTitleArabic?: string
  avatarUrl?: string
  role: 'Owner' | 'Moderator' | 'Member'
  joinedAt: Date
}

interface Community {
  id: string
  name: string
  nameArabic?: string
  description: string
  descriptionArabic?: string
  type: string
  visibility: string
  avatarUrl?: string
  coverUrl?: string
  memberCount: number
  discussionCount: number
  fileCount: number
  isMember: boolean
  isFollowing: boolean
  guidelines?: string
  guidelinesArabic?: string
  lastActivityAt: Date
  createdAt: Date
}

const community = ref<Community>({
  id: '',
  name: '',
  description: '',
  type: 'General',
  visibility: 'Public',
  memberCount: 0,
  discussionCount: 0,
  fileCount: 0,
  isMember: false,
  isFollowing: false,
  lastActivityAt: new Date(),
  createdAt: new Date()
})

const discussions = ref<Discussion[]>([])
const members = ref<Member[]>([])

const newDiscussion = ref({
  type: 'Discussion',
  title: '',
  content: ''
})

const discussionTypes = computed(() => [
  { label: isRTL.value ? 'نقاش' : 'Discussion', value: 'Discussion' },
  { label: isRTL.value ? 'سؤال' : 'Question', value: 'Question' },
  { label: isRTL.value ? 'إعلان' : 'Announcement', value: 'Announcement' },
  { label: isRTL.value ? 'استطلاع' : 'Poll', value: 'Poll' },
  { label: isRTL.value ? 'فكرة' : 'Idea', value: 'Idea' }
])

// Tab options
const tabOptions = computed(() => [
  { value: 'discussions', label: isRTL.value ? 'النقاشات' : 'Discussions', icon: 'pi-comments', count: community.value.discussionCount },
  { value: 'members', label: isRTL.value ? 'الأعضاء' : 'Members', icon: 'pi-users', count: community.value.memberCount },
  { value: 'files', label: isRTL.value ? 'الملفات' : 'Files', icon: 'pi-folder', count: community.value.fileCount },
  { value: 'about', label: isRTL.value ? 'حول' : 'About', icon: 'pi-info-circle' }
])

// Computed
const communityLeaders = computed(() => members.value.filter(m => m.role === 'Owner' || m.role === 'Moderator'))

const filteredDiscussions = computed(() => {
  if (!discussionSearch.value) return discussions.value
  const query = discussionSearch.value.toLowerCase()
  return discussions.value.filter(d =>
    d.title.toLowerCase().includes(query) ||
    d.contentPreview.toLowerCase().includes(query) ||
    d.authorName.toLowerCase().includes(query)
  )
})

// Helpers
const getName = (item: { name?: string; nameArabic?: string; displayName?: string; displayNameArabic?: string }) => {
  if (isRTL.value) {
    return item.nameArabic || item.displayNameArabic || item.name || item.displayName || ''
  }
  return item.name || item.displayName || ''
}

const getDescription = (item: { description?: string; descriptionArabic?: string }) => {
  return isRTL.value && item.descriptionArabic ? item.descriptionArabic : item.description
}

const getTypeConfig = (type: string) => TYPE_CONFIG[type] || TYPE_CONFIG.General
const getVisibilityConfig = (visibility: string) => VISIBILITY_CONFIG[visibility] || VISIBILITY_CONFIG.Public
const getDiscussionTypeConfig = (type: string) => DISCUSSION_TYPE_CONFIG[type] || DISCUSSION_TYPE_CONFIG.Discussion
const getRoleConfig = (role: string) => ROLE_CONFIG[role] || ROLE_CONFIG.Member

const formatDate = (date: Date) => {
  return new Intl.DateTimeFormat(isRTL.value ? 'ar-SA' : 'en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  }).format(new Date(date))
}

const formatRelativeTime = (date: Date) => {
  const now = new Date()
  const diff = now.getTime() - new Date(date).getTime()
  const days = Math.floor(diff / (1000 * 60 * 60 * 24))
  const hours = Math.floor(diff / (1000 * 60 * 60))
  const minutes = Math.floor(diff / (1000 * 60))

  if (days > 0) {
    return isRTL.value ? `منذ ${days} يوم` : `${days}d ago`
  } else if (hours > 0) {
    return isRTL.value ? `منذ ${hours} ساعة` : `${hours}h ago`
  } else {
    return isRTL.value ? `منذ ${minutes} دقيقة` : `${minutes}m ago`
  }
}

// Actions
const navigateBack = () => {
  router.push('/collaboration/communities')
}

const joinCommunity = () => {
  community.value.isMember = true
  community.value.memberCount++
}

const leaveCommunity = () => {
  community.value.isMember = false
  community.value.memberCount--
}

const toggleFollow = () => {
  community.value.isFollowing = !community.value.isFollowing
}

const goToDiscussion = (id: string) => {
  router.push({ name: 'discussion-detail', params: { id } })
}

const createDiscussion = () => {
  showNewDiscussionDialog.value = false
  newDiscussion.value = { type: 'Discussion', title: '', content: '' }
}

// Load community data
const LOADING_DELAY = 600

async function loadCommunity() {
  try {
    error.value = null
    loading.value = true

    await new Promise(resolve => setTimeout(resolve, LOADING_DELAY))

    community.value = {
      id: route.params.slug as string,
      name: 'Stadium Operations',
      nameArabic: 'عمليات الملاعب',
      description: 'Coordination hub for all stadium operations teams across all venues. Collaborate on venue management, event coordination, and operational excellence.',
      descriptionArabic: 'مركز التنسيق لجميع فرق عمليات الملاعب في جميع الأماكن. التعاون في إدارة المكان وتنسيق الفعاليات والتميز التشغيلي.',
      type: 'WorkingGroup',
      visibility: 'Private',
      memberCount: 156,
      discussionCount: 42,
      fileCount: 28,
      isMember: true,
      isFollowing: true,
      guidelines: 'Be respectful, stay on topic, and share knowledge constructively. All discussions should relate to stadium operations and event management.',
      guidelinesArabic: 'كن محترماً، والتزم بالموضوع، وشارك المعرفة بشكل بناء. يجب أن تتعلق جميع النقاشات بعمليات الملاعب وإدارة الفعاليات.',
      lastActivityAt: new Date(Date.now() - 1000 * 60 * 30),
      createdAt: new Date(Date.now() - 90 * 24 * 60 * 60 * 1000)
    }

    discussions.value = [
      {
        id: '1',
        title: 'Stadium Security Protocols Update',
        titleArabic: 'تحديث بروتوكولات أمن الملاعب',
        contentPreview: 'We need to review and update our security protocols ahead of the group stage matches. Key areas to address include entry screening, crowd management, and emergency response procedures.',
        type: 'Announcement',
        authorName: 'Mohammed Al-Rashid',
        authorAvatarUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Mohammed',
        viewCount: 234,
        replyCount: 18,
        likeCount: 45,
        isPinned: true,
        createdAt: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000)
      },
      {
        id: '2',
        title: 'Best practices for crowd management?',
        titleArabic: 'أفضل الممارسات لإدارة الحشود؟',
        contentPreview: 'Looking for advice on managing large crowds during peak entry times. What strategies have worked well for others?',
        type: 'Question',
        authorName: 'Sara Ali',
        authorAvatarUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Sara',
        viewCount: 156,
        replyCount: 12,
        likeCount: 28,
        isPinned: false,
        createdAt: new Date(Date.now() - 5 * 24 * 60 * 60 * 1000)
      },
      {
        id: '3',
        title: 'New Venue Layout Proposal',
        titleArabic: 'مقترح تخطيط مكان جديد',
        contentPreview: 'I have an idea for improving the fan zone layout that could reduce congestion and improve the overall experience.',
        type: 'Idea',
        authorName: 'Ahmed Hassan',
        authorAvatarUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Ahmed',
        viewCount: 89,
        replyCount: 7,
        likeCount: 15,
        isPinned: false,
        createdAt: new Date(Date.now() - 7 * 24 * 60 * 60 * 1000)
      },
      {
        id: '4',
        title: 'Weekly Operations Meeting Notes',
        titleArabic: 'ملاحظات اجتماع العمليات الأسبوعي',
        contentPreview: 'Summary of our weekly operations meeting covering venue readiness, staffing updates, and upcoming milestones.',
        type: 'Discussion',
        authorName: 'Fatima Khan',
        authorAvatarUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Fatima',
        viewCount: 178,
        replyCount: 23,
        likeCount: 34,
        isPinned: false,
        createdAt: new Date(Date.now() - 10 * 24 * 60 * 60 * 1000)
      }
    ]

    members.value = [
      { userId: '1', displayName: 'Mohammed Al-Rashid', displayNameArabic: 'محمد الراشد', jobTitle: 'Operations Director', jobTitleArabic: 'مدير العمليات', avatarUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Mohammed', role: 'Owner', joinedAt: new Date(Date.now() - 90 * 24 * 60 * 60 * 1000) },
      { userId: '2', displayName: 'Sara Ali', displayNameArabic: 'سارة علي', jobTitle: 'Project Manager', jobTitleArabic: 'مديرة المشاريع', avatarUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Sara', role: 'Moderator', joinedAt: new Date(Date.now() - 85 * 24 * 60 * 60 * 1000) },
      { userId: '3', displayName: 'Ahmed Hassan', displayNameArabic: 'أحمد حسن', jobTitle: 'Coordinator', jobTitleArabic: 'منسق', avatarUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Ahmed', role: 'Member', joinedAt: new Date(Date.now() - 60 * 24 * 60 * 60 * 1000) },
      { userId: '4', displayName: 'Fatima Khan', displayNameArabic: 'فاطمة خان', jobTitle: 'Safety Officer', jobTitleArabic: 'مسؤولة السلامة', avatarUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Fatima', role: 'Member', joinedAt: new Date(Date.now() - 45 * 24 * 60 * 60 * 1000) },
      { userId: '5', displayName: 'Omar Ibrahim', displayNameArabic: 'عمر إبراهيم', jobTitle: 'Venue Manager', jobTitleArabic: 'مدير المكان', avatarUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Omar', role: 'Moderator', joinedAt: new Date(Date.now() - 30 * 24 * 60 * 60 * 1000) },
      { userId: '6', displayName: 'Layla Ahmad', displayNameArabic: 'ليلى أحمد', jobTitle: 'Events Coordinator', jobTitleArabic: 'منسقة الفعاليات', avatarUrl: 'https://api.dicebear.com/7.x/avataaars/svg?seed=Layla', role: 'Member', joinedAt: new Date(Date.now() - 20 * 24 * 60 * 60 * 1000) }
    ]

    loading.value = false

    // Trigger animations
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
    console.error('Failed to load community:', e)
  }
}

async function handleRetry() {
  await loadCommunity()
}

onMounted(() => {
  loadCommunity()
})
</script>

<template>
  <div class="community-detail-view" :class="{ rtl: isRTL }">
    <!-- Loading State -->
    <div v-if="loading" class="loading-state">
      <div class="loading-header">
        <Skeleton height="200px" />
      </div>
      <div class="loading-content">
        <Skeleton width="60%" height="30px" class="mb-4" />
        <Skeleton width="100%" height="60px" class="mb-4" />
        <Skeleton width="40%" height="20px" />
      </div>
    </div>

    <!-- Error State -->
    <ErrorState
      v-else-if="error"
      :error="error"
      :title="isRTL ? 'فشل تحميل المجتمع' : 'Failed to load community'"
      show-retry
      @retry="handleRetry"
    />

    <template v-else>
      <!-- Community Header -->
      <header class="community-header">
        <div class="header-background">
          <div class="bg-gradient"></div>
          <div class="bg-pattern"></div>
        </div>

        <div class="header-content">
          <!-- Breadcrumb -->
          <nav class="breadcrumb">
            <a href="#" class="breadcrumb-link" @click.prevent="navigateBack">
              <i class="pi pi-arrow-left"></i>
              <span>{{ isRTL ? 'المجتمعات' : 'Communities' }}</span>
            </a>
          </nav>

          <div class="header-main">
            <div class="community-icon" :style="{ background: getTypeConfig(community.type).bgColor, color: getTypeConfig(community.type).color }">
              <i :class="getTypeConfig(community.type).icon"></i>
            </div>

            <div class="header-info">
              <div class="header-badges">
                <span class="type-badge" :style="{ background: getTypeConfig(community.type).bgColor, color: getTypeConfig(community.type).color }">
                  <i :class="getTypeConfig(community.type).icon"></i>
                  {{ isRTL ? getTypeConfig(community.type).labelAr : getTypeConfig(community.type).label }}
                </span>
                <span class="visibility-badge">
                  <i :class="getVisibilityConfig(community.visibility).icon"></i>
                  {{ isRTL ? getVisibilityConfig(community.visibility).labelAr : getVisibilityConfig(community.visibility).label }}
                </span>
              </div>

              <h1>{{ getName(community) }}</h1>
              <p class="community-description">{{ getDescription(community) }}</p>

              <div class="community-stats">
                <span class="stat">
                  <i class="pi pi-users"></i>
                  <strong>{{ community.memberCount.toLocaleString() }}</strong>
                  {{ isRTL ? 'عضو' : 'members' }}
                </span>
                <span class="stat">
                  <i class="pi pi-comments"></i>
                  <strong>{{ community.discussionCount }}</strong>
                  {{ isRTL ? 'نقاش' : 'discussions' }}
                </span>
                <span class="stat">
                  <i class="pi pi-clock"></i>
                  {{ isRTL ? 'آخر نشاط' : 'Last active' }} {{ formatRelativeTime(community.lastActivityAt) }}
                </span>
              </div>
            </div>

            <div class="header-actions">
              <Button
                v-if="!community.isMember"
                :label="isRTL ? 'انضمام' : 'Join Community'"
                icon="pi pi-user-plus"
                class="btn-join"
                @click="joinCommunity"
              />
              <template v-else>
                <Button
                  :label="community.isFollowing ? (isRTL ? 'متابَع' : 'Following') : (isRTL ? 'متابعة' : 'Follow')"
                  :icon="community.isFollowing ? 'pi pi-check' : 'pi pi-bell'"
                  :class="['btn-follow', { active: community.isFollowing }]"
                  @click="toggleFollow"
                />
                <Button
                  :label="isRTL ? 'مغادرة' : 'Leave'"
                  icon="pi pi-sign-out"
                  class="btn-leave"
                  severity="secondary"
                  text
                  @click="leaveCommunity"
                />
              </template>
            </div>
          </div>

          <!-- Members Preview -->
          <div v-if="communityLeaders.length" class="leaders-preview">
            <span class="leaders-label">{{ isRTL ? 'القادة:' : 'Leaders:' }}</span>
            <AvatarGroup>
              <Avatar
                v-for="leader in communityLeaders.slice(0, 3)"
                :key="leader.userId"
                :image="leader.avatarUrl"
                :name="leader.displayName"
                shape="circle"
                size="sm"
              />
            </AvatarGroup>
            <span class="leaders-names">
              {{ communityLeaders.slice(0, 2).map(l => getName(l)).join(', ') }}
              <template v-if="communityLeaders.length > 2">
                {{ isRTL ? `و ${communityLeaders.length - 2} آخرين` : `and ${communityLeaders.length - 2} more` }}
              </template>
            </span>
          </div>
        </div>
      </header>

      <!-- Tab Navigation -->
      <nav class="tabs-nav">
        <button
          v-for="tab in tabOptions"
          :key="tab.value"
          :class="['tab-btn', { active: activeTab === tab.value }]"
          @click="activeTab = tab.value as typeof activeTab"
        >
          <i :class="['pi', tab.icon]"></i>
          <span>{{ tab.label }}</span>
          <span v-if="tab.count !== undefined" class="tab-count">{{ tab.count }}</span>
        </button>
      </nav>

      <!-- Tab Content -->
      <main class="tab-content">
        <!-- Discussions Tab -->
        <div v-if="activeTab === 'discussions'" class="discussions-tab">
          <div class="tab-toolbar">
            <div class="search-box">
              <i class="pi pi-search"></i>
              <InputText
                v-model="discussionSearch"
                :placeholder="isRTL ? 'البحث في النقاشات...' : 'Search discussions...'"
                class="search-input"
              />
            </div>
            <Button
              :label="isRTL ? 'نقاش جديد' : 'New Discussion'"
              icon="pi pi-plus"
              class="btn-new-discussion"
              @click="showNewDiscussionDialog = true"
            />
          </div>

          <!-- Discussions List -->
          <div v-if="filteredDiscussions.length > 0" class="discussions-list">
            <div
              v-for="discussion in filteredDiscussions"
              :key="discussion.id"
              class="discussion-card"
              :class="{ pinned: discussion.isPinned }"
              @click="goToDiscussion(discussion.id)"
            >
              <div class="discussion-left">
                <Avatar
                  :image="discussion.authorAvatarUrl"
                  :name="discussion.authorName"
                  shape="circle"
                  size="lg"
                />
              </div>

              <div class="discussion-body">
                <div class="discussion-header">
                  <span v-if="discussion.isPinned" class="pinned-badge">
                    <i class="pi pi-thumbtack"></i>
                    {{ isRTL ? 'مثبت' : 'Pinned' }}
                  </span>
                  <span
                    class="type-badge"
                    :style="{ background: getDiscussionTypeConfig(discussion.type).bgColor, color: getDiscussionTypeConfig(discussion.type).color }"
                  >
                    <i :class="getDiscussionTypeConfig(discussion.type).icon"></i>
                    {{ isRTL ? getDiscussionTypeConfig(discussion.type).labelAr : getDiscussionTypeConfig(discussion.type).label }}
                  </span>
                </div>

                <h3 class="discussion-title">{{ discussion.title }}</h3>
                <p class="discussion-preview">{{ discussion.contentPreview }}</p>

                <div class="discussion-meta">
                  <span class="author">{{ discussion.authorName }}</span>
                  <span class="separator">•</span>
                  <span class="time">{{ formatRelativeTime(discussion.createdAt) }}</span>
                </div>
              </div>

              <div class="discussion-stats">
                <span class="stat" :title="isRTL ? 'المشاهدات' : 'Views'">
                  <i class="pi pi-eye"></i>
                  {{ discussion.viewCount }}
                </span>
                <span class="stat" :title="isRTL ? 'الردود' : 'Replies'">
                  <i class="pi pi-comment"></i>
                  {{ discussion.replyCount }}
                </span>
                <span class="stat" :title="isRTL ? 'الإعجابات' : 'Likes'">
                  <i class="pi pi-heart"></i>
                  {{ discussion.likeCount }}
                </span>
              </div>
            </div>
          </div>

          <!-- Empty State -->
          <div v-else class="empty-state">
            <div class="empty-icon">
              <i class="pi pi-comments"></i>
            </div>
            <h3>{{ isRTL ? 'لا توجد نقاشات' : 'No Discussions Yet' }}</h3>
            <p>{{ isRTL ? 'كن أول من يبدأ المحادثة!' : 'Be the first to start a conversation!' }}</p>
            <Button
              :label="isRTL ? 'بدء نقاش' : 'Start Discussion'"
              icon="pi pi-plus"
              @click="showNewDiscussionDialog = true"
            />
          </div>
        </div>

        <!-- Members Tab -->
        <div v-if="activeTab === 'members'" class="members-tab">
          <div class="members-header">
            <span class="members-count">{{ members.length }} {{ isRTL ? 'عضو' : 'members' }}</span>
            <Button
              :label="isRTL ? 'عرض جميع الأعضاء' : 'View All Team Members'"
              icon="pi pi-users"
              text
              size="small"
              class="view-all-btn"
              @click="router.push('/team')"
            />
          </div>
          <div class="members-grid">
            <div
              v-for="member in members"
              :key="member.userId"
              class="member-card"
              @click="router.push('/team')"
              role="button"
            >
              <Avatar
                :image="member.avatarUrl"
                :name="member.displayName"
                size="xl"
                shape="circle"
              />
              <div class="member-info">
                <h4 class="member-name">{{ getName(member) }}</h4>
                <p class="member-title">{{ isRTL && member.jobTitleArabic ? member.jobTitleArabic : member.jobTitle }}</p>
                <span
                  class="role-badge"
                  :style="{ background: getRoleConfig(member.role).bgColor, color: getRoleConfig(member.role).color }"
                >
                  {{ isRTL ? getRoleConfig(member.role).labelAr : getRoleConfig(member.role).label }}
                </span>
              </div>
              <p class="member-joined">
                {{ isRTL ? 'انضم' : 'Joined' }} {{ formatDate(member.joinedAt) }}
              </p>
            </div>
          </div>
        </div>

        <!-- Files Tab -->
        <div v-if="activeTab === 'files'" class="files-tab">
          <div class="empty-state">
            <div class="empty-icon">
              <i class="pi pi-folder"></i>
            </div>
            <h3>{{ isRTL ? 'لا توجد ملفات' : 'No Files Yet' }}</h3>
            <p>{{ isRTL ? 'شارك الملفات مع أعضاء المجتمع' : 'Share files with community members' }}</p>
            <Button
              :label="isRTL ? 'رفع ملف' : 'Upload File'"
              icon="pi pi-upload"
            />
          </div>
        </div>

        <!-- About Tab -->
        <div v-if="activeTab === 'about'" class="about-tab">
          <div class="about-grid">
            <div class="about-main">
              <section class="about-section">
                <h3>{{ isRTL ? 'الوصف' : 'Description' }}</h3>
                <p>{{ getDescription(community) }}</p>
              </section>

              <section v-if="community.guidelines" class="about-section">
                <h3>{{ isRTL ? 'إرشادات المجتمع' : 'Community Guidelines' }}</h3>
                <p>{{ isRTL && community.guidelinesArabic ? community.guidelinesArabic : community.guidelines }}</p>
              </section>

              <section class="about-section">
                <h3>{{ isRTL ? 'قادة المجتمع' : 'Community Leaders' }}</h3>
                <div class="leaders-list">
                  <div v-for="leader in communityLeaders" :key="leader.userId" class="leader-card">
                    <Avatar
                      :image="leader.avatarUrl"
                      :name="leader.displayName"
                      size="lg"
                      shape="circle"
                    />
                    <div class="leader-info">
                      <h4>{{ getName(leader) }}</h4>
                      <p>{{ isRTL && leader.jobTitleArabic ? leader.jobTitleArabic : leader.jobTitle }}</p>
                      <span
                        class="role-badge"
                        :style="{ background: getRoleConfig(leader.role).bgColor, color: getRoleConfig(leader.role).color }"
                      >
                        {{ isRTL ? getRoleConfig(leader.role).labelAr : getRoleConfig(leader.role).label }}
                      </span>
                    </div>
                  </div>
                </div>
              </section>
            </div>

            <aside class="about-sidebar">
              <div class="details-card">
                <h4>{{ isRTL ? 'التفاصيل' : 'Details' }}</h4>
                <div class="detail-row">
                  <span class="detail-label">{{ isRTL ? 'النوع' : 'Type' }}</span>
                  <span class="detail-value">
                    <i :class="getTypeConfig(community.type).icon" :style="{ color: getTypeConfig(community.type).color }"></i>
                    {{ isRTL ? getTypeConfig(community.type).labelAr : getTypeConfig(community.type).label }}
                  </span>
                </div>
                <div class="detail-row">
                  <span class="detail-label">{{ isRTL ? 'الرؤية' : 'Visibility' }}</span>
                  <span class="detail-value">
                    <i :class="getVisibilityConfig(community.visibility).icon"></i>
                    {{ isRTL ? getVisibilityConfig(community.visibility).labelAr : getVisibilityConfig(community.visibility).label }}
                  </span>
                </div>
                <div class="detail-row">
                  <span class="detail-label">{{ isRTL ? 'تاريخ الإنشاء' : 'Created' }}</span>
                  <span class="detail-value">{{ formatDate(community.createdAt) }}</span>
                </div>
                <div class="detail-row">
                  <span class="detail-label">{{ isRTL ? 'آخر نشاط' : 'Last Activity' }}</span>
                  <span class="detail-value">{{ formatRelativeTime(community.lastActivityAt) }}</span>
                </div>
              </div>
            </aside>
          </div>
        </div>
      </main>
    </template>

    <!-- New Discussion Dialog -->
    <Dialog
      v-model:visible="showNewDiscussionDialog"
      :header="isRTL ? 'نقاش جديد' : 'New Discussion'"
      :style="{ width: '600px' }"
      modal
      class="discussion-dialog"
    >
      <div class="discussion-form">
        <div class="form-field">
          <label>{{ isRTL ? 'نوع النقاش' : 'Discussion Type' }}</label>
          <Dropdown
            v-model="newDiscussion.type"
            :options="discussionTypes"
            optionLabel="label"
            optionValue="value"
            class="w-full"
          />
        </div>
        <div class="form-field">
          <label>{{ isRTL ? 'العنوان' : 'Title' }}</label>
          <InputText v-model="newDiscussion.title" class="w-full" />
        </div>
        <div class="form-field">
          <label>{{ isRTL ? 'المحتوى' : 'Content' }}</label>
          <Textarea v-model="newDiscussion.content" rows="6" class="w-full" />
        </div>
      </div>
      <template #footer>
        <Button
          :label="isRTL ? 'إلغاء' : 'Cancel'"
          severity="secondary"
          text
          @click="showNewDiscussionDialog = false"
        />
        <Button
          :label="isRTL ? 'نشر' : 'Post'"
          icon="pi pi-send"
          @click="createDiscussion"
        />
      </template>
    </Dialog>
  </div>
</template>

<style lang="scss" scoped>
@use '@/assets/styles/_variables.scss' as *;
@use '@/assets/styles/_mixins.scss' as *;

.community-detail-view {
  min-height: 100vh;
  animation: fadeIn 0.3s ease-out;

  &.rtl {
    direction: rtl;
  }
}

// Loading State
.loading-state {
  padding: $spacing-6;

  .loading-header {
    margin: (-$spacing-6) 0 $spacing-6;
  }

  .loading-content {
    max-width: 800px;
  }
}

// ============================================
// COMMUNITY HEADER
// ============================================

.community-header {
  position: relative;
  margin: (-$spacing-6) 0 $spacing-6;
  padding: $spacing-4 $spacing-6 $spacing-8;
  border-radius: 0 0 $radius-2xl $radius-2xl;
  overflow: hidden;

  .header-background {
    position: absolute;
    inset: 0;
    z-index: 0;

    .bg-gradient {
      position: absolute;
      inset: 0;
      background: $gradient-primary;
    }

    .bg-pattern {
      position: absolute;
      inset: 0;
      opacity: 0.1;
      background-image:
        radial-gradient(circle at 20% 50%, rgba(255, 255, 255, 0.3) 0%, transparent 50%),
        radial-gradient(circle at 80% 50%, rgba(255, 255, 255, 0.2) 0%, transparent 40%);
    }
  }

  .header-content {
    position: relative;
    z-index: 1;
    max-width: 1200px;
    margin: 0 auto;
  }
}

.breadcrumb {
  margin-bottom: $spacing-4;

  .breadcrumb-link {
    display: inline-flex;
    align-items: center;
    gap: $spacing-2;
    color: rgba(#fff, 0.8);
    font-size: $text-sm;
    text-decoration: none;
    transition: color $transition-fast;

    &:hover {
      color: #fff;
    }
  }
}

.header-main {
  display: flex;
  gap: $spacing-6;
  align-items: flex-start;
}

.community-icon {
  width: 80px;
  height: 80px;
  border-radius: $radius-xl;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 2rem;
  flex-shrink: 0;
  background: rgba(#fff, 0.2);
  backdrop-filter: blur(10px);
}

.header-info {
  flex: 1;

  .header-badges {
    display: flex;
    gap: $spacing-2;
    margin-bottom: $spacing-3;
  }

  .type-badge,
  .visibility-badge {
    display: inline-flex;
    align-items: center;
    gap: $spacing-1;
    padding: $spacing-1 $spacing-3;
    border-radius: $radius-full;
    font-size: $text-xs;
    font-weight: $font-weight-semibold;
  }

  .visibility-badge {
    background: rgba(#fff, 0.2);
    color: #fff;
  }

  h1 {
    margin: 0 0 $spacing-2;
    font-size: $text-3xl;
    font-weight: $font-weight-bold;
    color: #fff;
  }

  .community-description {
    margin: 0 0 $spacing-4;
    font-size: $text-base;
    color: rgba(#fff, 0.9);
    max-width: 600px;
    line-height: 1.6;
  }

  .community-stats {
    display: flex;
    gap: $spacing-6;

    .stat {
      display: flex;
      align-items: center;
      gap: $spacing-2;
      font-size: $text-sm;
      color: rgba(#fff, 0.85);

      i {
        font-size: 1rem;
      }

      strong {
        color: #fff;
      }
    }
  }
}

.header-actions {
  display: flex;
  gap: $spacing-2;
  flex-shrink: 0;

  .btn-join {
    background: #fff;
    color: $intalio-teal-600;
    border: none;
    font-weight: $font-weight-semibold;

    &:hover {
      background: rgba(#fff, 0.9);
    }
  }

  .btn-follow {
    background: rgba(#fff, 0.2);
    color: #fff;
    border: 1px solid rgba(#fff, 0.3);

    &:hover {
      background: rgba(#fff, 0.3);
    }

    &.active {
      background: #fff;
      color: $success-600;
      border-color: transparent;
    }
  }

  .btn-leave {
    color: rgba(#fff, 0.8);

    &:hover {
      color: #fff;
      background: rgba(#fff, 0.1);
    }
  }
}

.leaders-preview {
  display: flex;
  align-items: center;
  gap: $spacing-3;
  margin-top: $spacing-6;
  padding-top: $spacing-4;
  border-top: 1px solid rgba(#fff, 0.2);

  .leaders-label {
    font-size: $text-sm;
    color: rgba(#fff, 0.7);
  }

  .leaders-names {
    font-size: $text-sm;
    color: #fff;
  }

  :deep(.p-avatar) {
    border: 2px solid rgba(#fff, 0.3);
  }
}

// ============================================
// TABS NAVIGATION
// ============================================

.tabs-nav {
  display: flex;
  gap: $spacing-1;
  padding: 0 $spacing-6;
  margin-bottom: $spacing-6;
  border-bottom: 1px solid $slate-200;
}

.tab-btn {
  display: flex;
  align-items: center;
  gap: $spacing-2;
  padding: $spacing-3 $spacing-4;
  background: none;
  border: none;
  font-size: $text-sm;
  font-weight: $font-weight-medium;
  color: $slate-600;
  cursor: pointer;
  position: relative;
  transition: all $transition-fast;

  &:hover {
    color: $slate-900;
  }

  &.active {
    color: $intalio-teal-600;

    &::after {
      content: '';
      position: absolute;
      bottom: -1px;
      left: 0;
      right: 0;
      height: 2px;
      background: $intalio-teal-500;
      border-radius: $radius-full $radius-full 0 0;
    }
  }

  .tab-count {
    padding: 2px 6px;
    background: $slate-100;
    color: $slate-600;
    font-size: $text-xs;
    border-radius: $radius-full;
  }

  &.active .tab-count {
    background: $intalio-teal-50;
    color: $intalio-teal-600;
  }
}

// ============================================
// TAB CONTENT
// ============================================

.tab-content {
  padding: 0 $spacing-6 $spacing-6;
  max-width: 1200px;
  margin: 0 auto;
}

// Tab Toolbar
.tab-toolbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: $spacing-4;
  margin-bottom: $spacing-5;
}

.search-box {
  @include toolbar-search(300px);
}

.search-input {
  @include toolbar-search-input;
}

.btn-new-discussion {
  @include header-btn-primary;
}

// ============================================
// DISCUSSIONS TAB
// ============================================

.discussions-list {
  display: flex;
  flex-direction: column;
  gap: $spacing-4;
}

.discussion-card {
  display: flex;
  gap: $spacing-4;
  padding: $spacing-5;
  background: #fff;
  border-radius: $radius-xl;
  box-shadow: $shadow-card;
  cursor: pointer;
  transition: all $transition-base;
  border: 2px solid transparent;

  @include card-item-animation;

  &:hover {
    box-shadow: $shadow-card-hover;
    border-color: $intalio-teal-200;
    transform: translateY(-2px);
  }

  &.pinned {
    background: linear-gradient(to right, rgba($warning-50, 0.5), #fff);
    border-color: $warning-200;
  }
}

.discussion-left {
  flex-shrink: 0;
}

.discussion-body {
  flex: 1;
  min-width: 0;

  .discussion-header {
    display: flex;
    align-items: center;
    gap: $spacing-2;
    margin-bottom: $spacing-2;
  }

  .pinned-badge {
    display: inline-flex;
    align-items: center;
    gap: $spacing-1;
    padding: $spacing-1 $spacing-2;
    background: $warning-100;
    color: $warning-700;
    font-size: $text-xs;
    font-weight: $font-weight-semibold;
    border-radius: $radius-md;
  }

  .type-badge {
    display: inline-flex;
    align-items: center;
    gap: $spacing-1;
    padding: $spacing-1 $spacing-2;
    font-size: $text-xs;
    font-weight: $font-weight-medium;
    border-radius: $radius-md;
  }

  .discussion-title {
    margin: 0 0 $spacing-2;
    font-size: $text-lg;
    font-weight: $font-weight-semibold;
    color: $slate-900;
  }

  .discussion-preview {
    margin: 0 0 $spacing-3;
    font-size: $text-sm;
    color: $slate-600;
    line-height: 1.5;
    display: -webkit-box;
    -webkit-line-clamp: 2;
    -webkit-box-orient: vertical;
    overflow: hidden;
  }

  .discussion-meta {
    display: flex;
    align-items: center;
    gap: $spacing-2;
    font-size: $text-sm;
    color: $slate-500;

    .separator {
      color: $slate-300;
    }
  }
}

.discussion-stats {
  display: flex;
  flex-direction: column;
  gap: $spacing-2;
  flex-shrink: 0;

  .stat {
    display: flex;
    align-items: center;
    gap: $spacing-2;
    padding: $spacing-1 $spacing-2;
    font-size: $text-sm;
    color: $slate-500;
    background: $slate-50;
    border-radius: $radius-md;

    i {
      font-size: 0.875rem;
    }
  }
}

@include staggered-animation-delays('.discussion-card', 10, 0.05s);

// ============================================
// MEMBERS TAB
// ============================================

.members-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: $spacing-5;

  .members-count {
    font-size: $text-sm;
    font-weight: $font-weight-medium;
    color: $slate-600;
  }

  .view-all-btn {
    color: $intalio-teal-600;

    &:hover {
      background: $intalio-teal-50;
    }
  }
}

.members-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: $spacing-5;
}

.member-card {
  display: flex;
  flex-direction: column;
  align-items: center;
  text-align: center;
  padding: $spacing-6;
  background: #fff;
  border-radius: $radius-xl;
  box-shadow: $shadow-card;
  transition: all $transition-base;
  cursor: pointer;

  @include card-item-animation;

  &:hover {
    box-shadow: $shadow-card-hover;
    transform: translateY(-2px);
    border: 2px solid $intalio-teal-200;
  }

  .member-info {
    margin-top: $spacing-4;
  }

  .member-name {
    margin: 0 0 $spacing-1;
    font-size: $text-lg;
    font-weight: $font-weight-semibold;
    color: $slate-900;
  }

  .member-title {
    margin: 0 0 $spacing-3;
    font-size: $text-sm;
    color: $slate-500;
  }

  .role-badge {
    display: inline-block;
    padding: $spacing-1 $spacing-3;
    font-size: $text-xs;
    font-weight: $font-weight-semibold;
    border-radius: $radius-full;
  }

  .member-joined {
    margin-top: $spacing-4;
    padding-top: $spacing-4;
    border-top: 1px solid $slate-100;
    font-size: $text-xs;
    color: $slate-400;
    width: 100%;
  }
}

@include staggered-animation-delays('.member-card', 12, 0.05s);

// ============================================
// ABOUT TAB
// ============================================

.about-grid {
  display: grid;
  grid-template-columns: 1fr 300px;
  gap: $spacing-6;
}

.about-main {
  display: flex;
  flex-direction: column;
  gap: $spacing-6;
}

.about-section {
  padding: $spacing-5;
  background: #fff;
  border-radius: $radius-xl;
  box-shadow: $shadow-card;

  h3 {
    margin: 0 0 $spacing-4;
    font-size: $text-lg;
    font-weight: $font-weight-semibold;
    color: $slate-900;
  }

  p {
    margin: 0;
    font-size: $text-base;
    color: $slate-600;
    line-height: 1.7;
  }
}

.leaders-list {
  display: flex;
  flex-direction: column;
  gap: $spacing-4;
}

.leader-card {
  display: flex;
  align-items: center;
  gap: $spacing-4;
  padding: $spacing-4;
  background: $slate-50;
  border-radius: $radius-lg;

  .leader-info {
    h4 {
      margin: 0 0 $spacing-1;
      font-size: $text-base;
      font-weight: $font-weight-semibold;
      color: $slate-900;
    }

    p {
      margin: 0 0 $spacing-2;
      font-size: $text-sm;
      color: $slate-500;
    }

    .role-badge {
      display: inline-block;
      padding: $spacing-1 $spacing-2;
      font-size: $text-xs;
      font-weight: $font-weight-semibold;
      border-radius: $radius-md;
    }
  }
}

.about-sidebar {
  .details-card {
    padding: $spacing-5;
    background: #fff;
    border-radius: $radius-xl;
    box-shadow: $shadow-card;
    position: sticky;
    top: $spacing-6;

    h4 {
      margin: 0 0 $spacing-4;
      font-size: $text-base;
      font-weight: $font-weight-semibold;
      color: $slate-900;
    }
  }

  .detail-row {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: $spacing-3 0;
    border-bottom: 1px solid $slate-100;

    &:last-child {
      border-bottom: none;
    }

    .detail-label {
      font-size: $text-sm;
      color: $slate-500;
    }

    .detail-value {
      display: flex;
      align-items: center;
      gap: $spacing-2;
      font-size: $text-sm;
      font-weight: $font-weight-medium;
      color: $slate-900;

      i {
        font-size: 0.875rem;
      }
    }
  }
}

// ============================================
// EMPTY STATE
// ============================================

.empty-state {
  @include empty-state;
  padding: 4rem;
}

.empty-icon {
  @include empty-state-icon;
}

.empty-state h3 {
  @include empty-state-title;
}

.empty-state p {
  @include empty-state-text;
  margin-bottom: $spacing-4;
}

// ============================================
// DIALOG
// ============================================

.discussion-form {
  display: flex;
  flex-direction: column;
  gap: $spacing-4;

  .form-field {
    display: flex;
    flex-direction: column;
    gap: $spacing-2;

    label {
      font-size: $text-sm;
      font-weight: $font-weight-medium;
      color: $slate-700;
    }
  }
}

// ============================================
// RESPONSIVE
// ============================================

@media (max-width: $breakpoint-lg) {
  .about-grid {
    grid-template-columns: 1fr;
  }

  .about-sidebar {
    order: -1;

    .details-card {
      position: static;
    }
  }
}

@media (max-width: $breakpoint-md) {
  .community-header {
    margin: (-$spacing-4) 0 $spacing-4;
    padding: $spacing-3 $spacing-4 $spacing-6;
    border-radius: 0 0 $radius-xl $radius-xl;
  }

  .header-main {
    flex-direction: column;
    gap: $spacing-4;
  }

  .community-icon {
    width: 64px;
    height: 64px;
    font-size: 1.5rem;
  }

  .header-info {
    h1 {
      font-size: $text-2xl;
    }

    .community-stats {
      flex-wrap: wrap;
      gap: $spacing-3;
    }
  }

  .header-actions {
    width: 100%;
    flex-direction: column;

    button {
      width: 100%;
      justify-content: center;
    }
  }

  .leaders-preview {
    flex-wrap: wrap;
  }

  .tabs-nav {
    padding: 0 $spacing-4;
    overflow-x: auto;

    &::-webkit-scrollbar {
      display: none;
    }
  }

  .tab-btn {
    white-space: nowrap;
  }

  .tab-content {
    padding: 0 $spacing-4 $spacing-4;
  }

  .tab-toolbar {
    flex-direction: column;
    align-items: stretch;
  }

  .search-box {
    width: 100%;
  }

  .btn-new-discussion {
    width: 100%;
    justify-content: center;
  }

  .discussion-card {
    flex-direction: column;

    .discussion-stats {
      flex-direction: row;
      padding-top: $spacing-3;
      border-top: 1px solid $slate-100;
    }
  }

  .members-grid {
    grid-template-columns: 1fr;
  }
}

// Animations
@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

// ============================================
// REDUCED MOTION SUPPORT
// ============================================
@media (prefers-reduced-motion: reduce) {
  .community-detail-view {
    animation: none !important;
  }

  .discussion-card,
  .member-card {
    animation: none !important;
    opacity: 1;
    transform: none;
  }
}
</style>
